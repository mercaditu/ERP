using System.Collections.Generic;
using System.Linq;

namespace entity.Brillo.Logic
{
    public class Payment
    {
        public List<payment_promissory_note> payment_promissory_noteLIST
        {
            get { return _payment_promissory_noteLIST; }
            set
            {
                if (_payment_promissory_noteLIST != value)
                {
                    _payment_promissory_noteLIST = value;
                }
            }
        }
        private List<payment_promissory_note> _payment_promissory_noteLIST = new List<payment_promissory_note>();

        public List<payment_schedual> insert_Schedual(object obj_entity)
        {
            List<payment_schedual> payment_schedualList = new List<payment_schedual>();

            //SALES INVOICE
            if (obj_entity as sales_invoice != null)
            {
                sales_invoice sales_invoice = (sales_invoice)obj_entity;
                List<app_contract_detail> app_contract_details = null;
                bool IsPromisorry = false;

                if (sales_invoice.app_contract == null)
                {
                    using (db db = new db())
                    {
                        app_contract app_contract = db.app_contract.Find(sales_invoice.id_contract);
                        app_contract_details = app_contract.app_contract_detail.Where(x => x.is_order == false).ToList();
                        IsPromisorry = app_contract.is_promissory;
                    }
                }
                else
                {
                    app_contract_details = sales_invoice.app_contract.app_contract_detail.Where(x => x.is_order == false).ToList();
                    IsPromisorry = sales_invoice.app_contract.is_promissory;
                }

                foreach (app_contract_detail app_contract_detail in app_contract_details)
                {
                    payment_schedual payment_schedual = new payment_schedual();
                    payment_schedual.credit = 0;
                    payment_schedual.debit = sales_invoice.GrandTotal * app_contract_detail.coefficient;
                    payment_schedual.id_currencyfx = sales_invoice.id_currencyfx;
                    payment_schedual.sales_invoice = sales_invoice;
                    payment_schedual.trans_date = sales_invoice.trans_date;
                    payment_schedual.expire_date = sales_invoice.trans_date.AddDays(app_contract_detail.interval);
                    payment_schedual.status = Status.Documents_General.Approved;
                    payment_schedual.id_contact = sales_invoice.id_contact;

                    ///Checks if selected Contract has Promissory Note created.
                    if (IsPromisorry)
                    {
                        payment_promissory_note payment_promissory_note = new payment_promissory_note();
                        //Dates. Transactional (based on Sales Trans) and Expiry (based on Exp of Payment)...
                        payment_promissory_note.trans_date = sales_invoice.trans_date;
                        payment_promissory_note.expiry_date = sales_invoice.trans_date.AddDays(app_contract_detail.interval);
                        //Navigational Properties...
                        payment_promissory_note.id_branch = sales_invoice.id_branch;
                        payment_promissory_note.id_terminal = sales_invoice.id_terminal;
                        payment_promissory_note.id_company = sales_invoice.id_company;
                        payment_promissory_note.id_contact = sales_invoice.id_contact;
                        //Values...
                        payment_promissory_note.value = sales_invoice.GrandTotal * app_contract_detail.coefficient;
                        payment_promissory_note.id_currencyfx = sales_invoice.id_currencyfx;
                        payment_promissory_note.status = Status.Documents.Pending;
                        payment_promissory_noteLIST.Add(payment_promissory_note);

                        //Adding Payment Schedual into PromissoryNote
                        payment_promissory_note.payment_schedual.Add(payment_schedual);
                    }
                    else
                    {
                        payment_schedualList.Add(payment_schedual);
                    }

                }

                return payment_schedualList;
            }

            //SALES RETURN
            else if (obj_entity as sales_return != null)
            {
                sales_return sales_return = (sales_return)obj_entity;

                payment_schedual payment_schedual = new payment_schedual();
                payment_schedual.debit = sales_return.GrandTotal;
                payment_schedual.credit = 0;
                payment_schedual.id_currencyfx = sales_return.id_currencyfx;
                payment_schedual.sales_return = sales_return;
                payment_schedual.trans_date = sales_return.trans_date;
                payment_schedual.expire_date = sales_return.trans_date;
                payment_schedual.status = Status.Documents_General.Approved;
                payment_schedual.id_contact = sales_return.id_contact;
                payment_schedual.can_calculate = false;
                payment_schedualList.Add(payment_schedual);

                return payment_schedualList;
            }

            //PURCHASE RETURN
            else if (obj_entity as purchase_return != null)
            {
                purchase_return purchase_return = (purchase_return)obj_entity;

                payment_schedual payment_schedual = new payment_schedual();
                payment_schedual.credit = purchase_return.GrandTotal;
                payment_schedual.debit = 0;
                payment_schedual.id_currencyfx = purchase_return.id_currencyfx;
                payment_schedual.purchase_return = purchase_return;
                payment_schedual.trans_date = purchase_return.trans_date;
                payment_schedual.expire_date = purchase_return.trans_date;
                payment_schedual.status = Status.Documents_General.Approved;
                payment_schedual.id_contact = purchase_return.id_contact;
                payment_schedual.can_calculate = false;
                payment_schedualList.Add(payment_schedual);

                return payment_schedualList;
            }


            //SALES ORDER
            else if (obj_entity as sales_order != null)
            {
                sales_order sales_order = (sales_order)obj_entity;

                foreach (app_contract_detail app_contract_detail in sales_order.app_contract.app_contract_detail.Where(x => x.is_order))
                {
                    payment_schedual payment_schedual = new payment_schedual();
                    payment_schedual.credit = 0;
                    payment_schedual.debit = sales_order.GrandTotal * app_contract_detail.coefficient;
                    payment_schedual.id_currencyfx = sales_order.id_currencyfx;
                    payment_schedual.sales_order = sales_order;
                    payment_schedual.trans_date = sales_order.trans_date;
                    payment_schedual.expire_date = sales_order.trans_date.AddDays(app_contract_detail.interval);
                    payment_schedual.status = Status.Documents_General.Approved;
                    payment_schedual.id_contact = sales_order.id_contact;
                    payment_schedualList.Add(payment_schedual);
                }
                return payment_schedualList;
            }



            //PURCHASE INVOICE
            else if (obj_entity as purchase_invoice != null)
            {
                purchase_invoice purchase_invoice = (purchase_invoice)obj_entity;
                List<app_contract_detail> app_contract_details = null;
                bool IsPromisorry = false;

                if (purchase_invoice.app_contract == null)
                {
                    using (db db = new db())
                    {
                        app_contract app_contract = db.app_contract.Find(purchase_invoice.id_contract);
                        app_contract_details = app_contract.app_contract_detail.Where(x => x.is_order == false).ToList();
                        IsPromisorry = app_contract.is_promissory;
                    }
                }
                else
                {
                    app_contract_details = purchase_invoice.app_contract.app_contract_detail.Where(x => x.is_order == false).ToList();
                    IsPromisorry = purchase_invoice.app_contract.is_promissory;
                }

                foreach (app_contract_detail app_contract_detail in app_contract_details)
                {
                    payment_schedual payment_schedual = new payment_schedual();
                    payment_schedual.credit = purchase_invoice.GrandTotal * app_contract_detail.coefficient;
                    payment_schedual.debit = 0;
                    payment_schedual.id_currencyfx = purchase_invoice.id_currencyfx;
                    payment_schedual.purchase_invoice = purchase_invoice;
                    payment_schedual.trans_date = purchase_invoice.trans_date;
                    payment_schedual.expire_date = purchase_invoice.trans_date.AddDays(app_contract_detail.interval);
                    payment_schedual.status = Status.Documents_General.Pending;
                    payment_schedual.id_contact = purchase_invoice.id_contact;

                    ///Checks if selected Contract has Promissory Note created.
                    if (IsPromisorry)
                    {
                        payment_promissory_note payment_promissory_note = new payment_promissory_note();
                        //Dates. Transactional (based on Sales Trans) and Expiry (based on Exp of Payment)...
                        payment_promissory_note.trans_date = purchase_invoice.trans_date;
                        payment_promissory_note.expiry_date = purchase_invoice.trans_date.AddDays(app_contract_detail.interval);
                        //Navigational Properties...
                        payment_promissory_note.id_branch = purchase_invoice.id_branch;
                        payment_promissory_note.id_terminal = purchase_invoice.id_terminal;
                        payment_promissory_note.id_company = purchase_invoice.id_company;
                        payment_promissory_note.id_contact = purchase_invoice.id_contact;
                        //Values...
                        payment_promissory_note.value = purchase_invoice.GrandTotal * app_contract_detail.coefficient;
                        payment_promissory_note.id_currencyfx = purchase_invoice.id_currencyfx;
                        payment_promissory_note.status = Status.Documents.Pending;
                        payment_promissory_noteLIST.Add(payment_promissory_note);

                        //Adding Payment Schedual into PromissoryNote
                        payment_promissory_note.payment_schedual.Add(payment_schedual);
                    }
                    else
                    {
                        payment_schedualList.Add(payment_schedual);
                    }
                }
                return payment_schedualList;
            }


            //PURCHASE ORDER
            else if (obj_entity as purchase_order != null)
            {
                purchase_order purchase_order = (purchase_order)obj_entity;

                foreach (app_contract_detail app_contract_detail in purchase_order.app_contract.app_contract_detail.Where(x => x.is_order))
                {
                    payment_schedual payment_schedual = new payment_schedual();
                    payment_schedual.credit = purchase_order.GrandTotal * app_contract_detail.coefficient;
                    payment_schedual.debit = 0;
                    payment_schedual.id_currencyfx = purchase_order.id_currencyfx;
                    payment_schedual.purchase_order = purchase_order;
                    payment_schedual.trans_date = purchase_order.trans_date;
                    payment_schedual.expire_date = purchase_order.trans_date.AddDays(app_contract_detail.interval);
                    payment_schedual.status = Status.Documents_General.Pending;
                    payment_schedual.id_contact = purchase_order.id_contact;
                    payment_schedualList.Add(payment_schedual);
                }
                return payment_schedualList;
            }

            return null;
        }

