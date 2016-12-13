using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using entity;
using System.Data.Entity;

namespace Cognitivo.Product
{
    public partial class Promotions : Page
    {
        PromotionDB PromotionDB = new PromotionDB();
        CollectionViewSource sales_promotionViewSource, item_tagViewSource, item_tagBonusViewSource, app_currencyViewSource;

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

            app_currencyViewSource = FindResource("app_currencyViewSource") as CollectionViewSource;
            await PromotionDB.app_currency.Where(x => x.id_company == CurrentSession.Id_Company).LoadAsync();
            app_currencyViewSource.Source = PromotionDB.app_currency.Local;
            

            cbxType.ItemsSource = Enum.GetValues(typeof(sales_promotion.Types)).OfType<sales_promotion.Types>().ToList();
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
                sales_promotion.State = EntityState.Modified;
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
              
                sales_promotion.State = EntityState.Unchanged;
                sales_promotionViewSource.View.Refresh();
                sales_promotionViewSource.View.MoveCurrentTo(sales_promotion);
            }

        }

        private void toolBar_btnCancel_Click(object sender)
        {
            sales_promotion sales_promotion = sales_promotionViewSource.View.CurrentItem as sales_promotion;
            if (sales_promotion != null)
            {
                sales_promotion.State = EntityState.Unchanged;
                sales_promotionViewSource.View.Refresh();
                sales_promotionViewSource.View.MoveCurrentTo(sales_promotion);
            }
        }
        #endregion

        private void dgvPromotion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbxType_SelectionChanged(null, null);
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

            if (sales_promotion.type == entity.sales_promotion.Types.BuyThis_GetThat)
            {
                Total_Parameter.Visibility = Visibility.Collapsed;
                Tag_Parameter.Visibility = Visibility.Collapsed;
                Tag_Bonus.Visibility = Visibility.Collapsed;
                Item_Parameter.Visibility = Visibility.Visible;
                Item_Bonus.Visibility = Visibility.Visible;
                Discount.Visibility = Visibility.Collapsed;
                QuntityStep.Visibility = Visibility.Visible;

                item input = PromotionDB.items.Find(sales_promotion.reference);
                if (input != null)
                {
                    sales_promotion.InputName = input.name;
                    sales_promotion.RaisePropertyChanged("InputName");
                }

                item output = PromotionDB.items.Find(sales_promotion.reference_bonus);

                if (output != null)
                {
                    sales_promotion.OutputName = output.name;
                    sales_promotion.RaisePropertyChanged("OutputName");
                }
            }
            else if (sales_promotion.type == entity.sales_promotion.Types.BuyTag_GetThat)
            {
                Total_Parameter.Visibility = Visibility.Collapsed;
                Tag_Parameter.Visibility = Visibility.Visible;
                Tag_Bonus.Visibility = Visibility.Visible;
                Item_Parameter.Visibility = Visibility.Collapsed;
                Item_Bonus.Visibility = Visibility.Collapsed;
                Discount.Visibility = Visibility.Collapsed;
                QuntityStep.Visibility = Visibility.Visible;
                sales_promotion.reference = Convert.ToInt32(cbxparatag.SelectedValue);
                sales_promotion.reference_bonus = Convert.ToInt32(cbxbonustag.SelectedValue);
                item output = PromotionDB.items.Find(sales_promotion.reference_bonus);

                if (output != null)
                {
                    sales_promotion.OutputName = output.name;
                    sales_promotion.RaisePropertyChanged("OutputName");
                }
                 QuntityStep.Visibility = Visibility.Visible;
            }
            else if (sales_promotion.type == entity.sales_promotion.Types.Discount_onItem)
            {
                Total_Parameter.Visibility = Visibility.Collapsed;
                Tag_Parameter.Visibility = Visibility.Collapsed;
                Tag_Bonus.Visibility = Visibility.Collapsed;
                Item_Parameter.Visibility = Visibility.Visible;
                Item_Bonus.Visibility = Visibility.Collapsed;
                Discount.Visibility = Visibility.Visible;

                item input = PromotionDB.items.Find(sales_promotion.reference);
                if (input != null)
                {
                    sales_promotion.InputName = input.name;
                    sales_promotion.RaisePropertyChanged("InputName");
                }
                QuntityStep.Visibility = Visibility.Visible;
            }
            else if (sales_promotion.type == entity.sales_promotion.Types.Discount_onTag)
            {
                Total_Parameter.Visibility = Visibility.Collapsed;
                Tag_Parameter.Visibility = Visibility.Visible;
                Tag_Bonus.Visibility = Visibility.Collapsed;
                Item_Parameter.Visibility = Visibility.Collapsed;
                Item_Bonus.Visibility = Visibility.Collapsed;
                Discount.Visibility = Visibility.Visible;
                QuntityStep.Visibility = Visibility.Visible;
                sales_promotion.reference = Convert.ToInt32(cbxparatag.SelectedValue);
                
            }
            else if (sales_promotion.type == entity.sales_promotion.Types.Discount_onGrandTotal)
            {
                Total_Parameter.Visibility = Visibility.Visible;
                Tag_Parameter.Visibility = Visibility.Collapsed;
                Tag_Bonus.Visibility = Visibility.Collapsed;
                Item_Parameter.Visibility = Visibility.Collapsed;
                Item_Bonus.Visibility = Visibility.Collapsed;
                Discount.Visibility = Visibility.Visible;
                QuntityStep.Visibility = Visibility.Collapsed;
                sales_promotion.reference = Convert.ToInt32(cbxcurrency.SelectedValue);
            }
        }
    }
}
