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
        CollectionViewSource sales_promotionViewSource, item_tagViewSource, item_tagBonusViewSource;

        public Promotions()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            sales_promotionViewSource = FindResource("sales_promotionViewSource") as CollectionViewSource;
            await PromotionDB.sales_promotion.Where(x => x.id_company == CurrentSession.Id_Company).LoadAsync();
            sales_promotionViewSource.Source = PromotionDB.sales_promotion.Local;

            item_tagViewSource = FindResource("item_tagViewSource") as CollectionViewSource;
            item_tagBonusViewSource = FindResource("item_tagBonusViewSource") as CollectionViewSource;
            await PromotionDB.item_tag.Where(x => x.id_company == CurrentSession.Id_Company).LoadAsync();
            item_tagViewSource.Source = PromotionDB.item_tag.Local;
            item_tagBonusViewSource.Source = PromotionDB.item_tag.Local;

            cbxType.ItemsSource = Enum.GetValues(typeof(sales_promotion.Type)).OfType<sales_promotion.Type>().ToList();
        }

        #region ToolBar
        private void toolBar_btnNew_Click(object sender)
        {
            sales_promotion sales_promotion = PromotionDB.New();
            sales_promotion.IsSelected = true;
            PromotionDB.sales_promotion.Add(sales_promotion);
            sales_promotionViewSource.View.Refresh();
            sales_promotionViewSource.View.MoveCurrentTo(sales_promotion);
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            sales_promotion sales_promotion = sales_promotionViewSource.View.CurrentItem as sales_promotion;
            if (sales_promotion != null)
            {
                sales_promotion.IsSelected = true;
                sales_promotion.State = System.Data.Entity.EntityState.Modified;
                sales_promotionViewSource.View.Refresh();
                sales_promotionViewSource.View.MoveCurrentTo(sales_promotion);
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
                sales_promotionViewSource.View.MoveCurrentTo(sales_promotion);
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            sales_promotion sales_promotion = sales_promotionViewSource.View.CurrentItem as sales_promotion;
            if (sales_promotion != null)
            {
                sales_promotion.State = System.Data.Entity.EntityState.Unchanged;
                sales_promotionViewSource.View.Refresh();
                sales_promotionViewSource.View.MoveCurrentTo(sales_promotion);
            }
        }
        #endregion

        private void dgvPromotion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sales_promotion sales_promotion = sales_promotionViewSource.View.CurrentItem as sales_promotion;

            if (sales_promotion != null)
            {
                //Buy Item and get Bonus Item
                if (sales_promotion.types == entity.sales_promotion.Type.BuyThis_GetThat ||
                    sales_promotion.types == entity.sales_promotion.Type.BuyTag_GetThat)
                {
                    int BonusID = Convert.ToInt32(sales_promotion.reference_bonus);
                    if (BonusID > 0)
                    {
                        sbxBonusItem.Text = PromotionDB.items.Where(x => x.id_item == BonusID).FirstOrDefault().name;

                        int RefID = Convert.ToInt32(sales_promotion.reference);
                        sbxRefItem.Text = PromotionDB.items.Where(x => x.id_item == RefID).FirstOrDefault().name;
                    }

                    cbxType_SelectionChanged(null, null);
                }
            }
        }

        private void sbxRefItem_Select(object sender, RoutedEventArgs e)
        {
            if (sbxRefItem.ItemID > 0)
            {
                sales_promotion sales_promotion = sales_promotionViewSource.View.CurrentItem as sales_promotion;
                if (sales_promotion != null)
                {
                    sales_promotion.reference = sbxRefItem.ItemID;
                }
            }
        }

        private void sbxBonusItem_Select(object sender, RoutedEventArgs e)
        {
            if (sbxBonusItem.ItemID > 0)
            {
                sales_promotion sales_promotion = sales_promotionViewSource.View.CurrentItem as sales_promotion;
                if (sales_promotion != null)
                {
                    sales_promotion.reference_bonus = sbxBonusItem.ItemID;
                }
            }
        }

        private void cbxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sales_promotion sales_promotion = sales_promotionViewSource.View.CurrentItem as sales_promotion;
            
            //
            if (sales_promotion.types == entity.sales_promotion.Type.BuyThis_GetThat)
            {
                Tag_Parameter.Visibility = System.Windows.Visibility.Collapsed;
                Tag_Bonus.Visibility = System.Windows.Visibility.Collapsed;
                Item_Parameter.Visibility = System.Windows.Visibility.Visible;
                Item_Bonus.Visibility = System.Windows.Visibility.Visible;
            }
            //Buy Tag and get Bonus Item
            else if (sales_promotion.types == entity.sales_promotion.Type.BuyTag_GetThat)
            {
                Tag_Parameter.Visibility = System.Windows.Visibility.Visible;
                Tag_Bonus.Visibility = System.Windows.Visibility.Visible;
                Item_Parameter.Visibility = System.Windows.Visibility.Collapsed;
                Item_Bonus.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
