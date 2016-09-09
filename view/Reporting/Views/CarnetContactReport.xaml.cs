using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cognitivo.Reporting.Views
{
    public partial class CarnetContactReport : Page
    {
        public List<ContactInfo> ContactSubscription { get; set; }

        public CarnetContactReport(List<ContactInfo> _ContactSubscription)
        {
            InitializeComponent();
            ContactSubscription = _ContactSubscription;
            Fill(null, null);
        }

        public void Fill(object sender, EventArgs e)
        {
            MySqlConnection con = new MySqlConnection(Properties.Settings.Default.MySQLconnString);
            con.Open();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();

            reportDataSource1.Name = "Customer"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = ContactSubscription;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.CarnetContact.rdlc";

            this.reportViewer.RefreshReport();
        }
    }

    public class ContactInfo
    {
        public ContactInfo()
        {
            //Children = new List<ContactInfo>();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime DateBirth { get; set; }
        public string GovCode { get; set; }
        public DateTime StartDate { get; set; }
        public string Code { get; set; }
        public string SubscriptionItem { get; set; }
        public string Children { get; set; }
        //public List<ContactInfo> Children { get; set; }
    }
}
