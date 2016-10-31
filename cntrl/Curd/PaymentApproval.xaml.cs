using System;
using System.Collections.Generic;
using System.Windows;
using System.Data.Entity;
using entity;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;

namespace cntrl.Curd
{
    public partial class PaymentApproval : UserControl
    {
        public PaymentDB PaymentDB_Local;

        CollectionViewSource paymentpayment_detailViewSource;
        CollectionViewSource payment_schedualViewSource;

        public PaymentApproval(ref PaymentDB PaymentDB, List<payment_schedual> SchedualList)
        {
            PaymentDB_Local = PaymentDB;
            InitializeComponent();
          
            //Setting the Mode for this Window. Result of this variable will determine logic of the certain Behaviours.
            payment_schedualViewSource = (CollectionViewSource)this.FindResource("payment_schedualViewSource");
            paymentpayment_detailViewSource = (CollectionViewSource)this.FindResource("paymentpayment_detailViewSource");

            payment_schedualViewSource.Source = PaymentDB_Local.payment_schedual.Local;
            payment_schedualViewSource.View.MoveCurrentTo(SchedualList.FirstOrDefault());

            foreach (payment_schedual payment_schedual in SchedualList)
            {
                payment_detail payment_detail = new payment_detail();
                payment_detail.value = payment_schedual.credit; //SchedualList.Sum(x => x.AccountPayableBalance);
                payment_detail.IsSelected = true;
                payment_detail.id_account = CurrentSession.Id_Account > 0 ? CurrentSession.Id_Account : PaymentDB_Local.app_account.Where(x => x.id_company == CurrentSession.Id_Company).FirstOrDefault().id_account;
                payment_detail.id_currencyfx = payment_schedual.id_currencyfx;
                payment_detail.app_currencyfx = payment_schedual.app_currencyfx;
                payment_detail.State = EntityState.Added;
                PaymentDB_Local.payment_detail.Add(payment_detail);

                payment_detail.payment_schedual.Add(payment_schedual);
            }

            paymentpayment_detailViewSource.Source = PaymentDB_Local.payment_detail.Local;
            paymentpayment_detailViewSource.View.MoveCurrentToFirst();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource payment_typeViewSource = (CollectionViewSource)this.FindResource("payment_typeViewSource");
            await PaymentDB_Local.payment_type.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company).LoadAsync();
            payment_typeViewSource.Source = PaymentDB_Local.payment_type.Local;

            CollectionViewSource app_accountViewSource = (CollectionViewSource)this.FindResource("app_accountViewSource");
            await PaymentDB_Local.app_account.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company).LoadAsync();
            app_accountViewSource.Source = PaymentDB_Local.app_account.Local;

            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(PaymentDB_Local, App.Names.AccountsPayable, CurrentSession.Id_Branch, CurrentSession.Id_Company);

            paymentpayment_detailViewSource.View.Refresh();
        }

        #region Events

        private void lblCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PaymentDB_Local.CancelAllChanges();

            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }

        public event RoutedEventHandler SaveChanges;
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // payment_schedualViewSource.Source = PaymentDB_Local.payment_schedual.Local;

            string OrderNumber = "";

            List<payment_detail> DetailList = PaymentDB_Local.payment_detail.Where(x => x.payment_schedual.FirstOrDefault() != null).Include(y => y.payment_schedual).ToList();

            int RangeID = 0;

            foreach (payment_detail Detail in PaymentDB_Local.payment_detail.Local)
            {
                foreach (payment_schedual Schedual in Detail.payment_schedual)
                {
                    if (Schedual.id_range > 0)
                    {
                        RangeID = (int)Schedual.id_range;
                    }
                    else
                    {
                        Schedual.id_range = RangeID;
                    }
                }
            }

            app_document_range app_document_range = null;

            if (RangeID > 0)
            {
                app_document_range = await PaymentDB_Local.app_document_range.FindAsync(RangeID);

                if (app_document_range != null)
                {
                    entity.Brillo.Logic.Range.branch_Code = CurrentSession.Branches.Where(x => x.id_branch == CurrentSession.Id_Branch).FirstOrDefault().code;
                    entity.Brillo.Logic.Range.terminal_Code = CurrentSession.Terminals.Where(x => x.id_terminal == CurrentSession.Id_Terminal).FirstOrDefault().code;
                    OrderNumber = entity.Brillo.Logic.Range.calc_Range(app_document_range, true);
                }
            }

            List<payment_schedual> SchedualList = new List<payment_schedual>();

            foreach (payment_detail Detail in PaymentDB_Local.payment_detail.Local)
            {
                foreach (payment_schedual Schedual in Detail.payment_schedual)
                {
                    SchedualList.Add(Schedual);
                    Schedual.number = OrderNumber;
                    Schedual.status = Status.Documents_General.Approved;
                    Schedual.RaisePropertyChanged("status");
                }
            }

            if (app_document_range != null)
            {
                entity.Brillo.Document.Start.Manual(SchedualList, app_document_range);
            }

            try
            {
                PaymentDB_Local.SaveChanges();
            }
            catch (Exception) { throw; }

            SaveChanges?.Invoke(this, new RoutedEventArgs());
            Grid parentGrid = (Grid)Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }
        #endregion
    }
}
