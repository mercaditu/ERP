using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using entity;
namespace entity.Class
{
    public static class Promotion
    {
       public static List<sales_promotion> SalesPromotionList { get; set; }
       //public Promotion()
       //{
       //    using (db db = new db())
       //    {
       //        SalesPromotionList = db.sales_promotion.Where(x => x.id_company == CurrentSession.Id_Company
       //                                                    && (x.date_start.Date <= DateTime.Now.Date || x.date_end >= DateTime.Now.Date)).ToList();

       //    }
       //}


        public static void GetSalesPromotion(ref sales_invoice sales_invoice)
        {
            //foreach (sales_promotion sales_promotion in SalesPromotionList)
            //{
            //    Decimal TotalPromotion = 0;
            //    if (sales_promotion.types==entity.sales_promotion.Type.DiscountDuringPeriod)
            //    {
            //        Decimal PromotionDiscount = 0;
            //        foreach (sales_invoice_detail sales_invoice_detail in sales_invoice.sales_invoice_detail)
            //        {
            //            if (sales_promotion.quantity_min <=sales_invoice_detail.quantity || sales_promotion.quantity_max <=sales_invoice_detail.quantity)
            //            {
            //                if (sales_promotion.is_percentage)
            //                {
            //                    //PromotionDiscount =PromotionDiscount+ sales_invoice_detail
            //                }
            //            }
            //        } 
            //    }
            //}

          
        }
    }
}
