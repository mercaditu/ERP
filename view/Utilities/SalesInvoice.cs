using System.Linq;
using entity;
using System.Data.Entity;
using entity.Brillo;

namespace Cognitivo.Utilities
{
    public class SalesInvoice
    {
        public int Update_SalesCost()
        {
            int i = 0;

            using (db db = new db())
            {
                //Loads into memory for increased spead.
                db.sales_invoice_detail.Where(x => x.unit_cost == 0 && x.sales_invoice.status == Status.Documents_General.Approved)
                    .Include(z => z.sales_invoice)
                    .Include(y => y.item_movement)
                    .LoadAsync();

                foreach (sales_invoice_detail detail in db.sales_invoice_detail.Local)
                {
                    item_movement item_movement = detail.item_movement.FirstOrDefault();
                    if (item_movement != null)
                    {
                        if (item_movement.item_movement_value.Count() > 0)
                        {
                            detail.unit_cost = Currency.convert_Values
                                (
                                item_movement.item_movement_value.Sum(x => x.unit_value),
                                item_movement.item_movement_value.FirstOrDefault().id_currencyfx,
                                detail.sales_invoice.id_currencyfx,
                                entity.App.Modules.Sales
                                );

                            i += 1;
                        }
                    }
                }

                db.SaveChangesAsync();
            }

            return i;
        }
    }
}
