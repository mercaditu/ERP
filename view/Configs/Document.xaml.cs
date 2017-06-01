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
        private DocumentDB DocumentDB = new DocumentDB();
        private CollectionViewSource app_documentViewSource;

        public Document()
        {
            InitializeComponent();
        }

        private void toolBar_btnSave_Click(object sender)
        {
            if (DocumentDB.SaveChanges() > 0)
            {
                app_documentViewSource.View.Refresh();
                toolBar.msgSaved(DocumentDB.NumberOfRecords);
            }
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            if (MessageBox.Show(entity.Brillo.Localize.Question_Delete, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                foreach (app_document Document in DocumentDB.app_document.Local.Where(x => x.IsSelected))
                {
                    Document.is_active = false;
                    Document.State = EntityState.Modified;
                }

                DocumentDB.SaveChangesAsync();
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            DocumentDB.CancelAllChanges();
        }

        private void toolBar_btnNew_Click(object sender)
        {
            app_document app_document = new app_document()
            {
                State = EntityState.Added,
                IsSelected = true
            };

            DocumentDB.Entry(app_document).State = EntityState.Added;
            app_documentViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (app_documentDataGrid.SelectedItem != null)
            {
                app_document Document = (app_document)app_documentDataGrid.SelectedItem;
                Document.IsSelected = true;
                Document.State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            app_documentViewSource = FindResource("app_documentViewSource") as CollectionViewSource;
            await DocumentDB.app_document.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).LoadAsync();
            app_documentViewSource.Source = DocumentDB.app_document.Local;

            cbxApplication.ItemsSource = Enum.GetValues(typeof(entity.App.Names)).OfType<entity.App.Names>().ToList().OrderBy(x => x);

            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Documents\\"))
            {
                DirectoryInfo d = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "Documents\\");
                FileInfo[] Files = d.GetFiles("*.*");
            }
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            if (!string.IsNullOrEmpty(query) && app_documentViewSource != null)
            {
                app_documentViewSource.View.Filter = i =>
                {
                    if (i is app_document app_document)
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
                    }

                    return false;
                };
            }
            else
            {
                app_documentViewSource.View.Filter = null;
            }
        }
    }
}