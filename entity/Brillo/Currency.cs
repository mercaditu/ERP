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
        /// <param name="OriginalValue"></param>
        /// <param name="Old_CurrencyID"></param>
        /// <param name="id_app_currencyfx"></param>
        /// <param name="Modules"></param>
        /// <returns></returns>
        public static decimal convert_Values(decimal OriginalValue, int Old_CurrencyID, int New_CurrencyID, App.Modules? Modules)
        {
            app_currencyfx New_CurrencyFX = null;
            app_currencyfx Old_CurrencyFX = null;

            using (db db = new db())
            {
                New_CurrencyFX = db.app_currencyfx.Where(x => x.id_currencyfx == New_CurrencyID).FirstOrDefault();
                Old_CurrencyFX = db.app_currencyfx.Where(x => x.id_currencyfx == Old_CurrencyID).FirstOrDefault();

                //Ignore entire code if there is no new currencyID.
                if (New_CurrencyFX != null)
                {
                    decimal New_Rate = 0;
                    decimal Old_Rate = 0;

                    //Check which value to take, Sales = Buy Rate. Purchase = Sell Rate.
                    if (Modules == App.Modules.Sales)
                    {
                        New_Rate = New_CurrencyFX.buy_value;
                    }
                    else //Purchase Rates
                    {
                        New_Rate = New_CurrencyFX.sell_value;
                    }

                    if (Old_CurrencyFX != null)
                    {
                        if (Modules == App.Modules.Sales)
                        {
                            Old_Rate = Old_CurrencyFX.buy_value;
                        }
                        else //Purchase Rates
                        {
                            Old_Rate = Old_CurrencyFX.sell_value;
                        }
                    }

                    if (Old_CurrencyFX == null)
                    {
                        Old_Rate = New_Rate;
                    }

                    if (New_CurrencyFX.app_currency == null)
                    {
                        Rate_Previous = New_Rate;
                        return OriginalValue * New_Rate;
                    }
                    else
                    {
                        //Get Priority
                        bool New_Priority = New_CurrencyFX.app_currency.is_priority;
                        bool Old_Priority = true;

                        //This is incase there is no OldCurrency.
                        if (Old_CurrencyFX != null)
                        {
                            Old_Priority = Old_CurrencyFX.app_currency.is_priority;
                        }

                        //Neither currency is priority
                        if (New_Priority == false && Old_Priority == false) 
                        {
                            //Convert Towards Defualt
                            decimal Value_InPriority = TowardsDefault(Old_Rate, OriginalValue, New_CurrencyFX.is_reverse);

                            //Convert Away from Default
                            return AwayFromDefault(New_Rate, Value_InPriority, Old_CurrencyFX.is_reverse);
                        }   
                        else if (New_CurrencyFX.app_currency.is_priority == true)
                        { //Towards Default
                            return TowardsDefault(Old_Rate, OriginalValue, New_CurrencyFX.is_reverse);
                        }
                        else
                        { //Away from Default
                            return AwayFromDefault(New_Rate, OriginalValue, Old_CurrencyFX.is_reverse);
                        }
                    }
                }
            }
            return 0;
        }

        private static decimal AwayFromDefault(decimal Rate, decimal OriginalValue, bool IsReverse)
        {
            if (IsReverse)
            {
                return OriginalValue * (1 / Rate);
            }
            else
            {
                return OriginalValue * Rate;
            }
        }

        private static decimal TowardsDefault(decimal Rate, decimal OriginalValue, bool IsReverse)
        {
            if (IsReverse)
            {
                return OriginalValue * Rate;
            }
            else
            {
                return OriginalValue * (1 / Rate);
            }
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
