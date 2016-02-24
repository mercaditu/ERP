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

namespace Cognitivo.Configs
{
    /// <summary>
    /// Interaction logic for ContractRecords.xaml
    /// </summary>
    public partial class Contract : Page
    {
        entity.dbContext entity = new entity.dbContext();
        CollectionViewSource app_contractViewSource;
        entity.Properties.Settings _entity = new entity.Properties.Settings();

        public Contract()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            app_contractViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("app_contractViewSource")));
            entity.db.app_contract.Include(i => i.app_contract_detail).Include(i => i.app_condition).Where(a => a.id_company == _entity.company_ID).OrderByDescending(i => i.is_active).Load();
            app_contractViewSource.Source = entity.db.app_contract.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.contract objContract = new cntrl.contract();
            app_contract app_contract = new app_contract();
            entity.db.app_contract.Add(app_contract);
            app_contractViewSource.View.MoveCurrentToLast();
            objContract.app_contractViewSource = app_contractViewSource;
            objContract.entity = entity;
            crud_modal.Children.Add(objContract);
        }

        private void pnl_Contract_linkEdit_Click(object sender, int intContractId)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.contract objContract = new cntrl.contract();
            app_contractViewSource.View.MoveCurrentTo(entity.db.app_contract.Where(x => x.id_contract == intContractId).FirstOrDefault());
            objContract.app_contractViewSource = app_contractViewSource;
            objContract.entity = entity;
            crud_modal.Children.Add(objContract);
        }
    }
}
