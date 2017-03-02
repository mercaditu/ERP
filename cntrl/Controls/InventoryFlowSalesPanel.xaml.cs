using entity;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace cntrl.Controls
{
    public partial class InventoryFlowSalesPanel : UserControl
    {
        public long? ParentID { get; set; }
        public int ProductID { get; set; }
        public long? MovementID { get; set; }
        CollectionViewSource item_movementViewSource;

        public InventoryFlowSalesPanel()
        {
            InitializeComponent();           
        }

        public event SelectionChangedEventHandler SelectionChanged;
        //public event SelectionChangedEventHandler Selection_WithOut_ChildChanged;
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ItemMovement ItemMovement = item_movementViewSource.View.CurrentItem as ItemMovement;
            if (ItemMovement != null)
            {
                MovementID = ItemMovement.MovementID;
                SelectionChanged(sender, e);
            }
        }

        public void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            LoadData();
           
        }

        public void LoadData()
        {
            using (db db = new db())
            {
                var MovementList = (from item in db.item_movement
                                    join loc in db.app_location on item.id_location equals loc.id_location
                                    join b in db.app_branch on loc.id_branch equals b.id_branch
                                    join ip in db.item_product on item.id_item_product equals ip.id_item_product
                                    join i in db.items on ip.id_item equals i.id_item
                                    where item.parent.id_movement == ParentID && item.id_item_product == ProductID
                                    select new ItemMovement
                                    {
                                        MovementID = item.id_movement,
                                        Branch = b.name,
                                        Location = loc.name,
                                        ProductCode = i.code,
                                        ProductName = i.name,
                                        Date = item.trans_date,
                                        ExpiryDate = item.expire_date,
                                        BatchCode = item.code,
                                        Quantity = item.credit - item.debit,
                                        Cost = item.item_movement_value.Sum(x => x.unit_value),
                                        Comment = item.comment
                                    }).OrderByDescending(x => x.Date).ToList();

                item_movementViewSource = ((CollectionViewSource)(FindResource("item_movementViewSource")));
                item_movementViewSource.Source = MovementList;
                item_movementViewSource.View.Refresh();
            }
        }
    }
}
