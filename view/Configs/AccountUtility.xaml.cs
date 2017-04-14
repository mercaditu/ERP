using entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Configs
{
    public partial class AccountUtility : INotifyPropertyChanged
    {
        #region Load and Initilize

        private db db = new db();

        #region NotifyPropertyChange

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #endregion NotifyPropertyChange

        private CollectionViewSource app_accountViewSource
            , app_account_listViewSource
            , app_account_detail_adjustViewSource
            , app_accountapp_account_detailViewSource
            , amount_transferViewSource = null;

        private List<Class.clsTransferAmount> listTransferAmt = null;

        public bool IsActive
        {
            get { return _IsActive; }
            set { _IsActive = value; RaisePropertyChanged("IsActive"); }
        }

        private bool _IsActive;

        public DateTime LastUsed
        {
            get { return _LastUsed; }
            set { _LastUsed = value; RaisePropertyChanged("LastUsed"); }
        }

        private DateTime _LastUsed;

        public AccountUtility()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await db.app_account.Where(a => a.id_company == CurrentSession.Id_Company && a.is_active).LoadAsync();

            //Main Account DataGrid.
            app_accountViewSource = FindResource("app_accountViewSource") as CollectionViewSource;
            app_accountViewSource.Source = db.app_account.Local;
            app_accountapp_account_detailViewSource = this.FindResource("app_accountapp_account_detailViewSource") as CollectionViewSource;

            app_account_listViewSource = this.FindResource("app_account_listViewSource") as CollectionViewSource;
            app_account_listViewSource.Source = db.app_account.Local.Where(a => a.id_account_type == app_account.app_account_type.Terminal).ToList();

            CollectionViewSource app_accountDestViewSource = this.FindResource("app_accountDestViewSource") as CollectionViewSource;
            app_accountDestViewSource.Source = db.app_account.Local;

            //Payment Type
            CollectionViewSource payment_typeViewSource = this.FindResource("payment_typeViewSource") as CollectionViewSource;
            payment_typeViewSource.Source = db.payment_type.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company).ToList();

            //CurrencyFx
            CollectionViewSource app_currencyfxViewSource = this.FindResource("app_currencyfxViewSource") as CollectionViewSource;
            await db.app_currencyfx.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company).Include(x => x.app_currency).LoadAsync();
            app_currencyfxViewSource.Source = db.app_currencyfx.Local;

            //For Adjust Tab.
            app_account_detail_adjustViewSource = this.FindResource("app_account_detail_adjustViewSource") as CollectionViewSource;
            //await db.app_account_detail.Where(a => a.id_company == CurrentSession.Id_Company).LoadAsync();
            app_account_detail_adjustViewSource.Source = db.app_account_detail.Local;
            //app_account_detail_adjustViewSource.View.Filter = item =>
            //{
            //    app_account_detail objAcDetail = item as app_account_detail;
            //    if (objAcDetail.id_account_detail == 0)
            //        return true;

            //    return false;
            //};

            //Transfer
            listTransferAmt = new List<Class.clsTransferAmount>();
            amount_transferViewSource = this.FindResource("amount_transferViewSource") as CollectionViewSource;
            amount_transferViewSource.Source = listTransferAmt;
        }

        private void app_accountDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Account detail.
            app_account app_account = app_accountDataGrid.SelectedItem as app_account;

            if (app_account != null)
            {
                //Get the Very Last Session of this Account.
                app_account_session app_account_session = app_account.app_account_session.LastOrDefault();
                int SessionID = 0;

                ///Gets the Current
                if (app_account_session != null)
                {
                    IsActive = app_account_session.is_active;
                    LastUsed = app_account_session.app_account_detail.Select(x => x.trans_date).LastOrDefault();

                    //Sets the SessionID.
                    if (app_account_session.is_active)
                    {
                        SessionID = app_account.app_account_session.Where(y => y.is_active).Select(x => x.id_session).FirstOrDefault();
                    }
                }
                else
                {
                    IsActive = false;
                }

                app_account_detailDataGrid.ItemsSource = app_account.app_account_detail
                    .Where(x => x.id_session == SessionID)
                    .GroupBy(ad => new { ad.app_currencyfx.id_currency, ad.id_payment_type })
                    .Select(s => new
                    {
                        cur = s.Max(ad => ad.app_currencyfx.app_currency.name),
                        payType = s.Max(ad => ad.payment_type.name),
                        amount = s.Sum(ad => ad.credit) - s.Sum(ad => ad.debit)
                    }).ToList();

                CurrentSession.Id_Account = app_account.id_account;

                if (frmActive.Children.Count > 0)
                {
                    frmActive.Children.RemoveAt(0);
                }

                AccountActive AccountActive = new AccountActive();
                AccountActive.db = db;
                AccountActive.app_accountViewSource = app_accountViewSource;
                frmActive.Children.Add(AccountActive);
            }
        }

        #endregion Load and Initilize

        private void btnAdjust_Click(object sender, RoutedEventArgs e)
        {
            db.SaveChanges();

            app_accountViewSource.View.Refresh();
            app_accountapp_account_detailViewSource.View.Refresh();
            app_account_detail_adjustViewSource.View.Refresh();
            toolBar.msgSaved(1);
        }

        private void btnTransfer_Click(object sender, RoutedEventArgs e)
        {
            if (cbxAccountDestination.SelectedItem != null)
            {
                app_account idOriginAccount = ((app_accountViewSource.View.CurrentItem) as app_account); //Credit Account
                app_account idDestiAccount = cbxAccountDestination.SelectedItem as app_account; //Debit Account

                if (idOriginAccount.id_account == idDestiAccount.id_account)
                {
                    MessageBox.Show("Please select a different Destination", "Cognitivo ERP", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                foreach (Class.clsTransferAmount TransferAmount in listTransferAmt)
                {
                    payment_type payment_type = db.payment_type.Where(x => x.id_payment_type == TransferAmount.id_payment_type).FirstOrDefault();

                    if (idOriginAccount != null && idDestiAccount != null && payment_type != null)
                    {
                        app_account_detail objOriginAcDetail = new app_account_detail();
                        if (db.app_account_session.Where(x => x.id_account == idOriginAccount.id_account && x.is_active).Any())
                        {
                            objOriginAcDetail.id_session = db.app_account_session.Where(x => x.id_account == idOriginAccount.id_account && x.is_active).Select(y => y.id_session).FirstOrDefault();
                        }

                        objOriginAcDetail.id_account = idOriginAccount.id_account;
                        objOriginAcDetail.id_currencyfx = TransferAmount.id_currencyfx;
                        objOriginAcDetail.id_payment_type = TransferAmount.id_payment_type;
                        objOriginAcDetail.credit = 0;
                        objOriginAcDetail.debit = TransferAmount.amount;
                        objOriginAcDetail.comment = "Transfered to " + idDestiAccount.name + ".";
                        objOriginAcDetail.trans_date = DateTime.Now;

                        app_account_detail objDestinationAcDetail = new app_account_detail();
                        if (db.app_account_session.Where(x => x.id_account == idDestiAccount.id_account && x.is_active).Any())
                        {
                            objDestinationAcDetail.id_session = db.app_account_session.Where(x => x.id_account == idDestiAccount.id_account && x.is_active).Select(y => y.id_session).FirstOrDefault();
                        }

                        objDestinationAcDetail.id_account = idDestiAccount.id_account;
                        objDestinationAcDetail.id_currencyfx = TransferAmount.id_currencyfx;
                        objDestinationAcDetail.id_payment_type = TransferAmount.id_payment_type;
                        objDestinationAcDetail.credit = TransferAmount.amount;
                        objDestinationAcDetail.debit = 0;
                        objDestinationAcDetail.comment = "Transfered from " + idOriginAccount.name + ".";
                        objDestinationAcDetail.trans_date = DateTime.Now;

                        bool is_direct = payment_type.is_direct;
                        if (is_direct)
                        {
                            objOriginAcDetail.status = Status.Documents_General.Approved;
                            objDestinationAcDetail.status = Status.Documents_General.Approved;
                        }
                        else
                        {
                            objOriginAcDetail.status = Status.Documents_General.Pending;
                            objDestinationAcDetail.status = Status.Documents_General.Pending;
                        }

                        db.app_account_detail.Add(objOriginAcDetail);
                        db.app_account_detail.Add(objDestinationAcDetail);
                        db.SaveChanges();

                        //Reload Data.
                        cbxAccountDestination.SelectedIndex = 0;
                    }
                }

                listTransferAmt.Clear();
                amount_transferViewSource.View.Refresh();
                app_accountViewSource.View.Refresh();
                app_accountapp_account_detailViewSource.View.Refresh();
                app_account_detail_adjustViewSource.View.Refresh();
                toolBar.msgSaved(1);
            }
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                app_accountViewSource.View.Filter = i =>
                {
                    app_account app_account = i as app_account;
                    if (app_account.name.ToLower().Contains(query.ToLower()))
                    {
                        return true;
                    }

                    return false;
                };
            }
            else
            {
                app_accountViewSource.View.Filter = null;
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabAccount.SelectedIndex == 2)
            {
                app_account_listViewSource.Source = db.app_account.Where(a => a.is_active == true && a.id_account_type == app_account.app_account_type.Terminal && a.id_company == CurrentSession.Id_Company).ToList();
                app_account_listViewSource.View.Refresh();
            }
            if (app_accountViewSource != null)
            {
                app_accountViewSource.View.Refresh();
            }
            if (app_accountapp_account_detailViewSource != null)
            {
                app_accountapp_account_detailViewSource.View.Refresh();
            }
        }
    }
}