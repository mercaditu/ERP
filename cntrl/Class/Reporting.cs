using System.Collections.Generic;

namespace cntrl.Class
{
    public class Generate
    {
        public List<Report> ReportList { get; set; }

        public void GenerateReportList()
        {
            ReportList = new List<Report>
            {
            new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name = entity.Brillo.Localize.StringText("CostOfGoodsSold")!= string.Empty ? entity.Brillo.Localize.StringText("CostOfGoodsSold") :"CostOfGoodsSold",
                Path = "cntrl.Reports.Sales.CostOfGoodsSold.rdlc",
                Query = Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
            new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name = entity.Brillo.Localize.StringText("SalesDetails")!= string.Empty ? entity.Brillo.Localize.StringText("SalesDetails") :"SalesDetails",
                Path = "cntrl.Reports.Sales.SalesDetail.rdlc",
                Query = Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
            new Report
            {
                Application = entity.App.Names.SalesOrder,
                Name =entity.Brillo.Localize.StringText("SalesDetails")!= string.Empty ? entity.Brillo.Localize.StringText("SalesDetails") :"SalesDetails",
                Path = "cntrl.Reports.Sales.SalesDetail.rdlc",
                Query =  Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="sales_invoice",
                ReplaceWithString="sales_order"
            },
            new Report
            {
                Application = entity.App.Names.SalesBudget,
                Name =entity.Brillo.Localize.StringText("SalesDetails")!= string.Empty ? entity.Brillo.Localize.StringText("SalesDetails") :"SalesDetails",
                Path = "cntrl.Reports.Sales.SalesDetail.rdlc",
                Query=  Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="sales_invoice",
                ReplaceWithString="sales_budget"
            },
              new Report
            {
                Application = entity.App.Names.SalesReturn,
                Name =entity.Brillo.Localize.StringText("SalesDetails")!= string.Empty ? entity.Brillo.Localize.StringText("SalesDetails") :"SalesDetails",
                Path = "cntrl.Reports.Sales.SalesDetail.rdlc",
                Query=  Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="sales_invoice",
                ReplaceWithString="sales_return"
            },

            new Report
            {
                Application = entity.App.Names.SalesBudget,
                Name =entity.Brillo.Localize.StringText("SalesByItem")!= string.Empty ? entity.Brillo.Localize.StringText("SalesByItem") :"SalesByItem",
                Path = "cntrl.Reports.Sales.SalesByItem.rdlc",
                Query = Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                 ReplaceString="sales_invoice",
                ReplaceWithString="sales_budget"
            },
                new Report
            {
                Application = entity.App.Names.SalesOrder,
                Name =entity.Brillo.Localize.StringText("SalesByItem")!= string.Empty ? entity.Brillo.Localize.StringText("SalesByItem") :"SalesByItem",
                Path = "cntrl.Reports.Sales.SalesByItem.rdlc",
                Query = Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                 ReplaceString="sales_invoice",
                ReplaceWithString="sales_order"
            },
            new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name =entity.Brillo.Localize.StringText("SalesByItem")!= string.Empty ? entity.Brillo.Localize.StringText("SalesByItem") :"SalesByItem",
                Path = "cntrl.Reports.Sales.SalesByItem.rdlc",
                Query = Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
            },
                        new Report
            {
                Application = entity.App.Names.SalesReturn,
                Name =entity.Brillo.Localize.StringText("SalesByItem")!= string.Empty ? entity.Brillo.Localize.StringText("SalesByItem") :"SalesByItem",
                Path = "cntrl.Reports.Sales.SalesByItem.rdlc",
                Query = Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="sales_invoice",
                ReplaceWithString="sales_return"
            },


            /// Sales (Invoice, Order, and Budget) ByCustomer Reports

            new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name =entity.Brillo.Localize.StringText("SalesByCustomer")!= string.Empty ? entity.Brillo.Localize.StringText("SalesByCustomer") :"SalesByCustomer",
                Path = "cntrl.Reports.Sales.SalesByCustomer.rdlc",
                Query = Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },

            new Report
            {
                Application = entity.App.Names.SalesOrder,
                Name =entity.Brillo.Localize.StringText("SalesOrderByCustomer")!= string.Empty ? entity.Brillo.Localize.StringText("SalesOrderByCustomer") :"SalesOrderByCustomer",
                Path = "cntrl.Reports.Sales.SalesByCustomer.rdlc",
                Query = Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                 ReplaceString="sales_invoice",
                ReplaceWithString="sales_order"
            },

