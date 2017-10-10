using entity;
using System;
using System.ComponentModel;
using System.Linq;

namespace Cognitivo.Class

{
    internal class clsTransferAmount : INotifyPropertyChanged
    {
        #region NotifyPropertyChange

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #endregion NotifyPropertyChange

        public int id_payment_type { get; set; }
        public String PaymentTypeName { get; set; }
        public int id_currencyfxdest { get; set; }
        public int id_currencydest { get; set; }
        public int? id_accountdest { get; set; }
        public int? id_accountorigin { get; set; }
        public string AccountDest { get; set; }
        public string AccountOrigin { get; set; }
        public String Currencyfxnamedest { get; set; }
        public int id_currencyfxorigin { get; set; }
        public int id_currencyorigin { get; set; }
        public String Currencyfxnameorigin { get; set; }
        public decimal amount
        {
            get
            {
                return _amount;
            }
            set
            {
                _amount = value;
                app_currencyfx originapp_currencyfx = CurrentSession.CurrencyFX_ActiveRates.Where(x => x.id_currency == id_currencyorigin).FirstOrDefault();
                app_currency dest_appcurrency = CurrentSession.Currencies.Where(x => x.id_currency == id_currencydest).FirstOrDefault();
                if (originapp_currencyfx != null)
                {
                    if (dest_appcurrency!=null)
                    {
                        amountCounted = Math.Round(entity.Brillo.Currency.convert_Values(value, originapp_currencyfx.id_currencyfx, dest_appcurrency,FXRate, entity.App.Modules.Sales),2);
                        RaisePropertyChanged("amountCounted");
                    }
                  
                }
              
               
            }
        }
        decimal _amount;
        public decimal amountCounted { get; set; }
        public decimal FXRate
        {
            get
            {
                return _FXRate;
            }
            set
            {
                _FXRate = value;
                app_currencyfx originapp_currencyfx = CurrentSession.CurrencyFX_ActiveRates.Where(x => x.id_currency == id_currencyorigin).FirstOrDefault();
                app_currency dest_appcurrency = CurrentSession.Currencies.Where(x => x.id_currency == id_currencydest).FirstOrDefault();
                if (originapp_currencyfx != null)
                {
                    if (dest_appcurrency != null)
                    {
                        amountCounted = Math.Round(entity.Brillo.Currency.convert_Values(amount, originapp_currencyfx.id_currencyfx, dest_appcurrency, value, entity.App.Modules.Sales), 2);
                        RaisePropertyChanged("amountCounted");
                    }

                }


            }
        }
        decimal _FXRate;
    }
}