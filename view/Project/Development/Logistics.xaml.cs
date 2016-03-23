using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Data.Entity;
using entity;
using Microsoft.Windows.Controls.Primitives;
using System.Windows.Media;
using System.ComponentModel;

namespace Cognitivo.Project.Development
{
    public partial class Logistics : Page, INotifyPropertyChanged
    {
        public bool is_select { get; set; }
        entity.dbContext entity = new dbContext();
        CollectionViewSource projectViewSource, contractViewSource;
        entity.Properties.Settings _entity = new entity.Properties.Settings();
        cntrl.Curd.ItemRequest ItemRequest;
        //SetIsEnableProperty
        public static readonly DependencyProperty SetIsEnableProperty =
            DependencyProperty.Register("SetIsEnable", typeof(bool), typeof(Logistics),
            new FrameworkPropertyMetadata(false));
        public int noofrows { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
        public Logistics()
        {
            InitializeComponent();
            is_select = false;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            projectViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("projectViewSource")));
            entity.db.projects.Where(a => a.is_active && a.id_company == _entity.company_ID).Load();
            projectViewSource.Source = entity.db.projects.Local;



            contractViewSource = (CollectionViewSource)this.FindResource("contractViewSource");
            contractViewSource.Source = entity.db.app_contract.Where(a => a.is_active == true && a.id_company == _entity.company_ID).ToList();

            CollectionViewSource conditionViewSource = (CollectionViewSource)this.FindResource("conditionViewSource");
            conditionViewSource.Source = entity.db.app_condition.Where(a => a.is_active == true && a.id_company == _entity.company_ID).OrderBy(a => a.name).ToList();

            CollectionViewSource app_dimensionViewSource = ((CollectionViewSource)(FindResource("app_dimensionViewSource")));
            entity.db.app_dimension.Where(a => a.id_company == _entity.company_ID).Load();
            app_dimensionViewSource.Source = entity.db.app_dimension.Local;

            CollectionViewSource app_measurementViewSource = ((CollectionViewSource)(FindResource("app_measurementViewSource")));
            entity.db.app_measurement.Where(a => a.id_company == _entity.company_ID).Load();
            app_measurementViewSource.Source = entity.db.app_measurement.Local;
        }


