using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using entity;

namespace Cognitivo.Configs
{
    public partial class Promotion : Page
    {
        dbContext entity = new dbContext();
        CollectionViewSource sales_promotionViewSource;

        public Promotion()
        { InitializeComponent(); }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            sales_promotionViewSource = ((CollectionViewSource)(this.FindResource("sales_promotionViewSource")));
            entity.db.sales_promotion.Where(a=>a.id_company == CurrentSession.Id_Company).Load();
            sales_promotionViewSource.Source = entity.db.sales_promotion.Local;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.promotion objPromotion = new cntrl.promotion();
            sales_promotion sales_promotion = new sales_promotion();
            sales_promotion.State = EntityState.Added;
            entity.db.sales_promotion.Add(sales_promotion);
            sales_promotionViewSource.View.MoveCurrentToLast();
            objPromotion.sales_promotionViewSource = sales_promotionViewSource;
            objPromotion.entity = entity;
            crud_modal.Children.Add(objPromotion);
        }

        private void pnl_Promotion_linkEdit_Click(object sender, int intPromotionId)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.promotion objPromotion = new cntrl.promotion();
            sales_promotion sales_promotion = entity.db.sales_promotion.Where(x => x.id_sales_promotion == intPromotionId).FirstOrDefault();
            if (sales_promotion!=null)
            {
                sales_promotion.State = EntityState.Modified;
            }
            sales_promotionViewSource.View.MoveCurrentTo(sales_promotion);
            objPromotion.sales_promotionViewSource = sales_promotionViewSource;
            objPromotion.entity = entity;
            crud_modal.Children.Add(objPromotion);
        }
    }
}
