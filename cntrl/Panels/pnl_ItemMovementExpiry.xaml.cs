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
    public partial class pnl_ItemMovementExpiry : UserControl
    {

        public ProductMovementDB ProductMovementDB = new ProductMovementDB();
        CollectionViewSource item_movementViewSource;
        public item_movement item_movement { get; set; }
        public int  id_item_product { get; set; }

        public pnl_ItemMovementExpiry()
        {
            InitializeComponent();
        }

     


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            item_movementViewSource = ((CollectionViewSource)(FindResource("item_movementViewSource")));
            ProductMovementDB.item_movement.Where(a =>a.id_item_product== id_item_product && a.id_company == CurrentSession.Id_Company && a.code!=null && a.expire_date!=null && a.avlquantity>0).Load();
            item_movementViewSource.Source = ProductMovementDB.item_movement.Local;
            
        }
       
        private void btnCancel_Click(object sender, MouseButtonEventArgs e)
        {
            item_inventory_detailDataGrid.CancelEdit();
            Grid parentGrid = (Grid)Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            item_movement = item_movementViewSource.View.CurrentItem as item_movement;
            Grid parentGrid = (Grid)Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }






    }
}
