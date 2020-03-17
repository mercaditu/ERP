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
    public partial class AccountFix : Page
    {
        private CollectionViewSource accountViewSource;
        private db db;

        public bool IsActive
        {
            get { return _IsActive; }
            set { _IsActive = value; }
        }

        private bool _IsActive;

        public DateTime LastUsed
        {
            get { return _LastUsed; }
            set { _LastUsed = value; }
        }

        private DateTime _LastUsed;

        public AccountFix()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            db = new db();
            string query = @"select *,count from (SELECT trans_date, comment,sum(debit) as debit,sum(credit) as credit, count(*) as count,timestamp,max(id_session) as id_session
                FROM app_account_detail
            where id_payment_detail is null and year(trans_date) > 2017 and tran_type = 2
                               group by timestamp) as detail where debit != credit and count = 2";
            DataTable dt = new DataTable();
            dt = QueryExecutor.DT(query);
            accountDataGrid.ItemsSource = dt.DefaultView;

        }

        private void accountDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listbox = (ListBox)sender;
            DataRowView row = listbox.SelectedItem as DataRowView;
            DataTable dt = new DataTable();

            string query = @"select *,app_currency.name,buy_value,sell_value,0 as status,app_currencyfx.id_currency from app_account_detail   
                           inner join app_currencyfx  on app_currencyfx.id_currencyfx = app_account_detail.id_currencyfx
                           inner join app_currency  on app_currencyfx.id_currency = app_currency.id_currency
                           where app_account_detail.timestamp = '@TimeStamp'";
            query = query.Replace("@TimeStamp", Convert.ToDateTime(row["timestamp"]).ToString("yyyy-MM-dd hh:mm:ss"));



            dt = QueryExecutor.DT(query);
            accountdetailDataGrid.ItemsSource = dt.DefaultView;


        }

        private void app_account_detailDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid datagrid = (DataGrid)sender;
            DataRowView row = datagrid.SelectedItem as DataRowView;

            Calc(row);


        }

        private void Calc(DataRowView row)
        {
            DataTable dt = new DataTable();

            if (row["id_account"] != null)
            {
                int id_account = Convert.ToInt32(row["id_account"]);

                // app_account_session app_account_session = db.app_account_session.Where(x => x.id_session == id_session).FirstOrDefault();
                app_account app_account = db.app_account.Where(x => x.id_account == id_account).FirstOrDefault();
                app_account_session app_account_session = app_account.app_account_session.LastOrDefault();
                if (app_account_session != null && app_account != null)
                {
                    IsActive = app_account_session.is_active;
                    LastUsed = app_account_session.app_account_detail.Select(x => x.trans_date).LastOrDefault();

                    int SessionID = 0;
                    //Sets the SessionID.
                    //if (app_account_session.is_active)
                    //{
                    SessionID = app_account_session.id_session;
                    //app_account.app_account_session
                    //.OrderByDescending(x => x.op_date)
                    //.Select(x => x.id_session)
                    //.FirstOrDefault();
                    //}
                    if (app_account.id_account_type == app_account.app_account_type.Bank)
                    {
                        app_account_detailDataGrid.ItemsSource =
                                              app_account_session.app_account_detail
                                          .Where
                                           (x => x.id_session == SessionID &&
                                          x.status == Status.Documents_General.Approved) //Gets only Approved Items into view.
                                          .GroupBy(ad => new { ad.app_currencyfx.id_currency })
                                          .Select(s => new
                                          {
                                              cur = s.Max(ad => ad.app_currencyfx.app_currency.name),
                                              payType = s.Max(ad => ad.payment_type.name),
                                              amount = s.Sum(ad => ad.credit) - s.Sum(ad => ad.debit)
                                          }).ToList();
                    }
                    else
                    {
                        app_account_detailDataGrid.ItemsSource =
                                          app_account_session.app_account_detail
                                      .Where
                                      (x => x.id_session == SessionID && //Gets Current Session Items Only.
                                      (x.status == Status.Documents_General.Approved || x.status == Status.Documents_General.Pending)) //Gets only Approved Items into view.
                                      .GroupBy(ad => new { ad.app_currencyfx.id_currency, ad.id_payment_type })
                                      .Select(s => new
                                      {
                                          cur = s.Max(ad => ad.app_currencyfx.app_currency.name),
                                          payType = s.Max(ad => ad.payment_type.name),
                                          amount = s.Sum(ad => ad.credit) - s.Sum(ad => ad.debit)
                                      }).ToList();
                    }

                }
                else
                {
                    IsActive = false;
                    app_account_detailDataGrid.ItemsSource = null;
                }
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            try
            {
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                throw ex;
            }

           
        }

        private void accountdetailDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataGrid datagrid = (DataGrid)sender;
            DataGridColumn col = e.Column as DataGridColumn;
            DataRowView row = datagrid.SelectedItem as DataRowView;
            row["status"] = 1;
            Int32 id_currencyfx = 0;
            if (col.Header.ToString() == "Value")
            {
                using (db dbnew = new db())
                {
                    app_currencyfx app_currencyfx = new app_currencyfx();
                    app_currencyfx.id_currency = Convert.ToInt16(row["id_currency"]);
                    app_currencyfx.buy_value = Convert.ToDecimal(row["buy_value"]);
                    app_currencyfx.sell_value = Convert.ToDecimal(row["buy_value"]);
                    app_currencyfx.is_active = false;
                    dbnew.app_currencyfx.Add(app_currencyfx);
                    dbnew.SaveChanges();
                    id_currencyfx = app_currencyfx.id_currencyfx;
                }
                    
            }
            int id_account_detail = Convert.ToInt16(row["id_account_detail"]);
            app_account_detail account_Detail = db.app_account_detail.Where(x => x.id_account_detail == id_account_detail).FirstOrDefault();
            if (account_Detail != null)
            {
                account_Detail.debit = Convert.ToDecimal(row["debit"]);
                account_Detail.credit = Convert.ToDecimal(row["credit"]);
                if(id_currencyfx > 0)
                {
                    account_Detail.id_currencyfx = id_currencyfx;
                }
              
            }
            Calc(row);


        }
    }
}
