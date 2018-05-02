using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using entity;
using System.Data.Entity;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;

using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Cognitivo.Configs
{
    /// <summary>
    /// Interaction logic for PendingReceivable.xaml
    /// </summary>
    public partial class PendingReceivable : Page
    {
        public PendingReceivable()
        {
            InitializeComponent();
        }
        private entity.dbContext entity = new entity.dbContext();
        private CollectionViewSource app_accountViewSource;
        List<payment_schedual> payment_schedualList;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                app_accountViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("app_accountViewSource")));
                entity.db.app_account.Where(a => a.id_company == CurrentSession.Id_Company).Load();
                app_accountViewSource.Source = entity.db.app_account.Local;
            }
            catch { }
        }
        private void Download_Click(object sender, RoutedEventArgs e)
        {
            PaymentDB PaymentDB = new PaymentDB();
           
            List<contact> contactLIST = new List<contact>();
            payment_schedualList = PaymentDB.payment_schedual.ToList();
            if (payment_schedualList.Where(x=>x.AccountReceivableBalance>0).Count() > 0)
            {
                foreach (payment_schedual payment in payment_schedualList.Where(x => x.AccountReceivableBalance > 0).ToList())
                {
                    if (contactLIST.Contains(payment.contact) == false)
                    {
                        contact contact = new contact();
                        contact = payment.contact;
                        contactLIST.Add(contact);
                    }
                }


            }

            System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Payments");

            string fileHeader = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Payments\\header-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

            if (!File.Exists(fileHeader))
            {
                using (StreamWriter sw = new StreamWriter(fileHeader))
                {
                    sw.Write(DateTime.Now.ToString("yyyy-MM-dd") + ";" + contactLIST.Count() + ";" + Environment.NewLine);
                    foreach (contact contact in contactLIST)
                    {

                        sw.Write(contact.gov_code + ";" + contact.name + ";" + Environment.NewLine);
                    }
                }
            }

            string fileDetail = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Payments\\detail-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

            if (!File.Exists(fileDetail))
            {
                using (StreamWriter sw = new StreamWriter(fileDetail))
                {
                    Decimal Balance = 0;
                  
                    foreach (payment_schedual payment_schedual in payment_schedualList.Where(x => x.AccountReceivableBalance > 0).ToList())
                    {
                        Balance += payment_schedual.AccountReceivableBalance;
                    }

                    sw.Write(DateTime.Now.ToString("yyyy-MM-dd") + ";" + payment_schedualList.Where(x => x.AccountReceivableBalance > 0).ToList() + ";" + Balance + Environment.NewLine);
                    foreach (payment_schedual payment_schedual in payment_schedualList.Where(x => x.AccountReceivableBalance > 0).ToList())
                    {
                        string gov_code = payment_schedual.contact.gov_code;
                        string id_payment_schedual = payment_schedual.id_payment_schedual.ToString();
                        string comment = "Cuota" + payment_schedual.trans_date.ToString("yyyy-MM-dd");
                        string date = payment_schedual.expire_date.ToString("yyyy-MM-dd");
                        string currency = payment_schedual.app_currencyfx.app_currency.code == "PYG" ? "1" : "2";
                        string balance = payment_schedual.AccountReceivableBalance.ToString();
                        string other = "0;F;0;0;N;1";
                        string detail = gov_code + ";" + id_payment_schedual + ";" + comment + ";" + date
                            + date + ";" + balance + ";"
                             + other + ";" + Environment.NewLine;
                        sw.Write(detail);
                    }
                }
            }
            string startpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Payments";
            string zippath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + DateTime.Now.ToString("yyyy-mm-dd") + ".zip";
            ZipFile.CreateFromDirectory(startpath, zippath);

        }

        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string line;
            int counter = 0;
            PaymentDB PaymentDB = new PaymentDB();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    System.IO.StreamReader file = new System.IO.StreamReader(@filename);
                    while ((line = file.ReadLine()) != null)
                    {

                        if (counter > 0)
                        {
                            string[] values = line.Split(';');
                            if (values[0] != "" && values.Count()>9)
                            {


                                int id = Convert.ToInt32(values[9]);
                                List<payment_schedual> SchedualList = PaymentDB.payment_schedual.Where(x => x.id_payment_schedual == id).ToList();
                                payment payment = PaymentDB.New(true);

                                if (payment != null)
                                {
                                    payment_detail payment_detail = new payment_detail();
                                    payment_detail.payment = payment;
                                    //Get current Active Rate of selected Currency.
                                    string currency = Convert.ToInt32(values[10]) == 1 ? "PYG" : "USD";
                                    app_currencyfx app_currencyfx = PaymentDB.app_currencyfx.Where(x => x.app_currency.code == currency && x.id_company == CurrentSession.Id_Company && x.is_active).FirstOrDefault();
                                    if (app_currencyfx != null)
                                    {
                                        payment_detail.Default_id_currencyfx = app_currencyfx.id_currencyfx;
                                        payment_detail.id_currencyfx = app_currencyfx.id_currencyfx;
                                        payment_detail.payment.id_currencyfx = app_currencyfx.id_currencyfx;
                                        payment_detail.app_currencyfx = app_currencyfx;
                                    }

                                    //Always get total value of Accounts Receivable from a particular Currency, and not Currency Rate. This is very important when Currency Fluctates.

                                    payment_detail.value = Convert.ToDecimal(values[6]);


                                    //If PaymentTypeID is not null, then this transaction has a PaymentApproval

                                    payment_detail.IsLocked = true;
                                    payment_detail.id_account = Convert.ToInt32(cbxAccount.SelectedValue);
                                    payment_detail.IsLocked = false;

                                    payment_type payment_type = PaymentDB.payment_type.Where(x => x.payment_behavior == payment_type.payment_behaviours.Normal).FirstOrDefault();
                                    payment_detail.id_payment_type = payment_type.id_payment_type;




                                    payment_detail.IsSelected = true;

                                    payment.payment_detail.Add(payment_detail);

                                    PaymentDB.payments.Add(payment);
                                    PaymentDB.Approve(SchedualList, true, false);
                                }
                            }

                        }
                        counter++;
                    }

                }
            }
        }
    }
}
