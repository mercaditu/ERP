using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Linq;
namespace entity
{
    public partial class ProductTransferDB : BaseDB
    {
        public override int SaveChanges()
        {
            validate_ProductTransfer();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_ProductTransfer();
            return base.SaveChangesAsync();
        }

        private void validate_ProductTransfer()
        {
            foreach (item_transfer item_transfer in base.item_transfer.Local)
            {
                if (item_transfer.IsSelected)
                // && item_transfer.Error == null)
                {
                    if (item_transfer.State == EntityState.Added)
                    {
                        item_transfer.timestamp = DateTime.Now;
                        item_transfer.State = EntityState.Unchanged;
                        Entry(item_transfer).State = EntityState.Added;
                    }
                    else if (item_transfer.State == EntityState.Modified)
                    {
                        item_transfer.timestamp = DateTime.Now;
                        item_transfer.State = EntityState.Unchanged;
                        Entry(item_transfer).State = EntityState.Modified;
                    }
                    else if (item_transfer.State == EntityState.Deleted)
                    {
                        item_transfer.timestamp = DateTime.Now;
                        item_transfer.State = EntityState.Unchanged;
                        base.item_transfer.Remove(item_transfer);
                    }
                }
                else if (item_transfer.State > 0)
                {
                    if (item_transfer.State != EntityState.Unchanged)
                    {
                        Entry(item_transfer).State = EntityState.Unchanged;
                    }
                }
            }
        }

        public void Approve(decimal cost,int origin ,int dest)
        {
            foreach (item_transfer item_transfer in base.item_transfer.Local)
            {
                if (CurrentSession.Id_Branch == origin)
                {
                    foreach (item_transfer_detail item_transfer_detail in item_transfer.item_transfer_detail)
                    {
                        if (item_transfer_detail.status != Status.Documents_General.Approved && item_transfer.status != Status.Documents_General.Approved)
                        {
                            item_movement item_movement_origin = new item_movement();
                            item_movement_origin.debit = 0;
                            item_movement_origin.credit = item_transfer_detail.quantity_origin;
                            item_movement_origin.id_application = global::entity.App.Names.Transfer;
                            item_movement_origin.id_location = base.app_location.Where(x=>x.id_branch==origin).FirstOrDefault().id_location;
                            item_movement_origin.transaction_id = 0;
                            item_movement_origin.status = Status.Stock.InStock;
                            item_movement_origin.trans_date = item_transfer_detail.item_transfer.trans_date;
                            if (item_transfer_detail.item_product != null)
                            {

                                item_movement_origin.id_item_product = item_transfer_detail.item_product.id_item_product;

                            }

                            if (item_transfer_detail.quantity_origin == 0)
                            {
                                item_movement_value item_movement_value = new item_movement_value();
                                item_movement_value.unit_value = cost / item_transfer_detail.quantity_destination;
                                item_movement_value.id_currencyfx = 0;
                                item_movement_value.comment = String.Format("Transaction from transfer");
                                item_movement_origin.item_movement_value.Add(item_movement_value);
                            }


                            base.item_movement.Add(item_movement_origin);
                            item_transfer_detail.status = Status.Documents_General.Approved;
                        }


                    }
                }

                if (CurrentSession.Id_Branch == dest)
                {
                    foreach (item_transfer_detail item_transfer_detail in item_transfer.item_transfer_detail)
                    {
                        if (item_transfer_detail.status != Status.Documents_General.Approved && item_transfer.status != Status.Documents_General.Approved)
                        {
                            item_movement item_movement_dest = new item_movement();
                            item_movement_dest.debit = 0;
                            item_movement_dest.credit = item_transfer_detail.quantity_origin;
                            item_movement_dest.id_application = global::entity.App.Names.Transfer;
                            item_movement_dest.id_location = base.app_location.Where(x => x.id_branch == dest).FirstOrDefault().id_location; ;
                            item_movement_dest.transaction_id = 0;
                            item_movement_dest.status = Status.Stock.InStock;
                            item_movement_dest.trans_date = item_transfer_detail.item_transfer.trans_date;
                            if (item_transfer_detail.item_product != null)
                            {

                                item_movement_dest.id_item_product = item_transfer_detail.item_product.id_item_product;

                            }

                            base.item_movement.Add(item_movement_dest);
                            item_transfer_detail.status = Status.Documents_General.Approved;

                        }


                    }
                    item_transfer.status = Status.Documents_General.Approved;
                    entity.Brillo.Document.Start.Automatic(item_transfer, item_transfer.app_document_range);
                }
            }
            base.SaveChanges();
        }
    }
}
