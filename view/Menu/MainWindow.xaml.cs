using Cognitivo.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Cognitivo.Menu
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private bool _is_LoggedIn = false;
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
                Settings.Default.wallpaper_Image = String.Format("https://source.unsplash.com/user/cognitivo/likes/{0}x{1}", width, height);
                Settings.Default.Save();
            }
        }

        #region "Blur Animations"

        private bool IsBlured = false;

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

        #endregion "Blur Animations"

        private void btnOpenInWindow_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Save();
        }

        public void Nav_Frame(string _modName)
        {
            if (_modName == "Config")
            {
                mainFrame.Navigate(new MainConfiguration());
            }
            else if (_modName == "Settings")
            {
                mainFrame.Navigate(new Configs.Settings());
            }
            else if (_modName == "LogOut")
            {
                is_LoggedIn = false;
                mainFrame.Navigate(new MainLogIn());
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

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.cognitivo.in/support/");
        }

        private void winMain_Closing(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (gridBackground.Background != null)
            {
                ImageBrush IB = FindResource("Wallpaper") as ImageBrush;
                if (IB != null)
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(IB.ImageSource.ToString(), entity.Brillo.IO.CreateIfNotExists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CogntivoERP\\Backgrounds\\Img.jpg"));
                }
            }
        }

        private void lblConnectionRetry_Click(object sender, MouseButtonEventArgs e)
        {
            Check_Connection();
        }

        public bool Check_Connection()
        {
            try
            {
                using (entity.db db = new entity.db())
                {
                    db.app_company.Find(entity.CurrentSession.Id_Company);
                }
                //If all is well, then Connection isn't Lost.
                entity.CurrentSession.ConnectionLost = false;
                return true;
            }
            catch
            {
                entity.CurrentSession.ConnectionLost = true;
                return false;
            }
        }
    }
}