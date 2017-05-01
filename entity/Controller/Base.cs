using entity.Brillo;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace entity.Controller
{
    public class Base
    {
        public db db { get; set; }

        public int NumberOfRecords;

        public DateTime Start_Range
        {
            get { return _start_Range; }
            set
            {
                if (_start_Range != value)
                {
                    _start_Range = value;
                }
            }
        }
        private DateTime _start_Range = DateTime.Now.AddDays(-7);

        public DateTime End_Range
        {
            get { return _end_Range; }
            set
            {
                if (_end_Range != value)
                {
                    _end_Range = value;
                }
            }
        }
        private DateTime _end_Range = DateTime.Now.AddDays(+1);

        /// <summary>
        /// Initilize the Context.
        /// </summary>
        public void Initialize()
        {
            db = new db();
        }
       

        /// <summary>
        /// Cancel Changes by Asking Question First.
        /// </summary>
        /// <returns></returns>
        public bool CancelAllChanges()
        {
            if (MessageBox.Show(Localize.Question_Cancel, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                foreach (var entry in db.ChangeTracker.Entries())
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

            return true;
        }
    }
}
