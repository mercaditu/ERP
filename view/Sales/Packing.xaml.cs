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
       db dbcontext= new db();
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
                    dgvItem.ItemsSource = sales_invoice.sales_invoice_detail.ToList();
                    InvocieComboBox.focusGrid = false;
                    InvocieComboBox.Text = payment_schedual.sales_invoice.number;

                    ///Start Thread to get Data.
                  
                }
            }
            catch (Exception ex)
            {
              //  toolBar.msgError(ex);
            }
        }

        private void InvocieComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            DateTime CurrentDate = DateTime.Now;
            payment_schedualViewSource.Source = dbcontext.payment_schedual.Where(x => x.id_company == _setting.company_ID &&
                                                                                x.expire_date <= CurrentDate && (x.debit - (x.child.Count() > 0 ? x.child.Sum(y => y.credit) : 0)) == 0 && x.debit > 0 && x.sales_invoice.status == Status.Documents_General.Approved).ToList(); ;
        }

        private void dgvItem_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            sales_invoice_detail sales_invoice_detail = ((DataGrid)sender).SelectedItem as sales_invoice_detail;
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
                 group a by new { a.item_product,a.app_location }
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
    }
}
