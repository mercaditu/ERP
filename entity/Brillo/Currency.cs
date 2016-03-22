using System.Linq;

namespace entity.Brillo
{
    public static class Currency
    {
        /// <summary>
        /// 
        /// </summary>
        public static decimal Rate_Previous { get; set; }

        public static app_currency get_Default(int id_company)
        {
            app_currency app_currency = new app_currency();
            if (id_company > 0)
            {
                using (db db = new db())
                {
                    app_currency = db.app_currency.Where(x => x.id_company == id_company).FirstOrDefault();
                }
            }
            return app_currency;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalValue"></param>
        /// <param name="app_currencyfx"></param>
        /// <param name="Modules"></param>
        /// <returns></returns>
        public static decimal convert_Value(decimal originalValue, int id_app_currencyfx, App.Names Modules)
        {
            decimal rate = 0;
            app_currencyfx app_currencyfx = null;

            using (db db = new db())
            {
                app_currencyfx = db.app_currencyfx.Where(x => x.id_currencyfx == id_app_currencyfx).FirstOrDefault();

                if (app_currencyfx != null)
                {
                    rate = get_specificRate(app_currencyfx.id_currencyfx, Modules);

                    if (app_currencyfx.app_currency == null)
                    {
                        Rate_Previous = rate;
                        return originalValue * rate;
                    }
                    if (app_currencyfx.app_currency.is_priority == true) //Towards Default
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
                if (application == App.Names.SalesBudget ||
                    application == App.Names.SalesOrder || 
                    application == App.Names.SalesInvoice)
                {
                    r = db.app_currencyfx.Where(x => x.id_currency == id_currency && x.is_active == true).FirstOrDefault().buy_value;
                }
                else if (application == App.Names.PurchaseTender ||
                        application == App.Names.PurchaseOrder || 
                        application == App.Names.PurchaseInvoice)
                {
                    r = db.app_currencyfx.Where(x => x.id_currency == id_currency && x.is_active == true).FirstOrDefault().sell_value;
                }
            }
            return r;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id_currencyfx"></param>
        /// <param name="application"></param>
        /// <returns></returns>
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
