using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using entity;
using System.Data.Entity;
using System.ComponentModel;
namespace Cognitivo.Configs
{
    /// <summary>
    /// Interaction logic for AccountActive.xaml
    /// </summary>
    public partial class AccountActive : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
        List<Class.clsTransferAmount> listOpenAmt = null;
        public db db { get; set; }
        public CollectionViewSource app_accountViewSource { get; set; }
        public Boolean is_active { get; set; }
        public AccountActive()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            getInitialAmount();
        }
        private void getInitialAmount()
        {
            if (db == null)
            {
                db = new db();
            }

            if (db.app_account.Where(x => x.id_account == CurrentSession.Id_Account).FirstOrDefault() != null)
            {
                app_account objAccount = db.app_account.Where(x => x.id_account == CurrentSession.Id_Account).FirstOrDefault();

                is_active = objAccount.is_active;
                RaisePropertyChanged("is_active");
                var app_account_detailList = objAccount.app_account_detail.Where(x => x.payment_type.is_direct)
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

                }
                else
                {

                    string paymenttypename = "";
                    int id_paymentType = 0;
                    string curname = "";
                    int id_currencyfx = 0;
                    List<app_currency> app_currencyList = new List<app_currency>();
                    app_currencyList = db.app_currency.ToList();
                    foreach (app_currency app_currency in app_currencyList)
                    {
                        Class.clsTransferAmount clsTransferAmount = new Class.clsTransferAmount();
                        if (db.payment_type.Where(x => x.is_default).FirstOrDefault() != null)
                        {
                            payment_type payment_type = db.payment_type.Where(x => x.is_default).FirstOrDefault();
                            paymenttypename = payment_type.name;
                            id_paymentType = payment_type.id_payment_type;
                        }

                        clsTransferAmount.PaymentTypeName = paymenttypename;
                        clsTransferAmount.amount = 0;
                        if (db.app_currencyfx.Where(x => x.id_currency == app_currency.id_currency && x.is_active).FirstOrDefault() != null)
                        {
                            app_currencyfx app_currencyfx = db.app_currencyfx.Where(x => x.id_currency == app_currency.id_currency && x.is_active).FirstOrDefault();
                            curname = app_currencyfx.app_currency.name;
                            id_currencyfx = app_currencyfx.id_currencyfx;
                        }
                        clsTransferAmount.Currencyfxname = curname;
                        clsTransferAmount.id_payment_type = id_paymentType;
                        clsTransferAmount.id_currencyfx = id_currencyfx;
                        listOpenAmt.Add(clsTransferAmount);
                    }

                }

                CashDataGrid.ItemsSource = listOpenAmt;
            }
        }

        private void btnActivateAccount_Click(object sender, RoutedEventArgs e)
        {
            if (db.app_account.Where(x => x.id_account == CurrentSession.Id_Account).FirstOrDefault() != null)
            {

                app_account app_account = db.app_account.Where(x => x.id_account == CurrentSession.Id_Account).FirstOrDefault();
               
                foreach (Class.clsTransferAmount list in listOpenAmt)
                {
                    app_account_detail app_account_detail = new global::entity.app_account_detail();
                    if (db.app_account_session.Where(x => x.id_account == app_account.id_account && x.is_active).FirstOrDefault() != null)
                    {
                        app_account_detail.id_session = db.app_account_session.Where(x => x.id_account == app_account.id_account && x.is_active).FirstOrDefault().id_session;
                    }
                    if (app_account.is_active == true)
                    {
                        //Make Inactive
                        app_account_detail.debit = list.amountCounted;
                    }
                    else
                    {
                        //Make Active
                        app_account_detail.credit = list.amountCounted;
                    }

                    app_account_detail.id_account = app_account.id_account;
                    app_account_detail.id_currencyfx = list.id_currencyfx;
                    app_account_detail.id_payment_type = list.id_payment_type;
                 
                 
                    if (app_account.is_active)
                    {
                        app_account_detail.comment = "For Closing Cash.";
                        if (db.app_account_session.Where(x=>x.id_account==app_account.id_account && x.is_active).FirstOrDefault()!=null)
                        {
                            app_account_detail.id_session = db.app_account_session.Where(x => x.id_account == app_account.id_account && x.is_active).FirstOrDefault().id_session;    
                        }
                        
                        app_account_detail.tran_type = app_account_detail.tran_types.ClosingBalance;
                    }
                    else
                    {
                        app_account_detail.comment = "For Opening Cash.";
                        app_account_session app_account_session = new entity.app_account_session();
                        app_account_session.id_account = app_account.id_account;
                        db.app_account_session.Add(app_account_session);
                        app_account_detail.id_session = app_account_session.id_session;
                        app_account_detail.tran_type = app_account_detail.tran_types.ClosingBalance;
                    }

                    app_account_detail.trans_date = DateTime.Now;
                    db.app_account_detail.Add(app_account_detail);
                }

                if (app_account.is_active == true)
                {
                    //Make Inactive
                    db.Entry(app_account).Entity.is_active = false;
                }
                else
                {
                    //Make Active
                    db.Entry(app_account).Entity.is_active = true;
                }
                // entity.db.Entry(app_account).Entity.initial_amount = Convert.ToDecimal(txtInitialAmount.Text.Trim());
                db.Entry(app_account).State = EntityState.Modified;

                db.SaveChanges();

                //Reload Data
                db.Entry(app_account).Reload();

                if (app_account.is_active)
                {
                    is_active = app_account.is_active;
                    RaisePropertyChanged("is_active");
                    MessageBox.Show("Account is Activated:");
                }
                else
                {
                    is_active = app_account.is_active;
                    RaisePropertyChanged("is_active");
                    MessageBox.Show("Account is DeActivated:");
                }

                if (app_accountViewSource != null)
                {
                    if (app_accountViewSource.View != null)
                    {
                        app_accountViewSource.View.Refresh();
                    }


                }

            }
        }
    }
}
