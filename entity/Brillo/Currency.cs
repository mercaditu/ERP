using System.Linq;

namespace entity.Brillo
{
    public static class Currency
    {
        /// <summary>
        /// 
        /// </summary>
        public static decimal Rate_Previous { get; set; }

        /// <summary>
        /// Gets Default Currency for Company in Session.
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static app_currency get_Default(db db)
        {
            if (CurrentSession.Id_Company > 0)
            {
                return db.app_currency.Where(x => x.is_priority && x.id_company == CurrentSession.Id_Company).FirstOrDefault();
            }

            //Returns Null if Nothing was found.
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <returns>CurrencyFX Entity</returns>
        public static app_currencyfx get_DefaultFX(db db)
        {
            if (CurrentSession.Id_Company > 0 && CurrentSession.Id_Company != null)
            {
                app_currency app_currency = db.app_currency.Where(x => x.is_priority && x.id_company == CurrentSession.Id_Company).FirstOrDefault();
                return db.app_currencyfx.Where(x => x.is_active && x.id_currency == app_currency.id_currency).FirstOrDefault();
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalValue"></param>
        /// <param name="old_app_currencyfx"></param>
        /// <param name="id_app_currencyfx"></param>
        /// <param name="Modules"></param>
        /// <returns></returns>
        public static decimal convert_Values(decimal originalValue, int old_app_currencyfx, int id_app_currencyfx, App.Modules? Modules)
        {
            decimal rate = 0;
            app_currencyfx app_currencyfx = null;
            app_currencyfx app_currencyfxold = null;

            using (db db = new db())
            {
                app_currencyfx = db.app_currencyfx.Where(x => x.id_currencyfx == id_app_currencyfx).FirstOrDefault();
                app_currencyfxold = db.app_currencyfx.Where(x => x.id_currencyfx == old_app_currencyfx).FirstOrDefault();

                if (app_currencyfx != null)
                {
                    if (Modules == App.Modules.Sales)
                    {
                        rate = app_currencyfx.buy_value;
                    }
                    else //Purchase Rates
                    {
                        rate = app_currencyfx.sell_value;
                    }

                    if (app_currencyfx.app_currency == null)
                    {
                        Rate_Previous = rate;
                        return originalValue * rate;
                    }
                    else
                    {
                        bool is_priority = true;
                        if (app_currencyfxold != null)
                        {

                            if (app_currencyfxold.app_currency.is_priority)
                            {
                                is_priority = app_currencyfxold.app_currency.is_priority;
                            }
                        }
                        if (app_currencyfx.app_currency.is_priority == false && is_priority==false)
                        {
                            app_currencyfx app_currencyfxprior = db.app_currencyfx.Where(x => x.app_currency.is_priority).FirstOrDefault();
                        }
                        else if (app_currencyfx.app_currency.is_priority == true) //Towards Default
                        {



                            if (app_currencyfxold != null)
                            {
                                if (Modules == App.Modules.Sales)
                                {
                                    rate = app_currencyfxold.buy_value;
                                }
                                else //Purchase Rates
                                {
                                    rate = app_currencyfxold.sell_value;
                                }
                            }

                            if (app_currencyfx.is_reverse)
                            {
                                return originalValue * rate;
                            }
                            return originalValue * (1 / rate);
                        }
                        else //Away from Default
                        {
                            Rate_Previous = rate;
                            if (app_currencyfx.is_reverse)
                            {
                                return originalValue * (1 / rate);
                            }
                            return originalValue * rate;
                        }
                    }


                }
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalValue"></param>
        /// <param name="id_app_currencyfx"></param>
        /// <param name="Modules"></param>
        /// <returns></returns>
        public static decimal convert_BackValue(decimal originalValue, int id_app_currencyfx, App.Modules? Modules)
        {
            decimal rate = 0;
            app_currencyfx app_currencyfx = null;

            using (db db = new db())
            {
                app_currencyfx = db.app_currencyfx.Where(x => x.id_currencyfx == id_app_currencyfx).FirstOrDefault();


                if (app_currencyfx != null)
                {
                    if (Modules == App.Modules.Sales)
                    {
                        rate = app_currencyfx.buy_value;
                    }
                    else //Purchase Rates
                    {
                        rate = app_currencyfx.sell_value;
                    }

                    if (app_currencyfx.app_currency == null)
                    {
                        Rate_Previous = rate;
                        return originalValue * rate;
                    }

                    if (app_currencyfx.app_currency.is_priority != true) //Towards Default
                    {
                        if (Rate_Previous == 0)
                        {
                            Rate_Previous = rate;
                        }

                        return originalValue * (1 / Rate_Previous);
                    }
                    else //Away from Default
                    {
                        Rate_Previous = rate;
                        return originalValue * rate;
                    }
                }
            }
            return 0;
        }

        public static decimal get_activeRate(int id_currency, App.Names application)
        {
            decimal r = 0;
            using (db db = new db())
            {
                if (application == App.Names.SalesOrder
                    || application == App.Names.SalesInvoice)
                {
                    r = db.app_currencyfx.Where(x => x.id_currency == id_currency && x.is_active == true).FirstOrDefault().buy_value;
                }
                else if (application == App.Names.PurchaseOrder
                         || application == App.Names.PurchaseInvoice)
                {
                    r = db.app_currencyfx.Where(x => x.id_currency == id_currency && x.is_active == true).FirstOrDefault().sell_value;
                }
            }
            return r;
        }

        public static decimal get_specificRate(int id_currencyfx, App.Names application)
        {
            decimal r = 0;
            using (db db = new db())
            {
                if (id_currencyfx > 0)
                {
                    if (application == App.Names.SalesOrder
                        || application == App.Names.SalesReturn
                        || application == App.Names.AccountsReceivable
                        || application == App.Names.SalesInvoice)
                    {
                        r = db.app_currencyfx.Where(x => x.id_currencyfx == id_currencyfx).FirstOrDefault().buy_value;
                    }
                    else if (application == App.Names.PurchaseOrder
                             || application == App.Names.PurchaseReturn
                             || application == App.Names.AccountsPayable
                             || application == App.Names.PurchaseInvoice)
                    {
                        r = db.app_currencyfx.Where(x => x.id_currencyfx == id_currencyfx).FirstOrDefault().sell_value;
                    }
                }
            }
            return r;
        }
    }
}
