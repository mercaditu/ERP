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
using System.Data.Entity;

namespace Cognitivo.Configs
{
    /// <summary>
    /// Interaction logic for ItemBrand.xaml
    /// </summary>
    public partial class ItemBrand : Page
    {
        entity.dbContext _entity = new entity.dbContext();
        CollectionViewSource item_brandViewSource;
        entity.Properties.Settings _settings = new entity.Properties.Settings();

        public ItemBrand()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            item_brandViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("item_brandViewSource")));
            _entity.db.item_brand.Where(a => a.id_company == _settings.company_ID).OrderByDescending(a => a.name).Load();
            item_brandViewSource.Source = _entity.db.item_brand.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.Curd.item_brand _item_brand = new cntrl.Curd.item_brand();
            item_brand item_brand = new item_brand();
            _entity.db.item_brand.Add(item_brand);
            item_brandViewSource.View.MoveCurrentToLast();
            _item_brand.item_brandViewSource = item_brandViewSource;
            _item_brand._entity = _entity;
            crud_modal.Children.Add(_item_brand);
        }

        private void pnl_Curd_linkEdit_click(object sender, int intId)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.Curd.item_brand _item_brand = new cntrl.Curd.item_brand();
            item_brandViewSource.View.MoveCurrentTo(_entity.db.item_brand.Where(x => x.id_brand == intId).FirstOrDefault());
            _item_brand.item_brandViewSource = item_brandViewSource;
            _item_brand._entity = _entity;
            crud_modal.Children.Add(_item_brand);
        }
    }
}
