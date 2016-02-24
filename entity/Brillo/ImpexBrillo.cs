using System.Linq;

namespace entity.Brillo
{
    public class ImpexBrillo
    {
        db db = new db();
        public void Impex_Update(int id_module, int id_trans)
        {
            //Need to calculate cost. 

            if (id_module == (int)App.Names.SalesInvoice)
            {
                if (db.impex_export.Where(x => x.id_impex == id_trans).Count() > 0)
                {
                    int id_sales_invoice = db.impex_export.Where(x => x.id_impex == id_trans).FirstOrDefault().id_sales_invoice;
                    if (db.payment_schedual.Where(x => x.id_sales_invoice == id_sales_invoice).Count() > 0)
                    {

                        

                        //payment_schedual payment_schedual = db.payment_schedual.Where(x => x.id_sales_invoice == id_sales_invoice).FirstOrDefault();
                        //if (payment_schedual != null)
                        //{
                        //    decimal total_exp = db.impex_expense.Where(x => x.id_impex == id_trans).Sum(x => x.value);
                        //    payment_schedual.debit = payment_schedual.debit + total_exp;
                        //}
                    }
                }
            }
            else if (id_module == (int)App.Names.PurchaseInvoice)
            {
                //int id_Purchase_invoice = db.impex_import.Where(x => x.id_impex == id_trans).FirstOrDefault().id_purchase_invoice;
                //payment_schedual payment_schedual = db.payment_schedual.Where(x => x.id_purchase_invoice == id_Purchase_invoice).FirstOrDefault();
                //if (payment_schedual!=null)
                //{
                //   decimal total_exp = db.impex_expense.Where(x => x.id_impex == id_trans).Sum(x => x.value);
                //    payment_schedual.credit = payment_schedual.credit + total_exp;
                //}
            }

            db.SaveChanges();
        }
    }
}
