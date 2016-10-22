using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using entity;
using System.Data.Entity.Validation;


namespace cntrl
{
    public partial class Item_Asset_Group : UserControl
    {
        CollectionViewSource item_asset_groupViewSource = null;
        //public CollectionViewSource objCollectionViewSource { get { return ItemsSource; } set { ItemsSource = value; } }

        private dbContext entity = new dbContext();
        //public entity.dbContext entity { get { return _entity; } set { _entity = value; } }

        private entity.item_asset_group _item_asset_groupobject = null;
        public entity.item_asset_group item_asset_groupobject { get { return _item_asset_groupobject; } set { _item_asset_groupobject = value; } }

        private Class.clsCommon.Mode _operationMode = 0;
        public Class.clsCommon.Mode operationMode { get { return _operationMode; } set { _operationMode = value; } }


       // entity.Properties.Settings _settings = new entity.Properties.Settings();

        public Item_Asset_Group()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
               // 

                item_asset_groupViewSource = (CollectionViewSource)FindResource("item_asset_groupViewSource");
                entity.db.item_asset_group.Where(x => x.id_company == CurrentSession.Id_Company).ToList();
                item_asset_groupViewSource.Source = entity.db.item_asset_group.Local;
          
             
               

            
                if (operationMode == Class.clsCommon.Mode.Add)
                {
                    item_asset_group item_asset_group = new item_asset_group();
                    item_asset_group.name = "item_asset_group";

                    entity.db.item_asset_group.Add(item_asset_group);
                  //  entity.db.SaveChanges();

                    item_asset_groupViewSource.View.Refresh();
                    item_asset_groupViewSource.View.MoveCurrentToLast();
                }
                else
                {
                    item_asset_groupViewSource.View.MoveCurrentTo(entity.db.item_asset_group.Where(x => x.id_item_asset_group == item_asset_groupobject.id_item_asset_group).FirstOrDefault());
                   // btnDelete.Visibility = System.Windows.Visibility.Visible;
                }
                stackMainAc.DataContext = item_asset_groupViewSource;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
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
            { throw ex; }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                entity.CancelChanges();
                item_asset_groupViewSource.View.Refresh();
                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception ex)
            { throw ex; }
        }

        //private void btnDelete_Click(object sender, RoutedEventArgs e)
        //{
        //    MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
        //    if (res == MessageBoxResult.Yes)
        //    {
        //        entity.item_asset_group objid_item_asset_group = item_asset_groupViewSource.View.CurrentItem as entity.item_asset_group;
        //        objid_item_asset_group.is_active = false;
        //        btnSave_Click(sender, e);
        //    }
        //}

        //Esc Key
        private void UserControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure want to close this window?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    btnCancel_Click(sender, e);
                }
            }
        }

     
    }
}