        private void projectDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }
        public void LoadData()
        {
            try
            {
                if (projectViewSource != null)
                {
                    item_ProductDataGrid.ItemsSource = null;
                    item_RawDataGrid.ItemsSource = null;
                    item_ServiceDataGrid.ItemsSource = null;
                    item_CapitalDataGrid.ItemsSource = null;
                    dgvSupplies.ItemsSource = null;

                    int _id_project = 0;
                    _id_project = ((project)projectViewSource.View.CurrentItem).id_project;

                    if (_id_project > 0)
                    {
                        var productlistbasic = (from IT in entity.db.project_task
                                                where (IT.status == Status.Project.Approved)
                                                && IT.status != null && IT.id_project == _id_project
                                                join IK in entity.db.item_product on IT.id_item equals IK.id_item
                                                join IO in entity.db.item_movement on IK.id_item_product equals IO.id_item_product into a
                                                from IM in a.DefaultIfEmpty()
                                                group IT by new { IM, IT.items }
                                                    into last
                                                    select new
                                                    {
                                                        _id_item = last.Key.items.id_item != 0 ? last.Key.items.id_item : 0,
                                                        _code = last.Key.items != null ? last.Key.items.code : "",
                                                        _name = last.Key.items != null ? last.Key.items.name : "",
                                                        _id_task = last.Max(x => x.id_project_task),
                                                        _ordered_quantity = last.Sum(x => x.quantity_est) != 0 ? last.Sum(x => x.quantity_est) : 0,
                                                        avlqtyColumn = last.Key.IM.credit != null ? last.Key.IM.credit : 0 - last.Key.IM.debit != null ? last.Key.IM.debit : 0,
                                                        buyqty = (last.Sum(x => x.quantity_est) != 0 ? last.Sum(x => x.quantity_est) : 0) - (last.Key.IM.credit != null ? last.Key.IM.credit : 0 - last.Key.IM.debit != null ? last.Key.IM.debit : 0),
                                                        item = last.Key.items
                                                    }).ToList();

                        var productlist = (from PL in productlistbasic
                                           group PL by new { PL.item }
                                               into last
                                               select new
                                               {
                                                   _id_item = last.Key.item.id_item != 0 ? last.Key.item.id_item : 0,
                                                   _code = last.Key.item != null ? last.Key.item.code : "",
                                                   _name = last.Key.item != null ? last.Key.item.name : "",
                                                   _id_task = last.Max(x => x._id_task),
                                                   _ordered_quantity = last.Max(x => x._ordered_quantity),
                                                   avlqtyColumn = last.Sum(x => x.avlqtyColumn),
                                                   buyqty = last.Sum(x => x.avlqtyColumn) < last.Max(x => x._ordered_quantity) ? (last.Max(x => x._ordered_quantity) != 0 ? last.Max(x => x._ordered_quantity) : 0) - (last.Sum(x => x.avlqtyColumn) != 0 ? last.Sum(x => x.avlqtyColumn) : 0) : 0,
                                                   item = last.Key.item
                                               }).ToList();

                        item_ProductDataGrid.ItemsSource = productlist.Where(IT => IT.item.id_item_type == item.item_type.Product).ToList();

                        item_RawDataGrid.ItemsSource = productlist.Where(IT => IT.item.id_item_type == item.item_type.RawMaterial).ToList(); ;
                        var servicelist = (from IT in entity.db.project_task
                                           where IT.status == Status.Project.Approved //) 
                                           && IT.status != null && IT.id_project == _id_project

                                           group IT by new { IT.items } into last
                                           select new
                                           {
                                               _id_item = last.Key.items.id_item != 0 ? last.Key.items.id_item : 0,
                                               _code = last.Key.items != null ? last.Key.items.code : "",
                                               _name = last.Key.items != null ? last.Key.items.name : "",
                                               _id_task = last.Max(x => x.id_project_task),
                                               _ordered_quantity = last.Sum(x => x.quantity_est) != 0 ? last.Sum(x => x.quantity_est) : 0,
                                               item = last.Key.items
                                               // avlqtyColumn =  entity.db.item_movement.Where(x => x.item_product.id_item == last.Key.items.id_item)!=null?entity.db.item_movement.Where(x => x.item_product.id_item == last.Key.items.id_item).Sum(x => x.credit != 0 ? x.credit : 0 - x.debit != 0 ? x.debit : 0):0
                                           }).ToList();
                        item_ServiceDataGrid.ItemsSource = servicelist.Where(IT => IT.item.id_item_type == item.item_type.Service).ToList();
                        item_CapitalDataGrid.ItemsSource = servicelist.Where(IT => IT.item.id_item_type == item.item_type.FixedAssets).ToList(); ;
                        dgvSupplies.ItemsSource = servicelist.Where(IT => IT.item.id_item_type == item.item_type.Supplies).ToList(); ;
                    }
                }

            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        #region Add Purchase Tender
        //private void btnNewTender_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        project objProject = projectViewSource.View.CurrentItem as project;
        //        if (objProject != null)
        //        {
        //            txtName.Text = string.Empty;
        //            txtComment.Text = string.Empty;

        //            int idContact = Convert.ToInt32(objProject.id_contact);
        //            contact contact = entity.db.contacts.Where(a => a.id_contact == idContact).Include("app_contract").Include("app_currency").FirstOrDefault();
        //            if (contact.app_contract != null)
        //            {
        //                cbxCondition.SelectedValue = Convert.ToInt32(contact.app_contract.app_condition.id_condition);
        //                cbxContract.SelectedValue = Convert.ToInt32(contact.app_contract.id_contract);
        //            }
        //            if (contact.app_currency != null && contact.app_currency.app_currencyfx != null)
        //            {
        //                cbxCurrency.SelectedValue = Convert.ToInt32(contact.app_currency.app_currencyfx.Where(a => a.is_active == true).FirstOrDefault().id_currencyfx);
        //            }
        //            crud_modal.Visibility = System.Windows.Visibility.Visible;
        //        }
        //        else
        //        {
        //            toolBar.msgWarning("Error getting Project Details");
        //        }
        //    }
        //    catch
        //    {

        //    }
        //}
        //private void lblCancel_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    crud_modal.Visibility = System.Windows.Visibility.Hidden;
        //}

        //private void btnSaveTender_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        project objProject = projectViewSource.View.CurrentItem as project;
        //        if (objProject != null)
        //        {
        //            using (db dbTender = new db())
        //            {
        //                int idProject = objProject.id_project;
        //                purchase_tender purchase_tender = new purchase_tender();
        //                if (idProject > 0)
        //                    purchase_tender.id_project = idProject;
        //                purchase_tender.name = txtName.Text;
        //                //purchase_tender.code = Convert.ToInt16(txtCode.Text);
        //                purchase_tender.comment = txtComment.Text;

        //                purchase_tender_contact purchase_tender_contact = new purchase_tender_contact();
        //                int idContact = Convert.ToInt32(objProject.id_contact);
        //                contact contact = entity.db.contacts.Where(a => a.id_contact == idContact).Include("app_contract").Include("app_currency").FirstOrDefault();
        //                if (contact.app_contract != null)
        //                {
        //                    purchase_tender_contact.id_condition = Convert.ToInt32(contact.app_contract.app_condition.id_condition);
        //                    purchase_tender_contact.id_contract = Convert.ToInt32(contact.app_contract.id_contract);
        //                }
        //                else
        //                {
        //                    purchase_tender_contact.id_condition = Convert.ToInt32(entity.db.app_contract.FirstOrDefault().app_condition.id_condition);
        //                    purchase_tender_contact.id_contract = Convert.ToInt32(entity.db.app_contract.FirstOrDefault().id_contract);
        //                }
        //                if (contact.app_currency != null && contact.app_currency.app_currencyfx != null)
        //                {
        //                    purchase_tender_contact.id_currencyfx = Convert.ToInt32(contact.app_currency.app_currencyfx.Where(a => a.is_active == true).FirstOrDefault().id_currencyfx);
        //                }
        //                else
        //                {
        //                    purchase_tender_contact.id_currencyfx = Convert.ToInt32(entity.db.app_currencyfx.Where(a => a.is_active == true).FirstOrDefault().id_currencyfx);
        //                }
        //                purchase_tender_contact.id_contact = Convert.ToInt32(objProject.id_contact);

        //                purchase_tender.purchase_tender_contact_detail.Add(purchase_tender_contact);

        //                int id = ((project)projectDataGrid.SelectedItem).id_project;


        //                List<project_task> productlist = itemDataGrid.ItemsSource.OfType<project_task>().ToList();
        //                productlist = productlist.Where(x => x.IsSelected == true && x.items.id_item_type != item.item_type.Task).ToList();
        //                foreach (project_task data in productlist)
        //                {
        //                    //if (data._selectd == true && data._type != item.item_type.Task)
        //                    //{
        //                        purchase_tender_item purchase_tender_item = new purchase_tender_item();
        //                        if (dbTender.app_cost_center.Where(a => a.id_company == _entity.company_ID && a.is_active == true && a.is_administrative == true).FirstOrDefault()!=null)
        //                        {
        //                            purchase_tender_item.id_cost_center = dbTender.app_cost_center.Where(a => a.id_company == _entity.company_ID && a.is_active == true && a.is_administrative == true).FirstOrDefault().id_cost_center;
        //                        }
        //                        else
        //                        {
        //                            MessageBox.Show("Please enter cost center", "Cognitivo", MessageBoxButton.OK, MessageBoxImage.Error);
        //                            return;
        //                        }

        //                      int idItem = data.items.id_item;
        //                        purchase_tender_item.id_item = idItem;
        //                        purchase_tender_item.item_description = dbTender.items.Where(a => a.id_item == idItem).FirstOrDefault().name;
        //                        purchase_tender_item.quantity = (decimal)data.quantity_est;
        //                        purchase_tender.purchase_tender_item_detail.Add(purchase_tender_item);
        //                   // }

        //                }

        //                dbTender.purchase_tender.Add(purchase_tender);
        //                dbTender.SaveChanges();
        //                //dbTender.Entry(purchase_tender).State = EntityState.Detached;
        //                //entity.db.purchase_tender.Attach(purchase_tender);
        //            }
        //            lblCancel_MouseDown(null, null);
        //        }
        //        else
        //        {
        //            toolBar.msgWarning("Error getting Project Details");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        toolBar.msgError(ex);
        //    }
        //}

        private void btnRequestResource_Click(object sender, RoutedEventArgs e)
        {
            ItemRequest = new cntrl.Curd.ItemRequest();
            crud_modal.Visibility = Visibility.Visible;
            ItemRequest.listdepartment = entity.db.app_department.ToList();
            ItemRequest.item_request_Click += item_request_Click;
            crud_modal.Children.Add(ItemRequest);

        }


        public void item_request_Click(object sender)
        {
            project project = ((project)projectViewSource.View.CurrentItem);
            int id_project = ((project)projectViewSource.View.CurrentItem).id_project;

            List<project_task> productlist = entity.db.project_task.ToList();
            productlist = productlist.Where(x => x.IsSelected == true).ToList();

            item_request item_request = new item_request();
            item_request.name = ItemRequest.name;
            item_request.comment = ItemRequest.comment;
            item_request.id_branch = project.id_branch;
            item_request.id_department = ItemRequest.id_department;
            item_request.id_project = id_project;
            item_request.request_date = DateTime.Now;

            foreach (project_task project_task in productlist)
            {
                item_request_detail item_request_detail = new entity.item_request_detail();
                item_request_detail.date_needed_by = ItemRequest.neededDate;
                item_request_detail.id_project_task = project_task.id_project_task;
                item_request_detail.urgency = ItemRequest.Urgencies;
                item_request_detail.comment = ItemRequest.comment;
                int idItem = (int)project_task.id_item;
                item_request_detail.id_item = idItem;
                item_request_detail.quantity = (int)project_task.quantity_est;

                List<project_task_dimension> project_task_dimensionList = entity.db.project_task_dimension.Where(x => x.id_project_task == project_task.id_project_task).ToList();
                foreach (project_task_dimension project_task_dimension in project_task_dimensionList)
                {
                    item_request_dimension item_request_dimension = new item_request_dimension();
                    item_request_dimension.id_dimension = project_task_dimension.id_dimension;
                    item_request_dimension.id_measurement = project_task_dimension.id_measurement;
                    item_request_dimension.value = project_task_dimension.value;
                    string comment = "";

                    comment += project_task_dimension.value.ToString();
                    comment += "X";


                    item_request_detail.comment = comment.Substring(0, comment.Length - 1);
                    item_request_detail.item_request_dimension.Add(item_request_dimension);
                }


                item_request.item_request_detail.Add(item_request_detail);

            }
            entity.db.item_request.Add(item_request);
            entity.db.SaveChanges();

            crud_modal.Children.Clear();
            crud_modal.Visibility = System.Windows.Visibility.Collapsed;
        }

        #endregion

        private void item_ProductDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int _id_project = ((project)projectViewSource.View.CurrentItem).id_project;
                dynamic obj = (dynamic)item_ProductDataGrid.SelectedItem;
                if (obj != null)
                {
                    int _id_item = obj._id_item;
                    listLogistics(item.item_type.Product, _id_project, _id_item);
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void item_RawDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int _id_project = ((project)projectViewSource.View.CurrentItem).id_project;
                dynamic obj = (dynamic)item_RawDataGrid.SelectedItem;
                if (obj != null)
                {
                    int _id_item = obj._id_item;
                    listLogistics(item.item_type.RawMaterial, _id_project, _id_item);
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void item_ServiceDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int _id_project = ((project)projectViewSource.View.CurrentItem).id_project;
                dynamic obj = (dynamic)item_ServiceDataGrid.SelectedItem;
                if (obj != null)
                {
                    int _id_item = obj._id_item;
                    listLogistics(item.item_type.Service, _id_project, _id_item);
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void item_CapitalDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int _id_project = ((project)projectViewSource.View.CurrentItem).id_project;
                dynamic obj = (dynamic)item_CapitalDataGrid.SelectedItem;
                if (obj != null)
                {
                    int _id_item = obj._id_item;
                    listLogistics(item.item_type.FixedAssets, _id_project, _id_item);
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void dgvSupplies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int _id_project = _id_project = ((project)projectViewSource.View.CurrentItem).id_project;
                dynamic obj = (dynamic)dgvSupplies.SelectedItem;
                if (obj != null)
                {
                    int _id_item = obj._id_item;
                    listLogistics(item.item_type.Supplies, _id_project, _id_item);
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void listLogistics(item.item_type type, int _id_project, int _id_item)
        {
            List<project_task> list = entity.db.project_task.Where(IT => IT.items.id_item_type == type && IT.status == Status.Project.Approved && IT.status != null && IT.id_project == _id_project && IT.id_item == _id_item).ToList();
            itemDataGrid.ItemsSource = list.ToList();
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    projectViewSource.View.Filter = i =>
                    {
                        project project = i as project;
                        if (project.name.ToLower().Contains(query.ToLower()))
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
                    projectViewSource.View.Filter = null;
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void item_ProductDataGrid_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            dynamic _item_product = ((DataGrid)sender).SelectedItem;
            int id_product = _item_product._id_item;
            DataGrid item_movementDataGrid = e.DetailsElement as DataGrid;
            var movement =
                (from items in entity.db.items

                 join item_product in entity.db.item_product on items.id_item equals item_product.id_item
                     into its
                 from p in its
                 join item_movement in entity.db.item_movement on p.id_item_product equals item_movement.id_item_product
                 into IMS
                 from a in IMS
                 join AM in entity.db.app_branch on a.app_location.id_branch equals AM.id_branch
                 where a.status == Status.Stock.InStock && a.item_product.id_item == id_product

                 group a by new { a.item_product }
                     into last
                     select new
                     {
                         code = last.Key.item_product.item.code,
                         name = last.Key.item_product.item.name,
                         BranchName = last.OrderBy(m => m.app_location.id_branch),
                         itemid = last.Key.item_product.item.id_item,
                         quntitiy = last.Sum(x => x.credit) - last.Sum(x => x.debit),
                         measurement = last.Key.item_product.item.id_measurement
                     }).ToList();
            item_movementDataGrid.ItemsSource = movement;

        }

        private void itemDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((TabItem)TabLogistics.SelectedItem).Name == "tabproduct" || ((TabItem)TabLogistics.SelectedItem).Name == "tabraw")
            {

                itemDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;

            }
            else
            {
                itemDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
            }
        }

        private void itemDataGrid_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            project_task project_task = (project_task)itemDataGrid.SelectedItem;
            CollectionViewSource project_task_dimensionViewSource = ((CollectionViewSource)(FindResource("project_task_dimensionViewSource")));

            project_task_dimensionViewSource.Source = entity.db.project_task_dimension.Where(a => a.id_company == _entity.company_ID && a.id_project_task == project_task.id_project_task).ToList();
        }



        private void itemDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            List<project_task> productlist = entity.db.project_task.ToList();
            noofrows = productlist.Where(x => x.IsSelected == true).Count();
            RaisePropertyChanged("noofrows");
        }

        private void TabLogistics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source.GetType() == typeof(TabControl))
            {
                LoadData();
                itemDataGrid.ItemsSource = null;
            }
        }

        private void dgvServiceContract_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int _id_project = _id_project = ((project)projectViewSource.View.CurrentItem).id_project;
            dynamic obj = (dynamic)dgvServiceContract.SelectedItem;
            if (obj != null)
            {
                int _id_item = obj._id_item;
                listLogistics(item.item_type.ServiceContract, _id_project, _id_item);
            }
        }
    }
}
