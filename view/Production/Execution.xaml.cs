using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using entity;
using System;
using System.Windows.Input;

namespace Cognitivo.Production
{
    /// <summary>
    /// Interaction logic for Execution.xaml
    /// </summary>
    public partial class Execution : Page
    {
        ExecutionDB dbContext=new ExecutionDB();

        CollectionViewSource projectViewSource, production_orderViewSource, production_executionViewSource, production_execution_detailitemViewSource, production_execution_detairawlViewSource, production_execution_detailcapitalViewSource, production_execution_detailserviceViewSource, production_execution_detailsupplierViewSource;
        CollectionViewSource production_order_detaillproductViewSource, production_order_detaillrawViewSource, production_order_detaillsupplierViewSource, production_order_detaillserviceViewSource, production_order_detaillcapitalViewSource, item_dimensionViewSource;

        public Execution()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            entity.Properties.Settings _setting = new entity.Properties.Settings();

            item_dimensionViewSource = (CollectionViewSource)this.FindResource("item_dimensionViewSource");
            item_dimensionViewSource.Source = dbContext.item_dimension.ToList();

         

            production_executionViewSource = ((CollectionViewSource)(FindResource("production_executionViewSource")));
            dbContext.production_execution.Where(a => a.id_company == _setting.company_ID).Include("production_execution_detail").Load();
            production_executionViewSource.Source = dbContext.production_execution.Local;

            production_execution_detailitemViewSource = ((CollectionViewSource)(FindResource("production_execution_detailitemViewSource")));
            production_execution_detairawlViewSource = ((CollectionViewSource)(FindResource("production_execution_detairawlViewSource")));
            production_execution_detailserviceViewSource = ((CollectionViewSource)(FindResource("production_execution_detailserviceViewSource")));
            production_execution_detailcapitalViewSource = ((CollectionViewSource)(FindResource("production_execution_detailcapitalViewSource")));
            production_execution_detailsupplierViewSource = ((CollectionViewSource)(FindResource("production_execution_detailsupplierViewSource")));

            production_order_detaillproductViewSource = ((CollectionViewSource)(FindResource("production_order_detaillproductViewSource")));
            production_order_detaillserviceViewSource = ((CollectionViewSource)(FindResource("production_order_detaillserviceViewSource")));
            production_order_detaillsupplierViewSource = ((CollectionViewSource)(FindResource("production_order_detaillsupplierViewSource")));
            production_order_detaillrawViewSource = ((CollectionViewSource)(FindResource("production_order_detaillrawViewSource")));
            production_order_detaillcapitalViewSource = ((CollectionViewSource)(FindResource("production_order_detaillcapitalViewSource")));

            CollectionViewSource contactViewSource = ((CollectionViewSource)(FindResource("contactViewSource")));
            dbContext.contacts.Where(a => a.id_company == _setting.company_ID && a.is_employee == true).Load();
            contactViewSource.Source = dbContext.contacts.Local;

            CollectionViewSource contactSupplierViewSource = ((CollectionViewSource)(FindResource("contactSupplierViewSource")));
            contactSupplierViewSource.Source = dbContext.contacts.Where(a => a.id_company == _setting.company_ID && a.is_supplier == true).ToList();

            production_orderViewSource = ((CollectionViewSource)(FindResource("production_orderViewSource")));
            dbContext.production_order.Load();
            production_orderViewSource.Source = dbContext.production_order.Local;

            projectViewSource = ((CollectionViewSource)(this.FindResource("projectViewSource")));
            dbContext.projects.Where(a => a.id_company == _setting.company_ID).Load();
            projectViewSource.Source = dbContext.projects.Local;
            
            CollectionViewSource production_lineViewSource = ((CollectionViewSource)(FindResource("production_lineViewSource")));
            dbContext.production_line.Load();
            production_lineViewSource.Source = dbContext.production_line.Local;

            CollectionViewSource hr_time_coefficientViewSource = ((CollectionViewSource)(FindResource("hr_time_coefficientViewSource")));
            dbContext.hr_time_coefficient.Load();
            hr_time_coefficientViewSource.Source = dbContext.hr_time_coefficient.Local;

