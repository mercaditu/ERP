using entity;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace cntrl
{
    public partial class branch : UserControl
    {
        private CollectionViewSource _branchViewSource = null;
        public CollectionViewSource app_branchViewSource { get { return _branchViewSource; } set { _branchViewSource = value; } }

        private entity.dbContext _entity = null;
        public entity.dbContext entity { get { return _entity; } set { _entity = value; } }

        public branch()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                stackMain.DataContext = app_branchViewSource;

                CollectionViewSource app_vatViewSource = ((CollectionViewSource)(this.FindResource("app_vatViewSource")));
                app_vatViewSource.Source = entity.db.app_vat.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToList();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            entity.db.SaveChanges();
            CurrentSession.Load_BasicData(null, null);

            if (CurrentSession.Id_Branch == 0)
            {
                CurrentSession.Id_Branch = entity.db.app_branch.Where(x => x.id_company == CurrentSession.Id_Company).FirstOrDefault().id_branch;
            }

            btnCancel_Click(sender, e);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                entity.CancelChanges();
                app_branchViewSource.View.Refresh();
                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure want to delete ?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                app_branch objbranch = app_branchViewSource.View.CurrentItem as entity.app_branch;
                objbranch.is_active = false;
                btnSave_Click(sender, e);
            }
        }

        private void chkIsDefault_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckBox objIsCheck = sender as CheckBox;
                int isCheckCtr = 0;
                foreach (CheckBox checkBox in Class.clsCommon.FindVisualChildren<CheckBox>(app_locationDataGrid, "chkIsDefault"))
                {
                    if (checkBox.IsChecked == true)
                        isCheckCtr++;
                }
                if (isCheckCtr > 1)
                {
                    foreach (CheckBox checkBox in Class.clsCommon.FindVisualChildren<CheckBox>(app_locationDataGrid, "chkIsDefault"))
                    {
                        checkBox.IsChecked = false;
                    }
                    objIsCheck.IsChecked = true;
                }
            }
            catch (Exception)
            {
                //throw;
            }
        }
    }
}