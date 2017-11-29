using entity;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace cntrl
{
    public partial class promotion : UserControl
    {
        private CollectionViewSource _sales_promotionViewSource = null;
        public CollectionViewSource sales_promotionViewSource { get { return _sales_promotionViewSource; } set { _sales_promotionViewSource = value; } }

        private entity.dbContext objentity = null;
        public entity.dbContext entity { get { return objentity; } set { objentity = value; } }

        public promotion()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {

                cbxType.ItemsSource = Enum.GetValues(typeof(sales_promotion.salesPromotion)).OfType<sales_promotion.salesPromotion>().ToList();
               
            }
        }

        public void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            entity.CancelChanges();
            sales_promotionViewSource.View.Refresh();
            Grid parentGrid = (Grid)Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }

        private void cbxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxType.SelectedItem != null)
            {
                sales_promotion.salesPromotion Type = (sales_promotion.salesPromotion)cbxType.SelectedItem;

                if (Type == sales_promotion.salesPromotion.BuyThis_GetThat)
                {
                    Curd.Promotion.BuyThis_GetThat BuyThis_GetThat = new Curd.Promotion.BuyThis_GetThat();
                    BuyThis_GetThat.sales_promotionViewSource = sales_promotionViewSource;
                    BuyThis_GetThat.entity = entity;
                    PromotionFrame.Navigate(BuyThis_GetThat);

                }
                else if (Type == sales_promotion.salesPromotion.BuyTag_GetThat)
                {
                    Curd.Promotion.BuyTag_GetThat BuyTag_GetThat = new Curd.Promotion.BuyTag_GetThat();
                    BuyTag_GetThat.sales_promotionViewSource = sales_promotionViewSource;
                    BuyTag_GetThat.entity = entity;
                    PromotionFrame.Navigate(BuyTag_GetThat);
                }
                else if (Type == sales_promotion.salesPromotion.Discount_onItem)
                {
                    Curd.Promotion.Discount_OnItem Discount_onItem = new Curd.Promotion.Discount_OnItem();
                    Discount_onItem.sales_promotionViewSource = sales_promotionViewSource;
                    Discount_onItem.entity = entity;
                    PromotionFrame.Navigate(Discount_onItem);
                }
                else if (Type == sales_promotion.salesPromotion.Discount_onTag)
                {
                    Curd.Promotion.Discount_OnTag Discount_OnTag = new Curd.Promotion.Discount_OnTag();
                    Discount_OnTag.sales_promotionViewSource = sales_promotionViewSource;
                    Discount_OnTag.entity = entity;
                    PromotionFrame.Navigate(Discount_OnTag);
                }
                else if (Type == sales_promotion.salesPromotion.Discount_onGrandTotal)
                {
                    Curd.Promotion.Discount_OnGrandTotal Discount_OnGrandTotal = new Curd.Promotion.Discount_OnGrandTotal();
                    Discount_OnGrandTotal.sales_promotionViewSource = sales_promotionViewSource;
                    Discount_OnGrandTotal.entity = entity;
                    PromotionFrame.Navigate(Discount_OnGrandTotal);
                }
                else if (Type == sales_promotion.salesPromotion.Discount_onCustomerType)
                {
                    Curd.Promotion.Discount_OnContactType Discount_OnContactType = new Curd.Promotion.Discount_OnContactType();
                    Discount_OnContactType.sales_promotionViewSource = sales_promotionViewSource;
                    Discount_OnContactType.entity = entity;
                    PromotionFrame.Navigate(Discount_OnContactType);
                }
            }
        }
    }
}