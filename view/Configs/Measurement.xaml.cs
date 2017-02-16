using entity;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Configs
{
    public partial class Measurement : Page
    {
        private dbContext entity = new dbContext();
        private CollectionViewSource measurementViewSource;

        public Measurement()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            measurementViewSource = ((CollectionViewSource)(this.FindResource("app_measurementViewSource")));
            entity.db.app_measurement.Where(a => a.id_company == CurrentSession.Id_Company).Include(x => x.app_measurement_type).Load();
            measurementViewSource.Source = entity.db.app_measurement.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.measurement objMeasurement = new cntrl.measurement();
            app_measurement app_measurement = new app_measurement();
            entity.db.app_measurement.Add(app_measurement);
            measurementViewSource.View.MoveCurrentToLast();
            objMeasurement.app_measurementViewSource = measurementViewSource;
            objMeasurement._entity = entity;
            crud_modal.Children.Add(objMeasurement);
        }

        private void pnl_Measurement_linkEdit_Click(object sender, int intMeasurementId)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.measurement objMeasurement = new cntrl.measurement();
            measurementViewSource.View.MoveCurrentTo(entity.db.app_measurement.Where(x => x.id_measurement == intMeasurementId).FirstOrDefault());
            objMeasurement.app_measurementViewSource = measurementViewSource;
            objMeasurement._entity = entity;
            crud_modal.Children.Add(objMeasurement);
        }
    }
}