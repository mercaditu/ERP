using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using entity;

namespace Cognitivo.Commercial
{

    public partial class ContactRole : Page
    {
        entity.dbContext entity = new entity.dbContext();
        CollectionViewSource contact_roleViewSource = null;
        entity.Properties.Settings _setting = new entity.Properties.Settings();

        public ContactRole()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            contact_roleViewSource = (CollectionViewSource)this.FindResource("contact_roleViewSource");
            entity.db.contact_role.Where(a => a.id_company == _setting.company_ID).OrderBy(a => a.name).Load();
            contact_roleViewSource.Source = entity.db.contact_role.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.Contact_Role objcontact_role = new cntrl.Curd.Contact_Role();
            contact_role contact_role = new contact_role();
            entity.db.contact_role.Add(contact_role);
            contact_roleViewSource.View.MoveCurrentToLast();
            objcontact_role.objCollectionViewSource = contact_roleViewSource;
            objcontact_role.entity = entity;
            crud_modal.Children.Add(objcontact_role);
        }

        private void pnl_Curd_linkEdit_click(object sender, int intId)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.Contact_Role objcontact_role = new cntrl.Curd.Contact_Role();
            contact_roleViewSource.View.MoveCurrentTo(entity.db.contact_role.Where(x => x.id_contact_role == intId).FirstOrDefault());
            objcontact_role.objCollectionViewSource = contact_roleViewSource;
            objcontact_role.entity = entity;
            crud_modal.Children.Add(objcontact_role);
        }
    }
}
