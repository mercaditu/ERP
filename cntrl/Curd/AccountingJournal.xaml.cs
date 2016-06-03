using System;
using System.Collections.Generic;
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
using entity;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace cntrl.Curd
{
    /// <summary>
    /// Interaction logic for AccountingJournal.xaml
    /// </summary>
    public partial class AccountingJournal : UserControl
    {
        public accounting_journal accounting_journal { get; set; }
        entity.Properties.Settings _setting = new entity.Properties.Settings();
        public db db { get; set; }
        CollectionViewSource accounting_journalViewSource, accounting_journal_detailViewSource;
        public AccountingJournal()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            accounting_journalViewSource = ((CollectionViewSource)(FindResource("accounting_journalViewSource")));
            accounting_journal_detailViewSource = ((CollectionViewSource)(FindResource("accounting_journal_detailViewSource")));
            if (accounting_journal != null)
            {
                db.accounting_journal.Where(x => x.id_journal == accounting_journal.id_journal).Load();
                accounting_journalViewSource.Source = db.accounting_journal.Local;
                //if (accounting_journal_detailDataGrid != null)
                //{
                //    accounting_journal_detailDataGrid.ItemsSource = db.accounting_journal_detail.Where(x => x.id_journal == accounting_journal.id_journal).ToList();
                //}
            }
            else
            {
                accounting_journal = new accounting_journal();
                accounting_journal.id_cycle = db.accounting_cycle.Where(x => x.id_company == _setting.company_ID && x.is_active == true).FirstOrDefault().id_cycle;
                db.accounting_journal.Add(accounting_journal);

                accounting_journalViewSource.Source = db.accounting_journal.Local;
                //if (accounting_journal_detailDataGrid != null)
                //{
                //    accounting_journal_detailDataGrid.ItemsSource = accounting_journal.accounting_journal_detail;
                //}
                accounting_journalViewSource.View.MoveCurrentTo(accounting_journal);
            }


            CollectionViewSource accounting_chartViewSource = ((CollectionViewSource)(FindResource("accounting_chartViewSource")));
            accounting_chartViewSource.Source = db.accounting_chart.Where(x => x.id_company == _setting.company_ID).ToList();

            CollectionViewSource app_currencyfxViewSource = ((CollectionViewSource)(FindResource("app_currencyfxViewSource")));
            List<app_currencyfx> app_currencyfxList = db.app_currencyfx.Where(x => x.id_company == _setting.company_ID && x.is_active
                && x.type == app_currencyfx.CurrencyFXTypes.Accounting).ToList();
            app_currencyfxViewSource.Source = app_currencyfxList.Where(x =>x.timestamp.Date == accounting_journal.trans_date.Date).ToList();


        }


        private void accounting_journal_detailDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataGridColumn column = e.Column;
            if (column.Header.ToString() == "Currency")
            {

                accounting_journal_detail accounting_journal_detail = (accounting_journal_detail)e.Row.Item;
                int code = accounting_journal_detail.accounting_journal.code;
                int id_chart = accounting_journal_detail.accounting_journal.id_cycle;
                List<accounting_journal_detail> lst_accounting_journal_detail = db.accounting_journal_detail.Where(x => x.id_company == _setting.company_ID && x.accounting_journal.code == code && x.accounting_journal.id_cycle == id_chart).ToList();

                int id = 0;
                id = accounting_journal_detail.id_currencyfx;
                if (db.app_currencyfx.Where(x => x.id_currencyfx == id).FirstOrDefault() != null)
                {
                    accounting_journal_detail.app_currencyfx = db.app_currencyfx.Where(x => x.id_company == _setting.company_ID && x.id_currencyfx == id).FirstOrDefault();
                    accounting_journal_detail.RaisePropertyChanged("app_currencyfx");
                    foreach (accounting_journal_detail item in lst_accounting_journal_detail)
                    {
                        item.id_currencyfx = id;
                        item.app_currencyfx = db.app_currencyfx.Where(x => x.id_company == _setting.company_ID && x.id_currencyfx == id).FirstOrDefault();
                        item.RaisePropertyChanged("app_currencyfx");
                        item.RaisePropertyChanged("id_currencyfx");
                    }
                }

            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (accounting_journal.accounting_journal_detail.Sum(x => x.credit) == accounting_journal.accounting_journal_detail.Sum(x => x.debit))
            {
                db.SaveChanges();
                btnCancel_Click(sender, null);
            }
            else
            {
                MessageBox.Show("Verify balance :-" + accounting_journal.code);
            }
        }

        private void btnCancel_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {

                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception ex)
            { throw ex; }
        }

        //private void accounting_journal_detailDataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        //{
        //       accounting_journal_detail accounting_journal_detail = (accounting_journal_detail)e.NewItem;
        //    //    IEnumerable<DbEntityValidationResult> validationresult = db.GetValidationErrors();
        //    //    accounting_journal accounting_journal;
        //    //    if (validationresult.Count() == 0)
        //    //    {
        //    //        if (accounting_journal_detail.id_journal_detail == 0)
        //    //        {
        //    //            accounting_journal = new accounting_journal();
        //    //            accounting_journal.id_cycle = db.accounting_cycle.Where(x => x.id_company == _setting.company_ID && x.is_active == true).FirstOrDefault().id_cycle; accounting_journal.comment = "entry from journal";
        //    //            accounting_journal_detail.accounting_journal = accounting_journal;
        //    //            accounting_journal.accounting_journal_detail.Add(accounting_journal_detail);
        //    //           // db.accounting_journal.Add(accounting_journal);

        //    //        }
        //    //        else
        //    //        {
        //       accounting_journal_detail.accounting_journal = accounting_journal;
        //    // }




        //    // }

        //}

        private void cbxAccount_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && cbxAccount.Data != null)
            {
                int id = Convert.ToInt32(((accounting_chart)cbxAccount.Data).id_chart);
                if (id > 0)
                {
                    accounting_journal accounting_journal = accounting_journalViewSource.View.CurrentItem as accounting_journal;
                    if (accounting_journal != null)
                    {
                        accounting_journal_detail _accounting_journal_detail = new entity.accounting_journal_detail();
                        _accounting_journal_detail.accounting_journal = accounting_journal;
                        if (db.app_currencyfx.Where(x => x.id_company == _setting.company_ID && x.is_active && x.app_currency.is_priority).FirstOrDefault()!=null)
                        {
                            _accounting_journal_detail.id_currencyfx = db.app_currencyfx.Where(x => x.id_company == _setting.company_ID && x.is_active && x.app_currency.is_priority).FirstOrDefault().id_currencyfx;
                            _accounting_journal_detail.app_currencyfx = db.app_currencyfx.Where(x => x.id_company == _setting.company_ID && x.is_active && x.app_currency.is_priority).FirstOrDefault();          
                        }
                  
                        _accounting_journal_detail.is_head = true;
                        _accounting_journal_detail.accounting_chart = (accounting_chart)cbxAccount.Data;
                        _accounting_journal_detail.id_chart = id;
                        accounting_journal.accounting_journal_detail.Add(_accounting_journal_detail);
                        accounting_journalViewSource.View.Refresh();
                        accounting_journal_detailViewSource.View.Refresh();
                    }
                }
            }
        }

        private void cbxAccount_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            int id = Convert.ToInt32(((accounting_chart)cbxAccount.Data).id_chart);
            if (id > 0)
            {
                accounting_journal accounting_journal = accounting_journalViewSource.View.CurrentItem as accounting_journal;
                if (accounting_journal != null)
                {

                    accounting_journal_detail _accounting_journal_detail = new entity.accounting_journal_detail();
                    _accounting_journal_detail.accounting_journal = accounting_journal;
                    if (db.app_currencyfx.Where(x => x.id_company == _setting.company_ID && x.is_active && x.app_currency.is_priority).FirstOrDefault() != null)
                    {
                        _accounting_journal_detail.id_currencyfx = db.app_currencyfx.Where(x => x.id_company == _setting.company_ID && x.is_active && x.app_currency.is_priority).FirstOrDefault().id_currencyfx;
                        _accounting_journal_detail.app_currencyfx = db.app_currencyfx.Where(x => x.id_company == _setting.company_ID && x.is_active && x.app_currency.is_priority).FirstOrDefault();
                    }
                    _accounting_journal_detail.is_head = true;
                    _accounting_journal_detail.accounting_chart = (accounting_chart)cbxAccount.Data;
                    _accounting_journal_detail.id_chart = id;
                    accounting_journal.accounting_journal_detail.Add(_accounting_journal_detail);


                    accounting_journalViewSource.View.Refresh();
                    accounting_journal_detailViewSource.View.Refresh();

                }

            }
        }

    }
}
