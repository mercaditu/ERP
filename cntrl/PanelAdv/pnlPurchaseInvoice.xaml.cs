using entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace cntrl.PanelAdv
{
    public partial class pnlPurchaseInvoice : UserControl
    {
        private CollectionViewSource purchase_invoiceViewSource;

        private List<purchase_invoice> _selected_purchase_invoice = new List<purchase_invoice>();
        public List<purchase_invoice> selected_purchase_invoice { get { return _selected_purchase_invoice; } set { _selected_purchase_invoice = value; } }

        public ImpexDB _entity { get; set; }
        public contact _contact { get; set; }

        public bool IsImpex { get; set; }

        public pnlPurchaseInvoice()
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
                    Get_PurchaseInvoice(_contact);
                }
            }
        }

        private void Get_PurchaseInvoice(contact _contact)
        {
            purchase_invoiceViewSource = Resources["purchase_invoiceViewSource"] as CollectionViewSource;

            if (IsImpex == true)
            {
                //Only bring Purchase invoice that are Active and that have not been already linked to a previous Impex.
                List<purchase_invoice> PurchaseList = _entity.purchase_invoice.Where(x =>
                x.id_contact == _contact.id_contact
                && x.is_impex
                && x.status == Status.Documents_General.Approved
                && x.impex_import.Count() == 0
                ).ToList();
                purchase_invoiceViewSource.Source = PurchaseList;
            }
            else
            {
                //Only bring Purchase invoice that are Active
                purchase_invoiceViewSource.Source = _entity.purchase_invoice.Where(x => x.id_contact == _contact.id_contact && x.status == Status.Documents_General.Approved).ToList();
            }
        }

        public event btnSave_ClickedEventHandler PurchaseInvoice_Click;

        public delegate void btnSave_ClickedEventHandler(object sender);

        public void btnSave_MouseUp(object sender, EventArgs e)
        {
            selected_purchase_invoice = null;
            List<purchase_invoice> purchase_invoice = purchase_invoiceDatagrid.ItemsSource.OfType<purchase_invoice>().ToList();
            selected_purchase_invoice = purchase_invoice.Where(x => x.IsSelected == true).ToList();

            PurchaseInvoice_Click?.Invoke(sender);
        }

        private void sales_invoiceDatagrid_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            if (_entity.purchase_invoice_detail.Count() > 0)
            {
                purchase_invoice _purchase_invoice = ((DataGrid)sender).SelectedItem as purchase_invoice;
                int id_purchase_invoice = _purchase_invoice.id_purchase_invoice;
                Grid Grid = e.DetailsElement as Grid;
                var purchaseInvoice = _purchase_invoice.purchase_invoice_detail;

                if (Grid != null)
                {
                    Grid.DataContext = purchaseInvoice;
                }
            }
        }

        private void ContactPref(object sender, RoutedEventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                _contact = _entity.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();

                if (_contact != null)
                {
                    Get_PurchaseInvoice(_contact);
                }
            }
        }

        private void ToggleSwitch_IsCheckedChanged(object sender, EventArgs e)
        {
            MahApps.Metro.Controls.ToggleSwitch ToggleSwitch = sender as MahApps.Metro.Controls.ToggleSwitch;

            if (ToggleSwitch.IsChecked == true)
            {
                purchase_invoiceDatagrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
            }
            else
            {
                purchase_invoiceDatagrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
            }
        }
    }
}