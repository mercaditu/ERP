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
        CollectionViewSource myViewSource = new CollectionViewSource();

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

        public contract()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                CollectionViewSource app_conditionViewSource = (System.Windows.Data.CollectionViewSource)this.FindResource("app_conditionViewSource");
                app_conditionViewSource.Source = entity.db.app_condition.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToList();

                //if (!isExternalCall)
                //{
                stackMain.DataContext = _app_contractViewSource;
                //}
                //else
                //{
                //    MainViewSource.View.MoveCurrentTo(curObject);
                //    if (operationMode == Class.clsCommon.Mode.Add)
                //    {
                //        entity.app_contract newContract = new entity.app_contract();
                //        mydb.db.app_contract.Add(newContract);
                //        myViewSource.Source = mydb.db.app_contract.Local;
                //        myViewSource.View.Refresh();
                //        myViewSource.View.MoveCurrentTo(newContract);
                //        stackMain.DataContext = myViewSource;
                //        btnDelete.Visibility = System.Windows.Visibility.Collapsed;
                //    }
                //    else if (operationMode == Class.clsCommon.Mode.Edit)
                //    {
                //        app_contractViewSource.View.MoveCurrentTo(app_contractobject);
                //        stackMain.DataContext = app_contractViewSource;
                //    }
                //}

            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                IEnumerable<DbEntityValidationResult> validationresult = entity.db.GetValidationErrors();
                if (validationresult.Count() == 0)
                {

                    app_contract app_contract = app_contractViewSource.View.CurrentItem as app_contract;

                    //Coefficient must always equal 1.
                    if (app_contract.app_contract_detail.Sum(x=> x.coefficient) != 1)
                    {
                        //Play Animation
                        return;
                    }

                    if (app_contract.is_default == true)
                    {
                        //Checks if any other Contract within same company has the same Default.
                        List<app_contract> list_app_contract = entity.db.app_contract.Where(a => a.id_contract != app_contract.id_contract && a.id_company == CurrentSession.Id_Company).ToList();
                        foreach (var item in list_app_contract)
                        {
                            item.is_default = false;
                        }
                    }

                    entity.db.SaveChanges();
                    btnCancel_Click(sender, e);
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
                entity.CancelChanges();
                app_contractViewSource.View.Refresh();

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
