using entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace cntrl.Curd
{
    /// <summary>
    /// Interaction logic for vat_group.xaml
    /// </summary>
    public partial class vat_group : UserControl
    {
        private entity.dbContext mydb = new entity.dbContext();
        private CollectionViewSource myViewSource = new CollectionViewSource();
        public bool isExternalCall { get; set; }

        private CollectionViewSource _MainViewSource = null;
        public CollectionViewSource MainViewSource { get { return _MainViewSource; } set { _MainViewSource = value; } }
        public object curObject { get; set; }
        public Class.clsCommon.Mode operationMode { get; set; }

        private CollectionViewSource _app_vat_groupViewSource, app_vatViewSource = null;
        public CollectionViewSource app_vat_groupViewSource { get { return _app_vat_groupViewSource; } set { _app_vat_groupViewSource = value; } }

        private entity.dbContext objentity = null;
        public entity.dbContext _entity { get { return objentity; } set { objentity = value; } }

        private entity.app_vat_group _vat_groupObject = null;
        public entity.app_vat_group vat_groupObject { get { return _vat_groupObject; } set { _vat_groupObject = value; } }

        public vat_group()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                {
                    app_vatViewSource = this.FindResource("app_vatViewSource") as CollectionViewSource;
                    app_vatViewSource.Source = _entity.db.app_vat.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToList();

                    if (!isExternalCall)
                    {
                        stackMain.DataContext = app_vat_groupViewSource;
                    }
                    else
                    {
                        MainViewSource.View.MoveCurrentTo(curObject);
                        if (operationMode == Class.clsCommon.Mode.Add)
                        {
                            entity.app_vat_group newvat_group = new entity.app_vat_group();
                            mydb.db.app_vat_group.Add(newvat_group);
                            myViewSource.Source = mydb.db.app_vat_group.Local;
                            myViewSource.View.Refresh();
                            myViewSource.View.MoveCurrentTo(newvat_group);
                            stackMain.DataContext = myViewSource;
                        }
                        else if (operationMode == Class.clsCommon.Mode.Edit)
                        {
                            app_vat_groupViewSource.View.MoveCurrentTo(vat_groupObject);
                            stackMain.DataContext = app_vat_groupViewSource;
                        }
                    }
                }
            }
            catch
            { }
        }

        private void btnCancel_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (!isExternalCall)
                {
                    _entity.CancelChanges();
                    app_vat_groupViewSource.View.Refresh();
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
            catch
            {
                //throw ex;
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
                        entity.app_vat_group app_vat_group = app_vat_groupViewSource.View.CurrentItem as entity.app_vat_group;
                        app_vat_group.timestamp = DateTime.Now;
                        _entity.SaveChanges();
                        btnCancel_Click(sender, null);
                    }
                }
                else
                {
                    IEnumerable<DbEntityValidationResult> validationresult = mydb.db.GetValidationErrors();
                    if (validationresult.Count() == 0)
                    {
                        if (operationMode == Class.clsCommon.Mode.Add)
                        {
                            entity.app_vat_group app_vat_group = myViewSource.View.CurrentItem as entity.app_vat_group;
                            app_vat_group.timestamp = DateTime.Now;
                            mydb.SaveChanges();

                            mydb.db.Entry(app_vat_group).State = EntityState.Detached;
                            _entity.db.app_vat_group.Attach(app_vat_group);
                            app_vat_groupViewSource.View.Refresh();
                            app_vat_groupViewSource.View.MoveCurrentTo(app_vat_group);
                            MainViewSource.View.Refresh();
                            MainViewSource.View.MoveCurrentTo(curObject);
                            btnCancel_Click(sender, null);
                        }
                        else if (operationMode == Class.clsCommon.Mode.Edit)
                        {
                            btnCancel_Click(sender, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!isExternalCall)
                {
                    MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (res == MessageBoxResult.Yes)
                    {
                        app_vat_group app_vat_group = app_vat_groupViewSource.View.CurrentItem as app_vat_group;
                        app_vat_group.is_active = false;
                        btnSave_Click(sender, e);
                    }
                }
            }
            catch
            {
                //throw ex;
            }
        }
    }
}