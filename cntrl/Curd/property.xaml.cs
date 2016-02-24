using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace cntrl.Curd
{
    /// <summary>
    /// Interaction logic for property.xaml
    /// </summary>
    public partial class property : UserControl
    {
        entity.dbContext mydb = new entity.dbContext();
        CollectionViewSource myViewSource = new CollectionViewSource();
        public bool isExternalCall { get; set; }

        CollectionViewSource _MainViewSource = null;
        public CollectionViewSource MainViewSource { get { return _MainViewSource; } set { _MainViewSource = value; } }

        public object curObject { get; set; }

        public Class.clsCommon.Mode operationMode { get; set; }

        private entity.app_property _objapp_property = null;
        public entity.app_property objapp_property { get { return _objapp_property; } set { _objapp_property = value; } }

        CollectionViewSource _app_propertyViewSource = null;
        public CollectionViewSource app_propertyViewSource { get { return _app_propertyViewSource; } set { _app_propertyViewSource = value; } }

        private entity.dbContext objentity = null;
        public entity.dbContext _entity { get { return objentity; } set { objentity = value; } }

        public property()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                if (!isExternalCall)
                {
                    stackMain.DataContext = app_propertyViewSource;
                }
                else
                {
                    MainViewSource.View.MoveCurrentTo(curObject);
                    if (operationMode == Class.clsCommon.Mode.Add)
                    {
                        entity.app_property app_property = new entity.app_property();
                        mydb.db.app_property.Add(app_property);
                        myViewSource.Source = mydb.db.app_property.Local;
                        myViewSource.View.Refresh();
                        myViewSource.View.MoveCurrentTo(app_property);
                        stackMain.DataContext = myViewSource;
                    }
                    else if (operationMode == Class.clsCommon.Mode.Edit)
                    {
                        app_propertyViewSource.View.MoveCurrentTo(objapp_property);
                        stackMain.DataContext = app_propertyViewSource;
                    }
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
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
                            entity.app_property app_property = myViewSource.View.CurrentItem as entity.app_property;
                            mydb.db.Entry(app_property).State = EntityState.Detached;
                            _entity.db.app_property.Attach(app_property);
                            app_propertyViewSource.View.Refresh();
                            app_propertyViewSource.View.MoveCurrentTo(app_property);
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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!isExternalCall)
                {
                    _entity.CancelChanges();
                    app_propertyViewSource.View.Refresh();
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
    }
}
