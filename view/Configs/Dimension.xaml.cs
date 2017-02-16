using entity;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Configs
{
    /// <summary>
    /// Interaction logic for Dimension.xaml
    /// </summary>
    public partial class Dimension : Page
    {
        private entity.dbContext entity = new entity.dbContext();
        private CollectionViewSource dimension_viewsource;
        // entity.Properties.Settings _entity = new entity.Properties.Settings();

        public Dimension()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dimension_viewsource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("app_dimensionViewSource")));
            entity.db.app_dimension.Where(a => a.id_company == CurrentSession.Id_Company).Load();
            dimension_viewsource.Source = entity.db.app_dimension.Local;
        }

        private void pnlDimension_linkEdit_click(object sender, int intId)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.Curd.dimension objdimension = new cntrl.Curd.dimension();

            dimension_viewsource.View.MoveCurrentTo(entity.db.app_dimension.Where(x => x.id_dimension == intId).FirstOrDefault());
            objdimension.app_dimensionViewSource = dimension_viewsource;
            objdimension._entity = entity;
            crud_modal.Children.Add(objdimension);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.Curd.dimension objdimension = new cntrl.Curd.dimension();

            app_dimension app_dimension = new app_dimension();
            entity.db.app_dimension.Add(app_dimension);
            dimension_viewsource.View.MoveCurrentToLast();
            objdimension.app_dimensionViewSource = dimension_viewsource;
            objdimension._entity = entity;
            crud_modal.Children.Add(objdimension);
        }
    }
}