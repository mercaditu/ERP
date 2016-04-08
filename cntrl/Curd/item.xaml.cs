using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using entity;

namespace cntrl.Curd
{
    public partial class item : UserControl
    {
        public entity.dbContext entity { get; set; }

        private entity.item _itemobject = null;
        public entity.item itemobject { get { return _itemobject; } set { _itemobject = value; } }
        public List<entity.item> itemList { get; set; }

        CollectionViewSource itemViewSource;

        public item()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                using (db db = new db())
                {
                    if (db.item_brand.Where(a => a.id_company == CurrentSession.Id_Company) != null)
                    {
                        CollectionViewSource item_brandViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("item_brandViewSource")));
                        item_brandViewSource.Source = db.item_brand.Where(a => a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToList();
                    }

                    if (db.app_vat_group.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company) != null)
                    {
                        CollectionViewSource app_vat_groupViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("app_vat_groupViewSource")));
                        app_vat_groupViewSource.Source = db.app_vat_group.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToList();
                    }

                    if (db.app_currency.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company) != null)
                    {
                        CollectionViewSource app_currencyViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("app_currencyViewSource")));
                        app_currencyViewSource.Source = db.app_currency.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToList();
                    }

                    if (db.item_price_list.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company) != null)
                    {
                        CollectionViewSource item_price_listViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("item_price_listViewSource")));
                        item_price_listViewSource.Source = db.item_price_list.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToList();
                    }
                }

                cmbitem.ItemsSource = Enum.GetValues(typeof(entity.item.item_type));

                itemList = new List<global::entity.item>();

                if (itemobject != null)
                {
                    itemList.Add(itemobject);
                }

                itemViewSource = (CollectionViewSource)this.FindResource("itemViewSource");
                itemViewSource.Source = itemList;
                itemViewSource.View.Refresh();
                itemViewSource.View.MoveCurrentToFirst();
            }
        }

        private void btnCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Primitives.Popup popup = this.Parent as System.Windows.Controls.Primitives.Popup;
            popup.IsOpen = false;
            popup.Visibility = System.Windows.Visibility.Collapsed;
        }

        public event btnSave_ClickedEventHandler btnSave_Click;
        public delegate void btnSave_ClickedEventHandler(object sender);
        public void btnSave_MouseUp(object sender, EventArgs e)
        {


            if (btnSave_Click != null)
            {
                btnSave_Click(sender);
            }
        }

        private void cmbitem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            entity.item item = itemViewSource.View.CurrentItem as entity.item;
            if (item != null)
            {
                //Product
                if (item.id_item_type == global::entity.item.item_type.Product)
                {
                    if (item.item_product.Count == 0)
                    {
                        item_product _product = new item_product();
                        item.item_product.Add(_product);
                    }
                    if (item.item_asset.Count > 0)
                    {
                        List<item_asset> records = item.item_asset.ToList();
                        foreach (var record in records)
                        {
                            entity.db.item_asset.Remove(record);
                        }
                    }
                }
                //Searvice
                else if (item.id_item_type == global::entity.item.item_type.Service)
                {
                    if (item.item_asset.Count > 0)
                    {
                        List<item_asset> records = item.item_asset.ToList();
                        foreach (var record in records)
                        {
                            entity.db.item_asset.Remove(record);
                        }
                    }
                    if (item.item_product.Count > 0)
                    {
                        List<item_product> records = item.item_product.ToList();
                        foreach (var record in records)
                        {
                            entity.db.item_product.Remove(record);
                        }
                    }
                }
                //Capital Resource
                else if (item.id_item_type == global::entity.item.item_type.FixedAssets)
                {
                    if (item.item_asset.Count == 0)
                    {
                        item_asset _capital = new item_asset();
                        item.item_asset.Add(_capital);
                    }
                    if (item.item_product.Count > 0)
                    {
                        List<item_product> records = item.item_product.ToList();
                        foreach (var record in records)
                        {
                            entity.db.item_product.Remove(record);
                        }
                    }
                }
            }
            //Product
        }

        private void item_priceDataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            entity.item item = itemViewSource.View.CurrentItem as entity.item;
            entity.item_price item_price = e.NewItem as entity.item_price;
            if (item != null)
            {
                item_price.id_item = item.id_item;
                item_price.item = item;
            }
        }
    }
}
