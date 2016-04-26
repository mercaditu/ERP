using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using System.Data;
using entity;
using System.Data.Entity.Validation;
using System.Windows.Input;

namespace cntrl.Panels
{
    /// <summary>
    /// Interaction logic for pnl_ItemMovement.xaml
    /// </summary>
    public partial class pnl_ItemMovement : UserControl
    {
        CollectionViewSource item_movementViewSource;
        ProductMovementDB ProductMovementDB = new ProductMovementDB();
        public int id_item_product { get; set; }
        public int id_location { get; set; }
        public decimal quantity { get; set; }
        public pnl_ItemMovement()
        {
            InitializeComponent();
        }

        private void item_transfer_detailDataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            item_movement item_movement = e.NewItem as item_movement;
            item_movement.id_item_product = id_item_product;
            item_movement.id_application = App.Names.Inventory;
            item_movement.id_location = id_location;
            item_movement.IsSelected = true;
            item_movement.State = EntityState.Added;
            //item_movement_dest.transaction_id = 0;
            item_movement.status = Status.Stock.InStock;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            item_movementViewSource = ((CollectionViewSource)(FindResource("item_movementViewSource")));
            ProductMovementDB.item_movement.Where(a => a.id_company == CurrentSession.Id_Company && a.id_item_product == id_item_product && a.id_location == id_location).Load();
            item_movementViewSource.Source = ProductMovementDB.item_movement.Local;

            CollectionViewSource app_dimensionViewSource = ((CollectionViewSource)(FindResource("app_dimensionViewSource")));
            ProductMovementDB.app_dimension.Where(a => a.id_company == CurrentSession.Id_Company).Load();
            app_dimensionViewSource.Source = ProductMovementDB.app_dimension.Local;


            //CollectionViewSource app_currencyfxViewSource = ((CollectionViewSource)(FindResource("app_currencyfxViewSource")));
            //ProductMovementDB.app_currencyfx.Where(a => a.id_company == CurrentSession.Id_Company).Load();
            //app_currencyfxViewSource.Source = ProductMovementDB.app_currencyfx.Local;



        }

        private void btnCancel_Click(object sender, MouseButtonEventArgs e)
        {
            item_transfer_detailDataGrid.CancelEdit();
            Grid parentGrid = (Grid)Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            List<item_movement> item_movementList = item_movementViewSource.View.OfType<item_movement>().ToList();
            quantity = item_movementList.Sum(y => y.credit - y.debit); ;
            ProductMovementDB.SaveChanges();
            btnCancel_Click(sender, null);
        }

        private void item_movement_valueDataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            item_movement_value item_movement_value = e.NewItem as item_movement_value;
            if (ProductMovementDB.app_currencyfx.Where(x => x.app_currency.is_priority && x.is_active).FirstOrDefault() != null)
            {
                item_movement_value.id_currencyfx = ProductMovementDB.app_currencyfx.Where(x => x.app_currency.is_priority && x.is_active).FirstOrDefault().id_currencyfx;
            }

        }
    }
}
