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
    /// Interaction logic for DebeHaber.xaml
    /// </summary>
    public partial class DebeHaber : Page
    {
        public bool isReady { get; set; }
        public bool serverStatus { get; set; }
        public bool apiStatus { get; set; }

        public DebeHaber()
        {
            InitializeComponent();

            //Check KeyStatus on thread
            CheckStatus();
        }

        private void CheckStatus()
        {
            isReady = true;
            serverStatus = true;
            apiStatus = true;
        }
            
        private void ClickInformation(object sender, MouseButtonEventArgs e)
        {

        }

        private void OpenConfig(object sender, MouseButtonEventArgs e) => popConnBuilder.IsOpen = true;
    }
}
