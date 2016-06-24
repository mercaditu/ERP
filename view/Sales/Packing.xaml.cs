using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using entity;
using System.Windows.Input;

namespace Cognitivo.Sales
{
    public partial class Packing : Page
    {
        db dbContext = new db();
        CollectionViewSource item_movementViewSource;
        sales_invoice sales_invoice;

        public string InvoiceNumber 
        {
            get { return _InvoiceNumber; }
            set
            {
                if (_InvoiceNumber != value)
                {
                    _InvoiceNumber = value;
                    //if (_InvoiceNumber.Length > 3)
                    //{
                    //    ListProducts();
                    //}
                }
            }
        }

        private string _InvoiceNumber;

        public Packing()
        {
            InitializeComponent();
            item_movementViewSource = ((CollectionViewSource)(FindResource("item_movementViewSource")));
        }

        private void ListProducts(object sender, EventArgs e)
        {
            if (InvoiceNumber != string.Empty)
            {
                List<sales_invoice_detail> sales_invoice_detailLIST = dbContext.sales_invoice_detail
                    .Where(x => x.sales_invoice.number.Contains(InvoiceNumber) && 
                        x.sales_invoice.payment_schedual.Sum(z => z.credit) > 0 &&
                        x.sales_invoice.status == Status.Documents_General.Approved).ToList();

                List<item_movement> item_movementLIST = new List<item_movement>();

                foreach (sales_invoice_detail sales_invoice_detail in sales_invoice_detailLIST.Where(x => x.item.item_product != null))
                {
                    item_movement item_movement = new entity.item_movement();

                    item_movement.trans_date = DateTime.Now;
                    item_movement.id_item_product = sales_invoice_detail.item.item_product.FirstOrDefault().id_item_product;
                    item_movement.item_product = sales_invoice_detail.item.item_product.FirstOrDefault();
                    item_movement.id_sales_invoice_detail = sales_invoice_detail.id_sales_invoice_detail;
                    item_movement.debit = 0;
                    item_movement.credit = 0;
                    item_movement.status = Status.Stock.InStock;
                    item_movement.timestamp = DateTime.Now;
                    item_movement.State = System.Data.Entity.EntityState.Added;

                    if (sales_invoice_detail.id_location != null || sales_invoice_detail.id_location > 0)
                    {
                        item_movement.id_location = (int)sales_invoice_detail.id_location;
                    }
                    else
                    {
                        //find location code
                        item_movement.id_location = 1;
                    }

                    if (sales_invoice_detail.item_movement != null)
	                {
                        item_movement.debit = sales_invoice_detail.quantity - sales_invoice_detail.item_movement.Sum(x => x.debit);
	                }
                    else
                    {
                        item_movement.debit = sales_invoice_detail.quantity;
                    }

                    if (item_movement.debit > 0)
                    {
                        item_movementLIST.Add(item_movement);
                    }
                }

                dbContext.item_movement.AddRange(item_movementLIST);
                item_movementViewSource.Source = item_movementLIST;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            dbContext.SaveChanges();
            item_movementViewSource.Source = null;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            btnCancel_Click(null, null);
            item_movementViewSource.Source = null;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //Cancel Code.
        }
    }
}
