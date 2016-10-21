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
    public partial class Hr_position : Page
    {

        entity.dbContext entity = new entity.dbContext();
        CollectionViewSource hr_positionViewSource;
        entity.Properties.Settings _entity = new entity.Properties.Settings();
        public Hr_position()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            hr_positionViewSource = ((CollectionViewSource)(FindResource("hr_positionViewSource")));
            entity.db.hr_position.Where(a => a.id_company == _entity.company_ID).OrderBy(a => a.name).Load();
            hr_positionViewSource.Source = entity.db.hr_position.Local;
        }
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.hr_position hr_postion = new cntrl.hr_position();
            hr_postion.objCollectionViewSource = hr_positionViewSource;
            hr_postion.operationMode = cntrl.Class.clsCommon.Mode.Add;
            //app_account app_account = new app_account();
            //entity.db.app_account.Add(app_account);
            //account.accountobject = app_account;
            // app_accountViewSource.View.MoveCurrentToLast();
            //account.objCollectionViewSource = app_accountViewSource;
            //account.entity = entity;
            crud_modal.Children.Add(hr_postion);
        }

        private void pnl_Account_Click(object sender, int id_position)
        {
            entity.hr_position hr_position = entity.db.hr_position.Where(x => x.id_position == id_position).FirstOrDefault();
            crud_modal.Visibility = Visibility.Visible;
            cntrl.hr_position hr_postion = new cntrl.hr_position();
            hr_postion.hr_positionobject = hr_position;
            hr_postion.objCollectionViewSource = hr_positionViewSource;
            hr_postion.operationMode = cntrl.Class.clsCommon.Mode.Edit;
            //app_account app_account = new app_account();
            //entity.db.app_account.Add(app_account);
            //account.accountobject = app_account;
            // app_accountViewSource.View.MoveCurrentToLast();
            //account.objCollectionViewSource = app_accountViewSource;
            //account.entity = entity;
            crud_modal.Children.Add(hr_postion);
        }

        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            hr_positionViewSource = ((CollectionViewSource)(FindResource("hr_positionViewSource")));
            entity.db.hr_position.Where(a => a.id_company == _entity.company_ID).OrderBy(a => a.name).Load();
            hr_positionViewSource.Source = entity.db.hr_position.Local;

        }
    }
}

