using Microsoft.Reporting.WinForms;
using System;
using System.IO;
using System.Windows;

namespace entity.Brillo.Document
{
    public class Normal
    {
        Logic.Reciept TicketPrint = new Logic.Reciept();
        public enum PrintStyles
        {
            Automatic,
            Manual
        }

        public Normal(object Document, app_document_range app_range, PrintStyles PrintStyle)
        {
            if (app_range.app_document != null ? app_range.app_document.style_reciept : false || app_range.app_document != null ? app_range.app_document.id_application==App.Names.PointOfSale:false)
            {
                TicketPrint.Document_Print(app_range.app_document.id_document, Document);
            }
            else
            {
                string PathFull = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CogntivoERP\\TemplateFiles\\" + app_range.app_document.name + ".rdlc";

                if (Directory.Exists(PathFull) == false)
                {
                    CreateFile(app_range);
                }

                DataSource DataSource = new DataSource();

                ///
                if (PrintStyle == PrintStyles.Automatic && !app_range.use_default_printer && app_range.printer_name != null)
                {
                    LocalReport LocalReport = new LocalReport();
                    PrintInvoice PrintInvoice = new PrintInvoice();
                    LocalReport.ReportPath = PathFull; // Path of the rdlc file
                    LocalReport.DataSources.Add(DataSource.Create(Document));
                    PrintInvoice.Export(LocalReport);
                    PrintInvoice.Print(app_range.printer_name);
                }
                else
                {
                    DocumentViewr DocumentViewr = new DocumentViewr();
                    DocumentViewr.reportViewer.LocalReport.ReportPath = PathFull; // Path of the rdlc file
                    DocumentViewr.reportViewer.LocalReport.DataSources.Add(DataSource.Create(Document));
                    DocumentViewr.reportViewer.RefreshReport();

                    Window window = new Window
                    {
                        Title = "Report",
                        Content = DocumentViewr
                    };

                    window.ShowDialog();
                }
            }
        }

        private void CreateFile(app_document_range app_range)
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
            if (!File.Exists(path + SubFolder + "\\" + app_range.app_document.name + ".rdlc"))
            {
                //Add Logic
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory  + app_range.app_document.id_application.ToString() + ".rdlc"))
                {
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + app_range.app_document.id_application.ToString() + ".rdlc",
                           path + SubFolder + "\\" + app_range.app_document.name + ".rdlc");
                }
               
            }
        }
    
    }
}
