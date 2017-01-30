using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Globalization;
using Cognitivo.Properties;
using System.ComponentModel;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Data;
using System.Data.Entity;

namespace Cognitivo.Menu
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        bool _is_LoggedIn = false;
        public List<entity.security_crud> security_curdList { get; set; }

        public bool is_LoggedIn
        {
            get { return _is_LoggedIn; }
            set
            {
                _is_LoggedIn = value;
                OnPropertyChanged("is_LoggedIn");
            }
        }


        public double width = SystemParameters.WorkArea.Width;
        public double height = SystemParameters.WorkArea.Height;


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                CultureInfo ci = new CultureInfo(Settings.Default.language_ISO);
                WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = ci;
            }
            catch (CultureNotFoundException)
            {
                Settings.Default.language_ISO = "en-US";
                Settings.Default.Save();
                CultureInfo eci = new CultureInfo(Settings.Default.language_ISO);
                WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = eci;
            }
            catch (Exception)
            {
                mainFrame.Navigate(new Configs.ConnectionBuilder());
                return;
            }

            if (Settings.Default.wallpaper_Image == "")
            {
                string img = String.Format("https://source.unsplash.com/user/cognitivo/likes/{0}x{1}", width, height);
                Settings.Default.wallpaper_Image = img;
                Settings.Default.Save();
            }

            mainFrame.Navigate(new mainLogIn());

        }

        #region "Blur Animations"
        bool IsBlured = false;
        private void mainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (mainFrame.NavigationService.Content != null)
            {
                string source = mainFrame.NavigationService.Content.ToString();
                if (source.Contains("mainMenu"))
                {
                    unblur();
                }
                else if (IsBlured == false)
                {
                    blur();
                }
            }
        }

        private void blur()
        {
            Storyboard blur = (Storyboard)FindResource("Blur");
            blur.Begin();
            IsBlured = true;
        }
        private void unblur()
        {
            Storyboard unblur = (Storyboard)FindResource("UnBlur");
            unblur.Begin();
            IsBlured = false;
        }
        #endregion

        private void btnOpenInWindow_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Save();
        }

        public void Nav_Frame(string _modName)
        {
            if (_modName == "Config")
            {
                mainFrame.Navigate(new mainConfiguration());
            }
            else if (_modName == "Settings")
            {
                mainFrame.Navigate(new Configs.Settings());
            }
            else if (_modName == "LogOut")
            {
                is_LoggedIn = false;
                mainFrame.Navigate(new mainLogIn());
            }
        }

        private void Nav_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (mainFrame.CanGoBack)
            {
                mainFrame.GoBack();
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (mainFrame.Content.ToString().Contains("Corporate"))
            {
                if (e.Key != Key.Enter)
                {
                    mainMenu_Corporate mainMenu = mainFrame.Content as mainMenu_Corporate;
                    mainMenu.SearchMode = true;
                }
            }
        }

        private void btnChangeWallpaper_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Reload();
        }

        private void lang_Select(object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton selectedLanguage = sender as RadioButton;
                string lang = selectedLanguage.Name.ToString();
                lang = lang.Replace("_", "-");
                Settings.Default.language_ISO = lang;
                Settings.Default.Save();

                CultureInfo ci = new CultureInfo(Settings.Default.language_ISO);
                NumberFormatInfo LocalFormat = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();

                WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = ci;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Settings_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (is_LoggedIn)
            {
                cbxBranch.ItemsSource = entity.CurrentSession.Branches;
            }
        }

        private async void cbxBranch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ////entity.Properties.Settings.Default.Save();
            ////entity.CurrentSession.Id_Branch = entity.Properties.Settings.Default.branch_ID;

            //using (entity.db db = new entity.db())
            //{
            //    cbxTerminal.ItemsSource = await db.app_terminal.Where(x =>
            //        x.id_company == entity.CurrentSession.Id_Company &&
            //        x.is_active &&
            //        x.id_branch == entity.CurrentSession.Id_Branch)
            //        .ToListAsync();
            //}
        }

        private async void cbxTerminal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //entity.Properties.Settings.Default.Save();
            //entity.CurrentSession.Id_Terminal = entity.Properties.Settings.Default.terminal_ID;

            //using (entity.db db = new entity.db())
            //{
            //    cbxAccount.ItemsSource = await db.app_account.Where(x =>
            //        x.id_company == entity.CurrentSession.Id_Company &&
            //        x.is_active &&
            //        (x.id_account_type == entity.app_account.app_account_type.Bank || x.id_terminal == entity.CurrentSession.Id_Terminal))
            //        .ToListAsync();
            //}
        }

        private void cbxAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //    entity.Properties.Settings.Default.Save();
            //    entity.CurrentSession.Id_Account = entity.Properties.Settings.Default.account_ID;
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.cognitivo.in/support/");
        }
    }
}
