using System;
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public bool can_New
        {
            get { return _can_new; }
            set
            {
                _can_new = new entity.Brillo.Security(entity.App.Names.Items).create ? value : false;
                RaisePropertyChanged("can_New");
            }
        }

        private bool _can_new;

        public bool AutoShow
        {
            get { return _AutoShow; }
            set
            {
                _AutoShow = value;
            }
        }

        private bool _AutoShow;
  

        public bool can_Edit
        {
            get { return _can_edit; }
            set
            {
                _can_edit = new entity.Brillo.Security(entity.App.Names.Items).edit ? value : false;
                RaisePropertyChanged("can_Edit");
            }
        }

        private bool _can_edit;

        public event RoutedEventHandler Select;

        private void ContactGrid_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (contactViewSource.View != null)
            {
                if (contactViewSource.View.CurrentItem is entity.BrilloQuery.Contact Contact)
                {
                    ContactID = Contact.ID;
                    Text = Contact.Name;
                    if (AutoShow==false)
                    {
                        popContact.IsOpen = false;
                    }
                  

                    if (popContactInfo.IsOpen)
                    {
                        OpenContactCRUD(null, null);
                    }

                    Select?.Invoke(this, new RoutedEventArgs());
                }
            }
        }

        public int ContactID { get; set; }
        public IQueryable<entity.BrilloQuery.Contact> ContactList { get; set; }

        public bool Get_Customers { get; set; }
        public bool Get_Suppliers { get; set; }
        public bool Get_Employees { get; set; }
        public bool Get_Users { get; set; }
        public bool ExactSearch { get; set; }

        private Task taskSearch;
        private CancellationTokenSource tokenSource;
        private CancellationToken token;

        private CollectionViewSource contactViewSource;

        public SmartBox_Contact()
        {
            InitializeComponent();

            ///Exists code if in design view.
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            contactViewSource = FindResource("contactViewSource") as CollectionViewSource;

            LoadData();

            IsVisibleChanged += new DependencyPropertyChangedEventHandler(LoginControl_IsVisibleChanged);

            if (rbtnCode.IsChecked == true)
            {
                smartBoxContactSetting.Default.SearchFilter.Add("Code");
            }

            if (rbtnName.IsChecked == true)
            {
                smartBoxContactSetting.Default.SearchFilter.Add("Name");
            }

            if (rbtnGov_ID.IsChecked == true)
            {
                smartBoxContactSetting.Default.SearchFilter.Add("GovID");
            }
            if (rbtnTel.IsChecked == true)
            {
                smartBoxContactSetting.Default.SearchFilter.Add("Tel");
            }
         
        }

        public void LoadData()
        {
            progBar.Visibility = Visibility.Visible;
            Task task = Task.Factory.StartNew(() => LoadData_Thread());
        }

        private void LoadData_Thread()
        {
            ContactList = null;
            using (entity.BrilloQuery.GetContacts Execute = new entity.BrilloQuery.GetContacts())
            {
                ContactList = Execute.List.AsQueryable();
            }

            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate () { progBar.Visibility = Visibility.Collapsed; }));
        }

        private void LoginControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                Dispatcher.BeginInvoke(
                DispatcherPriority.ContextIdle,
                new Action(delegate ()
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
                if (contactViewSource != null)
                {
                    if (contactViewSource.View != null)
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
                if (rbtnExactCode.IsChecked == true)
                {
                    ExactSearch = true;
                }
                else
                {
                    ExactSearch = false;
                }
                if (SearchText.Count() >= 1)
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
            SearchText = SearchText.ToUpper();
            var predicate = PredicateBuilder.True<entity.BrilloQuery.Contact>();

            if (Get_Customers)
            {
                predicate = (x => x.IsCustomer == true && x.IsActive);
            }

            if (Get_Suppliers)
            {
                predicate = (x => x.IsSupplier == true && x.IsActive);
            }

            if (Get_Employees)
            {
                predicate = (x => x.IsEmployee == true && x.IsActive);
            }
            var predicateOR = PredicateBuilder.False<entity.BrilloQuery.Contact>();
            if (ExactSearch)
            {
                predicateOR = (x => x.Code.ToUpper().Equals(SearchText));
                predicate = predicate.And
              (
                  predicateOR
              );
            }
            else
            {
                var param = smartBoxContactSetting.Default.SearchFilter;

                predicateOR = (x => x.Name.ToUpper().Contains(SearchText));

                if (param.Contains("Code"))
                {
                    predicateOR = predicateOR.Or(x => x.Code.ToUpper().Contains(SearchText));
                }

                if (param.Contains("GovID"))
                {
                    predicateOR = predicateOR.Or(x => x.Gov_Code.ToUpper().Contains(SearchText));
                }

                if (param.Contains("Tel"))
                {
                    predicateOR = predicateOR.Or(x => x.Telephone.ToUpper().Contains(SearchText));
                }

                predicate = predicate.And
                (
                    predicateOR
                );
            }

            Dispatcher.InvokeAsync(new Action(() =>
            {
                if (popContact.IsOpen == false)
                {
                    popContact.IsOpen = true;
                }

                contactViewSource.Source = ContactList.Where(predicate).ToList();
            }));
        }

        private void Label_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            popupCustomize.IsOpen = true;
            popupCustomize.Visibility = Visibility.Visible;
        }

        private void PopupCustomize_Closed(object sender, EventArgs e)
        {
            smartBoxContactSetting.Default.Save();
            popupCustomize.IsOpen = false;
            popupCustomize.Visibility = Visibility.Collapsed;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (smartBoxContactSetting.Default.SearchFilter != null)
            {
                smartBoxContactSetting.Default.SearchFilter.Clear();
            }

            if (rbtnCode.IsChecked == true)
            {
                smartBoxContactSetting.Default.SearchFilter.Add("Code");
            }
            if (rbtnName.IsChecked == true)
            {
                smartBoxContactSetting.Default.SearchFilter.Add("Name");
            }
            if (rbtnGov_ID.IsChecked == true)
            {
                smartBoxContactSetting.Default.SearchFilter.Add("GovID");
            }
            if (rbtnTel.IsChecked == true)
            {
                smartBoxContactSetting.Default.SearchFilter.Add("Tel");
            }

            smartBoxContactSetting.Default.Save();
        }

        private void OpenContactCRUD(object sender, MouseButtonEventArgs e)
        {
            //Check if ContactID is Zero and user can create a new contact.
            if (ContactID == 0 && new entity.Brillo.Security(entity.App.Names.Contact).create)
            {
                popContactInfo.IsOpen = true;
            }
            else if (ContactID > 0 && new entity.Brillo.Security(entity.App.Names.Contact).edit)
            {
                popContactInfo.IsOpen = true;

                using (entity.db db = new entity.db())
                {
                    entity.contact contact = db.contacts.Where(x => x.id_contact == ContactID).FirstOrDefault();
                    if (contact != null)
                    {
                        tbxName.Text = contact.name;
                        tbxGovernmentID.Text = contact.gov_code;
                        tbxAddress.Text = contact.address;
                        tbxTelephone.Text = contact.telephone;
                        tbxEmail.Text = contact.email;
                        tbxContactRole.Content = contact.contact_role != null ? contact.contact_role.name : "";
                        tbxPriceList.Content = contact.item_price_list != null ? contact.item_price_list.name : "";
                    }
                }
            }
        }

        private void SaveContact_Click(object sender, RoutedEventArgs e)
        {
            string Name = string.Empty;
            
            if (ContactID == 0)
            {
                entity.contact contact = new entity.contact()
                {
                    name = tbxName.Text,
                    gov_code = tbxGovernmentID.Text,
                    address = tbxAddress.Text,
                    telephone = tbxTelephone.Text,
                    email = tbxEmail.Text
                };

                if (Get_Customers)
                { contact.is_customer = true; }
                else if (Get_Suppliers)
                { contact.is_supplier = true; }
                else if (Get_Employees)
                { contact.is_employee = true; }

                using (entity.db db = new entity.db())
                {
                    int RoleID = db.contact_role
                        .Where(x => x.is_principal && x.id_company == entity.CurrentSession.Id_Company)
                        .Select(x => x.id_contact_role)
                        .FirstOrDefault();

                    if (RoleID > 0)
                    {
                        contact.id_contact_role = RoleID;
                    }
                    else
                    {
                        entity.contact_role role = new entity.contact_role()
                        {
                            name = "Default Role",
                            is_principal = true,
                            can_transact = true
                        };

                        db.contact_role.Add(role);
                        db.SaveChanges();

                        contact.id_contact_role = role.id_contact_role;
                    }

                    int PriceListID = db.item_price_list
                    .Where(x => x.is_default && x.id_company == entity.CurrentSession.Id_Company)
                    .Select(x => x.id_price_list)
                    .FirstOrDefault();

                    if (PriceListID > 0)
                    {
                        contact.id_price_list = PriceListID;
                    }
                    else
                    {
                        entity.item_price_list list = new entity.item_price_list()
                        {
                            name = "Default Price List",
                            is_default = true
                        };

                        db.item_price_list.Add(list);
                        db.SaveChanges();

                        contact.id_price_list = list.id_price_list;
                    }
                    if (smartBoxContactSetting.Default.EmailNecessary==true)
                    {
                        if (string.IsNullOrEmpty(contact.email))
                        {
                            tbxEmail.Background.brush
                            return;
                        }
                    }
                    db.contacts.Add(contact);
                    db.SaveChanges();
                }
            }
            else
            {
                using (entity.db db = new entity.db())
                {
                    entity.contact contact = db.contacts.Where(x => x.id_contact == ContactID).FirstOrDefault();

                    if (contact != null)
                    {
                        contact.name = tbxName.Text;
                        contact.gov_code = tbxGovernmentID.Text;
                        contact.address = tbxAddress.Text;
                        contact.telephone = tbxTelephone.Text;
                        contact.email = tbxEmail.Text;
                        if (smartBoxContactSetting.Default.EmailNecessary == true)
                        {
                            if (string.IsNullOrEmpty(contact.email))
                            {
                                return;
                            }
                        }
                        db.SaveChanges();

                        Name = contact.name;
                    }
                }
            }

            LoadData();
            tbxSearch.Text = Name;

            popContactInfo.IsOpen = false;
        }

        private void Refresh_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            LoadData();
        }

        private void btnCancel_Click(object sender, MouseButtonEventArgs e)
        {
            
            popContactInfo.IsOpen = false;
        }
    }
}