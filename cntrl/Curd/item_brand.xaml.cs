using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;

namespace cntrl.Curd
{
    public partial class item_brand : UserControl
    {
        entity.dbContext mydb = new entity.dbContext();
        CollectionViewSource myViewSource = new CollectionViewSource();
        //CollectionViewSource contactViewSource = null;
        public bool isExternalCall { get; set; }

        CollectionViewSource _MainViewSource = null;
        public CollectionViewSource MainViewSource { get { return _MainViewSource; } set { _MainViewSource = value; } }
        public object curObject { get; set; }
        public Class.clsCommon.Mode operationMode { get; set; }

        CollectionViewSource _item_brandViewSource;
        public CollectionViewSource item_brandViewSource { get { return _item_brandViewSource; } set { _item_brandViewSource = value; } }

        private entity.dbContext objentity = null;
        public entity.dbContext _entity { get { return objentity; } set { objentity = value; } }

        private entity.item_brand _item_brandobject = null;
        public entity.item_brand item_brandobject { get { return _item_brandobject; } set { _item_brandobject = value; } }

        entity.Properties.Settings _settings = new entity.Properties.Settings();

        public item_brand()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {

                entity.item_brand _item_brand = null;

                if (!isExternalCall)
                {

                  
                    stackMain.DataContext = item_brandViewSource;
                    _item_brand = item_brandViewSource.View.CurrentItem as entity.item_brand;
                 
                }
                else
                {

                 
                    MainViewSource.View.MoveCurrentTo(curObject);
                    if (operationMode == Class.clsCommon.Mode.Add)
                    {
                        entity.item_brand newBrand = new entity.item_brand();
                        mydb.db.item_brand.Add(newBrand);
                        myViewSource.Source = mydb.db.item_brand.Local;
                        myViewSource.View.Refresh();
                        myViewSource.View.MoveCurrentTo(newBrand);
                        stackMain.DataContext = myViewSource;
                        _item_brand = myViewSource.View.CurrentItem as entity.item_brand;
                    }
                    else if (operationMode == Class.clsCommon.Mode.Edit)
                    {
                        item_brandViewSource.View.MoveCurrentTo(item_brandobject);
                        stackMain.DataContext = item_brandViewSource;
                        _item_brand = item_brandViewSource.View.CurrentItem as entity.item_brand;
                    }
                  
                }
            }
        }

        private void btnCancel_MouseDown(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!isExternalCall)
                {
                    _entity.CancelChanges();
                    item_brandViewSource.View.Refresh();
                }
                else
                {
                    if (operationMode == Class.clsCommon.Mode.Add)
                        mydb.CancelChanges();
                }
                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!isExternalCall)
                {
                    IEnumerable<DbEntityValidationResult> validationresult = _entity.db.GetValidationErrors();
                    if (validationresult.Count() == 0)
                    {
                        _entity.db.SaveChanges();
                        btnCancel_MouseDown(sender, e);
                    }
                }
                else
                {
                    IEnumerable<DbEntityValidationResult> validationresult = mydb.db.GetValidationErrors();
                    if (validationresult.Count() == 0)
                    {
                        if (operationMode == Class.clsCommon.Mode.Add)
                        {
                            mydb.SaveChanges();
                            entity.item_brand item_brand = myViewSource.View.CurrentItem as entity.item_brand;
                            //mydb.db.Entry(item_brand).State = EntityState.Detached;
                            //_entity.db.item_brand.Attach(item_brand);
                            item_brandViewSource.View.Refresh();
                            item_brandViewSource.View.MoveCurrentTo(item_brand);
                            MainViewSource.View.Refresh();
                            MainViewSource.View.MoveCurrentTo(curObject);
                            btnCancel_MouseDown(sender, e);
                        }
                        else if (operationMode == Class.clsCommon.Mode.Edit)
                        {
                            btnCancel_MouseDown(sender, e);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void set_ContactPref(object sender, RoutedEventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                entity.contact contact = mydb.db.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                entity.item_brand item_brand = item_brandViewSource.View.CurrentItem as entity.item_brand;
                item_brand.id_contact = contact.id_contact;
              

          
            }
        }

     
    }
}
