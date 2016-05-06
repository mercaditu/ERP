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
        entity.dbContext entity = new entity.dbContext();
        CollectionViewSource app_accountViewSource
            , app_account_listViewSource
            , app_account_detail_adjustViewSource
            , app_accountapp_account_detailViewSource
            , amount_transferViewSource = null;
        List<Class.clsTransferAmount> listTransferAmt = null;
        entity.Properties.Settings _entity = new entity.Properties.Settings();
        List<Class.clsTransferAmount> listOpenAmt = null;
        
        public AccountUtility()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Main Account DataGrid.
            app_accountViewSource = (CollectionViewSource)this.FindResource("app_accountViewSource");
            entity.db.app_account.Include("app_account_detail")
                .Where(a => a.id_company == _entity.company_ID).Load();
            app_accountViewSource.Source = entity.db.app_account.Local;
            app_accountapp_account_detailViewSource = this.FindResource("app_accountapp_account_detailViewSource") as CollectionViewSource;

            app_account_listViewSource = this.FindResource("app_account_listViewSource") as CollectionViewSource;
            app_account_listViewSource.Source =
                entity.db.app_account.Where(a => a.is_active == true && a.id_account_type == app_account.app_account_type.Terminal && a.id_company == _entity.company_ID).ToList();

           // getInitialAmount();

            CollectionViewSource app_accountDestViewSource = this.FindResource("app_accountDestViewSource") as CollectionViewSource;
            app_accountDestViewSource.Source = entity.db.app_account.Where(a => a.is_active == true && a.id_company == _entity.company_ID).ToList();
            //Payment Type 
            CollectionViewSource payment_typeViewSource = this.FindResource("payment_typeViewSource") as CollectionViewSource;
            payment_typeViewSource.Source = entity.db.payment_type.Where(a => a.is_active == true).ToList();

            //CurrencyFx
            CollectionViewSource app_currencyfxViewSource = this.FindResource("app_currencyfxViewSource") as CollectionViewSource;
            entity.db.app_currencyfx.Include("app_currency").Where(a => a.is_active == true).Load();
            app_currencyfxViewSource.Source = entity.db.app_currencyfx.Local;

            //For Adjust Tab.
            app_account_detail_adjustViewSource = this.FindResource("app_account_detail_adjustViewSource") as CollectionViewSource;
            entity.db.app_account_detail.Where(a => a.id_company == _entity.company_ID).Load();
            app_account_detail_adjustViewSource.Source = entity.db.app_account_detail.Local;
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

        //private void getInitialAmount()
        //{
        //    app_account objAccount = (app_account)app_accountDataGrid.SelectedItem;
        //    var app_account_detailList = objAccount.app_account_detail
        // .GroupBy(ad => new { ad.id_currencyfx })
        // .Select(s => new
        // {
        //     id_currencyfx = s.Max(ad => ad.app_currencyfx.id_currencyfx),
        //     id_paymenttype = s.Max(ad => ad.id_payment_type),
        //     cur = s.Max(ad => ad.app_currencyfx.app_currency.name),
        //     payType = s.Max(ad => ad.payment_type.name),
        //     amount = s.Sum(ad => ad.credit) - s.Sum(ad => ad.debit)
        // }).ToList();

        //    var app_account_detailFinalList = app_account_detailList.GroupBy(x => x.cur).Select(s => new
        //    {
        //        id_currencyfx=s.Max(x=>x.id_currencyfx),
        //        id_paymenttype = s.Max(x => x.id_paymenttype),
        //        cur = s.Max(ad => ad.cur),
        //        payType = s.Max(ad => ad.payType),
        //        amount = s.Sum(ad => ad.amount)
        //    }).ToList();
        //    listOpenAmt = new List<Class.clsTransferAmount>();
        //    foreach (dynamic item in app_account_detailFinalList)
        //    {
        //        Class.clsTransferAmount clsTransferAmount = new Class.clsTransferAmount();
        //        clsTransferAmount.PaymentTypeName = item.payType;
        //        clsTransferAmount.amount = item.amount;
        //        clsTransferAmount.Currencyfxname = item.cur;
        //        clsTransferAmount.id_payment_type = item.id_paymenttype;
        //        clsTransferAmount.id_currencyfx = item.id_currencyfx;
        //        listOpenAmt.Add(clsTransferAmount);
        //    }
        //    CashDataGrid.ItemsSource = listOpenAmt;
        //}

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
            AccountActive.db = entity.db;
            AccountActive.app_accountViewSource = app_accountViewSource;
            frmActive.Children.Add(AccountActive);
        }
        #endregion

        //private void btnActivateAccount_Click(object sender, RoutedEventArgs e)
        //{
        //    if (app_accountDataGrid.SelectedItem != null)
        //    {
        //        app_account app_account = app_accountDataGrid.SelectedItem as app_account;
        //        foreach (Class.clsTransferAmount list in listOpenAmt)
        //        {
        //            app_account_detail app_account_detail = new global::entity.app_account_detail();
                  
        //            if (app_account.is_active == true)
        //            {
        //                //Make Inactive
        //                app_account_detail.debit = list.amountCounted;
        //            }
        //            else
        //            {
        //                //Make Active
        //                app_account_detail.credit = list.amountCounted;
        //            }

        //            app_account_detail.id_account = app_account.id_account;
        //            app_account_detail.id_currencyfx = list.id_currencyfx;
        //            app_account_detail.id_payment_type = list.id_payment_type;
        //            app_account_detail.comment = "For Opening or closing Cash.";
        //            app_account_detail.trans_date = DateTime.Now;
        //            entity.db.app_account_detail.Add(app_account_detail);
        //        }

        //        if (app_account.is_active == true)
        //        {
        //            //Make Inactive
        //            entity.db.Entry(app_account).Entity.is_active = false;
        //        }
        //        else
        //        {
        //            //Make Active
        //            entity.db.Entry(app_account).Entity.is_active = true;
        //        }
        //        // entity.db.Entry(app_account).Entity.initial_amount = Convert.ToDecimal(txtInitialAmount.Text.Trim());
        //        entity.db.Entry(app_account).State = EntityState.Modified;

        //        entity.SaveChanges();

        //        //Reload Data
        //        entity.db.Entry(app_account).Reload();
        //        app_accountViewSource.View.Refresh();
        //        app_account_listViewSource.Source = entity.db.app_account.Where(a => a.is_active == true && a.id_account_type == app_account.app_account_type.Terminal).ToList();
        //        app_account_listViewSource.View.Refresh();
        //    }
        //}

        private void btnAdjust_Click(object sender, RoutedEventArgs e)
        {
            entity.SaveChanges();

            app_accountViewSource.View.Refresh();
            app_accountapp_account_detailViewSource.View.Refresh();
            app_account_detail_adjustViewSource.View.Refresh();
            MessageBox.Show("Adjustment Completed Successfully!", "Cognitivo ERP", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnTransfer_Click(object sender, RoutedEventArgs e)
        {
            if (cbxAccountDestination.SelectedItem != null)
            {
                if (listTransferAmt.Count >= 1)
                {
                    app_account idOriginAccount = ((app_accountViewSource.View.CurrentItem) as app_account); //Credit Account
                    app_account idDestiAccount = (app_account)cbxAccountDestination.SelectedItem; //Debit Account
                    if (idOriginAccount != null && idDestiAccount != null)
                    {
                        app_account_detail objOriginAcDetail = new app_account_detail();
                        if (entity.db.app_account_session.Where(x => x.id_account == idOriginAccount.id_account && x.is_active).FirstOrDefault() != null)
                        {
                            objOriginAcDetail.id_session = entity.db.app_account_session.Where(x => x.id_account == idOriginAccount.id_account && x.is_active).FirstOrDefault().id_session;
                        }
                        objOriginAcDetail.id_account = idOriginAccount.id_account;
                        objOriginAcDetail.id_currencyfx = listTransferAmt[0].id_currencyfx;
                        objOriginAcDetail.id_payment_type = listTransferAmt[0].id_payment_type;
                        objOriginAcDetail.credit = 0;
                        objOriginAcDetail.debit = listTransferAmt[0].amount;
                        objOriginAcDetail.comment = "Amount Transfer from " + idOriginAccount.name + " to " + idDestiAccount.name + ".";
                        objOriginAcDetail.trans_date = DateTime.Now;

                        app_account_detail objDestinationAcDetail = new app_account_detail();
                        if (entity.db.app_account_session.Where(x => x.id_account == idDestiAccount.id_account && x.is_active).FirstOrDefault() != null)
                        {
                            objDestinationAcDetail.id_session = entity.db.app_account_session.Where(x => x.id_account == idDestiAccount.id_account && x.is_active).FirstOrDefault().id_session;
                        }
                        objDestinationAcDetail.id_account = idDestiAccount.id_account;
                        objDestinationAcDetail.id_currencyfx = listTransferAmt[0].id_currencyfx;
                        objDestinationAcDetail.id_payment_type = listTransferAmt[0].id_payment_type;
                        objDestinationAcDetail.credit = listTransferAmt[0].amount;
                        objDestinationAcDetail.debit = 0;
                        objDestinationAcDetail.comment = "Amount Transfer from " + idOriginAccount.name + " to " + idDestiAccount.name + ".";
                        objDestinationAcDetail.trans_date = DateTime.Now;

                        entity.db.Entry(objOriginAcDetail).State = EntityState.Added;
                        entity.db.Entry(objDestinationAcDetail).State = EntityState.Added;
                        entity.SaveChanges();

                        //Reload Data.
                        cbxAccountDestination.SelectedIndex = 0;
                       
                        listTransferAmt.Clear();
                        amount_transferViewSource.View.Refresh();
                        app_accountViewSource.View.Refresh();
                        app_accountapp_account_detailViewSource.View.Refresh();
                        //app_account_detailViewSource.View.Refresh();
                        app_account_detail_adjustViewSource.View.Refresh();
                        MessageBox.Show("Transfer Completed Successfully!", "Cognitivo ERP", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
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
                app_account_listViewSource.Source = entity.db.app_account.Where(a => a.is_active == true && a.id_account_type == app_account.app_account_type.Terminal).ToList();
                app_account_listViewSource.View.Refresh();
            }
            //if (tabAccount.SelectedIndex==1)
            //{
            //    Configs.AccountActive AccountActive = new AccountActive();
            //    AccountActive.app_accountViewSource = app_accountViewSource;
            //    frmActive.Children.Add(AccountActive);
            //}
        }
    }

}
