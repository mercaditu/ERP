using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace entity.Brillo
{
    class General
    {
        public app_terminal Get_Terminal(int id_terminal)
        {
            using (db db = new db())
            {
                return db.app_terminal.Where(x => x.id_terminal == id_terminal).FirstOrDefault();
            }
        }

        public int Get_Currency(int id_company)
        {
            using (db db = new db())
            {
                if (db.app_currency.Where(x => x.id_company == id_company).FirstOrDefault() != null && db.app_currency.Any(x => x.is_priority == true))
                {
                    return db.app_currency.Where(x => x.id_company == id_company).FirstOrDefault().id_currency;
                }
                else
                {
                    return 0;
                }
            }
        }
        public int get_price_list(int id_company)
        {
            using (db db = new db())
            {
                if (db.item_price_list.Where(a => a.id_company == id_company && a.is_default == true).FirstOrDefault() != null)
                    return db.item_price_list.Where(a => a.id_company == id_company && a.is_default == true).FirstOrDefault().id_price_list;
                else
                    return 0;
            }
        }

        ///// <summary>
        ///// Get value with vat in item_price by item_id
        ///// </summary>
        ///// <param name="item_id"></param>
        ///// <param name="price"></param>
        ///// <returns></returns>
        //public decimal getValueWithVatItemPrice(int item_id, decimal price)
        //{
        //    using (db db = new db())
        //    {
        //        item _item = db.items.Where(a => a.id_item == item_id).FirstOrDefault();
        //        if (_item != null)
        //        {
        //            if (_item.app_vat_group != null)
        //            {
        //                decimal totalVAT = 0;
        //                foreach (item_vat vat in _item.item_vat)
        //                {
        //                    List<app_vat_group_details> app_vat_group_details = db.app_vat_group_details.Where(x => x.id_vat_group == vat.id_vat_group).ToList();
        //                    foreach (app_vat_group_details app_vat_group in app_vat_group_details)
        //                    {
        //                        totalVAT += (app_vat_group.app_vat.coefficient) * price;
        //                    }
        //                }
        //                return price + totalVAT;
        //            }
        //            else
        //            {
        //                return price;
        //            }
        //        }
        //        else
        //        {
        //            return price;
        //        }
        //    }
        //}

        /// <summary>
        /// Get value with vat in item_price by item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public decimal getValueWithVatItemPrice(int id_item, decimal price)
        {
            using (db db = new db())
            {
                item item = db.items.Where(x => x.id_item == id_item).FirstOrDefault();
                app_vat_group app_vat_group;
                List<app_vat_group_details> _app_vat_group_details;
                if (item != null)
                {


                    app_vat_group = db.app_vat_group.Where(x => x.id_vat_group == item.id_vat_group).FirstOrDefault();

                    if (app_vat_group != null)
                    {
                        decimal totalVAT = 0;
                        //foreach (item_vat vat in item.item_vat)
                        //{
                        
                            _app_vat_group_details = db.app_vat_group_details.Where(x => x.id_vat_group == app_vat_group.id_vat_group).ToList();

                            foreach (app_vat_group_details _app_vat_group in _app_vat_group_details)
                            {
                                totalVAT += (_app_vat_group.app_vat.coefficient) * price;
                            }
                       
                        return price + totalVAT;
                    }
                    else
                    {
                        return price;
                    }
                }
                else
                {
                    return price;
                }
            }
        }
        public decimal getValueWithVatItemPrice(item item, decimal price)
        {
            using (db db = new db())
            {
               // item item = db.items.Where(x => x.id_item == id_item).FirstOrDefault();
                app_vat_group app_vat_group;
                List<app_vat_group_details> _app_vat_group_details;
                if (item != null)
                {


                    app_vat_group = db.app_vat_group.Where(x => x.id_vat_group == item.id_vat_group).FirstOrDefault();

                    if (app_vat_group != null)
                    {
                        decimal totalVAT = 0;
                        //foreach (item_vat vat in item.item_vat)
                        //{

                        _app_vat_group_details = db.app_vat_group_details.Where(x => x.id_vat_group == app_vat_group.id_vat_group).ToList();

                        foreach (app_vat_group_details _app_vat_group in _app_vat_group_details)
                        {
                            totalVAT += (_app_vat_group.app_vat.coefficient) * price;
                        }

                        return price + totalVAT;
                    }
                    else
                    {
                        return price;
                    }
                }
                else
                {
                    return price;
                }
            }
        }
        /// <summary>
        /// Use to get Value With VAT in item_price by item_id
        /// </summary>
        /// <param name="item_id"></param>
        /// <returns></returns>
        //public decimal getOriginalItemPrice(int item_id, decimal price)
        //{
        //    using (db db = new db())
        //    {
        //        item _item = db.items.Where(a => a.id_item == item_id).FirstOrDefault();
        //        if (_item != null)
        //        {
        //            if (_item.app_vat_group != null)
        //            {
        //                decimal originalVal = 0;
        //                decimal totalCoeff = 0;
        //                //foreach (item_vat vat in _item.item_vat)
        //                //{
        //                    //List<app_vat_group_details> app_vat_group_details = db.app_vat_group_details.Where(x => x.id_vat_group == vat.id_vat_group).ToList();
        //                    foreach (app_vat_group_details app_vat_group in item.app_vat_group.app_vat_group_details)
        //                    {
        //                        totalCoeff += (app_vat_group.app_vat.coefficient);

        //                    }
        //                    //originalVal += price * vat_coefficient; 
        //                //}
        //                originalVal = price / (1 + totalCoeff);
        //                return originalVal;
        //            }
        //            else
        //            {
        //                return price;
        //            }
        //        }
        //        else
        //        {
        //            return price;
        //        }
        //    }
        //}

        /// <summary>
        /// Use to get Value With VAT in item_price by item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public decimal getOriginalItemPrice(item item, decimal price)
        {
            if (item != null)
            {
                if (item.app_vat_group != null)
                {
                    decimal originalVal = 0;
                    decimal totalCoeff = 0;
                    //foreach (item_vat vat in item.item_vat)
                    //{
                    //originalVal += price * vat_coefficient;
                    //List<app_vat_group_details> app_vat_group_details = db.app_vat_group_details.Where(x => x.id_vat_group == vat.id_vat_group).ToList();
                    foreach (app_vat_group_details app_vat_group in item.app_vat_group.app_vat_group_details)
                    {
                        totalCoeff += (app_vat_group.app_vat.coefficient);
                    }
                    //}
                    originalVal = price / (1 + totalCoeff);
                    return originalVal;
                }
                else
                {
                    return price;
                }
            }
            else
            {
                return price;
            }
        }
    }
}
