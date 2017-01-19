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
using entity.BrilloQuery;

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
            string query = @"select l.id_location, b.id_branch, i.code, i.name, im.code, im.expire_date, im.credit, sum(child.debit), im.credit - sum(child.debit) as Balance
                                from item_movement as im
                                left join item_movement as child on im.id_movement = child.parent_id_movement
                                inner join item_product as ip on im.id_item_product = ip.id_item_product
                                inner join items as i on ip.id_item = i.id_item
                                inner join app_location as l on im.id_location = l.id_location
                                inner join app_branch as b on l.id_branch = b.id_branch
                                where im.credit - sum(child.debit) > 0 and ip.can_expire = true and l.id_branch = @BranchID and l.id_company = @CompanyID
                                group by im.id_movement
                                order by im.expire_date";
            query = query.Replace("@CompanyID", CurrentSession.Id_Company.ToString());
            query = query.Replace("@BranchID", CurrentSession.Id_Branch.ToString());
            item_movementViewSource.Source = QueryExecutor.DT(query);
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
