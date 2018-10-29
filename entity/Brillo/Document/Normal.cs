using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace entity.Brillo.Document
{
    public class Normal
    {
        private Logic.Reciept TicketPrint = new Logic.Reciept();

        public enum PrintStyles
        {
            Automatic,
            Manual
        }

        public Normal()
        { }

        public Normal(object Document, app_document_range app_range, PrintStyles PrintStyle)
        {
            if (app_range.app_document != null ? app_range.app_document.style_reciept : false || app_range.app_document != null ? app_range.app_document.id_application == App.Names.PointOfSale : false)
            {
                TicketPrint.Document_Print(app_range.id_range, Document);
            }
            else
            {
                string DocumentName = string.Empty;
                if (app_range != null)
                {
                    DocumentName = app_range.app_document.name;
                }
                else
                {
                    //Purchase invoice does not have Range. So we default to this if not existant.
                    DocumentName = "PurchaseInvoice";
                }

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
                    DocumentViewer DocumentViewer = new DocumentViewer();

                    DocumentViewer.reportViewer.LocalReport.ReportPath = PathFull; // Path of the rdlc file
                    DocumentViewer.reportViewer.LocalReport.DataSources.Add(DataSource.Create(Document));
                    DocumentViewer.reportViewer.RefreshReport();

                    Window window = new Window
                    {
                        Title = "Report",
                        Content = DocumentViewer
                    };

                    window.ShowDialog();
                }
            }
        }

        public Normal(object Document, string DocumentName)
        {
            string PathFull = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CogntivoERP\\TemplateFiles\\" + DocumentName + ".rdlc";

            if (Directory.Exists(PathFull) == false)
            {
                CreateFile(DocumentName);
            }

            DataSource DataSource = new DataSource();

            DocumentViewer DocumentViewer = new DocumentViewer();
            DocumentViewer.reportViewer.LocalReport.ReportPath = PathFull; // Path of the rdlc file
            string BaseName = Document.GetType().BaseType.ToString();
            string AppName = Document.GetType().ToString();

            if (AppName == typeof(impex).ToString() || BaseName == typeof(impex).ToString())
            {
                DocumentViewer.reportViewer.LocalReport.DataSources.Add(DataSource.Impex((impex)Document).ElementAt(0));
                DocumentViewer.reportViewer.LocalReport.DataSources.Add(DataSource.Impex((impex)Document).ElementAt(1));
            }
            else if (AppName == typeof(project).ToString() || BaseName == typeof(project).ToString())
            {
                DocumentViewer.reportViewer.LocalReport.DataSources.Add(DataSource.TechnicalProject((project)Document));
            }
            else if (AppName == typeof(production_order).ToString() || BaseName == typeof(production_order).ToString())
            {
                DocumentViewer.reportViewer.LocalReport.DataSources.Add(DataSource.TechnicalProduction((production_order)Document));
            }
            else if (AppName == typeof(contact).ToString() || BaseName == typeof(contact).ToString())
            {
                DocumentViewer.reportViewer.LocalReport.DataSources.Add(DataSource.Contact((contact)Document));
            }
            else
            {
                ReportParameter Parameters = new ReportParameter()
                {
                    Name = "Parameters"
                };
                item_movement item_movement = (item_movement)Document;
                BarcodeLib.Barcode b = new BarcodeLib.Barcode();
                try
                {
                    Image img = b.Encode(BarcodeLib.TYPE.CODE128, item_movement.barcode, 250, 100);
                    using (var ms = new MemoryStream())
                    {
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        ms.Position = 0;

                        var bi = new BitmapImage();
                        bi.BeginInit();
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.StreamSource = ms;
                        bi.EndInit();
                        Parameters.Values.Add(Convert.ToBase64String(ms.ToArray()));

                    }
                }
                catch
                {
                    MessageBox.Show("please Update Barcode From Startup Window...");
                }
              

              

                try
                {
                    DocumentViewer.reportViewer.LocalReport.SetParameters(new ReportParameter[] { Parameters });
                }
                catch
                {
                    throw;
                }
             
                DocumentViewer.reportViewer.LocalReport.DataSources.Add(DataSource.ItemMovementLabel((item_movement)Document));
            }
            DocumentViewer.reportViewer.RefreshReport();

            Window window = new Window
            {
                Title = "Report",
                Content = DocumentViewer
            };

            window.ShowDialog();
        }

        public Normal(object Document, string DocumentName,app_location app_Location)
        {
            string PathFull = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CogntivoERP\\TemplateFiles\\" + DocumentName + ".rdlc";

            if (Directory.Exists(PathFull) == false)
            {
                CreateFile(DocumentName);
            }

            DataSource DataSource = new DataSource();

            DocumentViewer DocumentViewer = new DocumentViewer();
            DocumentViewer.reportViewer.LocalReport.ReportPath = PathFull; // Path of the rdlc file
            string BaseName = Document.GetType().BaseType.ToString();
            string AppName = Document.GetType().ToString();

            if (AppName == typeof(item_inventory).ToString() || BaseName == typeof(item_inventory).ToString())
            {
                DocumentViewer.reportViewer.LocalReport.DataSources.Add(DataSource.ItemInventory(Document, app_Location));
              
            }
            DocumentViewer.reportViewer.RefreshReport();

            Window window = new Window
            {
                Title = "Report",
                Content = DocumentViewer
            };

            window.ShowDialog();
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
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\" + app_range.app_document.id_application.ToString() + ".rdlc"))
                {
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\" + app_range.app_document.id_application.ToString() + ".rdlc",
                           path + SubFolder + "\\" + app_range.app_document.name + ".rdlc");
                }
            }
        }

        public void CreateFile(app_document app_document)
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
            if (!File.Exists(path + SubFolder + "\\" + app_document.name + ".rdlc"))
            {
                //Add Logic
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\" + app_document.id_application.ToString() + ".rdlc"))
                {
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\" + app_document.id_application.ToString() + ".rdlc",
                           path + SubFolder + "\\" + app_document.name + ".rdlc");
                }
            }
        }

        public void CreateFile(string app_document)
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
            if (!File.Exists(path + SubFolder + "\\" + app_document + ".rdlc"))
            {
                //Add Logic
                
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\" + app_document + ".rdlc"))
                {
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\" + app_document + ".rdlc",
                           path + SubFolder + "\\" + app_document + ".rdlc");
                }
            }
        }

        public void loadCarnetcontactReport(contact contact)
        {
            try
            {
                ReportDataSource reportDataSource = new ReportDataSource();
                reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc

                if (contact.child.Count > 0)
                {
                    List<contact> contact_detail = contact.child.ToList();
                    reportDataSource.Value = contact_detail
                       .Select(g => new
                       {
                           id_contact = g.id_contact,
                           contacts_name = g.parent.name,
                           date_birth = g.parent.date_birth,
                           gove_code = g.parent.gov_code,
                           trans_date = g.parent.timestamp,
                           contacts_code = g.parent.code,
                           Product_code = g.parent.contact_subscription.FirstOrDefault().item.name,
                           name = g.name
                       }).ToList();
                }
                else
                {
                    List<contact> contact_detail = new List<contact>();
                    contact_detail.Add(contact);
                    reportDataSource.Value = contact_detail
                       .Select(g => new
                       {
                           id_contact = g.id_contact,
                           contacts_name = g.name,
                           date_birth = g.date_birth,
                           gove_code = g.gov_code,
                           trans_date = g.timestamp,
                           contacts_code = g.code,
                           Product_code = g.contact_subscription.FirstOrDefault() != null ? g.contact_subscription.FirstOrDefault().item != null ? g.contact_subscription.FirstOrDefault().item.name : "" : "",
                           name = ""
                       }).ToList();
                }

                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                path = path + "\\CogntivoERP";
                string SubFolder = "";
                SubFolder = "\\TemplateFiles";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Directory.CreateDirectory(path + SubFolder);
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Carnet_Contact.rdlc", path + SubFolder + "\\Carnet_Contact.rdlc");
                }
                else if (!Directory.Exists(path + SubFolder))
                {
                    Directory.CreateDirectory(path + SubFolder);
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Carnet_Contact.rdlc", path + SubFolder + "\\Carnet_Contact.rdlc");
                }
                else if (!File.Exists(path + SubFolder + "\\Carnet_Contact.rdlc"))
                {
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Carnet_Contact.rdlc", path + SubFolder + "\\Carnet_Contact.rdlc");
                }

                DocumentViewer DocumentViewr = new DocumentViewer();
                DocumentViewr.reportViewer.LocalReport.ReportPath = path + SubFolder + "\\Carnet_Contact.rdlc"; // Path of the rdlc file
                DocumentViewr.reportViewer.LocalReport.DataSources.Add(reportDataSource);
                DocumentViewr.reportViewer.RefreshReport();

                Window window = new Window
                {
                    Title = "Report",
                    Content = DocumentViewr
                };

                window.ShowDialog();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}