using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace entity
{
    public partial class DocumentViewr : UserControl
    {
        public DocumentViewr()
        {
            InitializeComponent();
        }

        public void loadCarnetcontactReport(int id)
        {
            try
            {
                ReportDataSource reportDataSource = new ReportDataSource();
                reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc
                contact contact = db.contacts.Where(x => x.id_contact == id).FirstOrDefault();
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
                    List<contact> contact_detail = new List<entity.contact>();
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

                reportViewer.LocalReport.ReportPath = path + SubFolder + "\\Carnet_Contact.rdlc"; // Path of the rdlc file
                reportViewer.LocalReport.DataSources.Add(reportDataSource);
                reportViewer.RefreshReport();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void loadCarnetcontactReportall()
        {
            try
            {
                ReportDataSource reportDataSource = new ReportDataSource();
                reportDataSource.Name = "DataSet1"; // Name of the DataSet we set in .rdlc


                List<contact> contact_detail = db.contacts.ToList();
                reportDataSource.Value = contact_detail
                   .Select(g => new
                   {
                       id_contact = g.id_contact,
                       contacts_name = g.name,
                       date_birth = g.date_birth!=null?g.date_birth:DateTime.Now,
                       gove_code = g.gov_code!=null?g.gov_code:"",
                       trans_date = g.timestamp != null ? g.timestamp : DateTime.Now,
                       contacts_code = g.code!= null ?g.code:"",
                       Product_code = "",
                       name = g.name
                   }).ToList();





                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                path = path + "\\CogntivoERP";
                string SubFolder = "";
                SubFolder = "\\TemplateFiles";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Directory.CreateDirectory(path + SubFolder);
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Carnet_Contact_ALL.rdlc", path + SubFolder + "\\Carnet_Contact_ALL.rdlc");
                }
                else if (!Directory.Exists(path + SubFolder))
                {
                    Directory.CreateDirectory(path + SubFolder);
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Carnet_Contact_ALL.rdlc", path + SubFolder + "\\Carnet_Contact_ALL.rdlc");

                }
                else if (!File.Exists(path + SubFolder + "\\Carnet_Contact.rdlc"))
                {
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\debug\\Carnet_Contact_ALL.rdlc", path + SubFolder + "\\Carnet_Contact_ALL.rdlc");
                }

                reportViewer.LocalReport.ReportPath = path + SubFolder + "\\Carnet_Contact_ALL.rdlc"; // Path of the rdlc file
                reportViewer.LocalReport.DataSources.Add(reportDataSource);
                reportViewer.RefreshReport();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}


