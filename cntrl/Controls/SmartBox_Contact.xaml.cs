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
        public IQueryable<entity.BrilloQuery.Contact> ContactList { get; set; }

        public bool Get_Customers { get; set; }
        public bool Get_Suppliers { get; set; }
        public bool Get_Employees { get; set; }
        public bool Get_Users { get; set; }



        Task taskSearch;
        CancellationTokenSource tokenSource;
        CancellationToken token;

        CollectionViewSource contactViewSource;

        public SmartBox_Contact()
        {
            InitializeComponent();

            ///Exists code if in design view.
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            contactViewSource = ((CollectionViewSource)(FindResource("contactViewSource")));

            Task task = Task.Factory.StartNew(() => LoadData());

            this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(LoginControl_IsVisibleChanged);

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

        private void LoadData()
        {
            using (entity.BrilloQuery.GetContacts Execute = new entity.BrilloQuery.GetContacts())
            {
                contactViewSource.Source = Execute.List.AsQueryable();
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
            var predicate = PredicateBuilder.True<entity.BrilloQuery.Contact>();

            if (Get_Customers)
            {
                predicate = (x => x.IsCustomer == true);
            }
            else
            {
                predicate = (x => x.IsSupplier == true);
            }

            var predicateOR = PredicateBuilder.False<entity.BrilloQuery.Contact>();
            var param = smartBoxContactSetting.Default.SearchFilter;

            predicateOR = (x => x.Name.Contains(SearchText));

            if (param.Contains("Code"))
            {
                predicateOR = predicateOR.Or(x => x.Code == SearchText);
            }

            if (param.Contains("GovID"))
            {
                predicateOR = predicateOR.Or(x => x.Gov_Code.Contains(SearchText));
            }

            if (param.Contains("Tel"))
            {
                predicateOR = predicateOR.Or(x => x.Telephone.Contains(SearchText));
            }

            predicate = predicate.And
            (
                predicateOR
            );

            Dispatcher.InvokeAsync(new Action(() =>
            {
                if (popContact.IsOpen == false)
                {
                    popContact.IsOpen = true;
                }
                try
                {
                    contactViewSource.Source = ContactList.Where(predicate).ToList();
                }
                catch (Exception)
                {

                    throw;
                }

            }));
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
