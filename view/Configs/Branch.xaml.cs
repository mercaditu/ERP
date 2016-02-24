using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;
using entity;
using cntrl;

namespace Cognitivo.Configs
{
    public partial class Branch : Page
    {
        entity.dbContext entity = new entity.dbContext();
        CollectionViewSource branchViewSource;
        entity.Properties.Settings _entity = new entity.Properties.Settings();

        public Branch()
        { InitializeComponent(); }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                branchViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("app_branchViewSource")));
                entity.db.app_branch.Where(a=>a.id_company == _entity.company_ID).OrderByDescending(a => a.is_active).Load();
                branchViewSource.Source = entity.db.app_branch.Local;
            }
            catch { }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.branch objBranch = new cntrl.branch();
            app_branch app_branch = new app_branch();
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
