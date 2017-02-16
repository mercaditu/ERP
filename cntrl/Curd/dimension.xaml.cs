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
    /// Interaction logic for dimension.xaml
    /// </summary>
    public partial class dimension : UserControl
    {
        private entity.dbContext mydb = new entity.dbContext();
        private CollectionViewSource myViewSource = new CollectionViewSource();
        public bool isExternalCall { get; set; }

        private CollectionViewSource _MainViewSource = null;
        public CollectionViewSource MainViewSource { get { return _MainViewSource; } set { _MainViewSource = value; } }

        public object curObject { get; set; }

        public Class.clsCommon.Mode operationMode { get; set; }

        private entity.app_dimension _objapp_dimension = null;
        public entity.app_dimension objapp_dimension { get { return _objapp_dimension; } set { _objapp_dimension = value; } }

        private CollectionViewSource _app_dimensionViewSource = null;
        public CollectionViewSource app_dimensionViewSource { get { return _app_dimensionViewSource; } set { _app_dimensionViewSource = value; } }

        private entity.dbContext objentity = null;
        public entity.dbContext _entity { get { return objentity; } set { objentity = value; } }

        public dimension()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                if (!isExternalCall)
                {
                    stackMain.DataContext = app_dimensionViewSource;
                }
                else
                {
                    MainViewSource.View.MoveCurrentTo(curObject);
                    if (operationMode == Class.clsCommon.Mode.Add)
                    {
                        entity.app_dimension app_dimension = new entity.app_dimension();
                        mydb.db.app_dimension.Add(app_dimension);
                        myViewSource.Source = mydb.db.app_dimension.Local;
                        myViewSource.View.Refresh();
                        myViewSource.View.MoveCurrentTo(app_dimension);
                        stackMain.DataContext = myViewSource;
                    }
                    else if (operationMode == Class.clsCommon.Mode.Edit)
                    {
                        app_dimensionViewSource.View.MoveCurrentTo(objapp_dimension);
                        stackMain.DataContext = app_dimensionViewSource;
                    }
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
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
                            entity.app_dimension app_dimension = myViewSource.View.CurrentItem as entity.app_dimension;
                            mydb.db.Entry(app_dimension).State = EntityState.Detached;
                            _entity.db.app_dimension.Attach(app_dimension);
                            app_dimensionViewSource.View.Refresh();
                            app_dimensionViewSource.View.MoveCurrentTo(app_dimension);
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
                    app_dimensionViewSource.View.Refresh();
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