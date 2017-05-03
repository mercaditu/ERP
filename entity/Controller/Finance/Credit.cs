using System.Data.Entity;
using System.Linq;

namespace entity.Controller.Finance
{
    public class Credit
    {
        #region Finance

        /// <summary>
        /// Method checks for Credit Limits of Customers. Returns True if Credit is avaiable or not
        /// calculated. But if Limit has been exceeded, returns False.
        /// </summary>
        /// <param name="GrandTotal">Total of Budget, Order, or Invoice</param>
        /// <param name="CurrencyFX">CurrencyFX of Budget, Order, or Invoice</param>
        /// <param name="Customer">Contact relatd to Sale</param>
        /// <param name="Contract">Contract used during Sale</param>
        /// <returns>Boolean where True = Can Transact | False = Limit Exceded</returns>
        public bool CheckLimit_InSales(decimal GrandTotal, app_currencyfx CurrencyFX, contact Customer, app_contract Contract)
        {
            //If Contact Credit Limit is none, we will assume that Credit Limit is not enforced.
            if (Customer.credit_limit != null)
            {
                //If Sales Contract is Cash. Credit Limit is not enforced.
                if (Contract.app_contract_detail.Sum(x => x.interval) > 0)
                {
                    //Script that checks Contacts Credit Availability.
                    //sales_invoice.contact.Check_CreditAvailability();
                    if (Customer.credit_limit != null)
                    {
                        if (Customer.credit_limit > 0 && Customer.id_contact != 0)
                        {
                            using (db db = new db())
                            {

                                var BalanceList =
                                    from c in db.app_currency
                                    join fx in db.app_currencyfx on c.id_currency equals fx.id_currency
                                    join ps in db.payment_schedual on fx.id_currencyfx equals ps.id_currencyfx
                                    group ps by new { fx.id_currency } into g
                                    select new
                                    {
                                        CurrencyID = g.Max(x => x.app_currencyfx.id_currency),
                                        Balance = g.Sum(s => s.debit - s.credit)
                                    };

                                decimal BalanceInDefault = 0;

                                foreach (var SumByCurrency in BalanceList)
                                {
                                    int OriginalFXRate = CurrentSession.CurrencyFX_ActiveRates.Where(x => x.id_currency == SumByCurrency.CurrencyID).Select(x => x.id_currencyfx).FirstOrDefault();
                                    int DefaultFXRate = CurrentSession.CurrencyFX_ActiveRates.Where(x => x.id_currency == CurrencyFX.id_currency).Select(x => x.id_currencyfx).FirstOrDefault();
                                    BalanceInDefault += Brillo.Currency.convert_Values(SumByCurrency.Balance, OriginalFXRate, DefaultFXRate, App.Modules.Sales);
                                }

                                Customer.credit_availability = Customer.credit_limit - BalanceInDefault;
                                Customer.RaisePropertyChanged("credit_availability");

                                //Check if Availability is greater than 0.
                                if (Customer.credit_availability > 0)
                                {
                                    decimal TotalSales = GrandTotal;
                                    decimal CreditAvailability = (decimal)Customer.credit_availability;

                                    if (CreditAvailability < TotalSales)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Customer.credit_availability = Customer.credit_limit;
                    }
                }
                else
                {
                    Customer.credit_availability = Customer.credit_limit;
                }
            }

            return true;
        }

        /// <summary>
        /// Method checks for your Credit Limits with the Supplier. Returns True if Credit is avaiable or not
        /// calculated. But if Limit has been exceeded, returns False.
        /// </summary>
        /// <param name="GrandTotal"></param>
        /// <param name="Currency"></param>
        /// <param name="Supplier"></param>
        /// <param name="Contract"></param>
        /// <returns></returns>
        public bool CheckLimit_InPurchase(decimal GrandTotal, app_currencyfx CurrencyFX, contact Supplier, app_contract Contract)
        {
            //If Contact Credit Limit is none, we will assume that Credit Limit is not enforced.
            if (Supplier.credit_limit != null)
            {
                //If Purchase Contract is Cash. Credit Limit is not enforced.
                if (Contract.app_contract_detail.Sum(x => x.interval) > 0)
                {
                    if (Supplier.credit_limit > 0 && Supplier.id_contact != 0)
                    {
                        using (db db = new db())
                        {
                            var BalanceList =
                                from c in db.app_currency
                                join fx in db.app_currencyfx on c.id_currency equals fx.id_currency
                                join ps in db.payment_schedual on fx.id_currencyfx equals ps.id_currencyfx
                                group ps by new { fx.id_currency } into g
                                select new
                                {
                                    CurrencyID = g.Max(x => x.app_currencyfx.id_currency),
                                    Balance = g.Sum(s => s.credit - s.debit)
                                };

                            decimal BalanceInDefault = 0;

                            foreach (var SumByCurrency in BalanceList)
                            {
                                int OriginalFXRate = CurrentSession.CurrencyFX_ActiveRates.Where(x => x.id_currency == SumByCurrency.CurrencyID).Select(x => x.id_currencyfx).FirstOrDefault();
                                int DefaultFXRate = CurrentSession.CurrencyFX_ActiveRates.Where(x => x.id_currency == CurrencyFX.id_currency).Select(x => x.id_currencyfx).FirstOrDefault();
                                BalanceInDefault += Brillo.Currency.convert_Values(SumByCurrency.Balance, OriginalFXRate, DefaultFXRate, App.Modules.Sales);
                            }

                            Supplier.credit_availability = Supplier.credit_limit - BalanceInDefault;
                            Supplier.RaisePropertyChanged("credit_availability");

                            //Check if Availability is greater than 0.
                            if (Supplier.credit_availability > 0)
                            {
                                decimal TotalSales = GrandTotal;
                                decimal CreditAvailability = (decimal)Supplier.credit_availability;

                                if (CreditAvailability < TotalSales)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }
        #endregion
    }
}
