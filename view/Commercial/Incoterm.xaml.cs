using entity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Commercial
{
    /// <summary>
    /// Interaction logic for Incoterm.xaml
    /// </summary>
    public partial class Incoterm : Page
    {
        //entity.dbContext entity = new entity.dbContext();
        private IncotermDB IncotermDB = new IncotermDB();

        private CollectionViewSource impex_incotermViewSource = null;

        public Incoterm()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, EventArgs e)
        {
            impex_incotermViewSource = this.FindResource("impex_incotermViewSource") as CollectionViewSource;
            await IncotermDB.impex_incoterm.Where(a => a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).LoadAsync();
            impex_incotermViewSource.Source = IncotermDB.impex_incoterm.Local;

            CollectionViewSource incoterm_conditionViewSource = this.FindResource("incoterm_conditionViewSource") as CollectionViewSource;
            incoterm_conditionViewSource.Source = await IncotermDB.impex_incoterm_condition.Where(a => a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToListAsync();
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            IncotermDB.CancelAllChanges();
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            try
            {
                if (MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.Yes)
                {
                    IncotermDB.impex_incoterm.Remove((impex_incoterm)impex_incotermDataGrid.SelectedItem);
                    toolBar_btnSave_Click(sender);
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void toolBar_btnNew_Click(object sender)
        {
            impex_incoterm impex_incoterm = new impex_incoterm();
            impex_incoterm.State = EntityState.Added;
            impex_incoterm.IsSelected = true;
            IncotermDB.Entry(impex_incoterm).State = EntityState.Added;

            impex_incotermViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnSave_Click(object sender)
        {
            if (IncotermDB.SaveChanges() > 0)
            {
                impex_incotermViewSource.View.Refresh();
                toolBar.msgSaved(IncotermDB.NumberOfRecords);
            }
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (impex_incotermDataGrid.SelectedItem != null)
            {
                impex_incoterm impex_incoterm = (impex_incoterm)impex_incotermDataGrid.SelectedItem;
                impex_incoterm.IsSelected = true;
                impex_incoterm.State = EntityState.Modified;
                IncotermDB.Entry(impex_incoterm).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select a Record");
            }
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    impex_incotermViewSource.View.Filter = i =>
                    {
                        impex_incoterm impex_incoterm = i as impex_incoterm;
                        if (impex_incoterm.name.ToLower().Contains(query.ToLower()))
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
                    impex_incotermViewSource.View.Filter = null;
                }
            }
            catch (Exception)
            {
            }
        }
    }
}