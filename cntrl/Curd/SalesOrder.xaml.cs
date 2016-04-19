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
    /// <summary>
    /// Interaction logic for SalesOrder.xaml
    /// </summary>
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
        public SalesOrderDB db { get; set; }
        public Boolean Generate_Invoice { get; set; }

        public SalesOrder()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (project != null)
            {
                List<project_task> project_task = project.project_task.Where(x => x.IsSelected).ToList();

                sales_order sales_order = new entity.sales_order();
                sales_order.id_contact = (int)project.id_contact;
                sales_order.contact = db.contacts.Where(x => x.id_contact == (int)project.id_contact).FirstOrDefault();
                sales_order.id_range = (int)cbxDocument.SelectedValue;
                sales_order.id_project = project.id_project;
                sales_order.id_condition = (int)cbxCondition.SelectedValue;
                sales_order.id_contract = (int)cbxContract.SelectedValue;
                sales_order.id_currencyfx = (int)cbxCurrency.SelectedValue;
                sales_order.comment = "Project -> " + project.name;

                sales_order_detail sales_order_detail = null;

                foreach (project_task _project_task in project_task)
                {
                    if (_project_task.items.id_item_type == item.item_type.Task)
                    {
                        sales_order_detail = new sales_order_detail();
                        sales_order_detail.id_sales_order = sales_order.id_sales_order;
                        sales_order_detail.sales_order = sales_order;
                        sales_order_detail.id_item = (int)_project_task.id_item;
                        sales_order_detail.item_description = _project_task.item_description;
                        sales_order_detail.quantity = (decimal)(_project_task.quantity_est == null ? 0M : _project_task.quantity_est);
                        sales_order_detail.UnitPrice_Vat = (decimal)(_project_task.unit_price_vat == null ? 0M : _project_task.unit_price_vat);
                        sales_order_detail.id_project_task = _project_task.id_project_task;
                        _project_task.IsSelected = false;
                    }
                    else
                    {
                        if (sales_order_detail != null)
                        {
                            sales_order_detail.id_project_task = _project_task.id_project_task; ;
                            _project_task.IsSelected = false;
                        }
                    }
                    if (sales_order_detail!=null)
                    {
                        sales_order.sales_order_detail.Add(sales_order_detail);
                    }
                  
                }

                sales_order.State = EntityState.Added;
                sales_order.IsSelected = true;
                if (sales_order.sales_order_detail.Count()==0)
                {
                    return;
                }
                db.sales_order.Add(sales_order);
                db.SaveChanges();
               
                if (Generate_Invoice)
                {
                    db.Approve();
                    sales_invoice sales_invoice = new entity.sales_invoice();
                    sales_invoice.id_contact = (int)project.id_contact;
                    sales_invoice.contact = db.contacts.Where(x => x.id_contact == (int)project.id_contact).FirstOrDefault();
                    sales_invoice.sales_order = sales_order;
                    sales_invoice.id_project = project.id_project;
                    sales_invoice.id_condition = (int)cbxCondition.SelectedValue;
                    sales_invoice.id_contract = (int)cbxContract.SelectedValue;
                    sales_invoice.id_currencyfx = (int)cbxCurrency.SelectedValue;
                    sales_invoice.comment = "Project -> " + project.name;
                    sales_invoice.trans_date = DateTime.Now;
                    sales_invoice_detail sales_invoice_detail = null;

                    foreach (project_task _project_task in project_task)
                    {
                        if (_project_task.items.id_item_type == item.item_type.Task)
                        {
                            sales_invoice_detail = new sales_invoice_detail();
                            sales_invoice_detail.id_sales_invoice = sales_invoice.id_sales_invoice;
                            sales_invoice_detail.sales_invoice = sales_invoice;
                            sales_invoice_detail.id_item = (int)_project_task.id_item;
                            sales_invoice_detail.item_description = _project_task.item_description;
                            sales_invoice_detail.quantity = (decimal)(_project_task.quantity_est == null ? 0M : _project_task.quantity_est);
                            sales_invoice_detail.UnitPrice_Vat = (decimal)(_project_task.unit_price_vat == null ? 0M : _project_task.unit_price_vat);
                            sales_invoice_detail.id_project_task = _project_task.id_project_task;
                            _project_task.IsSelected = false;
                        }
                        else
                        {
                            if (sales_invoice_detail != null)
                            {
                                sales_invoice_detail.id_project_task = _project_task.id_project_task;
                                _project_task.IsSelected = false;
                            }
                        }
                        sales_invoice.sales_invoice_detail.Add(sales_invoice_detail);
                    }

                    sales_invoice.State = EntityState.Added;
                    sales_invoice.IsSelected = true;
                    crm_opportunity crm_opportunity = sales_order.crm_opportunity;
                    crm_opportunity.sales_invoice.Add(sales_invoice);
                    db.crm_opportunity.Attach(crm_opportunity);
                    db.sales_invoice.Add(sales_invoice);
                }
                db.SaveChanges();
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
            catch (Exception ex)
            { throw ex; }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            db.app_contract.Where(a => a.is_active == true && a.id_company == entity.Properties.Settings.Default.company_ID).ToList();
            cbxContract.ItemsSource = db.app_contract.Local;

            db.app_condition.Where(a => a.is_active == true && a.id_company == entity.Properties.Settings.Default.company_ID).OrderBy(a => a.name).ToList();
            cbxCondition.ItemsSource = db.app_condition.Local;

            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(entity.App.Names.SalesOrder, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);

            stackMain.DataContext = project;
            cbxDocument.SelectedIndex = 0;
        }

        private void cbxCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxCondition.SelectedItem != null)
            {

                app_condition app_condition = cbxCondition.SelectedItem as app_condition;
                cbxContract.ItemsSource = db.app_contract.Where(a => a.is_active == true
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
                app_document_range _app_range = db.app_document_range.Where(x => x.id_range == app_document_range.id_range).FirstOrDefault();

                if (db.app_branch.Where(x => x.id_branch == CurrentSession.Id_Branch).FirstOrDefault() != null)
                {
                    entity.Brillo.Logic.Range.branch_Code = db.app_branch.Where(x => x.id_branch == CurrentSession.Id_Branch).FirstOrDefault().code;
                }
                if (db.app_terminal.Where(x => x.id_terminal == CurrentSession.Id_Terminal).FirstOrDefault() != null)
                {
                    entity.Brillo.Logic.Range.terminal_Code = db.app_terminal.Where(x => x.id_terminal == CurrentSession.Id_Terminal).FirstOrDefault().code;
                }
                if (db.security_user.Where(x => x.id_user == CurrentSession.Id_User).FirstOrDefault() != null)
                {
                    entity.Brillo.Logic.Range.user_Code = db.security_user.Where(x => x.id_user == CurrentSession.Id_User).FirstOrDefault().code;
                }

                txtnumber.Text = entity.Brillo.Logic.Range.calc_Range(_app_range, false);
            }
        }
    }
}
