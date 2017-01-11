using System;
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
    public partial class PaymentPrint : Page
    {
        CollectionViewSource payment_typeViewSource, payment_detailViewSource;
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

        private void dgvPaymnet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            payment_detail payment_detail = PaymnetDetailDataGrid.SelectedItem as payment_detail;
            if (payment_detail != null)
            {
                if (payment_detail.id_range > 0 && payment_detail.app_document_range != null)
                {
                    app_document app_document = payment_detail.app_document_range.app_document;
                    string DocumentName = app_document.name;

                    string PathFull = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CogntivoERP\\TemplateFiles\\" + app_document.name + ".rdlc";

                    if (Directory.Exists(PathFull) == false)
                    {
                        Normal Normal = new Normal();
                        Normal.CreateFile(app_document);
                    }

                    DataSource DataSource = new DataSource();

                    reportViewer.LocalReport.ReportPath = PathFull; // Path of the rdlc file
                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.DataSources.Add(DataSource.Create(payment_detail));
                    reportViewer.LocalReport.Refresh();
                }
            }
            reportViewer.RefreshReport();
        }

        private void PaymentTypeDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            payment_type payment_type = payment_typeViewSource.View.CurrentItem as payment_type;
            if (payment_type != null)
            {
                payment_detailViewSource.Source = db.payment_detail.Where(x => x.id_payment_type == payment_type.id_payment_type && x.is_read == Print.IsChecked).ToList();
                cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(db, payment_type.app_document.id_application, CurrentSession.Id_Branch, CurrentSession.Id_Terminal);
            }

        }


        private void Print_Checked(object sender, RoutedEventArgs e)
        {
            payment_type payment_type = payment_typeViewSource.View.CurrentItem as payment_type;
            if (payment_type != null)
            {
                payment_detailViewSource.Source = db.payment_detail.Where(x => x.id_payment_type == payment_type.id_payment_type && x.is_read == Print.IsChecked).ToList();
            }
        }

        private void cbxDocument_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            app_document_range app_document_range = cbxDocument.SelectedItem as app_document_range;
            if (app_document_range != null)
            {
                dgvPaymnet_SelectionChanged(sender, null);
            }
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            payment_detail payment_detail = PaymnetDetailDataGrid.SelectedItem as payment_detail;
            if (payment_detail.id_range > 0 && payment_detail.app_document_range != null)
            {
                if (payment_detail.payment != null && payment_detail.payment.id_branch > 0)
                {
                    entity.Brillo.Logic.Range.branch_Code = CurrentSession.Branches.Where(x => x.id_branch == payment_detail.payment.id_branch).Select(x => x.code).FirstOrDefault();
                }
                if (payment_detail.payment != null && payment_detail.payment.id_terminal > 0)
                {
                    entity.Brillo.Logic.Range.terminal_Code = CurrentSession.Terminals.Where(x => x.id_terminal == payment_detail.payment.id_terminal).Select(x => x.code).FirstOrDefault();
                }

                app_document_range app_document_range = db.app_document_range.Where(x => x.id_range == payment_detail.id_range).FirstOrDefault();
                payment_detail.payment_type_number = entity.Brillo.Logic.Range.calc_Range(app_document_range, true);
                payment_detail.RaisePropertyChanged("payment_type_number");
                payment_detail.is_read = true;
                Start.Automatic(payment_detail, app_document_range);

            }

            db.SaveChanges();
        }
    }
}
