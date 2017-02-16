using entity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Product
{
    /// <summary>
    /// Interaction logic for Project.xaml
    /// </summary>
    public partial class BranchWalkin : Page
    {
        private BranchWalkinsDB BranchWalkinsDB = new BranchWalkinsDB();
        private int company_ID;
        private CollectionViewSource app_branch_walkinsViewSource;

        public BranchWalkin()
        {
            InitializeComponent();
            company_ID = CurrentSession.Id_Company;
        }

        private void btnNew_Click(object sender)
        {
            entity.app_branch_walkins app_branch_walkins = new entity.app_branch_walkins();
            app_branch_walkins.IsSelected = true;
            app_branch_walkins.State = EntityState.Added;
            BranchWalkinsDB.app_branch_walkins.Add(app_branch_walkins);
            app_branch_walkinsViewSource.View.MoveCurrentToLast();
        }

        private void btnDelete_Click(object sender)
        {
            try
            {
                if (MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    app_branch_walkins app_branch_walkins = (app_branch_walkins)BranchWalkinsDataGrid.SelectedItem;
                    app_branch_walkins.is_head = false;
                    app_branch_walkins.State = EntityState.Deleted;
                    app_branch_walkins.IsSelected = true;
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void btnSave_Click(object sender)
        {
            if (BranchWalkinsDB.SaveChanges() > 0)
            {
                app_branch_walkinsViewSource.View.Refresh();
            }
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (BranchWalkinsDataGrid.SelectedItem != null)
            {
                app_branch_walkins app_branch_walkins = (app_branch_walkins)BranchWalkinsDataGrid.SelectedItem;
                app_branch_walkins.IsSelected = true;
                app_branch_walkins.State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select an Item");
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            BranchWalkinsDB.CancelAllChanges();
        }

        private void Project_Loaded(object sender, RoutedEventArgs e)
        {
            BranchWalkinsDB.app_branch_walkins.Where(a => a.id_company == company_ID
                                            ).ToList();

            app_branch_walkinsViewSource = ((CollectionViewSource)(FindResource("app_branch_walkinsViewSource")));
            app_branch_walkinsViewSource.Source = BranchWalkinsDB.app_branch_walkins.Local;

            //cbxBranch.ItemsSource = BranchWalkinsDB.app_branch.Where(b => b.id_company == CurrentSession.Id_Company && b.is_active).OrderBy(b => b.name).ToList();
        }
    }
}