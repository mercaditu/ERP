using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cognitivo.Reporting.Data;

namespace Cognitivo.Class
{
    public class StockList
    {
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string Location { get; set; }
        public int ProductID { get; set; }
        public int LocationID { get; set; }
        public decimal Quantity { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string Measurement { get; set; }
    }

    public class StockCalculations
    {
        public List<StockList> CalculateStock_ByBranch(int BranchID, DateTime TransDate)
        {

            ProductDS ProductDS = new ProductDS();
            ProductDS.BeginInit();

            Cognitivo.Reporting.Data.ProductDSTableAdapters.Check_InStockTableAdapter Check_InStockTableAdapter = new Cognitivo.Reporting.Data.ProductDSTableAdapters.Check_InStockTableAdapter();

            //fill data
            Check_InStockTableAdapter.ClearBeforeFill = true;
            DataTable dt = Check_InStockTableAdapter.GetData(entity.CurrentSession.Id_Company, BranchID, TransDate);

            ProductDS.EndInit();
            return GenerateList(dt);

        }
        public List<StockList> CalculateProduct_InLocation(int LocationID, DateTime TransDate)
        {
            ProductDS ProductDS = new ProductDS();
            ProductDS.BeginInit();

            Cognitivo.Reporting.Data.ProductDSTableAdapters.Check_InStockTableAdapter Check_InStockTableAdapter = new Cognitivo.Reporting.Data.ProductDSTableAdapters.Check_InStockTableAdapter();

            //fill data
            Check_InStockTableAdapter.ClearBeforeFill = true;
            DataTable dt = Check_InStockTableAdapter.GetDataBy(entity.CurrentSession.Id_Company, LocationID, TransDate);

            ProductDS.EndInit();
            return GenerateList(dt);
        }
        public List<StockList> CalculateProduct_InItemBranch(int BranchID, int ProductID, DateTime TransDate)
        {
            ProductDS ProductDS = new ProductDS();
            ProductDS.BeginInit();

            Cognitivo.Reporting.Data.ProductDSTableAdapters.Check_InStockTableAdapter Check_InStockTableAdapter = new Cognitivo.Reporting.Data.ProductDSTableAdapters.Check_InStockTableAdapter();

            //fill data
            Check_InStockTableAdapter.ClearBeforeFill = true;
            DataTable dt = Check_InStockTableAdapter.GetDataByItemBranch(entity.CurrentSession.Id_Company, BranchID, TransDate, ProductID);

            ProductDS.EndInit();
            return GenerateList(dt);
        }
        public List<StockList> CalculateProduct_InItemLocation(int LocationID, int ProductID, DateTime TransDate)
        {
            ProductDS ProductDS = new ProductDS();
            ProductDS.BeginInit();

            Cognitivo.Reporting.Data.ProductDSTableAdapters.Check_InStockTableAdapter Check_InStockTableAdapter = new Cognitivo.Reporting.Data.ProductDSTableAdapters.Check_InStockTableAdapter();

            //fill data
            Check_InStockTableAdapter.ClearBeforeFill = true;
            DataTable dt = Check_InStockTableAdapter.GetDataByItemLocation(entity.CurrentSession.Id_Company, LocationID, TransDate, ProductID);

            ProductDS.EndInit();
            return GenerateList(dt);
        }
        public List<StockList> CalculateProduct_InItemLocationByDetail(int LocationID, int ProductID, DateTime TransDate)
        {
            ProductDS ProductDS = new ProductDS();
            ProductDS.BeginInit();

            Cognitivo.Reporting.Data.ProductDSTableAdapters.stockflowTableAdapter stockflowTableAdapter = new Cognitivo.Reporting.Data.ProductDSTableAdapters.stockflowTableAdapter();

            //fill data
            stockflowTableAdapter.ClearBeforeFill = true;
            DataTable dt = stockflowTableAdapter.GetDataByItemLocation(ProductID, entity.CurrentSession.Id_Company, LocationID);

            ProductDS.EndInit();
            return GenerateList(dt);
        }
        public List<StockList> GenerateList(DataTable dt)
        {
            List<StockList> StockList = new List<Class.StockList>();
            foreach (DataRow DataRow in dt.Rows)
            {
                StockList Stock = new Class.StockList();
                Stock.ItemCode = DataRow["ItemCode"].ToString();
                Stock.ItemName = DataRow["ItemName"].ToString();
                Stock.Location = DataRow["Location"].ToString();
                Stock.LocationID = Convert.ToInt16(DataRow["LocationID"]);
                Stock.Measurement = DataRow["Measurement"].ToString();
                Stock.Quantity = Convert.ToDecimal(DataRow["Quantity"]);
                Stock.ProductID = Convert.ToInt16(DataRow["ProductID"]);
                StockList.Add(Stock);
            }
            return StockList;
        }
    }
}
