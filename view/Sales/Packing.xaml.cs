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
        db dbcontext = new db();
        CollectionViewSource payment_schedualViewSource;
        sales_invoice sales_invoice;
        entity.Properties.Settings _setting = new entity.Properties.Settings();
        public Packing()
        {
            InitializeComponent();
            payment_schedualViewSource = ((CollectionViewSource)(FindResource("payment_schedualViewSource")));
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            PackingListDB PackingListDB = new entity.PackingListDB();

            sales_packing sales_packing = PackingListDB.New();
            sales_packing.id_contact = sales_invoice.id_contact;

            foreach (dynamic item in dgvItem.Items)
            {
                 int id_product = item.id_item;
                 decimal quan = item.quantity;
                sales_packing_detail sales_packing_detail = new sales_packing_detail();
                sales_packing_detail.id_item = id_product;
                sales_packing_detail.quantity = quan;
                sales_packing_detail.id_location = item.id_location;

                sales_packing_relation sales_packing_relation = new sales_packing_relation();
                sales_packing_relation.id_sales_packing_detail = sales_packing_detail.id_sales_packing_detail;
                sales_packing_relation.sales_packing_detail = sales_packing_detail;
                sales_packing_relation.id_sales_invoice_detail = item.id_sales_invoice_detail;
                sales_packing_detail.sales_packing_relation.Add(sales_packing_relation);
                sales_packing.sales_packing_detail.Add(sales_packing_detail);
            }
            PackingListDB.SaveChanges();
            PackingListDB.Approve();

           


        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void set_InvoicePrefKeyStroke(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                set_InvoicePref(sender, e);
            }
        }

        private void set_InvoicePref(object sender, EventArgs e)
        {
            try
            {
                if (InvocieComboBox.Data != null)
                {
                    payment_schedual payment_schedual = (payment_schedual)InvocieComboBox.Data;
                    sales_invoice = payment_schedual.sales_invoice;
                    if (sales_invoice.app_contract.app_contract_detail.FirstOrDefault().interval == 0)
                    {
                        if (payment_schedual.debit - (payment_schedual.child.Count() > 0 ? payment_schedual.child.Sum(y => y.credit) : 0) > 0)
                        {
                            MessageBox.Show("Payment is Due ...");
                        }
                        else
                        {
                            dgvItem.ItemsSource = (from salesDetail in sales_invoice.sales_invoice_detail

                                                   join sales_packing_relation in dbcontext.sales_packing_relation on salesDetail.id_sales_invoice_detail equals sales_packing_relation.id_sales_invoice_detail
                                                   into b
                                                   from a in b.DefaultIfEmpty()
                                                   select new
                                                   {
                                                       Name = salesDetail.item.name,
                                                       quantity = salesDetail.quantity - (a != null ? a.sales_packing_detail.quantity : 0),
                                                       id_item = salesDetail.item!=null? salesDetail.item.id_item:0,
                                                       id_location=salesDetail.app_location!=null?salesDetail.app_location.id_location:0,
                                                       id_sales_invoice_detail = salesDetail.id_sales_invoice_detail
                                                   }).ToList();
                            InvocieComboBox.focusGrid = false;
                            InvocieComboBox.Text = payment_schedual.sales_invoice.number;
                        }
                    }



                    ///Start Thread to get Data.

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void InvocieComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            DateTime CurrentDate = DateTime.Now.Date;
            payment_schedualViewSource.Source = dbcontext.payment_schedual.Where(x => x.id_company == _setting.company_ID
                                                                               && x.sales_invoice.status == Status.Documents_General.Approved && x.debit>0).ToList().Distinct();
        }

        private void dgvItem_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            dynamic sales_invoice_detail = ((DataGrid)sender).SelectedItem;
            int id_product = sales_invoice_detail.id_item;
            DataGrid item_movementDataGrid = e.DetailsElement as DataGrid;
            var movement =
                (from items in dbcontext.items

                 join item_product in dbcontext.item_product on items.id_item equals item_product.id_item
                     into its
                 from p in its
                 join item_movement in dbcontext.item_movement on p.id_item_product equals item_movement.id_item_product
                 into IMS
                 from a in IMS
                 join AM in dbcontext.app_location on a.app_location.id_location equals AM.id_location
                 where a.status == Status.Stock.InStock && a.item_product.id_item == id_product
                 && a.app_location.id_branch == _setting.branch_ID
                 group a by new { a.item_product, a.app_location }
                     into last
                     select new
                     {
                         LocationName = last.Key.app_location.name,
                         quntitiy = last.Sum(x => x.credit) - last.Sum(x => x.debit)

                     }).ToList();
            item_movementDataGrid.ItemsSource = movement;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void toolIcon_Click(object sender, RoutedEventArgs e)
        {
            InvocieComboBox.focusGrid = false;
            InvocieComboBox.Text = "";
            dgvItem.ItemsSource = null;
        }

        private void toolIcon_Click_1(object sender, RoutedEventArgs e)
        {
            InvocieComboBox.focusGrid = false;
            InvocieComboBox.Text = "";
            dgvItem.ItemsSource = null;
        }
    }
}
