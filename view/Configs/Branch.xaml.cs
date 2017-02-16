using entity;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Configs
{
    public partial class Branch : Page
    {
        private dbContext entity = new dbContext();
        private CollectionViewSource branchViewSource;

        public Branch()
        { InitializeComponent(); }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                branchViewSource = ((CollectionViewSource)(this.FindResource("app_branchViewSource")));
                entity.db.app_branch.Where(a => a.id_company == CurrentSession.Id_Company).OrderByDescending(a => a.is_active).Load();
                branchViewSource.Source = entity.db.app_branch.Local;
            }
            catch { }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.branch objBranch = new cntrl.branch();
            app_branch app_branch = new app_branch();
            app_branch.id_company = CurrentSession.Id_Company;
            entity.db.app_branch.Add(app_branch);
            branchViewSource.View.MoveCurrentToLast();
            objBranch.app_branchViewSource = branchViewSource;
            objBranch.entity = entity;
            crud_modal.Children.Add(objBranch);
        }

        private void pnl_Branch_Click(object sender, int idBranch)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.branch objBranch = new cntrl.branch();
            branchViewSource.View.MoveCurrentTo(entity.db.app_branch.Where(x => x.id_branch == idBranch).FirstOrDefault());
            objBranch.app_branchViewSource = branchViewSource;
            objBranch.entity = entity;
            crud_modal.Children.Add(objBranch);
        }
    }
}