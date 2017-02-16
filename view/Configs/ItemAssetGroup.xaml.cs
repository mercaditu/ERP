using cntrl;
using entity;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Configs
{
    public partial class ItemAssetGroup : Page
    {
        private dbContext entity = new dbContext();
        private CollectionViewSource item_asset_groupViewSource;

        public ItemAssetGroup()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            item_asset_groupViewSource = ((CollectionViewSource)(FindResource("item_asset_groupViewSource")));
            entity.db.item_asset_group.Where(a => a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).Load();
            item_asset_groupViewSource.Source = entity.db.item_asset_group.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            Item_Asset_Group Item_Asset_Group = new Item_Asset_Group();
            Item_Asset_Group.operationMode = cntrl.Class.clsCommon.Mode.Add;
            crud_modal.Children.Add(Item_Asset_Group);
        }

        private void pnl_Account_Click(object sender, int idAccount)
        {
            crud_modal.Visibility = Visibility.Visible;
            Item_Asset_Group Item_Asset_Group = new Item_Asset_Group();
            Item_Asset_Group.operationMode = cntrl.Class.clsCommon.Mode.Edit;
            Item_Asset_Group.item_asset_groupobject = entity.db.item_asset_group.Where(x => x.id_item_asset_group == idAccount).FirstOrDefault();
            crud_modal.Children.Add(Item_Asset_Group);
        }

        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            item_asset_groupViewSource = ((CollectionViewSource)(FindResource("item_asset_groupViewSource")));
            entity.db.item_asset_group.Where(a => a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).Load();
            item_asset_groupViewSource.Source = entity.db.item_asset_group.Local;
        }
    }
}