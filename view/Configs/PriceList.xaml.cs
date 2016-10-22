using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;
using entity;
using System.Data.Entity.Infrastructure;

namespace Cognitivo.Product
{
    /// <summary>
    /// Interaction logic for PriceList.xaml
    /// </summary>
    public partial class PriceList : Page
    {
        entity.dbContext entity = new entity.dbContext();
        CollectionViewSource item_price_listViewSource = null;
       // entity.Properties.Settings _entity = new entity.Properties.Settings();

        public PriceList()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                item_price_listViewSource = (System.Windows.Data.CollectionViewSource)this.FindResource("item_price_listViewSource");
                entity.db.item_price_list.Where(a=>a.id_company == CurrentSession.Id_Company).OrderByDescending(a => a.is_active).Load();
                item_price_listViewSource.Source = entity.db.item_price_list.Local;
            }
            catch { }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
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
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.Curd.price_list objPriceList = new cntrl.Curd.price_list();
            item_price_listViewSource.View.MoveCurrentTo(entity.db.item_price_list.Where(x => x.id_price_list == intPriceListId).FirstOrDefault());
            objPriceList.item_price_listViewSource = item_price_listViewSource;
            objPriceList._entity = entity;
            crud_modal.Children.Add(objPriceList);
        }
    }
}
