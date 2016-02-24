using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using entity;

namespace Cognitivo.Configs
{
    /// <summary>
    /// Interaction logic for Property.xaml
    /// </summary>
    public partial class Property : Page
    {
        entity.dbContext entity = new entity.dbContext();
        CollectionViewSource app_propertyViewSource;
        public Property()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            app_propertyViewSource = ((CollectionViewSource)(this.FindResource("app_propertyViewSource")));
            entity.db.app_property.Load();
            app_propertyViewSource.Source = entity.db.app_property.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.property property = new cntrl.Curd.property();
            app_property app_property = new app_property();
            entity.db.app_property.Add(app_property);
            app_propertyViewSource.View.MoveCurrentToLast();
            property.app_propertyViewSource = app_propertyViewSource;
            property._entity = entity;
            crud_modal.Children.Add(property);
        }

        private void pnl_Curd_linkEdit_click(object sender, int intId)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.property property = new cntrl.Curd.property();
            app_propertyViewSource.View.MoveCurrentTo(entity.db.app_property.Where(x => x.id_property == intId).FirstOrDefault());
            property.app_propertyViewSource = app_propertyViewSource;
            property._entity = entity;
            crud_modal.Children.Add(property);
        }
    }
}
