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
        public PaymentDB PaymentDBold;

        CollectionViewSource paymentpayment_detailViewSource;
        CollectionViewSource paymentViewSource;
        CollectionViewSource payment_schedualViewSource;

        public PaymentApproval(ref PaymentDB PaymentDB, List<payment_schedual> SchedualList)
        {
            PaymentDBold = PaymentDB;
            InitializeComponent();
          
            //Setting the Mode for this Window. Result of this variable will determine logic of the certain Behaviours.
            payment_schedualViewSource = (CollectionViewSource)this.FindResource("payment_schedualViewSource");
            paymentpayment_detailViewSource = (CollectionViewSource)this.FindResource("paymentpayment_detailViewSource");

            payment_schedualViewSource.Source = PaymentDBold.payment_schedual.Local;
            payment_schedualViewSource.View.MoveCurrentTo(SchedualList.FirstOrDefault());

            foreach (payment_schedual payment_schedual in SchedualList)
            {
                payment_detail payment_detail = new payment_detail();
                payment_detail.value = SchedualList.Sum(x => x.AccountPayableBalance);
                payment_detail.IsSelected = true;
                payment_detail.id_currencyfx = SchedualList.FirstOrDefault().id_currencyfx;
                payment_detail.State = EntityState.Added;
                PaymentDBold.payment_detail.Add(payment_detail);

                payment_detail.payment_schedual.Add(payment_schedual);
            }

            paymentpayment_detailViewSource.Source = PaymentDBold.payment_detail.Local;
            paymentpayment_detailViewSource.View.MoveCurrentToFirst();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource payment_typeViewSource = (CollectionViewSource)this.FindResource("payment_typeViewSource");
            await PaymentDBold.payment_type.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company).LoadAsync();
            payment_typeViewSource.Source = PaymentDBold.payment_type.Local;

            CollectionViewSource app_accountViewSource = (CollectionViewSource)this.FindResource("app_accountViewSource");
            await PaymentDBold.app_account.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company).LoadAsync();
            app_accountViewSource.Source = PaymentDBold.app_account.Local;

            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(PaymentDBold, App.Names.AccountsPayable, CurrentSession.Id_Branch, CurrentSession.Id_Company);

            // paymentViewSource.View.Refresh();
            paymentpayment_detailViewSource.View.Refresh();
        }

        #region Events

        private void lblCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PaymentDBold.CancelAllChanges();

            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }

        public event RoutedEventHandler SaveChanges;
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            payment_schedual payment_schedual = payment_schedualViewSource.View.CurrentItem as payment_schedual;
            if (payment_schedual != null)
            {
                if (payment_schedual.id_range > 0)
                {
                    app_document_range app_document_range = PaymentDBold.app_document_range.Where(x => x.id_range == payment_schedual.id_range).FirstOrDefault();
                    if (app_document_range != null)
                    {
                        payment_schedual.number = entity.Brillo.Logic.Range.calc_Range(app_document_range, true);

                        entity.Brillo.Document.Start.Manual(payment_schedual, app_document_range);
                    }
                }

                payment_schedual.status = Status.Documents_General.Approved;
                payment_schedual.RaisePropertyChanged("status");

                try
                {
                    PaymentDBold.SaveChanges();
                }
                catch (Exception)
                {
                    
                    throw;
                }

                if (SaveChanges != null)
                { SaveChanges(this, new RoutedEventArgs()); }

                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = Visibility.Hidden;
            }
        }
        #endregion
    }
}
