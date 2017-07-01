using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using entity;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace cntrl.PanelAdv
{
    /// <summary>
    /// Interaction logic for pnlPurchasePacking.xaml
    /// </summary>
    public partial class pnlPurchasePacking : UserControl
    {
        private CollectionViewSource purchase_packingViewSource;

        private List<purchase_packing> _selected_purchase_packing = null;
        public List<purchase_packing> selected_purchase_packing { get { return _selected_purchase_packing; } set { _selected_purchase_packing = value; } }

        public contact _contact { get; set; }
        public db _entity { get; set; }
        public purchase_invoice _purchase_invoice;
        public pnlPurchasePacking()
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
                    purchase_packingViewSource = (CollectionViewSource)Resources["purchase_packingViewSource"];
                    purchase_packingViewSource.Source = _entity.purchase_packing.Where(x => x.id_contact == _contact.id_contact && x.status == Status.Documents_General.Approved).ToList();
                }
            }
        }

        public event btnSave_ClickedEventHandler Link_Click;

        public delegate void btnSave_ClickedEventHandler(object sender);

        public void btnSave_MouseUp(object sender, EventArgs e)
        {
            // List<sales_packing> sales_packing = sales_packingDataGrid.ItemsSource.OfType<sales_packing>().ToList();
            selected_purchase_packing = purchase_packingDataGrid.ItemsSource.OfType<purchase_packing>().Where(x => x.selected == true).ToList();
            if (Link_Click != null)
            {
                Link_Click(sender);
            }
        }

        private void sbxContact_Select(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sbxContact.ContactID > 0)
                {
                    contact contact = _entity.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();

                    purchase_packingViewSource = (CollectionViewSource)Resources["purchase_packingViewSource"];
                    purchase_packingViewSource.Source = _entity.sales_packing.Where(x => x.id_contact == contact.id_contact).ToList();
                }
            }
            catch
            {
                //  toolBar.msgError(ex);
            }
        }
    }
}
