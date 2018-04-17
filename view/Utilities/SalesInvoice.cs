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
                db.sales_invoice_detail.Where(x => x.sales_invoice.status == Status.Documents_General.Approved)
                    .Include(z => z.sales_invoice)
                    .Include(y => y.item_movement)
                    .LoadAsync();

                foreach (sales_invoice_detail detail in db.sales_invoice_detail.Local)
                {
                    item_movement item_movement = detail.item_movement.FirstOrDefault();
                    if (item_movement != null)
                    {
                        if (item_movement.id_item_product == 240)
                        {
                            var iv = 1;
                            iv += iv;
                        }
                        if (item_movement.item_movement_value_rel != null)
                        {
                            detail.unit_cost = Currency.convert_Values
                                (
                                item_movement.item_movement_value_rel.item_movement_value_detail.Sum(x => x.unit_value),
                                CurrentSession.Get_Currency_Default_Rate().id_currencyfx,
                                detail.sales_invoice.id_currencyfx,
                                entity.App.Modules.Sales
                                );

                            i += 1;
                        }
                    }
                    else
                    {
                        if (detail.sales_packing_relation.Count() > 0)
                        {
                            if (detail.sales_packing_relation.FirstOrDefault().sales_packing_detail != null)
                            {
                                if (detail.sales_packing_relation.FirstOrDefault().sales_packing_detail.item_movement.FirstOrDefault() != null)
                                {
                                    item_movement = detail.sales_packing_relation.FirstOrDefault().sales_packing_detail.item_movement.FirstOrDefault();
                                    if (item_movement.item_movement_value_rel != null)
                                    {
                                        detail.unit_cost = Currency.convert_Values
                                            (
                                            item_movement.item_movement_value_rel.item_movement_value_detail.Sum(x => x.unit_value),
                                            CurrentSession.Get_Currency_Default_Rate().id_currencyfx,
                                            detail.sales_invoice.id_currencyfx,
                                            entity.App.Modules.Sales
                                            );

                                        i += 1;
                                    }
                                }
                            }

                        }
                        
                    }


                }

                db.SaveChanges();
            }

            return i;
        }
    }
}
