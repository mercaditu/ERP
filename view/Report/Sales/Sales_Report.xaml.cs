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

namespace Cognitivo.Report
{
    public partial class Sales_Report : Page
    {
        ReportPage ReportPage = Application.Current.Windows.OfType<ReportPage>() as ReportPage;

        public Sales_Report()
        {
            InitializeComponent();

            QueryBuilder();
        }

        private void QueryBuilder()
        {

        }
    }
}
