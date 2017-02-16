using System.Windows;
using System.Windows.Controls;

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