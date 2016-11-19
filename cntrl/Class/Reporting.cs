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
                Dataset = "SalesInvoiceSummary",
                Path = "cntrl.Reports.Reports.SalesInvoice.rdlc",
                QueryPath = "Reports/Queries/Sales/Sales.sql",
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
            new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name = Localize.Text<string>("SalesByCustomer"),
                Dataset = "SalesInvoiceSummary",
                Path = "cntrl.Reports.Reports.SalesInvoice.rdlc",
                QueryPath = "Reports/Queries/Sales/Sales.sql",
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
            new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name = Localize.Text<string>("SalesByProductsAndBranch"),
                Dataset = "SalesInvoiceSummary",
                Path = "cntrl.Reports.Reports.SalesInvoice.rdlc",
                QueryPath = "Reports/Queries/Sales/Sales.sql",
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
            new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name = Localize.Text<string>("SalesByBranch"),
                Dataset = "SalesInvoiceSummary",
                Path = "cntrl.Reports.Reports.SalesInvoice.rdlc",
                QueryPath = "Reports/Queries/Sales/Sales.sql",
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
              new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name =Localize.Text<string>( "SalesBySalesRep"),
                Path = "cntrl.Reports.Reports.SalesInvoice.rdlc",
                QueryPath = "Reports/Queries/Sales/Sales.sql",
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            },
            new Report
            {
                Application = entity.App.Names.SalesInvoice,
                Name = Localize.Text<string>("SalesByGeography"),
                Dataset = "SalesInvoiceSummary",
                Path = "cntrl.Reports.Reports.SalesInvoice.rdlc",
                QueryPath = "Reports/Queries/Sales/Sales.sql",
                Parameters = new List<Report.Types> { Report.Types.StartDate, Report.Types.EndDate}
            }
            };


        }
    }



    public class Report
    {
        public enum Types { StartDate, EndDate }
        public entity.App.Names Application { get; set; }
        public string Name { get; set; }
        public string Dataset { get; set; }
        public string Path { get; set; }
        public string QueryPath { get; set; }
        public string ReplaceString { get; set; }
        public string ReplaceWithString { get; set; }

        public ICollection<Types> Parameters { get; set; }
    }
}
