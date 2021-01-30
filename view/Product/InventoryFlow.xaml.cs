using entity;
using System.Linq;
using System.Printing;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Product
{
    public partial class InventoryFlow : Page
    {
        private db db = new db();
        private CollectionViewSource item_productViewSource;

        public InventoryFlow()
        {
            InitializeComponent();
            item_productViewSource = ((CollectionViewSource)(FindResource("item_productViewSource")));

            item_productViewSource.Source = db.item_product.Where(x => x.id_company == CurrentSession.Id_Company).OrderBy(x => x.item.name).ToList();
            item_productViewSource.View.Refresh();
        }

        private void tbxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtsearch.Text != string.Empty)
            {
                if (item_productViewSource != null)
                {
                    if (item_productViewSource.View != null)
                    {
                        item_productViewSource.View.Filter = i =>
                        {
                            item_product TmpInventory = (item_product)i;
                            if (TmpInventory.item.name.ToUpper().Contains(txtsearch.Text.ToUpper()) ||
                                TmpInventory.item.code.ToUpper().Contains(txtsearch.Text.ToUpper()))
                                return true;
                            else
                                return false;
                        };
                    }
                }
            }
        }

        //First Time Selection Changed
        private void dgvItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            stackFlow.Children.Clear();
            item_product item_product = item_productViewSource.View.CurrentItem as item_product;

            if (item_product != null)
            {
                cntrl.Controls.InventoryFlowDataGrid invnetoryflow = new cntrl.Controls.InventoryFlowDataGrid();
                invnetoryflow.ParentID = null;
                invnetoryflow.ProductID = item_product.id_item_product;
                invnetoryflow.SelectionChanged += Invnetoryflow_SelectionChanged;
                stackFlow.Children.Add(invnetoryflow);
            }
        }

        private void Invnetoryflow_SelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            DataGrid SelcetedGrid = sender as DataGrid;
            cntrl.Controls.InventoryFlowDataGrid InventoryFlow = SelcetedGrid.Parent as cntrl.Controls.InventoryFlowDataGrid;

            if (InventoryFlow != null)
            {
                stackFlow.Children.RemoveRange(stackFlow.Children.IndexOf(InventoryFlow) + 1, stackFlow.Children.Count - stackFlow.Children.IndexOf(InventoryFlow));

                item_product item_product = item_productViewSource.View.CurrentItem as item_product;
                if (item_product != null)
                {
                    cntrl.Controls.InventoryFlowDataGrid _InventoryFlow = new cntrl.Controls.InventoryFlowDataGrid();
                    _InventoryFlow.ParentID = InventoryFlow.MovementID;
                    _InventoryFlow.ProductID = item_product.id_item_product;
                    _InventoryFlow.SelectionChanged += Invnetoryflow_SelectionChanged;
                    stackFlow.Children.Add(_InventoryFlow);
                }
            }
        }

       

        private void Print_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            PrintDialog dialog = new PrintDialog();
            if (dialog.ShowDialog() == true)
            {
                //Set PageOrientation to Landscape
                dialog.PrintTicket.PageOrientation = PageOrientation.Landscape;
                dialog.PrintVisual(PrintCanvas, "My Canvas");
            }
        }
    }
}