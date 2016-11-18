using System.Windows.Controls;

namespace Cognitivo.Reporting
{
    /// <summary>
    /// Interaction logic for ReportViewer.xaml
    /// </summary>
    public partial class ReportViewer : Page
    {

        public entity.App.Names appName { get; set; }
        public ReportViewer()
        {
            InitializeComponent();
        }
    }
}
