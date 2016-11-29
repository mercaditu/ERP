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
using entity.Brillo;

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
        List<item_movement> item_movementList = new List<item_movement>();
        CollectionViewSource payment_schedualViewSource, item_movementViewSource;
  
        public ActionPanel()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            CollectionViewSource item_movementViewSource = (CollectionViewSource)FindResource("item_movementViewSource");
            CollectionViewSource payment_schedualViewSource = (CollectionViewSource)FindResource("payment_schedualViewSource");

            if (Application == App.Names.SalesInvoice)
            {
                sales_invoice sales_invoice = db.sales_invoice.Find(ID);


                PaymentSchedualList = sales_invoice.payment_schedual.Where(x=>x.debit>0).ToList();
                payment_schedualViewSource.Source = PaymentSchedualList;
                List<app_contract_detail> app_contract_detailList = sales_invoice.app_contract.app_contract_detail.Where(x => x.is_order == false).ToList();


                for (int i = 1; i <= app_contract_detailList.Count; i++)

                {
                    payment_schedual payment_schedual = PaymentSchedualList.Skip(i - 1).Take(1).FirstOrDefault();
                    app_contract_detail app_contract_detail = app_contract_detailList.Skip(i - 1).Take(1).FirstOrDefault();
                    if (payment_schedual != null)
                    {
                        if (payment_schedual.child!=null)
                        {
                            payment_schedual.ActionStatus = payment_schedual.ActionsStatus.Red;
                            payment_schedual.Action=payment_schedual.Actions.ReApprove;
                        }
                        else
                        {
                            payment_schedual.ActionStatus = payment_schedual.ActionsStatus.Green;
                            payment_schedual.Action = payment_schedual.Actions.Delete;
                        }
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

                item_movementList = db.item_movement.Where(x => x.sales_invoice_detail.id_sales_invoice == sales_invoice.id_sales_invoice && x.debit > 0).ToList();
                item_movementViewSource.Source = item_movementList;
                foreach (item_movement item_movement in item_movementList)
                {

                    if (item_movement.parent.avlquantity> item_movement.sales_invoice_detail.quantity)
                    {
                        item_movement.debit = item_movement.sales_invoice_detail.quantity;
                    }
                    else
                    {
                        entity.Brillo.Logic.Stock _stock = new entity.Brillo.Logic.Stock();
                        entity.Brillo.Stock stock = new entity.Brillo.Stock();
                        List<StockList> Items_InStockLIST = stock.List(item_movement.sales_invoice_detail.app_location.id_branch, (int)item_movement.sales_invoice_detail.id_location, item_movement.sales_invoice_detail.item.item_product.FirstOrDefault().id_item_product);

                        db.item_movement.AddRange(_stock.DebitOnly_MovementLIST(db, Items_InStockLIST, Status.Stock.InStock,
                                                    App.Names.SalesInvoice,
                                                    item_movement.sales_invoice_detail.id_sales_invoice,
                                                    item_movement.sales_invoice_detail.id_sales_invoice_detail,
                                                    sales_invoice.id_currencyfx,
                                                    item_movement.sales_invoice_detail.item.item_product.FirstOrDefault(),
                                                    (int)item_movement.sales_invoice_detail.id_location,
                                                    item_movement.sales_invoice_detail.quantity,
                                                    sales_invoice.trans_date,
                                                    "Sales Invoice Fix"
                                                    ));
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
            if (Application == App.Names.SalesInvoice)
            {
                sales_invoice sales_invoice = db.sales_invoice.Find(ID);
                if (PaymentSchedualList.Sum(x => x.debit) == sales_invoice.GrandTotal)
                {
                    foreach (payment_schedual payment_schedual in PaymentSchedualList)
                    {
                        entity.Brillo.Logic.Payment _Payment = new entity.Brillo.Logic.Payment();
                        _Payment.ModifyPaymentSchedual(db, payment_schedual.id_payment_schedual);
                    }

                }
                foreach (item_movement item_movement in item_movementList)
                {
                    if (item_movement.Action == item_movement.Actions.Delete)

                    {
                        db.item_movement.Remove(item_movement);
                    }
                    else
                    {
                       
                    }
                  
                }
               
            }

        }

     
    }
}
