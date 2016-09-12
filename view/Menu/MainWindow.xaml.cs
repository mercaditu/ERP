using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Globalization;
using Cognitivo.Properties;
using System.ComponentModel;

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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
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
                mainFrame.Navigate(new Cognitivo.Configs.ConnectionBuilder());
                return;
            }

            if (Settings.Default.wallpaper_Image == "")
            {
                double width = SystemParameters.WorkArea.Width;
                double height = SystemParameters.WorkArea.Height;
                string img = String.Format("https://source.unsplash.com/user/cognitivo/likes/{0}x{1}", width, height);
                Settings.Default.wallpaper_Image =  img;
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
    }
}
