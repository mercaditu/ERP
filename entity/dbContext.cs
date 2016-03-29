using System.Linq;
using System.Data.Entity;
using System.Windows;
using System.Data.Entity.Infrastructure;
using WPFLocalizeExtension.Extensions;

namespace entity
{
    public class dbContext
    {
        public db db
        {
            get
            { return dbCntxt; }
            set
            { dbCntxt = value; }
        }
        db dbCntxt = new db();

        public dbContext()
        {
            dbCntxt.Configuration.LazyLoadingEnabled = true;
        }

        public bool HasUnsavedChanges()
        {
            return dbCntxt.ChangeTracker.Entries().Any(e => e.State == EntityState.Added
                                                         || e.State == EntityState.Modified
                                                         || e.State == EntityState.Deleted);
        }

        public void SaveChanges()
        {
            dbCntxt.SaveChanges();
        }

        /// <summary>
        /// Cancels changes made to Entity asking the user to approve this method before continuing. 
        /// </summary>
        public void CancelChanges_withQuestion()
        {
            string str = LocExtension.GetLocalizedValue<string>("Cognitivo:local:" + "Question_Cancel");
            if (MessageBox.Show(str, "Cognitivo ERP",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                CancelChanges();
            }
        }

        /// <summary>
        /// Cancels changes made to Enttiy without asking the user. 
        /// Mostly used in CURD Controls.
        /// </summary>
        public void CancelChanges()
        {
            foreach (DbEntityEntry entry in db.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                    default: break;
                }
            }
        }

        /// <summary>
        /// Cancel changes for selected entity only.
        /// </summary>
        /// <param name="entry">Object of DbEntityEntry</param>
        public void CancelChangesForSingleEntry(DbEntityEntry entry)
        {
            string str = LocExtension.GetLocalizedValue<string>("Cognitivo:local:" + "Question_Cancel");
            if (MessageBox.Show(str, "Cognitivo ERP",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                    default: break;
                }
            }
        }
        
        public static object Entry(item_inventory item_inventory)
        {
            throw new System.NotImplementedException();
        }
    }
}
