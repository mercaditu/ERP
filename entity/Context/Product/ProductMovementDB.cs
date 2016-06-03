using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace entity
{
    public partial class ProductMovementDB : BaseDB
    {
        public override int SaveChanges()
        {
            //validate_ProductMovement();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            //validate_ProductMovement();
            return base.SaveChangesAsync();
        }

        private void validate_ProductMovement()
        {
            foreach (item_movement item_movement in base.item_movement.Local)
            {
                if (item_movement.IsSelected)
                {
                    if (item_movement.State == EntityState.Added)
                    {
                        item_movement.timestamp = DateTime.Now;
                        item_movement.State = EntityState.Unchanged;
                        Entry(item_movement).State = EntityState.Added;
                    }
                    else if (item_movement.State == EntityState.Modified)
                    {
                        item_movement.timestamp = DateTime.Now;
                        item_movement.State = EntityState.Unchanged;
                        Entry(item_movement).State = EntityState.Modified;
                    }
                    else if (item_movement.State == EntityState.Deleted)
                    {
                        item_movement.timestamp = DateTime.Now;
                        item_movement.State = EntityState.Unchanged;
                        base.item_movement.Remove(item_movement);
                    }
                }
                else if (item_movement.State > 0)
                {
                    if (item_movement.State != EntityState.Unchanged)
                    {
                        Entry(item_movement).State = EntityState.Unchanged;
                    }
                }
            }
        }

        //List<item_movement> item_movementcredit = base.item_movement.OrderBy(x => new { x.trans_date, x.credit }).Where(x=> x.credit > 0).ToList();
        //List<item_movement> item_movementdebit = base.item_movement.OrderBy(x => new { x.trans_date, x.credit }).Where(x => x.debit > 0).ToList();

        // foreach (item_movement _item_movementdebit in item_movementdebit)
        //{
        //    foreach (item_movement _item_movementcredit in item_movementcredit)
        //    {

        //    }
        //}

        public void ReArrange_ProductMovement()
        {
            List<app_location> app_locationList = app_location.Where(x => x.id_company == CurrentSession.Id_Company).ToList();
            List<item_product> item_productList = item_product.Where(x => x.id_company == CurrentSession.Id_Company).ToList();

            foreach (app_location location in app_locationList)
            {
                foreach (item_product item in item_productList)
                {
                    List<item_movement> movement = item_movement
                        .Where(x => x.id_company == CurrentSession.Id_Company && x.id_location == location.id_location && x.id_item_product == item.id_item_product)
                        .OrderBy(x => x.trans_date).ToList();

                    foreach (item_movement credit_movement in movement.Where(x => x.credit > 0))
                    {
                        decimal credit = credit_movement.credit;

                        foreach (item_movement debit_movement in movement.Where(x => x.debit > 0 && x.is_read == false))
                        {
                            debit_movement.is_read = true;

                            item_movement new_debit_movement = new item_movement
                            {
                                id_location = debit_movement.id_location,
                                id_item_product = debit_movement.id_item_product,
                                status = debit_movement.status,
                                debit = debit_movement.debit,
                                credit = debit_movement.credit,
                                comment = debit_movement.comment,
                                code = debit_movement.code,
                                expire_date = debit_movement.expire_date,
                                trans_date = debit_movement.trans_date,
                                id_company = debit_movement.id_company,
                                timestamp = debit_movement.timestamp,
                                id_transfer_detail = debit_movement.item_transfer_detail != null ? debit_movement.id_transfer_detail : null,
                                id_purchase_invoice_detail = debit_movement.purchase_invoice_detail != null ? debit_movement.id_purchase_invoice_detail : null,
                                id_purchase_return_detail = debit_movement.purchase_return_detail != null ? debit_movement.id_purchase_return_detail : null,
                                id_sales_invoice_detail = debit_movement.sales_invoice_detail != null ? debit_movement.id_sales_invoice_detail : null,
                                id_sales_packing_detail = debit_movement.sales_packing_detail != null ? debit_movement.id_sales_packing_detail : null,
                                id_sales_return_detail = debit_movement.sales_return_detail != null ? debit_movement.id_sales_return_detail : null,
                                id_inventory_detail = debit_movement.id_inventory_detail != null ? debit_movement.id_inventory_detail : null,
                                id_execution_detail = debit_movement.production_execution_detail != null ? debit_movement.id_execution_detail : null,
                                is_read = debit_movement.is_read
                            };

                            base.item_movement.Remove(debit_movement);
                            credit_movement._child.Add(new_debit_movement);

                            decimal Value_CreditMovement = credit_movement.item_movement_value.Sum(x => x.unit_value);

                            if (Value_CreditMovement >= 0)
                            {
                                //if (credit_movement.id_inventory_detail > 0 && Value_CreditMovement == 0)
                                //{
                                //    //Check Value of other Inventory that does have value.

                                //        Value_CreditMovement = base.item_inventory_detail
                                //            .Where(x => x.id_item_product == credit_movement.id_item_product &&
                                //          x.status == Status.Documents.Issued &&
                                //          x.unit_value > 0).FirstOrDefault().unit_value;



                                //    }


                                if (credit_movement.item_movement_value.FirstOrDefault() != null)
                                {
                                    //Insert Cost into DebitMovement
                                    item_movement_value item_movement_value = new entity.item_movement_value
                                    {
                                        unit_value = credit_movement.item_movement_value.Sum(x => x.unit_value),
                                        id_currencyfx = credit_movement.item_movement_value.FirstOrDefault().id_currencyfx,
                                        comment = "Base Cost",
                                        timestamp = debit_movement.timestamp
                                    };
                                    new_debit_movement.item_movement_value.Add(item_movement_value);
                                }
                                else
                                {
                                    item_movement Parentitem_movement = base.item_movement.Where(x => x._parent.id_movement == credit_movement.id_movement).FirstOrDefault();
                                    if (Parentitem_movement != null)
                                    {
                                        if (Parentitem_movement.item_movement_value.FirstOrDefault() != null)
                                        {
                                            item_movement_value item_movement_value = new entity.item_movement_value
                                            {
                                                unit_value = Parentitem_movement.item_movement_value.Sum(x => x.unit_value),
                                                id_currencyfx = Parentitem_movement.item_movement_value.FirstOrDefault().id_currencyfx,
                                                comment = "Base Cost",
                                                timestamp = debit_movement.timestamp
                                            };
                                            new_debit_movement.item_movement_value.Add(item_movement_value);
                                        }   
                                    }
                                }

                                ///This will add cost of the Value into Sales Invoice for quick calculations.
                                if (new_debit_movement.sales_invoice_detail != null)
                                { new_debit_movement.sales_invoice_detail.unit_cost = Value_CreditMovement; }
                            }

                            ///Breaks DebitLoop and continues with CreditLoop. 
                            ///This allows us to only consume Debit transactions that equals the credit value.
                            credit -= new_debit_movement.debit;

                            if (credit <= 0)
                            {
                                break;
                            }
                        }
                    }
                }
                //Saves Changes
                SaveChanges();
            }
        }
    }
}
