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

namespace cntrl.PanelAdv
{
    /// <summary>
    /// Interaction logic for ActionPanel.xaml
    /// </summary>
    public partial class ActionPanel : UserControl
    {
        public int ID { get; set; }
        public App.Names Application { get; set; }
        public db db { get; set; }
        List<payment_schedual> PaymentSchedualList = new List<payment_schedual>();
        CollectionViewSource payment_schedualViewSource;
        sales_invoice sales_invoice;
        public ActionPanel()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
             sales_invoice = db.sales_invoice.Find(ID);
            CollectionViewSource payment_schedualViewSource = (CollectionViewSource)FindResource("payment_schedualViewSource");
          
            if (Application == App.Names.SalesInvoice)
            {
               
              
              PaymentSchedualList = sales_invoice.payment_schedual.ToList();
                payment_schedualViewSource.Source = PaymentSchedualList;
                List<app_contract_detail> app_contract_detailList = sales_invoice.app_contract.app_contract_detail.Where(x => x.is_order==false).ToList();
                

                for (int i = 1; i <= app_contract_detailList.Count; i++)
                {
                    payment_schedual payment_schedual = PaymentSchedualList.Skip(i - 1).Take(1).FirstOrDefault();
                    app_contract_detail app_contract_detail= app_contract_detailList.Skip(i - 1).Take(1).FirstOrDefault();
                    if (payment_schedual!=null)
                    {
                        payment_schedual.ShouldValue = sales_invoice.GrandTotal * app_contract_detail.coefficient;
                        payment_schedual.ShouldExpDATE = sales_invoice.trans_date.AddDays(app_contract_detail.interval);
                    }
                    else
                    {
                        payment_schedual payment_schedualNew = new payment_schedual();
                        payment_schedualNew.ShouldValue = sales_invoice.GrandTotal * app_contract_detail.coefficient;
                        payment_schedualNew.ShouldExpDATE = sales_invoice.trans_date.AddDays(app_contract_detail.interval);
                        payment_schedualNew.id_currencyfx = sales_invoice.id_currencyfx;
                        payment_schedualNew.sales_invoice = sales_invoice;
                        payment_schedualNew.trans_date = sales_invoice.trans_date;
                        payment_schedualNew.expire_date = sales_invoice.trans_date.AddDays(app_contract_detail.interval);
                        payment_schedualNew.status = Status.Documents_General.Pending;
                        payment_schedualNew.id_contact = sales_invoice.id_contact;
                        PaymentSchedualList.Add(payment_schedualNew);
                    } 
                }
            }
            payment_schedualViewSource.View.Refresh();
        }

        private void btnCancel_Click(object sender, MouseButtonEventArgs e)
        {
            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (PaymentSchedualList.Sum(x=>x.debit)==sales_invoice.GrandTotal)
            {
                foreach (payment_schedual payment_schedual in PaymentSchedualList)
                {
                    entity.Brillo.Logic.Payment _Payment = new entity.Brillo.Logic.Payment();
                    _Payment.ModifyPaymentSchedual(db, payment_schedual.id_payment_schedual);
                }
               
            }
          
        }
    }
}
