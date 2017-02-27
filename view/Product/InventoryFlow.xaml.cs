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
namespace Cognitivo.Product
{
    /// <summary>
    /// Interaction logic for InventoryFlow.xaml
    /// </summary>
    public partial class InventoryFlow : Page
    {
        entity.db db = new entity.db();
        CollectionViewSource item_productViewSource;
        public InventoryFlow()
        {
            InitializeComponent();
            item_productViewSource = ((CollectionViewSource)(FindResource("item_productViewSource")));

            item_productViewSource.Source = db.item_product.Where(x => x.id_company == CurrentSession.Id_Company).ToList();

            item_productViewSource.View.Refresh();
        }

        private void tbxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void dgvItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            stackFlow.Children.Clear();
           item_product item_product = item_productViewSource.View.CurrentItem as item_product;
            if (item_product != null)
            {
                cntrl.Controls.InventoryFlowDataGrid invnetoryflow = new cntrl.Controls.InventoryFlowDataGrid(null, item_product.id_item_product);
                
                stackFlow.Children.Add(invnetoryflow);
            }
        }

        private void Invnetoryflow_Save_Click(object sender)
        {
            cntrl.Controls.InventoryFlowDataGrid invnetoryflow = sender as cntrl.Controls.InventoryFlowDataGrid;
            stackFlow.Children.RemoveRange(stackFlow.Children.IndexOf(invnetoryflow) + 1, stackFlow.Children.Count- stackFlow.Children.IndexOf(invnetoryflow));
            stackFlow.Children.Clear();
            item_product item_product = item_productViewSource.View.CurrentItem as item_product;
            if (item_product != null)
            {
                cntrl.Controls.InventoryFlowDataGrid invnetoryflownew = new cntrl.Controls.InventoryFlowDataGrid(invnetoryflow.MovementID, item_product.id_item_product);
                stackFlow.Children.Add(invnetoryflow);
            }
        }
    }
}
