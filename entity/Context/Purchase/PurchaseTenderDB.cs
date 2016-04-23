using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity
{
    public partial class PurchaseTenderDB : BaseDB
    {
        public override int SaveChanges()
        {
            validate_Order();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Order();
            return base.SaveChangesAsync();
        }

        private void validate_Order()
        {
            foreach (purchase_tender purchase_tender in base.purchase_tender.Local)
            {
                if (purchase_tender.IsSelected)
                {
                    if (purchase_tender.State == EntityState.Added)
                    {
                        purchase_tender.timestamp = DateTime.Now;
                        purchase_tender.State = EntityState.Unchanged;
                        Entry(purchase_tender).State = EntityState.Added;
                    }
                    else if (purchase_tender.State == EntityState.Modified)
                    {
                        purchase_tender.timestamp = DateTime.Now;
                        purchase_tender.State = EntityState.Unchanged;
                        Entry(purchase_tender).State = EntityState.Modified;
                    }
                    else if (purchase_tender.State == EntityState.Deleted)
                    {
                        purchase_tender.timestamp = DateTime.Now;
                        purchase_tender.State = EntityState.Unchanged;
                        base.purchase_tender.Remove(purchase_tender);
                    }
                }
                else if (purchase_tender.State > 0)
                {
                    if (purchase_tender.State != EntityState.Unchanged)
                    {
                        Entry(purchase_tender).State = EntityState.Unchanged;
                    }
                }
            }
        }
        public void Approve()
        {
            foreach (purchase_tender purchase_tender in base.purchase_tender.Local.Where(x => x.IsSelected == true))
            {

                if (purchase_tender.id_purchase_tender == 0)
                {
                    SaveChanges();
                }

                if (purchase_tender.status != Status.Documents_General.Approved)
                {
                    foreach (purchase_tender_contact purchase_tender_contact in purchase_tender.purchase_tender_contact_detail)
                    {
                        purchase_order purchase_order = new purchase_order();

                        purchase_order.id_purchase_tender = purchase_tender.id_purchase_tender;
                        purchase_order.id_department = purchase_tender.id_department;
                        purchase_order.id_currencyfx = purchase_tender_contact.id_currencyfx;
                        purchase_order.recieve_date_est = purchase_tender_contact.recieve_date_est;
                        if (base.app_document_range.Where(x => x.app_document.id_application == App.Names.PurchaseOrder).FirstOrDefault() != null)
                        {
                            purchase_order.id_range = base.app_document_range.Where(x => x.app_document.id_application == App.Names.PurchaseOrder).FirstOrDefault().id_range;
                            app_document_range app_document_range = base.app_document_range.Where(x => x.id_range == purchase_order.id_range).FirstOrDefault();
                            purchase_order.number = Brillo.Logic.Range.calc_Range(app_document_range, true);
                            purchase_order.RaisePropertyChanged("number");
                        }

                        purchase_order.id_contact = purchase_tender_contact.id_contact;
                        purchase_order.contact = purchase_tender_contact.contact;
                        purchase_order.id_contract = purchase_tender_contact.id_contract;
                        purchase_order.id_condition = purchase_tender_contact.id_condition;
                        purchase_order.id_project = purchase_tender.id_project;
                        purchase_order.project = purchase_tender.project;
                        foreach (purchase_tender_detail purchase_tender_detail in purchase_tender_contact.purchase_tender_detail.Where(x=>x.IsSelected))
                        {
                            purchase_order_detail purchase_order_detail = new purchase_order_detail();
                            purchase_order_detail.purchase_tender_detail = purchase_tender_detail;
                            purchase_order_detail.id_purchase_tender_detail = purchase_tender_detail.id_purchase_tender_detail;
                            purchase_order_detail.item = purchase_tender_detail.purchase_tender_item.item;
                            purchase_order_detail.id_item = purchase_tender_detail.purchase_tender_item.id_item;
                            purchase_order_detail.unit_cost = purchase_tender_detail.unit_cost;
                            if (purchase_tender_detail.item_description=="")
                            {
                                purchase_order_detail.item_description = purchase_tender_detail.item_description;
                            }
                            else
                            {
                                purchase_order_detail.item_description = purchase_tender_detail.purchase_tender_item.item.name;
                            }
                            purchase_order_detail.quantity = purchase_tender_detail.quantity;
                            if (base.app_vat_group.Where(x => x.is_default == true).FirstOrDefault() != null)
                            {
                                purchase_order_detail.id_vat_group = base.app_vat_group.Where(x => x.is_default == true).FirstOrDefault().id_vat_group;
                            }
                            else
                            {
                                purchase_order_detail.id_vat_group = base.app_vat_group.Where(x => x.is_active == true).FirstOrDefault().id_vat_group;
                            }
                            purchase_order_detail.id_cost_center = base.app_cost_center.Where(x => x.is_active == true).FirstOrDefault().id_cost_center;

                            purchase_order.purchase_order_detail.Add(purchase_order_detail);
                            purchase_tender_detail.status = Status.Documents_General.Approved;
                        }
                        base.purchase_order.Add(purchase_order);
                    }


                    purchase_tender.status = Status.Documents_General.Approved;
                    SaveChanges();
                }


            }
        }
        public void Anull()
        { }
    }
}
