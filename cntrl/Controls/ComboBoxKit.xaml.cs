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

namespace cntrl.Controls
{
    /// <summary>
    /// Interaction logic for ComboBoxKit.xaml
    /// </summary>
    public partial class ComboBoxKit : UserControl
    {

        public string Label { get; set; }
        public string SelectedValuePath { get; set; }

        public ComboBoxKit()
        {
            InitializeComponent();
        }

        private void lblClear_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
