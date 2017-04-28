using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace entity.Controller.Production
{
    public class OrderController
    {
        public db db { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {
            db = new db();
        }

        public async void Load()
        {
            await db.production_order.Where(a => a.id_company == CurrentSession.Id_Company && a.type == production_order.ProductionOrderTypes.Fraction && a.production_line.app_location.id_branch == CurrentSession.Id_Branch).LoadAsync();
            await db.production_line.Where(x => x.id_company == CurrentSession.Id_Company && x.app_location.id_branch == CurrentSession.Id_Branch).LoadAsync();
            await db.hr_time_coefficient.Where(x => x.id_company == CurrentSession.Id_Company).LoadAsync();
        }

        public production_order Create()
        {
            production_order Order = new production_order();
            Order.State = EntityState.Added;
            Order.status = Status.Production.Pending;
            Order.type = production_order.ProductionOrderTypes.Fraction;
            Order.IsSelected = true;
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
    }
}
