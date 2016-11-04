using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using entity;
using System.Linq;

namespace Cognitivo.Production
{
    public partial class Line : Page
    {
        LineDB LineDB = new LineDB();
        CollectionViewSource production_lineViewSource, app_locationViewSource;

        public Line()
        {
            InitializeComponent();
        }

        private void New_Click(object sender)
        {
            production_line production_line = new production_line();
            production_line.State = EntityState.Added;
            production_line.IsSelected = true;

            LineDB.production_line.Add(production_line);
            production_lineViewSource.View.MoveCurrentTo(production_line);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            production_lineViewSource = ((CollectionViewSource)(this.FindResource("production_lineViewSource")));
            await LineDB.production_line.Where(x => x.id_company == CurrentSession.Id_Company).LoadAsync();
            production_lineViewSource.Source = LineDB.production_line.Local;

            app_locationViewSource = this.FindResource("app_locationViewSource") as CollectionViewSource;
            await LineDB.app_location.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active).LoadAsync();
            app_locationViewSource.Source = LineDB.app_location.Local;
        }

   
        private void toolBar_btnCancel_Click(object sender)
        {
            LineDB.CancelAllChanges();
            production_lineViewSource.View.Refresh();
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (project_templateDataGrid.SelectedItem != null)
            {
                production_line production_line = (production_line)project_templateDataGrid.SelectedItem;
                production_line.IsSelected = true;
                production_line.State = EntityState.Modified;
                LineDB.Entry(production_line).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select a record");
            }
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    production_lineViewSource.View.Filter = i =>
                    {
                        production_line production_line = i as production_line;
                        if (production_line.name.ToLower().Contains(query.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    };
                }
                else
                {
                    production_lineViewSource.View.Filter = null;
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void Save_Click(object sender)
        {
            if (LineDB.SaveChanges() > 0)
            {
                production_lineViewSource.View.Refresh();
                toolBar.msgSaved(LineDB.NumberOfRecords);   
            }
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            if (MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                production_line production_line = production_lineViewSource.View.CurrentItem as production_line;
                production_line.is_head = false;
                production_line.State = EntityState.Deleted;
                production_line.IsSelected = true;
            }
        }
    }
}
