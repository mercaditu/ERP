using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for MeasurementRecords.xaml
    /// </summary>
    public partial class Measurement : Page
    {
        entity.dbContext entity = new entity.dbContext();
        CollectionViewSource measurementViewSource;
       // entity.Properties.Settings _setting = new entity.Properties.Settings();

        public Measurement()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            measurementViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("app_measurementViewSource")));
            entity.db.app_measurement.Where(a => a.id_company == CurrentSession.Id_Company).Include("app_measurement_type").Load();
            measurementViewSource.Source = entity.db.app_measurement.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
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
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.measurement objMeasurement = new cntrl.measurement();
            measurementViewSource.View.MoveCurrentTo(entity.db.app_measurement.Where(x => x.id_measurement == intMeasurementId).FirstOrDefault());
            objMeasurement.app_measurementViewSource = measurementViewSource;
            objMeasurement._entity = entity;
            crud_modal.Children.Add(objMeasurement);
        }
    }
}
