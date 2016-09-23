using entity;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using System.ComponentModel;
using System.Collections.Generic;

namespace Cognitivo.Project
{
    public partial class EventFinance : Page, INotifyPropertyChanged
    {
        SalesOrderDB SalesOrderDB = new SalesOrderDB();
        CollectionViewSource project_taskViewSource;
        CollectionViewSource projectViewSource;

        public bool ViewAll { get; set; }

        public EventFinance()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            project_taskViewSource = ((CollectionViewSource)(FindResource("project_taskViewSource")));
            projectViewSource = ((CollectionViewSource)(FindResource("projectViewSource")));
            SalesOrderDB.projects.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).Load();
            projectViewSource.Source = SalesOrderDB.projects.Local;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string prop)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            project project = projectViewSource.View.CurrentItem as project;
            if (project != null)
            {
                decimal EventTotal = 0M;
                decimal InvoiceTotal = 0M;

                foreach (project_task project_task in project.project_task)
                {
                    project_task.CalcExecutedQty_TimerTaks();
                    EventTotal += project_task.SubTotal_WithVAT;
                }

                lblEventTotal.Content = EventTotal;

                foreach (sales_invoice sales_invoice in project.sales_invoice.Where(x => x.status == Status.Documents_General.Approved))
                {
                    InvoiceTotal += entity.Brillo.Currency.convert_Values(sales_invoice.GrandTotal, sales_invoice.id_currencyfx, CurrentSession.CurrencyFX_Default.id_currencyfx, entity.App.Modules.Sales);
                }

                lblInvoiceTotal.Content = InvoiceTotal;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            project project = projectViewSource.View.CurrentItem as project;

            if (project != null)
            {
                cntrl.SalesInvoice objSalesinvoice = new cntrl.SalesInvoice();

                objSalesinvoice.project = project;
                objSalesinvoice.db = SalesOrderDB;

                objSalesinvoice.TotalCost = project.project_task.Sum(x=>x.SubTotal_WithVAT);
                ///Crud Modal Visibility and Add.
                crud_modal.Visibility = Visibility.Visible;
                crud_modal.Children.Add(objSalesinvoice);
            }
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            if (!string.IsNullOrEmpty(query) && projectViewSource != null)
            {
                try
                {
                    projectViewSource.View.Filter = i =>
                    {
                        project project = i as project;
                        List<project_tag_detail> project_tag_detail = new List<project_tag_detail>();
                        project_tag_detail = project.project_tag_detail.ToList();

                        if (project.name.ToLower().Contains(query.ToLower()) || project_tag_detail.Where(x => x.project_tag.name.ToLower().Contains(query.ToLower())).Any())
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    };
                }
                catch (Exception ex)
                {
                    toolBar.msgError(ex);
                }
            }
            else
            {
                projectViewSource.View.Filter = null;
            }
        }

        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            projectViewSource.View.Refresh();
        }

    }
}