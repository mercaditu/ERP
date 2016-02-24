using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using entity;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace cntrl
{
    public partial class contract : UserControl
    {
        entity.dbContext mydb = new entity.dbContext();
        CollectionViewSource myViewSource = new CollectionViewSource();
        public bool isExternalCall { get; set; }

        CollectionViewSource _MainViewSource = null;
        public CollectionViewSource MainViewSource { get { return _MainViewSource; } set { _MainViewSource = value; } }
        public object curObject { get; set; }
        public Class.clsCommon.Mode operationMode { get; set; }

        CollectionViewSource _app_contractViewSource = null;
        public CollectionViewSource app_contractViewSource { get { return _app_contractViewSource; } set { _app_contractViewSource = value; } }

        private entity.dbContext _entity = null;
        public entity.dbContext entity { get { return _entity; } set { _entity = value; } }

        private entity.app_contract _app_contractobject = null;
        public entity.app_contract app_contractobject { get { return _app_contractobject; } set { _app_contractobject = value; } }

        entity.Properties.Settings _settings = new entity.Properties.Settings();

        public contract()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                CollectionViewSource app_conditionViewSource = (System.Windows.Data.CollectionViewSource)this.FindResource("app_conditionViewSource");
                app_conditionViewSource.Source = entity.db.app_condition.Where(a => a.is_active == true && a.id_company == _settings.company_ID).OrderBy(a => a.name).ToList();

                if (!isExternalCall)
                {
                    stackMain.DataContext = _app_contractViewSource;
                }
                else
                {
                    MainViewSource.View.MoveCurrentTo(curObject);
                    if (operationMode == Class.clsCommon.Mode.Add)
                    {


                        entity.app_contract newContract = new entity.app_contract();
                        mydb.db.app_contract.Add(newContract);
                        myViewSource.Source = mydb.db.app_contract.Local;
                        myViewSource.View.Refresh();
                        myViewSource.View.MoveCurrentTo(newContract);
                        stackMain.DataContext = myViewSource;
                        btnDelete.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else if (operationMode == Class.clsCommon.Mode.Edit)
                    {
                        app_contractViewSource.View.MoveCurrentTo(app_contractobject);
                        stackMain.DataContext = app_contractViewSource;
                    }
                }

            }
        }

        private void btnSave_Click(object sender, EventArgs e)
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
                            entity.app_contract app_contract = myViewSource.View.CurrentItem as entity.app_contract;
                            mydb.db.Entry(app_contract).State = EntityState.Detached;
                            _entity.db.app_contract.Attach(app_contract);
                            app_contractViewSource.View.Refresh();
                            app_contractViewSource.View.MoveCurrentTo(app_contract);
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
                    entity.CancelChanges();
                    app_contractViewSource.View.Refresh();
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
            MessageBoxResult res = MessageBox.Show("Are you sure want to delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                app_contract app_contract = app_contractViewSource.View.CurrentItem as entity.app_contract;
                app_contract.is_active = false;
                btnSave_Click(sender, e);
            }
        }

        private void dgvContractDetail_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            decimal coeficient = 0;
            app_contract app_contract = null;
            if (app_contractViewSource.View.CurrentItem != null)
                app_contract = app_contractViewSource.View.CurrentItem as entity.app_contract;
            else
                app_contract = myViewSource.View.CurrentItem as entity.app_contract;
            foreach (var detail in app_contract.app_contract_detail.ToList())
            {
                coeficient += detail.coefficient;
            }
            app_contract_detail contract_detail = e.NewItem as app_contract_detail;
            if (coeficient != 0)
            { contract_detail.coefficient = 1 - coeficient; }
            else
            { contract_detail.coefficient = 1; }
        }
    }
}
