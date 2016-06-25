using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using entity;
using System.ComponentModel;

namespace cntrl.Controls
{
    public partial class ImpexCurrencyBox : UserControl, INotifyPropertyChanged
    {
        List<app_currency> app_currencyList;

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

        public static DependencyProperty SelectedValueProperty
            = DependencyProperty.Register("SelectedValue", typeof(int), typeof(ImpexCurrencyBox), new PropertyMetadata(OnCurrencyChangeCallBack));

        public static readonly DependencyProperty TransDateProperty = DependencyProperty.Register("TransDate", typeof(DateTime), typeof(ImpexCurrencyBox));
        public DateTime TransDate
        {
            get { return (DateTime)GetValue(TransDateProperty); }
            set { SetValue(TransDateProperty, value); }
        }

        #region "INotifyPropertyChanged"
        private static void OnCurrencyChangeCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ImpexCurrencyBox c = sender as ImpexCurrencyBox;
            c = sender as ImpexCurrencyBox;
            if (c != null)
            {
                c.OnCurrencyChange((int)e.NewValue);
            }
        }

        protected virtual void OnCurrencyChange(int newvalue)
        {
            using (db db = new db())
            {
                if (db.app_currencyfx.Where(x => x.id_currencyfx == newvalue).FirstOrDefault() != null)
                {
                    cbCurrency.SelectedValue = db.app_currencyfx.Where(x => x.id_currencyfx == newvalue).FirstOrDefault().app_currency.id_currency;
                   
                }
            }
        }
        #endregion

        public ImpexCurrencyBox()
        {
            InitializeComponent();
        }

        private void CurrencyBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                using (db db = new db())
                {
                    entity.Properties.Settings _setting = new entity.Properties.Settings();
                    app_currencyList = db.app_currency.Where(x => x.id_company == _setting.company_ID && x.is_active == true).ToList();
                    cbCurrency.ItemsSource = app_currencyList;
                }
            }
        }

        private void cbCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RaisePropertyChanged("id_currency");
            using (db db = new db())
            {
                app_currencyfx app_currencyfx;
                List<app_currencyfx> app_currencyfxlist=db.app_currencyfx.ToList();
                app_currencyfx = app_currencyfxlist.Where(x => x.id_currency == (int)id_currency && x.type == entity.app_currencyfx.CurrencyFXTypes.Impex && x.timestamp.Date == TransDate.Date).FirstOrDefault();

                    if (app_currencyfx != null)
                    {
                        Rate_Previous = Rate_Current;
                        if (appName!=null)
                        {
                            if (appName==App.Names.PurchaseInvoice ||appName==App.Names.PurchaseOrder ||appName==App.Names.PurchaseTender)
                            {
                                Rate_Current = app_currencyfx.sell_value;
                            }
                            else
                            {
                                Rate_Current = app_currencyfx.buy_value;
                            }
                        }
                    

                        SetValue(SelectedValueProperty, app_currencyfx.id_currencyfx);
                        RaisePropertyChanged("Rate_Current");
                    }
                    else
                    { Rate_Current = 0.0M; }
              
            }
        }

        private void lblExchangeValue_LostFocus(object sender, RoutedEventArgs e)
        {
            using (db db = new db())
            {
                if (SelectedValue > 0)
                {
                    app_currencyfx app_currencyfx = new app_currencyfx();
                    app_currencyfx = db.app_currencyfx.Where(x => x.id_currencyfx == SelectedValue).FirstOrDefault();

                    decimal rate=0;
                     if (appName != null)
                        {
                            if (appName == App.Names.PurchaseInvoice || appName == App.Names.PurchaseOrder ||appName==App.Names.PurchaseTender)
                            {
                                rate = app_currencyfx.sell_value;
                            }
                            else
                            {
                                rate = app_currencyfx.buy_value;
                            }
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
            using (db db = new db())
            {
                if (db.app_currency.Where(x => x.id_currency == id_currency).FirstOrDefault() != null)
                {
                    app_currencyfx app_currencyfx = new app_currencyfx();
                    app_currencyfx.id_currency = id_currency;
                    app_currencyfx.app_currency = db.app_currency.Where(x => x.id_currency == id_currency).FirstOrDefault();
                    app_currencyfx.buy_value = Rate_Current;
                    app_currencyfx.sell_value = Rate_Current;
                    app_currencyfx.is_active = false;
                    app_currencyfx.type = entity.app_currencyfx.CurrencyFXTypes.Impex;
                    db.app_currencyfx.Add(app_currencyfx);
                    db.SaveChanges();
                    return app_currencyfx.id_currencyfx;
                }
                return 0;
            }
        }

        public void get_DefaultCurrencyActiveRate()
        {
            using (db db = new db())
            {
                int company_ID = entity.Properties.Settings.Default.company_ID;
                if (SelectedValue == 0)
                {


                    if (db.app_currencyfx.Where(x => x.is_active && x.app_currency.is_priority && x.id_company == company_ID) != null)
                    {
                        app_currencyfx app_currencyfx = db.app_currencyfx.Where(x => x.is_active
                                                                             && x.app_currency.is_priority
                                                                             && x.id_company == company_ID)
                                                                         .FirstOrDefault();
                        if (app_currencyfx != null && app_currencyfx.id_currencyfx > 0)
                        {
                            SelectedValue = Convert.ToInt32(app_currencyfx.id_currencyfx);
                        }
                        else
                        {
                            SelectedValue = 1;
                            // cbCurrency.SelectedValue = -1;
                        }
                    }
                }
            }
        }

        public void get_ActiveRateXContact(ref contact contact)
        {
            app_currencyfx app_currencyfx = null;
            if (contact.app_currency != null && contact.app_currency.app_currencyfx != null && contact.app_currency.app_currencyfx.Count > 0)
            {
                app_currencyfx = contact.app_currency.app_currencyfx.Where(a => a.is_active == true).First();
            }

            if (app_currencyfx != null && app_currencyfx.id_currencyfx > 0)
            { SelectedValue = Convert.ToInt32(app_currencyfx.id_currencyfx); }
            else
            {
                SelectedValue =1;
               // cbCurrency.SelectedValue = -1;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
