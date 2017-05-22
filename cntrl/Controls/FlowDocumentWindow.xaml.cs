
using System;
using System.Windows.Controls;

namespace cntrl.Controls
{
    /// <summary>
    /// Interaction logic for FlowDocumentWindow.xaml
    /// </summary>
    public partial class FlowDocumentWindow : UserControl
    {
        public FlowDocumentWindow()
        {
            InitializeComponent();
            Browser.Navigate(new Uri("about:blank"));
        }
    }
}
