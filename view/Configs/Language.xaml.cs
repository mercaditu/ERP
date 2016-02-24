using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WPFLocalizeExtension;

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
                Cognitivo.Properties.Settings.Default.language_ISO = lang;
                Cognitivo.Properties.Settings.Default.Save();

                CultureInfo ci = new CultureInfo(Cognitivo.Properties.Settings.Default.language_ISO);
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
