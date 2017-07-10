using entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using entity.BrilloQuery;
using System.Data;

namespace Cognitivo.Configs
{
    public class AccountBalance
    {
        public int CurrencyID { get; set; }
        public int PaymentTypeID { get; set; }
        public decimal Balance { get; set; }
    }

    public partial class AccountActive : UserControl, INotifyPropertyChanged
    {
        #region NotifyPropertyChange

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #endregion NotifyPropertyChange

        private List<Class.clsTransferAmount> listOpenAmt = null;

        public db db { get; set; }
        public CollectionViewSource app_accountViewSource { get; set; }
        public bool is_active { get; set; }
        public int id_account { get; set; }
        public AccountActive(int _id_account)
        {
            InitializeComponent();
            id_account = _id_account;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (db == null)
            {
                db = new db();
            }

            app_account app_account = db.app_account.Find(id_account);

            if (app_account != null)
            {
                //Get the Very Last Session of this Account.
                app_account_session app_account_session = app_account.app_account_session.LastOrDefault();

                ///Assign a Session ID for this block of code.
                int id_session = 0;

                ///Gets the Current
                if (app_account_session != null)
                {
                    is_active = app_account_session.is_active;
                    RaisePropertyChanged("is_active");

                    if (app_account_session.is_active)
                    {
                        id_session = app_account_session.id_session;
                    }
                }
                else
                {
                    is_active = false;
                    RaisePropertyChanged("is_active");
                }

                //Run Query to bring Sum (balance) of Current Session.
                string query = "";
                query = @"
                  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                  set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
                    select 
                        ptype.id_payment_type PaymentTypeID,
                        sum(accd.credit - accd.debit) as Balance, 
                        curr.name as Currency, 
                        curr.id_currency as CurrencyID
                    from app_account_detail as accd
                    join app_currencyfx as fx on accd.id_currencyfx = fx.id_currencyfx
                    join app_currency as curr on fx.id_currency = curr.id_currency
                    join payment_type as ptype on ptype.id_payment_type = accd.id_payment_type
                    join app_account as acc on accd.id_account = acc.id_account
                    join app_account_session as sess on accd.id_session = sess.id_session
                    where sess.is_active = true and acc.id_account ={0}
                    group by sess.id_session, fx.id_currency";
                query = string.Format(query, app_account.id_account);

                DataTable dt = new DataTable();
                dt = QueryExecutor.DT(query);

                List<AccountBalance> AccountBalanceList = new List<AccountBalance>();

                //Load Balance into Class.
                foreach (DataRow item in dt.Rows)
                {
                    AccountBalance AccountBalance = new AccountBalance()
                    {
                        CurrencyID = item["CurrencyID"] != null ? Convert.ToInt32(item["CurrencyID"]) : 0,
                        Balance = item["Balance"] != null ? Convert.ToDecimal(item["Balance"]) : 0,
                        PaymentTypeID = item["PaymentTypeID"] != null ? Convert.ToInt32(item["PaymentTypeID"]) : 0
                    };
                    AccountBalanceList.Add(AccountBalance);
                }

                listOpenAmt = new List<Class.clsTransferAmount>();

                //Check if Class List is Null.
                if (AccountBalanceList.Count() > 0)
                {
                    foreach (app_currency app_currency in CurrentSession.Currencies)
                    {
                        foreach (payment_type payment_type in db.payment_type.Where(x => x.payment_behavior == payment_type.payment_behaviours.Normal && x.id_company == CurrentSession.Id_Company).ToList())
                        {
                            Class.clsTransferAmount clsTransferAmount = new Class.clsTransferAmount();

                            //If this Currency and Payment Type is used in the Balance List. Then show Balance value, else show 0.
                            if (AccountBalanceList
                                        .Where
                                        (
                                        x => x.CurrencyID == app_currency.id_currency &&
                                        x.PaymentTypeID == payment_type.id_payment_type
                                        )
                                .Count() > 0)
                            {
                                //Take Balance value as amount.
                                clsTransferAmount.amount = AccountBalanceList
                                        .Where(
                                        x => x.CurrencyID == app_currency.id_currency &&
                                        x.PaymentTypeID == payment_type.id_payment_type
                                        )
                                        .FirstOrDefault()
                                        .Balance;
                            }
                            else
                            {
                                //No balance, so bring 0 value
                                clsTransferAmount.amount = 0;
                            }

                            clsTransferAmount.id_currencyfxorigin = CurrentSession.CurrencyFX_ActiveRates.Where(x => x.id_currency == app_currency.id_currency).FirstOrDefault().id_currencyfx;
                            clsTransferAmount.amountCounted = 0;
                            clsTransferAmount.Currencyfxnameorigin = app_currency.name;
                            clsTransferAmount.PaymentTypeName = payment_type.name;
                            clsTransferAmount.id_payment_type = payment_type.id_payment_type;
                            listOpenAmt.Add(clsTransferAmount);
                        }
                    }
                }
                else
                {
                    foreach (app_currency app_currency in CurrentSession.Currencies)
                    {
                        foreach (payment_type payment_type in db.payment_type.Where(x => x.payment_behavior == payment_type.payment_behaviours.Normal && x.id_company == CurrentSession.Id_Company).ToList())
                        {
                            Class.clsTransferAmount clsTransferAmount = new Class.clsTransferAmount()
                            {
                                id_currencyfxorigin = CurrentSession.CurrencyFX_ActiveRates.Where(x => x.id_currency == app_currency.id_currency).FirstOrDefault().id_currencyfx,
                                amountCounted = 0,
                                Currencyfxnameorigin = app_currency.name,
                                PaymentTypeName = payment_type.name,
                                id_payment_type = payment_type.id_payment_type
                            };

                            listOpenAmt.Add(clsTransferAmount);
                        }
                    }
                }
                CashDataGrid.ItemsSource = listOpenAmt;
            }
        }

