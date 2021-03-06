﻿using Cognitivo.Menu;
using Cognitivo.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cognitivo.Accounting
{
    public partial class DebeHaberLogInLatest : Page
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
        public string UserName { get; set; }

        private string Company_RUC = string.Empty;
        private string Company_Name = string.Empty;

        public class DebeHaberCompany
        {
            public int id { get; set; }
            public string gov_code { get; set; }
            public string name { get; set; }
            public string alias { get; set; }
            public deadlines deadline { get; set; }

        }
        public class deadlines
        {
            private DateTime date { get; set; }
            private int timezone_type { get; set; }
            private string timezone { get; set; }

        }



        public class DebeHaberRegistration
        {
            public string Key { get; set; }
        }

        public DebeHaberLogInLatest()
        {
            InitializeComponent();

            using (entity.db db = new entity.db())
            {
                entity.app_company company = db.app_company.Where(x => x.id_company == entity.CurrentSession.Id_Company).FirstOrDefault();
                if (company == null)
                {
                    company = db.app_company.FirstOrDefault();
                }

                Company_RUC = company.gov_code;

                Company_Name = company.name;

                //if (string.IsNullOrEmpty(company.hash_debehaber) == false)
                //{
                //    tabUpLoad.IsSelected = true;
                //    frameDebeHaberIntg.Refresh();
                //}
                try
                {
                    txtusername.Text = company.hash_debehaber;
                    RaisePropertyChanged("UserName");
                  //  check_api(company.hash_debehaber, Company_RUC);
                    if (_DebeHaberCompanyList.Count()>0)
                    {
                        tabUpLoad.IsSelected = true;
                        frameDebeHaberIntg.Refresh();
                    }
                  else
                    {
                        tabLogIn.IsSelected = true;
                    }

                }
                catch (Exception)
                {
                    tabLogIn.IsSelected = true;

                }

               

            }
        }

        private List<DebeHaberCompany> _DebeHaberCompanyList = new List<DebeHaberCompany>();
       

        private  void btnLogin_Click(object sender, RoutedEventArgs e)
        {
           
                tabVerification.IsSelected = true;
           
        }

        public static async Task<string> DownloadPage(string url)
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

        private  void btnApprove_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //string server = Settings.Default.DebeHaberConnString + "/api/registration/54HY3kXgamBsJ94hhd1DYsFSWzlI4KtF7aJMDxO9D4wnTVaEoqtuI42eC1sM5NMqFvZsHhYPgsudolP8Ug1JhKPyBMKxfbvGSnON/" + Company_RUC;

                //var json = await DownloadPage(server);
                //string Hash = JsonConvert.DeserializeObject<DebeHaberRegistration>(json).Key;
                using (entity.db db = new entity.db())
                {
                    entity.app_company company = db.app_company.Where(x => x.id_company == entity.CurrentSession.Id_Company).FirstOrDefault();
                    company.hash_debehaber = txtusername.Text;
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
        public async void check_api(string Hash, string GovCode)
        {
            try
            {
                string server = Settings.Default.DebeHaberConnString + "/api/transactionsverfiyV2/" + Hash + "/" + Company_RUC;
                var json = await DownloadPage(server);
                _DebeHaberCompanyList = JsonConvert.DeserializeObject<List<DebeHaberCompany>>(json);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection Error: " + ex.Message);
                tabLogIn.IsSelected = true;
                return;
            }

        }
    }
}