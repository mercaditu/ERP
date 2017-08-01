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
    /// Interaction logic for Configuration.xaml
    /// </summary>
    public partial class Configuration : Page
    {
        public Configuration()
        {
            InitializeComponent();
        }

        private void Set_ContactPref(object sender, RoutedEventArgs e)
        {
            Cognitivo.Properties.Settings.Default.DefultCustomer = sbxContact.ContactID;
            Cognitivo.Properties.Settings.Default.Save();

        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {

            Cognitivo.Properties.Settings.Default.Save();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (db db= new db())
            {
                int ContactID = Cognitivo.Properties.Settings.Default.DefultCustomer;
                contact contact = db.contacts.Where(x => x.id_contact == ContactID).FirstOrDefault();
                if (contact!=null)
                {
                    sbxContact.Text = contact.name;
                }
               
            }
        }
    }
}
