using entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cognitivo.Configs
{
    /// <summary>
    /// Interaction logic for itemMovementFraction.xaml
    /// </summary>
    public partial class itemMovementFraction : UserControl
    {
        CollectionViewSource item_movementViewSource;
        public ExecutionDB ExecutionDB { get; set; }
        public int id_item { get; set; }
        public long id_movement { get; set; }
        public item_movement item_movement { get; set; }
       // public production_execution _production_execution { get; set; }
        public production_order_detail production_order_detail { get; set; }
        public decimal Quantity { get; set; }
        public enum Types
        {
            Product,
            RawMaterial,
            Asset,
            Supplier,
            ServiceContract
        }

        public enum modes
        {
            Production,
            Execution
        }

        public modes mode { get; set; }
        public Types type { get; set; }
        CollectionViewSource app_measurementViewSource;
        public itemMovementFraction()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource app_dimensionViewSource = ((CollectionViewSource)(FindResource("app_dimensionViewSource")));
             app_measurementViewSource = ((CollectionViewSource)(FindResource("app_measurementViewSource")));

            item_movementViewSource = ((CollectionViewSource)(FindResource("item_movementViewSource")));
            List<item_movement> Items_InStockLIST = null;

            app_dimensionViewSource.Source = ExecutionDB.app_dimension.Where(a => a.id_company == CurrentSession.Id_Company).ToList();
            app_measurementViewSource.Source = ExecutionDB.app_measurement.Where(a => a.id_company == CurrentSession.Id_Company).ToList();

            Items_InStockLIST = ExecutionDB.item_movement.Where(x => x.id_location == production_order_detail.production_order.production_line.id_location &&
                                                                     x.item_product.id_item == id_item && 
                                                                     x.status == entity.Status.Stock.InStock && 
                                                                    (x.credit - (x.child.Count() > 0 ? x.child.Sum(y => y.debit) : 0)) > 0).ToList();

            if (Items_InStockLIST.Count() > 0)
            {
                item_movementViewSource.Source = Items_InStockLIST;
                item_movementViewSource.View.MoveCurrentToFirst();

                id_movement = (item_movementViewSource.View.CurrentItem as item_movement).id_movement;
                item_movement = item_movementViewSource.View.CurrentItem as item_movement;
            }

        }

        private void item_movement_detailDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (item_movementViewSource != null)
            {
                if (item_movementViewSource.View != null)
                {
                    if (item_movementViewSource.View.CurrentItem as item_movement != null)
                    {
                        id_movement = (item_movementViewSource.View.CurrentItem as item_movement).id_movement;
                        item_movement = item_movementViewSource.View.CurrentItem as item_movement;
                    }
                }

            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Insert_IntoDetail(production_order_detail, Quantity);
        }

        private void Insert_IntoDetail(production_order_detail production_order_detail, decimal Quantity)
        {
            //if (_production_execution != null)
            //{
                if (production_order_detail != null)
                {
                    production_execution_detail _production_execution_detail = new entity.production_execution_detail();

                    //Adds Parent so that during approval, because it is needed for approval.
                    if (production_order_detail.parent != null)
                    {
                        if (production_order_detail.parent.production_execution_detail != null)
                        {
                            _production_execution_detail.parent = production_order_detail.parent.production_execution_detail.FirstOrDefault();
                        }
                    }

                    _production_execution_detail.State = EntityState.Added;
                    _production_execution_detail.id_item = production_order_detail.id_item;
                    _production_execution_detail.item = production_order_detail.item;
                    _production_execution_detail.quantity = Quantity;
                    _production_execution_detail.id_project_task = production_order_detail.id_project_task;
                    _production_execution_detail.movement_id = (int)id_movement;

                    if (production_order_detail.item.unit_cost != null)
                    {
                        _production_execution_detail.unit_cost = (decimal)production_order_detail.item.unit_cost;
                    }

                  //  _production_execution_detail.production_execution = _production_execution;
                    _production_execution_detail.id_order_detail = production_order_detail.id_order_detail;

                    if (production_order_detail.item.is_autorecepie)
                    {
                        _production_execution_detail.is_input = false;
                    }
                    else
                    {
                        _production_execution_detail.is_input = true;
                    }
                    foreach (item_movement_dimension item_movement_dimension in item_movement.item_movement_dimension)
                    {
                        production_execution_dimension production_execution_dimension = new production_execution_dimension();
                        production_execution_dimension.id_dimension = item_movement_dimension.id_dimension;
                        production_execution_dimension.value = item_movement_dimension.value;
                        production_execution_dimension.id_measurement =(app_measurementViewSource.View.CurrentItem as app_measurement).id_measurement;
                        _production_execution_detail.production_execution_dimension.Add(production_execution_dimension);
                    }
                    production_order_detail.production_execution_detail.Add(_production_execution_detail);
                }
           // }


            btnCancel_Click(null, null);
        }

        private void btnCancel_Click(object sender, MouseButtonEventArgs e)
        {
            id_movement = 0;
            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
