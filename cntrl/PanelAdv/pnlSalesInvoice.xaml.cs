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
    /// Interaction logic for pnlSalesInvoice.xaml
    /// </summary>
    public partial class pnlSalesInvoice : UserControl
    {
        CollectionViewSource sales_invoiceViewSource;

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
        public ImpexDB _entity { get; set; }

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
                //Load your data here and assign the result to the CollectionViewSource.


                if (_contact != null)
                {
                    sales_invoiceViewSource = (CollectionViewSource)Resources["sales_invoiceViewSource"];
                    sales_invoiceViewSource.Source = _entity.sales_invoice.Where(x => x.id_contact == _contact.id_contact).ToList();




                }
            }
        }
        //private void load_SalesInvoice(int id_contact)
        //{
        //    var order = (from sales_invoice_detail in _entity.sales_invoice_detail
        //                 where sales_invoice_detail.sales_invoice.id_contact == id_contact &&
        //                 sales_invoice_detail.sales_invoice.status==Status.Documents_General.Approved
        //                 join sales_return_detail in _entity.sales_return_detail
        //           on sales_invoice_detail.id_sales_invoice_detail equals sales_return_detail.id_sales_invoice_detail into lst
        //                 from list in lst.DefaultIfEmpty()
        //                 group list by new
        //                 {
        //                     sales_invoice_detail = sales_invoice_detail,

        //                 }
        //                     into grouped
        //                     select new
        //                     {
        //                         id_sales_invoice = grouped.Key.sales_invoice_detail.sales_invoice.id_sales_invoice,
        //                         salesInvoice = grouped.Key.sales_invoice_detail.sales_invoice,
        //                         balance = grouped.Key.sales_invoice_detail.quantity != null ? grouped.Key.sales_invoice_detail.quantity : 0 - grouped.Sum(x => x.quantity != null ? x.quantity : 0)
        //                     }).ToList().Where(x => x.balance > 0).Select(x => x.id_sales_invoice);

        //    sales_invoiceViewSource = (CollectionViewSource)Resources["sales_invoiceViewSource"];

        //   sales_invoiceViewSource.Source = _entity.sales_invoice.Where(x => order.Contains(x.id_sales_invoice)).ToList();

        //}

        //private void loadInvoiceDetail(int id_sales_invoice, DataGridRowDetailsEventArgs e)
        //{
        //    var salesInvoice = (from sales_invoice_detail in _entity.sales_invoice_detail
        //                        where sales_invoice_detail.id_sales_invoice == id_sales_invoice
        //                        join sales_return_detail in _entity.sales_return_detail
        //                       on sales_invoice_detail.id_sales_invoice_detail equals sales_return_detail.id_sales_invoice_detail into lst
        //                        from list in lst.DefaultIfEmpty()
        //                        group list by new
        //                        {
        //                            sales_invoice_detail = sales_invoice_detail,
        //                        }
        //                            into grouped
        //                            select new
        //                            {
        //                                id = grouped.Key.sales_invoice_detail.id_sales_invoice,
        //                                item = grouped.Key.sales_invoice_detail.item.name,
        //                                quantity = grouped.Key.sales_invoice_detail.quantity > 0 ? grouped.Key.sales_invoice_detail.quantity : 0,
        //                                balance = grouped.Key.sales_invoice_detail.quantity > 0 ? grouped.Key.sales_invoice_detail.quantity : 0 - grouped.Sum(x => x.quantity > 0 ? x.quantity : 0),
        //                            }).ToList()
        //         .Where(x => x.balance > 0);
        //    Dispatcher.Invoke(new Action(() =>
        //    {
        //        System.Windows.Controls.DataGrid RowDataGrid = e.DetailsElement as System.Windows.Controls.DataGrid;
        //        RowDataGrid.ItemsSource = salesInvoice;
        //    }));

        //}
        public event btnSave_ClickedEventHandler SalesInvoice_Click;
        public delegate void btnSave_ClickedEventHandler(object sender);
        public void btnSave_MouseUp(object sender, EventArgs e)
        {

            List<sales_invoice> sales_invoice = sales_invocieDatagrid.ItemsSource.OfType<sales_invoice>().ToList();
            selected_sales_invoice = sales_invoice.Where(x => x.IsSelected == true).ToList();
         

            if (SalesInvoice_Click != null)
            {
                SalesInvoice_Click(sender);
            }
        }

        private void sales_invocieDatagrid_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            if (_entity.sales_invoice_detail.Count() > 0)
            {


                sales_invoice _sales_invoice = ((System.Windows.Controls.DataGrid)sender).SelectedItem as sales_invoice;
                int id_purchase_invoice = _sales_invoice.id_sales_invoice;
                System.Windows.Controls.DataGrid RowDataGrid = e.DetailsElement as System.Windows.Controls.DataGrid;
                var salesInvoice = _sales_invoice.sales_invoice_detail;
                RowDataGrid.ItemsSource = salesInvoice;
            }

        }

        private void ContactPref(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sbxContact.ContactID > 0)
                {
                    contact contact = _entity.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                    sales_invoiceViewSource = (CollectionViewSource)Resources["sales_invoiceViewSource"];
                    sales_invoiceViewSource.Source = _entity.sales_invoice.Where(x => x.id_contact == _contact.id_contact).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

    }
}
