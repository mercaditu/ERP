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
            // Sales (Invoice, Order, and Budget) Detail Reports
            new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name ="SalesDetail",
                Path = "cntrl.Reports.Sales.SalesDetail.rdlc",
                Query = Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
            new Report
            {
                Application = entity.App.Names.SalesOrder,
                Name ="SalesOrderDetail",
                Path = "cntrl.Reports.Sales.SalesDetail.rdlc",
                Query =  Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="sales_invoice",
                ReplaceWithString="sales_order"
            },
            new Report
            {
                Application = entity.App.Names.SalesBudget,
                Name ="SalesBudgetDetail",
                Path = "cntrl.Reports.Sales.SalesDetail.rdlc",
                Query=  Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="sales_invoice",
                ReplaceWithString="sales_budget"
            },
              new Report
            {
                Application = entity.App.Names.SalesReturn,
                Name ="SalesReturnDetail",
                Path = "cntrl.Reports.Sales.SalesDetail.rdlc",
                Query=  Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="sales_invoice",
                ReplaceWithString="sales_return"
            },

            /// Sales (Invoice, Order, and Budget) ByCustomer Reports

            new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name ="SalesByCustomer",
                Path = "cntrl.Reports.Sales.SalesByCustomer.rdlc",
                Query = Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },

            new Report
            {
                Application = entity.App.Names.SalesOrder,
                Name ="SalesOrderByCustomer",
                Path = "cntrl.Reports.Sales.SalesByCustomer.rdlc",
                Query = Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                 ReplaceString="sales_invoice",
                ReplaceWithString="sales_order"
            },

            new Report
            {
                Application = entity.App.Names.SalesBudget,
                Name ="SalesBudgetByCustomer",
                Path = "cntrl.Reports.Sales.SalesByCustomer.rdlc",
                Query = Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                  ReplaceString="sales_invoice",
                ReplaceWithString="sales_budget"
            },
             new Report
            {
                Application = entity.App.Names.SalesReturn,
                Name ="SalesReturnByCustomer",
                Path = "cntrl.Reports.Sales.SalesByCustomer.rdlc",
                Query=  Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="sales_invoice",
                ReplaceWithString="sales_return"
            },

            new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name ="SalesByBranch",
                Path = "cntrl.Reports.Sales.SalesByBranch.rdlc",
                Query = Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
              new Report
            {
                Application = entity.App.Names.SalesOrder,
                Name ="SalesOrderByBranch",
                Path = "cntrl.Reports.Sales.SalesByBranch.rdlc",
                Query = Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="sales_invoice",
                ReplaceWithString="sales_order"
            },
            new Report
            {
                Application = entity.App.Names.SalesBudget,
                Name ="SalesBudgetByBranch",
                Path = "cntrl.Reports.Sales.SalesByBranch.rdlc",
                Query = Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                 ReplaceString="sales_invoice",
                ReplaceWithString="sales_budget"

            },
               new Report
            {
                Application = entity.App.Names.SalesReturn,
                Name ="SalesReturnByBranch",
                Path = "cntrl.Reports.Sales.SalesByBranch.rdlc",
                Query=  Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="sales_invoice",
                ReplaceWithString="sales_return"
            },

                    new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name ="SalesBySalesRep",
                Path = "cntrl.Reports.Sales.SalesBySalesRep.rdlc",
                Query = Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
              new Report
            {
                Application = entity.App.Names.SalesOrder,
                Name ="SalesOrderBySalesRep",
                Path = "cntrl.Reports.Sales.SalesBySalesRep.rdlc",
                Query = Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="sales_invoice",
                ReplaceWithString="sales_order"
            },
            new Report
            {
                Application = entity.App.Names.SalesBudget,
                Name ="SalesBudgetBySalesRep",
                Path = "cntrl.Reports.Sales.SalesBySalesRep.rdlc",
                Query = Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                 ReplaceString="sales_invoice",
                ReplaceWithString="sales_budget"

            },
               new Report
            {
                Application = entity.App.Names.SalesReturn,
                Name ="SalesReturnBySalesRep",
                Path = "cntrl.Reports.Sales.SalesBySalesRep.rdlc",
                Query=  Reports.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="sales_invoice",
                ReplaceWithString="sales_return"
            },


               new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name ="SalesAnalysis",
                Path = "cntrl.Reports.Sales.SalesAnalysis.rdlc",
                Query = Reports.Sales.SalesAnalysis.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
            
            },


            //purchase
              new Report
            {
                Application = entity.App.Names.PurchaseInvoice,
                Name ="PurchaseDetail",
                Path = "cntrl.Reports.Purchases.PurchaseDetail.rdlc",
                Query = Reports.Purchase.Purchase.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
              new Report
            {
                Application = entity.App.Names.PurchaseOrder,
                Name ="PurchaseOrderDetail",
                Path = "cntrl.Reports.Purchases.PurchaseDetail.rdlc",
                Query = Reports.Purchase.Purchase.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="purchase_invoice",
                ReplaceWithString="purchase_order"
            },
                 new Report
            {
                Application = entity.App.Names.PurchaseInvoice,
                Name ="PurchaseBySupplier",
                Path = "cntrl.Reports.Purchases.PurchaseBySupplier.rdlc",
                Query = Reports.Purchase.Purchase.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
              new Report
            {
                Application = entity.App.Names.PurchaseOrder,
                Name ="PurchaseOrderBySupplier",
                Path = "cntrl.Reports.Purchases.PurchaseBySupplier.rdlc",
                Query = Reports.Purchase.Purchase.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="purchase_invoice",
                ReplaceWithString="purchase_order"
            },
                 new Report
            {
                Application = entity.App.Names.PurchaseInvoice,
                Name ="PurchaseByCostCenter",
                Path = "cntrl.Reports.Purchases.PurchaseByCostCenter.rdlc",
                Query = Reports.Purchase.Purchase.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
              new Report
            {
                Application = entity.App.Names.PurchaseOrder,
                Name ="PurchaseOrderByCostCenter",
                Path = "cntrl.Reports.Purchases.PurchaseByCostCenter.rdlc",
                Query = Reports.Purchase.Purchase.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="purchase_invoice",
                ReplaceWithString="purchase_order"
            },

              
           
          //stock
            new Report
            {
                Application = entity.App.Names.Stock,
                Name ="StockMovement",
                Path = "cntrl.Reports.Stocks.StockMovement.rdlc",
                Query = Reports.Stock.Stock.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
             
            },
              new Report
            {
                Application = entity.App.Names.Stock,
                Name ="StockFlow",
                Path = "cntrl.Reports.Stocks.StockFlow.rdlc",
                Query = Reports.Stock.Stock.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},

            },
            
             //projects and Production
                          new Report
            {
                Application = entity.App.Names.ActivityPlan,
                Name ="ActivityPlan",
                Path = "cntrl.Reports.Projects.Project.rdlc",
                Query =  Reports.Project.Project.query,
                Parameters = new List<Report.Types> { Report.Types.Project}
            },
               new Report
            {
                Application = entity.App.Names.ProjectExecution,
                Name ="ProjectExecution",
                Path = "cntrl.Reports.Projects.ProjectExecution.rdlc",
                Query = Reports.Project.Project.query,
                Parameters = new List<Report.Types> { Report.Types.Project }
            },  new Report
            {
                Application = entity.App.Names.ProjectFinance,
                Name ="ProjectFinance",
                Path = "cntrl.Reports.Projects.ProjectFinance.rdlc",
                Query =Reports.Project.Project.query,
                Parameters = new List<Report.Types> { Report.Types.Project }
            }, new Report
                {
                Application = entity.App.Names.TechnicalReport,
                Name ="TechnicalReport",
                Path = "cntrl.Reports.Projects.Technical.rdlc",
                Query = Reports.Project.Project.query,
                Parameters = new List<Report.Types> { Report.Types.Project }
            }
            , new Report
                {
                Application = entity.App.Names.ProductionOrder,
                Name ="ProductionOrder",
                Path = "cntrl.Reports.Productions.ProductionOrder.rdlc",
                Query = Reports.Production.Production.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate }
            }
            , new Report
                {
                Application = entity.App.Names.ProductionOrderStatus,
                Name ="ProductionOrderStatus",
                Path = "cntrl.Reports.Productions.ProductionStatus.rdlc",
                Query = Reports.Production.ProductionOrderStatus.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate }
            }, new Report
                {
                Application = entity.App.Names.EmployeesInProduction,
                Name ="EmployeesInProduction",
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
