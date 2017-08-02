using System.Windows;
using System.Windows.Controls;
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
            Sales.Settings.Default.Default_Customer = sbxContact.ContactID;
            Sales.Settings.Default.Save();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Sales.Settings.Default.Save();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (db db= new db())
            {
                int ContactID = Sales.Settings.Default.Default_Customer;
                contact contact = await db.contacts.FindAsync(ContactID);
                if (contact!=null)
                {
                    sbxContact.Text = contact.name;
                }
            }
        }
    }
}
