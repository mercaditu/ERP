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

            if (Directory.Exists(@Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Payments"))
            {
                var dir = new DirectoryInfo(@Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Payments");
                dir.Delete(true);
            }


            List<contact> contactLIST = new List<contact>();
            payment_schedualList = PaymentDB.payment_schedual
                    .Where(x => x.id_payment_detail == null && x.id_company == CurrentSession.Id_Company
                        && (x.id_sales_invoice > 0 || x.id_sales_order > 0)
                        && (x.debit - (x.child.Count() > 0 ? x.child.Sum(y => y.credit) : 0)) > 0)
                        .Include(x => x.sales_invoice)
                        .Include(x => x.contact)
                        .OrderBy(x => x.expire_date)
                        .ToList();
            if (payment_schedualList.Count() > 0)
            {
                foreach (payment_schedual payment in payment_schedualList.ToList())
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
                    sw.Write(DateTime.Now.ToString("yyyyMMdd") + ";" + contactLIST.Count() + ";" + Environment.NewLine);
                    foreach (contact contact in contactLIST)
                    {
                        string active = "N";
                        if (contact.is_active)
                        {
                            active = "S";
                        }
                        string gov_code = "";
                        if (contact.gov_code != null)
                        {
                            gov_code = contact.gov_code.Replace("-", "");
                            gov_code = gov_code.Remove(gov_code.Length - 1, 1).ToString();
                        }

                        sw.Write(gov_code + ";" + contact.name + ";" + active + ";" + Environment.NewLine);
                    }
                }
            }

            string fileDetail = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Payments\\detail-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

            if (!File.Exists(fileDetail))
            {
                using (StreamWriter sw = new StreamWriter(fileDetail))
                {
                    Decimal Balance = 0;

                    foreach (payment_schedual payment_schedual in payment_schedualList.ToList())
                    {
                        Balance += payment_schedual.AccountReceivableBalance;
                    }

                    sw.Write(DateTime.Now.ToString("yyyyMMdd") + ";" + payment_schedualList.Count() + ";" + Math.Round(Balance, 2) + Environment.NewLine);
                    foreach (payment_schedual payment_schedual in payment_schedualList.ToList())
                    {
                        string gov_code = "";
                        if (payment_schedual.contact.gov_code != null)
                        {
                            gov_code = payment_schedual.contact.gov_code.Replace("-", "");
                            gov_code = gov_code.Remove(gov_code.Length - 1, 1).ToString();
                        }


                        string id_payment_schedual = payment_schedual.id_payment_schedual.ToString();
                        string comment = "Cuota" + " " + DateTime.Now.ToString("yyyyMMdd");
                        string date = payment_schedual.trans_date.ToString("yyyyMMdd");
                        string Expdate = payment_schedual.expire_date.ToString("yyyyMMdd");
                        string currency = payment_schedual.app_currencyfx.app_currency.code == "PYG" ? "1" : "2";
                        string balance = Math.Round(payment_schedual.AccountReceivableBalance, 2).ToString();
                        string other = "0;F;0;0;N;1";
                        string detail = gov_code + ";" + id_payment_schedual + ";" + comment + ";" + date + ";"
                            + Expdate + ";" + currency + ";" + balance + ";"
                             + other + ";" + Environment.NewLine;
                        sw.Write(detail);
                    }
                }
            }
            string startpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Payments";
            string zippath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".zip";
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
                            List<string> values = new List<string>();
                            values.Add(new string(line.Skip(0).Take(8).ToArray()));
                            values.Add(new string(line.Skip(8).Take(8).ToArray()));
                            values.Add(new string(line.Skip(16).Take(6).ToArray()));
                            values.Add(new string(line.Skip(22).Take(6).ToArray()));
                            values.Add(new string(line.Skip(28).Take(30).ToArray()));
                            values.Add(new string(line.Skip(58).Take(14).ToArray()));
                            values.Add(new string(line.Skip(72).Take(1).ToArray()));
                            values.Add(new string(line.Skip(73).Take(1).ToArray()));
                            values.Add(new string(line.Skip(74).Take(14).ToArray()));
                            values.Add(new string(line.Skip(88).Take(10).ToArray()));
                            values.Add(new string(line.Skip(98).Take(2).ToArray()));
                            values.Add(new string(line.Skip(100).Take(4).ToArray()));
                            values.Add(new string(line.Skip(104).Take(14).ToArray()));
                            values.Add(new string(line.Skip(118).Take(14).ToArray()));
                            values.Add(new string(line.Skip(132).Take(3).ToArray()));
                            values.Add(new string(line.Skip(135).Take(30).ToArray()));
                            values.Add(new string(line.Skip(165).Take(30).ToArray()));

                            if (values[0] != "" && values.Count() > 0)
                            {


                                int id = Convert.ToInt32(values[4]);
                                List<payment_schedual> SchedualList = PaymentDB.payment_schedual.Where(x => x.id_payment_schedual == id).ToList();
                                if (SchedualList.Count() > 0)
                                {


                                    payment payment = PaymentDB.New(true);
                                    payment.trans_date = Convert.ToDateTime(new string(values[1].Take(4).ToArray()) + "/" + new string(values[1].Skip(4).Take(2).ToArray()) + "/" + new string(values[1].Skip(6).Take(2).ToArray()));

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

                                        payment_detail.value = Convert.ToDecimal(values[5]);


                                        //If PaymentTypeID is not null, then this transaction has a PaymentApproval

                                        payment_detail.IsLocked = true;
                                        payment_detail.id_account = Convert.ToInt32(cbxAccount.SelectedValue);
                                        payment_detail.IsLocked = false;

                                        if (Convert.ToInt32(values[7]) == 1)
                                        {
                                            payment_type payment_type = PaymentDB.payment_type.Where(x => x.payment_behavior == payment_type.payment_behaviours.Normal && x.is_direct).FirstOrDefault();
                                            payment_detail.id_payment_type = payment_type.id_payment_type;
                                        }
                                        else
                                        {
                                            payment_type payment_type = PaymentDB.payment_type.Where(x => x.payment_behavior == payment_type.payment_behaviours.Normal && x.is_direct == false).FirstOrDefault();
                                            payment_detail.id_payment_type = payment_type.id_payment_type;
                                        }






                                        payment_detail.IsSelected = true;

                                        payment.payment_detail.Add(payment_detail);

                                        PaymentDB.payments.Add(payment);
                                        PaymentDB.Approve(SchedualList, true, false);
                                    }
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
