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

namespace entity.Brillo.Promotion
{
    /// <summary>
    /// Interaction logic for PromotionProduct.xaml
    /// </summary>
    public partial class PromotionProduct : Window
    {
        public int ProductID { get; set; }
        public int TagID { get; set; }
        CollectionViewSource ItemViewSource;
        db db = new db();
        public PromotionProduct()
        {
            InitializeComponent();
        }
  
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            item item = ItemViewSource.View.CurrentItem as item;
            if (item!=null)
            {
                ProductID = item.id_item;
            }
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
             ItemViewSource = (System.Windows.Data.CollectionViewSource)this.FindResource("ItemViewSource");
             ItemViewSource.Source = db.items.Where(a => a.item_tag_detail.Where(x => x.id_tag == TagID).Count()>0 && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToList();
        }
    }
}
