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

        public string CheckValueChange(db db, int ID, entity.App.Names Application)
        {
            sales_invoice OriginalSalesInvoice;
            string movmessage = "";
            using (db temp = new db())
            {
                OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();


                sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);
               
                foreach (sales_invoice_detail sales_invoice_detail in Local_SalesInvoice.sales_invoice_detail)
                {
                    sales_invoice_detail Oldsales_invoice_detail = OriginalSalesInvoice.sales_invoice_detail.Where(x => x.id_sales_invoice_detail == sales_invoice_detail.id_sales_invoice_detail).FirstOrDefault();
                    {
                  
                        foreach (item_movement item_movement in sales_invoice_detail.item_movement)
                        {
                            item_movement_value item_movement_value = item_movement.item_movement_value.FirstOrDefault();
                            if (item_movement_value != null)
                            {
                                movmessage += item_movement_value.unit_value + "-->" + sales_invoice_detail.unit_price;
                            
                            }
                        }
                       
                    }
                }
            }
            if (movmessage != "")
            {
                String Message = "You Have Changed The Date So Following Changes Required..\n";
                Message += "This Movement Will be Changed..\n" + movmessage;
                return movmessage;
            }
            return "";

        }


        public string CheckQuantityUP(db db, int ID, entity.App.Names Application)
        {
            sales_invoice OriginalSalesInvoice;
            string movmessage = "";
            using (db temp = new db())
            {
                OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
          

            sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);
         
            foreach (sales_invoice_detail sales_invoice_detail in Local_SalesInvoice.sales_invoice_detail)
            {
                sales_invoice_detail Oldsales_invoice_detail = OriginalSalesInvoice.sales_invoice_detail.Where(x => x.id_sales_invoice_detail == sales_invoice_detail.id_sales_invoice_detail).FirstOrDefault();
                    if (Oldsales_invoice_detail != null)
                {
                    if (sales_invoice_detail.quantity > Oldsales_invoice_detail.quantity)
                    {
                        decimal Diff = sales_invoice_detail.quantity - Oldsales_invoice_detail.quantity;
                        if (Diff > 0)
                        {
                            foreach (item_movement item_movement in sales_invoice_detail.item_movement)
                            {
                                movmessage += item_movement.debit + "-->" + sales_invoice_detail.quantity;
                            }
                        }

                    }
                }
            }
            if (movmessage != "")
            {
                String Message = "You Have Changed The Date So Following Changes Required..\n";
                Message += "This Movement Will be Changed..\n" + movmessage;
                return Message;
            }
            return "";
            }

        }
        public string CheckQuantityDown(db db, int ID, entity.App.Names Application)
        {
            sales_invoice OriginalSalesInvoice;
            string movmessage = "";
            using (db temp = new db())
            {
                OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
       

            sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);
            foreach (sales_invoice_detail sales_invoice_detail in Local_SalesInvoice.sales_invoice_detail)
            {
                sales_invoice_detail Oldsales_invoice_detail = OriginalSalesInvoice.sales_invoice_detail.Where(x => x.id_sales_invoice_detail == sales_invoice_detail.id_sales_invoice_detail).FirstOrDefault();

                if (Oldsales_invoice_detail != null)
                {
                    if (sales_invoice_detail.quantity< Oldsales_invoice_detail.quantity)
                    {
                        decimal Diff = sales_invoice_detail.quantity - Oldsales_invoice_detail.quantity;
                        if (Diff < 0)
                        {

                            foreach (item_movement item_movement in sales_invoice_detail.item_movement)
                            {
                                movmessage += item_movement.debit + "-->" + sales_invoice_detail.quantity;
                            }
                        }
                    }
                }
            }
            if (movmessage != "")
            {
                String Message = "You Have Changed The Date So Following Changes Required..\n";
                Message += "This Movement Will be Changed..\n" + movmessage;
                return Message;
            }
            return "";
            }
        }

        public string CheckDateChange(db db, int ID, entity.App.Names Application)
        {
            sales_invoice OriginalSalesInvoice;
            string movmessage = "";
            using (db temp = new db())
            {
                OriginalSalesInvoice = temp.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
          

            sales_invoice Local_SalesInvoice = db.sales_invoice.Find(ID);
            if (OriginalSalesInvoice.trans_date != Local_SalesInvoice.trans_date)
            {
                movmessage += OriginalSalesInvoice.trans_date + "-->" + Local_SalesInvoice.trans_date;
              
            }

            if (movmessage != "")
            {
                String Message = "You Have Changed The Date So Following Changes Required..\n";
                Message += "This Movement Will be Changed..\n" + movmessage;
                return Message;
            }
            return "";
            }
        }
        public string CheckNewMovement(db db, int ID, entity.App.Names Application)
        {
            sales_invoice Oldsales_invoice = db.sales_invoice.Where(x => x.id_sales_invoice == ID).FirstOrDefault();
            string movmessage = "";
            sales_invoice sales_invoice = db.sales_invoice.Find(ID);
            foreach (sales_invoice_detail sales_invoice_detail in sales_invoice.sales_invoice_detail)
            {
                sales_invoice_detail Oldsales_invoice_detail = Oldsales_invoice.sales_invoice_detail.Where(x => x == sales_invoice_detail).FirstOrDefault();
                if (Oldsales_invoice_detail == null)
                {
                    movmessage += "New Movement will Be created";
                }
            }
            if (movmessage != "")
            {
                String Message = "You Have Changed The Date So Following Changes Required..\n";
                Message += "This Movement Will be Changed..\n" + movmessage;
                return Message;
            }
            return "";
        }

    }
}
