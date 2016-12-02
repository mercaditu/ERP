using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using entity;
using entity.Brillo;
using cntrl.PanelAdv;
using System.Windows;

namespace cntrl.Class
{
    public class CheckMovementReApprove
    {

        public bool CheckValueChange(db db, int ID, entity.App.Names Application)
        {
            sales_invoice OriginalSalesInvoice;

            using (db temp = new db())
            {
                OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
            }

            sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);

            foreach (sales_invoice_detail sales_invoice_detail in Local_SalesInvoice.sales_invoice_detail)
            {
                sales_invoice_detail Oldsales_invoice_detail = OriginalSalesInvoice.sales_invoice_detail.Where(x => x == sales_invoice_detail).FirstOrDefault();
                if (Oldsales_invoice_detail != null)
                {
                    if (sales_invoice_detail.unit_price != Oldsales_invoice_detail.unit_price)
                    {
                        return true;
                    }
                }
            }
            return false;
         
        }


        public bool CheckQuantityUP(db db, int ID, entity.App.Names Application)
        {
            sales_invoice OriginalSalesInvoice;

            using (db temp = new db())
            {
                OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
            }

            sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);
            foreach (sales_invoice_detail sales_invoice_detail in Local_SalesInvoice.sales_invoice_detail)
            {
                sales_invoice_detail Oldsales_invoice_detail = OriginalSalesInvoice.sales_invoice_detail.Where(x => x == sales_invoice_detail).FirstOrDefault();
                if (Oldsales_invoice_detail != null)
                {
                    if (sales_invoice_detail.quantity != Oldsales_invoice_detail.quantity)
                    {
                        decimal Diff = sales_invoice_detail.quantity - Oldsales_invoice_detail.quantity;
                        if (Diff > 0)
                        {

                            return true;

                        }
                    }
                }
            }
            return false;
        }
        public bool CheckQuantityDown(db db, int ID, entity.App.Names Application)
        {
            sales_invoice OriginalSalesInvoice;

            using (db temp = new db())
            {
                OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
            }

            sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);
            foreach (sales_invoice_detail sales_invoice_detail in Local_SalesInvoice.sales_invoice_detail)
            {
                sales_invoice_detail Oldsales_invoice_detail = OriginalSalesInvoice.sales_invoice_detail.Where(x => x == sales_invoice_detail).FirstOrDefault();
                if (Oldsales_invoice_detail != null)
                {
                    if (sales_invoice_detail.quantity != Oldsales_invoice_detail.quantity)
                    {
                        decimal Diff = sales_invoice_detail.quantity - Oldsales_invoice_detail.quantity;
                        if (Diff < 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;

        }

        public bool CheckDateChange(db db, int ID, entity.App.Names Application)
        {
            sales_invoice OriginalSalesInvoice;

            using (db temp = new db())
            {
                OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
            }

            sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);
            if (OriginalSalesInvoice.trans_date != Local_SalesInvoice.trans_date)
            {
                return true;
            }

            return false;

        }
        public bool CheckNewMovement(db db, int ID, entity.App.Names Application)
        {
            sales_invoice Oldsales_invoice = db.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
            sales_invoice sales_invoice = db.sales_invoice.Find(ID);
            foreach (sales_invoice_detail sales_invoice_detail in sales_invoice.sales_invoice_detail)
            {
                sales_invoice_detail Oldsales_invoice_detail = Oldsales_invoice.sales_invoice_detail.Where(x => x == sales_invoice_detail).FirstOrDefault();
                if (Oldsales_invoice_detail == null)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
