using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using entity;
using System.Data.Entity;
using System.ComponentModel;
using entity.Brillo;

namespace Cognitivo.Project
{
    public partial class EventCosting : Page, INotifyPropertyChanged, IDisposable
    {
        entity.EventManagement.EventDB EventDB = new entity.EventManagement.EventDB();
        CollectionViewSource
            project_costingViewSource,
            project_costingproject_event_template_variable_detailsViewSource,
            project_costingservices_per_event_detailsViewSource,
            itemViewSource,
            contractViewSource = null;

        int IDcurrencyfx = 0;

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        //Total Property
        public decimal TotalCost { get; set; }
        public decimal CostForPerPersonServices { get; set; }
        public decimal CostForPerEventServices { get; set; }
        public decimal CostForHall { get; set; }

        public EventCosting()
        {
            InitializeComponent();
        }

        private async void project_costing_Loaded(object sender, RoutedEventArgs e)
        {
            itemViewSource = FindResource("itemViewSource") as CollectionViewSource;

            contractViewSource = FindResource("contractViewSource") as CollectionViewSource;
            contractViewSource.Source = CurrentSession.Get_Contract();

            CollectionViewSource conditionViewSource = FindResource("conditionViewSource") as CollectionViewSource;
            conditionViewSource.Source = CurrentSession.Get_Condition();
            
            CollectionViewSource template_designerViewSource = FindResource("template_designerViewSource") as CollectionViewSource;
            template_designerViewSource.Source = await EventDB.project_event_template.Where(a => a.is_active && a.id_company == CurrentSession.Id_Company).OrderBy(x => x.name).ToListAsync();

            project_costingViewSource = FindResource("project_costingViewSource") as CollectionViewSource;
            await EventDB.project_event.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).LoadAsync();
            project_costingViewSource.Source = EventDB.project_event.Local;
            project_costingproject_event_template_variable_detailsViewSource = FindResource("project_costingproject_event_template_variable_detailsViewSource") as CollectionViewSource;
            project_costingservices_per_event_detailsViewSource = FindResource("project_costingservices_per_event_detailsViewSource") as CollectionViewSource;

            CollectionViewSource app_document_rangeViewSource = FindResource("app_document_rangeViewSource") as CollectionViewSource;
            app_document_rangeViewSource.Source = entity.Brillo.Logic.Range.List_Range(EventDB, entity.App.Names.SalesBudget, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);

            EstimateCost();
        }

        #region toolbar events
        private void toolBar_btnNew_Click(object sender)
        {
            project_event project_costing = new project_event();
            project_costing.IsSelected = true;
            project_costing.State = EntityState.Added;
            project_costing.status = Status.Documents_General.Pending;
            EventDB.Entry(project_costing).State = EntityState.Added;

            EventDB.project_event.Add(project_costing);
            project_costingViewSource.View.Refresh();
            project_costingViewSource.View.MoveCurrentToLast();
            EstimateCost();
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            project_costingViewSource.View.MoveCurrentToFirst();
            EventDB.CancelAllChanges();
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            EventDB.SaveChanges();
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (project_costingDataGrid.SelectedItem != null)
            {
                project_event project_costing = (project_event)project_costingDataGrid.SelectedItem;
                project_costing.IsSelected = true;
                project_costing.State = EntityState.Modified;
                EventDB.Entry(project_costing).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select an Item");
            }
        }

        private void toolBar_btnSave_Click(object sender)
        {
            EventDB.SaveChanges();
        }
        #endregion

