using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Cognitivo.Configs
{
    public partial class Language : Page
    {

        public Language()
        {
            InitializeComponent();
        }

        private void lang_Select(object sender, EventArgs e)
        {
            try
            {
                RadioButton selectedLanguage = sender as RadioButton;
                string lang = selectedLanguage.Name.ToString();
                lang = lang.Replace("_", "-");
                Properties.Settings.Default.language_ISO = lang;
                Properties.Settings.Default.Save();

                CultureInfo ci = new CultureInfo(Properties.Settings.Default.language_ISO);
                NumberFormatInfo LocalFormat =
        (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
                             
               WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = ci;
               // Thread.CurrentThread.CurrentUICulture = ci;
                
 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                //throw ex;
            }
        }

      
    }
}
