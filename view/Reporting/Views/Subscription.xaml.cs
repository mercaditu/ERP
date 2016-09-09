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
    public partial class ContactSubscription : Page
    {
        public List<ContactInfo> ContactInfoList { get; set; }

        entity.db db = new entity.db();
        public CollectionViewSource contactViewSource;

        public ContactSubscription()
        {
            InitializeComponent();
        }

        private void set_ContactPref(object sender, RoutedEventArgs e)
        {
            if (sbxContact.ContactID > 0)
            {
                contact _Contact = db.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                if (_Contact != null)
                {
                    if (ContactInfoList == null)
                    {
                        ContactInfoList = new List<ContactInfo>();
                    }

                    ContactInfo Parent = matchContact(_Contact);

                    foreach (contact contactChild in _Contact.child)
                    {
                        Parent.Children += ", " + contactChild.name;
                    }

                    
                    ContactInfoList.Add(Parent);

                    contactViewSource = ((CollectionViewSource)(FindResource("contactViewSource")));
                    contactViewSource.Source = ContactInfoList;
                    contactViewSource.View.Refresh();
                }
            }
        }

        private ContactInfo matchContact(entity.contact _Contact)
        {
            ContactInfo NewContact = new ContactInfo();
            NewContact.ID = _Contact.id_contact;
            NewContact.Code = _Contact.code;
            NewContact.Name = _Contact.name;
            NewContact.GovCode = _Contact.gov_code;
            NewContact.SubscriptionItem = _Contact.contact_subscription.FirstOrDefault() != null ? _Contact.contact_subscription.FirstOrDefault().item != null ? _Contact.contact_subscription.FirstOrDefault().item.name : "" : "";
            if (_Contact.date_birth != null)
            {
                NewContact.DateBirth = (System.DateTime)_Contact.date_birth;
            }
            
            NewContact.StartDate = _Contact.timestamp;

            return NewContact;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CarnetContactReport CarnetContactReport = new CarnetContactReport(ContactInfoList);
            
            Window window = new Window
            {
                Title = "Report",
                Content = CarnetContactReport
            };

            window.ShowDialog();
        }

    }
}
