﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
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
            cbxStatus.ItemsSource = Enum.GetValues(typeof(item_movement.Actions));
            cbxStatusPayment.ItemsSource = Enum.GetValues(typeof(payment_schedual.Actions));
            if (Application == App.Names.SalesInvoice)
            {
                sales_invoice sales_invoice = db.sales_invoice.Find(ID);


                PaymentSchedualList = sales_invoice.payment_schedual.Where(x => x.debit > 0).ToList();
                payment_schedualViewSource.Source = PaymentSchedualList;
                List<app_contract_detail> app_contract_detailList = sales_invoice.app_contract.app_contract_detail.Where(x => x.is_order == false).ToList();


                for (int i = 1; i <= app_contract_detailList.Count; i++)
                {
                    payment_schedual payment_schedual = PaymentSchedualList.Skip(i - 1).Take(1).FirstOrDefault();
                    payment_schedual.Action = null;
                    app_contract_detail app_contract_detail = app_contract_detailList.Skip(i - 1).Take(1).FirstOrDefault();
                    if (payment_schedual != null)
                    {
                        if (payment_schedual.child != null)
                        {
                            payment_schedual.ActionStatus = payment_schedual.ActionsStatus.Red;

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
                        payment_schedualNew.ActionStatus = payment_schedual.ActionsStatus.Green;
                        payment_schedualNew.Action = payment_schedual.Actions.ReApprove;
                        PaymentSchedualList.Add(payment_schedualNew);
                    }
                }

                item_movementList = db.item_movement.Where(x => x.sales_invoice_detail.id_sales_invoice == sales_invoice.id_sales_invoice && x.debit > 0).ToList();
                item_movementViewSource.Source = item_movementList;
                foreach (item_movement item_movement in item_movementList)
                {

                    if (item_movement.child.Count()>0)
                    {
                        item_movement.Action = item_movement.Actions.NotProcess;
                    }
                    else if (item_movement.debit!=item_movement.sales_invoice_detail.quantity)
                    {
                        item_movement.Action = item_movement.Actions.ReApprove;
                    }
                    else
                    {
                        item_movement.Action = item_movement.Actions.Delete;
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
                if (PaymentSchedualList.Where(x => x.Action == payment_schedual.Actions.NotProcess).Count() > 0)
                {
                    btnCancel_Click(sender, null);
                }
                else
                {
                   
                    foreach (payment_schedual payment_schedual in PaymentSchedualList)
                    {
                        if (payment_schedual.Action == payment_schedual.Actions.ReApprove)
                        {
                            payment_schedual.debit = payment_schedual.ShouldValue;
                            entity.Brillo.Logic.Payment _Payment = new entity.Brillo.Logic.Payment();
                            _Payment.ModifyPaymentSchedual(db, payment_schedual.id_payment_schedual);
                        }
                        else if (payment_schedual.Action == payment_schedual.Actions.Delete)
                        {
                            db.payment_schedual.Remove(payment_schedual);
                        }

                    }
                    
                    btnCancel_Click(sender,null);
                }

                if (item_movementList.Where(x => x.Action == item_movement.Actions.NotProcess).Count() > 0)
                {
                    btnCancel_Click(sender, null);
                }
                else
                {
                
                    foreach (item_movement item_movement in item_movementList)
                    {
                        if (item_movement.Action == item_movement.Actions.ReApprove)
                        {
                            item_movement.debit = item_movement.sales_invoice_detail.quantity;
                          
                        }
                        else if (item_movement.Action == item_movement.Actions.Delete)
                        {
                            db.item_movement.Remove(item_movement);
                        }

                    }
                  
                    btnCancel_Click(sender, null);
                }


            }
           

        }


    }
}
