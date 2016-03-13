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
    public partial class condition : UserControl
    {
        entity.dbContext mydb = new entity.dbContext();
        CollectionViewSource myViewSource = new CollectionViewSource();
        public bool isExternalCall { get; set; }

        CollectionViewSource _MainViewSource = null;
        public CollectionViewSource MainViewSource { get { return _MainViewSource; } set { _MainViewSource = value; } }
        
        public object curObject { get; set; }
        
        public Class.clsCommon.Mode operationMode { get; set; }

        private entity.app_condition _app_conditionobject = null;
        public entity.app_condition app_conditionobject { get { return _app_conditionobject; } set { _app_conditionobject = value; } }

        CollectionViewSource _conditionViewSource = null;
        public CollectionViewSource conditionViewSource { get { return _conditionViewSource; } set { _conditionViewSource = value; } }

        private entity.dbContext _entity = null;
        public entity.dbContext entity { get { return _entity; } set { _entity = value; } }

        public condition()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                if (!isExternalCall)
                {
                    stackFields.DataContext = conditionViewSource;
                }
                else
                {
                    MainViewSource.View.MoveCurrentTo(curObject);
                    if (operationMode == Class.clsCommon.Mode.Add)
                    {
                        entity.app_condition newCondition = new entity.app_condition();
                        mydb.db.app_condition.Add(newCondition);
                        myViewSource.Source = mydb.db.app_condition.Local;
                        myViewSource.View.Refresh();
                        myViewSource.View.MoveCurrentTo(newCondition);
                        stackFields.DataContext = myViewSource;
                        btnDelete.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else if (operationMode == Class.clsCommon.Mode.Edit)
                    {
                        conditionViewSource.View.MoveCurrentTo(app_conditionobject);
                        stackFields.DataContext = conditionViewSource;
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
                    IEnumerable<DbEntityValidationResult> validationresult = entity.db.GetValidationErrors();
                    if (validationresult.Count() == 0)
                    {
                        entity.db.SaveChanges();
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
                            entity.app_condition app_condition = myViewSource.View.CurrentItem as entity.app_condition;
                            mydb.db.Entry(app_condition).State = EntityState.Detached;
                            _entity.db.app_condition.Attach(app_condition);
                            conditionViewSource.View.Refresh();
                            conditionViewSource.View.MoveCurrentTo(app_condition);
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
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!isExternalCall)
            {
                MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    app_condition objCon = conditionViewSource.View.CurrentItem as app_condition;
                    objCon.is_active = false;
                    btnSave_Click(sender, e);
                }
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!isExternalCall)
                {
                    entity.CancelChanges();
                    conditionViewSource.View.Refresh();
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

        private void lblCancel_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

         
    }
}
