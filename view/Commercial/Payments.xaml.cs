
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
using System.Data.Entity;
using entity;
using System.Data.Entity.Validation;
using System.ComponentModel;

namespace Cognitivo.Commercial
{
    /// <summary>
    /// Interaction logic for Payments.xaml
    /// </summary>
    public partial class Payments : Page
    {
        CollectionViewSource payment_schedualMadeViewSource, payment_schedualReceive;
        PaymentDB PaymentDB = new entity.PaymentDB();
        public Payments()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            load_Schedual();
            load_SchedualReceived();
        }
        private void load_Schedual()
        {
            payment_schedualMadeViewSource = (CollectionViewSource)FindResource("payment_schedualMadeViewSource");
            payment_schedualMadeViewSource.Source =     PaymentDB.payment_schedual
                     .Where(x => x.id_payment_detail == null && x.id_company == CurrentSession.Id_Company
                         && (x.id_sales_invoice > 0 || x.id_sales_order > 0) && x.id_note == null
                         && (x.debit - (x.child.Count() > 0 ? x.child.Sum(y => y.credit) : 0)) > 0)
                         .OrderBy(x => x.expire_date).ToList();

        }
        private void load_SchedualReceived()
        {
            payment_schedualReceive = (CollectionViewSource)FindResource("payment_schedualReceive");
            payment_schedualReceive.Source = PaymentDB.payment_schedual
                                                                    .Where(x => x.id_company == CurrentSession.Id_Company
                                                                       && (x.id_purchase_invoice > 0 || x.id_purchase_order > 0) && x.id_note == null
                                                                       && (x.credit - (x.child.Count() > 0 ? x.child.Sum(y => y.debit) : 0)) > 0).OrderBy(x => x.expire_date)
                                                                    .ToList();

        }
    }
}
