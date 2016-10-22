using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using entity;
using System.Data.Entity;

namespace cntrl
{
    public partial class SalesOrder : UserControl
    {
        public project project
        {
            get { return _project; }
            set
            {
                if (_project != value)
                {
                    _project = value;

                    if (_project != null)
                    {
                        if (_project.contact != null)
                        {
                            contact contact = _project.contact;

                            if (contact.app_contract != null)
                                cbxCondition.SelectedValue = contact.app_contract.id_condition;
                            //Contract
                            if (contact.id_contract != null)
                                cbxContract.SelectedValue = Convert.ToInt32(contact.id_contract);

                            cbxCurrency.get_ActiveRateXContact(ref contact);
                        }
                    }
                }
            }
        }

        private project _project;
        public SalesOrderDB SalesOrderDB { get; set; }
        public Boolean Generate_Invoice { get; set; }
        public Boolean Generate_Budget { get; set; }
        public decimal Percentage { get; set; }

        public SalesOrder()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (project != null)
            {
                toolBar toolBar = new toolBar();
                List<project_task> project_taskLIST = project.project_task.Where(x => x.IsSelected).ToList();

                sales_order sales_order = new sales_order();
                if (Generate_Budget)
                {
                    SalesBudgetDB SalesBudgetDB = new SalesBudgetDB();

                    sales_budget sales_budget = new sales_budget();

                    sales_budget.id_contact = (int)project.id_contact;
                    sales_budget.contact = SalesBudgetDB.contacts.Where(x => x.id_contact == (int)project.id_contact).FirstOrDefault();
                    sales_budget.status = Status.Documents_General.Pending;
                    sales_budget.id_project = project.id_project;
                    sales_budget.id_condition = (int)cbxCondition.SelectedValue;
                    sales_budget.id_contract = (int)cbxContract.SelectedValue;
                    sales_budget.id_currencyfx = (int)cbxCurrency.SelectedValue;
                    sales_budget.comment = "Project -> " + project.name;
                    sales_budget.trans_date = DateTime.Now;

                    foreach (project_task _project_task in project_taskLIST)
                    {
                        decimal UnitPrice_Vat;
                        if (_project_task.UnitPrice_Vat==0 || _project_task.UnitPrice_Vat==null)
                        {
                            UnitPrice_Vat = Convert.ToDecimal(_project_task.unit_cost_est * (1 + Percentage));
                        }
                        else
                        {
                            UnitPrice_Vat = Convert.ToDecimal(_project_task.UnitPrice_Vat);
                        }
                        sales_budget_detail sales_budget_detail = new sales_budget_detail();
                        sales_budget_detail.State = EntityState.Added;
                        sales_budget_detail.id_sales_budget = sales_budget.id_sales_budget;
                        sales_budget_detail.sales_budget = sales_budget;
                        sales_budget_detail.id_item = (int)_project_task.id_item;
                        sales_budget_detail.item_description = _project_task.item_description;

                        if (project.is_Executable)
                        {
                            sales_budget_detail.quantity = (decimal)_project_task.quantity_exe;
                        }
                        else
                        {
                            sales_budget_detail.quantity = (decimal)_project_task.quantity_est;
                        }
                        
                        
                        if (UnitPrice_Vat != null)
                        {
                            sales_budget_detail.UnitPrice_Vat = UnitPrice_Vat;
                        }

                        sales_budget_detail.id_project_task = _project_task.id_project_task;
                        _project_task.IsSelected = false;

                        sales_budget.sales_budget_detail.Add(sales_budget_detail);
                    }

                    sales_budget.State = EntityState.Added;
                    sales_budget.IsSelected = true;
                    SalesBudgetDB.sales_budget.Add(sales_budget);
                    SalesBudgetDB.SaveChanges();
                }
                else
                {
                    if (project.sales_order.Count() == 0)
                    {


                        //if (Generate_Budget)
                        //{
                        //    sales_order.id_sales_budget = sales_budget.id_sales_budget;
                        //}
                        if (SalesOrderDB.contacts.Where(x => x.id_contact == (int)project.id_contact).FirstOrDefault() != null)
                        {
                            sales_order.id_contact = (int)project.id_contact;
                            sales_order.contact = SalesOrderDB.contacts.Where(x => x.id_contact == (int)project.id_contact).FirstOrDefault();
                        }
                        else
                        {
                            toolBar.msgWarning("Contact Not Found...");
                            return;
                        }

                        if (Generate_Invoice)
                        {
                            if (cbxDocument.SelectedValue != null)
                            {
                                sales_order.id_range = (int)cbxDocument.SelectedValue;
                            }
                            else
                            {
                                toolBar.msgWarning("Document Range Needed for Approval");
                                // return;
                            }
                        }
                        sales_order.id_project = project.id_project;

                        if (Convert.ToInt16(cbxCondition.SelectedValue) > 0)
                        {
                            sales_order.id_condition = (int)cbxCondition.SelectedValue;
                        }
                        else
                        {
                            toolBar.msgWarning("Condition Not Found...");
                            return;
                        }

                        if (Convert.ToInt16(cbxContract.SelectedValue) > 0)
                        {
                            sales_order.id_contract = (int)cbxContract.SelectedValue;
                        }
                        else
                        {
                            toolBar.msgWarning("Contract Not Found...");
                            return;
                        }

                        if (Convert.ToInt16(cbxCurrency.SelectedValue) > 0)
                        {
                            sales_order.id_currencyfx = (int)cbxCurrency.SelectedValue;
                        }
                        else
                        {
                            toolBar.msgWarning("Currency Not Found...");
                            return;
                        }

                        sales_order.comment = "Project -> " + project.name;

                        sales_order_detail sales_order_detail = null;

                        foreach (project_task _project_task in project_taskLIST)
                        {
                            decimal UnitPrice_Vat;
                            if (_project_task.UnitPrice_Vat == 0 || _project_task.UnitPrice_Vat == null)
                            {
                                UnitPrice_Vat = Convert.ToDecimal(_project_task.unit_cost_est * (1 + Percentage));
                            }
                            else
                            {
                                UnitPrice_Vat = Convert.ToDecimal(_project_task.UnitPrice_Vat);
                            }
                            if (_project_task.items.id_item_type == item.item_type.Task || _project_task.sales_detail == null)
                            {
                                sales_order_detail = new sales_order_detail();
                                sales_order_detail.State = EntityState.Added;
                                sales_order_detail.id_sales_order = sales_order.id_sales_order;
                                sales_order_detail.sales_order = sales_order;
                                if (Convert.ToInt16(_project_task.id_item) > 0)
                                {
                                    sales_order_detail.id_item = (int)_project_task.id_item;
                                }

                                sales_order_detail.item_description = _project_task.item_description;

                                if (project.is_Executable)
                                {
                                    sales_order_detail.quantity = (decimal)_project_task.quantity_exe;
                                }
                                else
                                {
                                    sales_order_detail.quantity = (decimal)_project_task.quantity_est;
                                }


                                if (UnitPrice_Vat != null)
                                {
                                    sales_order_detail.UnitPrice_Vat = UnitPrice_Vat;
                                }

                                sales_order_detail.id_project_task = _project_task.id_project_task;
                                _project_task.IsSelected = false;
                                _project_task.sales_detail = sales_order_detail;
                            }

                            if (sales_order_detail != null)
                            {
                                sales_order.sales_order_detail.Add(sales_order_detail);
                            }
                        }
                        sales_order.State = EntityState.Added;
                        sales_order.IsSelected = true;

                        SalesOrderDB.sales_order.Add(sales_order);
                        SalesOrderDB.SaveChanges();
                    }

                    if (sales_order.sales_order_detail.Count() > 0)
                    {
                     

                        if (Generate_Invoice && project.sales_invoice.Count()==0)
                        {
                            SalesOrderDB.Approve();
                            sales_invoice sales_invoice = new entity.sales_invoice();
                        
                            if (SalesOrderDB.app_document_range.Where(x => x.app_document.id_application == App.Names.SalesBudget).FirstOrDefault() != null)
                            {
                                sales_invoice.id_range = SalesOrderDB.app_document_range.Where(x => x.app_document.id_application == App.Names.SalesInvoice).FirstOrDefault().id_range;
                            }
                            sales_invoice.id_contact = (int)project.id_contact;
                            sales_invoice.contact = SalesOrderDB.contacts.Where(x => x.id_contact == (int)project.id_contact).FirstOrDefault();
                            sales_invoice.sales_order = sales_order;
                            sales_invoice.id_project = project.id_project;
                            sales_invoice.id_condition = (int)cbxCondition.SelectedValue;
                            sales_invoice.id_contract = (int)cbxContract.SelectedValue;
                            sales_invoice.id_currencyfx = (int)cbxCurrency.SelectedValue;
                            sales_invoice.comment = "Project -> " + project.name;
                            sales_invoice.trans_date = DateTime.Now;
                            
                            sales_invoice_detail sales_invoice_detail = null;

                            foreach (project_task _project_task in project_taskLIST)
                            {
                                decimal UnitPrice_Vat;
                                if (_project_task.UnitPrice_Vat == 0 || _project_task.UnitPrice_Vat == null)
                                {
                                    UnitPrice_Vat = Convert.ToDecimal(_project_task.unit_cost_est * (1 + Percentage));
                                }
                                else
                                {
                                    UnitPrice_Vat = Convert.ToDecimal(_project_task.UnitPrice_Vat);
                                }
                                sales_invoice_detail = new sales_invoice_detail();
                                sales_invoice_detail.State = EntityState.Added;
                                sales_invoice_detail.id_sales_invoice = sales_invoice.id_sales_invoice;
                                sales_invoice_detail.sales_invoice = sales_invoice;
                                sales_invoice_detail.id_item = (int)_project_task.id_item;
                                sales_invoice_detail.item_description = _project_task.item_description;

                                if (project.is_Executable)
                                {
                                    sales_invoice_detail.quantity = (decimal)_project_task.quantity_exe;
                                }
                                else
                                {
                                    sales_invoice_detail.quantity = (decimal)_project_task.quantity_est;
                                }

                                if (UnitPrice_Vat != null)
                                {
                                    sales_invoice_detail.UnitPrice_Vat = UnitPrice_Vat;

                                }

                                sales_invoice_detail.id_project_task = _project_task.id_project_task;
                                _project_task.IsSelected = false;

                                sales_invoice.sales_invoice_detail.Add(sales_invoice_detail);
                            }

                            sales_invoice.State = EntityState.Added;
                            sales_invoice.IsSelected = true;
                            crm_opportunity crm_opportunity = sales_order.crm_opportunity;
                            crm_opportunity.sales_invoice.Add(sales_invoice);
                            SalesOrderDB.crm_opportunity.Attach(crm_opportunity);
                            SalesOrderDB.sales_invoice.Add(sales_invoice);
                        }

                    }
                }

                SalesOrderDB.SaveChanges();
                btnCancel_Click(null, null);
            }
        }

