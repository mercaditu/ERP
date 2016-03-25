using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using entity;

namespace Cognitivo.Project.Development
{
    public partial class Logistics : Page
    {
        ProjectTaskDB ProjectTaskDB = new ProjectTaskDB();

        CollectionViewSource projectViewSource;
        cntrl.Curd.ItemRequest ItemRequest;

        public Logistics()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            projectViewSource = ((CollectionViewSource)(this.FindResource("projectViewSource")));
            await ProjectTaskDB.projects.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company).LoadAsync();
            projectViewSource.Source = ProjectTaskDB.projects.Local;

            CollectionViewSource app_dimensionViewSource = ((CollectionViewSource)(FindResource("app_dimensionViewSource")));
            await ProjectTaskDB.app_dimension.Where(a => a.id_company == CurrentSession.Id_Company).LoadAsync();
            app_dimensionViewSource.Source = ProjectTaskDB.app_dimension.Local;

            CollectionViewSource app_measurementViewSource = ((CollectionViewSource)(FindResource("app_measurementViewSource")));
            await ProjectTaskDB.app_measurement.Where(a => a.id_company == CurrentSession.Id_Company).LoadAsync();
            app_measurementViewSource.Source = ProjectTaskDB.app_measurement.Local;
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
                        var productlistbasic = (from IT in ProjectTaskDB.project_task
                                                where (IT.status == Status.Project.Approved)
                                                && IT.status != null && IT.id_project == _id_project
                                                join IK in ProjectTaskDB.item_product on IT.id_item equals IK.id_item
                                                join IO in ProjectTaskDB.item_movement on IK.id_item_product equals IO.id_item_product into a
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

                        item_RawDataGrid.ItemsSource = productlist.Where(IT => IT.item.id_item_type == item.item_type.RawMaterial).ToList();
                        var servicelist = (from IT in ProjectTaskDB.project_task
                                           where IT.status == Status.Project.Approved
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
                                           }).ToList();

                        item_ServiceDataGrid.ItemsSource = servicelist.Where(IT => IT.item.id_item_type == item.item_type.Service).ToList();
                        item_CapitalDataGrid.ItemsSource = servicelist.Where(IT => IT.item.id_item_type == item.item_type.FixedAssets).ToList();
                        dgvServiceContract.ItemsSource = servicelist.Where(IT => IT.item.id_item_type == item.item_type.ServiceContract).ToList();

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

        private void btnRequestResource_Click(object sender, EventArgs e)
        {
            if (itemDataGrid.ItemsSource != null)
            {
                ////List<project_task> project_task = new List<entity.project_task>();
                ////project_task = entity.project_task.Where(x => x.IsSelected == true).ToList();

                //if (ProjectTaskDB.project_task.Local.Where(x => x.IsSelected == true).Count() > 0)
                //{
                //    ItemRequest = new cntrl.Curd.ItemRequest();
                //    crud_modal.Visibility = Visibility.Visible;
                //    ItemRequest.listdepartment = ProjectTaskDB.app_department.ToList();
                //    ItemRequest.item_request_Click += item_request_Click;
                //    crud_modal.Children.Add(ItemRequest);
                //}
                //else
                //{
                //    toolBar.msgWarning("Select a Task");
                //}


                project project = ((project)projectViewSource.View.CurrentItem);
                int id_project = ((project)projectViewSource.View.CurrentItem).id_project;

                List<project_task> productlist = ProjectTaskDB.project_task.ToList();
                productlist = productlist.Where(x => x.IsSelected == true).ToList();

                purchase_tender purchase_tender = new purchase_tender();
                purchase_tender.status = Status.Documents_General.Pending;



                purchase_tender.name = project.name;
                purchase_tender.code = 000;
                purchase_tender.trans_date = DateTime.Now;
       

                foreach (project_task project_task in productlist)
                {
                   
                        if (project.id_branch != null)
                        {

                            purchase_tender.app_branch = ProjectTaskDB.app_branch.Where(x => x.id_branch == project.id_branch).FirstOrDefault();
                        }
                        else
                        {
                            purchase_tender.app_branch = ProjectTaskDB.app_branch.Where(x => x.can_invoice == true && x.can_stock == true).FirstOrDefault();
                        }

                   
                   
                  


                    purchase_tender.id_project = project_task.id_project;
                    purchase_tender_item purchase_tender_item = new purchase_tender_item();
                    purchase_tender_item.id_item = project_task.id_item;
                    purchase_tender_item.item_description = project_task.item_description;
                    purchase_tender_item.quantity =(decimal) project_task.quantity_est;


                    foreach (project_task_dimension project_task_dimension in project_task.project_task_dimension)
                    {
                        purchase_tender_dimension purchase_tender_dimension = new purchase_tender_dimension();
                        purchase_tender_dimension.id_dimension = project_task_dimension.id_dimension;
                        purchase_tender_dimension.id_measurement = project_task_dimension.id_measurement;
                        purchase_tender_dimension.value = project_task_dimension.value;
                        purchase_tender_item.purchase_tender_dimension.Add(purchase_tender_dimension);
                    }

                    purchase_tender.purchase_tender_item_detail.Add(purchase_tender_item);

                }
                ProjectTaskDB.purchase_tender.Add(purchase_tender);
                ProjectTaskDB.SaveChanges();
               
                toolBar.msgSaved();
            }
        }


