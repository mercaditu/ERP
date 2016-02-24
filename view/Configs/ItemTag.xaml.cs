using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Data.Entity;
using entity;
using System.Data;
using System.Windows.Controls;

namespace Cognitivo.Product
{
    public partial class ItemTag : Page
    {
        dbContext entity = new dbContext();
        CollectionViewSource item_tagViewSource = null;
        entity.Properties.Settings _entity = new entity.Properties.Settings();

        public ItemTag()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            item_tagViewSource = (CollectionViewSource)Resources["item_tagViewSource"];
            entity.db.item_tag.Where(a => a.id_company == _entity.company_ID && a.is_active == true).OrderBy(a => a.name).Load();
            item_tagViewSource.Source = entity.db.item_tag.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.ItemTag _ItemTag = new cntrl.Curd.ItemTag();
            item_tag item_tag = new item_tag();
            entity.db.item_tag.Add(item_tag);
            item_tagViewSource.View.MoveCurrentToLast();
            _ItemTag.item_tagViewSource = item_tagViewSource;
            _ItemTag.entity = entity;
            crud_modal.Children.Add(_ItemTag);
        }

        private void pnl_item_tag_linkEdit_Click(object sender, int idItemTag)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.ItemTag _ItemTag = new cntrl.Curd.ItemTag();
            item_tagViewSource.View.MoveCurrentTo(entity.db.item_tag.Where(x => x.id_tag == idItemTag).FirstOrDefault());
            _ItemTag.item_tagViewSource = item_tagViewSource;
            _ItemTag.entity = entity;
            crud_modal.Children.Add(_ItemTag);
        }

    }
}
