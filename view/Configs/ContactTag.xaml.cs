using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Data.Entity;
using entity;
using System.Data;
using System.Windows.Controls;

namespace Cognitivo.Contact
{
    public partial class ContactTag : Page
    {
        dbContext entity = new dbContext();
        CollectionViewSource contact_tagViewSource = null;
        entity.Properties.Settings _entity = new entity.Properties.Settings();

        public ContactTag()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            contact_tagViewSource = (CollectionViewSource)Resources["contact_tagViewSource"];
            entity.db.contact_tag.Where(a => a.id_company == _entity.company_ID && a.is_active == true).OrderBy(a => a.name).Load();
            contact_tagViewSource.Source = entity.db.contact_tag.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.ContactTag _ContactTag = new cntrl.Curd.ContactTag();
            contact_tag contact_tag = new contact_tag();
            entity.db.contact_tag.Add(contact_tag);
            contact_tagViewSource.View.MoveCurrentToLast();
            _ContactTag.contact_tagViewSource = contact_tagViewSource;
            _ContactTag.entity = entity;
            crud_modal.Children.Add(_ContactTag);
        }

        private void pnl_item_tag_linkEdit_Click(object sender, int idContactTag)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.ContactTag _ContactTag = new cntrl.Curd.ContactTag();
            contact_tagViewSource.View.MoveCurrentTo(entity.db.contact_tag.Where(x => x.id_tag == idContactTag).FirstOrDefault());
            _ContactTag.contact_tagViewSource = contact_tagViewSource;
            _ContactTag.entity = entity;
            crud_modal.Children.Add(_ContactTag);
        }

    }
}

