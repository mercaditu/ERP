using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
            string PathFull = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CogntivoERP\\TemplateFiles\\CarnetContact.rdlc";
            createFile();
            MySqlConnection con = new MySqlConnection(Properties.Settings.Default.MySQLconnString);
            con.Open();

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();

            reportDataSource1.Name = "Customer"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = ContactSubscription;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportPath = PathFull;

            this.reportViewer.RefreshReport();
        }
        public void createFile()
        {

            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CogntivoERP";

            //If path (CognitivoERP) does not exist, create path.
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            string SubFolder = "\\TemplateFiles";

            //If path (TemplateFiles) does not exist, create path
            if (!Directory.Exists(path + SubFolder))
            {
                Directory.CreateDirectory(path + SubFolder);
            }

            //If file does not exist, create file.
            if (!File.Exists(path + SubFolder + "\\CarnetContact.rdlc"))
            {
                //Add Logic
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "CarnetContact.rdlc"))
                {
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "CarnetContact.rdlc",
                           path + SubFolder + "\\" + "CarnetContact.rdlc");
                }
            }
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
