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

namespace Cognitivo.Accounting
{
    /// <summary>
    /// Interaction logic for BalanceGeneral.xaml
    /// </summary>
    public partial class Reports : Page
    {
        public Reports()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            View.ReportViewerAccounting ReportViewerAccounting = new View.ReportViewerAccounting();
            ReportViewerAccounting.loadBalanceGeneral();
            ReportViewerAccounting.Show();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            View.ReportViewerAccounting ReportViewerAccounting = new View.ReportViewerAccounting();
            ReportViewerAccounting.loaddiario(Convert.ToDateTime(dtpstart.Text),Convert.ToDateTime(dtpend.Text),false);
            ReportViewerAccounting.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            View.ReportViewerAccounting ReportViewerAccounting = new View.ReportViewerAccounting();
            ReportViewerAccounting.loaddiario(Convert.ToDateTime(dtpstart.Text).Date, Convert.ToDateTime(dtpend.Text).Date,true);
            ReportViewerAccounting.Show();

        }
    }
}
