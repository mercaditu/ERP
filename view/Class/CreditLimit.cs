using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cognitivo.Reporting.Data;
using entity;

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

        public void Check_CreditAvailability(object Document)
        {
            string BaseName = Document.GetType().BaseType.ToString();
            string AppName = Document.GetType().ToString();

            if (AppName == typeof(sales_invoice).ToString() || BaseName == typeof(sales_invoice).ToString())
            {
                sales_invoice sales_invoice = (sales_invoice)Document;
                if (sales_invoice != null && sales_invoice.contact != null && sales_invoice.contact.credit_limit != null)
                {
                    decimal Balance = (decimal)SpecialFXBalance_ByCustomer(sales_invoice.app_currencyfx.buy_value, sales_invoice.id_contact);
                    sales_invoice.contact.credit_availability = Balance;
                    sales_invoice.contact.RaisePropertyChanged("credit_availability");
                }
            }
            else if (AppName == typeof(sales_budget).ToString() || BaseName == typeof(sales_budget).ToString())
            {
                sales_budget sales_budget = (sales_budget)Document;
                if (sales_budget != null && sales_budget.contact != null && sales_budget.contact.credit_limit != null)
                {
                    decimal Balance = (decimal)SpecialFXBalance_ByCustomer(sales_budget.app_currencyfx.buy_value, sales_budget.id_contact);
                    sales_budget.contact.credit_availability = Balance;
                    sales_budget.contact.RaisePropertyChanged("credit_availability");
                }
            }
            else if (AppName == typeof(sales_order).ToString() || BaseName == typeof(sales_order).ToString())
            {
                sales_order sales_order = (sales_order)Document;
                if (sales_order != null && sales_order.contact != null && sales_order.contact.credit_limit != null)
                {
                    decimal Balance = (decimal)SpecialFXBalance_ByCustomer(sales_order.app_currencyfx.buy_value, sales_order.id_contact);
                    sales_order.contact.credit_availability = Balance;
                    sales_order.contact.RaisePropertyChanged("credit_availability");
                }
            }
        }

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
        /// <param name="SpecialFXID">FX Rate we want to use</param>
        /// <param name="CustomerID">Customer ID we wantt o calculate for</param>
        /// <returns>The Balance in the Special Currency</returns>
        public decimal? SpecialFXBalance_ByCustomer(decimal SpecialFXRate, int CustomerID)
        {
            FinanceDS FinanceDS = new FinanceDS();
            FinanceDS.BeginInit();

            Cognitivo.Reporting.Data.FinanceDSTableAdapters.ContactBalance ContactBalance = new Cognitivo.Reporting.Data.FinanceDSTableAdapters.ContactBalance();
            decimal i = Convert.ToDecimal(ContactBalance.SpecialBalance_ByContact_Currency(SpecialFXRate, CustomerID));

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
