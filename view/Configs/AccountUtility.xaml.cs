using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using entity;

namespace Cognitivo.Configs
{

    public partial class AccountUtility
    {
        #region Load and Initilize
        db db = new db();

        CollectionViewSource app_accountViewSource
            , app_account_listViewSource
            , app_account_detail_adjustViewSource
            , app_accountapp_account_detailViewSource
            , amount_transferViewSource = null;
        List<Class.clsTransferAmount> listTransferAmt = null;
        
        public AccountUtility()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Main Account DataGrid.
            app_accountViewSource = (CollectionViewSource)this.FindResource("app_accountViewSource");
            db.app_account
                .Where(a => a.id_company == CurrentSession.Id_Company).Take(100).Load();
            app_accountViewSource.Source = db.app_account.Local;
            app_accountapp_account_detailViewSource = this.FindResource("app_accountapp_account_detailViewSource") as CollectionViewSource;

            app_account_listViewSource = this.FindResource("app_account_listViewSource") as CollectionViewSource;
            app_account_listViewSource.Source =
                db.app_account.Where(a => a.is_active == true && a.id_account_type == app_account.app_account_type.Terminal && a.id_company == CurrentSession.Id_Company).ToList();

            CollectionViewSource app_accountDestViewSource = this.FindResource("app_accountDestViewSource") as CollectionViewSource;
            app_accountDestViewSource.Source = db.app_account.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).ToList();
            //Payment Type 
            CollectionViewSource payment_typeViewSource = this.FindResource("payment_typeViewSource") as CollectionViewSource;
            payment_typeViewSource.Source = db.payment_type.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).ToList();

            //CurrencyFx
            CollectionViewSource app_currencyfxViewSource = this.FindResource("app_currencyfxViewSource") as CollectionViewSource;
            db.app_currencyfx.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).Load();
            app_currencyfxViewSource.Source = db.app_currencyfx.Local;

            //For Adjust Tab.
            app_account_detail_adjustViewSource = this.FindResource("app_account_detail_adjustViewSource") as CollectionViewSource;
            db.app_account_detail.Where(a => a.id_company == CurrentSession.Id_Company && a.id_company == CurrentSession.Id_Company).Load();
            app_account_detail_adjustViewSource.Source = db.app_account_detail.Local;
            app_account_detail_adjustViewSource.View.Filter = item =>
            {
                app_account_detail objAcDetail = item as app_account_detail;
                if (objAcDetail.id_account_detail == 0)
                    return true;
                else
                    return false;
            };

            //Transfer
            listTransferAmt = new List<Class.clsTransferAmount>();
            amount_transferViewSource = this.FindResource("amount_transferViewSource") as CollectionViewSource;
            amount_transferViewSource.Source = listTransferAmt;
        }

        private void app_accountDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Account detail.
            app_account objAccount = (app_account)app_accountDataGrid.SelectedItem;
            app_account_detailDataGrid.ItemsSource = objAccount.app_account_detail
                .GroupBy(ad => new { ad.id_currencyfx, ad.id_payment_type })
                .Select(s => new
                {
                    cur = s.Max(ad => ad.app_currencyfx.app_currency.name),
                    payType = s.Max(ad => ad.payment_type.name),
                    amount = s.Sum(ad => ad.credit) - s.Sum(ad => ad.debit)
                }).ToList();
            CurrentSession.Id_Account = objAccount.id_account;

            if (frmActive.Children.Count>0)
            {
                frmActive.Children.RemoveAt(0);
            }

            Configs.AccountActive AccountActive = new AccountActive();
            AccountActive.db = db;
            AccountActive.app_accountViewSource = app_accountViewSource;
            frmActive.Children.Add(AccountActive);
        }
        #endregion

        private void btnAdjust_Click(object sender, RoutedEventArgs e)
        {

            db.SaveChanges();

            app_accountViewSource.View.Refresh();
            app_accountapp_account_detailViewSource.View.Refresh();
            app_account_detail_adjustViewSource.View.Refresh();
            MessageBox.Show("Adjustment Completed Successfully!", "Cognitivo ERP", MessageBoxButton.OK, MessageBoxImage.Information);
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
                        if (db.app_account_session.Where(x => x.id_account == idOriginAccount.id_account && x.is_active).FirstOrDefault() != null)
                        {
                            objOriginAcDetail.id_session = db.app_account_session.Where(x => x.id_account == idOriginAccount.id_account && x.is_active).FirstOrDefault().id_session;
                        }

                        objOriginAcDetail.id_account = idOriginAccount.id_account;
                        objOriginAcDetail.id_currencyfx = TransferAmount.id_currencyfx;
                        objOriginAcDetail.id_payment_type = TransferAmount.id_payment_type;
                        objOriginAcDetail.credit = 0;
                        objOriginAcDetail.debit = TransferAmount.amount;
                        objOriginAcDetail.comment = "Transfered to " + idDestiAccount.name + ".";
                        objOriginAcDetail.trans_date = DateTime.Now;

                        app_account_detail objDestinationAcDetail = new app_account_detail();
                        if (db.app_account_session.Where(x => x.id_account == idDestiAccount.id_account && x.is_active).FirstOrDefault() != null)
                        {
                            objDestinationAcDetail.id_session = db.app_account_session.Where(x => x.id_account == idDestiAccount.id_account && x.is_active).FirstOrDefault().id_session;
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
                            objOriginAcDetail.status = entity.Status.Documents_General.Approved;
                            objDestinationAcDetail.status = entity.Status.Documents_General.Approved;
                        }
                        else
                        {
                            objOriginAcDetail.status = entity.Status.Documents_General.Pending;
                            objDestinationAcDetail.status = entity.Status.Documents_General.Pending;
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
                MessageBox.Show("Transfer Completed Successfully!", "Cognitivo ERP", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    else
                    {
                        return false;
                    }
                };
            }
            else
            {
                app_accountViewSource.View.Filter = null;
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabAccount.SelectedIndex==2)
            {
                app_account_listViewSource.Source = db.app_account.Where(a => a.is_active == true && a.id_account_type == app_account.app_account_type.Terminal).ToList();
                app_account_listViewSource.View.Refresh();
            }
        }
    }
}
