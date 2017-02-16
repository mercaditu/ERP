using entity;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Configs
{
    public partial class MeasurementType : Page
    {
        private dbContext entity = new dbContext();
        private CollectionViewSource measurement_typeViewSource;

        public MeasurementType()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            measurement_typeViewSource = ((CollectionViewSource)(this.FindResource("app_measurement_typeViewSource")));
            entity.db.app_measurement_type.Where(x => x.id_company == CurrentSession.Id_Company).Load();
            measurement_typeViewSource.Source = entity.db.app_measurement_type.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.measurement_type _measurement_type = new cntrl.Curd.measurement_type();
            app_measurement_type app_measurement_type = new app_measurement_type();
            entity.db.app_measurement_type.Add(app_measurement_type);
            measurement_typeViewSource.View.MoveCurrentToLast();
            _measurement_type.app_measurement_typeViewSource = measurement_typeViewSource;
            _measurement_type._entity = entity;
            crud_modal.Children.Add(_measurement_type);
        }

        private void pnl_Curd_linkEdit_click(object sender, int intId)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.measurement_type _measurement_type = new cntrl.Curd.measurement_type();
            measurement_typeViewSource.View.MoveCurrentTo(entity.db.app_measurement_type.Where(x => x.id_measurement_type == intId).FirstOrDefault());
            _measurement_type.app_measurement_typeViewSource = measurement_typeViewSource;
            _measurement_type._entity = entity;
            crud_modal.Children.Add(_measurement_type);
        }
    }
}