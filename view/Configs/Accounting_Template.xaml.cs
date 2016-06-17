using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;

namespace Cognitivo.Configs
{
    /// <summary>
    /// Interaction logic for Accounting_Template.xaml
    /// </summary>
    public partial class Accounting_Template : Page
    {
        entity.dbContext entity = new entity.dbContext();
        CollectionViewSource accounting_templateViewSource;
        entity.Properties.Settings _entity = new entity.Properties.Settings();
        public Accounting_Template()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            accounting_templateViewSource = ((CollectionViewSource)(FindResource("accounting_templateViewSource")));
            entity.db.accounting_template.Where(a => a.id_company == _entity.company_ID && a.is_active == true).Include("accounting_template_detail").OrderBy(a => a.name).Load();
            accounting_templateViewSource.Source = entity.db.accounting_template.Local;
        }
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.Accounting_template Accounting_template = new cntrl.Curd.Accounting_template();
            Accounting_template.operationMode = cntrl.Class.clsCommon.Mode.Add;
            //app_account app_account = new app_account();
            //entity.db.app_account.Add(app_account);
            //account.accountobject = app_account;
            // app_accountViewSource.View.MoveCurrentToLast();
            //account.objCollectionViewSource = app_accountViewSource;
            //account.entity = entity;
            crud_modal.Children.Add(Accounting_template);
        }
        private void pnl_Account_Click(object sender, int idtemplate)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.Accounting_template Accounting_template = new cntrl.Curd.Accounting_template();
            Accounting_template.operationMode = cntrl.Class.clsCommon.Mode.Edit;
            Accounting_template.accounting_templateobject = entity.db.accounting_template.Where(x => x.id_template == idtemplate).FirstOrDefault();
            //app_accountViewSource.View.MoveCurrentTo();
            //account.objCollectionViewSource = app_accountViewSource;
            //account.entity = entity;
            crud_modal.Children.Add(Accounting_template);
        }

        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            accounting_templateViewSource = ((CollectionViewSource)(FindResource("accounting_templateViewSource")));
            entity.db.accounting_template.Where(a => a.id_company == _entity.company_ID && a.is_active == true).Include("accounting_template_detail").OrderBy(a => a.name).Load();
            accounting_templateViewSource.Source = entity.db.accounting_template.Local;

        }
    }
}
