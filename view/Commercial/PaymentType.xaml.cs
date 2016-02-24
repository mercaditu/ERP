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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;
using entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

namespace Cognitivo.Commercial
{
    /// <summary>
    /// Interaction logic for PaymentType.xaml
    /// </summary>
    public partial class PaymentType : Page
    {
        entity.dbContext entity = new entity.dbContext();
        CollectionViewSource payment_type_viewsource;

        public PaymentType()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            payment_type_viewsource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("payment_typeViewSource")));
            entity.db.payment_type.OrderByDescending(a => a.is_active).Load();
            payment_type_viewsource.Source = entity.db.payment_type.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.Curd.payment_type _payment_type = new cntrl.Curd.payment_type();
            payment_type payment_type = new payment_type();
            entity.db.payment_type.Add(payment_type);
            payment_type_viewsource.View.MoveCurrentToLast();
            _payment_type.objCollectionViewSource = payment_type_viewsource;
            _payment_type.entity = entity;
            crud_modal.Children.Add(_payment_type);
        }

        private void pnl_Curd_linkEdit_click(object sender, int intId)
        {
            crud_modal.Visibility = System.Windows.Visibility.Visible;
            cntrl.Curd.payment_type _payment_type = new cntrl.Curd.payment_type();
            payment_type_viewsource.View.MoveCurrentTo(entity.db.payment_type.Where(x => x.id_payment_type == intId).FirstOrDefault());
            _payment_type.objCollectionViewSource = payment_type_viewsource;
            _payment_type.entity = entity;
            crud_modal.Children.Add(_payment_type);
        }
    }
}