            cmbcoefficient.SelectedIndex = -1;
            
            filter_prodcuct();
            filter_service();
            filter_supplier();
            filter_raw();
            filter_capital();
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
            if (production_execution_detailcapitalViewSource != null)
            {
                if (production_execution_detailcapitalViewSource.View != null)
                {
                    production_execution_detailcapitalViewSource.View.Filter = i =>
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
            if (production_execution_detailitemViewSource != null)
            {
                if (production_execution_detailitemViewSource.View != null)
                {


                    production_execution_detailitemViewSource.View.Filter = i =>
                    {

                        production_execution_detail objproduction_execution_detail = (production_execution_detail)i;
                        if (objproduction_execution_detail.item != null)
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
            production_order_detail production_order_detail = (production_order_detail)treeservice.SelectedItem;
            if (production_execution_detailserviceViewSource != null)
            {
                if (production_execution_detailserviceViewSource.View != null)
                {
                    production_execution_detailserviceViewSource.View.Filter = i =>
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
            if (production_execution_detailsupplierViewSource != null)
            {
                if (production_execution_detailsupplierViewSource.View != null)
                {
                    production_execution_detailsupplierViewSource.View.Filter = i =>
                    {
                        production_execution_detail objproduction_execution_detail = (production_execution_detail)i;
                        if (objproduction_execution_detail.item != null)
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
            if (production_execution_detairawlViewSource != null)
            {
                if (production_execution_detairawlViewSource.View != null)
                {
                    production_execution_detairawlViewSource.View.Filter = i =>
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
        public void filter_prodcuct()
        {
            int id_production_order = 0;
            if (production_executionViewSource.View.CurrentItem != null)
            {
                id_production_order = ((production_execution)production_executionViewSource.View.CurrentItem).id_production_order;
            }
            if (production_order_detaillproductViewSource != null)
            {

                List<production_order_detail> _production_order_detail = 
                    dbContext.production_order_detail.Where(a => a.parent == null 
                         && a.production_order.status == Status.Production.Approved
                         && (a.item.id_item_type == item.item_type.Product 
                         || a.item.id_item_type == item.item_type.Task) 
                         && a.id_production_order == id_production_order)
                         .ToList();
                if (_production_order_detail.Count() > 0)
                {
                    production_order_detaillproductViewSource.Source = _production_order_detail;
                    //production_order_detaillserviceViewSource.View.Filter = i =>
                    //{
                    //    production_order_detail objproduction_order_detail = (production_order_detail)i;
                    //    if (objproduction_order_detail.parent == null)
                    //    {
                    //        return true;
                    //    }
                    //    else { return false; }
                    //};

                }
                else
                {
                    production_order_detaillproductViewSource.Source = null;

                }



            }

        }
        public void filter_service()
        {
            int id_production = 0;
            if (production_executionViewSource.View.CurrentItem != null)
            {
                id_production = ((production_execution)production_executionViewSource.View.CurrentItem).id_production_order;
            }
            if (production_order_detaillserviceViewSource != null)
            {

                List<production_order_detail> _production_order_detail = dbContext.production_order_detail.Where(a => a.parent == null && a.production_order.status == Status.Production.Approved
                         && (a.item.id_item_type == item.item_type.Service || a.item.id_item_type == item.item_type.Task) && a.id_production_order == id_production).ToList();
                if (_production_order_detail.Count() > 0)
                {
                    production_order_detaillserviceViewSource.Source = _production_order_detail;
                    //production_order_detaillserviceViewSource.View.Filter = i =>
                    //{
                    //    production_order_detail objproduction_order_detail = (production_order_detail)i;
                    //    if (objproduction_order_detail.parent == null)
                    //    {
                    //        return true;
                    //    }
                    //    else { return false; }
                    //};
                }

                else
                {
                    production_order_detaillserviceViewSource.Source = null;

                }
            }

        }
        public void filter_supplier()
        {
            int id_production = 0;
            if (production_executionViewSource.View.CurrentItem != null)
            {
                id_production = ((production_execution)production_executionViewSource.View.CurrentItem).id_production_order;
            }
            if (production_order_detaillsupplierViewSource != null)
            {

                List<production_order_detail> _production_order_detail = dbContext.production_order_detail.Where(a => a.parent == null && a.production_order.status == Status.Production.Approved
                           && (a.item.id_item_type == item.item_type.Supplies || a.item.id_item_type == item.item_type.Task) && a.id_production_order == id_production).ToList();
                if (_production_order_detail.Count() > 0)
                {
                    production_order_detaillsupplierViewSource.Source = _production_order_detail;
                    //production_order_detaillserviceViewSource.View.Filter = i =>
                    //{
                    //    production_order_detail objproduction_order_detail = (production_order_detail)i;
                    //    if (objproduction_order_detail.parent == null)
                    //    {
                    //        return true;
                    //    }
                    //    else { return false; }
                    //};
                }

                else
                {
                    production_order_detaillsupplierViewSource.Source = null;

                }
            }

        }
        public void filter_raw()
        {
            int id_production = 0;
            if (production_executionViewSource.View.CurrentItem != null)
            {
                id_production = ((production_execution)production_executionViewSource.View.CurrentItem).id_production_order;
            }
            if (production_order_detaillrawViewSource != null)
            {

                List<production_order_detail> _production_order_detail = dbContext.production_order_detail.Where(a => a.parent == null && a.production_order.status == Status.Production.Approved
                            && (a.item.id_item_type == item.item_type.RawMaterial || a.item.id_item_type == item.item_type.Task) && a.id_production_order == id_production).ToList();
                if (_production_order_detail.Count() > 0)
                {
                    production_order_detaillrawViewSource.Source = _production_order_detail;
                }
                else
                {
                    production_order_detaillrawViewSource.Source = null;

                }
            }
        }
        public void filter_capital()
        {
            int id_production = 0;
            if (production_executionViewSource.View.CurrentItem != null)
            {
                id_production = ((production_execution)production_executionViewSource.View.CurrentItem).id_production_order;
            }
            if (production_order_detaillcapitalViewSource != null)
            {

                List<production_order_detail> _production_order_detail = dbContext.production_order_detail.Where(a => a.parent == null
                && a.production_order.status == Status.Production.Approved
                         && (a.item.id_item_type == item.item_type.FixedAssets || a.item.id_item_type == item.item_type.Task) && a.id_production_order == id_production).ToList();
                if (_production_order_detail.Count() > 0)
                {
                    production_order_detaillcapitalViewSource.Source = _production_order_detail;
                }
                else
                {
                    production_order_detaillcapitalViewSource.Source = null;

                }
            }
        }

        public void adddataitem()
        {
            try
            {
                production_order_detail production_order_detail = (production_order_detail)treeproduct.SelectedItem;
                production_execution _production_execution = (production_execution)projectDataGrid.SelectedItem;

                if (production_order_detail != null)
                {
                    production_execution_detail _production_execution_detail = new entity.production_execution_detail();
                    //Check for contact

                    _production_execution_detail.id_item = production_order_detail.id_item;
                    _production_execution_detail.item = production_order_detail.item;
                    _production_execution_detail.quantity = Convert.ToInt32(txtitem.Text);
                    _production_execution_detail.unit_cost = (decimal)production_order_detail.item.unit_cost;
                    _production_execution_detail.production_execution = _production_execution;
                    _production_execution_detail.id_order_detail = production_order_detail.id_order_detail;
                    _production_execution.production_execution_detail.Add(_production_execution_detail);

                    production_execution_detailitemViewSource.View.Refresh();
                    production_execution_detailitemViewSource.View.MoveCurrentToLast();
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }
        public void adddatacapital()
        {
            try
            {
                production_order_detail production_order_detail = (production_order_detail)treecapital.SelectedItem;
                production_execution _production_execution = (production_execution)projectDataGrid.SelectedItem;

                if (production_order_detail != null)
                {


                    production_execution_detail _production_execution_detail = new production_execution_detail();
                    //Check for contact

                    _production_execution_detail.id_item = production_order_detail.id_item;
                    _production_execution_detail.item = production_order_detail.item;
                    _production_execution_detail.quantity = Convert.ToInt32(txtcapital.Text);
                    _production_execution_detail.unit_cost = (decimal)production_order_detail.item.unit_cost;
                    _production_execution_detail.production_execution = _production_execution;
                    _production_execution_detail.id_order_detail = production_order_detail.id_order_detail;
                    _production_execution.production_execution_detail.Add(_production_execution_detail);

                    production_execution_detailcapitalViewSource.View.Refresh();
                    production_execution_detailcapitalViewSource.View.MoveCurrentToLast();
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }
        public void adddataraw()
        {
            try
            {
                production_order_detail production_order_detail = (production_order_detail)treeraw.SelectedItem;
                production_execution _production_execution = (production_execution)projectDataGrid.SelectedItem;

                production_execution_detail _production_execution_detail = new entity.production_execution_detail();
                if (production_order_detail != null)
                {
                    _production_execution_detail.id_item = production_order_detail.id_item;
                    _production_execution_detail.item = production_order_detail.item;
                    _production_execution_detail.quantity = Convert.ToInt32(txtraw.Text);
                    _production_execution_detail.unit_cost = (decimal)production_order_detail.item.unit_cost;
                    _production_execution_detail.production_execution = _production_execution;
                    _production_execution_detail.id_order_detail = production_order_detail.id_order_detail;
                    _production_execution.production_execution_detail.Add(_production_execution_detail);

                    production_execution_detairawlViewSource.View.Refresh();
                    production_execution_detairawlViewSource.View.MoveCurrentToLast();
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }
        public void adddatasupplies()
        {
            try
            {
                production_order_detail production_order_detail = (production_order_detail)treesupplier.SelectedItem;
                production_execution _production_execution = (production_execution)projectDataGrid.SelectedItem;

                production_execution_detail _production_execution_detail = new entity.production_execution_detail();
                if (production_order_detail != null)
                {
                    _production_execution_detail.id_item = production_order_detail.id_item;
                    _production_execution_detail.item = production_order_detail.item;
                    _production_execution_detail.quantity = Convert.ToInt32(txtsupplier.Text);
                    _production_execution_detail.unit_cost = (decimal)production_order_detail.item.unit_cost;
                    _production_execution_detail.production_execution = _production_execution;
                    _production_execution_detail.id_order_detail = production_order_detail.id_order_detail;
                    _production_execution.production_execution_detail.Add(_production_execution_detail);

                    production_execution_detailsupplierViewSource.View.Refresh();
                    production_execution_detailsupplierViewSource.View.MoveCurrentToLast();
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void toolBar_btnSave_Click(object sender)
        {
            dbContext.SaveChanges();
        }

        private void itemserviceComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                adddatacontact(itemserviceComboBox, treeservice);
            }
            else
            {
                itemserviceComboBox.Data = null;
            }
        }

        private void itemserviceComboBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            adddatacontact(itemserviceComboBox,treeservice);
        }

        public void adddatacontact(cntrl.SearchableTextbox combo,TreeView treeview)
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
                             string start_date=dtpstartdate.Text +" "+ dtpstarttime.Text;
                             _production_execution_detail.start_date = Convert.ToDateTime(start_date);
                             string end_date = dtpenddate.Text + " " + dtpendtime.Text;
                             _production_execution_detail.end_date = Convert.ToDateTime(end_date);
                        
                         
                            _production_execution_detail.id_production_execution = _production_execution.id_production_execution;
                            _production_execution_detail.production_execution = _production_execution;
                            _production_execution_detail.id_order_detail = production_order_detail.id_order_detail;
                            _production_execution_detail.production_order_detail = production_order_detail;
                     
                            dbContext.production_execution_detail.Add(_production_execution_detail);

                            production_execution_detailserviceViewSource.View.Refresh();
                            production_execution_detailserviceViewSource.View.MoveCurrentToLast();
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
            dbContext.Entry(production_execution).State = EntityState.Added;

            production_executionViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (projectDataGrid.SelectedItem != null)
            {
                production_execution production_execution = (production_execution)projectDataGrid.SelectedItem;
                production_execution.IsSelected = true;
                production_execution.State = EntityState.Modified;
                dbContext.Entry(production_execution).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select an Item");
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            dbContext.CancelAllChanges();
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            if (MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question)
                            == MessageBoxResult.Yes)
            {
                dbContext.production_execution.Remove((production_execution)production_executionViewSource.View.CurrentItem);
                production_executionViewSource.View.MoveCurrentToFirst();
            }

        }

        private void toolBar_btnApprove_Click(object sender)
        {
            dbContext.Approve();
        }

        private void toolBar_btnAnull_Click(object sender)
        {

        }



        private void projectDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filter_prodcuct();
            filter_service();
            filter_supplier();
            filter_raw();
            filter_capital();
            filer_productionitem_execution();
            filer_productionraw_execution();
            filer_productionservice_execution();
            filer_productionsupply_execution();
            filer_productioncapital_execution();
        }

        private void treeraw_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            production_order_detail production_order_detail = (production_order_detail)treeraw.SelectedItem;
            if (production_order_detail != null)
            {


                production_execution_detairawlViewSource.View.Filter = i =>
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
            production_order_detail production_order_detail = (production_order_detail)treeservice.SelectedItem;
            if (production_order_detail != null)
            {
                production_execution_detailserviceViewSource.View.Filter = i =>
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
            production_order_detail production_order_detail = (production_order_detail)treecapital.SelectedItem;
            if (production_order_detail != null)
            {
                production_execution_detailcapitalViewSource.View.Filter = i =>
                {
                    production_execution_detail production_execution_detail = (production_execution_detail)i;
                    if (production_execution_detail.id_order_detail == production_order_detail.id_order_detail && production_execution_detail.item.id_item_type == item.item_type.FixedAssets)
                    {
                        return true;
                    }
                    else { return false; }
                };
                production_execution_detailitemViewSource.View.Filter = null;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                adddataitem();
            }

        }

        private void TextBox_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                adddataraw();
            }
        }

        private void TextBox_KeyDown_2(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                adddatacapital();
            }
        }

        private void dgproduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (production_execution_detailitemViewSource != null)
            {
                if (production_execution_detailitemViewSource.View != null)
                {
                    production_execution_detail obj = (production_execution_detail)production_execution_detailitemViewSource.View.CurrentItem;
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
            if (production_execution_detairawlViewSource != null)
            {
                if (production_execution_detairawlViewSource.View != null)
                {
                    production_execution_detail obj = (production_execution_detail)production_execution_detairawlViewSource.View.CurrentItem;
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
            if (production_execution_detailcapitalViewSource != null)
            {
                if (production_execution_detailcapitalViewSource.View != null)
                {
                    production_execution_detail obj = (production_execution_detail)production_execution_detailcapitalViewSource.View.CurrentItem;
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

        private void treesupplier_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            production_order_detail production_order_detail = (production_order_detail)treesupplier.SelectedItem;
            if (production_order_detail != null)
            {
                production_execution_detailsupplierViewSource.View.Filter = i =>
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

        private void treeproduct_SelectedItemChanged_1(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            production_order_detail production_order_detail = (production_order_detail)treeproduct.SelectedItem;
            if (production_order_detail != null)
            {


                production_execution_detailitemViewSource.View.Filter = i =>
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            adddataitem();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            adddataraw();

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            adddatacapital();
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

        private void dgSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (production_execution_detailsupplierViewSource != null)
            {
                if (production_execution_detailsupplierViewSource.View != null)
                {
                    production_execution_detail obj = (production_execution_detail)production_execution_detailsupplierViewSource.View.CurrentItem;
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

        private void txtsupplier_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                adddatasupplies();
            }

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            adddatasupplies();
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
                    dbContext.production_execution_detail.Remove(production_execution_detail);
                    production_execution_detailcapitalViewSource.View.Refresh();
                    production_execution_detailitemViewSource.View.Refresh();
                    production_execution_detailserviceViewSource.View.Refresh();
                    production_execution_detailsupplierViewSource.View.Refresh();
                    production_execution_detairawlViewSource.View.Refresh();
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }
    }
}
