using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using entity;

namespace cntrl.PanelAdv
{
    /// <summary>
    /// Interaction logic for ActionPanel.xaml
    /// </summary>
    public partial class ActionPanel : UserControl
    {
      
      
       public List<item_movement> item_movementOldList { get; set; }
        public List<item_movement> item_movementList { get; set; }
        CollectionViewSource item_movementOldViewSource, item_movementViewSource;

        public ActionPanel()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource item_movementViewSource = (CollectionViewSource)FindResource("item_movementViewSource");
            CollectionViewSource item_movementOldViewSource = (CollectionViewSource)FindResource("item_movementOldViewSource");
            item_movementViewSource.Source = item_movementList;
            item_movementOldViewSource.Source = item_movementOldList;
        }

        private void btnCancel_Click(object sender, MouseButtonEventArgs e)
        {
            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
           

        }


    }
}
