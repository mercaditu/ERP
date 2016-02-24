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
using System.Data.Entity.Core.Objects;
using System.Data;
using System.Globalization;
using entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

namespace Cognitivo.Configs
{
    /// <summary>
    /// Interaction logic for MeasurementType.xaml
    /// </summary>
    public partial class MeasurementType : Page
    {
        entity.dbContext entity = new entity.dbContext();
        CollectionViewSource measurement_typeViewSource;
        public MeasurementType()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            measurement_typeViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("app_measurement_typeViewSource")));
            entity.db.app_measurement_type.Load();
            measurement_typeViewSource.Source = entity.db.app_measurement_type.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
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
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.Curd.measurement_type _measurement_type = new cntrl.Curd.measurement_type();
            measurement_typeViewSource.View.MoveCurrentTo(entity.db.app_measurement_type.Where(x => x.id_measurement_type == intId).FirstOrDefault());
            _measurement_type.app_measurement_typeViewSource = measurement_typeViewSource;
            _measurement_type._entity = entity;
            crud_modal.Children.Add(_measurement_type);
        }
    }
}
