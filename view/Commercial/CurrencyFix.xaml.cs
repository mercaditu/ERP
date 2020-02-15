using entity;
using entity.BrilloQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cognitivo.Commercial
{
    
    /// <summary>
    /// Interaction logic for CurrencyFix.xaml
    /// </summary>
    public partial class CurrencyFix : Page
    {
        private CollectionViewSource currencyViewSource;
        private db db;

        public CurrencyFix()
        {
            InitializeComponent();
            currencyViewSource = FindResource("currencyViewSource") as CollectionViewSource;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            db = new db();
            db.app_currency.Where(x=>x.id_company == CurrentSession.Id_Company).Load();
            currencyViewSource.Source = db.app_currency.Local;
        }

        private void currencyDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listbox = (ListBox)sender;
            app_currency row = listbox.SelectedItem as app_currency;
            DataTable dt = new DataTable();

            string query = @"SELECT app_currencyfx.id_currencyfx,app_currencyfx.timestamp,buy_value,sell_value,app_company.name as CompanyName
                FROM app_currencyfx
                inner join app_company on app_company.id_company = app_currencyfx.id_company
                inner join app_currency on app_currency.id_currency = app_currencyfx.id_currency
                where app_currencyfx.id_currency =@CurrencyID";
            query = query.Replace("@CurrencyID", row.id_currency.ToString());
            
            dt = QueryExecutor.DT(query);
            currencyfxDataGrid.ItemsSource = dt.DefaultView;
        }

        private void currencyfxDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid datagrid = (DataGrid)sender;
            DataRowView row = datagrid.SelectedItem as DataRowView;
            DataTable dt = new DataTable();

            if(row !=null)
            {
                string query = @"SELECT app_account_detail.trans_date,app_account.name,debit,credit,comment
                FROM app_account_detail
                inner join app_account on app_account_detail.id_account = app_account.id_account
                where app_account_detail.id_currencyfx =@CurrencyfxID";
                query = query.Replace("@CurrencyfxID", row["id_currencyfx"].ToString());

                dt = QueryExecutor.DT(query);
                AccountDetaildataDataGrid.ItemsSource = dt.DefaultView;
            }
            
        }
    }
}
