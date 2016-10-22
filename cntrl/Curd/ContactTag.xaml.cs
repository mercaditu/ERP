using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using entity;
using System.Data.Entity.Validation;

namespace cntrl.Curd
{
    /// <summary>
    /// Interaction logic for ItemTag.xaml
    /// </summary>
    public partial class ContactTag : UserControl
    {
        //entity.Properties.Settings _setting = new entity.Properties.Settings();

        CollectionViewSource _contact_tagViewSource = null;
        public CollectionViewSource contact_tagViewSource { get { return _contact_tagViewSource; } set { _contact_tagViewSource = value; } }

        private entity.dbContext _entity = null;
        public entity.dbContext entity { get { return _entity; } set { _entity = value; } }
        
        public ContactTag()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                //Load your data here and assign the result to the CollectionViewSource.
                try
                {
                    stackMain.DataContext = contact_tagViewSource;
                    CollectionViewSource contactsViewSource = (System.Windows.Data.CollectionViewSource)this.FindResource("contactsViewSource");
                    contactsViewSource.Source = entity.db.contacts.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).AsNoTracking().ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                IEnumerable<DbEntityValidationResult> validationresult = entity.db.GetValidationErrors();
                if (validationresult.Count() == 0)
                {
                    entity.db.SaveChanges();
                    btnCancel_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                entity.CancelChanges();
                contact_tagViewSource.View.Refresh();
                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception)
            {
                //throw ex;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                entity.contact_tag contact_tag = contact_tagViewSource.View.CurrentItem as entity.contact_tag;
                contact_tag.is_active = false;
                btnSave_Click(sender, e);
            }
        }
    }
}

