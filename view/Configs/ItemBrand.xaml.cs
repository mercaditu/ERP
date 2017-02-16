using entity;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Configs
{
    public partial class ItemBrand : Page
    {
        private dbContext dbContext = new dbContext();
        private CollectionViewSource item_brandViewSource;

        public ItemBrand()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            item_brandViewSource = ((CollectionViewSource)(this.FindResource("item_brandViewSource")));
            dbContext.db.item_brand.Where(a => a.id_company == CurrentSession.Id_Company).Include(x => x.contact).OrderByDescending(a => a.name).Load();
            item_brandViewSource.Source = dbContext.db.item_brand.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.item_brand _item_brand = new cntrl.Curd.item_brand();
            item_brand item_brand = new item_brand();
            dbContext.db.item_brand.Add(item_brand);
            item_brandViewSource.View.MoveCurrentToLast();
            _item_brand.item_brandViewSource = item_brandViewSource;
            _item_brand._entity = dbContext;
            crud_modal.Children.Add(_item_brand);
        }

        private void pnl_Curd_linkEdit_click(object sender, int intId)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.Curd.item_brand _item_brand = new cntrl.Curd.item_brand();
            item_brandViewSource.View.MoveCurrentTo(dbContext.db.item_brand.Where(x => x.id_brand == intId).FirstOrDefault());
            _item_brand.item_brandViewSource = item_brandViewSource;
            _item_brand._entity = dbContext;
            crud_modal.Children.Add(_item_brand);
        }
    }
}