using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using entity;
using System.Data.Entity.Validation;

namespace cntrl
{
    public partial class VATWithholding : UserControl
    {
        List<object> _invoiceList = null;
        public List<object> invoiceList { get { return _invoiceList; } set { _invoiceList = value; } }

        private dbContext _entity = null;
        public dbContext objEntity { get { return _entity; } set { _entity = value; } }

        public enum Mode
        {
            Add,
            Edit
        }

       

        public VATWithholding()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(entity.App.Names.SalesInvoice, CurrentSession.Id_Branch, CurrentSession.Id_terminal);

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                try
                {
                    CollectionViewSource invoiceViewSource = new CollectionViewSource();
                    invoiceViewSource.Source = _invoiceList;
                    stackMain.DataContext = invoiceViewSource;

                    //CollectionViewSource geo_countryViewSource = this.FindResource("geo_countryViewSource") as CollectionViewSource;
                    //geo_countryViewSource.Source = objEntity.db.geo_country.ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

           
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IEnumerable<DbEntityValidationResult> validationresult = objEntity.db.GetValidationErrors();
                if (validationresult.Count() == 0)
                {
                    objEntity.SaveChanges();
                    entity.Properties.Settings.Default.company_ID = objEntity.db.app_company.FirstOrDefault().id_company;
                    entity.Properties.Settings.Default.company_Name = objEntity.db.app_company.FirstOrDefault().alias;
                    entity.Properties.Settings.Default.Save();
                    btnCancel_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                objEntity.CancelChanges();
                
                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

      
    }
}
