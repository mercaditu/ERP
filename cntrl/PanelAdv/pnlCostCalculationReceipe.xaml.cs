using entity;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace cntrl.PanelAdv
{
    public partial class pnlCostCalculationReceipe : UserControl
    {
        public List<item_recepie_detail> Outputitem_recepie_detailList { get; set; }
        public List<item_recepie> Inputitem_recepieList { get; set; }
        private CollectionViewSource inputViewSource;
        private CollectionViewSource outputViewSource;

        public pnlCostCalculationReceipe()
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
                inputViewSource.Source = CostCalculation.CalculateOutputOrderRecipe(Inputitem_recepieList);
                outputViewSource = FindResource("outputViewSource") as CollectionViewSource;
            }
        }
    }
}