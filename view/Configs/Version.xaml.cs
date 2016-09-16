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
using entity;

namespace Cognitivo.Configs
{
    /// <summary>
    /// Interaction logic for Version.xaml
    /// </summary>
    public partial class Version : Page
    {
        public int UserNumber { get; set; }
        public entity.CurrentSession.Versions version { get; set; }
        db db = new db();
        public Version()
        {
            using (entity.db db = new entity.db())
            {
                UserNumber = db.security_user.Where(x => x.id_company == entity.CurrentSession.Id_Company).Count();
            }

            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
          
            entity.Brillo.Activation Activation = new entity.Brillo.Activation();

            Activation.VersionEncrypt(version);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem tabitem = TabVersion.SelectedItem as TabItem;
            if (tabitem != null)
            {


                if (tabitem.Header.ToString() == "BASIC".ToString())
                {
                    version = entity.CurrentSession.Versions.Basic;
                }
                else if (tabitem.Header.ToString() == "PyMES")
                {
                    version = entity.CurrentSession.Versions.Medium;
                }
                else if (tabitem.Header.ToString() == "Full")
                {
                    version = entity.CurrentSession.Versions.Full;
                }
                else if (tabitem.Header.ToString() == "EnterPrice")
                {
                    version = entity.CurrentSession.Versions.Enterprise;
                }
              
            }
        }

    }
}