            new Report
            {
                Application = entity.App.Names.SalesBudget,
                Name =entity.Brillo.Localize.StringText("SalesBudgetByCustomer")!= string.Empty ? entity.Brillo.Localize.StringText("SalesBudgetByCustomer") :"SalesBudgetByCustomer",
                Path = "cntrl.Reports.Sales.SalesByCustomer.rdlc",
                Query = Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                  ReplaceString="sales_invoice",
                ReplaceWithString="sales_budget"
            },
             new Report
            {
                Application = entity.App.Names.SalesReturn,
                Name =entity.Brillo.Localize.StringText("SalesReturnByCustomer")!= string.Empty ? entity.Brillo.Localize.StringText("SalesReturnByCustomer") :"SalesReturnByCustomer",
                Path = "cntrl.Reports.Sales.SalesByCustomer.rdlc",
                Query=  Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="sales_invoice",
                ReplaceWithString="sales_return"
            },

            new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name =entity.Brillo.Localize.StringText("SalesByBranch")!= string.Empty ? entity.Brillo.Localize.StringText("SalesByBranch") :"SalesByBranch",
                Path = "cntrl.Reports.Sales.SalesByBranch.rdlc",
                Query = Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
              new Report
            {
                Application = entity.App.Names.SalesOrder,
                Name =entity.Brillo.Localize.StringText("SalesOrderByBranch")!= string.Empty ? entity.Brillo.Localize.StringText("SalesOrderByBranch") :"SalesOrderByBranch",
                Path = "cntrl.Reports.Sales.SalesByBranch.rdlc",
                Query = Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="sales_invoice",
                ReplaceWithString="sales_order"
            },
            new Report
            {
                Application = entity.App.Names.SalesBudget,
                Name =entity.Brillo.Localize.StringText("SalesBudgetByBranch")!= string.Empty ? entity.Brillo.Localize.StringText("SalesBudgetByBranch") :"SalesBudgetByBranch",
                Path = "cntrl.Reports.Sales.SalesByBranch.rdlc",
                Query = Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                 ReplaceString="sales_invoice",
                ReplaceWithString="sales_budget"
            },
               new Report
            {
                Application = entity.App.Names.SalesReturn,
                Name =entity.Brillo.Localize.StringText("SalesReturnByBranch")!= string.Empty ? entity.Brillo.Localize.StringText("SalesReturnByBranch") :"SalesReturnByBranch",
                Path = "cntrl.Reports.Sales.SalesByBranch.rdlc",
                Query=  Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="sales_invoice",
                ReplaceWithString="sales_return"
            },

                    new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name =entity.Brillo.Localize.StringText("SalesBySalesman")!= string.Empty ? entity.Brillo.Localize.StringText("SalesBySalesman") :"SalesBySalesman",
                Path = "cntrl.Reports.Sales.SalesBySalesRep.rdlc",
                Query = Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
              new Report
            {
                Application = entity.App.Names.SalesOrder,
                Name =entity.Brillo.Localize.StringText("SalesBySalesman")!= string.Empty ? entity.Brillo.Localize.StringText("SalesBySalesman") :"SalesBySalesman",
                Path = "cntrl.Reports.Sales.SalesBySalesRep.rdlc",
                Query = Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="sales_invoice",
                ReplaceWithString="sales_order"
            },
            new Report
            {
                Application = entity.App.Names.SalesBudget,
                Name =entity.Brillo.Localize.StringText("SalesBySalesman")!= string.Empty ? entity.Brillo.Localize.StringText("SalesBySalesman") :"SalesBySalesman",
                Path = "cntrl.Reports.Sales.SalesBySalesRep.rdlc",
                Query = Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                 ReplaceString="sales_invoice",
                ReplaceWithString="sales_budget"
            },
               new Report
            {
                Application = entity.App.Names.SalesReturn,
                Name =entity.Brillo.Localize.StringText("SalesBySalesman")!= string.Empty ? entity.Brillo.Localize.StringText("SalesBySalesman") :"SalesBySalesman",
                Path = "cntrl.Reports.Sales.SalesBySalesRep.rdlc",
                Query=  Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="sales_invoice",
                ReplaceWithString="sales_return"
            },

