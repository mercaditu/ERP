using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Windows;
using WPFLocalizeExtension.Extensions;

namespace entity
{
    public partial class BaseDB : db
    {
        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch(Exception ex)
            {
                throw ex;
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
            string str = LocExtension.GetLocalizedValue<string>("Cognitivo:local:" + "Question_Cancel");
            if (MessageBox.Show(str, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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

        public object GetClone(object obj,Type a)
        {   
            var source = obj;
            var clone = Activator.CreateInstance(a);
            
            base.Entry(clone).State = EntityState.Added;

            var sourceValues = base.Entry(source).CurrentValues;
            base.Entry(clone).CurrentValues.SetValues(sourceValues);
            return clone;
        }
    }
}
