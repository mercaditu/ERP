using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using entity;
using cntrl.Class;

namespace cntrl.PanelAdv
{
    /// <summary>
    /// Interaction logic for ActionPanel.xaml
    /// </summary>
    public partial class ActionPanelAnull : UserControl
    {
        public int ID { get; set; }
        public App.Names Application { get; set; }
        public db db { get; set; }
        List<payment_schedual> PaymentSchedualList = new List<payment_schedual>();
        List<item_movement> item_movementList = new List<item_movement>();

        public ActionPanelAnull()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            CollectionViewSource item_movementViewSource = (CollectionViewSource)FindResource("item_movementViewSource");
            CollectionViewSource payment_schedualViewSource = (CollectionViewSource)FindResource("payment_schedualViewSource");
            cbxStatus.ItemsSource = Enum.GetValues(typeof(item_movement.Actions)).OfType<item_movement.Actions>();
            cbxStatusPayment.ItemsSource = Enum.GetValues(typeof(payment_schedual.Actions)).OfType<item_movement.Actions>();
            if (Application == App.Names.SalesInvoice)
            {
                sales_invoice sales_invoice = db.sales_invoice.Find(ID);

                PaymentSchedualList = sales_invoice.payment_schedual.Where(x => x.debit > 0).ToList();
                payment_schedualViewSource.Source = PaymentSchedualList;
                foreach (payment_schedual payment_schedual in PaymentSchedualList)
                {

                    payment_schedual.ActionStatus = payment_schedual.ActionsStatus.Green;
                    payment_schedual.Action = payment_schedual.Actions.Delete;

                }

                item_movementList = db.item_movement.Where(x => x.sales_invoice_detail.id_sales_invoice == sales_invoice.id_sales_invoice && x.debit > 0).ToList();
                item_movementViewSource.Source = item_movementList;
                foreach (item_movement item_movement in item_movementList)
                {

                    item_movement.ActionStatus = item_movement.ActionsStatus.Green;
                    item_movement.Action = item_movement.Actions.Delete;


                }


            }
            else if (Application == App.Names.PurchaseInvoice)
            {
                purchase_invoice purchase_invoice = db.purchase_invoice.Find(ID);

                PaymentSchedualList = purchase_invoice.payment_schedual.Where(x => x.credit > 0).ToList();
                payment_schedualViewSource.Source = PaymentSchedualList;
                foreach (payment_schedual payment_schedual in PaymentSchedualList)
                {

                    payment_schedual.ActionStatus = payment_schedual.ActionsStatus.Green;
                    payment_schedual.Action = payment_schedual.Actions.Delete;

                }

                item_movementList = db.item_movement.Where(x => x.purchase_invoice_detail.id_purchase_invoice == purchase_invoice.id_purchase_invoice && x.credit > 0).ToList();
                item_movementViewSource.Source = item_movementList;
                foreach (item_movement item_movement in item_movementList)
                {

                    if (item_movement.child.Count() > 0)
                    {
                        item_movement.ActionStatus = item_movement.ActionsStatus.Red;
                        item_movement.Action = item_movement.Actions.ReApprove;
                    }
                    else
                    {
                        item_movement.ActionStatus = item_movement.ActionsStatus.Green;
                        item_movement.Action = item_movement.Actions.Delete;
                    }


                }
            }
            payment_schedualViewSource.View.Refresh();
            item_movementViewSource.View.Refresh();
        }

        private void btnCancel_Click(object sender, MouseButtonEventArgs e)
        {
            if (Application == App.Names.SalesInvoice)
            {
                sales_invoice sales_invoice = db.sales_invoice.Find(ID);
                sales_invoice.status = Status.Documents_General.Approved;
            }
            else if (Application == App.Names.PurchaseInvoice)
            {
                purchase_invoice purchase_invoice = db.purchase_invoice.Find(ID);
                purchase_invoice.status = Status.Documents_General.Approved;
            }

         
            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            foreach (payment_schedual payment_schedual in PaymentSchedualList)
            {
                if (payment_schedual.Action == payment_schedual.Actions.Delete)
                {
                    entity.Brillo.Logic.Payment _Payment = new entity.Brillo.Logic.Payment();
                    _Payment.DeletePaymentSchedual(db, payment_schedual.id_payment_schedual);
                }

            }

            foreach (item_movement item_movement in item_movementList)
            {
                if (item_movement.Action == item_movement.Actions.Delete)
                {
                    entity.Brillo.Logic.Stock _Stock = new entity.Brillo.Logic.Stock();
                    db.item_movement.Remove(item_movement);
                }
                else if (item_movement.Action == item_movement.Actions.ReApprove)
                {
                    foreach (var item in item_movement.child)
                    {
                      
                        List<item_movement> item_movementList = db.item_movement.Where(x => x.id_item_product == item_movement.id_item_product && x.id_movement != item_movement.id_movement && x.credit>0).ToList();
                        foreach (item_movement _item_movement in item_movementList)
                        {
                            if (_item_movement.avlquantity > item.credit)
                            {
                                item.parent = _item_movement;
                            }
                            else
                            {
                                item.parent = null;
                            }
                        }

                    }
                    db.item_movement.Remove(item_movement);


                }

            }
            
            db.SaveChanges();
            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }

    }
}
