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
        CollectionViewSource inventoryViewSource;
             
        public string InvoiceNumber 
        {
            get { return _InvoiceNumber; }
            set
            {
                if (_InvoiceNumber != value)
                {
                    _InvoiceNumber = value;
                }
            }
        }

        private string _InvoiceNumber;

        public Packing()
        {
            InitializeComponent();
            item_movementViewSource = ((CollectionViewSource)(FindResource("item_movementViewSource")));
            inventoryViewSource = ((CollectionViewSource)(FindResource("inventoryViewSource")));
        }

        private void ListProducts(object sender, EventArgs e)
        {
            if (InvoiceNumber != string.Empty)
            {
                List<sales_invoice_detail> sales_invoice_detailLIST = dbContext.sales_invoice_detail
                    .Where(x => x.sales_invoice.number.Contains(InvoiceNumber) &&
                        //Contado (Cash) + Payment Made
                        (
                        (x.sales_invoice.payment_schedual.Sum(z => z.credit) > 0 && x.sales_invoice.app_contract.app_contract_detail.Sum(z => z.coefficient) == 0) 
                        
                        ||
                        //Credit
                        (x.sales_invoice.app_contract.app_contract_detail.Sum(y => y.coefficient) > 0)
                        ) &&
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

        private void StockList(object sender, EventArgs e)
        {
            if (item_movementViewSource.View != null)
            {
                if (item_movementViewSource.View.CurrentItem != null)
                {
                    item_product _item_product = (item_movementViewSource.View.CurrentItem as item_movement).item_product;
                    if (_item_product != null && inventoryViewSource != null)
                    {
                        using (StockDB StockDB = new StockDB())
                        {
                            var movement =
                               (from items in StockDB.items
                                join item_product in StockDB.item_product on items.id_item equals item_product.id_item
                                    into its
                                from p in its
                                join item_movement in StockDB.item_movement on p.id_item_product equals item_movement.id_item_product
                                into IMS
                                from a in IMS
                                join AM in StockDB.app_branch on a.app_location.id_branch equals AM.id_branch
                                where a.status == Status.Stock.InStock
                                && a.id_item_product == _item_product.id_item_product
                                && a.trans_date <= DateTime.Now
                                && a.app_location.id_branch == CurrentSession.Id_Branch
                                group a by new { a.item_product, a.app_location }
                                    into last
                                    select new
                                    {
                                        code = last.Key.item_product.item.code,
                                        name = last.Key.item_product.item.name,
                                        location = last.Key.app_location.name,
                                        itemid = last.Key.item_product.item.id_item,
                                        quantity = last.Sum(x => x.credit) - last.Sum(x => x.debit),
                                        id_item_product = last.Key.item_product.id_item_product,
                                        measurement = last.Key.item_product.item.app_measurement.code_iso,
                                        id_location = last.Key.app_location.id_location
                                    }).ToList().OrderBy(y => y.name);

                            inventoryViewSource.Source = movement;
                        }
                    }
                }
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