        #region DeleteChild
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as project_event_variable != null)
            {
                e.CanExecute = true;
            }
            else if (e.Parameter as project_event_fixed != null)
            {
                e.CanExecute = true;
            }
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (e.Parameter as project_event_variable != null)
                    {
                        //project_event_template_variable_detailsDataGrid.CancelEdit();
                        EventDB.project_event_variable.Remove(e.Parameter as project_event_variable);
                        project_costingproject_event_template_variable_detailsViewSource.View.Refresh();
                        EstimateCost();
                    }
                    else if (e.Parameter as project_event_fixed != null)
                    {
                        //services_per_event_detailsDataGrid.CancelEdit();
                        EventDB.project_event_fixed.Remove(e.Parameter as project_event_fixed);
                        project_costingservices_per_event_detailsViewSource.View.Refresh();
                        EstimateCost();
                    }
                }
            }
            catch { }
        }
        #endregion

        private void GetServices(object sender, RoutedEventArgs e)
        {
            toolBar_btnSave_Click(sender);
            toolBar_btnEdit_Click(sender);

            if (project_costingViewSource != null)
            {
                project_event project_event = project_costingViewSource.View.CurrentItem as project_event;
                //if (project_event.project_event_fixed.Count()==0)
                //{


                if (project_event != null && id_template_designerComboBox.SelectedItem != null)
                {
                    project_event_template project_event_template = id_template_designerComboBox.SelectedItem as project_event_template;
                    if (project_event_template.project_event_template_variable != null && project_event_template.project_event_template_variable.Count > 0)
                    {
                        // EventDB.project_event_variable.RemoveRange(project_event.project_event_variable);
                        foreach (project_event_template_variable person_service in project_event_template.project_event_template_variable)
                        {

                            item_tag item_tag = person_service.item_tag;
                            foreach (item_tag_detail tag_detail in item_tag.item_tag_detail)
                            {
                                if (tag_detail.item.is_active)
                                {
                                    if (project_event.project_event_variable.Where(x => x.id_item == tag_detail.item.id_item).Any())
                                    {
                                        project_event_variable project_event_variable = project_event.project_event_variable.Where(x => x.id_item == tag_detail.item.id_item).FirstOrDefault();
                                        project_event_variable.project_event = project_event;
                                        project_event_variable.adult_consumption = person_service.adult_consumption * project_event.quantity_adult;
                                        project_event_variable.child_consumption = person_service.child_consumption * project_event.quantity_child;

                                    }
                                    else
                                    {
                                        project_event_variable project_event_variable = new project_event_variable();
                                        project_event_variable.project_event = project_event;
                                        project_event_variable.item = tag_detail.item;
                                        project_event_variable.id_item = tag_detail.id_item;
                                        project_event_variable.item_tag = tag_detail.item_tag;
                                        project_event_variable.id_tag = tag_detail.id_tag;
                                        project_event_variable.adult_consumption = person_service.adult_consumption * project_event.quantity_adult;
                                        project_event_variable.child_consumption = person_service.child_consumption * project_event.quantity_child;
                                        project_event_variable.is_included = false;
                                        project_event.project_event_variable.Add(project_event_variable);
                                    }


                                }
                            }
                        }
                        project_costingproject_event_template_variable_detailsViewSource.View.Refresh();
                    }
                }

                if (project_event != null && id_template_designerComboBox.SelectedItem != null)
                {
                    project_event_template project_event_template = id_template_designerComboBox.SelectedItem as project_event_template;
                    if (project_event_template.project_event_template_fixed != null && project_event_template.project_event_template_fixed.Count > 0)
                    {
                        // EventDB.project_event_fixed.RemoveRange(project_event.project_event_fixed);
                        foreach (project_event_template_fixed project_event_template_fixed in project_event_template.project_event_template_fixed)
                        {
                            item_tag item_tag = project_event_template_fixed.item_tag;
                            foreach (item_tag_detail tag_detail in item_tag.item_tag_detail)
                            {
                                if (tag_detail.item.is_active)
                                {
                                    if (project_event.project_event_fixed.Where(x => x.id_item == tag_detail.item.id_item).Any())
                                    {
                                        project_event_fixed services_per_event_details = project_event.project_event_fixed.Where(x => x.id_item == tag_detail.item.id_item).FirstOrDefault();
                                        services_per_event_details.project_event = project_event;
                                        services_per_event_details.consumption = services_per_event_details.consumption;

                                    }
                                    else
                                    {
                                        project_event_fixed services_per_event_details = new project_event_fixed();
                                        services_per_event_details.project_event = project_event;
                                        services_per_event_details.item = tag_detail.item;
                                        services_per_event_details.id_item = tag_detail.id_item;
                                        services_per_event_details.item_tag = tag_detail.item_tag;
                                        services_per_event_details.id_tag = tag_detail.id_tag;
                                        services_per_event_details.consumption = 1;
                                        services_per_event_details.is_included = false;
                                        project_event.project_event_fixed.Add(services_per_event_details);
                                    }
                                }
                            }
                        }
                        project_costingservices_per_event_detailsViewSource.View.Refresh();
                    }
                }
                //}
                EstimateCost();
            }
        }

        private void EstimateCost()
        {
            List<Event> EventList = new List<Event>();
            if (project_costingViewSource.View.CurrentItem != null)
            {
                IDcurrencyfx = (project_costingViewSource.View.CurrentItem as project_event).id_currencyfx;

            }
            int adult_guest = 0;
            int child_guest = 0;
            try
            {
                adult_guest = Convert.ToInt32(guest_adultTextBox.Text);
                child_guest = Convert.ToInt32(guest_childTextBox.Text);
            }
            catch
            {
                adult_guest = 0;
                child_guest = 0;
            }

            TotalCost = 0;
            CostForPerPersonServices = 0;
            CostForPerEventServices = 0;
            CostForHall = 0;

            if (adult_guest > 0 || child_guest > 0)
            {
                project_event project_event = project_costingViewSource.View.CurrentItem as project_event;

                contact contact = EventDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();

                if (project_event != null)
                {
                    ICollection<project_event_variable> project_event_variableLIST = project_event.project_event_variable;
                    List<project_event_variable> Selectedlist = project_event_variableLIST.Where(x => x.is_included == true).ToList();
                    CostForPerPersonServices = Selectedlist.Sum(x => ((x.adult_consumption) + (x.child_consumption)) * get_Price(contact, IDcurrencyfx, x.item, entity.App.Modules.Sales));
                    foreach (project_event_variable item in Selectedlist)
                    {
                        Event Event = new Event();
                        Event.code = item.item.code;
                        Event.description = item.item.name;
                        Event.Quantity = item.adult_consumption + item.child_consumption;
                        Event.UnitPrice = get_Price(contact, IDcurrencyfx, item.item, entity.App.Modules.Sales);
                        Event.SubTotal = (item.adult_consumption + item.child_consumption) * get_Price(contact, IDcurrencyfx, item.item, entity.App.Modules.Sales);
                        EventList.Add(Event);
                    }

                    ICollection<project_event_fixed> project_event_fixedLIST = project_event.project_event_fixed;
                    List<project_event_fixed> Selectedeventlist = project_event_fixedLIST.Where(x => x.is_included == true).ToList();
                    CostForPerEventServices = Selectedeventlist.Sum(x => x.consumption * get_Price(contact, IDcurrencyfx, x.item, entity.App.Modules.Sales));

                    foreach (project_event_fixed item in Selectedeventlist)
                    {
                        Event Event = new Event();
                        Event.code = item.item.code;
                        Event.description = item.item.name;
                        Event.Quantity = item.consumption;
                        Event.UnitPrice = get_Price(contact, IDcurrencyfx, item.item, entity.App.Modules.Sales);
                        Event.SubTotal = item.consumption * get_Price(contact, IDcurrencyfx, item.item, entity.App.Modules.Sales);
                        EventList.Add(Event);
                    }

                    if (project_event.item != null)
                        CostForHall = get_Price(contact, IDcurrencyfx, project_event.item, entity.App.Modules.Sales);

                    Event EventForHall = new Event();
                    if (project_event.item != null)
                    {
                        EventForHall.code = project_event.item.code;
                        EventForHall.description = project_event.item.name;
                        EventForHall.Quantity = 1;
                        EventForHall.UnitPrice = get_Price(contact, IDcurrencyfx, project_event.item, entity.App.Modules.Sales);
                        EventForHall.SubTotal = get_Price(contact, IDcurrencyfx, project_event.item, entity.App.Modules.Sales);
                        EventList.Add(EventForHall);
                    }

                }
                TotalCost = CostForPerEventServices + CostForPerPersonServices + CostForHall;
            }
            dgvorder.ItemsSource = EventList;
            RaisePropertyChanged("TotalCost");
            RaisePropertyChanged("CostForPerEventServices");
            RaisePropertyChanged("CostForPerPersonServices");
            RaisePropertyChanged("CostForHall");
        }

        private void chkisIncluded_Checked(object sender, RoutedEventArgs e)
        {
            EstimateCost();
        }
        
        private void id_template_designerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (id_template_designerComboBox.SelectedItem != null)
            {
                project_event_template template_designer = id_template_designerComboBox.SelectedItem as project_event_template;
                itemViewSource.Source = EventDB.items.Where(x => EventDB.item_tag_detail.Where(y => y.id_tag == template_designer.id_tag).Select(z => z.id_item).Contains(x.id_item) && x.is_active ).ToList();
            }
        }

        private void guestTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EstimateCost();
            }
        }

        private void Consumption_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EstimateCost();
            }
        }

        #region Add/Edit Contact
        private void AddContact_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void EditContact_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

        }
        #endregion

        #region Plase Order
        private void btnPlaceOrder_Click(object sender, RoutedEventArgs e)
        {

            id_conditionComboBox.SelectedIndex = -1;
            id_contractComboBox.SelectedIndex = -1;
            crud_modal.Visibility = System.Windows.Visibility.Visible;
        }

        private void lblCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            crud_modal.Visibility = System.Windows.Visibility.Hidden;
        }

        private void saveOrder_Click(object sender, RoutedEventArgs e)
        {
            project_event project_costing = project_costingViewSource.View.CurrentItem as project_event;
            if (project_costing != null)
            {
                contact contact = EventDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                app_condition app_condition = id_conditionComboBox.SelectedItem as app_condition;
                app_contract app_contract = id_contractComboBox.SelectedItem as app_contract;


                if (contact != null && app_condition != null && app_contract != null)
                {
                    using (SalesBudgetDB db = new SalesBudgetDB())
                    {
                        sales_budget sales_budget = new sales_budget();
                        sales_budget.IsSelected = true;
                        sales_budget.State = EntityState.Added;
                        sales_budget.status = Status.Documents_General.Pending;
                        sales_budget.id_contact = contact.id_contact;
                        sales_budget.contact = db.contacts.Where(x => x.id_contact == contact.id_contact).FirstOrDefault();

                        if (CurrentSession.Id_Branch > 0)
                            sales_budget.id_branch = CurrentSession.Id_Branch;
                        else
                            sales_budget.id_branch = db.app_branch.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).FirstOrDefault().id_branch;

                        sales_budget.id_condition = app_condition.id_condition;
                        sales_budget.id_contract = app_contract.id_contract;
                        sales_budget.id_currencyfx = project_costing.id_currencyfx;


                        foreach (project_event_variable project_event_variable in project_costing.project_event_variable.Where(a => a.is_included == true))
                        {
                            sales_budget_detail sales_budget_detail = new sales_budget_detail();
                            sales_budget_detail.sales_budget = sales_budget;
                            sales_budget_detail.item = db.items.Where(a => a.id_item == project_event_variable.id_item).FirstOrDefault();
                            sales_budget_detail.item_description = sales_budget_detail.item.name;
                            sales_budget_detail.id_item = project_event_variable.id_item;
                            sales_budget_detail.id_vat_group = project_event_variable.item.id_vat_group;
                            sales_budget_detail.quantity = ((project_event_variable.adult_consumption) + (project_event_variable.child_consumption));
                            sales_budget_detail.UnitPrice_Vat = Math.Round(get_Price(contact, IDcurrencyfx, sales_budget_detail.item, entity.App.Modules.Sales));
                            sales_budget.sales_budget_detail.Add(sales_budget_detail);
                        }

                        foreach (project_event_fixed project_event_fixed in project_costing.project_event_fixed.Where(a => a.is_included == true))
                        {
                            sales_budget_detail sales_budget_detail = new sales_budget_detail();
                            sales_budget_detail.sales_budget = sales_budget;
                            sales_budget_detail.item = db.items.Where(a => a.id_item == project_event_fixed.id_item).FirstOrDefault();
                            sales_budget_detail.id_item = project_event_fixed.id_item;
                            sales_budget_detail.id_vat_group = project_event_fixed.item.id_vat_group;
                            sales_budget_detail.quantity = project_event_fixed.consumption;
                            sales_budget_detail.UnitPrice_Vat = Math.Round(get_Price(contact, IDcurrencyfx, sales_budget_detail.item, entity.App.Modules.Sales));
                            sales_budget.sales_budget_detail.Add(sales_budget_detail);
                        }

                        sales_budget_detail sales_budget_detail_hall = new sales_budget_detail();
                        sales_budget_detail_hall.sales_budget = sales_budget;
                        sales_budget_detail_hall.item = db.items.Where(a => a.id_item == project_costing.id_item).FirstOrDefault();
                        sales_budget_detail_hall.id_item = project_costing.id_item;
                        sales_budget_detail_hall.id_vat_group = project_costing.item.id_vat_group;
                        sales_budget_detail_hall.quantity = 1;
                        sales_budget_detail_hall.UnitPrice_Vat = Math.Round(get_Price(contact, IDcurrencyfx, sales_budget_detail_hall.item, entity.App.Modules.Sales));
                        sales_budget.sales_budget_detail.Add(sales_budget_detail_hall);

                        db.sales_budget.Add(sales_budget);
                        db.SaveChanges();
                    }

                    lblCancel_MouseDown(null, null);
                }
                else
                {
                    if (MessageBox.Show("VALIDATION EXCEPTION : Please fill up all the fields", "Cognitivo", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk) == MessageBoxResult.Cancel)
                    {
                        lblCancel_MouseDown(null, null);
                    }
                }
            }
        }
        private void id_conditionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            app_condition app_condition = id_conditionComboBox.SelectedItem as app_condition;
            if (app_condition != null)
            {
                contractViewSource.View.Filter = a =>
                {
                    app_contract app_contract = a as app_contract;
                    if (app_contract.id_condition == app_condition.id_condition)
                        return true;
                    else
                        return false;
                };
                id_contractComboBox.SelectedIndex = 0;
            }
        }
        #endregion

        public decimal get_Price(contact contact, int id_currencyfx, item item, entity.App.Modules Module)
        {
            if (item != null && contact != null)
            {
                //Step 1. Get Price List.
                int id_priceList = 0;
                if (contact.id_price_list != null)
                {
                    id_priceList = (int)contact.id_price_list; //Get Price List from Contact.
                }
                else
                {
                    id_priceList = get_Default(item.id_company).id_price_list; //Get Price List from Default, because Contact has no values.
                }

                //Step 1 1/2. Check if Quantity gets us a better Price List.
                //Step 2. Get Price in Currency.
                int id_currency = 0;
                using (db db = new db())
                {
                    if (db.app_currencyfx.Where(x => x.id_currencyfx == id_currencyfx).FirstOrDefault() != null)
                    {
                        id_currency = db.app_currencyfx.Where(x => x.id_currencyfx == id_currencyfx).FirstOrDefault().id_currency;
                    }
                }

                item_price item_price = item.item_price.Where(x => x.id_currency == id_currency
                                    && x.id_price_list == id_priceList).FirstOrDefault();

                if (item_price != null)
                {
                    return item_price.valuewithVAT;
                    //  return Currency.convert_Value(item_price_value, id_currencyfx, entity.App.Modules.Sales);
                }
                else
                {
                    decimal item_price_value = 0;
                    //decimal currencyfx = Currency.get_specificRate(id_currencyfx, application);
                    if (item.item_price.Where(x => x.id_price_list == id_priceList) != null && item.item_price.Where(x => x.id_price_list == id_priceList).Count() > 0)
                    {
                        item_price = item.item_price.Where(y => y.id_price_list == id_priceList).FirstOrDefault();
                        item_price_value = item_price.valuewithVAT;
                        using (db db = new db())
                        {

                            if (db.app_currency.Where(x => x.id_currency == item_price.id_currency).FirstOrDefault() != null)
                            {
                                if (db.app_currency.Where(x => x.id_currency == item_price.id_currency).FirstOrDefault().app_currencyfx.Where(x => x.is_active).FirstOrDefault() != null)
                                {
                                    return Currency.convert_Values(item_price_value, db.app_currency.Where(x => x.id_currency == item_price.id_currency).FirstOrDefault().app_currencyfx.Where(x => x.is_active).FirstOrDefault().id_currencyfx, id_currencyfx, Module);

                                }
                            }
                        }

                    }
                    //return Currency.convert_Value(item_price_value, id_currencyfx, Module);            

                }
            }
            return 0;
        }

        public item_price get_Default(int? id_company)
        {
            item_price item_price = new item_price();
            using (db db = new db())
            {
                if (db.item_price.Where(x => x.item_price_list.is_active == true && x.item_price_list.id_company == id_company) != null)
                {
                    item_price = db.item_price.Where(x => x.item_price_list.is_active == true
                                                            && x.item_price_list.id_company == id_company).FirstOrDefault();
                }
                else
                {
                    item_price = db.item_price.Where(x => x.item_price_list.id_company == id_company).FirstOrDefault();
                }
            }
            return item_price;
        }

        private void btnPlaceProject_Click(object sender, RoutedEventArgs e)
        {
            //using (ProjectDB db = new ProjectDB())
            //{
            project project = new project();

            contact contact = EventDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
            project.id_contact = contact.id_contact;

            project.IsSelected = true;
            project.id_branch = CurrentSession.Id_Branch;
            project.name = txtName.Text;
            project.est_start_date = timestampCalendar.SelectedDate;
            project.est_end_date = timestampCalendar.SelectedDate;
            project.priority = 0;

            project_event project_costing = project_costingViewSource.View.CurrentItem as project_event;
            if (project_costing != null)
            {
                foreach (project_event_variable project_event_variable in project_costing.project_event_variable.Where(a => a.is_included == true))
                {
                    item item = project_event_variable.item;
                    if (item != null && item.id_item > 0 && item.is_autorecepie)
                    {
                        project_task project_task = new project_task();
                        project_task.status = Status.Project.Pending;
                        //project_task.project = project;
                        project_task.id_item = project_event_variable.id_item;
                        project_task.item_description = project_event_variable.item.name;
                        project_task.code = project_event_variable.item.code;
                        project_task.quantity_est = ((project_event_variable.adult_consumption) + (project_event_variable.child_consumption));
                        project_task.unit_cost_est = get_Price(contact, IDcurrencyfx, project_event_variable.item, entity.App.Modules.Sales);


                        if (item.item_recepie.Count() > 0)
                        {
                            foreach (item_recepie_detail item_recepie_detail in item.item_recepie.FirstOrDefault().item_recepie_detail)
                            {
                                project_task Subproject_task = new project_task();

                                Subproject_task.code = item_recepie_detail.item.code;
                                Subproject_task.item_description = item_recepie_detail.item.name;
                                Subproject_task.id_item = item_recepie_detail.item.id_item;
                                Subproject_task.items = item_recepie_detail.item;
                                project_task.status = Status.Project.Pending;
                                Subproject_task.RaisePropertyChanged("item");
                                if (item_recepie_detail.quantity > 0)
                                {
                                    Subproject_task.quantity_est = (decimal)item_recepie_detail.quantity * project_task.quantity_est;
                                }
                                project_task.child.Add(Subproject_task);
                            }
                        }


                        project.project_task.Add(project_task);
                    }
                    else
                    {
                        project_task project_task = new project_task();
                        project_task.status = Status.Project.Pending;
                        //project_task.project = project;
                        project_task.id_item = project_event_variable.id_item;
                        project_task.item_description = project_event_variable.item.name;
                        project_task.code = project_event_variable.item.code;
                        project_task.quantity_est = ((project_event_variable.adult_consumption) + (project_event_variable.child_consumption));
                        project_task.unit_cost_est = get_Price(contact, IDcurrencyfx, project_event_variable.item, entity.App.Modules.Sales);
                        project.project_task.Add(project_task);



                    }

                }

                foreach (project_event_fixed per_event_service in project_costing.project_event_fixed.Where(a => a.is_included == true))
                {
                    item item = per_event_service.item;
                    if (item != null && item.id_item > 0 && item.is_autorecepie)
                    {
                        project_task project_task = new project_task();
                        project_task.status = Status.Project.Approved;
                        //project_task.project = project;
                        project_task.id_item = per_event_service.id_item;
                        project_task.item_description = per_event_service.item.name;
                        project_task.code = per_event_service.item.code;
                        project_task.quantity_est = per_event_service.consumption;
                        project_task.unit_cost_est = get_Price(contact, IDcurrencyfx, per_event_service.item, entity.App.Modules.Sales);


                        foreach (item_recepie_detail item_recepie_detail in item.item_recepie.FirstOrDefault().item_recepie_detail)
                        {
                            project_task Subproject_task = new project_task();

                            Subproject_task.code = item_recepie_detail.item.name;
                            Subproject_task.status = Status.Project.Approved;

                            Subproject_task.item_description = item_recepie_detail.item.name;
                            Subproject_task.id_item = item_recepie_detail.item.id_item;
                            Subproject_task.items = item_recepie_detail.item;
                            Subproject_task.RaisePropertyChanged("item");
                            if (item_recepie_detail.quantity > 0)
                            {
                                Subproject_task.quantity_est = (decimal)item_recepie_detail.quantity * project_task.quantity_est;
                            }

                            project_task.child.Add(Subproject_task);
                        }
                        project.project_task.Add(project_task);
                    }
                    else
                    {
                        project_task project_task = new project_task();
                        project_task.status = Status.Project.Approved;
                        //project_task.project = project;
                        project_task.id_item = per_event_service.id_item;
                        project_task.item_description = per_event_service.item.name;
                        project_task.code = per_event_service.item.code;
                        project_task.quantity_est = per_event_service.consumption;
                        project_task.unit_cost_est = get_Price(contact, IDcurrencyfx, per_event_service.item, entity.App.Modules.Sales);
                        project.project_task.Add(project_task);
                    }
                }
                item _item = project_costing.item;

                if (_item != null && _item.id_item > 0 && _item.is_autorecepie)
                {
                    project_task _project_task = new project_task();
                    _project_task.status = Status.Project.Approved;
                    _project_task.id_item = project_costing.id_item;
                    _project_task.item_description = project_costing.item.name;
                    _project_task.code = project_costing.item.code;
                    _project_task.quantity_est = 1;
                    _project_task.unit_cost_est = get_Price(contact, IDcurrencyfx, project_costing.item, entity.App.Modules.Sales);

                    foreach (item_recepie_detail item_recepie_detail in _item.item_recepie.FirstOrDefault().item_recepie_detail)
                    {
                        project_task Subproject_task = new project_task();

                        Subproject_task.code = item_recepie_detail.item.name;
                        Subproject_task.item_description = item_recepie_detail.item.name;
                        Subproject_task.id_item = item_recepie_detail.item.id_item;
                        Subproject_task.items = item_recepie_detail.item;
                        Subproject_task.status = Status.Project.Approved;
                        Subproject_task.RaisePropertyChanged("item");

                        if (item_recepie_detail.quantity > 0)
                        {
                            Subproject_task.quantity_est = (decimal)item_recepie_detail.quantity * _project_task.quantity_est;
                        }
                        
                        _project_task.child.Add(Subproject_task);
                    }
                    project.project_task.Add(_project_task);
                }
                else
                {
                    project_task _project_task = new project_task();
                    _project_task.status = Status.Project.Approved;
                    _project_task.id_item = project_costing.id_item;
                    _project_task.item_description = project_costing.item.name;
                    _project_task.code = project_costing.item.code;
                    _project_task.quantity_est = 1;
                    _project_task.unit_cost_est = get_Price(contact, IDcurrencyfx, project_costing.item, entity.App.Modules.Sales);
                    project.project_task.Add(_project_task);
                }

                project_costing.status = Status.Documents_General.Approved;

                EventDB.projects.Add(project);
                EventDB.SaveChanges();
            }
        }

        public void get_ActiveRateXContact(ref contact contact)
        {
            app_currencyfx app_currencyfx = null;
            if (contact.app_currency != null && contact.app_currency.app_currencyfx != null && contact.app_currency.app_currencyfx.Count > 0)
                app_currencyfx = contact.app_currency.app_currencyfx.Where(a => a.is_active == true).First();
            if (app_currencyfx != null && app_currencyfx.id_currencyfx > 0)
            {
                cbxCurrency.SelectedValue = EventDB.app_currencyfx.Where(x => x.id_currency == app_currencyfx.id_currency && x.is_active).FirstOrDefault().id_currencyfx;
            }
            else
            {
                cbxCurrency.SelectedValue = EventDB.app_currencyfx.Where(x => x.app_currency.is_priority && x.is_active).FirstOrDefault().id_currencyfx;
            }

        }

        public void Dispose()
        {
            EventDB.Dispose();
        }

        private void cbxCurrency_LostFocus(object sender, RoutedEventArgs e)
        {
            EstimateCost();
        }
        public class Event
        {
            public string code { get; set; }
            public string description { get; set; }
            public decimal Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal SubTotal { get; set; }
        }

        private void dgvServicePerEvent_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

            if (e.Column.DisplayIndex == 0 || e.Column.DisplayIndex == 3)
            {
                EstimateCost();
            }
        }

        private void dgvServicePerperson_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.DisplayIndex == 0 || e.Column.DisplayIndex == 3 || e.Column.DisplayIndex == 4)
            {
                EstimateCost();
            }
        }

        private void item_Select(object sender, RoutedEventArgs e)
        {
            if (sbxItem.ItemID > 0)
            {
                if (project_costingViewSource != null)
                {
                    project_event project_event = project_costingViewSource.View.CurrentItem as project_event;

                    item item = EventDB.items.Where(x => x.id_item == sbxItem.ItemID).FirstOrDefault();
                    project_event_variable project_event_variable = new project_event_variable();
                    project_event_variable.project_event = project_event;
                    project_event_variable.is_included = true;
                    project_event_variable.item = item;
                    project_event_variable.id_item = item.id_item;
                    
                    if (item.item_tag_detail.Count() > 0)
                    {
                        if (item.item_tag_detail.FirstOrDefault().item_tag != null)
                        {
                            project_event_variable.item_tag = item.item_tag_detail.FirstOrDefault().item_tag;
                            project_event_variable.id_tag = item.item_tag_detail.FirstOrDefault().item_tag.id_tag;
                        }
                    }
                    else
                    {
                        toolBar.msgWarning("Item needs Tag");
                        return; 
                    }

                    project_event_variable.adult_consumption = 1;
                    project_event_variable.child_consumption = 1;
                    project_event_variable.is_included = false;
                    project_event.project_event_variable.Add(project_event_variable);
                }
                project_costingproject_event_template_variable_detailsViewSource.View.Refresh();
                toolBar_btnSave_Click(sender);
                toolBar_btnEdit_Click(sender);
                EstimateCost();
            }
        }

        private void sbxItemevent_Select(object sender, RoutedEventArgs e)
        {
            if (sbxItemevent.ItemID > 0)
            {
                if (project_costingViewSource != null)
                {
                    project_event project_event = project_costingViewSource.View.CurrentItem as project_event;

                    item item = EventDB.items.Where(x => x.id_item == sbxItemevent.ItemID).FirstOrDefault();


                    project_event_fixed services_per_event_details = new project_event_fixed();
                    services_per_event_details.project_event = project_event;
                    services_per_event_details.is_included = true;
                    services_per_event_details.item = item;
                    services_per_event_details.id_item = item.id_item;
                    if (item.item_tag_detail.Count() > 0)
                    {
                        if (item.item_tag_detail.FirstOrDefault().item_tag != null)
                        {
                            services_per_event_details.item_tag = item.item_tag_detail.FirstOrDefault().item_tag;
                            services_per_event_details.id_tag = item.item_tag_detail.FirstOrDefault().item_tag.id_tag;
                        }

                    }
                    services_per_event_details.consumption = 1;

                    services_per_event_details.is_included = false;
                    project_event.project_event_fixed.Add(services_per_event_details);
                }
                project_costingservices_per_event_detailsViewSource.View.Refresh();
                toolBar_btnSave_Click(sender);
                toolBar_btnEdit_Click(sender);
                EstimateCost();


            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (project_costingproject_event_template_variable_detailsViewSource != null)
            {
                if (project_costingproject_event_template_variable_detailsViewSource.View != null)
                {

                    project_costingproject_event_template_variable_detailsViewSource.View.Filter = i =>
                    {
                        project_event_variable project_event_variable = (project_event_variable)i;
                        if (project_event_variable.item.name.ToUpper().Contains(txtsearch.Text.ToUpper()) || project_event_variable.item_tag.name.ToUpper().Contains(txtsearch.Text.ToUpper()))
                            return true;
                        else
                            return false;
                    };

                }
            }
        }

        private void txtsearchevent_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (project_costingservices_per_event_detailsViewSource != null)
            {
                if (project_costingservices_per_event_detailsViewSource.View != null)
                {

                    project_costingservices_per_event_detailsViewSource.View.Filter = i =>
                    {
                        project_event_fixed project_event_fixed = (project_event_fixed)i;
                        if (project_event_fixed.item.name.ToUpper().Contains(txtsearchevent.Text.ToUpper()) || project_event_fixed.item_tag.name.ToUpper().Contains(txtsearchevent.Text.ToUpper()))
                            return true;
                        else
                            return false;
                    };

                }
            }
        }

        private void sbxContact_Select(object sender, RoutedEventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                project_event project_event = project_costingViewSource.View.CurrentItem as project_event;
                contact contact = EventDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                project_event.id_contact = contact.id_contact;
                get_ActiveRateXContact(ref contact);
            }

        }

        private void project_costingDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            project_event project_event = project_costingViewSource.View.CurrentItem as project_event;

            if (project_event != null)
            {
                if (project_event.id_contact!=null)
                {
                    sbxContact.ContactID = (int)project_event.id_contact;
                    if (project_costingproject_event_template_variable_detailsViewSource!=null && project_costingservices_per_event_detailsViewSource!=null )
                    {
                        if (project_costingproject_event_template_variable_detailsViewSource.View != null && project_costingservices_per_event_detailsViewSource.View != null)
                        { GetServices(sender, null); }
                        
                    }
                    
                }
              
            }
        }



    }
}
