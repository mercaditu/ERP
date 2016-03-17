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
    public partial class Execution : Page
    {
        ExecutionDB ExecutionDB = new ExecutionDB();

        //Production EXECUTION CollectionViewSource
        CollectionViewSource
            projectViewSource,
            production_executionViewSource,
            production_execution_detailProductViewSource,
            production_execution_detailRawViewSource,
            production_execution_detailAssetViewSource,
            production_execution_detailServiceViewSource,
            production_execution_detailSupplyViewSource;

        //Production ORDER CollectionViewSource
        CollectionViewSource
            production_orderViewSource,
            production_order_detaillProductViewSource,
            production_order_detaillRawViewSource,
            production_order_detaillServiceViewSource,
            production_order_detaillAssetViewSource,
            production_order_detaillSupplyViewSource,
            item_dimensionViewSource;

        public Execution()
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
            ExecutionDB.production_execution.Where(a => a.id_company == CurrentSession.Id_Company).Include("production_execution_detail").Load();
            production_executionViewSource.Source = ExecutionDB.production_execution.Local;

            production_execution_detailProductViewSource = FindResource("production_execution_detailProductViewSource") as CollectionViewSource;
            production_execution_detailRawViewSource = FindResource("production_execution_detailRawViewSource") as CollectionViewSource;
            production_execution_detailServiceViewSource = FindResource("production_execution_detailServiceViewSource") as CollectionViewSource;
            production_execution_detailAssetViewSource = FindResource("production_execution_detailAssetViewSource") as CollectionViewSource;
            production_execution_detailSupplyViewSource = FindResource("production_execution_detailSupplyViewSource") as CollectionViewSource;

            production_order_detaillProductViewSource = FindResource("production_order_detaillProductViewSource") as CollectionViewSource;
            production_order_detaillServiceViewSource = FindResource("production_order_detaillServiceViewSource") as CollectionViewSource;
            production_order_detaillRawViewSource = FindResource("production_order_detaillRawViewSource") as CollectionViewSource;
            production_order_detaillAssetViewSource = FindResource("production_order_detaillAssetViewSource") as CollectionViewSource;
            production_order_detaillSupplyViewSource = FindResource("production_order_detaillSupplyViewSource") as CollectionViewSource;

            CollectionViewSource employeeViewSource = FindResource("employeeViewSource") as CollectionViewSource;
            ExecutionDB.contacts.Where(a => a.id_company == CurrentSession.Id_Company && a.is_employee == true).Load();
            employeeViewSource.Source = ExecutionDB.contacts.Local;

            production_orderViewSource = FindResource("production_orderViewSource") as CollectionViewSource;
            ExecutionDB.production_order.Where(x => x.id_company == CurrentSession.Id_Company).Load();
            production_orderViewSource.Source = ExecutionDB.production_order.Local;

            projectViewSource = FindResource("projectViewSource") as CollectionViewSource;
            ExecutionDB.projects.Where(a => a.id_company == CurrentSession.Id_Company).Load();
            projectViewSource.Source = ExecutionDB.projects.Local;

            CollectionViewSource production_lineViewSource = FindResource("production_lineViewSource") as CollectionViewSource;
            ExecutionDB.production_line.Where(x => x.id_company == CurrentSession.Id_Company).Load();
            production_lineViewSource.Source = ExecutionDB.production_line.Local;

            CollectionViewSource hr_time_coefficientViewSource = FindResource("hr_time_coefficientViewSource") as CollectionViewSource;
            ExecutionDB.hr_time_coefficient.Where(x => x.id_company == CurrentSession.Id_Company).Load();
            hr_time_coefficientViewSource.Source = ExecutionDB.hr_time_coefficient.Local;

            cmbcoefficient.SelectedIndex = -1;

            filter_Product();
            filter_Service();
            filter_Supply();
            filter_Raw();
            filter_Asset();
            filer_productionitem_execution();
            filer_productionraw_execution();
            filer_productionsupply_execution();
            filer_productionservice_execution();
            filer_productioncapital_execution();
            dtpenddate.Text = DateTime.Now.ToString();
            dtpstartdate.Text = DateTime.Now.ToString();
        }

        public void filer_productioncapital_execution()
        {
            if (production_execution_detailAssetViewSource != null)
            {
                if (production_execution_detailAssetViewSource.View != null)
                {
                    production_execution_detailAssetViewSource.View.Filter = i =>
                    {
                        production_execution_detail objproduction_execution_detail = (production_execution_detail)i;
                        if (objproduction_execution_detail.item != null)
                        {
                            if (objproduction_execution_detail.item.id_item_type == item.item_type.FixedAssets)
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
        public void filer_productionitem_execution()
        {
            if (production_execution_detailProductViewSource != null)
            {
                if (production_execution_detailProductViewSource.View != null)
                {


                    production_execution_detailProductViewSource.View.Filter = i =>
                    {

                        production_execution_detail objproduction_execution_detail = (production_execution_detail)i;
                        if (objproduction_execution_detail != null && objproduction_execution_detail.item != null)
                        {
                            if (objproduction_execution_detail.item.id_item_type == item.item_type.Product)
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
        public void filer_productionservice_execution()
        {
            production_order_detail production_order_detail = (production_order_detail)treeService.SelectedItem;
            if (production_execution_detailServiceViewSource != null)
            {
                if (production_execution_detailServiceViewSource.View != null)
                {
                    production_execution_detailServiceViewSource.View.Filter = i =>
                    {
                        production_execution_detail objproduction_execution_detail = (production_execution_detail)i;

                        if (objproduction_execution_detail.contact != null && production_order_detail != null)
                        {
                            return true;
                        }
                        else { return false; }
                    };
                }
            }
        }
        public void filer_productionsupply_execution()
        {
            if (production_execution_detailSupplyViewSource != null)
            {
                if (production_execution_detailSupplyViewSource.View != null)
                {
                    production_execution_detailSupplyViewSource.View.Filter = i =>
                    {
                        production_execution_detail objproduction_execution_detail = i as production_execution_detail;
                        if (objproduction_execution_detail != null)
                        {
                            if (objproduction_execution_detail.item.id_item_type == item.item_type.Supplies)
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
        public void filer_productionraw_execution()
        {
            if (production_execution_detailRawViewSource != null)
            {
                if (production_execution_detailRawViewSource.View != null)
                {
                    production_execution_detailRawViewSource.View.Filter = i =>
                    {

                        production_execution_detail objproduction_execution_detail = (production_execution_detail)i;
                        if (objproduction_execution_detail.item != null)
                        {
                            if (objproduction_execution_detail.item.id_item_type == item.item_type.RawMaterial)
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

        public void filter_Product()
        {
            int id_production_order = 0;
            if (production_executionViewSource.View.CurrentItem != null)
            {
                id_production_order = ((production_execution)production_executionViewSource.View.CurrentItem).id_production_order;
            }
            if (production_order_detaillProductViewSource != null)
            {

                List<production_order_detail> _production_order_detail =
                    ExecutionDB.production_order_detail.Where(a => a.parent == null
                         && a.status == Status.Production.Approved
                         && (a.item.id_item_type == item.item_type.Product
                         || a.item.id_item_type == item.item_type.Task)
                         && a.id_production_order == id_production_order)
                         .ToList();
                if (_production_order_detail.Count() > 0)
                {
                    production_order_detaillProductViewSource.Source = _production_order_detail;
                }
                else
                {
                    production_order_detaillProductViewSource.Source = null;
                }
            }

        }
        public void filter_Service()
        {
            int id_production = 0;
            if (production_executionViewSource.View.CurrentItem != null)
            {
                id_production = ((production_execution)production_executionViewSource.View.CurrentItem).id_production_order;
            }

            if (production_order_detaillServiceViewSource != null)
            {
                List<production_order_detail> _production_order_detail = ExecutionDB.production_order_detail.Where(a => a.parent == null && a.status == Status.Production.Approved
                         && (a.item.id_item_type == item.item_type.Service || a.item.id_item_type == item.item_type.Task) && a.id_production_order == id_production).ToList();
                if (_production_order_detail.Count() > 0)
                {
                    production_order_detaillServiceViewSource.Source = _production_order_detail;
                }
                else
                {
                    production_order_detaillServiceViewSource.Source = null;
                }
            }

        }
        public void filter_Supply()
        {
            int id_production = 0;
            if (production_executionViewSource.View.CurrentItem != null)
            {
                id_production = ((production_execution)production_executionViewSource.View.CurrentItem).id_production_order;
            }
            if (production_execution_detailSupplyViewSource != null)
            {

                List<production_order_detail> _production_order_detail = ExecutionDB.production_order_detail.Where(a => a.parent == null && a.status == Status.Production.Approved
                           && (a.item.id_item_type == item.item_type.Supplies || a.item.id_item_type == item.item_type.Task) && a.id_production_order == id_production).ToList();
                if (_production_order_detail.Count() > 0)
                {
                    production_execution_detailSupplyViewSource.Source = _production_order_detail;
                }

                else
                {
                    production_execution_detailSupplyViewSource.Source = null;

                }
            }

        }
        public void filter_Raw()
        {
            int id_production = 0;
            if (production_executionViewSource.View.CurrentItem != null)
            {
                id_production = ((production_execution)production_executionViewSource.View.CurrentItem).id_production_order;
            }
            if (production_order_detaillRawViewSource != null)
            {

                List<production_order_detail> _production_order_detail = ExecutionDB.production_order_detail.Where(a => a.parent == null && a.status == Status.Production.Approved
                            && (a.item.id_item_type == item.item_type.RawMaterial || a.item.id_item_type == item.item_type.Task) && a.id_production_order == id_production).ToList();
                if (_production_order_detail.Count() > 0)
                {
                    production_order_detaillRawViewSource.Source = _production_order_detail;
                }
                else
                {
                    production_order_detaillRawViewSource.Source = null;

                }
            }
        }
        public void filter_Asset()
        {
            int id_production = 0;
            if (production_executionViewSource.View.CurrentItem != null)
            {
                id_production = ((production_execution)production_executionViewSource.View.CurrentItem).id_production_order;
            }
            if (production_order_detaillAssetViewSource != null)
            {

                List<production_order_detail> _production_order_detail = ExecutionDB.production_order_detail.Where(a => a.parent == null
                && a.status == Status.Production.Approved
                         && (a.item.id_item_type == item.item_type.FixedAssets || a.item.id_item_type == item.item_type.Task) && a.id_production_order == id_production).ToList();
                if (_production_order_detail.Count() > 0)
                {
                    production_order_detaillAssetViewSource.Source = _production_order_detail;
                }
                else
                {
                    production_order_detaillAssetViewSource.Source = null;

                }
            }
        }

        private void toolBar_btnSave_Click(object sender)
        {
            ExecutionDB.SaveChanges();
        }

        private void itemserviceComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                adddatacontact(itemserviceComboBox, treeService);
            }
            else
            {
                itemserviceComboBox.Data = null;
            }
        }

        private void itemserviceComboBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            adddatacontact(itemserviceComboBox, treeService);
        }

        public void adddatacontact(cntrl.SearchableTextbox combo, TreeView treeview)
        {
            production_order_detail production_order_detail = (production_order_detail)treeview.SelectedItem;
            if (production_order_detail != null)
            {


                if (combo.Data != null)
                {
                    itemserviceComboBox.focusGrid = false;
                    itemserviceComboBox.Text = ((contact)combo.Data).name;
                    //Product
                    int id = Convert.ToInt32(((contact)combo.Data).id_contact);
                    if (id > 0)
                    {
                        production_execution _production_execution = (production_execution)production_executionViewSource.View.CurrentItem;

                        production_execution_detail _production_execution_detail = new entity.production_execution_detail();
                        //Check for contact

                        _production_execution_detail.id_contact = ((contact)combo.Data).id_contact;
                        _production_execution_detail.contact = (contact)combo.Data;
                        _production_execution_detail.quantity = 1;
                        _production_execution.RaisePropertyChanged("quantity");
                        if (cmbcoefficient.SelectedValue != null)
                        {
                            _production_execution_detail.id_time_coefficient = (int)cmbcoefficient.SelectedValue;
                        }
                        string start_date = dtpstartdate.Text + " " + dtpstarttime.Text;
                        _production_execution_detail.start_date = Convert.ToDateTime(start_date);
                        string end_date = dtpenddate.Text + " " + dtpendtime.Text;
                        _production_execution_detail.end_date = Convert.ToDateTime(end_date);


                        _production_execution_detail.id_production_execution = _production_execution.id_production_execution;
                        _production_execution_detail.production_execution = _production_execution;
                        _production_execution_detail.id_order_detail = production_order_detail.id_order_detail;
                        _production_execution_detail.production_order_detail = production_order_detail;

                        ExecutionDB.production_execution_detail.Add(_production_execution_detail);

                        production_execution_detailServiceViewSource.View.Refresh();
                        production_execution_detailServiceViewSource.View.MoveCurrentToLast();

                        
                        decimal actuallqty = _production_execution.production_execution_detail.Where(x => x.item.id_item_type == item.item_type.Supplies).Sum(x => x.quantity);
                        decimal projectedqty = production_order_detail.quantity;
                        lblProjectedempqty.Content = "Total:-" + projectedqty.ToString();
                        lblTotalemp.Content = "Total:-" + actuallqty.ToString();
                        if (actuallqty > projectedqty)
                        {
                            lblTotalemp.Foreground = Brushes.Red;
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
            filter_Product();
            filter_Service();
            filter_Supply();
            filter_Raw();
            filter_Asset();
            filer_productionitem_execution();
            filer_productionraw_execution();
            filer_productionservice_execution();
            filer_productionsupply_execution();
            filer_productioncapital_execution();
        }

        private void treeraw_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            production_order_detail production_order_detail = (production_order_detail)treeRaw.SelectedItem;
            if (production_order_detail != null)
            {
                production_execution_detailRawViewSource.View.Filter = i =>
                {
                    production_execution_detail production_execution_detail = (production_execution_detail)i;
                    if (production_execution_detail.id_order_detail == production_order_detail.id_order_detail && production_execution_detail.item.id_item_type == item.item_type.RawMaterial)
                    {
                        return true;
                    }
                    else { return false; }
                };
            }
        }

        private void treeservice_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            production_order_detail production_order_detail = (production_order_detail)treeService.SelectedItem;
            if (production_order_detail != null)
            {
                production_execution_detailServiceViewSource.View.Filter = i =>
                {
                    production_execution_detail production_execution_detail = (production_execution_detail)i;

                    if (production_execution_detail.id_order_detail == production_order_detail.id_order_detail)
                    {
                        return true;
                    }
                    else { return false; }
                };
            }
        }

        private void treecapital_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            production_order_detail production_order_detail = (production_order_detail)treeAsset.SelectedItem;
            if (production_order_detail != null)
            {
                production_execution_detailAssetViewSource.View.Filter = i =>
                {
                    production_execution_detail production_execution_detail = (production_execution_detail)i;
                    if (production_execution_detail.id_order_detail == production_order_detail.id_order_detail && production_execution_detail.item.id_item_type == item.item_type.FixedAssets)
                    {
                        return true;
                    }
                    else { return false; }
                };
                production_execution_detailProductViewSource.View.Filter = null;
            }
        }

        private void dgSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (production_execution_detailSupplyViewSource != null)
            {
                if (production_execution_detailSupplyViewSource.View != null)
                {
                    production_execution_detail obj = production_execution_detailSupplyViewSource.View.CurrentItem as production_execution_detail;

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
            if (production_execution_detailProductViewSource != null)
            {
                if (production_execution_detailProductViewSource.View != null)
                {
                    production_execution_detail obj = (production_execution_detail)production_execution_detailProductViewSource.View.CurrentItem;
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

        private void dgCapital_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (production_execution_detailAssetViewSource != null)
            {
                if (production_execution_detailAssetViewSource.View != null)
                {
                    production_execution_detail obj = (production_execution_detail)production_execution_detailAssetViewSource.View.CurrentItem;
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

        private void treeSupply_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            production_order_detail production_order_detail = (production_order_detail)treeSupply.SelectedItem;
            if (production_order_detail != null)
            {
                production_execution_detailSupplyViewSource.View.Filter = i =>
                {
                    production_execution_detail production_execution_detail = (production_execution_detail)i;
                    if (production_execution_detail.id_order_detail == production_order_detail.id_order_detail)
                    {
                        return true;
                    }
                    else { return false; }
                };
            }
        }

        private void treeProduct_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            production_order_detail production_order_detail = (production_order_detail)treeProduct.SelectedItem;
            if (production_order_detail != null)
            {
                production_execution_detailProductViewSource.View.Filter = i =>
                {
                    production_execution_detail production_execution_detail = (production_execution_detail)i;
                    if (production_execution_detail.item != null)
                    {
                        if (production_execution_detail.id_order_detail == production_order_detail.id_order_detail && production_execution_detail.item.id_item_type == item.item_type.Product)
                        {
                            return true;
                        }
                        else { return false; }
                    }
                    else { return false; }
                };
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
                    //production_execution.production_execution_detail.Remove(production_execution_detail);
                    ExecutionDB.production_execution_detail.Remove(production_execution_detail);
                    production_execution_detailAssetViewSource.View.Refresh();
                    production_execution_detailProductViewSource.View.Refresh();
                    production_execution_detailServiceViewSource.View.Refresh();
                    production_order_detaillAssetViewSource.View.Refresh();
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
                production_order_detail = treeProduct.SelectedItem as production_order_detail;
            }
            else if (btn.Name.Contains("Raw"))
            {
                Quantity = Convert.ToDecimal(txtRaw.Text);
                production_order_detail = treeRaw.SelectedItem as production_order_detail;
            }
            else if (btn.Name.Contains("Asset"))
            {
                Quantity = Convert.ToDecimal(txtAsset.Text);
                production_order_detail = treeAsset.SelectedItem as production_order_detail;
            }
            else if (btn.Name.Contains("Supp"))
            {
                Quantity = Convert.ToDecimal(txtSupply.Text);
                production_order_detail = treeSupply.SelectedItem as production_order_detail;
            }

            try
            {

                if (production_order_detail != null && Quantity > 0)
                {
                    Insert_IntoDetail(production_order_detail, Quantity);

                    production_execution_detailRawViewSource.View.Refresh();
                    production_execution_detailRawViewSource.View.MoveCurrentToLast();

                    production_execution_detailSupplyViewSource.View.Refresh();
                    production_execution_detailSupplyViewSource.View.MoveCurrentToLast();

                    production_execution_detailProductViewSource.View.Refresh();
                    production_execution_detailProductViewSource.View.MoveCurrentToLast();

                    production_execution_detailAssetViewSource.View.Refresh();
                    production_execution_detailAssetViewSource.View.MoveCurrentToLast();
                    if (btn.Name.Contains("Prod"))
                    {
                        production_execution _production_execution = (production_execution)projectDataGrid.SelectedItem;
                        decimal actuallqty = _production_execution.production_execution_detail.Where(x => x.item.id_item_type == item.item_type.Product).Sum(x => x.quantity);
                        decimal projectedqty = production_order_detail.quantity;
                        lblProjectedProductQty.Content = "Total:-" + projectedqty.ToString();
                        lblTotalProduct.Content = "Total:-" + actuallqty.ToString();
                        if (actuallqty > projectedqty)
                        {
                            lblTotalProduct.Foreground = Brushes.Red;
                        }
                    }
                    else if (btn.Name.Contains("Raw"))
                    {
                        production_execution _production_execution = (production_execution)projectDataGrid.SelectedItem;
                        decimal actuallqty = _production_execution.production_execution_detail.Where(x => x.item.id_item_type == item.item_type.RawMaterial).Sum(x => x.quantity);
                        decimal projectedqty = production_order_detail.quantity;
                        lblProjectedRawQty.Content = "Total:-" + projectedqty.ToString();
                        lblTotalRaw.Content = "Total:-" + actuallqty.ToString();
                        if (actuallqty > projectedqty)
                        {
                            lblTotalRaw.Foreground = Brushes.Red;
                        }
                   
                    }
                    else if (btn.Name.Contains("Asset"))
                    {
                        production_execution _production_execution = (production_execution)projectDataGrid.SelectedItem;
                        decimal actuallqty = _production_execution.production_execution_detail.Where(x => x.item.id_item_type == item.item_type.FixedAssets).Sum(x => x.quantity);
                        decimal projectedqty = production_order_detail.quantity;
                        lblProjectedassetqty.Content = "Total:-" + projectedqty.ToString();
                        lblTotalasset.Content = "Total:-" + actuallqty.ToString();
                        if (actuallqty > projectedqty)
                        {
                            lblTotalasset.Foreground = Brushes.Red;
                        }
                      
                    }
                    else if (btn.Name.Contains("Supp"))
                    {
                        production_execution _production_execution = (production_execution)projectDataGrid.SelectedItem;
                        decimal actuallqty = _production_execution.production_execution_detail.Where(x => x.item.id_item_type == item.item_type.Supplies).Sum(x => x.quantity);
                        decimal projectedqty = production_order_detail.quantity;
                        lblProjectedSuppliesqty.Content = "Total:-" + projectedqty.ToString();
                        lblTotalsupplies.Content = "Total:-" + actuallqty.ToString();
                        if (actuallqty > projectedqty)
                        {
                            lblTotalsupplies.Foreground = Brushes.Red;
                        }
                       
                    }
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void Insert_IntoDetail(production_order_detail production_order_detail, decimal Quantity)
        {
            production_execution _production_execution = (production_execution)projectDataGrid.SelectedItem;
            production_execution_detail _production_execution_detail = new entity.production_execution_detail();

            _production_execution_detail.id_item = production_order_detail.id_item;
            _production_execution_detail.item = production_order_detail.item;
            _production_execution_detail.quantity = Quantity;
            _production_execution_detail.unit_cost = (decimal)production_order_detail.item.unit_cost;
            _production_execution_detail.production_execution = _production_execution;
            _production_execution_detail.id_order_detail = production_order_detail.id_order_detail;
            _production_execution.production_execution_detail.Add(_production_execution_detail);
        }
    }
}
