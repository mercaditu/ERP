using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using entity;
using System.Data;
using System;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.Generic;

namespace Cognitivo.Production
{
    public partial class Execution : Page, INotifyPropertyChanged
    {
        public bool ViewAll { get; set; }

        ExecutionDB ExecutionDB = new ExecutionDB();

        //Production EXECUTION CollectionViewSource
        CollectionViewSource project_task_dimensionViewSource, production_execution_detailViewSource;

        //Production ORDER CollectionViewSource
        CollectionViewSource
            production_orderViewSource,
            production_order_detaillViewSource,
       
            item_dimensionViewSource;

        public Execution()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            item_dimensionViewSource = FindResource("item_dimensionViewSource") as CollectionViewSource;
            item_dimensionViewSource.Source = await ExecutionDB.item_dimension.Where(x => x.id_company == CurrentSession.Id_Company).ToListAsync();

            production_execution_detailViewSource = FindResource("production_execution_detailViewSource") as CollectionViewSource;
            production_order_detaillViewSource = FindResource("production_order_detailViewSource") as CollectionViewSource;
       
            production_orderViewSource = FindResource("production_orderViewSource") as CollectionViewSource;
            await ExecutionDB.production_order.Where(x => x.id_company == CurrentSession.Id_Company && x.type != production_order.ProductionOrderTypes.Fraction).LoadAsync();
            production_orderViewSource.Source = ExecutionDB.production_order.Local;

            //projectViewSource = FindResource("projectViewSource") as CollectionViewSource;
            //ExecutionDB.projects.Where(a => a.id_company == CurrentSession.Id_Company).Load();
            //projectViewSource.Source = ExecutionDB.projects.Local;

            CollectionViewSource hr_time_coefficientViewSource = FindResource("hr_time_coefficientViewSource") as CollectionViewSource;
            await ExecutionDB.hr_time_coefficient.Where(x => x.id_company == CurrentSession.Id_Company).LoadAsync();
            hr_time_coefficientViewSource.Source = ExecutionDB.hr_time_coefficient.Local;

            CollectionViewSource app_dimensionViewSource = ((CollectionViewSource)(FindResource("app_dimensionViewSource")));
            app_dimensionViewSource.Source = await ExecutionDB.app_dimension.Where(a => a.id_company == CurrentSession.Id_Company).ToListAsync();

            CollectionViewSource app_measurementViewSource = ((CollectionViewSource)(FindResource("app_measurementViewSource")));
            app_measurementViewSource.Source = await ExecutionDB.app_measurement.Where(a => a.id_company == CurrentSession.Id_Company && a.is_active).ToListAsync();

            cmbcoefficient.SelectedIndex = -1;

            dtpenddate.Text = DateTime.Now.ToString();
            dtpstartdate.Text = DateTime.Now.ToString();
            if (!CurrentSession.User.security_role.see_cost)
            {
                btncost.Visibility = Visibility.Collapsed;
            }
            //This prevents bringing multiple
            filter_task();
            RefreshData();
        }

        private void toolBar_btnSave_Click(object sender)
        {
            ExecutionDB.SaveChanges();
        }

