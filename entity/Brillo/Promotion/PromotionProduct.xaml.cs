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
        public List<entity.Brillo.Promotion.DetailProduct> ProductList { get; set; }
        public int TagID { get; set; }
        public Decimal TotalQuantity { get; set; }

        List<entity.Brillo.Promotion.DetailProduct> TotalProduct = new List<DetailProduct>();
        db db = new db();
        public PromotionProduct()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TotalProduct.Where(x => x.Quantity > 0).Sum(x => x.Quantity) != TotalQuantity)
            {
                MessageBox.Show("Invalid quantity.. Total Quantity Is:" + TotalQuantity + " You have Selectd :-" + TotalProduct.Where(x => x.Quantity > 0).Sum(x => x.Quantity));
            }
            else
            {
                ProductList = TotalProduct.Where(x => x.Quantity > 0).ToList();
                this.Close();
            }


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IQueryable<entity.BrilloQuery.GetItem> ItemList;
        
            entity.BrilloQuery.GetItems Execute = new entity.BrilloQuery.GetItems();

            //List<item> items = db.items.Where(a => a.item_tag_detail.Where(x => x.id_tag == TagID).Count() > 0 && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToList();
            ItemList = Execute.List.AsQueryable();
            ItemList = ItemList.Where(x => x.InStock > 0).AsQueryable();
            foreach (entity.BrilloQuery.GetItem _items in ItemList)
            {
                entity.Brillo.Promotion.DetailProduct DetailProduct = new Promotion.DetailProduct();
                DetailProduct.Name = _items.Name;
                DetailProduct.Code = _items.Code;
                DetailProduct.ProductId = _items.ID;
                TotalProduct.Add(DetailProduct);
            }
            Item_detailDataGrid.ItemsSource = TotalProduct;
        }
    }
}