        private void btnCancel_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = System.Windows.Visibility.Hidden;
            }
            catch { }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SalesOrderDB.app_contract.Where(a => a.is_active == true && a.id_company == entity.Properties.Settings.Default.company_ID).ToList();
            cbxContract.ItemsSource = SalesOrderDB.app_contract.Local;

            SalesOrderDB.app_condition.Where(a => a.is_active == true && a.id_company == entity.Properties.Settings.Default.company_ID).OrderBy(a => a.name).ToList();
            cbxCondition.ItemsSource = SalesOrderDB.app_condition.Local;

            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(SalesOrderDB, entity.App.Names.SalesOrder, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);

            stackMain.DataContext = project;
            cbxDocument.SelectedIndex = 0;
        }

        private void cbxCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxCondition.SelectedItem != null)
            {
                app_condition app_condition = cbxCondition.SelectedItem as app_condition;
                cbxContract.ItemsSource = SalesOrderDB.app_contract.Where(a => a.is_active == true
                                                                        && a.id_company == entity.Properties.Settings.Default.company_ID
                                                                        && a.id_condition == app_condition.id_condition).ToList();
                cbxContract.SelectedIndex = 0;
            }
        }

        private void cbxDocument_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxDocument.SelectedItem != null)
            {
                app_document_range app_document_range = cbxDocument.SelectedItem as app_document_range;
                app_document_range _app_range = SalesOrderDB.app_document_range.Where(x => x.id_range == app_document_range.id_range).FirstOrDefault();

                if (SalesOrderDB.app_branch.Where(x => x.id_branch == CurrentSession.Id_Branch).FirstOrDefault() != null)
                {
                    entity.Brillo.Logic.Range.branch_Code = SalesOrderDB.app_branch.Where(x => x.id_branch == CurrentSession.Id_Branch).FirstOrDefault().code;
                }

                if (SalesOrderDB.app_terminal.Where(x => x.id_terminal == CurrentSession.Id_Terminal).FirstOrDefault() != null)
                {
                    entity.Brillo.Logic.Range.terminal_Code = SalesOrderDB.app_terminal.Where(x => x.id_terminal == CurrentSession.Id_Terminal).FirstOrDefault().code;
                }

                if (SalesOrderDB.security_user.Where(x => x.id_user == CurrentSession.Id_User).FirstOrDefault() != null)
                {
                    entity.Brillo.Logic.Range.user_Code = SalesOrderDB.security_user.Where(x => x.id_user == CurrentSession.Id_User).FirstOrDefault().code;
                }

                txtnumber.Text = entity.Brillo.Logic.Range.calc_Range(_app_range, false);
            }
        }
    }
}
