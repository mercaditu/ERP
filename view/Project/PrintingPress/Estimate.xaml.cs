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
using entity;
using System.Data;
using System.Reflection;
using Cognitivo.Project;

namespace Cognitivo.Project.PrintingPress
{
    public partial class Estimate
    {
        entity.dbContext entity = new entity.dbContext();
        List<PrintingPress.calc_Cost> final_cost = new List<PrintingPress.calc_Cost>();

        public int _product_Qty { get; set; }

        public Estimate()
        {
            InitializeComponent();
            cmbproject.SelectedValuePath = "id_project_template";
            cmbproject.DisplayMemberPath = "name";
            cmbproject.ItemsSource = entity.db.project_template.ToList();

            List<app_dimension> app_dimension = entity.db.app_dimension.ToList();

            CollectionViewSource app_dimensionsForHeightViewSource = (System.Windows.Data.CollectionViewSource)this.FindResource("app_dimensionsForHeightViewSource");
            app_dimensionsForHeightViewSource.Source = app_dimension;
            CollectionViewSource app_dimensionsForWeightViewSource = (System.Windows.Data.CollectionViewSource)this.FindResource("app_dimensionsForWeightViewSource");
            app_dimensionsForWeightViewSource.Source = app_dimension;
            CollectionViewSource app_dimensionsForWidthViewSource = (System.Windows.Data.CollectionViewSource)this.FindResource("app_dimensionsForWidthViewSource");
            app_dimensionsForWidthViewSource.Source = app_dimension;
        }


        private void tbCustomize_MouseUp(object sender, MouseButtonEventArgs e)
        {
            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            popupCustomize.StaysOpen = false;
            popupCustomize.IsOpen = true;
        }

        private void popupCustomize_Closed(object sender, EventArgs e)
        {
            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            Project.PrintingPress.Costing.Default.Long = (int)cbxLong.SelectedValue;
            Project.PrintingPress.Costing.Default.Short = (int)cbxShort.SelectedValue;
            Project.PrintingPress.Costing.Default.Weight = (int)cbxWeight.SelectedValue;
            Project.PrintingPress.Costing.Default.Save();
            popupCustomize.IsOpen = false;
        }

        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            TabItem TabItem = new TabItem();
            Project.PrintingPressCalculationPage PritingPressPage = new PrintingPressCalculationPage();

            if (cmbproject.SelectedValue == null)
            {
                MessageBox.Show("Selecionar un 'Tipo de Trabajo'...");
            }
            else
            {
                PritingPressPage._product_Qty = _product_Qty;
                PritingPressPage._id_project = (int)cmbproject.SelectedValue;
                PritingPressPage.pagename = "tabone";
                PritingPressPage.Tag = final_cost;
                Frame CostFrame = new Frame();
                CostFrame.Navigate(PritingPressPage);
                TabItem.Header = cmbproject.Text;
                TabItem.Content = CostFrame;
                CostTab.Items.Add(TabItem);
            }
        }

        private void TextBlock_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            Decimal _finalcost;
            final_cost.Clear();
            foreach (TabItem item in CostTab.Items)
            {
                if (item.Name != "ProjectTab")
                {
                    Frame costframe = (Frame)item.Content;
                    Project.PrintingPressCalculationPage PritingPressPage = (Project.PrintingPressCalculationPage)costframe.Content;
                    if (PritingPressPage._calc_Cost_BestPrinter != null)
                    {
                        final_cost.Add(PritingPressPage._calc_Cost_BestPrinter);
                    }
                }
            }
            _finalcost = decimal.Round(Convert.ToDecimal(final_cost.Sum(x => x.cost)), 2, MidpointRounding.AwayFromZero);
            MessageBox.Show("Total Cost is :- " + _finalcost.ToString());

        }

        private void TextBlock_MouseUp_2(object sender, MouseButtonEventArgs e)
        {
            if (((TabItem)CostTab.SelectedItem).Name != "ProjectTab")
            {
                CostTab.Items.Remove(((TabItem)CostTab.SelectedItem));
            }
        }

        private void TextBlock_MouseUp_3(object sender, MouseButtonEventArgs e)
        {
            View.ReportViewer PrintingPressviewr = new View.ReportViewer();
            PrintingPressviewr.loadReport(ref CostTab);
            PrintingPressviewr.Show();
        }
    }
}
