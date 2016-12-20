using System;
using System.Windows.Controls;
using System.Windows.Data;
using entity;
using System.Linq;
using System.Data.Entity;
using System.Timers;
using System.Collections.Generic;

namespace cntrl.Chart
{
    public partial class AccountReceivableControl : UserControl //, INotifyPropertyChanged
    {
        CollectionViewSource payment_schedualViewSource;


        public AccountReceivableControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Load_BasicData(sender, null);

            Timer myTimer = new Timer();
            myTimer.Elapsed += new ElapsedEventHandler(Load_BasicData);
            myTimer.Interval = 120000;
            myTimer.Start();
        }
        public void Load_BasicData(object sender, ElapsedEventArgs e)
        {
            List<payment_schedual> payment_schedualList = new List<payment_schedual>();
            db db = new db();


                payment_schedualList = db.payment_schedual
                   .Where(x => x.id_payment_detail == null && x.id_company == CurrentSession.Id_Company
                       && (x.id_sales_invoice > 0 || x.id_sales_order > 0) && x.id_note == null
                       && (x.debit - (x.child.Count() > 0 ? x.child.Sum(y => y.credit) : 0)) > 0 && x.expire_date < DateTime.Now)
                       .OrderBy(x => x.expire_date).ToList();

            Dispatcher.InvokeAsync(new Action(() =>
            {
                payment_schedualViewSource = (CollectionViewSource)FindResource("payment_schedualViewSource");
                if (payment_schedualViewSource != null)
                { payment_schedualViewSource.Source = payment_schedualList; }

            }));







        }
    }
}

