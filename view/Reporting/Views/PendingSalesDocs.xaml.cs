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
    public partial class PendingSalesDocs : Page
    {
        public DateTime EndDate
        {
            get { return _EndDate; }
            set { _EndDate = value; }
        }
        private DateTime _EndDate = DateTime.Now.AddDays(+1);

        public PendingSalesDocs()
        {
            InitializeComponent();
            Fill(null, null);
        }

        //
        public void Fill(object sender, EventArgs e)
        {
            this.reportViewer.Reset();

            try
            {
                MySqlConnection con = new MySqlConnection(Properties.Settings.Default.MySQLconnString);
                con.Open();

                string query = "call pending_sales_receipts('" + EndDate.AddDays(1).ToString("s") + "')";
                MySqlDataAdapter adpt = new MySqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                adpt.Fill(dt);

                Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();

                reportDataSource1.Name = "PendingSalesDocs"; //Name of the report dataset in our .RDLC file
                reportDataSource1.Value = dt;
                this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
                this.reportViewer.LocalReport.ReportEmbeddedResource = "Cognitivo.Reporting.Reports.PendingSalesDocs.rdlc";

                this.reportViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please create Procedure: " + ex.Message);
                //CreateStoredProcedure();
            }
        }

//        private void CreateStoredProcedure()
//        {

//            string StoredProc =
//            @"CREATE DEFINER=`root`@`%` PROCEDURE `pending_sales_receipts`(in enddate datetime)
//                begin
//                select c.code,
//	                c.name,
//	                c.alias,
//	                c.address,
//	                c.telephone,
//	                s.number,
//	                s.trans_date,
//	                paymentschedual.detalle,
//	                paymentschedual.monto,
//	                paymentschedual.monto - paymentschedual.credit as pendiente
//                from 
//	                sales_invoice as s
//                    left join
//	                 (select 	
//		                ps.trans_date,
//		                ps.id_sales_invoice,
//		                ps.id_sales_return,
//		                case when ps.id_sales_invoice is not null then 'Venta'
//			                 when ps.id_sales_return is not null then 'Nota de credito'
//		                END as detalle,
//		                case when ps.id_sales_invoice is not null then sum(ps.debit)
//			                 when ps.id_sales_return is not null then sum(credit.credit)
//		                END as monto,
//		                sum(ifnull(credit.credit,0)) as credit
//	                 from
//		                (select 
//			                trans_date,
//			                id_payment_schedual, 
//			                id_sales_invoice,
//			                id_sales_return, 
//			                debit from payment_schedual
//                            where trans_date<=enddate) as ps
//		                left join
//		                (select parent_id_payment_schedual,
//				                sum(credit) as credit 
//			                from payment_schedual 
//                            where trans_date<=enddate 
//                            group by 
//                            parent_id_payment_schedual) as credit
//		                on ps.id_payment_schedual = credit.parent_id_payment_Schedual
//		                group by ps.id_sales_invoice)
//	                as paymentschedual
//                    on paymentschedual.id_sales_invoice = s.id_sales_invoice
//                    left join contacts as c
//	                on c.id_contact = s.id_contact
//                    where s.trans_date <= enddate and s.status = 2
//                    having pendiente > 0
//                    order by c.name,paymentschedual.trans_date,s.number;
//                end";



//            Fill(null, null);
//        }
    }
}
