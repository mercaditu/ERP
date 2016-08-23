using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using entity.Brillo;

namespace entity
{
    public partial class ImpexDB : BaseDB
    {

        public override int SaveChanges()
        {
            validate_Impex();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_Impex();
            return base.SaveChangesAsync();
        }

        private void validate_Impex()
        {
            foreach (impex impex in base.impex.Local)
            {
                if (impex.IsSelected)
                {
                    if (impex.State == EntityState.Added)
                    {
                        impex.timestamp = DateTime.Now;
                        impex.State = EntityState.Unchanged;
                        Entry(impex).State = EntityState.Added;
                    }
                    else if (impex.State == EntityState.Modified)
                    {
                        impex.timestamp = DateTime.Now;
                        impex.State = EntityState.Unchanged;
                        Entry(impex).State = EntityState.Modified;
                    }
                    else if (impex.State == EntityState.Deleted)
                    {
                        impex.timestamp = DateTime.Now;
                        impex.State = EntityState.Unchanged;
                        //impex.Remove(impex);
                    }
                }
                else if (impex.State > 0)
                {
                    if (impex.State != EntityState.Unchanged)
                    {
                        Entry(impex).State = EntityState.Unchanged;
                    }
                }
            }
        }

        public List<entity.Class.Impex_CostDetail> Fill_ViewModel(impex impex)
        {
            List<entity.Class.Impex_CostDetail> Impex_CostDetailLIST = new List<Class.Impex_CostDetail>();
            List<impex_expense> impex_expense = impex.impex_expense.ToList();
            List<impex_import> impex_importList = impex.impex_import.ToList();
            decimal totalExpense = 0;

            foreach (var item in impex_expense)
            {
                totalExpense += item.value;
            }

            foreach (impex_import impex_import in impex_importList)
            {


                if (impex_import.purchase_invoice != null)
                {
                    //Insert Purchase Invoice Detail
                    List<purchase_invoice_detail> purchase_invoice_detail = impex_import.purchase_invoice.purchase_invoice_detail.ToList();

                    decimal TotalInvoiceAmount = 0;

                    foreach (var item in purchase_invoice_detail)
                    {
                        TotalInvoiceAmount += (item.quantity * item.UnitCost_Vat);
                    }



                    foreach (purchase_invoice_detail _purchase_invoice_detail in purchase_invoice_detail.Where(x => x.item != null && x.item.item_product != null))
                    {
                        int id_item = (int)_purchase_invoice_detail.id_item;

                        entity.Class.Impex_CostDetail ImpexImportDetails = new entity.Class.Impex_CostDetail();
                        ImpexImportDetails.number = _purchase_invoice_detail.purchase_invoice.number;
                        ImpexImportDetails.id_item = (int)_purchase_invoice_detail.id_item;
                        ImpexImportDetails.item = base.items.Where(a => a.id_item == _purchase_invoice_detail.id_item).FirstOrDefault().name;
                        ImpexImportDetails.quantity = _purchase_invoice_detail.quantity;
                        ImpexImportDetails.unit_cost = _purchase_invoice_detail.unit_cost;
                        ImpexImportDetails.id_invoice = _purchase_invoice_detail.id_purchase_invoice;
                        ImpexImportDetails.id_invoice_detail = _purchase_invoice_detail.id_purchase_invoice_detail;

                        if (totalExpense > 0)
                        {
                            ImpexImportDetails.prorated_cost = Math.Round(_purchase_invoice_detail.unit_cost + (totalExpense / purchase_invoice_detail.Sum(x => x.quantity)), 2);
                        }

                        decimal SubTotal = (_purchase_invoice_detail.quantity * ImpexImportDetails.prorated_cost);
                        ImpexImportDetails.sub_total = Math.Round(SubTotal, 2);
                        Impex_CostDetailLIST.Add(ImpexImportDetails);
                    }
                }
            }
            return Impex_CostDetailLIST;
        }

        public void ApproveImport()
        {
            foreach (impex impex in base.impex.Local.Where(x => x.status != Status.Documents_General.Approved && x.impex_type == entity.impex._impex_type.Import && x.IsSelected))
            {
                if (impex.Error == null)
                {
                    if (impex.id_impex == 0)
                    {
                        SaveChanges();
                    }

                    //fill up virtual class
                    List<entity.Class.Impex_CostDetail> ImpexImportDetails = Fill_ViewModel(impex);
                    List<impex_expense> impex_expenses = impex.impex_expense.ToList();

                    if (ImpexImportDetails.Count > 0)
                    {
                        //To make sure we have a Purchase Total
                        decimal purchaseTotal = ImpexImportDetails.Sum(i => i.sub_total);
                        if (purchaseTotal != 0)
                        {
                            foreach (entity.Class.Impex_CostDetail Impex_CostDetail in ImpexImportDetails)
                            {
                                //Get total value of a Product Row
                                decimal itemTotal = Impex_CostDetail.quantity * Impex_CostDetail.unit_cost;

                                purchase_invoice purchase_invoice = base.purchase_invoice.Where(x => x.id_purchase_invoice == Impex_CostDetail.id_invoice).FirstOrDefault();
                                item_movement item_movement = base.item_movement.Where(x => x.id_purchase_invoice_detail == Impex_CostDetail.id_invoice_detail).FirstOrDefault();

                                foreach (impex_expense _impex_expense in impex_expenses)
                                {
                                    decimal condition_value = _impex_expense.value;

                                    if (condition_value != 0 && itemTotal != 0)
                                    {
                                        //Coeficient is used to get prorated cost of one item
                                        item_movement_value item_movement_detail = new item_movement_value();

                                        decimal Cost = Math.Round(_impex_expense.value / ImpexImportDetails.Sum(x => x.quantity), 2);

                                        //decimal Cost = Impex_CostDetail.unit_cost * coeficient;

                                        //Improve this in future. For now take from Purchase
                                        using (db db = new db())
                                        {
                                            int ID_CurrencyFX_Default = CurrentSession.CurrencyFX_Default.id_currencyfx;
                                            decimal DefaultCurrency_Cost = Currency.convert_Values(Cost, purchase_invoice.id_currencyfx, ID_CurrencyFX_Default, null);

                                            item_movement_detail.unit_value = DefaultCurrency_Cost;
                                            item_movement_detail.id_currencyfx = ID_CurrencyFX_Default;
                                        }

                                        item_movement_detail.comment = _impex_expense.impex_incoterm_condition.name;
                                        if (item_movement!=null)
                                        {
                                            item_movement.item_movement_value.Add(item_movement_detail);
                                        }
                                      
                                    }
                                }
                            }
                            impex.status = Status.Documents_General.Approved;
                            base.SaveChanges();
                        }
                    }



                }

            }
        }
    }
}
