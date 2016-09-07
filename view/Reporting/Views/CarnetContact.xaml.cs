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

namespace Cognitivo.Reporting.Views
{
    /// <summary>
    /// Interaction logic for CarnetContact.xaml
    /// </summary>
    public partial class CarnetContact : Page
    {
        public List<ContactLists> ContactList { get; set; }
        entity.db db = new entity.db();
        public CarnetContact()
        {
            InitializeComponent();
            ContactList = new List<ContactLists>();
        }
     
        private void set_ContactPref(object sender, RoutedEventArgs e)
        {
            if (sbxContact.ContactID>0)
            {
                contact Contact = db.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                if (Contact!=null)
                {
                    ContactLists ContactListobj = new ContactLists();
                    ContactListobj.Code = Contact.code;
                    ContactListobj.Name = Contact.name;
                    ContactList.Add(ContactListobj);
                }
                
            }
            ContactGridView.ItemsSource = null;
            ContactGridView.ItemsSource = ContactList;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CarnetContactReport CarnetContactReport = new CarnetContactReport(ContactList);
            Window window = new Window
            {
                Title = "Report",
                Content = CarnetContactReport
            };

            window.ShowDialog();
        }

    }
}
