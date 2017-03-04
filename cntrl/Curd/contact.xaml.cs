using entity;
using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace cntrl.Curd
{
    public partial class contact : UserControl, INotifyPropertyChanged
    {
        private entity.ContactDB ContactDB { get; set; }
        private CollectionViewSource contactViewSource;

        #region NotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        #endregion NotifyPropertyChanged

        private entity.contact _contact = null;

        #region Properties

        public bool IsCustomer { get; set; }
        public bool IsSupplier { get; set; }
        public bool IsEmployee { get; set; }

        public int ContactID { get; set; }
        public string ContactName { get; set; }

        #endregion Properties

        #region Events

        private void btnCancel_MouseDown(object sender, EventArgs e)
        {
            if (this.Parent as StackPanel != null)
            {
                StackPanel parentGrid = this.Parent as StackPanel;
                parentGrid.Children.RemoveAt(0);
            }
            else if (this.Parent as Grid != null)
            {
                Grid parentGrid = this.Parent as Grid;
                parentGrid.Children.RemoveAt(0);
            }
            else if (this.Parent as System.Windows.Controls.Primitives.Popup != null)
            {
                System.Windows.Controls.Primitives.Popup parentPopUp = this.Parent as System.Windows.Controls.Primitives.Popup;
                parentPopUp.IsOpen = false;
            }
        }

        public event btnLoad_ClickedEventHandler LoadContact_Click;

        public delegate void btnLoad_ClickedEventHandler();

        public void btnLoad_MouseUp(object sender, EventArgs e)
        {
            if (LoadContact_Click != null)
            {
                LoadContact_Click();
            }
        }

        public event btnSave_ClickedEventHandler btnSave_Click;

        public delegate void btnSave_ClickedEventHandler(object sender);

        private void btnSave_MouseUp(object sender, RoutedEventArgs e)
        {
            entity.contact contact = contactViewSource.View.CurrentItem as entity.contact;

            if (ContactDB.SaveChanges() == 0)
            {
                MessageBox.Show("Saving Error");
            }
            //This is helpful when we want to Automate the search of contact when saving is done.
            if (contact != null)
            {
                ContactName = contact.name;

                btnSave_Click?.Invoke(sender);
                //Reloads all Data.
                CurrentSession.Load_BasicData(null, null);
            }

            btnCancel_MouseDown(null, null);
        }

        #endregion Events

        public contact()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                ContactDB = new ContactDB();

                if (ContactDB != null)
                {
                    ///Get Role List.
                    cbxRole.ItemsSource = ContactDB.contact_role.Where(a => a.id_company == CurrentSession.Id_Company && a.is_active == true).OrderBy(a => a.name).AsNoTracking().ToList();
                    contactViewSource = (CollectionViewSource)this.FindResource("contactViewSource");
                    ContactDB.contact_tag
            .Where(x => x.id_company == CurrentSession.Id_Company && x.is_active == true)
            .OrderBy(x => x.name).Load();
                    CollectionViewSource contact_tagViewSource = ((CollectionViewSource)(FindResource("contact_tagViewSource")));
                    contact_tagViewSource.Source = ContactDB.contact_tag.Local;
                    ///Check for ContactID to check if this form is in EDIT mode or NEW mode.
                    if (ContactID > 0)
                    {
                        ///If Contact IsNot Null, then this form is in EDIT MODE. Must add Contact into Context.
                        _contact = ContactDB.contacts.Where(x => x.id_contact == ContactID).FirstOrDefault();
                        ContactDB.contacts.Add(_contact);
                    }
                    else
                    {
                        ///If ContactID is Null, then this form is in NEW MODE. Must create Contact and add into Context.
                        if (ContactDB.contacts.Local.Where(x => x.id_contact == 0).Count() == 0)
                        {
                            _contact = ContactDB.New();
                            ContactDB.contacts.Add(_contact);
                        }
                    }

                    if (IsCustomer)
                    {
                        cbPriceList.ItemsSource = ContactDB.item_price_list.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).AsNoTracking().ToList();
                        _contact.is_customer = true;
                        _contact.is_supplier = false;
                        _contact.is_employee = false;
                    }

                    if (IsSupplier)
                    {
                        cbCostCenter.ItemsSource = ContactDB.app_cost_center.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).AsNoTracking().ToList();
                        _contact.is_supplier = true;
                        _contact.is_employee = false;
                        _contact.is_customer = false;
                    }

                    if (IsEmployee)
                    {
                        _contact.is_employee = true;
                        _contact.is_supplier = false;
                        _contact.is_customer = false;
                    }

                    ///Bring only InMemoria Data.
                    contactViewSource.Source = ContactDB.contacts.Local;
                    contactViewSource.View.MoveCurrentTo(_contact);
                }
            }
        }

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as contact_tag_detail != null)
            {
                e.CanExecute = true;
            }
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (e.Parameter as contact_tag_detail != null)
                    {
                        contact_tag_detailDataGrid.CancelEdit();
                        ContactDB.contact_tag_detail.Remove(e.Parameter as contact_tag_detail);

                        CollectionViewSource contactcontact_tag_detailViewSource = FindResource("contactcontact_tag_detailViewSource") as CollectionViewSource;
                        contactcontact_tag_detailViewSource.View.Refresh();
                    }
                }
            }
            catch (Exception)
            {
                //throw;
            }
        }

        private void cbxTag_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Add_Tag();
            }
        }

        private void cbxTag_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Add_Tag();
        }

        private void Add_Tag()
        {
            // CollectionViewSource item_tagViewSource = ((CollectionViewSource)(FindResource("item_tagViewSource")));
            if (cbxTag.Data != null)
            {
                int id = Convert.ToInt32(((contact_tag)cbxTag.Data).id_tag);
                if (id > 0)
                {
                    entity.contact contact = contactViewSource.View.CurrentItem as entity.contact;
                    if (contact != null)
                    {
                        contact_tag_detail contact_tag_detail = new contact_tag_detail();
                        contact_tag_detail.id_tag = ((contact_tag)cbxTag.Data).id_tag;
                        contact_tag_detail.contact_tag = ((contact_tag)cbxTag.Data);
                        contact.contact_tag_detail.Add(contact_tag_detail);
                        CollectionViewSource contactcontact_tag_detailViewSource = FindResource("contactcontact_tag_detailViewSource") as CollectionViewSource;
                        contactcontact_tag_detailViewSource.View.Refresh();
                    }
                }
            }
        }
    }
}