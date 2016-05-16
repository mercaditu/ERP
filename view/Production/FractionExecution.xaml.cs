using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using entity;
using System;
using System.Windows.Input;
using System.Windows.Media;

namespace Cognitivo.Production
{
    public partial class FractionExecution : Page
    {
        ExecutionDB ExecutionDB = new ExecutionDB();


        //Production EXECUTION CollectionViewSource
        CollectionViewSource
            projectViewSource,
            production_executionViewSource,
            production_execution_detailRawViewSource;

        //Production ORDER CollectionViewSource
        CollectionViewSource
            production_orderViewSource,
            production_order_detaillRawViewSource,
            item_dimensionViewSource;

        public FractionExecution()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            item_dimensionViewSource = FindResource("item_dimensionViewSource") as CollectionViewSource;
            item_dimensionViewSource.Source = ExecutionDB.item_dimension.Where(x => x.id_company == CurrentSession.Id_Company).ToList();

            production_executionViewSource = FindResource("production_executionViewSource") as CollectionViewSource;
            ExecutionDB.production_execution.Where(a => a.production_order.types == production_order.ProductionOrderTypes.Fraction && a.id_company == CurrentSession.Id_Company).Include("production_execution_detail").Load();
            production_executionViewSource.Source = ExecutionDB.production_execution.Local;


            production_execution_detailRawViewSource = FindResource("production_execution_detailRawViewSource") as CollectionViewSource;



            production_order_detaillRawViewSource = FindResource("production_order_detaillRawViewSource") as CollectionViewSource;


            production_orderViewSource = FindResource("production_orderViewSource") as CollectionViewSource;
            ExecutionDB.production_order.Where(x => x.types == production_order.ProductionOrderTypes.Fraction && x.id_company == CurrentSession.Id_Company).Load();
            production_orderViewSource.Source = ExecutionDB.production_order.Local;

            projectViewSource = FindResource("projectViewSource") as CollectionViewSource;
            ExecutionDB.projects.Where(a => a.id_company == CurrentSession.Id_Company).Load();
            projectViewSource.Source = ExecutionDB.projects.Local;

            CollectionViewSource production_lineViewSource = FindResource("production_lineViewSource") as CollectionViewSource;
            ExecutionDB.production_line.Where(x => x.id_company == CurrentSession.Id_Company).Load();
            production_lineViewSource.Source = ExecutionDB.production_line.Local;




            filter_order(production_order_detaillRawViewSource, item.item_type.RawMaterial);



            filter_execution(production_execution_detailRawViewSource, item.item_type.RawMaterial);

        }

        public void filter_execution(CollectionViewSource CollectionViewSource, item.item_type item_type)
        {
            if (CollectionViewSource != null)
            {
                if (CollectionViewSource.View != null)
                {
                    CollectionViewSource.View.Filter = i =>
                    {
                        production_execution_detail objproduction_execution_detail = (production_execution_detail)i;
                        if (objproduction_execution_detail.item != null)
                        {
                            if (objproduction_execution_detail.item.id_item_type == item_type)
                            {
                                return true;
                            }
                            else { return false; }
                        }
                        else { return false; }
                    };


                }
            }

        }

        public void filter_order(CollectionViewSource CollectionViewSource, item.item_type item_type)
        {
            int id_production_order = 0;
            if (production_executionViewSource.View.CurrentItem != null)
            {
                id_production_order = ((production_execution)production_executionViewSource.View.CurrentItem).id_production_order;
            }

            if (CollectionViewSource != null)
            {

                List<production_order_detail> _production_order_detail =
                    ExecutionDB.production_order_detail.Where(a =>
                           a.status == Status.Project.Approved
                        && (a.item.id_item_type == item_type || a.item.id_item_type == item.item_type.Task)
                        && a.id_production_order == id_production_order)
                         .ToList();

                if (_production_order_detail.Count() > 0)
                {
                    CollectionViewSource.Source = _production_order_detail;
                }
                else
                {
                    CollectionViewSource.Source = null;
                }
            }

            if (CollectionViewSource != null)
            {
                if (CollectionViewSource.View != null)
                {
                    CollectionViewSource.View.Filter = i =>
                    {
                        production_order_detail production_order_detail = (production_order_detail)i;
                        if (production_order_detail.parent == null)
                        {
                            return true;
                        }
                        else { return false; }

                    };
                }
            }
        }

        private void toolBar_btnSave_Click(object sender)
        {
            ExecutionDB.SaveChanges();
        }

        private void toolBar_btnNew_Click(object sender)
        {
            production_execution production_execution = new production_execution();
            production_execution.State = System.Data.Entity.EntityState.Added;
            production_execution.IsSelected = true;
            ExecutionDB.Entry(production_execution).State = EntityState.Added;

            production_executionViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (projectDataGrid.SelectedItem != null)
            {
                production_execution production_execution = (production_execution)projectDataGrid.SelectedItem;
                production_execution.IsSelected = true;
                production_execution.State = EntityState.Modified;
                ExecutionDB.Entry(production_execution).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select an Item");
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            ExecutionDB.CancelAllChanges();
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            if (MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question)
                            == MessageBoxResult.Yes)
            {
                ExecutionDB.production_execution.Remove((production_execution)production_executionViewSource.View.CurrentItem);
                production_executionViewSource.View.MoveCurrentToFirst();
            }

        }

