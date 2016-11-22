using entity.Brillo;
using System;
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
                Name = Localize.Text<string>("SalesDetail"),
                Path = "cntrl.Reports.Reports.SalesDetail.rdlc",
                Query = Reports.Queries.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
            new Report
            {
                Application = entity.App.Names.SalesOrder,
                Name = Localize.Text<string>("SalesDetail"),
                Path = "cntrl.Reports.Reports.SalesDetail.rdlc",
                Query =  Reports.Queries.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="sales_invoice",
                ReplaceWithString="sales_order"
            },
            new Report
            {
                Application = entity.App.Names.SalesBudget,
                Name = Localize.Text<string>("SalesDetail"),
                Path = "cntrl.Reports.Reports.SalesDetail.rdlc",
                Query=  Reports.Queries.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="sales_invoice",
                ReplaceWithString="sales_budget"
            },

            /// Sales (Invoice, Order, and Budget) ByCustomer Reports

            new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name = Localize.Text<string>("SalesByCustomer"),
                Path = "cntrl.Reports.Reports.SalesByCustomer.rdlc",
                Query = Reports.Queries.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },

            new Report
            {
                Application = entity.App.Names.SalesOrder,
                Name = Localize.Text<string>("SalesByCustomer"),
                Path = "cntrl.Reports.Reports.SalesByCustomer.rdlc",
                Query = Reports.Queries.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                 ReplaceString="sales_invoice",
                ReplaceWithString="sales_order"
            },

            new Report
            {
                Application = entity.App.Names.SalesBudget,
                Name = Localize.Text<string>("SalesByCustomer"),
                Path = "cntrl.Reports.Reports.SalesByCustomer.rdlc",
                Query = Reports.Queries.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                  ReplaceString="sales_invoice",
                ReplaceWithString="sales_budget"
            },

            new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name = Localize.Text<string>("SalesByBranch"),
                Path = "cntrl.Reports.Reports.SalesByBranch.rdlc",
                Query = Reports.Queries.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
              new Report
            {
                Application = entity.App.Names.SalesOrder,
                Name =Localize.Text<string>( "SalesByBranch"),
                Path = "cntrl.Reports.Reports.SalesByBranch.rdlc",
                Query = Reports.Queries.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="sales_invoice",
                ReplaceWithString="sales_order"
            },
            new Report
            {
                Application = entity.App.Names.SalesBudget,
                Name = Localize.Text<string>("SalesByBranch"),
                Path = "cntrl.Reports.Reports.SalesByBranch.rdlc",
                Query = Reports.Queries.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                 ReplaceString="sales_invoice",
                ReplaceWithString="sales_budget"

            },



            new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name = Localize.Text<string>("SalesByCustomer"),
                Path = "cntrl.Reports.Reports.SalesByCustomer.rdlc",
                Query = Reports.Queries.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
              new Report
            {
                Application = entity.App.Names.SalesOrder,
                Name =Localize.Text<string>( "SalesByCustomer"),
                Path = "cntrl.Reports.Reports.SalesByCustomer.rdlc",
                Query = Reports.Queries.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="sales_invoice",
                ReplaceWithString="sales_order"
            },
            new Report
            {
                Application = entity.App.Names.SalesBudget,
                Name = Localize.Text<string>("SalesByCustomer"),
                Path = "cntrl.Reports.Reports.SalesByCustomer.rdlc",
                Query = Reports.Queries.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                 ReplaceString="sales_invoice",
                ReplaceWithString="sales_budget"

            },
               new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name = Localize.Text<string>("SalesAnalysis"),
                Path = "cntrl.Reports.Reports.SalesAnalysis.rdlc",
                Query = Reports.Queries.Stock.SalesAnalysis.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
            
            },


            //purchase
              new Report
            {
                Application = entity.App.Names.PurchaseInvoice,
                Name = Localize.Text<string>("PurchaseDetail"),
                Path = "cntrl.Reports.Reports.PurchaseDetail.rdlc",
                Query = Reports.Queries.Purchase.Purchase.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
              new Report
            {
                Application = entity.App.Names.PurchaseOrder,
                Name = Localize.Text<string>("PurchaseDetail"),
                Path = "cntrl.Reports.Reports.PurchaseDetail.rdlc",
                Query = Reports.Queries.Purchase.Purchase.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="purchase_invoice",
                ReplaceWithString="purchase_order"
            },
                 new Report
            {
                Application = entity.App.Names.PurchaseInvoice,
                Name = Localize.Text<string>("PurchaseBySupplier"),
                Path = "cntrl.Reports.Reports.PurchaseBySupplier.rdlc",
                Query = Reports.Queries.Purchase.Purchase.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
              new Report
            {
                Application = entity.App.Names.PurchaseOrder,
                Name = Localize.Text<string>("PurchaseBySupplier"),
                Path = "cntrl.Reports.Reports.PurchaseBySupplier.rdlc",
                Query = Reports.Queries.Purchase.Purchase.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="purchase_invoice",
                ReplaceWithString="purchase_order"
            },
                 new Report
            {
                Application = entity.App.Names.PurchaseInvoice,
                Name = Localize.Text<string>("PurchaseByCostCenter"),
                Path = "cntrl.Reports.Reports.PurchaseByCostCenter.rdlc",
                Query = Reports.Queries.Purchase.Purchase.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
              new Report
            {
                Application = entity.App.Names.PurchaseOrder,
                Name = Localize.Text<string>("PurchaseByCostCenter"),
                Path = "cntrl.Reports.Reports.PurchaseByCostCenter.rdlc",
                Query = Reports.Queries.Purchase.Purchase.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate},
                ReplaceString="purchase_invoice",
                ReplaceWithString="purchase_order"
            },

              
           
          
            
             //projects and Production
                          new Report
            {
                Application = entity.App.Names.ActivityPlan,
                Name =Localize.Text<string>("ActivityPlan"),
                Path = "cntrl.Reports.Reports.Project.rdlc",
                Query =  Reports.Queries.Project.Project.query,
                Parameters = new List<Report.Types> { Report.Types.Project}
            },
               new Report
            {
                Application = entity.App.Names.ProjectExecution,
                Name =Localize.Text<string>("ProjectExecution"),
                Path = "cntrl.Reports.Reports.ProjectExecution.rdlc",
                Query = Reports.Queries.Project.Project.query,
                Parameters = new List<Report.Types> { Report.Types.Project }
            },  new Report
            {
                Application = entity.App.Names.ProjectFinance,
                Name =Localize.Text<string>("ProjectFinance"),
                Path = "cntrl.Reports.Reports.ProjectFinance.rdlc",
                Query =Reports.Queries.Project.Project.query,
                Parameters = new List<Report.Types> { Report.Types.Project }
            }, new Report
                {
                Application = entity.App.Names.TechnicalReport,
                Name =Localize.Text<string>("TechnicalReport"),
                Path = "cntrl.Reports.Reports.Technical.rdlc",
                Query = Reports.Queries.Project.Project.query,
                Parameters = new List<Report.Types> { Report.Types.Project }
            }
            , new Report
                {
                Application = entity.App.Names.ProductionOrder,
                Name =Localize.Text<string>("ProductionOrder"),
                Path = "cntrl.Reports.Reports.ProductionOrder.rdlc",
                Query = Reports.Queries.Production.Production.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate }
            }
            , new Report
                {
                Application = entity.App.Names.ProductionOrderStatus,
                Name =Localize.Text<string>("ProductionOrderStatus"),
                Path = "cntrl.Reports.Reports.ProductionStatus.rdlc",
                Query = Reports.Queries.Production.ProductionOrderStatus.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate }
            }, new Report
                {
                Application = entity.App.Names.EmployeesInProduction,
                Name =Localize.Text<string>("EmployeesInProduction"),
                Path = "cntrl.Reports.Reports.EmployeesInProduction.rdlc",
                Query =Reports.Queries.Production.EmployeesInProduction.query,
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
