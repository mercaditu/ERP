using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace cntrl.Controls
{
    public partial class SmartBox_Contact : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SmartBox_Contact));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        entity.dbContext db = new entity.dbContext();
        public event RoutedEventHandler Select;
        private void ContactGrid_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (contactViewSource.View != null)
            {
                Contact = contactViewSource.View.CurrentItem as entity.contact;

                if (Contact != null)
                {
                    ContactID = Contact.id_contact;
                    Text = Contact.name;

                    popContact.IsOpen = false;

                    if (Select != null)
                    { Select(this, new RoutedEventArgs()); }
                }
            }
        }

        public int ContactID { get; set; }
        public bool Get_Customers { get; set; }
        public bool Get_Suppliers { get; set; }
        public bool Get_Employees { get; set; }
        public bool Get_Users { get; set; }

        public entity.contact Contact { get; set; }

        int company_ID = entity.Properties.Settings.Default.company_ID;

        Task taskSearch;
        CancellationTokenSource tokenSource;
        CancellationToken token;

        CollectionViewSource contactViewSource;
       
        public SmartBox_Contact()
        {
            InitializeComponent();
            contactViewSource = ((CollectionViewSource)(FindResource("contactViewSource")));
        }

        private void StartSearch(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ContactGrid_MouseDoubleClick(sender, e);
            }

            else if(e.Key == Key.Up)
            {
                contactViewSource.View.MoveCurrentToPrevious();
                contactViewSource.View.Refresh();
            }
            else if (e.Key == Key.Down)
            {
                contactViewSource.View.MoveCurrentToNext();
                contactViewSource.View.Refresh();
            }
            else
            {
                string SearchText = tbxSearch.Text;

                if (SearchText.Count() >= 3)
                {
                    if (taskSearch != null)
                    {
                        if (taskSearch.Status == TaskStatus.Running)
                        {
                            tokenSource.Cancel();
                        }
                    }

                    tokenSource = new CancellationTokenSource();
                    token = tokenSource.Token;
                    taskSearch = Task.Factory.StartNew(() => Search_OnThread(SearchText), token);
                }
            }
        }

       

        private void Search_OnThread(string SearchText)
        {
            using(entity.db db = new entity.db())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.AutoDetectChangesEnabled = false;

                var param = smartBoxContactSetting.Default.SearchFilter;

                List<entity.contact> results = new List<entity.contact>();
                var predicate = PredicateBuilder.True<entity.contact>();

                if (Get_Customers)
                {
                    predicate = predicate.And(x => x.is_customer == true);
                }

                if (Get_Suppliers)
                {
                    predicate = predicate.And(x => x.is_supplier == true);
                }

                if (Get_Employees)
                {
                    predicate = predicate.And(x => x.is_employee == true);
                }

                if (param.Contains("Code"))
                {
                    predicate = predicate.Or(x => x.code == SearchText);
                }

                if (param.Contains("Name"))
                {
                    predicate = predicate.Or(x => x.name == SearchText);
                }

                if (param.Contains("GovID"))
                {
                    predicate = predicate.Or(x => x.gov_code == SearchText);
                }

                if (param.Contains("Tel"))
                {
                    predicate = predicate.Or(x => x.telephone == SearchText);
                }

                predicate = predicate.And(x => x.contact_role.can_transact);
                predicate = predicate.And(x => x.is_active);
                

                results.AddRange(db.contacts
                   .Where(predicate).OrderBy(x=>x.name).ToList());
                Dispatcher.InvokeAsync(new Action(() =>
                {
                    contactViewSource.Source = results;
                    Contact = contactViewSource.View.CurrentItem as entity.contact;

                    popContact.IsOpen = true;
                }));
            }
        }

        private void Add_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            crudContact.contactobject = new entity.contact();
            
            crudContact.entity = db;
            popCrud.IsOpen = true;

            popCrud.Visibility = Visibility.Visible;
        }

        private void crudContact_btnSave_Click(object sender)
        {
                if (crudContact.contactList.Count()>0)
                {
                    foreach (entity.contact contact in crudContact.contactList)
                    {
                        if (contact.id_contact==0)
                        {
                            db.db.contacts.Add(contact);
                        }
                    }
                    db.SaveChanges();
                }
            popCrud.IsOpen = false;
            popCrud.Visibility = System.Windows.Visibility.Collapsed;
        }

      

        private void Edit_PreviewMouseUp_1(object sender, MouseButtonEventArgs e)
        {
            crudContact.contactobject = Contact;
            crudContact.entity = db;
            popCrud.IsOpen = true;
            popCrud.Visibility = System.Windows.Visibility.Visible;
        }

        private void Label_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            popupCustomize.IsOpen = true;
            popupCustomize.Visibility = System.Windows.Visibility.Visible;
        }

        private void popupCustomize_Closed(object sender, EventArgs e)
        {
            Controls.smartBoxContactSetting.Default.Save();
            popupCustomize.IsOpen = false;
            popupCustomize.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Controls.smartBoxContactSetting.Default.SearchFilter.Clear();

            if (rbtnCode.IsChecked==true)
            {
                Controls.smartBoxContactSetting.Default.SearchFilter.Add("Code");
            }
            if (rbtnName.IsChecked==true)
            {
                Controls.smartBoxContactSetting.Default.SearchFilter.Add("Name");
            }
            if (rbtnGov_ID.IsChecked == true)
            {
                Controls.smartBoxContactSetting.Default.SearchFilter.Add("GovID");
            }
            if (rbtnTel.IsChecked == true)
            {
                Controls.smartBoxContactSetting.Default.SearchFilter.Add("Tel");
            }

            Controls.smartBoxContactSetting.Default.Save();
        }

        private void _SmartBox_Contact_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ContactID = 0;
        }
    }
}
