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
        private db db = new db();
        private CollectionViewSource app_accountViewSource;

        public Account()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            app_accountViewSource = FindResource("app_accountViewSource") as CollectionViewSource;
            await db.app_account.Where(a => a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).LoadAsync();
            app_accountViewSource.Source = db.app_account.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            account account = new account()
            {
                operationMode = cntrl.Class.clsCommon.Mode.Add
            };
            crud_modal.Children.Add(account);
        }

        private void pnl_Account_Click(object sender, int idAccount)
        {
            crud_modal.Visibility = Visibility.Visible;
            account account = new account()
            {
                operationMode = cntrl.Class.clsCommon.Mode.Edit,
                accountobject = db.app_account.Where(x => x.id_account == idAccount).FirstOrDefault()
            };
            crud_modal.Children.Add(account);
        }

        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Page_Loaded(null, null);
        }
    }
}