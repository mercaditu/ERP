using entity;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace cntrl.Controls
{
    public partial class CurrencyBox : UserControl, INotifyPropertyChanged
    {
        public decimal Rate_Previous { get; set; }
        public decimal Rate_Current { get; set; }
        public int id_currency { get; set; }
        public App.Names appName { get; set; }

        public int SelectedValue
        {
            get
            {
                return (int)GetValue(SelectedValueProperty);
            }
            set
            {
                SetValue(SelectedValueProperty, value);
            }
        }

        public static DependencyProperty SelectedValueProperty = DependencyProperty.Register("SelectedValue", typeof(int), typeof(CurrencyBox), new PropertyMetadata(OnCurrencyChangeCallBack));

        #region "INotifyPropertyChanged"

        private static void OnCurrencyChangeCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CurrencyBox c = sender as CurrencyBox;
            if (c != null)
            {
                c.OnCurrencyChange((int)e.NewValue);
            }
        }

        protected virtual void OnCurrencyChange(int newvalue)
        {
            using (db db = new db())
            {
                app_currencyfx app_currencyfx = db.app_currencyfx.Where(x => x.id_currencyfx == newvalue).FirstOrDefault();

                if (app_currencyfx != null)
                {
                    cbCurrency.SelectedValue = app_currencyfx.app_currency.id_currency;
                }
            }
        }

        #endregion "INotifyPropertyChanged"

        public CurrencyBox()
        {
            InitializeComponent();
        }

        private void cbCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RaisePropertyChanged("id_currency");
            app_currencyfx app_currencyfx = CurrentSession.CurrencyFX_ActiveRates.Where(x => x.id_currency == id_currency).FirstOrDefault();

            if (app_currencyfx != null)
            {
                Rate_Previous = Rate_Current;

                if (appName == App.Names.PurchaseInvoice || appName == App.Names.PurchaseOrder || appName == App.Names.PurchaseTender)
                {
                    Rate_Current = app_currencyfx.sell_value;
                }
                else
                {
                    Rate_Current = app_currencyfx.buy_value;
                }

                RaisePropertyChanged("Rate_Current");
                SetValue(SelectedValueProperty, app_currencyfx.id_currencyfx);
            }
            else
            { Rate_Current = 0.0M; }
        }

        private async void lblExchangeValue_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SelectedValue > 0)
            {
                using (db db = new db())
                {
                    app_currencyfx app_currencyfx = await db.app_currencyfx.FindAsync(SelectedValue);

                    decimal rate = 0;

                    if (appName == App.Names.PurchaseInvoice || appName == App.Names.PurchaseOrder || appName == App.Names.PurchaseTender)
                    {
                        rate = app_currencyfx.sell_value;
                    }
                    else
                    {
                        rate = app_currencyfx.buy_value;
                    }
                    if (Convert.ToDecimal(Rate_Current) != rate)
                    {
                        if (cbCurrency.SelectedValue != null)
                        {
                            int id = (int)cbCurrency.SelectedValue;
                            SetValue(SelectedValueProperty, save_Rate(id));
                        }
                    }
                }
            }
        }

        private int save_Rate(int id_currency)
        {
            if (CurrentSession.Currencies.Where(x => x.id_currency == id_currency) != null)
            {
                app_currencyfx app_currencyfx = new app_currencyfx();
                app_currencyfx.id_currency = id_currency;
                app_currencyfx.buy_value = Rate_Current;
                app_currencyfx.sell_value = Rate_Current;
                app_currencyfx.is_active = false;

                using (db db = new db())
                {
                    db.app_currencyfx.Add(app_currencyfx);
                    db.SaveChanges();
                }

                return app_currencyfx.id_currencyfx;
            }
            return 0;
        }

        public void get_DefaultCurrencyActiveRate()
        {
            if (SelectedValue == 0)
            {
                app_currencyfx app_currencyfx = CurrentSession.Get_Currency_Default_Rate();

                if (app_currencyfx != null && app_currencyfx.id_currencyfx > 0)
                {
                    SelectedValue = Convert.ToInt32(app_currencyfx.id_currencyfx);
                }
                else
                {
                    SelectedValue = 1;
                }
            }
        }

        public void get_ActiveRateXContact(ref contact contact)
        {
            int CurrencyID = (contact.app_currency != null ? contact.app_currency.id_currency : 0);

            //Company Default Currency && Contact Currency are the same. Use default currency if Contact does not have currency assigned.
            if (CurrencyID == 0)
            {
                SelectedValue = CurrentSession.Get_Currency_Default_Rate().id_currencyfx;
            }
            else //Company Default Currency is not same as Customers. Customer might be empty too. We need to check.
            {
                app_currencyfx app_currencyfx = CurrentSession.CurrencyFX_ActiveRates.Where(x => x.id_currency == CurrencyID).FirstOrDefault();
                if (app_currencyfx != null)
                {
                    SelectedValue = app_currencyfx.id_currencyfx;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}