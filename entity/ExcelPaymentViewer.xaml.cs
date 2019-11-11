using entity.Brillo;
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

namespace entity
{
    /// <summary>
    /// Interaction logic for ExcelPaymentViewer.xaml
    /// </summary>
    public partial class ExcelPaymentViewer : UserControl
    {
        public List<ExcelList> paymnetlist = new List<ExcelList>();
        public ExcelPaymentViewer()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgPaymnet.ItemsSource = paymnetlist;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window Parent = this.Parent as Window;
            Parent.DialogResult = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Window Parent = this.Parent as Window;
            Parent.DialogResult = false;
        }
    }
}
