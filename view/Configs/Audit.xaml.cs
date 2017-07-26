using System.Windows.Controls;
using entity;
using System.Linq;
using System.Windows.Data;
using System.Data.Entity;

namespace Cognitivo.Configs
{
    public partial class Audit : Page
    {
        private dbContext entity = new dbContext();
        CollectionViewSource auditViewSource = new CollectionViewSource();

        public Audit()
        {
            InitializeComponent();
            entity.db.Configuration.AutoDetectChangesEnabled = false;
            entity.db.Configuration.LazyLoadingEnabled = true;
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            auditViewSource = FindResource("auditViewSource") as CollectionViewSource;
            auditViewSource.Source = entity.db.AuditEntries
                .Where(x => x.State != Z.EntityFramework.Plus.AuditEntryState.RelationshipAdded)
                .OrderByDescending(x => x.CreatedDate).ToList();

            //List<Z.EntityFramework.Plus.AuditEntry> entityLIST = new List<Z.EntityFramework.Plus.AuditEntry>();
            //if (entity.db.AuditEntries.Local.Count() > 0)
            //{
            //    foreach (AuditEntries application in entity.db.AuditEntries.Local.OrderBy(x => x..name).ToList())
            //    {
            //        if (contactLIST.Contains(payment.contact) == false)
            //        {
            //            contact contact = new contact();
            //            contact = payment.contact;
            //            contactLIST.Add(contact);
            //        }
            //    }
            //}
            //contactViewSource.Source = contactLIST;
        }

        private void dgvApplication_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void auditDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (auditViewSource != null)
            {
                if (auditViewSource.View != null)
                {
                    Z.EntityFramework.Plus.AuditEntry AuditEntry = auditViewSource.View.CurrentItem as Z.EntityFramework.Plus.AuditEntry;

                    if (AuditEntry != null)
                    {
                        CollectionViewSource auditpropertiesViewSource = FindResource("auditpropertiesViewSource") as CollectionViewSource;
                        auditpropertiesViewSource.Source = entity.db.AuditEntryProperties.Where(x => x.AuditEntryID == AuditEntry.AuditEntryID).ToList();
                    }
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (auditViewSource != null)
            {
                if (auditViewSource.View != null)
                {
                    auditViewSource.View.Filter = i =>
                    {
                        Z.EntityFramework.Plus.AuditEntry AuditEntry = i as Z.EntityFramework.Plus.AuditEntry;

                        if (
                            AuditEntry.EntitySetName.ToUpper().Contains(txtsearch.Text.ToUpper()) ||
                            AuditEntry.CreatedBy.ToUpper().Contains(txtsearch.Text.ToUpper())
                           )
                        {
                            return true;
                        }

                        return false;
                    };
                }
            }
        }
    }
}
