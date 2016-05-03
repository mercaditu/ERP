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
using entity;
namespace cntrl.Curd
{
    /// <summary>
    /// Interaction logic for ItemRequest.xaml
    /// </summary>
    public partial class ItemRequest : UserControl
    {
        public DateTime neededDate { get; set; }
        public entity.item_request_detail.Urgencies Urgencies { get; set; }
        public string name { get; set; }
        public string  comment { get; set; }
        public int? id_department { get; set; }
        public List<app_department> listdepartment { get; set; }
        public ItemRequest()
        {
            InitializeComponent();
        }



        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cbxurgencies.ItemsSource = Enum.GetValues(typeof(entity.item_request_detail.Urgencies));
            cbxdate.Text = DateTime.Now.ToString();
            CollectionViewSource app_departmentViewSource = ((CollectionViewSource)(FindResource("app_departmentViewSource")));
            app_departmentViewSource.Source = listdepartment;

        }
        public event btnSave_ClickedEventHandler item_request_Click;
        public delegate void btnSave_ClickedEventHandler(object sender);
        public void btnSave_MouseUp(object sender, EventArgs e)
        {

            if (item_request_Click != null)
            {
                item_request_Click(sender);
            }

        }

        private void btnCancel_Click(object sender, MouseButtonEventArgs e)
        {
            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;

        }
    }
}
