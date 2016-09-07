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
    /// <summary>
    /// Interaction logic for SalesByTag.xaml
    /// </summary>
    public partial class CarnetContactReport : Page
    {
        public object ContactList { get; set; }

        public CarnetContactReport(object ContactLists)
        {
            InitializeComponent();
            ContactList = ContactLists;
            Fill(null, null);
        }

        public void Fill(object sender, EventArgs e)
        {
            MySqlConnection con = new MySqlConnection(Properties.Settings.Default.MySQLconnString);
            con.Open();


            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();

            reportDataSource1.Name = "Customer"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = ContactList;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.CarnetContact.rdlc";

            this.reportViewer.RefreshReport();
        }


    }
    public class ContactLists
    {

        public int id_contact { get; set; }
        public string contacts_name { get; set; }
        public string date_birth { get; set; }
        public string gove_code { get; set; }
        public string trans_date { get; set; }
        public string contacts_code { get; set; }
        public string Product_code { get; set; }
        public string name { get; set; }
    }
}
