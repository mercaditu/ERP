using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
    public partial class DebeHaberLogIn : Page
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        string ErrorMessage;
        public DebeHaberLogIn()
        {
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string api = "http://104.131.70.188/api/verification_api/" + UserName +"/" + Password ;
            var r = await DownloadPage(api);
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                dgEmployee.ItemsSource = r;
            }));
        }

        static async Task<string> DownloadPage(string url)
        {
            using (var client = new HttpClient())
            {
                using (var r = await client.GetAsync(new Uri(url)))
                {
                    string result = await r.Content.ReadAsStringAsync();
                    return result;
                }
            }
        }
        private void Verify_Access(string User, string Password, string ServerAddress)
        {

        }
    }
}
