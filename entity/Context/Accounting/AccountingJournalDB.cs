
using System;
using System.Data.Entity;
using System.Threading.Tasks;


namespace entity
{
    public partial class AccountingJournalDB:BaseDB
    {
        public override int SaveChanges()
        {
            validate_Journal();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Journal();
            return base.SaveChangesAsync();
        }


        private void validate_Journal()
        {
            NumberOfRecords = 0;

            foreach (accounting_journal accounting_journal in base.accounting_journal.Local)
            {
                if (accounting_journal.IsSelected && accounting_journal.Error == null)
                {
                    if (accounting_journal.State == EntityState.Added)
                    {
                        accounting_journal.timestamp = DateTime.Now;
                        accounting_journal.State = EntityState.Unchanged;
                        Entry(accounting_journal).State = EntityState.Added;
                    }
                    else if (accounting_journal.State == EntityState.Modified)
                    {
                        accounting_journal.timestamp = DateTime.Now;
                        accounting_journal.State = EntityState.Unchanged;
                        Entry(accounting_journal).State = EntityState.Modified;
                    }
                    else if (accounting_journal.State == EntityState.Deleted)
                    {
                        accounting_journal.timestamp = DateTime.Now;
                        accounting_journal.State = EntityState.Unchanged;
                        base.accounting_journal.Remove(accounting_journal);
                    }
                    NumberOfRecords += 1;
                }
                else if (accounting_journal.State > 0)
                {
                    if (accounting_journal.State != EntityState.Unchanged)
                    {
                        Entry(accounting_journal).State = EntityState.Unchanged;
                    }
                }
            }
        }

        public bool Approve()
        {
            validate_Journal();
            SaveChanges();

            return true;
        }

       
    }
}
