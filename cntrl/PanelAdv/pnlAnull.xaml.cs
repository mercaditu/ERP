using entity;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace cntrl.PanelAdv
{
    public partial class pnlAnull : UserControl
    {
        public int ID { get; set; }
        private db db = new db();

        public pnlAnull()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, MouseButtonEventArgs e)
        {
            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                List<object> ListMovement = new List<object>();

                List<purchase_invoice_detail> purchase_invoice_detailList = db.purchase_invoice_detail.Where(x => x.id_purchase_invoice == ID).ToList();
                foreach (purchase_invoice_detail purchase_invoice_detail in purchase_invoice_detailList)
                {
                    ListMovement.AddRange(db.item_movement.Where(x => x.id_purchase_invoice_detail == purchase_invoice_detail.id_purchase_invoice_detail).ToList().Select(x => new { x.comment, Balance = (x.credit - x.debit), x.id_movement }));
                }

                AddDataGrid(ListMovement);
            }
        }

        public void AddDataGrid(List<object> ListMovement)
        {
            if (ListMovement.Count() > 0)
            {
                DataGrid DataGrid = new DataGrid();
                DataGrid.ItemsSource = ListMovement;
                DataGrid.SelectionChanged += DataGrid_SelectionChanged;
                stpgrid.Children.Add(DataGrid);
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid DataGrid = sender as DataGrid;
            dynamic item_movement = DataGrid.SelectedItem as dynamic;
            if (item_movement != null)
            {
                List<object> ListMovement = new List<object>();
                int id_movement = (int)item_movement.id_movement;
                ListMovement.AddRange(db.item_movement.Where(x => x.parent.id_movement == id_movement).ToList().Select(x => new { x.comment, Balance = (x.credit - x.debit), x.id_movement }));
                stpgrid.Children.RemoveRange(stpgrid.Children.IndexOf(DataGrid) + 1, stpgrid.Children.Count - stpgrid.Children.IndexOf(DataGrid));
                AddDataGrid(ListMovement);
            }
        }
    }
}