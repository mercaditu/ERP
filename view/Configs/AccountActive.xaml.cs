using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using entity;
using System.ComponentModel;

namespace Cognitivo.Configs
{
    public partial class AccountActive : UserControl, INotifyPropertyChanged
    {
        #region NotifyPropertyChange
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
        #endregion

        List<Class.clsTransferAmount> listOpenAmt = null;

        public db db { get; set; }
        public CollectionViewSource app_accountViewSource { get; set; }
        public bool is_active { get; set; }

        public AccountActive()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (db == null)
            {
                db = new db();
            }

            if (db.app_account.Where(x => x.id_account == CurrentSession.Id_Account).FirstOrDefault() != null)
            {
                ///Gets the Current
                app_account app_account = db.app_account.Where(x => x.id_account == CurrentSession.Id_Account).FirstOrDefault();
                is_active = app_account.is_active;
                RaisePropertyChanged("is_active");

                ///Assign a Session ID for this block of code.
                int id_session = 0;
                if (db.app_account_session.Where(x => x.id_account == app_account.id_account && x.is_active).FirstOrDefault() != null)
                {
                    id_session = db.app_account_session.Where(x => x.id_account == app_account.id_account && x.is_active).FirstOrDefault().id_session;
                }

                var app_account_detailList = 
                    app_account.app_account_detail.Where(x => 
                    x.payment_type.payment_behavior == payment_type.payment_behaviours.Normal && 
                    x.id_company == CurrentSession.Id_Company &&
                    x.id_session == id_session)
                     .GroupBy(ad => new { ad.id_currencyfx, ad.id_payment_type })
                     .Select(s => new
                     {
                         id_currencyfx = s.Max(ad => ad.app_currencyfx.id_currencyfx),
                         id_paymenttype = s.Max(ad => ad.id_payment_type),
                         cur = s.Max(ad => ad.app_currencyfx.app_currency.name),
                         payType = s.Max(ad => ad.payment_type.name),
                         amount = s.Sum(ad => ad.credit) - s.Sum(ad => ad.debit)
                     }).ToList();

                var app_account_detailFinalList = app_account_detailList.GroupBy(ad => new { ad.cur, ad.payType }).Select(s => new
                {
                    id_currencyfx = s.Max(x => x.id_currencyfx),
                    id_paymenttype = s.Max(x => x.id_paymenttype),
                    cur = s.Max(ad => ad.cur),
                    payType = s.Max(ad => ad.payType),
                    amount = s.Sum(ad => ad.amount)
                }).ToList();

                listOpenAmt = new List<Class.clsTransferAmount>();

                if (app_account_detailFinalList.Count > 0)
                {

                    foreach (dynamic item in app_account_detailFinalList)
                    {
                        Class.clsTransferAmount clsTransferAmount = new Class.clsTransferAmount();
                        clsTransferAmount.PaymentTypeName = item.payType;
                        clsTransferAmount.amount = item.amount;
                        clsTransferAmount.Currencyfxname = item.cur;
                        clsTransferAmount.id_payment_type = item.id_paymenttype;
                        clsTransferAmount.id_currencyfx = item.id_currencyfx;
                        listOpenAmt.Add(clsTransferAmount);
                    }

                    foreach (app_currencyfx app_currencyfx in db.app_currencyfx.Where(x => x.is_active).ToList())
                    {
                        if (listOpenAmt.Where(x => x.id_currencyfx == app_currencyfx.id_currencyfx).FirstOrDefault() == null)
                        {
                            foreach (payment_type payment_type in db.payment_type.Where(x => x.payment_behavior == payment_type.payment_behaviours.Normal).ToList())
                            {
                                Class.clsTransferAmount clsTransferAmount = new Class.clsTransferAmount();
                                clsTransferAmount.PaymentTypeName = payment_type.name;
                                clsTransferAmount.amount = 0;
                                clsTransferAmount.Currencyfxname = app_currencyfx.app_currency.name;
                                clsTransferAmount.id_payment_type = payment_type.id_payment_type;
                                clsTransferAmount.id_currencyfx = app_currencyfx.id_currencyfx;
                                listOpenAmt.Add(clsTransferAmount);
                            }
                        }
                        else
                        {
                            foreach (payment_type payment_type in db.payment_type.Where(x => x.payment_behavior == payment_type.payment_behaviours.Normal).ToList())
                            {
                                if (listOpenAmt.Where(x => x.id_payment_type == payment_type.id_payment_type && x.id_currencyfx == app_currencyfx.id_currencyfx).FirstOrDefault() == null)
                                {
                                    Class.clsTransferAmount clsTransferAmount = new Class.clsTransferAmount();
                                    clsTransferAmount.PaymentTypeName = payment_type.name;
                                    clsTransferAmount.amount = 0;
                                    clsTransferAmount.Currencyfxname = app_currencyfx.app_currency.name;
                                    clsTransferAmount.id_payment_type = payment_type.id_payment_type;
                                    clsTransferAmount.id_currencyfx = app_currencyfx.id_currencyfx;
                                    listOpenAmt.Add(clsTransferAmount);
                                    
                                }
                            }
                        }
                    }
                }
                else
                {
                    List<app_currency> app_currencyList = new List<app_currency>();
                    app_currencyList = db.app_currency.Where(x => x.id_company == CurrentSession.Id_Company).ToList();

                    foreach (app_currency app_currency in app_currencyList)
                    {
                        foreach (payment_type payment_type in db.payment_type.Where(x => x.payment_behavior == payment_type.payment_behaviours.Normal && x.id_company == CurrentSession.Id_Company).ToList())
                        {

                            Class.clsTransferAmount clsTransferAmount = new Class.clsTransferAmount();
                            clsTransferAmount.PaymentTypeName = payment_type.name;
                         
                            clsTransferAmount.id_payment_type = payment_type.id_payment_type;

                            clsTransferAmount.amount = app_account.app_account_detail.Where(x => 
                                x.app_currencyfx.id_currency == app_currency.id_currency 
                                && x.id_payment_type == payment_type.id_payment_type)
                                .Sum(x => x.credit - x.debit);

                            clsTransferAmount.Currencyfxname = app_currency.name;
                            clsTransferAmount.id_currencyfx = db.app_currencyfx.Where(x => x.id_currency == app_currency.id_currency && x.is_active).FirstOrDefault().id_currencyfx;
                            listOpenAmt.Add(clsTransferAmount);
                        }
                    }
                }

                CashDataGrid.ItemsSource = listOpenAmt;
            }
        }

