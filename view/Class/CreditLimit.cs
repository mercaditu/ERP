using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cognitivo.Reporting.Data;

namespace Cognitivo.Class
{
    //public class CreditList
    //{
    //    public string ItemName { get; set; }
    //    public string ItemCode { get; set; }
    //    public string Location { get; set; }
    //    public int ProductID { get; set; }
    //    public int LocationID { get; set; }
    //    public decimal Quantity { get; set; }
    //    public decimal Cost { get; set; }
    //    public string Measurement { get; set; }
    //}

    public class CreditLimit
    {
        //public List<StockList> ByBranch(int BranchID, DateTime TransDate)
        //{

        //    ProductDS ProductDS = new ProductDS();
        //    ProductDS.BeginInit();

        //    Cognitivo.Reporting.Data.ProductDSTableAdapters.Check_InStockTableAdapter Check_InStockTableAdapter = new Cognitivo.Reporting.Data.ProductDSTableAdapters.Check_InStockTableAdapter();

        //    //fill data
        //    Check_InStockTableAdapter.ClearBeforeFill = true;
        //    DataTable dt = Check_InStockTableAdapter.GetDataByBranch(entity.CurrentSession.Id_Company, BranchID, TransDate);

        //    ProductDS.EndInit();
        //    return GenerateList(dt);

        //}

        //public List<StockList> ByBranchLocation(int LocationID, DateTime TransDate)
        //{
        //    ProductDS ProductDS = new ProductDS();
        //    ProductDS.BeginInit();

        //    Cognitivo.Reporting.Data.ProductDSTableAdapters.Check_InStockTableAdapter Check_InStockTableAdapter = new Cognitivo.Reporting.Data.ProductDSTableAdapters.Check_InStockTableAdapter();

        //    //fill data
        //    Check_InStockTableAdapter.ClearBeforeFill = true;
        //    DataTable dt = Check_InStockTableAdapter.GetDataByBranchLocation(entity.CurrentSession.Id_Company, LocationID, TransDate);

        //    ProductDS.EndInit();
        //    return GenerateList(dt);
        //}

        /// <summary>
        /// Gets the Balance of the customer by the company's default currency.
        /// </summary>
        /// <param name="CustomerID">Contact we want to use.</param>
        /// <returns>The Balance in the Default Currency</returns>
        public decimal? DefaultBalance_ByCustomer(int CustomerID)
        {
            FinanceDS FinanceDS = new FinanceDS();
            FinanceDS.BeginInit();

            Cognitivo.Reporting.Data.FinanceDSTableAdapters.ContactBalance ContactBalance = new Cognitivo.Reporting.Data.FinanceDSTableAdapters.ContactBalance();
            decimal i = Convert.ToDecimal(ContactBalance.DefaultBalance_ByCustomer(CustomerID));

            FinanceDS.EndInit();
            
            return i;
        }

        /// <summary>
        /// Gets the Balance of the Customer by whichever FX Rate you wish to pass.
        /// </summary>
        /// <param name="SpecialFXID">FX ID we want to use</param>
        /// <param name="CustomerID">Customer ID we wantt o calculate for</param>
        /// <returns>The Balance in the Special Currency</returns>
        public decimal? SpecialFXBalance_ByCustomer(int SpecialFXID, int CustomerID)
        {
            FinanceDS FinanceDS = new FinanceDS();
            FinanceDS.BeginInit();

            Cognitivo.Reporting.Data.FinanceDSTableAdapters.ContactBalance ContactBalance = new Cognitivo.Reporting.Data.FinanceDSTableAdapters.ContactBalance();
            decimal i = Convert.ToDecimal(ContactBalance.SpecialBalance_ByContact_Currency(SpecialFXID, CustomerID));

            FinanceDS.EndInit();

            return i;
        }

        //private List<StockList> GenerateList(DataTable dt)
        //{
        //    List<StockList> StockList = new List<Class.StockList>();
        //    foreach (DataRow DataRow in dt.Rows)
        //    {
        //        StockList Stock = new Class.StockList();
        //        Stock.ItemCode = DataRow["ItemCode"].ToString();
        //        Stock.ItemName = DataRow["ItemName"].ToString();
        //        Stock.Location = DataRow["Location"].ToString();
        //        Stock.LocationID = Convert.ToInt16(DataRow["LocationID"]);
        //        Stock.Measurement = DataRow["Measurement"].ToString();
        //        Stock.Quantity = Convert.ToDecimal(DataRow["Quantity"]);
        //        Stock.ProductID = Convert.ToInt16(DataRow["ProductID"]);
        //        Stock.Cost = Convert.ToDecimal(DataRow["Cost"]);
        //        StockList.Add(Stock);
        //    }
        //    return StockList;
        //}
    }
}