               new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name =entity.Brillo.Localize.StringText("SalesAnalysis")!= string.Empty ? entity.Brillo.Localize.StringText("SalesAnalysis") :"SalesAnalysis",
                Path = "cntrl.Reports.Sales.SalesAnalysis.rdlc",
                Query = Reports.Sales.SalesAnalysis.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
            },

            //purchase
              new Report
            {
                Application = entity.App.Names.PurchaseInvoice,
                Name =entity.Brillo.Localize.StringText("PurchaseDetail")!= string.Empty ? entity.Brillo.Localize.StringText("PurchaseDetail") :"PurchaseDetail",
                Path = "cntrl.Reports.Purchases.PurchaseDetail.rdlc",
                Query = Reports.Purchase.Purchase.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
              new Report
            {
                Application = entity.App.Names.PurchaseOrder,
                Name =entity.Brillo.Localize.StringText("PurchaseDetail")!= string.Empty ? entity.Brillo.Localize.StringText("PurchaseDetail") :"PurchaseDetail",
                Path = "cntrl.Reports.Purchases.PurchaseDetail.rdlc",
                Query = Reports.Purchase.Purchase.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="purchase_invoice",
                ReplaceWithString="purchase_order"
            },
              new Report
            {
                Application = entity.App.Names.PurchaseReturn,
                Name =entity.Brillo.Localize.StringText("PurchaseDetail")!= string.Empty ? entity.Brillo.Localize.StringText("PurchaseDetail") :"PurchaseDetail",
                Path = "cntrl.Reports.Purchases.PurchaseDetail.rdlc",
                Query = Reports.Purchase.Purchase.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="purchase_invoice",
                ReplaceWithString="purchase_return"
            },
                 new Report
            {
                Application = entity.App.Names.PurchaseInvoice,
                Name =entity.Brillo.Localize.StringText("PurchaseBySupplier")!= string.Empty ? entity.Brillo.Localize.StringText("PurchaseBySupplier") :"PurchaseBySupplier",
                Path = "cntrl.Reports.Purchases.PurchaseBySupplier.rdlc",
                Query = Reports.Purchase.Purchase.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
              new Report
            {
                Application = entity.App.Names.PurchaseOrder,
                Name =entity.Brillo.Localize.StringText("PurchaseOrderBySupplier")!= string.Empty ? entity.Brillo.Localize.StringText("PurchaseOrderBySupplier") :"PurchaseOrderBySupplier",
                Path = "cntrl.Reports.Purchases.PurchaseBySupplier.rdlc",
                Query = Reports.Purchase.Purchase.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="purchase_invoice",
                ReplaceWithString="purchase_order"
            },
               new Report
            {
                Application = entity.App.Names.PurchaseReturn,
                Name =entity.Brillo.Localize.StringText("PurchaseReturnBySupplier")!= string.Empty ? entity.Brillo.Localize.StringText("PurchaseReturnBySupplier") :"PurchaseReturnBySupplier",
                Path = "cntrl.Reports.Purchases.PurchaseBySupplier.rdlc",
                Query = Reports.Purchase.Purchase.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="purchase_invoice",
                ReplaceWithString="purchase_return"
            },
                 new Report
            {
                Application = entity.App.Names.PurchaseInvoice,
                Name =entity.Brillo.Localize.StringText("PurchaseByCostCenter")!= string.Empty ? entity.Brillo.Localize.StringText("PurchaseByCostCenter") :"PurchaseByCostCenter",
                Path = "cntrl.Reports.Purchases.PurchaseByCostCenter.rdlc",
                Query = Reports.Purchase.Purchase.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
              new Report
            {
                Application = entity.App.Names.PurchaseOrder,
                Name =entity.Brillo.Localize.StringText("PurchaseOrderByCostCenter")!= string.Empty ? entity.Brillo.Localize.StringText("PurchaseOrderByCostCenter") :"PurchaseOrderByCostCenter",
                Path = "cntrl.Reports.Purchases.PurchaseByCostCenter.rdlc",
                Query = Reports.Purchase.Purchase.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="purchase_invoice",
                ReplaceWithString="purchase_order"
            },
                  new Report
            {
                Application = entity.App.Names.PurchaseReturn,
                Name =entity.Brillo.Localize.StringText("PurchaseReturnByCostCenter")!= string.Empty ? entity.Brillo.Localize.StringText("PurchaseReturnByCostCenter") :"PurchaseReturnByCostCenter",
                Path = "cntrl.Reports.Purchases.PurchaseByCostCenter.rdlc",
                Query = Reports.Purchase.Purchase.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="purchase_invoice",
                ReplaceWithString="purchase_return"
            },
                  new Report
            {
                Application = entity.App.Names.PurchaseTender,
                Name =entity.Brillo.Localize.StringText("PurchaseTender"),
                Path = "cntrl.Reports.Purchases.PurchaseTender.rdlc",
                Query = Reports.Purchase.PurchaseTender.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
            },

                //stock

                new Report
            {
                Application = entity.App.Names.PriceList,
                Name =entity.Brillo.Localize.StringText("PriceList"),
                Path = "cntrl.Reports.Stocks.PriceList.rdlc",
                Query = Reports.Stock.PriceList.query,
                Parameters = new List<Report.Types> { }
            },
            new Report
            {
                Application = entity.App.Names.Stock,
                Name = entity.Brillo.Localize.StringText("Items"),
                Path = "cntrl.Reports.Stocks.Items.rdlc",
                Query = Reports.Stock.Item.query,
                Parameters = new List<Report.Types> { }
            },
                new Report
            {
                Application = entity.App.Names.Stock,
                Name =entity.Brillo.Localize.StringText("StockMovement"),
                Path = "cntrl.Reports.Stocks.StockMovement.rdlc",
                Query = Reports.Stock.Stock.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
            },
            new Report
            {
                Application = entity.App.Names.Stock,
                Name = entity.Brillo.Localize.StringText("StockAnalysis")!= string.Empty ? entity.Brillo.Localize.StringText("StockAnalysis") :"StockAnalysis",
                Path = "cntrl.Reports.Stocks.StockAnalysis.rdlc",
                Query = Reports.Stock.StockAnalysis.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
              new Report
            {
                Application = entity.App.Names.Stock,
                Name =entity.Brillo.Localize.StringText("StockFlow")!= string.Empty ? entity.Brillo.Localize.StringText("StockFlow") :"StockFlow",
                Path = "cntrl.Reports.Stocks.StockFlow.rdlc",
                Query = Reports.Stock.Stock.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
            },
                 new Report
            {
                Application = entity.App.Names.Stock,
                Name =entity.Brillo.Localize.StringText("StockValue")!= string.Empty ? entity.Brillo.Localize.StringText("StockValue") :"StockValue",
                Path = "cntrl.Reports.Stocks.StockValue.rdlc",
                Query = Reports.Stock.InventoryValue.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
            },
                  new Report
            {
                Application = entity.App.Names.Inventory,
                Name =entity.Brillo.Localize.StringText("Inventory"),
                Path = "cntrl.Reports.Stocks.Inventory.rdlc",
                Query = Reports.Stock.InventorySummary.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
            },
             new Report
            {
                Application = entity.App.Names.Inventory,
                Name =entity.Brillo.Localize.StringText("Batch"),
                Path = "cntrl.Reports.Stocks.InventoryByBatch.rdlc",
                Query = Reports.Stock.InventorySummary.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
            },
                    new Report
            {
                Application = entity.App.Names.Movement,
                Name =entity.Brillo.Localize.StringText("Movement")!= string.Empty ? entity.Brillo.Localize.StringText("Movement") :"Movement",
                Path = "cntrl.Reports.Stocks.Movement.rdlc",
                Query = Reports.Stock.MovementSummary.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
            },
                                        new Report
            {
                Application = entity.App.Names.Transfer,
                Name =entity.Brillo.Localize.StringText("Transfer")!= string.Empty ? entity.Brillo.Localize.StringText("Transfer") :"Transfer",
                Path = "cntrl.Reports.Stocks.Movement.rdlc",
                Query = Reports.Stock.TransferSummary.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
            },
                                         new Report
            {
                Application = entity.App.Names.Recipe,
                Name =entity.Brillo.Localize.StringText("Recipe")!= string.Empty ? entity.Brillo.Localize.StringText("Recipe") :"Recipe",
                Path = "cntrl.Reports.Stocks.ItemReceipe.rdlc",
                Query =  Reports.Stock.ItemReceipe.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},

            },

              //Customers
                 new Report
            {
                Application = entity.App.Names.Customer,
                Name =entity.Brillo.Localize.StringText("ContactsByBank")!= string.Empty ? entity.Brillo.Localize.StringText("ContactsByBank") :"ContactsByBank",
                Path = "cntrl.Reports.Commercials.ContactsByBank.rdlc",
                Query = Reports.Commercial.Customer.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
            },
            new Report
            {
                Application = entity.App.Names.Customer,
                Name =entity.Brillo.Localize.StringText("ContactsByGeography")!= string.Empty ? entity.Brillo.Localize.StringText("ContactsByGeography") :"ContactsByGeography",
                Path = "cntrl.Reports.Commercials.ContactsByGeography.rdlc",
                Query = Reports.Commercial.Customer.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
            },
               new Report
            {
                Application = entity.App.Names.Customer,
                Name =entity.Brillo.Localize.StringText("ContactsByTag")!= string.Empty ? entity.Brillo.Localize.StringText("ContactsByTag") :"ContactsByTag",
                Path = "cntrl.Reports.Commercials.ContactsByTag.rdlc",
                Query = Reports.Commercial.Customer.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
            },

               //supplier
                     new Report
            {
                Application = entity.App.Names.Supplier,
                Name =entity.Brillo.Localize.StringText("ContactsByBank")!= string.Empty ? entity.Brillo.Localize.StringText("ContactsByBank") :"ContactsByBank",
                Path = "cntrl.Reports.Commercials.ContactsByBank.rdlc",
                Query = Reports.Commercial.Customer.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
            },
            new Report
            {
                Application = entity.App.Names.Supplier,
                Name =entity.Brillo.Localize.StringText("ContactsByGeography")!= string.Empty ? entity.Brillo.Localize.StringText("ContactsByGeography") :"ContactsByGeography",
                Path = "cntrl.Reports.Commercials.ContactsByGeography.rdlc",
                Query = Reports.Commercial.Customer.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
            },

                new Report
            {
                Application = entity.App.Names.Supplier,
                Name =entity.Brillo.Localize.StringText("ContactsByTag")!= string.Empty ? entity.Brillo.Localize.StringText("ContactsByTag") :"ContactsByTag",
                Path = "cntrl.Reports.Commercials.ContactsByTag.rdlc",
                Query = Reports.Commercial.Customer.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
            },

                //Finance

                new Report
            {
                Application = entity.App.Names.AccountsPayable,
                Name =entity.Brillo.Localize.StringText("AccountPayable")!= string.Empty ? entity.Brillo.Localize.StringText("AccountPayable") :"AccountPayable",
                Path = "cntrl.Reports.Finances.AccountBalance.rdlc",
                Query = Reports.Finance.PendingPayables.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
            },

                new Report
            {
                Application = entity.App.Names.AccountsPayable,
                Name = entity.Brillo.Localize.StringText("NonDirect_Payment"),
                Path = "cntrl.Reports.Finances.NonDirect_Payment.rdlc",
                Query = Reports.Finance.NonDirect_Payment_Supplier.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
            },

                new Report
            {
                Application = entity.App.Names.AccountsReceivable,
                Name =entity.Brillo.Localize.StringText("AccountReceivable")!= string.Empty ? entity.Brillo.Localize.StringText("AccountReceivable") :"AccountReceivable",
                Path = "cntrl.Reports.Finances.AccountBalance.rdlc",
                Query = Reports.Finance.PendingReceivables.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
            },
            new Report
            {
                Application = entity.App.Names.AccountsReceivable,
                Name = entity.Brillo.Localize.StringText("PaymentRecievableByDate"),
                Path = "cntrl.Reports.Finances.PaymentRecievableByDate.rdlc",
                Query = Reports.Finance.PaymentRecievables.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
            },
            new Report
            {
                Application = entity.App.Names.AccountsReceivable,
                Name = entity.Brillo.Localize.StringText("PaymentRecievablesBySalesman"),
                Path = "cntrl.Reports.Finances.PaymentRecievablesBySalesman.rdlc",
                Query = Reports.Finance.PaymentRecievables.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
            },
            new Report
            {
                Application = entity.App.Names.AccountsReceivable,
                Name = entity.Brillo.Localize.StringText("NonDirect_Payment"),
                Path = "cntrl.Reports.Finances.NonDirect_Payment.rdlc",
                Query = Reports.Finance.NonDirect_Payment.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
            },
             //projects and Production
            new Report
            {
                Application = entity.App.Names.ActivityPlan,
                Name = entity.Brillo.Localize.StringText("ActivityPlan"),
                Path = "cntrl.Reports.Projects.Project.rdlc",
                Query =  Reports.Project.Project.query,
                Parameters = new List<Report.Types> { Report.Types.Project}
            },
            new Report
                {
                Application = entity.App.Names.ActivityPlan,
                Name =entity.Brillo.Localize.StringText("HumanResource"),
                Path = "cntrl.Reports.Projects.ProjectHR.rdlc",
                Query = Reports.Project.Project.query,
                Parameters = new List<Report.Types> { Report.Types.Project }
            },
            new Report
                {
                Application = entity.App.Names.ActivityPlan,
                Name =entity.Brillo.Localize.StringText("RawMaterials"),
                Path = "cntrl.Reports.Projects.ProjectRawMaterial.rdlc",
                Query = Reports.Project.Project.query,
                Parameters = new List<Report.Types> { Report.Types.Project }
            },
            new Report
                {
                Application = entity.App.Names.ActivityPlan,
                Name =entity.Brillo.Localize.StringText("Percentage"),
                Path = "cntrl.Reports.Projects.ProjectImportance.rdlc",
                Query = Reports.Project.Project.query,
                Parameters = new List<Report.Types> { Report.Types.Project }
            },
                new Report
                {
                Application = entity.App.Names.ActivityPlan,
                Name =entity.Brillo.Localize.StringText("TechnicalReport"),
                Path = "cntrl.Reports.Projects.Technical.rdlc",
                Query = Reports.Project.Project.query,
                Parameters = new List<Report.Types> { Report.Types.Project }
            },
               new Report
            {
                Application = entity.App.Names.ProjectExecution,
                Name =entity.Brillo.Localize.StringText("ProjectExecution")!= string.Empty ? entity.Brillo.Localize.StringText("ProjectExecution") :"ProjectExecution",
                Path = "cntrl.Reports.Projects.ProjectExecution.rdlc",
                Query = Reports.Project.Project.query,
                Parameters = new List<Report.Types> { Report.Types.Project }
            },  new Report
            {
                Application = entity.App.Names.ProjectFinance,
                Name =entity.Brillo.Localize.StringText("ProjectFinance")!= string.Empty ? entity.Brillo.Localize.StringText("ProjectFinance") :"ProjectFinance",
                Path = "cntrl.Reports.Projects.ProjectFinance.rdlc",
                Query =Reports.Project.ProjectFinance.query,
                Parameters = new List<Report.Types> { Report.Types.Project }
            }
            , new Report
                {
                Application = entity.App.Names.ProductionOrder,
                Name =entity.Brillo.Localize.StringText("ProductionOrder")!= string.Empty ? entity.Brillo.Localize.StringText("ProductionOrder") :"ProductionOrder",
                Path = "cntrl.Reports.Productions.ProductionOrder.rdlc",
                Query = Reports.Production.Production.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate }
            }
            , new Report
                {
                Application = entity.App.Names.ProductionOrder,
                Name =entity.Brillo.Localize.StringText("ProductionOrderStatus")!= string.Empty ? entity.Brillo.Localize.StringText("ProductionOrderStatus") :"ProductionOrderStatus",
                Path = "cntrl.Reports.Productions.ProductionStatus.rdlc",
                Query = Reports.Production.ProductionOrderStatus.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate }
            } , new Report
                {
                Application = entity.App.Names.ProductionOrder,
                Name =entity.Brillo.Localize.StringText("HR")!= string.Empty ? entity.Brillo.Localize.StringText("HR") :"HR",
                Path = "cntrl.Reports.Productions.HR.rdlc",
                Query =Reports.Production.Production.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate }
            },new Report
                {
                Application = entity.App.Names.ProductionOrder,
                Name =entity.Brillo.Localize.StringText("EmployeesInProduction")!= string.Empty ? entity.Brillo.Localize.StringText("EmployeesInProduction") :"EmployeesInProduction",
                Path = "cntrl.Reports.Productions.EmployeesInProduction.rdlc",
                Query =Reports.Production.EmployeesInProduction.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate }
            }
            };

        }
    }

    public class Report
    {
        public enum Types
        {
            StartDate, EndDate,
            Project
        }

        public entity.App.Names Application { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Query { get; set; }
        public string ReplaceString { get; set; }
        public string ReplaceWithString { get; set; }

        public ICollection<Types> Parameters { get; set; }
    }
}