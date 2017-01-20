using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using entity;
using System.Windows.Input;
using entity.BrilloQuery;
using System.Data;
using System;
using System.Collections.Generic;

namespace cntrl.Panels
{
    public partial class pnl_ItemMovementExpiry : UserControl
    {

        public ProductMovementDB ProductMovementDB = new ProductMovementDB();
        CollectionViewSource ExpiryInStockViewSource;
        public item_movement item_movement { get; set; }

        public pnl_ItemMovementExpiry(int? BranchID, int? LocationID, int ProductID)
        {
            InitializeComponent();
            UserControl_Loaded(BranchID, LocationID, ProductID);
        }

        private void UserControl_Loaded(int? BranchID, int? LocationID, int ProductID)
        {
            ExpiryInStockViewSource = ((CollectionViewSource)(FindResource("ExpiryInStockViewSource")));

            //We are not certain if we should search by Location or Branch. This helps in choosing only Branch if is selected.
            string LocationWhere = "";
            if (BranchID != null)
            {
                LocationWhere = "and l.id_branch = " + BranchID;
            }
            else
            {
                LocationWhere = "and l.id_location = " + LocationID;
            }

            string query = @"select * from(select im.id_movement as MovementID, l.name as Location, b.name as Branch, i.code as Code, i.name as Items, 
                                im.code as BatchCode, im.expire_date as ExpiryDate, 
                                (im.credit - sum(IFNULL(child.debit,0))) as Balance
                                from item_movement as im
                                left join item_movement as child on im.id_movement = child.parent_id_movement
                                inner join item_product as ip on im.id_item_product = ip.id_item_product
                                inner join items as i on ip.id_item = i.id_item
                                inner join app_location as l on im.id_location = l.id_location
                                inner join app_branch as b on l.id_branch = b.id_branch
                                where ip.can_expire = true 
                                      @LocationWhere and l.id_company = @CompanyID
                                      and im.id_item_product = @ProductID
                                group by im.id_movement
                                order by im.expire_date) as movement where Balance>0";

            query = query.Replace("@LocationWhere", LocationWhere);
            query = query.Replace("@CompanyID", CurrentSession.Id_Company.ToString());
            query = query.Replace("@ProductID", ProductID.ToString());
            ExpiryInStockViewSource.Source = BatchCodeLoader(QueryExecutor.DT(query));
        }
       
        private void btnCancel_Click(object sender, MouseButtonEventArgs e)
        {
            item_inventory_detailDataGrid.CancelEdit();
            Grid parentGrid = (Grid)Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ExpiryInStock ExpiryInStock = ExpiryInStockViewSource.View.CurrentItem as ExpiryInStock;

            using (db db = new db())
            {
                item_movement = await db.item_movement.FindAsync(ExpiryInStock.MovementID);
            }

            Grid parentGrid = (Grid)Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }

        private List<ExpiryInStock> BatchCodeLoader(DataTable dt)
        {
            List<ExpiryInStock> ListOfStock = new List<ExpiryInStock>();

            foreach (DataRow DataRow in dt.Rows)
            {
                ExpiryInStock ExpiryInStock = new ExpiryInStock();
                ExpiryInStock.MovementID = Convert.ToInt32(DataRow["MovementID"]);
                ExpiryInStock.Location = Convert.ToString(DataRow["Location"]);
                ExpiryInStock.Branch = Convert.ToString(DataRow["Branch"]);
                ExpiryInStock.Code = Convert.ToString(DataRow["Code"]);
                ExpiryInStock.Items = Convert.ToString(DataRow["Items"]);
                ExpiryInStock.BatchCode = Convert.ToString(DataRow["BatchCode"]);
                ExpiryInStock.ExpiryDate = Convert.ToDateTime(DataRow["ExpiryDate"] is DBNull?null: DataRow["ExpiryDate"]);
                ExpiryInStock.Balance = Convert.ToDecimal(DataRow["Balance"] is DBNull ? 0 : DataRow["Balance"]);

                ListOfStock.Add(ExpiryInStock);
            }

            return ListOfStock;
        }

       
    }

    public class ExpiryInStock
    {
        public int MovementID { get; set; }
        public string Location { get; set; }
        public string Branch { get; set; }
        public string Code { get; set; }
        public string Items { get; set; }
        public string BatchCode { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal Balance { get; set; }
    }

}
