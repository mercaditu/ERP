
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
        CollectionViewSource payment_detailMadeViewSource, payment_detailReceive;
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
            payment_detailMadeViewSource = (CollectionViewSource)FindResource("payment_detailMadeViewSource");
            payment_detailMadeViewSource.Source = PaymentDB.payment_detail
                     .Where(x => x.id_company == CurrentSession.Id_Company && x.payment_schedual.Where(y=>y.credit>0).Count()==0)                         
                         .ToList();

        }
        private void load_SchedualReceived()
        {
            payment_detailReceive = (CollectionViewSource)FindResource("payment_detailReceive");
            payment_detailReceive.Source = PaymentDB.payment_detail
                     .Where(x => x.id_company == CurrentSession.Id_Company && x.payment_schedual.Where(y => y.debit > 0).Count() == 0)
                         .ToList();

        }

        private void EditCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as payment_detail != null)
            {
                e.CanExecute = true;
            }
        }

        private void EditCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            payment_detail payment_detail = e.Parameter as payment_detail;


            cntrl.Curd.payment_display payment_display = new cntrl.Curd.payment_display(cntrl.Curd.payment_display.Modes.Recievable, payment_detail.payment.id_contact, payment_detail.payment.id_payment, payment_detail.id_payment_detail);

            crud_modal.Visibility = System.Windows.Visibility.Visible;
            crud_modal.Children.Add(payment_display);
        }
    }
}
