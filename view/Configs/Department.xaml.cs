using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;
using entity;

namespace Cognitivo.Configs
{
    /// <summary>
    /// Interaction logic for Department.xaml
    /// </summary>
    public partial class Department : Page
    {
        entity.dbContext entity = new entity.dbContext();
        CollectionViewSource app_departmentViewSource;
        entity.Properties.Settings _entity = new entity.Properties.Settings();

        public Department()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            app_departmentViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("app_departmentViewSource")));
            entity.db.app_department.Where(a => a.id_company == _entity.company_ID).OrderByDescending(a => a.is_active).Load();
            app_departmentViewSource.Source = entity.db.app_department.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.Curd.department _department = new cntrl.Curd.department();
            app_department app_department = new app_department();
            entity.db.app_department.Add(app_department);
            app_departmentViewSource.View.MoveCurrentToLast();
            _department.app_departmentViewSource = app_departmentViewSource;
            _department._entity = entity;
            crud_modal.Children.Add(_department);
        }

        private void pnl_Curd_linkEdit_click(object sender, int intId)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.Curd.department _department = new cntrl.Curd.department();
            app_departmentViewSource.View.MoveCurrentTo(entity.db.app_department.Single(x => x.id_department == intId));
            _department.app_departmentViewSource = app_departmentViewSource;
            _department._entity = entity;
            crud_modal.Children.Add(_department);
        }
    }
}
