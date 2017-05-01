using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace entity.Controller.Production
{
    public class OrderController : Base
    {
       

        public async void Load(production_order.ProductionOrderTypes ProductionOrderTypes)
        {
            if (ProductionOrderTypes==production_order.ProductionOrderTypes.Fraction)
            {
                await db.production_order.Where(a => a.id_company == CurrentSession.Id_Company && a.type == ProductionOrderTypes && a.production_line.app_location.id_branch == CurrentSession.Id_Branch).OrderByDescending(x => x.trans_date).LoadAsync();
            }
            else
            {
                await db.production_order.Where(a => a.id_company == CurrentSession.Id_Company && (a.type == ProductionOrderTypes || a.type == production_order.ProductionOrderTypes.Internal) && a.production_line.app_location.id_branch == CurrentSession.Id_Branch).OrderByDescending(x => x.trans_date).LoadAsync();
            }
            
            await db.production_line.Where(x => x.id_company == CurrentSession.Id_Company && x.app_location.id_branch == CurrentSession.Id_Branch).LoadAsync();
            await db.hr_time_coefficient.Where(x => x.id_company == CurrentSession.Id_Company).LoadAsync();
            await db.app_dimension.Where(a => a.id_company == CurrentSession.Id_Company).LoadAsync();
            await db.app_measurement.Where(a => a.id_company == CurrentSession.Id_Company).LoadAsync();
        }

        public production_order Create_Fraction(int Line)
        {
            production_order Order = new production_order()
            {
                State = EntityState.Added,
                status = Status.Production.Pending,
                type = production_order.ProductionOrderTypes.Fraction,
                IsSelected = true
            };

            db.production_order.Add(Order);

            return Order;
        }

        public production_order Create_Normal(int Line)
        {
            production_order Order = new production_order()
            {
                State = EntityState.Added,
                status = Status.Production.Pending,
                type = production_order.ProductionOrderTypes.Production,
                IsSelected = true
            };

            db.production_order.Add(Order);

            return Order;
        }

        public void CreateDetail()
        {

        }

        public bool Edit(production_order Order)
        {
            Order.IsSelected = true;
            Order.State = EntityState.Modified;
            db.Entry(Order).State = EntityState.Modified;

            return true;
        }

        public void EditDetail(production_order_detail Detail)
        {

        }

        public bool Archive()
        {
            MessageBoxResult res = MessageBox.Show(Brillo.Localize.Question_Delete, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                foreach (production_order Order in db.production_order.Local.Where(x => x.IsSelected))
                {
                    Order.is_archived = true;
                }
            }

            db.SaveChanges();
            return true;
        }

        public void Approve()
        {

        }

        public void ApproveDetail()
        {

        }

        public void AnnullDetail()
        {

        }

        public bool SaveChanges_WithValidation()
        {
            NumberOfRecords = 0;

            foreach (production_order production_order in db.production_order.Local.Where(x => x.id_production_line > 0))
            {
                if (production_order.IsSelected && production_order.Error == null)
                {
                    if (production_order.State == EntityState.Added)
                    {
                        production_order.timestamp = DateTime.Now;
                        production_order.State = EntityState.Unchanged;
                        db.Entry(production_order).State = EntityState.Added;
                    }
                    else if (production_order.State == EntityState.Modified)
                    {
                        production_order.timestamp = DateTime.Now;
                        production_order.State = EntityState.Unchanged;
                        db.Entry(production_order).State = EntityState.Modified;
                    }
                    else if (production_order.State == EntityState.Deleted)
                    {
                        production_order.timestamp = DateTime.Now;
                        production_order.State = EntityState.Unchanged;
                        db.production_order.Remove(production_order);
                    }
                    NumberOfRecords += 1;
                }
                else if (production_order.State > 0)
                {
                    if (production_order.State != EntityState.Unchanged)
                    {
                        db.Entry(production_order).State = EntityState.Unchanged;
                    }
                }
            }

            foreach (var error in db.GetValidationErrors())
            {
                db.Entry(error.Entry.Entity).State = EntityState.Detached;
            }

            if (db.GetValidationErrors().Count() > 0)
            {
                return false;
            }
            else
            {
                db.SaveChanges();
                return true;
            }
        }
    }
}
