using Syncfusion.Windows.Shared;
using System.Windows;

namespace Cognitivo.Reporting
{
    /// <summary>
    /// Interaction logic for ReportDesigner.xaml
    /// </summary>
    public partial class ReportDesigner
    {
        public ReportDesigner()
        {
            InitializeComponent();
            //This updates the UI to Office 2013 Design.
            SkinStorage.SetVisualStyle(this, "Office2013");
        }


    }
}
