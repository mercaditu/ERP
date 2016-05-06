using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using entity;
using System.Data.Entity;
using System.Collections.Generic;

namespace cntrl.Curd
{
    public partial class contact : UserControl
    {
        private entity.ContactDB ContactDB { get; set;}

        CollectionViewSource contactViewSource;

        #region Properties

        public bool IsCustomer 
        {
            get { return _IsCustomer; }
            set
            {
                if (_IsCustomer != value)
                {
                    cbPriceList.ItemsSource = ContactDB.item_price_list.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).AsNoTracking().ToList();
                    chbxCustomer.IsChecked = true;
                }
            }
        }
        private bool _IsCustomer;

        public bool IsSupplier
        {
            get { return _IsSupplier; }
            set
            {
                if (_IsSupplier != value)
                {
                    cbCostCenter.ItemsSource = ContactDB.app_cost_center.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).AsNoTracking().ToList();
                    chbxSupplier.IsChecked = true;
                }
            }
        }
        private bool _IsSupplier;

        public bool IsEmployee
        {
            get { return _IsEmployee; }
            set
            {
                if (_IsEmployee != value)
                {
                    chbxEmployee.IsChecked = true;
                }
            }
        }
        private bool _IsEmployee;

        public int ContactID { get; set; }
        public string ContactName { get; set; }
        #endregion

        #region Events

        public event btnCancel_ClickedEventHandler btnCancel_Click;
        public delegate void btnCancel_ClickedEventHandler(object sender);
        private void btnCancel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (btnCancel_Click != null)
            {
                btnCancel_Click(sender);
            }
        }

        #endregion

        public contact()
        {
            InitializeComponent();

            ContactDB = new ContactDB();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                if (ContactDB != null)
                {
                    ///Get Role List.
                    cbxRole.ItemsSource = ContactDB.contact_role.Where(a => a.id_company == CurrentSession.Id_Company && a.is_active == true).OrderBy(a => a.name).AsNoTracking().ToList();

                    contactViewSource = (CollectionViewSource)this.FindResource("contactViewSource");

                    entity.contact contact = null;

                    ///Check for ContactID to check if this form is in EDIT mode or NEW mode.
                    if (ContactID > 0)
                    {
                        ///If Contact IsNot Null, then this form is in EDIT MODE. Must add Contact into Context.
                        contact = ContactDB.contacts.Where(x => x.id_contact == ContactID).FirstOrDefault();
                        ContactDB.contacts.Add(contact);
                    }
                    else
                    {
                        ///If ContactID is Null, then this form is in NEW MODE. Must create Contact and add into Context.
                        contact = ContactDB.New();
                        ContactDB.contacts.Add(contact);
                    }

                    ///Bring only InMemoria Data.
                    contactViewSource.Source = ContactDB.contacts.Local;
                    contactViewSource.View.MoveCurrentTo(contact);
                }
            }
        }

        private void btnSave_MouseUp(object sender, RoutedEventArgs e)
        {
            entity.contact contact = contactViewSource.View as entity.contact;
            
            //This is helpful when we want to Automate the search of contact when saving is done.
            if (contact != null)
            {
                ContactName = contact.name;
            }

            if (ContactDB.SaveChanges() == 0)
            {
                MessageBox.Show("Saving Error");
            }


        }
    }
}
