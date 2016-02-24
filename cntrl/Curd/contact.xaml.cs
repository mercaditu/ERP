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
        entity.dbContext entity = new entity.dbContext();
        entity.Properties.Settings _Settings = new entity.Properties.Settings();

        //CollectionViewSource _contactViewSource = null;
        //public CollectionViewSource contactViewSource { get { return _contactViewSource; } set { _contactViewSource = value; } }

        //CollectionViewSource _MainViewSource = null;
        //public CollectionViewSource MainViewSource { get { return _MainViewSource; } set { _MainViewSource = value; } }
        //public object curObject { get; set; }

        //private dbContext entity = null;
        //public dbContext _entity { get { return entity; } set { entity = value; } }

        //private app.Applications _application = 0;
        //public app.Applications application { get { return _application; } set { _application = value; } }

        //private Class.clsCommon.Mode _operationMode = 0;
        //public Class.clsCommon.Mode operationMode { get { return _operationMode; } set { _operationMode = value; } }


        private entity.contact _contactobject = null;
        public entity.contact contactobject { get { return _contactobject; } set { _contactobject = value; } }

        public contact()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                cbPriceList.ItemsSource = entity.db.item_price_list.Where(a => a.is_active == true && a.id_company == _Settings.company_ID).OrderBy(a => a.name).AsNoTracking().ToList();
                cbCostCenter.ItemsSource = entity.db.app_cost_center.Where(a => a.is_active == true && a.id_company == _Settings.company_ID).OrderBy(a => a.name).AsNoTracking().ToList();
                cbxRole.ItemsSource = entity.db.contact_role.Where(a => a.id_company == _Settings.company_ID && a.is_active == true).OrderBy(a => a.name).AsNoTracking().ToList();
                ///////cbxRelation.ItemsSource = entity.db.contacts.Local.ToList();



                CollectionViewSource contactViewSource = (CollectionViewSource)this.FindResource("contactViewSource");
                List<entity.contact> contactList = new List<entity.contact>();
                contactList.Add(contactobject);
                contactViewSource.Source = contactList;
                //if (operationMode == Class.clsCommon.Mode.Add)
                //{
                //    entity.contact newContact = new entity.contact();
                //    if (_application == app.Applications.SalesInvoice)
                //        newContact.is_customer = true;
                //    if (_application == app.Applications.PurchaseInvoice)
                //        newContact.is_supplier = true;
                //    _entity.db.contacts.Add(newContact);
                //    contactViewSource.View.MoveCurrentTo(newContact);//Last();
                //}
                //else
                //{
                //    contactViewSource.View.MoveCurrentTo(contactobject);
                //}
                //stackContact.DataContext = contactViewSource;
            }
        }

        public event btnSave_ClickedEventHandler btnSave_Click;
        public delegate void btnSave_ClickedEventHandler(object sender);
        public void btnSave_MouseUp(object sender, EventArgs e)
        {


            if (btnSave_Click != null)
            {
                btnSave_Click(sender);
            }
        }
        //private void btnSave_Click(object sender, RoutedEventArgs e)
        //{
        //    if (contactViewSource.View.CurrentItem != null)
        //    {
        //        entity.contact mycontact = contactViewSource.View.CurrentItem as entity.contact;
        //        if (string.IsNullOrEmpty(mycontact.Error))
        //        {
        //            isValid = true;
        //            btnCancel_Click(sender, e);
        //        }
        //    }
        //}

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //if (!isValid && operationMode == Class.clsCommon.Mode.Add)
            //{
            //    if (contactViewSource.View.CurrentItem != null)
            //        _entity.db.contacts.Remove(contactViewSource.View.CurrentItem as entity.contact);
            //}
            //else if (!isValid && operationMode == Class.clsCommon.Mode.Edit)
            //{
            //    if (_entity.db.Entry(contactobject).State == EntityState.Modified)
            //    {
            //        _entity.db.Entry(contactobject).State = EntityState.Unchanged;
            //    }
            //}
            //contactViewSource.View.Refresh();
            //if (contactobject != null)
            //{
            //    STbox.Text = contactobject.name;
            //    STbox.Data = contactobject;
            //}
            //MainViewSource.View.Refresh();
            //MainViewSource.View.MoveCurrentTo(curObject);
            Grid crud = this.Parent as Grid;
            crud.Children.Clear();
            crud.Visibility = Visibility.Hidden;
        }

        

        //private void cbxRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (cbxRole.SelectedItem != null)
        //    {
        //        entity.contact_role contact_role = cbxRole.SelectedItem as entity.contact_role;
        //        if (contact_role.is_principal == true)
        //            cbxRelation.IsEnabled = false;
        //        else
        //            cbxRelation.IsEnabled = true;
        //    }
        //}


    }
}