        public void item_request_Click(object sender)
        {
            project project = ((project)projectViewSource.View.CurrentItem);
            int id_project = ((project)projectViewSource.View.CurrentItem).id_project;

            List<project_task> productlist = ProjectTaskDB.project_task.ToList();
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

                List<project_task_dimension> project_task_dimensionList = ProjectTaskDB.project_task_dimension.Where(x => x.id_project_task == project_task.id_project_task).ToList();
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
            ProjectTaskDB.item_request.Add(item_request);
            ProjectTaskDB.SaveChanges();

            crud_modal.Children.Clear();
            crud_modal.Visibility = System.Windows.Visibility.Collapsed;
        }

        #endregion

        private void item_ProductDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid DataGrid = sender as DataGrid;
            dgv_SelectionChanged(item.item_type.Product, DataGrid);
        }

        private void item_RawDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid DataGrid = sender as DataGrid;
            dgv_SelectionChanged(item.item_type.RawMaterial, DataGrid);
        }

        private void item_ServiceDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid DataGrid = sender as DataGrid;
            dgv_SelectionChanged(item.item_type.Service, DataGrid);
        }

        private void item_CapitalDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid DataGrid = sender as DataGrid;
            dgv_SelectionChanged(item.item_type.FixedAssets, DataGrid);
        }

        private void dgvSupplies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid DataGrid = sender as DataGrid;
            dgv_SelectionChanged(item.item_type.Supplies, DataGrid);
        }

        private void dgvServiceContract_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid DataGrid = sender as DataGrid;
            dgv_SelectionChanged(item.item_type.ServiceContract, DataGrid);
        }

        private void dgv_SelectionChanged(item.item_type Type, DataGrid DataGrid)
        {
            try
            {
                int _id_project = _id_project = ((project)projectViewSource.View.CurrentItem).id_project;
                dynamic obj = (dynamic)DataGrid.SelectedItem;
                if (obj != null)
                {
                    int _id_item = obj._id_item;

                    List<project_task> list = ProjectTaskDB.project_task.Where(IT =>
                            IT.items.id_item_type == Type &&
                            IT.status == Status.Project.Approved &&
                            IT.status != null &&
                            IT.id_project == _id_project &&
                            IT.id_item == _id_item)
                              .ToList();
                    itemDataGrid.ItemsSource = list.ToList();
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
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
            catch
            { }
        }

        private void item_ProductDataGrid_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            dynamic _item_product = ((DataGrid)sender).SelectedItem;
            int id_product = _item_product._id_item;
            DataGrid item_movementDataGrid = e.DetailsElement as DataGrid;
            var movement =
                (from items in ProjectTaskDB.items

                 join item_product in ProjectTaskDB.item_product on items.id_item equals item_product.id_item
                     into its
                 from p in its
                 join item_movement in ProjectTaskDB.item_movement on p.id_item_product equals item_movement.id_item_product
                 into IMS
                 from a in IMS
                 join AM in ProjectTaskDB.app_branch on a.app_location.id_branch equals AM.id_branch
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

            project_task_dimensionViewSource.Source = ProjectTaskDB.project_task_dimension.Where(a => a.id_company == CurrentSession.Id_Company && a.id_project_task == project_task.id_project_task).ToList();
        }

        private void TabLogistics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source.GetType() == typeof(TabControl))
            {
                LoadData();
                itemDataGrid.ItemsSource = null;
            }
        }


    }
}
