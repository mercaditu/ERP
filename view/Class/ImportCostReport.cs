using entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cognitivo.Class
{
    public class ImportCostReport
    {

        ImpexDB ImpexDB = new ImpexDB();
        public List<impex_expense> GetExpensesForAllIncoterm(purchase_invoice PurchaseInvoice)
        {
            List<impex_expense> impex_expenseList = new List<impex_expense>();
            List<impex_incoterm> impex_incotermList = ImpexDB.impex_incoterm.ToList();
            List<entity.Class.Impex_Products> Impex_ProductsLIST = new List<entity.Class.Impex_Products>();
            if (PurchaseInvoice != null)
            {
                foreach (purchase_invoice_detail _purchase_invoice_detail in PurchaseInvoice.purchase_invoice_detail.Where(x => x.item != null && x.item.item_product != null))
                {
                    int id_item = (int)_purchase_invoice_detail.id_item;

                    if (Impex_ProductsLIST.Where(x => x.id_item == id_item).Count() == 0)
                    {
                        entity.Class.Impex_Products ImpexImportProductDetails = new entity.Class.Impex_Products();
                        ImpexImportProductDetails.id_item = (int)_purchase_invoice_detail.id_item;
                        ImpexImportProductDetails.item = ImpexDB.items.Where(a => a.id_item == _purchase_invoice_detail.id_item).FirstOrDefault().name;
                        Impex_ProductsLIST.Add(ImpexImportProductDetails);
                    }
                }

                foreach (impex_incoterm Incoterm in impex_incotermList)
                {



                    List<impex_incoterm_detail> IncotermDetail = ImpexDB.impex_incoterm_detail.Where(i => i.id_incoterm == Incoterm.id_incoterm && i.buyer == true).ToList();



                    foreach (entity.Class.Impex_Products product in Impex_ProductsLIST)
                    {
                        foreach (var item in IncotermDetail)
                        {
                            impex_expense impex_expense = new impex_expense();


                            if (ImpexDB.impex_incoterm_condition.Where(x => x.id_incoterm_condition == item.id_incoterm_condition).FirstOrDefault() != null)
                            {
                                impex_expense.impex_incoterm_condition = ImpexDB.impex_incoterm_condition.Where(x => x.id_incoterm_condition == item.id_incoterm_condition).FirstOrDefault();
                            }

                            impex_expense _impex_expense = ImpexDB.impex_expense.Where(x => x.id_incoterm_condition == item.id_incoterm_condition && x.id_purchase_invoice == PurchaseInvoice.id_purchase_invoice).FirstOrDefault();
                            if (_impex_expense != null)
                            {
                                impex_expense.value = _impex_expense.value;
                            }
                            else
                            {
                                impex_expense.value = 0;
                            }

                            impex_expense.id_incoterm_condition = item.id_incoterm_condition;
                            impex_expense.id_currency = PurchaseInvoice.app_currencyfx.id_currency;
                            impex_expense.id_currencyfx = PurchaseInvoice.id_currencyfx;
                            impex_expense.id_purchase_invoice = PurchaseInvoice.id_purchase_invoice;
                            impex_expense.id_item = (int)product.id_item;
                            impex_expenseList.Add(impex_expense);
                        }


                    }
                }

            }

            return impex_expenseList;
        }

    }
}
