using System;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using entity.Brillo;

namespace entity
{
    public partial class PromissoryNoteDB : BaseDB
    {
       

      

        #region Save
        public override int SaveChanges()
        {
            validate_PromissoryNote();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_PromissoryNote();
            return base.SaveChangesAsync();
        }

        private void validate_PromissoryNote()
        {
            foreach (payment_promissory_note payment_promissory_note in base.payment_promissory_note.Local)
            {
                if (payment_promissory_note.IsSelected)
                {
                    if (payment_promissory_note.State == EntityState.Added)
                    {
                        payment_promissory_note.timestamp = DateTime.Now;
                        payment_promissory_note.State = EntityState.Unchanged;
                        Entry(payment_promissory_note).State = EntityState.Added;
                    }
                    else if (payment_promissory_note.State == EntityState.Modified)
                    {
                        payment_promissory_note.timestamp = DateTime.Now;
                        payment_promissory_note.State = EntityState.Unchanged;
                        Entry(payment_promissory_note).State = EntityState.Modified;
                    }
                    else if (payment_promissory_note.State == EntityState.Deleted)
                    {
                        payment_promissory_note.timestamp = DateTime.Now;
                        payment_promissory_note.State = EntityState.Unchanged;
                        base.payment_promissory_note.Remove(payment_promissory_note);
                    }
                }
                else if (payment_promissory_note.State > 0)
                {
                    if (payment_promissory_note.State != EntityState.Unchanged)

                        Entry(payment_promissory_note).State = EntityState.Unchanged;
                }
            }

        }
        #endregion

        public void Approve()
        {
            foreach (payment_promissory_note payment_promissory_note in base.payment_promissory_note.Local.Where(x =>
                                                 x.status == Status.Documents.Pending
                                                         && x.IsSelected && x.Error == null))
            {
                payment_promissory_note.status = Status.Documents.Issued;
                entity.Brillo.Document.Start.Automatic(payment_promissory_note, payment_promissory_note.app_document_range);

            }

            base.SaveChanges();
        }
        public void Anull()
        {
            foreach (payment_promissory_note payment_promissory_note in base.payment_promissory_note.Local.Where(x =>
                                                 x.status == Status.Documents.Pending
                                                         && x.IsSelected && x.Error == null))
            {
                payment_promissory_note.status = Status.Documents.Returned;
            }
            base.SaveChanges();

        }
     
    }
}