        public List<payment_schedual> revert_Schedual(object obj_entity)
        {
            List<payment_schedual> payment_schedualList = new List<payment_schedual>();

            //SALES INVOICE
            if (obj_entity as sales_invoice != null)
            {
                sales_invoice sales_invoice = (sales_invoice)obj_entity;
                if (sales_invoice.payment_schedual != null)
                {
                    foreach (payment_schedual payment in sales_invoice.payment_schedual)
                    {
                        payment_schedualList.Add(payment);
                    }
                }
            }
            //SALES ORDER
            else if (obj_entity as sales_order != null)
            {
                sales_order sales_order = (sales_order)obj_entity;
                if (sales_order.payment_schedual != null)
                {
                    foreach (payment_schedual payment in sales_order.payment_schedual)
                    {
                        payment_schedualList.Add(payment);
                    }
                }
            }
            //PURCHASE INVOICE
            else if (obj_entity as purchase_invoice != null)
            {
                purchase_invoice purchase_invoice = (purchase_invoice)obj_entity;
                if (purchase_invoice.payment_schedual != null)
                {
                    foreach (payment_schedual payment in purchase_invoice.payment_schedual)
                    {
                        payment_schedualList.Add(payment);
                    }
                }
            }
            //PURCHASE ORDER
            else if (obj_entity as purchase_order != null)
            {
                //If function to liberate paid amount
                purchase_order purchase_order = (purchase_order)obj_entity;
                if (purchase_order.payment_schedual != null)
                {
                    foreach (payment_schedual payment in purchase_order.payment_schedual)
                    {
                        payment_schedualList.Add(payment);
                    }
                }
            }
            //PURCHASE RETURN
            else if (obj_entity as purchase_return != null)
            {
                //If function to liberate paid amount
                purchase_return purchase_return = (purchase_return)obj_entity;
                if (purchase_return.payment_schedual != null)
                {
                    foreach (payment_schedual payment in purchase_return.payment_schedual)
                    {
                        payment_schedualList.Add(payment);
                    }
                }
            }
            //SALES RETURN
            else if (obj_entity as sales_return != null)
            {
                //If function to liberate paid amount
                sales_return sales_return = (sales_return)obj_entity;
                if (sales_return.payment_schedual != null)
                {
                    foreach (payment_schedual payment in sales_return.payment_schedual)
                    {
                        payment_schedualList.Add(payment);
                    }
                }
            }

            return payment_schedualList;
        }

        public void DeletePaymentSchedual(db db, int SchedualID)
        {
            payment_schedual payment_schedual = db.payment_schedual.Find(SchedualID);
            db.payment_schedual.Remove(payment_schedual);
        }

        public void InsertPaymentSchedal()
        {
           
        }

        public void ModifyPaymentSchedual(db db, int SchedualID)
        {
            payment_schedual payment_schedual = db.payment_schedual.Find(SchedualID);

            if (payment_schedual.debit < payment_schedual.child.Sum(x => x.credit))
            {


                payment_schedual parent_paymnet_schedual = payment_schedual.child.FirstOrDefault();
                if (parent_paymnet_schedual != null)
                {
                    parent_paymnet_schedual.credit = payment_schedual.debit;
                    parent_paymnet_schedual.payment_detail.value = payment_schedual.debit;
                    parent_paymnet_schedual.payment_detail.app_account_detail.FirstOrDefault().debit = payment_schedual.debit;
                }



                db.SaveChanges();
            }
        }

    }
}
