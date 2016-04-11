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

        public AccountUtility()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Main Account DataGrid.
            app_accountViewSource = (CollectionViewSource)this.FindResource("app_accountViewSource");
            entity.db.app_account.Include("app_account_detail")
                .Where(a =>a.id_company == _entity.company_ID).Load();
            app_accountViewSource.Source = entity.db.app_account.Local;
            app_accountapp_account_detailViewSource = this.FindResource("app_accountapp_account_detailViewSource") as CollectionViewSource;

            app_account_listViewSource = this.FindResource("app_account_listViewSource") as CollectionViewSource;
            app_account_listViewSource.Source =
                entity.db.app_account.Where(a => a.is_active == true && a.id_account_type == app_account.app_account_type.Terminal && a.id_company == _entity.company_ID).ToList();

            //For Active Tab.
            txtInitialAmount.Text = getInitialAmount().ToString();

            //Terminal
            List<app_terminal> listAppTerminal = entity.db.app_terminal.Where(a => a.is_active == true).ToList();
            CollectionViewSource app_terminalViewSource = (CollectionViewSource)this.FindResource("app_terminalViewSource");
            app_terminalViewSource.Source = listAppTerminal;
            CollectionViewSource app_terminalViewSource2 = (CollectionViewSource)this.FindResource("app_terminalViewSource2");
            app_terminalViewSource2.Source = listAppTerminal;

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
        private decimal getInitialAmount()
        {
            decimal debit = 0;
            decimal credit = 0;
            decimal initial_amount = 0;
            if (app_accountDataGrid.SelectedItem != null)
            {
                app_account app_account = app_accountDataGrid.SelectedItem as app_account;
                if (app_account != null)
                {
                    initial_amount = Convert.ToDecimal(app_account.initial_amount);
                    if (app_account.app_account_detail != null)
                    {
                        debit = Convert.ToDecimal(app_account.app_account_detail.Sum(a => a.debit));
                        credit = Convert.ToDecimal(app_account.app_account_detail.Sum(a => a.credit));
                    }
                }
            }
            return ((initial_amount + credit) - debit);
        }

        private void app_accountDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Account detail.
            app_account objAccount = (app_account)app_accountDataGrid.SelectedItem;
            app_account_detailDataGrid.ItemsSource = objAccount.app_account_detail
                .GroupBy(ad => new { ad.id_currencyfx, cur_name = ad.app_currencyfx.app_currency.name, paymentType = ad.payment_type.name })
                .Select(s => new
                {
                    cur = s.Key.cur_name,
                    payType = s.Key.paymentType,
                    amount = s.Sum(ad => ad.credit) - s.Sum(ad => ad.debit)
                }).ToList();
        }
        #endregion

        private void btnActivateAccount_Click(object sender, RoutedEventArgs e)
        {
            if (app_accountDataGrid.SelectedItem != null)
            {
                app_account app_account = app_accountDataGrid.SelectedItem as app_account;
                if (app_account.is_active == true)
                {
                    //Make Inactive
                    entity.db.Entry(app_account).Entity.is_active = false;
                }
                else
                {
                    //Make Active
                    entity.db.Entry(app_account).Entity.is_active = true;
                }
                entity.db.Entry(app_account).Entity.initial_amount = Convert.ToDecimal(txtInitialAmount.Text.Trim());
                entity.db.Entry(app_account).State = EntityState.Modified;
                entity.SaveChanges();

                //Reload Data
                entity.db.Entry(app_account).Reload();
                app_accountViewSource.View.Refresh();
                app_account_listViewSource.Source = entity.db.app_account.Where(a => a.is_active == true && a.id_account_type == app_account.app_account_type.Terminal).ToList();
                app_account_listViewSource.View.Refresh();
            }
        }

        private void btnAdjust_Click(object sender, RoutedEventArgs e)
        {
            entity.SaveChanges();

            //Reload Data
            app_accountViewSource.View.Refresh();
            app_accountapp_account_detailViewSource.View.Refresh();
            //app_account_detailViewSource.View.Refresh();
            app_account_detail_adjustViewSource.View.Refresh();
            MessageBox.Show("Adjustment Completed Successfully!", "Adjustment", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnTransfer_Click(object sender, RoutedEventArgs e)
        {
            if (cbxTerminalOrigin.SelectedItem != null && cbxTerminalDestination.SelectedItem != null)
            {
                if (listTransferAmt.Count >= 1)
                {
                    short originTerminalId = Convert.ToInt16(cbxTerminalOrigin.SelectedValue);
                    short destinationTerminalId = Convert.ToInt16(cbxTerminalDestination.SelectedValue);
                    string strOriginTerminal = Convert.ToString(cbxTerminalOrigin.Text);
                    string strDestinationTerminal = Convert.ToString(cbxTerminalDestination.Text);

                    int idOriginAccount = entity.db.app_account.Where(a => a.id_terminal == originTerminalId).FirstOrDefault().id_account; //Credit Account
                    int idDestiAccount = entity.db.app_account.Where(a => a.id_terminal == destinationTerminalId).FirstOrDefault().id_account; //Debit Account
                    if (idOriginAccount > 0 && idDestiAccount > 0)
                    {
                        app_account_detail objOriginAcDetail = new app_account_detail();
                        //objOriginAcDetail.id_company = 1;
                        objOriginAcDetail.id_account = idOriginAccount;
                        objOriginAcDetail.id_currencyfx = listTransferAmt[0].id_currencyfx;
                        objOriginAcDetail.id_payment_type = listTransferAmt[0].id_payment_type;
                        objOriginAcDetail.debit = 0;
                        objOriginAcDetail.credit = listTransferAmt[0].amount;
                        objOriginAcDetail.comment = "Amount Transfer from " + strOriginTerminal + " to " + strDestinationTerminal + ".";
                        objOriginAcDetail.trans_date = DateTime.Now;

                        app_account_detail objDestinationAcDetail = new app_account_detail();
                        //objDestinationAcDetail.id_company = 1;
                        objDestinationAcDetail.id_account = idDestiAccount;
                        objDestinationAcDetail.id_currencyfx = listTransferAmt[0].id_currencyfx;
                        objDestinationAcDetail.id_payment_type = listTransferAmt[0].id_payment_type;
                        objDestinationAcDetail.debit = listTransferAmt[0].amount;
                        objDestinationAcDetail.credit = 0;
                        objDestinationAcDetail.comment = "Amount Transfer from " + strOriginTerminal + " to " + strDestinationTerminal + ".";
                        objDestinationAcDetail.trans_date = DateTime.Now;

                        entity.db.Entry(objOriginAcDetail).State = EntityState.Added;
                        entity.db.Entry(objDestinationAcDetail).State = EntityState.Added;
                        entity.SaveChanges();

                        //Reload Data.
                        cbxTerminalDestination.SelectedIndex = 0;
                        cbxTerminalOrigin.SelectedIndex = 0;
                        listTransferAmt.Clear();
                        amount_transferViewSource.View.Refresh();
                        app_accountViewSource.View.Refresh();
                        app_accountapp_account_detailViewSource.View.Refresh();
                        //app_account_detailViewSource.View.Refresh();
                        app_account_detail_adjustViewSource.View.Refresh();
                        MessageBox.Show("Transfer Completed Successfully!", "Transfer", MessageBoxButton.OK, MessageBoxImage.Information);
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
    }
}
