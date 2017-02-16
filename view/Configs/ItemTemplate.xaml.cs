using entity;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Product
{
    public partial class ItemTemplate : Page
    {
        private dbContext entity = new dbContext();
        private CollectionViewSource item_templateViewSource = null;
        private CollectionViewSource item_templateitem_template_detailViewSource = null;

        public ItemTemplate()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            item_templateViewSource = (CollectionViewSource)Resources["item_templateViewSource"];
            entity.db.item_template.Where(a => a.id_company == CurrentSession.Id_Company && a.is_active == true).OrderBy(a => a.name).Load();
            item_templateViewSource.Source = entity.db.item_template.Local;
            item_templateitem_template_detailViewSource = (CollectionViewSource)Resources["item_templateitem_template_detailViewSource"];
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.ItemTemplate _ItemTemplate = new cntrl.Curd.ItemTemplate();
            item_template item_template = new item_template();
            item_template.name = "Template";
            entity.db.item_template.Add(item_template);

            item_templateViewSource.View.MoveCurrentToLast();
            _ItemTemplate.item_templateViewSource = item_templateViewSource;
            _ItemTemplate.item_templateDetailViewSource = item_templateitem_template_detailViewSource;
            _ItemTemplate.entity = entity;
            crud_modal.Children.Add(_ItemTemplate);
        }

        private void pnl_item_tag_linkEdit_Click(object sender, int idItemTemplate)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.ItemTemplate _ItemTemplate = new cntrl.Curd.ItemTemplate();
            item_templateViewSource.View.MoveCurrentTo(entity.db.item_template.Where(x => x.id_template == idItemTemplate).FirstOrDefault());
            _ItemTemplate.item_templateViewSource = item_templateViewSource;
            _ItemTemplate.item_templateDetailViewSource = item_templateitem_template_detailViewSource;
            _ItemTemplate.entity = entity;
            crud_modal.Children.Add(_ItemTemplate);
        }
    }
}