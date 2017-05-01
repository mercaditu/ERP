using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Windows;

namespace entity
{
    public partial class BaseDB : db
    {
        public int NumberOfRecords { get; set; }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (Exception)
            {
                //try to go through possible Validation Errors and remove before trying to re-attempt save.
                foreach (var error in base.GetValidationErrors())
                {
                    base.Entry(error.Entry).State = EntityState.Detached;
                }

                return base.SaveChanges();
            }
        }

        public override Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        /// <summary>
        /// Takes all Changes made to the Entity, and reverts it to the original state.
        /// </summary>
        public void CancelAllChanges()
        {
            if (MessageBox.Show(Brillo.Localize.Question_Cancel, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                foreach (var entry in ChangeTracker.Entries())
                {
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            {
                                entry.CurrentValues.SetValues(entry.OriginalValues);
                                entry.State = EntityState.Unchanged;
                                break;
                            }
                        case EntityState.Deleted:
                            {
                                entry.State = EntityState.Unchanged;
                                break;
                            }
                        case EntityState.Added:
                            {
                                entry.State = EntityState.Detached;
                                break;
                            }
                    }
                }
            }
        }

        public object GetClone(object obj, Type a)
        {
            var source = obj;
            var clone = Activator.CreateInstance(a);

            Entry(clone).State = EntityState.Added;

            var sourceValues = base.Entry(source).CurrentValues;
            Entry(clone).CurrentValues.SetValues(sourceValues);
            return clone;
        }
    }
}