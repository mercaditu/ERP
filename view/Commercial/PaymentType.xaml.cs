using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using entity;

namespace Cognitivo.Commercial
{
    public partial class PaymentType : Page
    {
        dbContext entity = new dbContext();
        CollectionViewSource payment_type_viewsource;

        public PaymentType()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            payment_type_viewsource = ((CollectionViewSource)(this.FindResource("payment_typeViewSource")));
            await entity.db.payment_type.Where(x => x.id_company == CurrentSession.Id_Company).OrderByDescending(a => a.is_active).LoadAsync();
            payment_type_viewsource.Source = entity.db.payment_type.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
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
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.payment_type _payment_type = new cntrl.Curd.payment_type();
            payment_type_viewsource.View.MoveCurrentTo(entity.db.payment_type.Where(x => x.id_payment_type == intId).FirstOrDefault());
            _payment_type.objCollectionViewSource = payment_type_viewsource;
            _payment_type.entity = entity;
            crud_modal.Children.Add(_payment_type);
        }
    }
}
