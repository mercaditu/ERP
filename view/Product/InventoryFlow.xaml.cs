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

namespace Cognitivo.Product
{
    /// <summary>
    /// Interaction logic for InventoryFlow.xaml
    /// </summary>
    public partial class InventoryFlow : Page
    {
        public InventoryFlow()
        {
            InitializeComponent();

            using (entity.db db = new entity.db())
            {
                //db.item_product.
            }
        }

        private void tbxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void dgvItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
