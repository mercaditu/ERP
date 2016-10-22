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
    /// Interaction logic for Geography.xaml
    /// </summary>
    public partial class Geography : Page
    {
        entity.dbContext entity = new entity.dbContext();
        CollectionViewSource app_geographyViewSource;
       // entity.Properties.Settings _entity = new entity.Properties.Settings();

        public Geography()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            app_geographyViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("app_geographyViewSource")));
            entity.db.app_geography.Where(a => a.id_company == CurrentSession.Id_Company).OrderByDescending(a => a.is_active).Load();
            app_geographyViewSource.Source = entity.db.app_geography.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.Curd.Geography _Geography = new cntrl.Curd.Geography();
            app_geography _app_geography = new app_geography();
            entity.db.app_geography.Add(_app_geography);
            app_geographyViewSource.View.MoveCurrentToLast();
            _Geography.objCollectionViewSource = app_geographyViewSource;
            _Geography.entity = entity;
            crud_modal.Children.Add(_Geography);
        }

        private void pnl_Curd_linkEdit_click(object sender, int intId)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.Curd.Geography _Geography = new cntrl.Curd.Geography();
            app_geographyViewSource.View.MoveCurrentTo(entity.db.app_geography.Single(x => x.id_geography == intId));
            _Geography.objCollectionViewSource = app_geographyViewSource;
            _Geography.entity = entity;
            crud_modal.Children.Add(_Geography);
        }
    }
}