        private void toolBar_btnApprove_Click(object sender)
        {
            ExecutionDB.Approve();
        }

        private void toolBar_btnAnull_Click(object sender)
        {

        }

        private void projectDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            filter_order(production_order_detaillRawViewSource, item.item_type.RawMaterial);




            filter_execution(production_execution_detailRawViewSource, item.item_type.RawMaterial);

        }

        private void dgproduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (production_execution_detailRawViewSource != null)
            {
                if (production_execution_detailRawViewSource.View != null)
                {
                    production_execution_detail obj = (production_execution_detail)production_execution_detailRawViewSource.View.CurrentItem;
                    if (obj != null)
                    {
                        if (obj.id_item != null)
                        {
                            int _id_item = (int)obj.id_item;
                            item_dimensionViewSource.View.Filter = i =>
                            {
                                item_dimension item_dimension = i as item_dimension;
                                if (item_dimension.id_item == _id_item)
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            };
                        }

                    }
                }
            }

        }

        private void treeProduct_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            production_order_detail production_order_detail = (production_order_detail)treeProduct.SelectedItem_;
            if (production_order_detail != null)
            {
                production_execution_detailRawViewSource.View.Filter = i =>
                {
                    production_execution_detail production_execution_detail = (production_execution_detail)i;
                    if (production_execution_detail.item != null)
                    {
                        if (production_execution_detail.id_order_detail == production_order_detail.id_order_detail && production_execution_detail.item.id_item_type == item.item_type.RawMaterial)
                        {
                            return true;
                        }
                        else { return false; }
                    }
                    else { return false; }
                };

                loadProductTotal(production_order_detail);

            }
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    production_executionViewSource.View.Filter = i =>
                    {
                        production_execution production_execution = i as production_execution;
                        if (production_execution.production_order.name.ToLower().Contains(query.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    };
                }
                else
                {
                    production_executionViewSource.View.Filter = null;
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }

        }

        private void txtsupplier_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button btn = new Button();
                btn.Name = "Supp";
                btnInsert_Click(btn, e);
            }

        }

        private void DeleteCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as production_execution_detail != null)
            {
                e.CanExecute = true;
            }
        }

        private void DeleteCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                DataGrid exexustiondetail = (DataGrid)e.Source;
                MessageBoxResult result = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    production_execution production_execution = production_executionViewSource.View.CurrentItem as production_execution;
                    //DeleteDetailGridRow
                    exexustiondetail.CancelEdit();
                    production_execution_detail production_execution_detail = e.Parameter as production_execution_detail;
                    production_execution_detail.State = EntityState.Deleted;
                    //production_execution.production_execution_detail.Remove(production_execution_detail);
                    ExecutionDB.production_execution_detail.Remove(production_execution_detail);

                    production_execution_detailRawViewSource.View.Refresh();
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            production_order_detail production_order_detail = null;
            Button btn = sender as Button;
            decimal Quantity = 0M;

            if (btn.Name.Contains("Prod"))
            {
                Quantity = Convert.ToDecimal(txtProduct.Text);
                production_order_detail = treeProduct.SelectedItem_ as production_order_detail;
            }

            try
            {

                if (production_order_detail != null && Quantity > 0)
                {
                    Insert_IntoDetail(production_order_detail, Quantity);

                    production_execution_detailRawViewSource.View.Refresh();
                    production_execution_detailRawViewSource.View.MoveCurrentToLast();



                    if (btn.Name.Contains("Prod"))
                    {
                        loadProductTotal(production_order_detail);
                    }
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }
        public void loadProductTotal(production_order_detail production_order_detail)
        {
            production_execution _production_execution = (production_execution)projectDataGrid.SelectedItem;
            decimal actuallqty = _production_execution.production_execution_detail.Where(x => x.item.id_item_type == item.item_type.Product && x.id_order_detail == production_order_detail.id_order_detail).Sum(x => x.quantity);
            decimal projectedqty = production_order_detail.quantity;
        }

        private void Insert_IntoDetail(production_order_detail production_order_detail, decimal Quantity)
        {
            production_execution _production_execution = (production_execution)projectDataGrid.SelectedItem;
            production_execution_detail _production_execution_detail = new entity.production_execution_detail();
            _production_execution_detail.State = EntityState.Added;
            _production_execution_detail.id_item = production_order_detail.id_item;
            _production_execution_detail.item = production_order_detail.item;
            _production_execution_detail.quantity = Quantity;
            _production_execution_detail.id_project_task = production_order_detail.id_project_task;
            _production_execution_detail.unit_cost = (decimal)production_order_detail.item.unit_cost;
            _production_execution_detail.production_execution = _production_execution;
            _production_execution_detail.id_order_detail = production_order_detail.id_order_detail;
            _production_execution_detail.is_input = false;
            _production_execution.production_execution_detail.Add(_production_execution_detail);
        }
    }
}
