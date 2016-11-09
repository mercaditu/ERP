using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using entity;

namespace Cognitivo.Product
{
    public partial class PriceList : Page
    {
        dbContext entity = new dbContext();
        CollectionViewSource item_price_listViewSource = null;

        public PriceList()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                item_price_listViewSource = (CollectionViewSource)this.FindResource("item_price_listViewSource");
                entity.db.item_price_list.Where(a=>a.id_company == CurrentSession.Id_Company).OrderByDescending(a => a.is_active).Load();
                item_price_listViewSource.Source = entity.db.item_price_list.Local;
            }
            catch { }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.price_list objPriceList = new cntrl.Curd.price_list();
            item_price_list item_price_list = new item_price_list();
            entity.db.item_price_list.Add(item_price_list);
            item_price_listViewSource.View.MoveCurrentToLast();
            objPriceList.item_price_listViewSource = item_price_listViewSource;
            objPriceList._entity = entity;
            crud_modal.Children.Add(objPriceList);
        }
        private void pnl_PriceList_Edit_Click(object sender, int intPriceListId)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.price_list objPriceList = new cntrl.Curd.price_list();
            item_price_listViewSource.View.MoveCurrentTo(entity.db.item_price_list.Where(x => x.id_price_list == intPriceListId).FirstOrDefault());
            objPriceList.item_price_listViewSource = item_price_listViewSource;
            objPriceList._entity = entity;
            crud_modal.Children.Add(objPriceList);
        }
    }
}
