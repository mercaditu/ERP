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
                                var BalanceList = db.payment_schedual
                                        .Where(x => x.id_contact == Customer.id_contact)
                                        .GroupBy(ps => new { ps.app_currencyfx.id_currency })
                                        .Select(s => new
                                        {
                                            CurrencyID = s.Max(ps => ps.app_currencyfx.id_currency),
                                            Balance = s.Sum(ps => ps.debit - ps.credit)
                                        }).ToList();

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
                                    db.payment_schedual
                                    .Where(x => x.id_contact == Supplier.id_contact)
                                    .GroupBy(ps => new { ps.app_currencyfx.id_currency })
                                    .Select(s => new
                                    {
                                        CurrencyID = s.Max(ps => ps.app_currencyfx.id_currency),
                                        Balance = s.Sum(ps => ps.credit - ps.debit)
                                    }).ToList();

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
