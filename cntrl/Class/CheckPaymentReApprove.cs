using System;
using System.Windows;
using entity;
using System.Linq;
using System.Collections.Generic;
using entity.Brillo.Logic;
namespace cntrl.Class
{
    public class CheckPaymentReApprove
    {

        public bool Start(db db, int ID, entity.App.Names Application)
        {
            return true;
        }

        public bool Check_ContractChanges(db db, int ID, entity.App.Names Application)
        {
            sales_invoice OriginalSalesInvoice;

            using (db temp = new db())
            {
                OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
            }

            sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);

            if (OriginalSalesInvoice.id_contract != Local_SalesInvoice.id_contract)
            {
                return true;
            }
            return false;
        }

        public bool Check_ValueUP(db db, int ID, entity.App.Names Application)
        {
            sales_invoice OriginalSalesInvoice;

            using (db temp = new db())
            {
                OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
            }

            sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);

            if (OriginalSalesInvoice.GrandTotal < Local_SalesInvoice.GrandTotal)
            {
                return true;
            }
            return false;
        }
        public bool Check_ValueDown(db db, int ID, entity.App.Names Application)
        {
            sales_invoice OriginalSalesInvoice;

            using (db temp = new db())
            {
                OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
            }

            sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);
            if (OriginalSalesInvoice.GrandTotal > Local_SalesInvoice.GrandTotal)
            {
                return true;


            }
            return false;
        }
        public bool CurrencyChange(db db, int ID, entity.App.Names Application)
        {
            sales_invoice OriginalSalesInvoice;

            using (db temp = new db())
            {
                OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
            }

            sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);
            if (OriginalSalesInvoice.id_currencyfx != Local_SalesInvoice.id_currencyfx)
            {
                return true;
            }
            return false;
        }

        public bool Check_DateChange(db db, int ID, entity.App.Names Application)
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
    }
}
