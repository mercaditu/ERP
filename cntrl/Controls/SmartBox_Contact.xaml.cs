using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace cntrl.Controls
{
    public partial class SmartBox_Contact : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SmartBox_Contact));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
        public bool can_New
        {
            get { return _can_new; }
            set
            {
                entity.Brillo.Security Sec = new entity.Brillo.Security(entity.App.Names.Items);
                if (Sec.create)
                {
                    _can_new = value;
                    RaisePropertyChanged("can_New");
                }
                else
                {
                    _can_new = false;
                    RaisePropertyChanged("can_New");
                }
            }
        }
        bool _can_new;
        public bool can_Edit
        {
            get { return _can_new; }
            set
            {
                entity.Brillo.Security Sec = new entity.Brillo.Security(entity.App.Names.Items);
                if (Sec.edit)
                {
                    _can_edit = value;
                    RaisePropertyChanged("can_Edit");
                }
                else
                {
                    _can_edit = false;
                    RaisePropertyChanged("can_Edit");
                }
            }
        }
        bool _can_edit;

        //entity.dbContext db = new entity.dbContext();

        public event RoutedEventHandler Select;
        private void ContactGrid_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (contactViewSource.View != null)
            {
                entity.contact Contact = contactViewSource.View.CurrentItem as entity.contact;

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

        //public entity.contact Contact { get; set; }

        Task taskSearch;
        CancellationTokenSource tokenSource;
        CancellationToken token;

        CollectionViewSource contactViewSource;

        public SmartBox_Contact()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            InitializeComponent();
            this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(LoginControl_IsVisibleChanged);
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                contactViewSource = ((CollectionViewSource)(FindResource("contactViewSource")));

                if (rbtnCode.IsChecked == true)
                {
                    Controls.smartBoxContactSetting.Default.SearchFilter.Add("Code");
                }
                if (rbtnName.IsChecked == true)
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
            }
        }

        void LoginControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                Dispatcher.BeginInvoke(
                DispatcherPriority.ContextIdle,
                new Action(delegate()
                {
                    tbxSearch.Focus();
                }));
            }
        }  
        private void StartSearch(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ContactGrid_MouseDoubleClick(sender, e);
            }

            else if (e.Key == Key.Up)
            {
                if (contactViewSource!=null)
                {
                    if (contactViewSource.View!=null)
                    {
                        contactViewSource.View.MoveCurrentToPrevious();
                        contactViewSource.View.Refresh();
                    }
                    
                }
          
            }
            else if (e.Key == Key.Down)
            {
                if (contactViewSource != null)
                {
                    if (contactViewSource.View != null)
                    {
                        contactViewSource.View.MoveCurrentToNext();
                        contactViewSource.View.Refresh();
                    }

                }
              
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

            var param = smartBoxContactSetting.Default.SearchFilter;

            var predicate = PredicateBuilder.True<entity.contact>();

            predicate = (x => x.is_active && x.id_company == entity.CurrentSession.Id_Company);

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

            var predicateOR = PredicateBuilder.False<entity.contact>();
            
            if (param.Contains("Code"))
            {
                predicateOR = predicateOR.Or(x => x.code == SearchText);
            }

            if (param.Contains("Name"))
            {
                predicateOR = predicateOR.Or(x => x.name.Contains(SearchText));
            }

            if (param.Contains("GovID"))
            {
                predicateOR = predicateOR.Or(x => x.gov_code.Contains(SearchText));
            }

            if (param.Contains("Tel"))
            {
                predicateOR = predicateOR.Or(x => x.telephone.Contains(SearchText));
            }

            predicate = predicate.And
            (
                predicateOR
            );



            predicate = predicate.And(x => x.contact_role.can_transact);

            using (entity.db db = new entity.db())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.AutoDetectChangesEnabled = false;

                List<entity.contact> results = new List<entity.contact>();

                //Getting the data based on Predicates
                results.AddRange(db.contacts
                    .Where(predicate).OrderBy(x => x.name).ToList());

                Dispatcher.InvokeAsync(new Action(() =>
                {
                    contactViewSource.Source = results;
                    //Contact = contactViewSource.View.CurrentItem as entity.contact;

                    popContact.IsOpen = true;
                }));
            }
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
            if (Controls.smartBoxContactSetting.Default.SearchFilter != null)
            {
                Controls.smartBoxContactSetting.Default.SearchFilter.Clear();
            }

            if (rbtnCode.IsChecked == true)
            {
                Controls.smartBoxContactSetting.Default.SearchFilter.Add("Code");
            }
            if (rbtnName.IsChecked == true)
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

        private void crudContact_btnCancel_Click(object sender)
        {
            popCrud.IsOpen = false;
        }

        private void openContactCRUD(object sender, MouseButtonEventArgs e)
        {
            entity.Brillo.Security Sec = new entity.Brillo.Security(entity.App.Names.Contact);

            if (Sec.create)
            {
                cntrl.Curd.contact contactCURD = new Curd.contact();

                if (Get_Customers)
                {
                    contactCURD.IsCustomer = true;
                }
                else if (Get_Suppliers)
                {
                    contactCURD.IsSupplier = true;
                }
                else if (Get_Employees)
                {
                    contactCURD.IsEmployee = true;
                }

                popCrud.IsOpen = true;
                stackCRUD.Children.Add(contactCURD);
            }
        }       
    }
}
