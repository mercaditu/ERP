using entity;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Configs
{
    public partial class Currency : Page
    {
        private dbContext entity = new dbContext();
        private CollectionViewSource app_currencyViewSource;
        private int _IdCurrency;

        public Currency()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            app_currencyViewSource = ((CollectionViewSource)(this.FindResource("app_currencyViewSource")));
            entity.db.app_currency.Where(a => a.id_company == CurrentSession.Id_Company).OrderByDescending(a => a.is_active).Load();
            app_currencyViewSource.Source = entity.db.app_currency.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.currency objCur = new cntrl.currency();
            objCur.CurrencyId = 0;
            _IdCurrency = 0;
            crud_modal.Children.Add(objCur);
        }

        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (crud_modal.Visibility == Visibility.Hidden)
            {
                app_currency app_currency = entity.db.app_currency.Where(x => x.id_currency == _IdCurrency).FirstOrDefault();
                if (app_currency != null)
                    entity.db.Entry(app_currency).Reload();

                entity.db.app_currency.Where(a => a.id_company == CurrentSession.Id_Company).OrderByDescending(a => a.is_active).Load();
                app_currencyViewSource.Source = entity.db.app_currency.Local;
                app_currencyViewSource.View.Refresh();
            }
        }

        private void pnl_Currency_linkEdit_Click(object sender, int intCurrencyId)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.currency objCur = new cntrl.currency();
            objCur.CurrencyId = intCurrencyId;
            _IdCurrency = intCurrencyId;
            crud_modal.Children.Add(objCur);
        }
    }
}