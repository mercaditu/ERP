using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using cntrl;
using entity;

namespace Cognitivo.Configs
{
    /// <summary>
    /// Interaction logic for hr_coefficient.xaml
    /// </summary>
    public partial class Hr_coefficient : Page
    {

        entity.dbContext entity = new entity.dbContext();
        CollectionViewSource hr_time_coefficientViewSource;
        entity.Properties.Settings _entity = new entity.Properties.Settings();
        public Hr_coefficient()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            hr_time_coefficientViewSource = ((CollectionViewSource)(FindResource("hr_time_coefficientViewSource")));
            entity.db.hr_time_coefficient.Where(a => a.id_company == _entity.company_ID).OrderBy(a => a.name).Load();
            hr_time_coefficientViewSource.Source = entity.db.hr_time_coefficient.Local;
        }
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            hr_coefficient hr_coefficient = new hr_coefficient();
            hr_coefficient.objCollectionViewSource = hr_time_coefficientViewSource;
            hr_coefficient.operationMode = cntrl.Class.clsCommon.Mode.Add;
            //app_account app_account = new app_account();
            //entity.db.app_account.Add(app_account);
            //account.accountobject = app_account;
            // app_accountViewSource.View.MoveCurrentToLast();
            //account.objCollectionViewSource = app_accountViewSource;
            //account.entity = entity;
            crud_modal.Children.Add(hr_coefficient);
        }

        private void pnl_Account_Click(object sender, int idAccount)
        {
            hr_time_coefficient hr_time_coefficient = entity.db.hr_time_coefficient.Where(x => x.id_time_coefficient == idAccount).FirstOrDefault();
            crud_modal.Visibility = Visibility.Visible;
            hr_coefficient hr_coefficient = new hr_coefficient();
            hr_coefficient.hr_time_coefficientobject = hr_time_coefficient;
            hr_coefficient.objCollectionViewSource = hr_time_coefficientViewSource;
            hr_coefficient.operationMode = cntrl.Class.clsCommon.Mode.Edit;
            //app_account app_account = new app_account();
            //entity.db.app_account.Add(app_account);
            //account.accountobject = app_account;
            // app_accountViewSource.View.MoveCurrentToLast();
            //account.objCollectionViewSource = app_accountViewSource;
            //account.entity = entity;
            crud_modal.Children.Add(hr_coefficient);
        }

        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            hr_time_coefficientViewSource = ((CollectionViewSource)(FindResource("hr_time_coefficientViewSource")));
            entity.db.hr_time_coefficient.Where(a => a.id_company == _entity.company_ID).OrderBy(a => a.name).Load();
            hr_time_coefficientViewSource.Source = entity.db.hr_time_coefficient.Local;

        }
    }
}

