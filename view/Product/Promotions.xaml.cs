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

namespace Cognitivo.Product
{
    public partial class Promotions : Page
    {
        PromotionDB PromotionDB = new PromotionDB();
        CollectionViewSource sales_promotionViewSource;

        public Promotions()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            sales_promotionViewSource = FindResource("sales_promotionViewSource") as CollectionViewSource;
            PromotionDB.sales_promotion.Where(x => x.id_company == CurrentSession.Id_Company).Load();
            sales_promotionViewSource.Source = PromotionDB.sales_promotion.Local;
            cbxType.ItemsSource = Enum.GetValues(typeof(sales_promotion.Type)).OfType<sales_promotion.Type>().ToList();
        }

        #region ToolBar
        private void toolBar_btnNew_Click(object sender)
        {
            sales_promotion sales_promotion = PromotionDB.New();

            PromotionDB.sales_promotion.Add(sales_promotion);
            sales_promotionViewSource.View.Refresh();
            sales_promotionViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            sales_promotion sales_promotion = sales_promotionViewSource.View.CurrentItem as sales_promotion;
            if (sales_promotion != null)
            {
                sales_promotion.State = System.Data.Entity.EntityState.Modified;
                sales_promotionViewSource.View.Refresh();
                sales_promotionViewSource.View.MoveCurrentToLast();
            }
        }

        private void toolBar_btnSave_Click(object sender)
        {
            PromotionDB.SaveChanges();
            sales_promotion sales_promotion = sales_promotionViewSource.View.CurrentItem as sales_promotion;
            if (sales_promotion != null)
            {
                sales_promotion.State = System.Data.Entity.EntityState.Unchanged;
                sales_promotionViewSource.View.Refresh();
                sales_promotionViewSource.View.MoveCurrentToLast();
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            sales_promotion sales_promotion = sales_promotionViewSource.View.CurrentItem as sales_promotion;
            if (sales_promotion != null)
            {
                sales_promotion.State = System.Data.Entity.EntityState.Unchanged;
                sales_promotionViewSource.View.Refresh();
                sales_promotionViewSource.View.MoveCurrentToLast();
            }
        }

        private void toolBar_btnApprove_Click(object sender)
        {

        }

        private void toolBar_btnAnull_Click(object sender)
        {

        }
        #endregion

        private void dgvPromotion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sales_promotion sales_promotion = sales_promotionViewSource.View.CurrentItem as sales_promotion;
            if (sales_promotion != null)
            {
                int BonusID = Convert.ToInt32(sales_promotion.reference_bonus);
                if (BonusID>0)
                {
                    sbxBonusItem.Text = PromotionDB.items.Where(x => x.id_item == BonusID).FirstOrDefault().name;

                    int RefID = Convert.ToInt32(sales_promotion.reference);
                    sbxRefItem.Text = PromotionDB.items.Where(x => x.id_item == RefID).FirstOrDefault().name;
                }
            
            }
        }

        private void sbxRefItem_Select(object sender, RoutedEventArgs e)
        {
            if (sbxRefItem.ItemID>0)
            {
                   sales_promotion sales_promotion = sales_promotionViewSource.View.CurrentItem as sales_promotion;
                   sales_promotion.reference = sbxRefItem.ItemID;
            }

        }

        private void sbxBonusItem_Select(object sender, RoutedEventArgs e)
        {
            if (sbxBonusItem.ItemID > 0)
            {
              
                sales_promotion sales_promotion = sales_promotionViewSource.View.CurrentItem as sales_promotion;
                sales_promotion.reference_bonus = sbxBonusItem.ItemID;
            }
        }
    }
}
