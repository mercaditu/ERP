using System;
using System.Linq;

namespace entity.Brillo
{
    public static class Currency
    {
        /// <summary>
        /// Used Internally for this class.
        /// </summary>
        private static decimal Rate_Previous { get; set; }

        /// <summary>
        /// Convert values from one currency to another.
        /// </summary>
        /// <param name="OriginalValue"></param>
        /// <param name="Old_CurrencyID"></param>
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
                db.Configuration.AutoDetectChangesEnabled = false;

                app_currencyfx = db.app_currencyfx.Find(id_app_currencyfx);
                app_currencyfxold = db.app_currencyfx.Find(old_app_currencyfx);

                if (app_currencyfx != null && app_currencyfxold != null)
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
                        if (app_currencyfx.id_currency != app_currencyfxold.id_currency)
                        {
                            bool is_priority = true;
                            if (app_currencyfxold != null)
                            {
                                if (app_currencyfxold.app_currency.is_priority)
                                {
                                    is_priority = app_currencyfxold.app_currency.is_priority;
                                }
                                else
                                {
                                    is_priority = false;
                                }
                            }

                            if (app_currencyfx.app_currency.is_priority == false && is_priority == false)
                            {
                                app_currencyfx _app_currencyfx = CurrentSession.Get_Currency_Default_Rate(); //db.app_currencyfx.Where(x => x.app_currency.is_priority).FirstOrDefault();
                                if (_app_currencyfx != null)
                                {
                                    //Convert Towards Defualt
                                    decimal Value_InPriority = convert_Values(originalValue, old_app_currencyfx, _app_currencyfx.id_currencyfx, App.Modules.Sales);
                                    //Convert Away from Default
                                    return convert_Values(Value_InPriority, _app_currencyfx.id_currencyfx, id_app_currencyfx, App.Modules.Sales);
                                }
                            }
                            else if (app_currencyfx.app_currency.is_priority == true) //Towards Default
                            {
                                if (app_currencyfxold != null)
                                {
                                    if (Modules == App.Modules.Sales)
                                    {
                                        rate = app_currencyfxold.buy_value == 0 ? 1 : app_currencyfxold.buy_value;
                                    }
                                    else //Purchase Rates
                                    {
                                        rate = app_currencyfxold.sell_value == 0 ? 1 : app_currencyfxold.sell_value;
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
            }
            return originalValue;
        }

        public static decimal convert_Values(decimal v, int id_currencyfx1, int id_currencyfx2, object sales)
        {
            throw new NotImplementedException();
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
                app_currencyfx = db.app_currencyfx.Include("app_currency").Where(x => x.id_currencyfx == id_app_currencyfx).FirstOrDefault();
            }

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
            return 0;
        }
    }
}