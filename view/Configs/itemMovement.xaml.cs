using entity;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using entity.Brillo;
using System;

namespace Cognitivo.Configs
{
    public partial class itemMovement : UserControl
    {
        // private CollectionViewSource item_movementViewSource;
        public db db { get; set; }

        public int id_location { get; set; }
        public int id_item { get; set; }
        public item_movement item_movement { get; set; }
        Stock stock = new Stock();
        private CollectionViewSource app_measurementViewSource;

        public itemMovement()
        {
            InitializeComponent();
        }

        public item_request_decision.Decisions Decision { get; set; }
        public decimal? Quantity { get; set; }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource app_dimensionViewSource = ((CollectionViewSource)(FindResource("app_dimensionViewSource")));
            app_measurementViewSource = ((CollectionViewSource)(FindResource("app_measurementViewSource")));

            app_dimensionViewSource.Source = db.app_dimension.Where(a => a.id_company == CurrentSession.Id_Company).ToList();
            app_measurementViewSource.Source = db.app_measurement.Where(a => a.id_company == CurrentSession.Id_Company).ToList();

            app_location app_location = db.app_location.Find(id_location);

            if (id_item > 0 && app_location != null)
            {
                item_movement_detailDataGrid.ItemsSource = stock.getProducts_InStock(app_location.id_branch, DateTime.Now, true).Where(x => x.LocationID == id_location && x.ItemID == id_item).ToList();
            }
        }

        private void item_movement_detailDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StockList StockList = item_movement_detailDataGrid.SelectedItem as StockList;

            if (StockList != null)
            {
                item_movement = db.item_movement.Where(x => x.id_movement == StockList.MovementID).FirstOrDefault();
            }
        }

        public event RoutedEventHandler Save;

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Save?.Invoke(this, new RoutedEventArgs());

            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Visibility = Visibility.Hidden;
        }

        private void btnCancel_Click(object sender, MouseButtonEventArgs e)
        {
            item_movement = null;
            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Visibility = Visibility.Hidden;
        }

        private void item_movement_detailDataGrid_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            StockList StockList = item_movement_detailDataGrid.SelectedItem as StockList;
            DataGrid item_inventory_dimentionDataGrid = e.DetailsElement.FindName("item_inventory_dimentionDataGrid") as DataGrid;
            if (StockList != null)
            {
                if (item_inventory_dimentionDataGrid != null)
                {
                    item_inventory_dimentionDataGrid.ItemsSource = db.item_movement_dimension.Where(x => x.id_movement == StockList.MovementID).ToList();
                }
            }
        }
    }
}