using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using entity;
using System.Data.Entity.Validation;
using System.Data.Entity;

namespace cntrl.Curd
{
    public partial class PurchaseTender : UserControl
    {
        


        public ProjectTaskDB ProjectTaskDB { get; set; }

        public List<project_task> project_taskList { get; set; }
    


        public PurchaseTender()
        {
            InitializeComponent();
         
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                try
                {
                    CollectionViewSource app_dimensionViewSource = ((CollectionViewSource)(FindResource("app_dimensionViewSource")));
                    ProjectTaskDB.app_dimension.Where(a => a.id_company == CurrentSession.Id_Company).Load();
                    app_dimensionViewSource.Source = ProjectTaskDB.app_dimension.Local;

                    CollectionViewSource app_measurementViewSource = ((CollectionViewSource)(FindResource("app_measurementViewSource")));
                    ProjectTaskDB.app_measurement.Where(a => a.id_company == CurrentSession.Id_Company).Load();
                    app_measurementViewSource.Source = ProjectTaskDB.app_measurement.Local;

                    cmbtask.ItemsSource = project_taskList;

                    purchase_tender purchase_tender = new purchase_tender();
                    purchase_tender.status = Status.Documents_General.Pending;



                    purchase_tender.name = project_taskList.FirstOrDefault().project.name;
                    purchase_tender.code = 000;
                    purchase_tender.trans_date = DateTime.Now;


                    foreach (project_task project_task in project_taskList)
                    {

                        if (project_task.project.id_branch != null)
                        {

                            purchase_tender.app_branch = ProjectTaskDB.app_branch.Where(x => x.id_branch == project_task.project.id_branch).FirstOrDefault();
                        }
                        else
                        {
                            purchase_tender.app_branch = ProjectTaskDB.app_branch.Where(x => x.can_invoice == true && x.can_stock == true).FirstOrDefault();
                        }




                        if (purchase_tender.purchase_tender_item_detail.Where(x => x.id_item == project_task.id_item).Count() == 0)
                        {



                            purchase_tender.id_project = project_task.id_project;
                            purchase_tender_item purchase_tender_item = new purchase_tender_item();
                            purchase_tender_item.id_item = project_task.id_item;
                            purchase_tender_item.id_project_task = project_task.id_project_task;
                            purchase_tender_item.item_description = project_task.item_description;
                            purchase_tender_item.quantity = (decimal)project_task.quantity_est;

                            foreach (project_task_dimension project_task_dimension in project_task.project_task_dimension)
                            {
                                purchase_tender_dimension purchase_tender_dimension = new purchase_tender_dimension();
                                purchase_tender_dimension.id_dimension = project_task_dimension.id_dimension;
                                purchase_tender_dimension.id_measurement = project_task_dimension.id_measurement;
                                purchase_tender_dimension.value = 0;
                                purchase_tender_item.purchase_tender_dimension.Add(purchase_tender_dimension);
                            }

                            purchase_tender.purchase_tender_item_detail.Add(purchase_tender_item);
                        }
                        else
                        {
                            purchase_tender_item purchase_tender_item = purchase_tender.purchase_tender_item_detail.Where(x => x.id_item == project_task.id_item).FirstOrDefault();
                            purchase_tender_item.quantity = purchase_tender_item.quantity + (decimal)project_task.quantity_est;
                           
                        }
                    }
                    ProjectTaskDB.purchase_tender.Add(purchase_tender);
                    CollectionViewSource purchase_tender_itemViewSource = (CollectionViewSource)this.FindResource("purchase_tender_itemViewSource");
                    purchase_tender_itemViewSource.Source = ProjectTaskDB.purchase_tender_item_detail.Local.Where(x=>x.id_purchase_tender_item==0);

                   
               
              

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

      

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProjectTaskDB.CancelAllChanges();
                
                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            
            ProjectTaskDB.SaveChanges();
            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = System.Windows.Visibility.Collapsed;
        }

      


    }

}
