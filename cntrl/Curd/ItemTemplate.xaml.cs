using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace cntrl.Curd
{
    /// <summary>
    /// Interaction logic for ProjectTag.xaml
    /// </summary>
    public partial class ItemTemplate : UserControl
    {
        // entity.Properties.Settings _setting = new entity.Properties.Settings();

        private CollectionViewSource _item_templateViewSource = null;
        public CollectionViewSource item_templateViewSource { get { return _item_templateViewSource; } set { _item_templateViewSource = value; } }

        private CollectionViewSource _item_templateDetailViewSource = null;
        public CollectionViewSource item_templateDetailViewSource { get { return _item_templateDetailViewSource; } set { _item_templateDetailViewSource = value; } }
        private entity.dbContext _entity = null;
        public entity.dbContext entity { get { return _entity; } set { _entity = value; } }

        public ItemTemplate()
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
                    stackMain.DataContext = item_templateViewSource;
                    stackSub.DataContext = item_templateDetailViewSource;
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
                item_templateViewSource.View.Refresh();
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
                entity.item_template item_template = item_templateViewSource.View.CurrentItem as entity.item_template;
                item_template.is_active = false;
                btnSave_Click(sender, e);
            }
        }
    }
}