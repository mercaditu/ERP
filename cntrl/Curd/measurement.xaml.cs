using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using entity;
using System.Data.Entity.Validation;
using System.Data.Entity;

namespace cntrl
{
    public partial class measurement : UserControl
    {
        entity.dbContext mydb = new entity.dbContext();
        CollectionViewSource myViewSource = new CollectionViewSource();
        public bool isExternalCall { get; set; }

        CollectionViewSource _MainViewSource = null;
        public CollectionViewSource MainViewSource { get { return _MainViewSource; } set { _MainViewSource = value; } }

        public object curObject { get; set; }

        public Class.clsCommon.Mode operationMode { get; set; }

        private entity.app_measurement _objapp_measurement = null;
        public entity.app_measurement objapp_measurement { get { return _objapp_measurement; } set { _objapp_measurement = value; } }

        CollectionViewSource _app_measurementViewSource = null;
        public CollectionViewSource app_measurementViewSource { get { return _app_measurementViewSource; } set { _app_measurementViewSource = value; } }

        private entity.dbContext objentity = null;
        public entity.dbContext _entity { get { return objentity; } set { objentity = value; } }

        public measurement()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                try
                {
                    CollectionViewSource measurement_typeViewSource = (System.Windows.Data.CollectionViewSource)this.FindResource("app_measurement_typeViewSource");
                    measurement_typeViewSource.Source = _entity.db.app_measurement_type.OrderBy(a => a.name).ToList();

                    if (!isExternalCall)
                    {
                        stackMain.DataContext = app_measurementViewSource;
                    }
                    else
                    {
                        MainViewSource.View.MoveCurrentTo(curObject);
                        if (operationMode == Class.clsCommon.Mode.Add)
                        {
                            entity.app_measurement app_measurement = new entity.app_measurement();
                            mydb.db.app_measurement.Add(app_measurement);
                            myViewSource.Source = mydb.db.app_measurement.Local;
                            myViewSource.View.Refresh();
                            myViewSource.View.MoveCurrentTo(app_measurement);
                            stackMain.DataContext = myViewSource;
                            btnDelete.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else if (operationMode == Class.clsCommon.Mode.Edit)
                        {
                            app_measurementViewSource.View.MoveCurrentTo(objapp_measurement);
                            stackMain.DataContext = app_measurementViewSource;
                        }
                    }
                }
                catch (Exception ex) { throw ex; }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isExternalCall)
                {
                    IEnumerable<DbEntityValidationResult> validationresult = _entity.db.GetValidationErrors();
                    if (validationresult.Count() == 0)
                    {
                        _entity.db.SaveChanges();
                        btnCancel_Click(sender, e);
                    }
                }
                else
                {
                    IEnumerable<DbEntityValidationResult> validationresult = mydb.db.GetValidationErrors();
                    if (validationresult.Count() == 0)
                    {
                        if (operationMode == Class.clsCommon.Mode.Add)
                        {
                            mydb.SaveChanges();
                            entity.app_measurement app_measurement = myViewSource.View.CurrentItem as entity.app_measurement;
                            mydb.db.Entry(app_measurement).State = EntityState.Detached;
                            _entity.db.app_measurement.Attach(app_measurement);
                            app_measurementViewSource.View.Refresh();
                            app_measurementViewSource.View.MoveCurrentTo(app_measurement);
                            MainViewSource.View.Refresh();
                            MainViewSource.View.MoveCurrentTo(curObject);
                            btnCancel_Click(sender, e);
                        }
                        else if (operationMode == Class.clsCommon.Mode.Edit)
                        {
                            btnCancel_Click(sender, e);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isExternalCall)
                {
                    _entity.CancelChanges();
                    app_measurementViewSource.View.Refresh();
                }
                else
                {
                    if (operationMode == Class.clsCommon.Mode.Add)
                        mydb.CancelChanges();
                }
                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isExternalCall)
                {
                    MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (res == MessageBoxResult.Yes)
                    {
                        app_measurement app_measurement = app_measurementViewSource.View.CurrentItem as entity.app_measurement;
                        app_measurement.is_active = false;
                        btnSave_Click(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
