using entity;
using entity.BrilloQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace cntrl.Panels
{
    public partial class pnl_ItemMovementExpiry : UserControl
    {
        public ProductMovementDB ProductMovementDB = new ProductMovementDB();
        private CollectionViewSource ExpiryInStockViewSource;
        public int MovementID { get; set; }

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
                                im.code as BatchCode, im.expire_date as ExpiryDate, im.barcode as BarCode,
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

            txtsearch.Focus();
        }

        private void btnCancel_Click(object sender, MouseButtonEventArgs e)
        {
            item_inventory_detailDataGrid.CancelEdit();
            Grid parentGrid = (Grid)Parent;
            parentGrid.Visibility = Visibility.Hidden;
            parentGrid.Children.Clear();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ExpiryInStock ExpiryInStock = ExpiryInStockViewSource.View.CurrentItem as ExpiryInStock;

            if (ExpiryInStock!=null)
            {
                MovementID = ExpiryInStock.MovementID;
            }
           
            Grid parentGrid = (Grid)Parent;
            parentGrid.Visibility = Visibility.Hidden;
            parentGrid.Children.Clear();
        }

        private List<ExpiryInStock> BatchCodeLoader(DataTable dt)
        {
            List<ExpiryInStock> ListOfStock = new List<ExpiryInStock>();
			BarcodeGenerator.BarcodeGenerate BG = new BarcodeGenerator.BarcodeGenerate();
			

			foreach (DataRow DataRow in dt.Rows)
            {
                ExpiryInStock ExpiryInStock = new ExpiryInStock()
                {
                    MovementID = Convert.ToInt32(DataRow["MovementID"]),
                    BarCode = Convert.ToString(BG.Decodestring(Convert.ToString(DataRow["BarCode"]))),
                    Location = Convert.ToString(DataRow["Location"]),
                    Branch = Convert.ToString(DataRow["Branch"]),
                    Code = Convert.ToString(DataRow["Code"]),
                    Items = Convert.ToString(DataRow["Items"]),
                    BatchCode = Convert.ToString(DataRow["BatchCode"]),
                    ExpiryDate = Convert.ToDateTime(DataRow["ExpiryDate"] is DBNull ? null : DataRow["ExpiryDate"]),
                    Balance = Convert.ToDecimal(DataRow["Balance"] is DBNull ? 0 : DataRow["Balance"])
                };

                ListOfStock.Add(ExpiryInStock);
            }

            return ListOfStock;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ExpiryInStockViewSource != null)
            {
                if (ExpiryInStockViewSource.View != null)
                {
                    ExpiryInStockViewSource.View.Filter = i =>
                    {
                        dynamic ExpiryInStock = (ExpiryInStock)i;

                        if (ExpiryInStock.BarCode.ToUpper().Contains(txtsearch.Text.ToUpper()) ||
                            ExpiryInStock.Code.ToUpper().Contains(txtsearch.Text.ToUpper()) ||
                            ExpiryInStock.Location.ToUpper().Contains(txtsearch.Text.ToUpper()))
                        {
                            //This code checks for Quantity after checking for name. This will cause less loops.
                            return true;
                        }

                        return false;
                    };
                }
            }
        }

		private void txtsearch_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				TextBox_TextChanged(null, null);
			}
		}
	}

	public class ExpiryInStock
    {
        public int MovementID { get; set; }
        public string Location { get; set; }
        public string Branch { get; set; }
        public string Code { get; set; }
        public string Items { get; set; }
        public string BarCode { get; set; }
        public string BatchCode { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal Balance { get; set; }
    }
}