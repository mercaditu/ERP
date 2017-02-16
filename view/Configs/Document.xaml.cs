using entity;
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace Cognitivo.Configs
{
    public partial class Document : Page
    {
        private DocumentDB dbcontext = new DocumentDB();
        private CollectionViewSource app_documentViewSource;

        public Document()
        {
            InitializeComponent();
        }

        private void toolBar_btnSave_Click(object sender)
        {
            if (dbcontext.SaveChanges() > 0)
            {
                app_documentViewSource.View.Refresh();
                toolBar.msgSaved(dbcontext.NumberOfRecords);
            }
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                dbcontext.app_document.Remove((app_document)app_documentDataGrid.SelectedItem);
                app_documentViewSource.View.MoveCurrentToFirst();
                toolBar_btnSave_Click(sender);
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            dbcontext.CancelAllChanges();
        }

        private void toolBar_btnNew_Click(object sender)
        {
            app_document app_document = new app_document();
            app_document.State = EntityState.Added;
            app_document.IsSelected = true;
            dbcontext.Entry(app_document).State = EntityState.Added;
            app_documentViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (app_documentDataGrid.SelectedItem != null)
            {
                app_document Document = (app_document)app_documentDataGrid.SelectedItem;
                Document.IsSelected = true;
                Document.State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select a Contact");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            app_documentViewSource = ((CollectionViewSource)(this.FindResource("app_documentViewSource")));
            dbcontext.app_document.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderByDescending(a => a.is_active).Load();
            app_documentViewSource.Source = dbcontext.app_document.Local;

            cbxApplication.ItemsSource = Enum.GetValues(typeof(entity.App.Names)).OfType<entity.App.Names>().ToList().OrderBy(x => x);

            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Documents\\"))
            {
                DirectoryInfo d = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "Documents\\");
                FileInfo[] Files = d.GetFiles("*.*");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            FlowDocument doc = new FlowDocument(new Paragraph(new Run(rtfheader.Text)));
            doc.Name = "FlowDoc";
            IDocumentPaginatorSource idpSource = doc;
            pd.PrintDocument(idpSource.DocumentPaginator, "Hello WPF Printing.");
        }

        private void cbxApplication_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxApplication.SelectedItem != null)
            {
                entity.App.Names module = (entity.App.Names)cbxApplication.SelectedItem;
                if (module == entity.App.Names.Movement)
                {
                    lstfield.Items.Add("<<item>>");
                    lstfield.Items.Add("<<quantity>>");
                }
            }
        }

        private void lstfield_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            rtfrow.Text += lstfield.SelectedItem.ToString();
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            if (!string.IsNullOrEmpty(query) && app_documentViewSource != null)
            {
                try
                {
                    app_documentViewSource.View.Filter = i =>
                    {
                        app_document app_document = i as app_document;
                        if (app_document != null)
                        {
                            string name = "";

                            if (app_document.name != null)
                            {
                                name = app_document.name.ToLower();
                            }

                            if (name.Contains(query.ToLower()))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    };
                }
                catch (Exception ex)
                {
                    toolBar.msgError(ex);
                }
            }
            else
            {
                app_documentViewSource.View.Filter = null;
            }
        }
    }
}