using cntrl;
using entity;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Configs
{
    public partial class Account : Page
    {
        private entity.dbContext entity = new entity.dbContext();
        private CollectionViewSource app_accountViewSource;

        public Account()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            app_accountViewSource = ((CollectionViewSource)(FindResource("app_accountViewSource")));
            entity.db.app_account.Where(a => a.id_company == CurrentSession.Id_Company).Include("app_account_detail").OrderBy(a => a.name).Load();
            app_accountViewSource.Source = entity.db.app_account.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            account account = new account();
            account.operationMode = cntrl.Class.clsCommon.Mode.Add;
            crud_modal.Children.Add(account);
        }

        private void pnl_Account_Click(object sender, int idAccount)
        {
            crud_modal.Visibility = Visibility.Visible;
            account account = new account();
            account.operationMode = cntrl.Class.clsCommon.Mode.Edit;
            account.accountobject = entity.db.app_account.Where(x => x.id_account == idAccount).FirstOrDefault();
            crud_modal.Children.Add(account);
        }

        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            app_accountViewSource = ((CollectionViewSource)(FindResource("app_accountViewSource")));
            entity.db.app_account.Where(a => a.id_company == CurrentSession.Id_Company && a.is_active == true).Include("app_account_detail").OrderBy(a => a.name).Load();
            app_accountViewSource.Source = entity.db.app_account.Local;
        }
    }
}