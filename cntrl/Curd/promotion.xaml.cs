using entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace cntrl
{
    public partial class promotion : UserControl
    {
        private CollectionViewSource item_tagViewSource, app_currencyViewSource, contact_tagViewSource;
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
                stackMain.DataContext = sales_promotionViewSource;

                item_tagViewSource = FindResource("item_tagViewSource") as CollectionViewSource;

                entity.db.item_tag.Where(x => x.id_company == CurrentSession.Id_Company).Load();
                item_tagViewSource.Source = entity.db.item_tag.Local;

                contact_tagViewSource = FindResource("contact_tagViewSource") as CollectionViewSource;

                entity.db.contact_tag.Where(x => x.id_company == CurrentSession.Id_Company).Load();
                contact_tagViewSource.Source = entity.db.contact_tag.Local;

                app_currencyViewSource = FindResource("app_currencyViewSource") as CollectionViewSource;
                entity.db.app_currency.Where(x => x.id_company == CurrentSession.Id_Company).Load();
                app_currencyViewSource.Source = entity.db.app_currency.Local;

                cbxType.ItemsSource = Enum.GetValues(typeof(sales_promotion.salesPromotion)).OfType<sales_promotion.salesPromotion>().ToList();

                cbxparaContacttag.SelectedValue = -1;
                cbxcurrency.SelectedIndex = -1;
                cbxparatag.SelectedIndex = -1;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (entity.db.GetValidationErrors().Count() == 0)
            {
                entity.SaveChanges();
                btnCancel_Click(sender, e);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            entity.CancelChanges();
            sales_promotionViewSource.View.Refresh();
            Grid parentGrid = (Grid)Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = Visibility.Hidden;
        }

        private void sbxRefItem_Select(object sender, RoutedEventArgs e)
        {
            if (sbxRefItem.ItemID > 0)
            {
                if (sales_promotionViewSource.View.CurrentItem is sales_promotion sales_promotion)
                {
                    sales_promotion.reference = sbxRefItem.ItemID;
                }
            }
        }

        private void cbxparaContacttag_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxparaContacttag.SelectedValue != null)
            {
                if (sales_promotionViewSource.View.CurrentItem is sales_promotion sales_promotion)
                {
                    if (sales_promotion.type == sales_promotion.salesPromotion.Discount_onCustomerType)
                    {
                        if (sales_promotion.id_sales_promotion > 0)
                        {
                            cbxparaContacttag.SelectedValue = sales_promotion.reference;
                        }
                        else
                        {
                            sales_promotion.reference = Convert.ToInt32(cbxparaContacttag.SelectedValue);
                        }
                    }
                }
            }
        }

        private void sbxBonusItem_Select(object sender, RoutedEventArgs e)
        {
            if (sbxBonusItem.ItemID > 0)
            {
                if (sales_promotionViewSource.View.CurrentItem is sales_promotion sales_promotion)
                {
                    sales_promotion.reference_bonus = sbxBonusItem.ItemID;
                }
            }
        }

        private void cbxparatag_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxparatag.SelectedValue != null)
            {
                if (sales_promotionViewSource.View.CurrentItem is sales_promotion sales_promotion)
                {
                    if (sales_promotion.type == sales_promotion.salesPromotion.BuyTag_GetThat || sales_promotion.type == sales_promotion.salesPromotion.Discount_onTag)
                    {
                        if (cbxparatag.SelectedValue.ToString() == "0")
                        {
                            cbxparatag.SelectedValue = sales_promotion.reference;
                        }
                        else
                        {
                            sales_promotion.reference = Convert.ToInt32(cbxparatag.SelectedValue);
                        }

                    }
                }
            }
        }

        private void cbxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sales_promotion sales_promotion = sales_promotionViewSource.View.CurrentItem as sales_promotion;

            if (sales_promotion.type == sales_promotion.salesPromotion.BuyThis_GetThat)
            {
                Total_Parameter.Visibility = Visibility.Collapsed;
                Tag_Parameter.Visibility = Visibility.Collapsed;
                Item_Parameter.Visibility = Visibility.Visible;
                Item_Bonus.Visibility = Visibility.Visible;
                Discount.Visibility = Visibility.Collapsed;
                QuntityStep.Visibility = Visibility.Visible;
                ContactTag_Parameter.Visibility = Visibility.Collapsed;
                item input = entity.db.items.Find(sales_promotion.reference);
                if (input != null)
                {
                    sales_promotion.InputName = input.name;
                    sales_promotion.RaisePropertyChanged("InputName");
                }

                item output = entity.db.items.Find(sales_promotion.reference_bonus);
                if (output != null)
                {
                    sales_promotion.OutputName = output.name;
                    sales_promotion.RaisePropertyChanged("OutputName");
                }
            }
            else if (sales_promotion.type == sales_promotion.salesPromotion.BuyTag_GetThat)
            {
                Total_Parameter.Visibility = Visibility.Collapsed;
                Tag_Parameter.Visibility = Visibility.Visible;
                Item_Parameter.Visibility = Visibility.Collapsed;
                Item_Bonus.Visibility = Visibility.Visible;
                Discount.Visibility = Visibility.Collapsed;
                QuntityStep.Visibility = Visibility.Visible;
                ContactTag_Parameter.Visibility = Visibility.Collapsed;

                item output = entity.db.items.Find(sales_promotion.reference_bonus);

                if (output != null)
                {
                    sales_promotion.OutputName = output.name;
                    sales_promotion.RaisePropertyChanged("OutputName");
                }
                QuntityStep.Visibility = Visibility.Visible;
            }
            else if (sales_promotion.type == sales_promotion.salesPromotion.Discount_onItem)
            {
                Total_Parameter.Visibility = Visibility.Collapsed;
                Tag_Parameter.Visibility = Visibility.Collapsed;
                Item_Parameter.Visibility = Visibility.Visible;
                Item_Bonus.Visibility = Visibility.Collapsed;
                Discount.Visibility = Visibility.Visible;
                ContactTag_Parameter.Visibility = Visibility.Collapsed;
                item input = entity.db.items.Find(sales_promotion.reference);
                if (input != null)
                {
                    sales_promotion.InputName = input.name;
                    sales_promotion.RaisePropertyChanged("InputName");
                }
                QuntityStep.Visibility = Visibility.Visible;
            }
            else if (sales_promotion.type == sales_promotion.salesPromotion.Discount_onTag)
            {
                Total_Parameter.Visibility = Visibility.Collapsed;
                Tag_Parameter.Visibility = Visibility.Visible;
                Item_Parameter.Visibility = Visibility.Collapsed;
                Item_Bonus.Visibility = Visibility.Collapsed;
                Discount.Visibility = Visibility.Visible;
                QuntityStep.Visibility = Visibility.Visible;
                ContactTag_Parameter.Visibility = Visibility.Collapsed;
            }
            else if (sales_promotion.type == sales_promotion.salesPromotion.Discount_onGrandTotal)
            {
                Total_Parameter.Visibility = Visibility.Visible;
                Tag_Parameter.Visibility = Visibility.Collapsed;
                Item_Parameter.Visibility = Visibility.Collapsed;
                Item_Bonus.Visibility = Visibility.Collapsed;
                Discount.Visibility = Visibility.Visible;
                QuntityStep.Visibility = Visibility.Collapsed;
            }
            else if (sales_promotion.type == sales_promotion.salesPromotion.Discount_onCustomerType)
            {
                Total_Parameter.Visibility = Visibility.Collapsed;
                Tag_Parameter.Visibility = Visibility.Collapsed;
                Item_Parameter.Visibility = Visibility.Collapsed;
                Item_Bonus.Visibility = Visibility.Collapsed;
                Discount.Visibility = Visibility.Visible;
                QuntityStep.Visibility = Visibility.Visible;
                ContactTag_Parameter.Visibility = Visibility.Visible;
            }
        }
    }
}