        private void itemserviceComboBox_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (CmbService.ContactID > 0)
            {
                contact contact = ExecutionDB.contacts.Where(x => x.id_contact == CmbService.ContactID).FirstOrDefault();
                if (contact != null)
                {
                    adddatacontact(contact, treeOrder);
                    production_order_detail production_order_detail = (production_order_detail)treeOrder.SelectedItem;
                    if (production_order_detail != null)
                    {
                        production_execution_detailViewSource.Source = ExecutionDB.production_execution_detail.Local.Where(x => x.id_order_detail == production_order_detail.id_order_detail).ToList();
                        production_execution_detailViewSource.View.Refresh();
                    }
                }
            }
        }

        public void adddatacontact(contact contact, cntrl.ExtendedTreeView treeview)
        {
            production_order_detail production_order_detail = (production_order_detail)treeview.SelectedItem_;
            if (production_order_detail != null)
            {
                if (contact != null)
                {
                    //Product
                    int id = contact.id_contact;
                    if (id > 0)
                    {
                      //  production_execution _production_execution = (production_execution)production_executionViewSource.View.CurrentItem;
                        production_execution_detail _production_execution_detail = new production_execution_detail();

                        //Check for contact
                        _production_execution_detail.id_contact = contact.id_contact;
                        _production_execution_detail.contact = contact;
                        _production_execution_detail.quantity = 1;
                        _production_execution_detail.item = production_order_detail.item;
                        _production_execution_detail.id_item = production_order_detail.item.id_item;
                        _production_execution_detail.is_input = true;
                        _production_execution_detail.name = contact.name + ": " + production_order_detail.name;

                        if (production_order_detail.id_project_task > 0)
                        {
                            _production_execution_detail.id_project_task = production_order_detail.id_project_task;
                        }

                        //Gets the Employee's contracts Hourly Rate.
                        hr_contract contract = ExecutionDB.hr_contract.Where(x => x.id_contact == id && x.is_active).FirstOrDefault();
                        if (contract != null)
                        {
                            _production_execution_detail.unit_cost = contract.Hourly;
                        }

                        if (cmbcoefficient.SelectedValue != null)
                        {
                            _production_execution_detail.id_time_coefficient = (int)cmbcoefficient.SelectedValue;
                            _production_execution_detail.hr_time_coefficient = (hr_time_coefficient)cmbcoefficient.SelectedItem;

                            if (production_order_detail.item.id_item_type == item.item_type.Service)
                            {
                                string start_date = string.Format("{0} {1}", dtpstartdate.Text, dtpstarttime.Text);
                                _production_execution_detail.start_date = Convert.ToDateTime(start_date);
                                string end_date = string.Format("{0} {1}", dtpenddate.Text, dtpendtime.Text);
                                _production_execution_detail.end_date = Convert.ToDateTime(end_date);
                            }
                            else if (production_order_detail.item.id_item_type == item.item_type.ServiceContract)
                            {
                                string start_date = string.Format("{0} {1}", dtpscstartdate.Text, dtpscstarttime.Text);
                                _production_execution_detail.start_date = Convert.ToDateTime(start_date);
                                string end_date = string.Format("{0} {1}", dtpscenddate.Text, dtpscendtime.Text);
                                _production_execution_detail.end_date = Convert.ToDateTime(end_date);
                            }

                            _production_execution_detail.id_project_task = production_order_detail.id_project_task;
                            _production_execution_detail.id_order_detail = production_order_detail.id_order_detail;
                            _production_execution_detail.production_order_detail = production_order_detail;

                            production_order_detail.production_execution_detail.Add(_production_execution_detail);
                            ExecutionDB.SaveChangesWithoutValidation();
                            RefreshData();
                        }
                    }
                }
            }
            else
            {
                toolBar.msgWarning("select Production order for insert");
            }
        }


        private void toolBar_btnNew_Click(object sender)
        {
           // production_execution production_execution =OrderDB.NewExecustion();
     
           

           // production_executionViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (projectDataGrid.SelectedItem != null)
            {
                production_order production_order = (production_order)projectDataGrid.SelectedItem;
                production_order.IsSelected = true;
                production_order.State = EntityState.Modified;
                ExecutionDB.Entry(production_order).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select an Item");
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            production_order production_order = (production_order)projectDataGrid.SelectedItem;
            production_order.State = EntityState.Unchanged;
           
        }

        private void toolBar_btnApprove_Click(object sender)
        {
            toolBar_btnSave_Click(sender);

            if (ExecutionDB.Approve(entity.production_order.ProductionOrderTypes.Production) > 0)
            {
                toolBar.msgApproved(1);
            }
        }

        private void treeProduct_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            production_order_detail production_order_detail = (production_order_detail)treeOrder.SelectedItem;
            if (production_order_detail != null)
            {
                if (production_order_detail.item.id_item_type == item.item_type.Product)
                {
                    tabProduct.IsSelected = true;
                    production_execution_detailViewSource.Source = production_order_detail.production_execution_detail.ToList();
                    txtProduct.Text = (production_order_detail.quantity).ToString();

                    //ExecutionDB.production_execution_detail.Where(x => x.id_order_detail == production_order_detail.id_order_detail).ToListAsync();
                }
                else if (production_order_detail.item.id_item_type == item.item_type.RawMaterial)
                {
                    tabRaw.IsSelected = true;
                    production_execution_detailViewSource.Source = production_order_detail.production_execution_detail.ToList();
                    txtraw.Text = (production_order_detail.quantity).ToString();

                    if (production_order_detail.project_task != null)
                    {
                        int _id_task = production_order_detail.project_task.id_project_task;
                        project_task_dimensionViewSource = (CollectionViewSource)FindResource("project_task_dimensionViewSource");
                        project_task_dimensionViewSource.Source = ExecutionDB.project_task_dimension.Where(x => x.id_project_task == _id_task).ToList();
                    }
                }
                else if (production_order_detail.item.id_item_type == item.item_type.FixedAssets)
                {
                    tabFixedAsset.IsSelected = true;
                    production_execution_detailViewSource.Source = production_order_detail.production_execution_detail.ToList();
                    txtAsset.Text = (production_order_detail.quantity).ToString();
                }
                else if (production_order_detail.item.id_item_type == item.item_type.Supplies)
                {
                    tabSupplies.IsSelected = true;
                    production_execution_detailViewSource.Source = production_order_detail.production_execution_detail.ToList();
                    txtSupply.Text = (production_order_detail.quantity).ToString();
                }
                else if (production_order_detail.item.id_item_type == item.item_type.Service)
                {
                    tabService.IsSelected = true;
                    production_execution_detailViewSource.Source = production_order_detail.production_execution_detail.ToList();
                    
                }
                else if (production_order_detail.item.id_item_type == item.item_type.ServiceContract)
                {
                    tabServiceContract.IsSelected = true;
                    production_execution_detailViewSource.Source = production_order_detail.production_execution_detail.ToList();
                    txtServicecontract.Text = (production_order_detail.quantity).ToString();
                }
                else
                {
                    tabBlank.IsSelected = true;
                }
            }

        }


        private void dgSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (production_execution_detailViewSource != null)
            {
                if (production_execution_detailViewSource.View != null)
                {
                    production_execution_detail obj = production_execution_detailViewSource.View.CurrentItem as production_execution_detail;

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

        private void dgproduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (production_execution_detailViewSource != null)
            {
                if (production_execution_detailViewSource.View != null)
                {
                    production_execution_detail obj = (production_execution_detail)production_execution_detailViewSource.View.CurrentItem;
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

        private void dgRaw_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (production_execution_detailViewSource != null)
            {
                if (production_execution_detailViewSource.View != null)
                {
                    production_execution_detail obj = (production_execution_detail)production_execution_detailViewSource.View.CurrentItem;
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

        private void dgCapital_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (production_execution_detailViewSource != null)
            {
                if (production_execution_detailViewSource.View != null)
                {
                    production_execution_detail obj = (production_execution_detail)production_execution_detailViewSource.View.CurrentItem;
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

     

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    production_orderViewSource.View.Filter = i =>
                    {
                        production_order production_order = i as production_order;
                        if (production_order.name.ToLower().Contains(query.ToLower()))
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
                    production_orderViewSource.View.Filter = null;
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }

        }

        private void btnInsert_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox tbx = sender as TextBox;
                Button btn = new Button();
                btn.Name = tbx.Name;
                btnInsert_Click(btn, e);

                //This is to clean contents after enter.
                tbx.Text = string.Empty;
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
                    production_order_detail production_order_detail = treeOrder.SelectedItem_ as production_order_detail;
                    //production_execution production_execution = production_executionViewSource.View.CurrentItem as production_execution;
                    //DeleteDetailGridRow
                    exexustiondetail.CancelEdit();
                    production_execution_detail production_execution_detail = e.Parameter as production_execution_detail;
                    production_execution_detail.State = EntityState.Deleted;
                    production_order_detail.production_execution_detail.Remove(production_execution_detail);
                    ExecutionDB.production_execution_detail.Remove(production_execution_detail);
                    ExecutionDB.SaveChanges();
                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        public void RefreshData()
        {
            try
            {
                production_orderViewSource.View.Refresh();
                production_order_detaillViewSource.View.Refresh();

                production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
                foreach (production_order_detail production_order_detail in production_order.production_order_detail)
                {
                    production_order_detail.CalcExecutedQty_TimerTaks();
                    production_order_detail.CalcExecutedCost_TimerTaks();
                   
                }
            }
            catch { }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            production_order_detail production_order_detail = null;
            Button btn = sender as Button;
            decimal Quantity = 0M;
            CollectionViewSource Collection = null; 
            item.item_type type = item.item_type.Task;
            production_order_detail = treeOrder.SelectedItem_ as production_order_detail;
            Collection = production_execution_detailViewSource;
            if (btn.Name.Contains("Prod"))
            {
                Quantity = Convert.ToDecimal(txtProduct.Text);
               
                type = item.item_type.Product;
              
            }
            else if (btn.Name.Contains("Raw"))
            {
                Quantity = Convert.ToDecimal(txtraw.Text);
              
                type = item.item_type.RawMaterial;
               
            }
            else if (btn.Name.Contains("Asset"))
            {
                Quantity = Convert.ToDecimal(txtAsset.Text);
               
                type = item.item_type.FixedAssets;
               
            }
            else if (btn.Name.Contains("Supp"))
            {
                Quantity = Convert.ToDecimal(txtSupply.Text);
                
                type = item.item_type.Supplies;
              
            }
            else if (btn.Name.Contains("ServiceContract"))
            {
                Quantity = Convert.ToDecimal(txtServicecontract.Text);
               
                type = item.item_type.ServiceContract;
               
            }

            try
            {
                if (production_order_detail.is_input)
                {
                    if (production_order_detail != null && Quantity > 0 && (
                        type == item.item_type.Product ||
                        type == item.item_type.RawMaterial ||
                        type == item.item_type.Supplies)
                        )
                    {
                        if (production_order_detail.item.item_dimension.Count() > 0)
                        {
                            Cognitivo.Configs.itemMovementFraction DimensionPanel = new Cognitivo.Configs.itemMovementFraction();
                            DimensionPanel.mode = Configs.itemMovementFraction.modes.Execution;

                           // production_execution _production_execution = production_executionViewSource.View.CurrentItem as production_execution;

                            DimensionPanel.id_item = (int)production_order_detail.id_item;
                            DimensionPanel.ExecutionDB = ExecutionDB;
                            DimensionPanel.production_order_detail = production_order_detail;
                            //DimensionPanel._production_execution = _production_execution;
                            DimensionPanel.Quantity = Quantity;

                            crud_modal.Visibility = Visibility.Visible;
                            crud_modal.Children.Add(DimensionPanel);
                        }
                        else
                        {
                            Insert_IntoDetail(production_order_detail, Quantity);
                            RefreshData();
                        }
                    }
                    else
                    {
                        Insert_IntoDetail(production_order_detail, Quantity);
                        RefreshData();
                    }
                }
                else
                {
                    Insert_IntoDetail(production_order_detail, Quantity);
                    RefreshData();
                }

                Collection.Source = ExecutionDB.production_execution_detail.Local.Where(x => x.id_order_detail == production_order_detail.id_order_detail);
                //production_order_detaillProductViewSource.View.MoveCurrentTo(production_order_detail);
               
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void Insert_IntoDetail(production_order_detail production_order_detail, decimal Quantity)
        {
           // production_execution _production_execution = (production_execution)projectDataGrid.SelectedItem;
            production_execution_detail _production_execution_detail = new entity.production_execution_detail();

            //Adds Parent so that during approval, because it is needed for approval.
            if (production_order_detail.parent != null)
            {
                if (production_order_detail.parent.production_execution_detail != null)
                {
                    _production_execution_detail.parent = production_order_detail.parent.production_execution_detail.FirstOrDefault();
                }
            }

            _production_execution_detail.State = EntityState.Added;
            _production_execution_detail.id_item = production_order_detail.id_item;
            _production_execution_detail.item = production_order_detail.item;
            _production_execution_detail.quantity = Quantity;
            _production_execution_detail.id_project_task = production_order_detail.id_project_task;
            
            if (production_order_detail.item.unit_cost != null)
            {
                _production_execution_detail.unit_cost = (decimal)production_order_detail.item.unit_cost;
            }

          //  _production_execution_detail.production_execution = _production_execution;
            _production_execution_detail.id_order_detail = production_order_detail.id_order_detail;

            if (production_order_detail.item.is_autorecepie)
            {
                _production_execution_detail.is_input = false;
            }
            else
            {
                _production_execution_detail.is_input = true;
            }
               production_order_detail.production_execution_detail.Add(_production_execution_detail);

           // ExecutionDB.SaveChanges();
        }


        private void dgServicecontract_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (production_execution_detailViewSource != null)
            {
                if (production_execution_detailViewSource.View != null)
                {
                    production_execution_detail obj = production_execution_detailViewSource.View.CurrentItem as production_execution_detail;

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

        private void CmbServicecontract_Select(object sender, RoutedEventArgs e)
        {
            if (CmbServicecontract.ContactID > 0)
            {
                contact contact = ExecutionDB.contacts.Where(x => x.id_contact == CmbServicecontract.ContactID).FirstOrDefault();
                adddatacontact(contact, treeOrder);
                production_order_detail production_order_detail = (production_order_detail)treeOrder.SelectedItem_;
                if (production_order_detail != null)
                {
                    production_execution_detailViewSource.Source = ExecutionDB.production_execution_detail.Where(x => x.id_order_detail == production_order_detail.id_order_detail).ToList();
                    //production_order_detaillProductViewSource.View.MoveCurrentTo(production_order_detail);
                }
            }
        }
        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            crud_modal.Children.Clear();
            RefreshData();
        }

        private void projectDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filter_task();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            production_order production_order = production_orderViewSource.View.CurrentItem as production_order;
            if (production_order != null)
            {
                List<production_order_detail> production_order_detailList = production_order.production_order_detail.Where(x => x.is_input).ToList();
                if (production_order_detailList.Count > 0)
                {
                    Class.CostCalculation CostCalculation = new Class.CostCalculation();
                    CostDataGrid.ItemsSource = CostCalculation.CalculateOrderCost(production_order_detailList);
                    crud_modal_cost.Visibility = Visibility.Visible;
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            crud_modal_cost.Visibility = Visibility.Collapsed;
        }


        public void filter_task()
        {
            if (production_order_detaillViewSource != null)
            {
                if (production_order_detaillViewSource.View != null)
                {
                    production_order_detaillViewSource.View.Filter = i =>
                    {
                        production_order_detail objproduction_order_detail = (production_order_detail)i;
                        if (objproduction_order_detail.parent == null)
                            return true;
                        else
                            return false;
                    };
                }
            }
        }

        private void btnExpandAll_Checked(object sender, RoutedEventArgs e)
        {
            ViewAll = !ViewAll;
            RaisePropertyChanged("ViewAll");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string prop)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
