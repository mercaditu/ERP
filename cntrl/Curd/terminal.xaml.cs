using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using entity;
using System.Data.Entity.Validation;

namespace cntrl
{
    public partial class terminal : UserControl
    {
      //  entity.Properties.Settings _entity = new entity.Properties.Settings();

        CollectionViewSource _app_terminalViewSource = null;
        public CollectionViewSource app_terminalViewSource { get { return _app_terminalViewSource; } set { _app_terminalViewSource = value; } }

        private entity.dbContext objentity = null;
        public entity.dbContext entity { get { return objentity; } set { objentity = value; } }

        public terminal()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                stackMain.DataContext = app_terminalViewSource;

                CollectionViewSource app_branchViewSource = (System.Windows.Data.CollectionViewSource)this.FindResource("app_branchViewSource");
                app_branchViewSource.Source = entity.db.app_branch.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToList();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IEnumerable<DbEntityValidationResult> validationresult = entity.db.GetValidationErrors();
                if (validationresult.Count() == 0)
                {
                    entity.db.SaveChanges();
                    if (CurrentSession.Id_Terminal == 0)
                    {
                        CurrentSession.Id_Terminal = entity.db.app_terminal.FirstOrDefault().id_terminal;
                    }
                    btnCancel_Click(sender, e);
                }
            }
            catch (Exception)
            {
                //throw ex;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    app_terminal app_terminal = app_terminalViewSource.View.CurrentItem as app_terminal;
                    app_terminal.is_active = false;
                    btnSave_Click(sender, e);
                }
            }
            catch (Exception)
            {
                //throw ex;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                entity.CancelChanges();
                app_terminalViewSource.View.Refresh();
                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
