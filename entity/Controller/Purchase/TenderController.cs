using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace entity.Controller.Purchase
{
    public class TenderController: Base
    {
        public async void Load()
        {
            await db.purchase_tender.Where(a => a.id_company == CurrentSession.Id_Company)
                .Include(x => x.project)
                .OrderByDescending(x => x.trans_date).LoadAsync();
            await db.app_department.Where(b => b.is_active && b.id_company == CurrentSession.Id_Company).OrderBy(b => b.name).ToListAsync();
            await db.app_currencyfx.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).ToListAsync();
            await db.app_dimension.Where(a => a.id_company == CurrentSession.Id_Company).LoadAsync();
            await db.app_measurement.Where(a => a.id_company == CurrentSession.Id_Company).LoadAsync();
        }

        public purchase_tender Create(int TransDate_OffSet)
        {
            purchase_tender Tender = new purchase_tender();
            Tender.State = EntityState.Added;
            Tender.IsSelected = true;
            Tender.trans_date = DateTime.Now.AddDays(TransDate_OffSet);
            db.Entry(Tender).State = EntityState.Added;

            return Tender;
        }

        public purchase_tender Edit(purchase_tender Tender)
        {
            Tender.IsSelected = true;
            Tender.State = EntityState.Modified;
            db.Entry(Tender).State = EntityState.Modified;

            return Tender;
        }

        public int SaveChanges_Validate()
        {
            NumberOfRecords = 0;

            foreach (purchase_tender purchase_tender in db.purchase_tender.Local.Where(x => x.IsSelected))
            {
                if (purchase_tender.IsSelected)
                {
                    if (purchase_tender.State == EntityState.Added)
                    {
                        purchase_tender.timestamp = DateTime.Now;
                        purchase_tender.State = EntityState.Unchanged;
                        db.Entry(purchase_tender).State = EntityState.Added;
                    }
                    else if (purchase_tender.State == EntityState.Modified)
                    {
                        purchase_tender.timestamp = DateTime.Now;
                        purchase_tender.State = EntityState.Unchanged;
                        db.Entry(purchase_tender).State = EntityState.Modified;
                    }

                    NumberOfRecords += 1;
                }
                else if (purchase_tender.State > 0)
                {
                    if (purchase_tender.State != EntityState.Unchanged)
                    {
                        db.Entry(purchase_tender).State = EntityState.Unchanged;
                    }
                }
            }

            return NumberOfRecords;
        }

        public void Archive()
        {

        }

        public void Approve()
        {
            NumberOfRecords = 0;

            foreach (purchase_tender purchase_tender in db.purchase_tender.Local.Where(x => x.IsSelected == true))
            {
                if (purchase_tender.id_purchase_tender == 0)
                {
                    SaveChanges_Validate();
                }

                if (purchase_tender.status != Status.Documents_General.Approved)
                {
                    foreach (purchase_tender_contact purchase_tender_contact in purchase_tender.purchase_tender_contact_detail)
                    {
                        purchase_order purchase_order = new purchase_order()
                        {
                            id_purchase_tender = purchase_tender.id_purchase_tender,
                            id_department = purchase_tender.id_department,
                            id_currencyfx = purchase_tender_contact.id_currencyfx,
                            recieve_date_est = purchase_tender_contact.recieve_date_est,
                            id_contact = purchase_tender_contact.id_contact,
                            contact = purchase_tender_contact.contact,
                            id_condition = purchase_tender_contact.id_condition,
                            id_contract = purchase_tender_contact.id_contract,
                            id_project = purchase_tender.id_project
                        };

                        ///Don't approve if there is nothing selected. Sometimes Users make mistakes.
                        foreach (purchase_tender_item item in purchase_tender.purchase_tender_item_detail)
                        {
                            List<purchase_tender_detail> purchase_tender_detailList = purchase_tender_contact.purchase_tender_detail.Where(x => x.IsSelected && x.id_purchase_tender_item == item.id_purchase_tender_item).ToList();
                            foreach (purchase_tender_detail purchase_tender_detail in purchase_tender_detailList)
                            {
                                purchase_order_detail purchase_order_detail = new purchase_order_detail()
                                {
                                    purchase_tender_detail = purchase_tender_detail,
                                    id_purchase_tender_detail = purchase_tender_detail.id_purchase_tender_detail,
                                    unit_cost = purchase_tender_detail.unit_cost,
                                    id_vat_group = purchase_tender_detail.id_vat_group
                                };

                                if (purchase_tender_detail.purchase_tender_item.item != null)
                                {
                                    purchase_order_detail.item = purchase_tender_detail.purchase_tender_item.item;
                                    purchase_order_detail.id_item = purchase_tender_detail.purchase_tender_item.id_item;
                                    purchase_order_detail.item_description = purchase_tender_detail.purchase_tender_item.item_description;

                                    app_cost_center app_cost_center = db.app_cost_center.Where(x => x.is_active == true && x.is_product).FirstOrDefault();
                                    if (app_cost_center != null)
                                    {
                                        purchase_order_detail.id_cost_center = app_cost_center.id_cost_center;
                                    }
                                    else
                                    {
                                        app_cost_center = new app_cost_center();
                                        app_cost_center.name = "Merchandice";
                                        app_cost_center.is_product = true;
                                        db.app_cost_center.Add(app_cost_center);
                                        purchase_order_detail.app_cost_center = app_cost_center;
                                    }
                                }
                                else
                                {
                                    purchase_order_detail.item_description = purchase_tender_detail.purchase_tender_item.item_description;

                                    app_cost_center app_cost_center = db.app_cost_center.Where(x => x.is_active == true && x.is_administrative).FirstOrDefault();
                                    if (app_cost_center != null)
                                    {
                                        purchase_order_detail.id_cost_center = app_cost_center.id_cost_center;
                                    }
                                    else
                                    {
                                        app_cost_center = new app_cost_center();
                                        app_cost_center.name = "Administrative";
                                        app_cost_center.is_administrative = true;
                                        db.app_cost_center.Add(app_cost_center);
                                        purchase_order_detail.app_cost_center = app_cost_center;
                                    }
                                }

                                /// We need to check the quantity ordered is not greater than quantity required from this supplier.
                                /// If so, then use Order Quantity, which is a partial amount.
                                /// If not, then get out of this code, and go for the next loop.

                                decimal OrderedQuantity = purchase_tender_detail.purchase_order_detail.Where(x => x.purchase_order.status != Status.Documents_General.Annulled).Sum(x => x.quantity);

                                if (OrderedQuantity < purchase_tender_detail.quantity)
                                {
                                    //Gets balance of remaining amount. Balance can never be 0?
                                    decimal Balance = purchase_tender_detail.quantity - OrderedQuantity;

                                    if (Balance > purchase_tender_detail.OrderQuantity)
                                    { //If balance is greater than Order Quantity. Order the OrderQuantity only. Keep status pending to allow future buying.
                                        purchase_order_detail.quantity = purchase_tender_detail.OrderQuantity;
                                    }
                                    else if (Balance == purchase_tender_detail.OrderQuantity)
                                    {
                                        //If Quantity is exactly the same, then use quantity and approve, to stop further buying.
                                        purchase_order_detail.quantity = purchase_tender_detail.OrderQuantity;
                                        purchase_tender_detail.status = Status.Documents_General.Approved;
                                    }
                                    else
                                    { //If balance is smaller than OrdeRQuantity, Order the Balance only. Approve to stop further buying.
                                        purchase_order_detail.quantity = purchase_tender_detail.OrderQuantity - Balance;
                                        purchase_tender_detail.status = Status.Documents_General.Approved;
                                    }
                                }
                                else
                                {
                                    continue;
                                }

                                foreach (purchase_tender_dimension purchase_tender_dimension in purchase_tender_detail.purchase_tender_item.purchase_tender_dimension)
                                {
                                    purchase_order_dimension purchase_order_dimension = new purchase_order_dimension()
                                    {
                                        id_company = CurrentSession.Id_Company,
                                        id_dimension = purchase_tender_dimension.id_dimension,
                                        id_measurement = purchase_tender_dimension.id_measurement,
                                        value = purchase_tender_dimension.value
                                    };

                                    purchase_order_detail.purchase_order_dimension.Add(purchase_order_dimension);
                                }

                                purchase_order.purchase_order_detail.Add(purchase_order_detail);
                                purchase_tender_detail.IsSelected = false;
                            }
                        }

                        if (purchase_order.purchase_order_detail.Count() > 0)
                        {
                            NumberOfRecords += 1;
                            db.purchase_order.Add(purchase_order);
                        }
                    }

                    if (purchase_tender.id_range != null || purchase_tender.id_range > 0)
                    {
                        app_document_range app_document_range = db.app_document_range.Find(purchase_tender.id_range);
                        purchase_tender.number = Brillo.Logic.Range.calc_Range(app_document_range, true);
                        purchase_tender.RaisePropertyChanged("number");
                    }
                    else
                    {
                        app_document_range app_document_range = db.app_document_range.Where(x => x.app_document.id_application == App.Names.PurchaseTender && x.is_active).FirstOrDefault();
                        if (app_document_range != null)
                        {
                            purchase_tender.id_range = app_document_range.id_range;
                            purchase_tender.number = Brillo.Logic.Range.calc_Range(app_document_range, true);
                            purchase_tender.RaisePropertyChanged("number");
                        }
                    }

                    //Block this.
                    //purchase_tender.status = Status.Documents_General.Approved;

                    SaveChanges_Validate();

                    purchase_tender.IsSelected = false;
                }
            }

            foreach (purchase_tender tender in db.purchase_tender.Local.Where(x => x.IsSelected && x.status == Status.Documents_General.Pending))
            {
                tender.status = Status.Documents_General.Approved;
            }

            db.SaveChanges();
        }

        public void Annull()
        {
            foreach (purchase_tender purchase_tender in db.purchase_tender.Local.Where(x => x.IsSelected == true))
            {
                if (purchase_tender.status == Status.Documents_General.Approved)
                {
                    foreach (purchase_tender_contact purchase_tender_contact in purchase_tender.purchase_tender_contact_detail)
                    {
                        foreach (purchase_tender_detail purchase_tender_detail in purchase_tender_contact.purchase_tender_detail.Where(x => x.status == Status.Documents_General.Approved))
                        {
                            if (purchase_tender_detail.purchase_order_detail != null)
                            {
                                if (purchase_tender_detail.purchase_order_detail.FirstOrDefault().purchase_order.status == Status.Documents_General.Pending)
                                {
                                    //Incase Order does not have Invoice, Annull Order
                                    if (purchase_tender_detail.purchase_order_detail.FirstOrDefault().purchase_order.purchase_invoice == null)
                                    {
                                        purchase_tender_detail.purchase_order_detail.FirstOrDefault().purchase_order.status = Status.Documents_General.Annulled;
                                    }

                                    purchase_tender_detail.status = Status.Documents_General.Annulled;
                                    purchase_tender.status = Status.Documents_General.Annulled;
                                }
                                else
                                {
                                    purchase_tender.status = Status.Documents_General.Approved;
                                    purchase_tender_detail.status = Status.Documents_General.Approved;
                                }
                            }
                            else //Order is Null
                            {
                                //If no Order Exists, then just Annull directly.
                                purchase_tender_detail.status = Status.Documents_General.Annulled;
                                purchase_tender.status = Status.Documents_General.Annulled;
                            }
                        }
                    }
                }
            }

            SaveChanges_Validate();
        }
    }
}