        private void btnActivateAccount_Click(object sender, RoutedEventArgs e)
        {
            if (db.app_account.Where(x => x.id_account == CurrentSession.Id_Account).FirstOrDefault() != null)
            {
                //Get the correct Account.
                app_account app_account = db.app_account.Where(x => x.id_account == CurrentSession.Id_Account).FirstOrDefault();

                app_account_session app_account_session = null;

                if (db.app_account_session.Where(x => x.id_account == CurrentSession.Id_Account && x.is_active).FirstOrDefault() != null)
                {
                    app_account_session = db.app_account_session.Where(x => x.id_account == CurrentSession.Id_Account && x.is_active).FirstOrDefault();
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
                        app_account_detail app_account_detail = new global::entity.app_account_detail();
                        app_account_detail.id_session = app_account_session.id_session;
                        app_account_detail.id_account = app_account_session.id_account;
                        app_account_detail.id_currencyfx = counted_account_detail.id_currencyfx;
                        app_account_detail.id_payment_type = counted_account_detail.id_payment_type;
                        app_account_detail.debit = counted_account_detail.amountCounted;
                        app_account_detail.comment = "Closing Balance";
                        app_account_detail.tran_type = app_account_detail.tran_types.Close;
                        app_account_detail.trans_date = DateTime.Now;

                        //CHECK
                        app_account_detail.id_session = app_account_session.id_session;
                        app_account_session.cl_date = DateTime.Now;
                        app_account_session.is_active = false;

                        db.app_account_detail.Add(app_account_detail);
                        //app_account.is_active = false;
                        //Save Changes
                        db.SaveChanges();

                        is_active = app_account_session.is_active;
                        RaisePropertyChanged("is_active");
                    }

                    if (MessageBox.Show("Session is Closed, thank you for using CognitivoERP! "
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
                    app_account_session = new entity.app_account_session();
                    app_account_session.id_account = app_account.id_account;

                    //Loop through each account and create an Account Detail for the Closing Balance.
                    foreach (Class.clsTransferAmount counted_account_detail in listOpenAmt)
                    {
                        app_account_detail app_account_detail = new global::entity.app_account_detail();
                        app_account_detail.id_account = app_account.id_account;
                        app_account_detail.id_currencyfx = counted_account_detail.id_currencyfx;
                        app_account_detail.id_payment_type = counted_account_detail.id_payment_type;
                        app_account_detail.credit = counted_account_detail.amountCounted;
                        app_account_detail.comment = "Opening Balance";
                        app_account_detail.tran_type = app_account_detail.tran_types.Open;
                        app_account_detail.trans_date = DateTime.Now;

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
