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
using System.Collections;

namespace Cognitivo.Reporting.Views
{
    public partial class CarnetContact : Page
    {
        public object ContactList { get; set; }
        entity.db db = new entity.db();
        public CarnetContact()
        {
            InitializeComponent();

        }

        private void set_ContactPref(object sender, RoutedEventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                contact Contact = db.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();

                if (Contact != null)
                {
                    List<contact> contact_detail = new List<entity.contact>();
                    contact_detail.Add(Contact);
                    List<ContactLists> a = new List<ContactLists>();
                    ContactList = contact_detail
                        .Select(g => new
                       {
                           id_contact = g.id_contact,
                           contacts_name = g.name,
                           date_birth = g.date_birth,
                           gove_code = g.gov_code,
                           trans_date = g.timestamp,
                           contacts_code = g.code,
                           Product_code = g.contact_subscription.FirstOrDefault() != null ? g.contact_subscription.FirstOrDefault().item != null ? g.contact_subscription.FirstOrDefault().item.name : "" : "",
                           name = ""
                       });
                }
            }
            ContactGridView.ItemsSource = (IEnumerable)ContactList;
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
