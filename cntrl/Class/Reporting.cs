using System.Collections.Generic;
using System.Linq;

namespace cntrl.Class
{
    public class Generate
    {
        public List<Report> ReportList { get; set; }

        public void GenerateReportList()
        {
            ReportList = new List<Report>();
            Report Report = new Class.Report
            {
                Application=entity.App.Names.SalesInvoice,
                Name="Test",
                Description="Test",
                Path="Cognitivo.View.Reporting.SalesInvoiceSummary.rdlc",
                QueryPath="Reports/queries/SalesInvoice.sql",




            };
            Report_Parameter Report_ParameterStartDate = new Class.Report_Parameter
            {
                Type=Report_Parameter.Types.StartDate
            };
            Report_Parameter Report_ParameterEndDate = new Class.Report_Parameter
            {
                Type = Report_Parameter.Types.EndDate
            };
            Report.Parameters.Add(Report_ParameterStartDate);
            Report.Parameters.Add(Report_ParameterEndDate);
            ReportList.Add(Report);

        }
    }

    public class Report
    {
       public Report()
        {
            Parameters = new List<Class.Report_Parameter>();
        }
        public entity.App.Names Application { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public string QueryPath { get; set; }

       public List<Report_Parameter> Parameters { get; set; }
    }

    public class Report_Parameter
    {
        public enum Types { StartDate, EndDate }
        public Types Type { get; set; }
    }
}
