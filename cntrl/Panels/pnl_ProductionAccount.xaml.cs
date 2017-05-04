using entity;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace cntrl.Panels
{
    public partial class pnl_ProductionAccount : UserControl
    {
        private CollectionViewSource production_accountViewSource;
        public db ExecutionDB { get; set; }

        public production_execution_detail production_execution_detail { get; set; }

        public decimal Quantity_to_Execute { get; set; }

        public pnl_ProductionAccount()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            production_accountViewSource = FindResource("production_accountViewSource") as CollectionViewSource;

            List< production_service_account> production_service_accountList=
            ExecutionDB.production_service_account
            .Where
            (
                x =>
                x.id_company == CurrentSession.Id_Company &&
                x.id_item == production_execution_detail.id_item
            )
            .Include(x => x.item)
            .ToList();
            production_service_accountList= production_service_accountList.Where(x => x.Balance > 0).ToList();
            production_accountViewSource.Source = production_service_accountList;

            if (production_service_accountList.Count() == 0)
            {
                gridSave.Visibility = Visibility.Hidden;
            }
        }

        private void btnCancel_Click(object sender, MouseButtonEventArgs e)
        {
            item_inventory_detailDataGrid.CancelEdit();

            Grid parentGrid = Parent as Grid;
            if (parentGrid != null)
            {
                parentGrid.Children.Clear();
                parentGrid.Visibility = Visibility.Hidden;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //Make proper logic for Quantites
            production_service_account SelectedAccount = production_accountViewSource.View.OfType< production_service_account>().ToList().Where(x => x.IsSelected).FirstOrDefault();

            //if (SelectedAccount == null)
            //{ SelectedAccount = ExecutionDB.production_service_account.Local.FirstOrDefault(); }

            if (SelectedAccount != null)
            {
                production_service_account production_service_account = new production_service_account();
                production_service_account.id_contact = production_execution_detail.id_contact;
                production_service_account.id_order_detail = production_execution_detail.id_order_detail;
                production_service_account.id_item = (int)production_execution_detail.id_item;
                production_service_account.unit_cost = SelectedAccount.unit_cost;
                production_service_account.debit = production_execution_detail.quantity;
                production_service_account.credit = 0;
                SelectedAccount.child.Add(production_service_account);
            }
            ExecutionDB.SaveChanges();
            btnCancel_Click(sender, null);
        }
    }
}