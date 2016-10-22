using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using cntrl;
using entity;

namespace Cognitivo.Configs
{
    public partial class ItemAssetGroup : Page
    {
        entity.dbContext entity = new entity.dbContext();
        CollectionViewSource item_asset_groupViewSource;        
       // entity.Properties.Settings _entity = new entity.Properties.Settings();

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
            //app_account app_account = new app_account();
            //entity.db.app_account.Add(app_account);
            //account.accountobject = app_account;
           // app_accountViewSource.View.MoveCurrentToLast();
            //account.objCollectionViewSource = app_accountViewSource;
            //account.entity = entity;
            crud_modal.Children.Add(Item_Asset_Group);
        }

        private void pnl_Account_Click(object sender, int idAccount)
        {
            crud_modal.Visibility = Visibility.Visible;
            Item_Asset_Group Item_Asset_Group = new Item_Asset_Group();
            Item_Asset_Group.operationMode = cntrl.Class.clsCommon.Mode.Edit;
            Item_Asset_Group.item_asset_groupobject = entity.db.item_asset_group.Where(x => x.id_item_asset_group == idAccount).FirstOrDefault();
            //app_accountViewSource.View.MoveCurrentTo();
            //account.objCollectionViewSource = app_accountViewSource;
            //account.entity = entity;
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
