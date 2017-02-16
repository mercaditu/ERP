using entity;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace cntrl.PanelAdv
{
    public partial class pnlCostCalculation : UserControl
    {
        public List<production_order_detail> Inputproduction_order_detailList { get; set; }
        public List<production_order_detail> Outputproduction_order_detailList { get; set; }
        private CollectionViewSource inputViewSource;
        private CollectionViewSource outputViewSource;

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
                Class.CostCalculation CostCalculation = new Class.CostCalculation();
                inputViewSource = FindResource("inputViewSource") as CollectionViewSource;

                if (Outputproduction_order_detailList != null && Inputproduction_order_detailList != null)
                {
                    outputViewSource = FindResource("outputViewSource") as CollectionViewSource;
                    outputViewSource.Source = CostCalculation.CalculateOutputOrder(Outputproduction_order_detailList, Inputproduction_order_detailList);
                }
            }
        }
    }
}