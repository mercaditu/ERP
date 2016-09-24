using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using entity;
using System.Data.Entity.Validation;


namespace cntrl
{
    public partial class account : UserControl
    {
        CollectionViewSource accountsViewSource = null;
        //public CollectionViewSource objCollectionViewSource { get { return ItemsSource; } set { ItemsSource = value; } }

        private dbContext entity = new dbContext();
        //public entity.dbContext entity { get { return _entity; } set { _entity = value; } }

        private entity.app_account _accountobject = null;
        public entity.app_account accountobject { get { return _accountobject; } set { _accountobject = value; } }

        private Class.clsCommon.Mode _operationMode = 0;
        public Class.clsCommon.Mode operationMode { get { return _operationMode; } set { _operationMode = value; } }


        entity.Properties.Settings _settings = new entity.Properties.Settings();

        public account()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
               // 
 
                accountsViewSource = (CollectionViewSource)FindResource("accountsViewSource");
                entity.db.app_account.Where(x => x.is_active == true && x.id_company == _settings.company_ID).ToList();
                accountsViewSource.Source = entity.db.app_account.Local;
          
             
                cbxAccountType.ItemsSource = Enum.GetValues(typeof(app_account.app_account_type));

                CollectionViewSource app_bankViewSource = (System.Windows.Data.CollectionViewSource)this.FindResource("app_bankViewSource");
                app_bankViewSource.Source = entity.db.app_bank.Where(a => a.is_active == true && a.id_company == _settings.company_ID).OrderBy(a => a.name).ToList();

                CollectionViewSource app_terminalViewSource = (System.Windows.Data.CollectionViewSource)this.FindResource("app_terminalViewSource");
                app_terminalViewSource.Source = entity.db.app_terminal.Where(a => a.is_active == true).OrderBy(a => a.name).ToList();

                CollectionViewSource app_currencyViewSource = (System.Windows.Data.CollectionViewSource)this.FindResource("app_currencyViewSource");
                app_currencyViewSource.Source = entity.db.app_currency.Where(a => a.is_active == true && a.id_company == _settings.company_ID).OrderBy(a => a.name).ToList();
                if (operationMode == Class.clsCommon.Mode.Add)
                {
                    app_account app_account = new app_account();
                    app_account.name = "account";
                    app_account.is_active =true;
                    entity.db.app_account.Add(app_account);
                  //  entity.db.SaveChanges();

                    accountsViewSource.View.Refresh();
                    accountsViewSource.View.MoveCurrentToLast();
                }
                else
                {
                    accountsViewSource.View.MoveCurrentTo(entity.db.app_account.Where(x => x.id_account == accountobject.id_account).FirstOrDefault());
                }
                stackMainAc.DataContext = accountsViewSource;
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
                    if (CurrentSession.Id_Account == 0)
                    {
                        CurrentSession.Id_Account = entity.db.app_account.FirstOrDefault().id_account;
                    }
                    btnCancel_Click(sender, e);
                }
            }
            catch (Exception ex)
            { throw ex; }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                entity.CancelChanges();
                accountsViewSource.View.Refresh();
                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception ex)
            { throw ex; }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                entity.app_account objAccount = accountsViewSource.View.CurrentItem as entity.app_account;
                objAccount.is_active = false;
                btnSave_Click(sender, e);
            }
        }

        //Esc Key
        private void UserControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure want to close this window?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    btnCancel_Click(sender, e);
                }
            }
        }

     
    }
}
