using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using entity;
using System.Data.Entity;

namespace Cognitivo.Commercial
{
    /// <summary>
    /// Interaction logic for Reconciliation.xaml
    /// </summary>
    public partial class Reconciliation : Page
    {
        CollectionViewSource app_accountViewSource;
        CollectionViewSource app_accountapp_account_detailViewSource;
        db db = new db();
        public Reconciliation()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            app_accountViewSource = ((CollectionViewSource)(FindResource("app_accountViewSource")));
            db.app_account.Where(x=>x.id_company==CurrentSession.Id_Company && x.id_account_type==app_account.app_account_type.Bank).Load();
            app_accountViewSource.Source = db.app_account.Local;
            app_accountapp_account_detailViewSource = ((CollectionViewSource)(FindResource("app_accountapp_account_detailViewSource")));
        }

     

       
        private void ApproveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as app_account_detail != null)
            {
                e.CanExecute = true;
            }
        }

        private void ApproveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            app_account_detail app_account_detail = app_accountapp_account_detailViewSource.View.CurrentItem as app_account_detail;
            app_account_detail.status = Status.Documents_General.Approved;

            app_accountapp_account_detailViewSource.View.Refresh();

        }

        private void AnullCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as app_account_detail != null)
            {
                e.CanExecute = true;
            }
        }

        private void AnullCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            app_account_detail app_account_detail = app_accountapp_account_detailViewSource.View.CurrentItem as app_account_detail;
            app_account_detail.status = Status.Documents_General.Annulled;

            app_accountapp_account_detailViewSource.View.Refresh();

        }

        private void PendingCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as app_account_detail != null)
            {
                e.CanExecute = true;
            }
        }

        private void PendingCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            app_account_detail app_account_detail = app_accountapp_account_detailViewSource.View.CurrentItem as app_account_detail;
            app_account_detail.status = Status.Documents_General.Pending;

            app_accountapp_account_detailViewSource.View.Refresh();
        }

        private void toolBar_btnSave_Click(object sender)
        {
            app_account app_account = app_accountViewSource.View.CurrentItem as app_account;
            db.SaveChanges();
            app_account.State = EntityState.Unchanged;
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            app_account app_account = app_accountViewSource.View.CurrentItem as app_account;
            if (app_account!=null)
            {
                app_account.State = EntityState.Modified;
            }
         
        }
    }
}
