using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using entity;

namespace cntrl.Class
{
   public class MovementReApprove
    {
        public bool Start(db db, int ID, entity.App.Names Application)
        {
            return true;



        }
        public void ValueChange(db db, int ID, entity.App.Names Application)
        {
            sales_invoice Oldsales_invoice = db.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
            sales_invoice sales_invoice = db.sales_invoice.Find(ID);
            foreach (sales_invoice_detail sales_invoice_detail in sales_invoice.sales_invoice_detail)
            {
                sales_invoice_detail Oldsales_invoice_detail = Oldsales_invoice.sales_invoice_detail.Where(x => x == sales_invoice_detail).FirstOrDefault();
                if (Oldsales_invoice_detail!=null)
                {
                    if (sales_invoice_detail.unit_price!= Oldsales_invoice_detail.unit_price)
                    {
                        foreach (item_movement item_movement in sales_invoice_detail.item_movement)
                        {
                            item_movement_value item_movement_value = item_movement.item_movement_value.FirstOrDefault();
                            if (item_movement_value!=null)
                            {
                                item_movement_value.unit_value = sales_invoice_detail.unit_price;
                            }
                        }
                    }
                }
            }
              
        }
        public void QuantityUP(db db, int ID, entity.App.Names Application)
        {
            sales_invoice Oldsales_invoice = db.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
            sales_invoice sales_invoice = db.sales_invoice.Find(ID);
            foreach (sales_invoice_detail sales_invoice_detail in sales_invoice.sales_invoice_detail)
            {
                sales_invoice_detail Oldsales_invoice_detail = Oldsales_invoice.sales_invoice_detail.Where(x => x == sales_invoice_detail).FirstOrDefault();
                if (Oldsales_invoice_detail != null)
                {
                    if (sales_invoice_detail.quantity != Oldsales_invoice_detail.quantity)
                    {
                        decimal Diff = sales_invoice_detail.quantity - Oldsales_invoice_detail.quantity;
                        foreach (item_movement item_movement in sales_invoice_detail.item_movement)
                        {

                            if (item_movement.parent==null)
                            {
                                item_movement.debit = sales_invoice_detail.quantity;
                            }
                            else
                            {
                               
                            }
                        }
                    }
                }
            }

        }
    }
}
