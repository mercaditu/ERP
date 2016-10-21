using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using entity;

namespace cntrl.PanelAdv
{
    public partial class pnlCostCalculation : UserControl
    {
        public List<production_order_detail> Inputproduction_order_detailList { get; set; }
        public List<production_order_detail> Outputproduction_order_detailList { get; set; }

        public pnlCostCalculation()
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
                //Load your data here and assign the result to the CollectionViewSource.
                Class.CostCalculation CostCalculation = new Class.CostCalculation();
                InputDataGrid.ItemsSource = CostCalculation.CalculateOrderCost(Inputproduction_order_detailList);
                OutPutDataGrid.ItemsSource = CostCalculation.CalculateOutputOrder(Outputproduction_order_detailList);

            }
        }

        private void OutPutDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OutPutDataGrid.SelectedItem!=null)
            {
                Class.OutputList OutputList = OutPutDataGrid.SelectedItem as Class.OutputList;

                Class.CostCalculation CostCalculation = new Class.CostCalculation();
                InputDataGrid.ItemsSource = CostCalculation.CalculateOrderCost(Inputproduction_order_detailList.Where(x=>x.parent.id_order_detail == OutputList.id_order_detail).ToList());
            }
        }
    }
}