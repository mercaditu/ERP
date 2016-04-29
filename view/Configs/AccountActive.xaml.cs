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
namespace Cognitivo.Configs
{
    /// <summary>
    /// Interaction logic for AccountActive.xaml
    /// </summary>
    public partial class AccountActive : UserControl
    {
        List<Class.clsTransferAmount> listOpenAmt = null;
        public db db { get; set; }
        public CollectionViewSource  app_accountViewSource { get; set; }
    
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


                    var app_account_detailList = objAccount.app_account_detail
                 .GroupBy(ad => new { ad.id_currencyfx })
                 .Select(s => new
                 {
                     id_currencyfx = s.Max(ad => ad.app_currencyfx.id_currencyfx),
                     id_paymenttype = s.Max(ad => ad.id_payment_type),
                     cur = s.Max(ad => ad.app_currencyfx.app_currency.name),
                     payType = s.Max(ad => ad.payment_type.name),
                     amount = s.Sum(ad => ad.credit) - s.Sum(ad => ad.debit)
                 }).ToList();

                    var app_account_detailFinalList = app_account_detailList.GroupBy(x => x.cur).Select(s => new
                    {
                        id_currencyfx = s.Max(x => x.id_currencyfx),
                        id_paymenttype = s.Max(x => x.id_paymenttype),
                        cur = s.Max(ad => ad.cur),
                        payType = s.Max(ad => ad.payType),
                        amount = s.Sum(ad => ad.amount)
                    }).ToList();
                    listOpenAmt = new List<Class.clsTransferAmount>();
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
                    app_account_detail.comment = "For Opening or closing Cash.";
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

                if (app_accountViewSource!=null)
                {
                    if (app_accountViewSource.View!=null)
                    {
                        app_accountViewSource.View.Refresh();
                    }
                 
                     
                }
               
            }
        }
    }
}
