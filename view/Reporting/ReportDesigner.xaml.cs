using Syncfusion.Windows.Shared;
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

namespace Cognitivo.Reporting
{
    /// <summary>
    /// Interaction logic for ReportDesigner.xaml
    /// </summary>
    public partial class ReportDesigner : Page
    {
        public ReportDesigner()
        {
            InitializeComponent();

            SkinStorage.SetVisualStyle(this, "Office2013");
        }
    }
}
