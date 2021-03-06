﻿using entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace cntrl.PanelAdv
{
    public partial class pnlSalesInvoice : UserControl
    {
        private CollectionViewSource sales_invoiceViewSource;

        private List<sales_invoice> _selected_sales_invoice;

        public List<sales_invoice> selected_sales_invoice
        {
            get { return _selected_sales_invoice; }
            set
            {
                _selected_sales_invoice = value;
            }
        }

        public contact _contact { get; set; }
        public db db { get; set; }

        public pnlSalesInvoice()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, MouseButtonEventArgs e)
        {
            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                if (_contact != null)
                {
                    LoadInvoice();
                }
            }
        }

        public event btnSave_ClickedEventHandler SalesInvoice_Click;

        public delegate void btnSave_ClickedEventHandler(object sender);

        public void btnSave_MouseUp(object sender, EventArgs e)
        {
            if (sales_invocieDatagrid.ItemsSource != null)
            {
                List<sales_invoice> sales_invoice = sales_invocieDatagrid.ItemsSource.OfType<sales_invoice>().ToList();
                selected_sales_invoice = sales_invoice.Where(x => x.IsSelected == true).ToList();

                SalesInvoice_Click?.Invoke(sender);
            }
        }

        private void LoadInvoice()
        {
            sales_invoiceViewSource = (CollectionViewSource)Resources["sales_invoiceViewSource"];
            List<sales_invoice> SalesList = db.sales_invoice
                .Where(x =>
                x.id_contact == _contact.id_contact &&
                x.status == Status.Documents_General.Approved
                ).ToList();
            List<int> sales = SalesList.Where(x => x.sales_invoice_detail.Where(y => y.Balance > 0).Count() > 0).Select(x => x.id_sales_invoice).ToList();
           
            sales_invoiceViewSource.Source = db.sales_invoice.Where(x => sales.Contains(x.id_sales_invoice)).ToList();
        }

        private void sales_invocieDatagrid_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            if (db.sales_invoice_detail.Count() > 0)
            {
                sales_invoice _sales_invoice = ((DataGrid)sender).SelectedItem as sales_invoice;
                if (_sales_invoice != null)
                {
                    DataGrid RowDataGrid = e.DetailsElement as DataGrid;
                    var salesInvoice = _sales_invoice.sales_invoice_detail.Where(x => x.Balance > 0);
                    RowDataGrid.ItemsSource = salesInvoice;
                }
            }
        }

        private void ContactPref(object sender, RoutedEventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                _contact = db.contacts.Find(sbxContact.ContactID);
            }
        }
    }
}