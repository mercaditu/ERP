using entity;
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

namespace Cognitivo.Commercial
{
    public partial class PromissoryNote : Page
    {
        PromissoryNoteDB PromissoryNoteDB = new PromissoryNoteDB();
        CollectionViewSource payment_promissory_noteViewSource;
        public PromissoryNote()
        {
            InitializeComponent();
        }

     

        private void toolBar_btnApprove_Click(object sender)
        {
            PromissoryNoteDB.Approve();
        }

        private void toolBar_btnAnull_Click(object sender)
        {
            PromissoryNoteDB.Anull();
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            if (!string.IsNullOrEmpty(query) && payment_promissory_noteViewSource != null)
            {
                try
                {
                    payment_promissory_noteViewSource.View.Filter = i =>
                    {
                        payment_promissory_note payment_promissory_note = i as payment_promissory_note;

                        if (payment_promissory_note != null)
        {

                            if ((payment_promissory_note.contact != null ? payment_promissory_note.contact.name.ToLower().Contains(query.ToLower()) : false)
                                || payment_promissory_note.note_number.Contains(query)
                                || (payment_promissory_note.trans_date != null ? payment_promissory_note.trans_date.ToString() == query : false))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
        }

                        }
                        else
        {
                            return false;
                        }
                    };
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                payment_promissory_noteViewSource.View.Filter = null;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            payment_promissory_noteViewSource = ((CollectionViewSource)(FindResource("payment_promissory_noteViewSource")));
            payment_promissory_noteViewSource.Source = PromissoryNoteDB.payment_promissory_note.Local;
        }

        private void set_ContactPref(object sender, EventArgs e)
        {
            if (sbxContact.ContactID > 0)
        {
                contact contact = PromissoryNoteDB.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                payment_promissory_note payment_promissory_note = (payment_promissory_note)payment_promissory_noteViewSource.View.CurrentItem;
                payment_promissory_note.id_contact = contact.id_contact;
                payment_promissory_note.contact = contact;


        }
    }
}
}