        private void btnActivateAccount_Click(object sender, RoutedEventArgs e)
        {
            if (db.app_account.Where(x => x.id_account == id_account).FirstOrDefault() != null)
            {
                //Get the correct Account.
                app_account app_account = db.app_account.Where(x => x.id_account == id_account).FirstOrDefault();
                app_account_session app_account_session = null;

                if (db.app_account_session.Where(x => x.id_account == id_account && x.is_active).FirstOrDefault() != null)
                {
                    app_account_session = db.app_account_session.Where(x => x.id_account == id_account && x.is_active).FirstOrDefault();
                }

                if (app_account_session != null && app_account_session.is_active)
                {
                    ///We need to CLOSE (InActive) the active Session.
                    ///For this we will need...
                    ///- Create Account Details for each type of Closing Balance.
                    ///- Close Session.
                    ///- Keep Account Active. (Previously we used to close Account, now Session handles that.)

                    //Loop through each account and create an Account Detail for the Opening Balance.
                    foreach (Class.clsTransferAmount counted_account_detail in listOpenAmt)
                    {
                        app_account_detail app_account_detail = new app_account_detail()
                        {
                            id_session = app_account_session.id_session,
                            id_account = app_account_session.id_account,
                            id_currencyfx = counted_account_detail.id_currencyfxorigin,
                            id_payment_type = counted_account_detail.id_payment_type,
                            debit = counted_account_detail.amountCounted,
                            comment = "Closing Balance",
                            tran_type = app_account_detail.tran_types.Close,
                            trans_date = DateTime.Now,
                            app_account_session = app_account_session,
                        };

                        app_account_session.cl_date = DateTime.Now;
                        app_account_session.is_active = false;

                        db.app_account_detail.Add(app_account_detail);
                        //app_account.is_active = false;
                        //Save Changes
                        db.SaveChanges();

                        is_active = app_account_session.is_active;
                        RaisePropertyChanged("is_active");
                    }

                    if (MessageBox.Show("Session is Closed, thank you for using Cognitivo ERP! "
                                   + "/n Would you like to Print the Z-Report?", "Print Z-Report?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        try
                        {
                            entity.Brillo.Logic.Reciept TicketPrint = new entity.Brillo.Logic.Reciept();
                            TicketPrint.ZReport(app_account_session);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: Trying to print Z-Report : " + ex.Message);
                        }
                    }
                }
                else
                {
                    //We need to OPEN (Activate) the inactive Session..

                    //Create New Session.
                    app_account_session = new entity.app_account_session()
                    {
                        id_account = app_account.id_account
                    };

                    //Loop through each account and create an Account Detail for the Closing Balance.
                    foreach (Class.clsTransferAmount counted_account_detail in listOpenAmt)
                    {
                        app_account_detail app_account_detail = new global::entity.app_account_detail();
                        app_account_detail.id_account = app_account.id_account;
                        app_account_detail.id_currencyfx = counted_account_detail.id_currencyfxorigin;
                        app_account_detail.id_payment_type = counted_account_detail.id_payment_type;
                        app_account_detail.credit = counted_account_detail.amountCounted;
                        app_account_detail.comment = "Opening Balance";
                        app_account_detail.tran_type = app_account_detail.tran_types.Open;
                        app_account_detail.trans_date = DateTime.Now;
                        app_account_detail.app_account_session = app_account_session;
                        app_account_session.app_account_detail.Add(app_account_detail);
                        db.app_account_session.Add(app_account_session);
                    }

                    //  app_account.is_active = true;
                    //Save Changes
                    db.SaveChanges();

                    is_active = app_account_session.is_active;
                    RaisePropertyChanged("is_active");
                    MessageBox.Show("Session is Open, Good Luck!");
                }

                if (app_accountViewSource != null)
                {
                    if (app_accountViewSource.View != null)
                    {
                        app_accountViewSource.View.Refresh();
                    }
                }

                //Reload Data
                db.Entry(app_account).Reload();
            }
        }
    }
}