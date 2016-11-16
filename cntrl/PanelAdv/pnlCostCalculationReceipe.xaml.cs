using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using entity;
using System;

namespace cntrl.PanelAdv
{
    public partial class pnlCostCalculationReceipe : UserControl
    {
        public List<item_recepie_detail> Outputitem_recepie_detailList { get; set; }
        CollectionViewSource inputViewSource;
        CollectionViewSource outputViewSource;
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

                outputViewSource = FindResource("outputViewSource") as CollectionViewSource;
                outputViewSource.Source = CostCalculation.CalculateOutputOrderRecipe(Outputitem_recepie_detailList);


            }
        }

      
    }
}