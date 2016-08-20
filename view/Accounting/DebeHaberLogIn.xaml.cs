using Cognitivo.Properties;
using Newtonsoft.Json;
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

        string Company_RUC = string.Empty;
        string Company_Name = string.Empty;

        public class DebeHaberCompany
        {
            public string gov_code { get; set; }
            public string name { get; set; }
            public string alias { get; set; }
        }

        public class DebeHaberRegistration
        {
            public string hash_integretion { get; set; }
        }

        public DebeHaberLogIn()
        {
            InitializeComponent();

            using (entity.db db = new entity.db())
            {
                entity.app_company company = db.app_company.Where(x => x.id_company == entity.CurrentSession.Id_Company).FirstOrDefault();
                if (company != null)
                {
                    Company_RUC = company.gov_code;
                    Company_Name = company.name;
                }

                if (company.hash_debehaber != null)
                {
                    tabUpLoad.IsSelected = true;
                }
            }
        }

        List<DebeHaberCompany> _DebeHaberCompanyList = new List<DebeHaberCompany>();

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string server = Cognitivo.Properties.Settings.Default.DebeHaberConnString + "/api/verification/" + UserName + "/" + Password;
                var json = await DownloadPage(server);
                _DebeHaberCompanyList = JsonConvert.DeserializeObject<List<DebeHaberCompany>>(json);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection Error: " + ex.Message);
                return;
            }

            DebeHaberCompany _DebeHaberCompany = _DebeHaberCompanyList.Where(x => x.gov_code == Company_RUC).FirstOrDefault();

            if (_DebeHaberCompany != null)
            {
                lblCompanyName.Content = _DebeHaberCompany.name;
                tabVerification.IsSelected = true;
            }
            else
            {
                MessageBox.Show("0 Companies Found with these credentials. Please check and try again.");
            }

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

        private async void btnApprove_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string server = Cognitivo.Properties.Settings.Default.DebeHaberConnString + "/api/registration/54HY3kXgamBsJ94hhd1DYsFSWzlI4KtF7aJMDxO9D4wnTVaEoqtuI42eC1sM5NMqFvZsHhYPgsudolP8Ug1JhKPyBMKxfbvGSnON/" + Company_RUC;
                var json = await DownloadPage(server);
                string Hash = JsonConvert.DeserializeObject<DebeHaberRegistration>(json).hash_integretion;
                using (entity.db db = new entity.db())
                {
                    entity.app_company company = db.app_company.Where(x => x.id_company == entity.CurrentSession.Id_Company).FirstOrDefault();
                    company.hash_debehaber = Hash;
                    db.SaveChanges();
                }

                tabUpLoad.IsSelected = true;
                frameDebeHaberIntg.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection Error: " + ex.Message);
                return;
            }
        }

        private void tbxServer_LostFocus(object sender, RoutedEventArgs e)
        {
            Settings SalesSettings = new Settings();

            Settings.Default.Save();
            SalesSettings = Settings.Default;
        }
    }
}
