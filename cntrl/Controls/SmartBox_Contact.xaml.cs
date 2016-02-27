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

                    ContactPopUp.IsOpen = false;

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

                    progBar.IsActive = true;

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

                List<entity.contact> results = new List<entity.contact>();

                results.AddRange(db.contacts
                .Where(x =>
                            x.id_company == company_ID &&
                            (
                                x.code.Contains(SearchText) ||
                                x.name.Contains(SearchText) ||
                                x.gov_code.Contains(SearchText)
                            )
                            &&
                            (
                                x.is_customer == Get_Customers ||
                                x.is_supplier == Get_Suppliers ||
                                x.is_employee == Get_Employees
                            )
                            &&
                            x.is_active == true
                        )
                .OrderBy(x => x.name)
                .ToList()
                );

                Dispatcher.InvokeAsync(new Action(() =>
                {
                    contactViewSource.Source = results;
                    Contact = contactViewSource.View.CurrentItem as entity.contact;

                    ContactPopUp.IsOpen = true;
                    progBar.IsActive = false;
                }));
            }
        }
    }
}
