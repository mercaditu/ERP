using System;
using Microsoft.Reporting.WinForms;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using entity;
using System.Data.Entity;
using entity.Brillo.Document;
using System.IO;

namespace Cognitivo.Reporting
{
    /// <summary>
    /// Interaction logic for PaymentPrint.xaml
    /// </summary>
    public partial class PaymentPrint : Page
    {
        CollectionViewSource payment_typeViewSource, paymentViewSource, payment_detailViewSource;
        db db = new db();
        public PaymentPrint()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            payment_detailViewSource = ((CollectionViewSource)(FindResource("payment_detailViewSource")));
            db.payment_type.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active && x.id_document > 0).Load();

            payment_typeViewSource = ((CollectionViewSource)(FindResource("payment_typeViewSource")));
            payment_typeViewSource.Source = db.payment_type.Local;
        }

        private void dgvPaymnetDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            payment_detail payment_detail = dgvPaymnetDetail.SelectedItem as payment_detail;
            if (payment_detail != null)
            {
                if (payment_detail.payment_type != null && payment_detail.payment_type.app_document != null)
                {
                    app_document app_document = payment_detail.payment_type.app_document;
                    string DocumentName = app_document.name;

                    string PathFull = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CogntivoERP\\TemplateFiles\\" + app_document.name + ".rdlc";


                    if (Directory.Exists(PathFull) == false)
                    {
                        Normal Normal = new Normal();
                        Normal.CreateFile(app_document);
                    }

                    DataSource DataSource = new DataSource();



                    reportViewer.LocalReport.ReportPath = PathFull; // Path of the rdlc file
                    reportViewer.LocalReport.DataSources.Add(DataSource.Create(payment_detail));
                    reportViewer.RefreshReport();


                }
            }




        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            foreach (payment_detail payment_detail in payment_detailViewSource.View.OfType<payment_detail>().ToList())
            {
                if (payment_detail.payment_type != null && payment_detail.payment_type.app_document != null)
                {
                    string PathFull = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CogntivoERP\\TemplateFiles\\" + payment_detail.payment_type.app_document.name + ".rdlc";
                    LocalReport LocalReport = new LocalReport();
                    DataSource DataSource = new DataSource();
                    PrintInvoice PrintInvoice = new PrintInvoice();
                    LocalReport.ReportPath = PathFull; // Path of the rdlc file
                    LocalReport.DataSources.Add(DataSource.Create(payment_detail));
                    PrintInvoice.Export(LocalReport);
                 //   PrintInvoice.Print(app_range.printer_name);
                }
            }

        }

        private void PaymentTypeDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            payment_type payment_type = payment_typeViewSource.View.CurrentItem as payment_type;
            if (payment_type != null)
            {
                db.payments.Where(x => x.id_company == CurrentSession.Id_Company && x.status == Status.Documents_General.Approved).Load();

                paymentViewSource = ((CollectionViewSource)(FindResource("paymentViewSource")));
                paymentViewSource.Source = db.payments.Local;
            }
        }
    }
}
