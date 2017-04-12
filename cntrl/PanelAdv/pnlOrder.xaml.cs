using entity;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace cntrl.PanelAdv
{
    public partial class pnlOrder : UserControl
    {
        private CollectionViewSource production_orderViewSource, production_lineViewSource; //, itemViewSource;
        public List<project_task> project_taskLIST { get; set; }
        public db Shared_dbContext { get; set; }
        public CollectionViewSource projectViewSource { get; set; }
        //private dbContext _dbContext = new dbContext();

        public pnlOrder()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (project_taskLIST.FirstOrDefault() == null)
            {
                return;
            }

            project project = project_taskLIST.FirstOrDefault().project;

            if (project == null)
            {
                return;
            }

            production_order production_order = new production_order();

            // Do not load your data at design time.
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                //Load your data here and assign the result to the CollectionViewSource.
                production_orderViewSource = (CollectionViewSource)this.Resources["production_orderViewSource"];

                Shared_dbContext.production_order.Add(production_order);

                production_orderViewSource.Source = Shared_dbContext.production_order.Local;

                production_lineViewSource = (CollectionViewSource)this.Resources["production_lineViewSource"];
                Shared_dbContext.production_line.Load();
                production_lineViewSource.Source = Shared_dbContext.production_line.Local;
            }

            if (project_taskLIST.Count() > 0)
            {
                production_order.id_project = project.id_project;
                contact contact = project.contact;

                if (contact != null)
                {
                    if (contact.app_cost_center != null)
                    {
                        production_order.project_cost_center = contact.app_cost_center.name;
                    }
                }
                if (project.id_branch != null)
                {
                    production_order.id_branch = (int)project.id_branch;
                }

                //Get Name.
                production_order.name = project.name;
                production_order.RaisePropertyChanged("name");

                //Date check. Get the range from task first, if blank get from Project.
                production_order.start_date_est = project_taskLIST.OrderBy(x => x.start_date_est).FirstOrDefault().start_date_est;
                production_order.end_date_est = project_taskLIST.OrderByDescending(x => x.end_date_est).FirstOrDefault().end_date_est;

                if (production_order.start_date_est == null || production_order.end_date_est == null)
                {
                    production_order.start_date_est = project_taskLIST.OrderBy(x => x.start_date_est).Select(x => x.project.est_start_date).FirstOrDefault();
                    production_order.RaisePropertyChanged("start_date_est");

                    production_order.end_date_est = project_taskLIST.OrderByDescending(x => x.end_date_est).Select(x => x.project.est_end_date).FirstOrDefault();
                    production_order.RaisePropertyChanged("end_date_est");
                }

                foreach (project_task item in project_taskLIST.Where(x => x.status == Status.Project.Approved || x.status == Status.Project.InProcess))
                {
                    project_task _project_task = item;
                    production_order_detail production_order_detail = new production_order_detail();
                    if (production_order_detail.item != null)
                    {
                        if (production_order_detail.item.id_item_type != entity.item.item_type.Task)
                        {
                            production_order_detail.status = Status.Production.Pending;
                        }
                    }

                    production_order_detail.id_order_detail = _project_task.id_project_task;
                    production_order_detail.name = _project_task.item_description;
                    production_order_detail.code = _project_task.code;
                    //Ref Keys
                    production_order_detail.item = _project_task.items;
                    production_order_detail.id_item = _project_task.id_item;

                    //If Item has Recepie
                    if (_project_task.items.item_recepie.Count > 0)
                    {
                        production_order_detail.is_input = false;
                    }
                    else
                    {
                        production_order_detail.is_input = true;
                    }

                    if (_project_task.parent != null)
                    {
                        production_order_detail _production_order_detail = production_order.production_order_detail.Where(x => x.id_project_task == _project_task.parent.id_project_task).FirstOrDefault();
                        if (_production_order_detail != null)
                        {
                            production_order_detail.parent = _production_order_detail;
                        }
                    }

                    production_order_detail.id_project_task = _project_task.id_project_task;
                    if (_project_task.quantity_est > 0)
                    {
                        production_order_detail.quantity = (decimal)_project_task.quantity_est;
                    }

                    production_order.status = entity.Status.Production.Pending;
                    production_order.production_order_detail.Add(production_order_detail);
                }

                if (production_order == null)
                {
                    Shared_dbContext.production_order.Add(production_order);
                }

                production_orderViewSource.View.MoveCurrentToLast();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (project_task item in project_taskLIST)
            {
                project_task _project_task = item;

                if (_project_task.status == Status.Project.Approved)
                {
                    _project_task.status = entity.Status.Project.InProcess;
                    _project_task.IsSelected = false;
                }
            }

            Shared_dbContext.SaveChanges();
            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;

            filter_task();
        }

        public void filter_task()
        {
            if (projectViewSource != null)
            {
                if (projectViewSource.View != null)
                {
                    projectViewSource.View.Filter = i =>
                    {
                        project_task _project_task = (project_task)i;
                        if (_project_task.parent == null)
                            return true;
                        else
                        {
                            return false;
                        }
                    };
                }
            }
        }

        private void lblCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            production_order production_order = (production_order)production_orderViewSource.View.CurrentItem;
            Shared_dbContext.production_order.Remove(production_order);

            foreach (var item in project_taskLIST)
            {
                project_task _project_task = (project_task)item;
                _project_task.IsSelected = false;
            }

            projectViewSource.View.Refresh();

            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }
    }
}