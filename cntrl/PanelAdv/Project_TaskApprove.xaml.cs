using entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cntrl.PanelAdv
{
    /// <summary>
    /// Interaction logic for Project_TaskApprove.xaml
    /// </summary>
    public partial class Project_TaskApprove : UserControl
    {
        public Project_TaskApprove()
        {
            InitializeComponent();
        }
        public int? id_range { get; set; }
        public string number { get; set; }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cbxDocument.ItemsSource = entity.Brillo.Logic.Range.List_Range(entity.App.Names.ActivityPlan, CurrentSession.Id_Branch, CurrentSession.Id_terminal);
        }
        public event btnSave_ClickedEventHandler Save_Click;
        public delegate void btnSave_ClickedEventHandler(object sender);
        public void btnSave_MouseUp(object sender, EventArgs e)
        {
            


            if (Save_Click != null)
            {
                Save_Click(sender);
            }
            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;

        }

     
    }
}
