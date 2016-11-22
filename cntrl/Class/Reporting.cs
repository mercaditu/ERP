using entity.Brillo;
using System.Collections.Generic;
using System.Linq;

namespace cntrl.Class
{
    public class Generate
    {
        public List<Report> ReportList { get; set; }

        public void GenerateReportList()
        {
            ReportList = new List<Report> { new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name =Localize.Text<string>("SalesByDate"),
                Dataset = "DataSet1",
                Path = "cntrl.Reports.Reports.SalesInvoiceDetail.rdlc",
                Query = Reports.Queries.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
            new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name = Localize.Text<string>("SalesByCustomer"),
                Dataset = "SalesInvoiceSummary",
                Path = "cntrl.Reports.Reports.SalesInvoice.rdlc",
                Query = Reports.Queries.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
            new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name = Localize.Text<string>("SalesByProductsAndBranch"),
                Dataset = "SalesInvoiceSummary",
                Path = "cntrl.Reports.Reports.SalesInvoice.rdlc",
                Query =  Reports.Queries.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
            new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name = Localize.Text<string>("SalesByBranch"),
                Dataset = "SalesInvoiceSummary",
                Path = "cntrl.Reports.Reports.SalesInvoice.rdlc",
                Query = Reports.Queries.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
              new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name =Localize.Text<string>( "SalesBySalesRep"),
                Path = "cntrl.Reports.Reports.SalesInvoice.rdlc",
                Query = Reports.Queries.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
            new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name = Localize.Text<string>("SalesByGeography"),
                Dataset = "SalesInvoiceSummary",
                Path = "cntrl.Reports.Reports.SalesInvoice.rdlc",
                Query = Reports.Queries.Sales.Sales.query,
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
            new Report
            {
                Application = entity.App.Names.ActivityPlan,
                Name =Localize.Text<string>("ActivityPlan"),
                Dataset = "DataSet1",
                Path = "cntrl.Reports.Reports.Project.rdlc",
                Query =  Reports.Queries.Project.Project.query,
                Parameters = new List<Report.Types> { Report.Types.Project}
            },
               new Report
            {
                Application = entity.App.Names.ProjectExecution,
                Name =Localize.Text<string>("ProjectExecution"),
                Dataset = "DataSet1",
                Path = "cntrl.Reports.Reports.ProjectExecution.rdlc",
                Query = Reports.Queries.Project.Project.query,
                Parameters = new List<Report.Types> { Report.Types.Project }
            },  new Report
            {
                Application = entity.App.Names.ProjectFinance,
                Name =Localize.Text<string>("ProjectFinance"),
                Dataset = "DataSet1",
                Path = "cntrl.Reports.Reports.ProjectFinance.rdlc",
                Query =Reports.Queries.Project.Project.query,
                Parameters = new List<Report.Types> { Report.Types.Project }
            }, new Report
                {
                Application = entity.App.Names.TechnicalReport,
                Name =Localize.Text<string>("TechnicalReport"),
                Dataset = "DataSet1",
                Path = "cntrl.Reports.Reports.Technical.rdlc",
                Query = Reports.Queries.Project.Project.query,
                Parameters = new List<Report.Types> { Report.Types.Project }
            }
            , new Report
                {
                Application = entity.App.Names.ProductionOrder,
                Name =Localize.Text<string>("ProductionOrder"),
                Dataset = "DataSet1",
                Path = "cntrl.Reports.Reports.Production.rdlc",
                Query = Reports.Queries.Production.ProductionOrder.query,
                Parameters = new List<Report.Types> { Report.Types.Project }
            }
            , new Report
                {
                Application = entity.App.Names.ProductionOrderStatus,
                Name =Localize.Text<string>("ProductionOrderStatus"),
                Dataset = "DataSet1",
                Path = "cntrl.Reports.Reports.ProductionStatus.rdlc",
                Query = Reports.Queries.Production.ProductionOrderStatus.query,
                Parameters = new List<Report.Types> { Report.Types.Project }
            }, new Report
                {
                Application = entity.App.Names.EmployeesInProduction,
                Name =Localize.Text<string>("EmployeesInProduction"),
                Dataset = "DataSet1",
                Path = "cntrl.Reports.Reports.EmployeesInProduction.rdlc",
                Query =Reports.Queries.Production.EmployeesInProduction.query,
                Parameters = new List<Report.Types> { Report.Types.Project }
            }

            };


        }
    }



    public class Report
    {
        public enum Types { StartDate, EndDate,
            Project
        }
        public entity.App.Names Application { get; set; }
        public string Name { get; set; }
        public string Dataset { get; set; }
        public string Path { get; set; }
        public string Query { get; set; }
        public string ReplaceString { get; set; }
        public string ReplaceWithString { get; set; }

        public ICollection<Types> Parameters { get; set; }
    }
}
