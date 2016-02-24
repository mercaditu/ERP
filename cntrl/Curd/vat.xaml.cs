using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using entity;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace cntrl
{
    public partial class vat : UserControl
    {
        CollectionViewSource _app_vatViewSource = null;
        public CollectionViewSource app_vatViewSource { get { return _app_vatViewSource; } set { _app_vatViewSource = value; } }

        private entity.dbContext objentity = null;
        public entity.dbContext entity { get { return objentity; } set { objentity = value; } }

        public vat()
        {
            InitializeComponent();
        }

        private void tbxCoeficient_LostFocus(object sender, RoutedEventArgs e)
        {
            //string str = tbxCoeficient.Text;
            //if (str.Contains("%"))
            //{
            //    try
            //    {
            //        str.Replace("%", "");
            //        decimal coef = decimal.Parse(str) / 100;
            //        tbxCoeficient.Text = coef.ToString();
            //    }
            //    catch
            //    { tbxCoeficient.Background = Brushes.Pink; }
            //}
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                stackMain.DataContext = app_vatViewSource;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IEnumerable<DbEntityValidationResult> validationresult = entity.db.GetValidationErrors();
                if (validationresult.Count() == 0)
                {
                    entity.SaveChanges();
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
                entity.CancelChanges();
                app_vatViewSource.View.Refresh();
                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    app_vat app_vat = app_vatViewSource.View.CurrentItem as app_vat;
                    app_vat.is_active = false;
                    btnSave_Click(sender, e);
                }
            }
            catch (Exception)
            {
                //throw ex;
            }
        }
    }
}
