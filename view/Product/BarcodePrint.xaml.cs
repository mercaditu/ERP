using entity;
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
using System.Windows.Shapes;

namespace Cognitivo.Product
{
    /// <summary>
    /// Interaction logic for BarcodePrint.xaml
    /// </summary>
    public partial class BarcodePrint : Page
    {
        private CollectionViewSource inventoryViewSource;
        public BarcodePrint()
        {
            InitializeComponent();
        }
        private void Item_Select(object sender, RoutedEventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                inventoryViewSource.Source = CurrentItems.getProducts_InStock_GroupBy(CurrentSession.Id_Branch, DateTime.Now, false).Where(x => x.ItemID == sbxItem.ItemID).ToList();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            inventoryViewSource = FindResource("inventoryViewSource") as CollectionViewSource;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (inventoryViewSource.View!=null)
            {
                List<entity.Brillo.StockList> ItemMovement = inventoryViewSource.View.OfType<entity.Brillo.StockList>().Where(x => x.IsSelected).ToList();
                foreach (entity.Brillo.StockList item in ItemMovement)
                {
                    int? MovementID = item.MovementID;
                    if (MovementID != null)
                    {
                        using (db db = new db())
                        {
                            entity.Brillo.Document.Start.Automatic(db.item_movement.Find(MovementID), "MovementLabel");
                        }
                    }

                }
            }
           
        }
    }
}
