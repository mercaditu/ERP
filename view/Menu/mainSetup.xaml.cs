using System;
using System.Windows;
using System.Windows.Controls;

namespace Cognitivo.Menu
{
    public partial class mainSetup : Page
    {
        enum MyEnum
        {
            Configs_Company,
            Security_User,
            Commercial_ContactRole,
            Configs_Branch,
            Configs_Currency,
            Configs_Terminal,
            Configs_Bank,
            Configs_Account,
            Configs_CostCenter,
            Configs_PriceList,
            Configs_Settings

        }

        MyEnum getenum = MyEnum.Configs_Company;

        public mainSetup()
        {
            InitializeComponent();
           
            if (entity.CurrentSession.ConnectionString==null)
            {
                Properties.Settings ViewSettings = new Properties.Settings();
                entity.CurrentSession.ConnectionString = ViewSettings.MySQLconnString;
            }
           
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            string[] data = getenum.ToString().Split('_');
            frameContainer.Navigate(new Uri(data[0] + "/" + data[1] + ".xaml", UriKind.RelativeOrAbsolute));
            btnNext.IsEnabled = true;
            btnPrev.IsEnabled = false;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            getenum = getenum + 1;
            string[] data = getenum.ToString().Split('_');
            frameContainer.Navigate(new Uri(data[0] + "/" + data[1] + ".xaml", UriKind.RelativeOrAbsolute));
            if (getenum == MyEnum.Configs_Settings)
            {
                btnNext.IsEnabled = false;
            }
            if (getenum == MyEnum.Commercial_ContactRole)
            {
                btnPrev.IsEnabled = true;
            }
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            getenum = getenum - 1;
            string[] data = getenum.ToString().Split('_');
            frameContainer.Navigate(new Uri(data[0] + "/" + data[1] + ".xaml", UriKind.RelativeOrAbsolute));
            if (getenum == MyEnum.Configs_Company)
            {
                btnPrev.IsEnabled = false;
                btnNext.IsEnabled = true;
            }
            if (getenum == MyEnum.Configs_PriceList)
            {
                btnNext.IsEnabled = true;
            }
        }
    }
}
