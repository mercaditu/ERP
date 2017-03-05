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
            LoadContact_Click?.Invoke();
        }

        public event btnSave_ClickedEventHandler btnSave_Click;

        public delegate void btnSave_ClickedEventHandler(object sender);

        private void btnSave_MouseUp(object sender, RoutedEventArgs e)
        {
            entity.contact contact = contactViewSource.View.CurrentItem as entity.contact;
            if (contact != null && contact.Error == null)
            {
                contact.IsSelected = true;
                contact.id_contact_role = ContactDB.contact_role.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active && x.is_principal).Select(x => x.id_contact_role).FirstOrDefault();
                contact.id_price_list = ContactDB.item_price_list.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active && x.is_default).Select(x => x.id_price_list).FirstOrDefault();

                if (ContactDB.SaveChanges() == 0)
                {
                    MessageBox.Show("Saving Error");
                }

                //This is helpful when we want to Automate the search of contact when saving is done.
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

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                ContactDB = new ContactDB();

                if (ContactDB != null)
                {
                    ///Get Role List.
                    //cbxRole.ItemsSource = ContactDB.contact_role.Where(a => a.id_company == CurrentSession.Id_Company && a.is_active == true).OrderBy(a => a.name).AsNoTracking().ToList();
                    contactViewSource = this.FindResource("contactViewSource") as CollectionViewSource;
                    await ContactDB.contact_tag
                        .Where(x => 
                        x.id_company == CurrentSession.Id_Company && 
                        x.is_active == true)
                        .OrderBy(x => x.name).LoadAsync();
                    CollectionViewSource contact_tagViewSource = FindResource("contact_tagViewSource") as CollectionViewSource;
                    contact_tagViewSource.Source = ContactDB.contact_tag.Local;
                    
                    ///Check for ContactID to check if this form is in EDIT mode or NEW mode.
                    if (ContactID > 0)
                    {
                        ///If Contact IsNot Null, then this form is in EDIT MODE. Must add Contact into Context.
                        _contact = ContactDB.contacts.Find(ContactID);
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