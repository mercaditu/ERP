using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.Entity;
using entity;

namespace Cognitivo.Configs
{
    /// <summary>
    /// Interaction logic for CurrencyRecords.xaml
    /// </summary>
    public partial class Currency : Page
    {
        entity.dbContext entity = new entity.dbContext();
        CollectionViewSource app_currencyViewSource;
        private int _IdCurrency;
      //  entity.Properties.Settings _entity = new entity.Properties.Settings();

        public Currency()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            app_currencyViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("app_currencyViewSource")));
            entity.db.app_currency.Where(a => a.id_company == CurrentSession.Id_Company).OrderByDescending(a => a.is_active).Load();
            app_currencyViewSource.Source = entity.db.app_currency.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.currency objCur = new cntrl.currency();
            objCur.CurrencyId = 0;
            _IdCurrency = 0;
            crud_modal.Children.Add(objCur);
        }

        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (crud_modal.Visibility == System.Windows.Visibility.Hidden)
            {
                app_currency objCurrency = entity.db.app_currency.Where(x => x.id_currency == _IdCurrency).FirstOrDefault();
                if (objCurrency != null)
                    entity.db.Entry<app_currency>(objCurrency).Reload();
                entity.db.app_currency.Where(a => a.id_company == CurrentSession.Id_Company).OrderByDescending(a => a.is_active).Load();
                app_currencyViewSource.Source = entity.db.app_currency.Local;
                app_currencyViewSource.View.Refresh();
            }
        }

        private void pnl_Currency_linkEdit_Click(object sender, int intCurrencyId)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.currency objCur = new cntrl.currency();
            objCur.CurrencyId = intCurrencyId;
            _IdCurrency = intCurrencyId;
            crud_modal.Children.Add(objCur);
        }
    }
}
