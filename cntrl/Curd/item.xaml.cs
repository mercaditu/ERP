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
        private db db = new db();

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
                if (db != null)
                {
                    List<app_vat_group> app_vat_groupList = new List<app_vat_group>();
                    List<app_currency> app_currencyList = new List<app_currency>();
                    List<item_price_list> item_price_listList = new List<item_price_list>();
                    
                    using (db _db = new db())
                    {
                        _db.Configuration.AutoDetectChangesEnabled = false;

                        app_vat_groupList = _db.app_vat_group.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).AsNoTracking().ToList();
                        if (app_vat_groupList.Count > 0)
                        {
                            CollectionViewSource app_vat_groupViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("app_vat_groupViewSource")));
                            app_vat_groupViewSource.Source = app_vat_groupList;
                        }

                        app_currencyList = _db.app_currency.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).AsNoTracking().ToList();
                        if (app_currencyList.Count > 0)
                        {
                            CollectionViewSource app_currencyViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("app_currencyViewSource")));
                            app_currencyViewSource.Source = db.app_currency.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToList();
                        }

                        item_price_listList = _db.item_price_list.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).AsNoTracking().ToList();
                        if (item_price_listList.Count > 0)
                        {
                            CollectionViewSource item_price_listViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("item_price_listViewSource")));
                            item_price_listViewSource.Source = item_price_listList;
                        }
                    }

                    cmbitem.ItemsSource = Enum.GetValues(typeof(entity.item.item_type));

                    if (itemobject != null)
                    {
                        db.items.Add(itemobject);
                    }

                    itemViewSource = (CollectionViewSource)this.FindResource("itemViewSource");
                    itemViewSource.Source = db.items.Local;
                    itemViewSource.View.MoveCurrentTo(itemobject);
                }
            }
        }

        public event btnCancel_ClickedEventHandler btnCancel_Click;
        public delegate void btnCancel_ClickedEventHandler(object sender);
        private void btnCancel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (btnCancel_Click != null)
            {
                btnCancel_Click(sender);
            }
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
                            db.item_asset.Remove(record);
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
                            db.item_asset.Remove(record);
                        }
                    }
                    if (item.item_product.Count > 0)
                    {
                        List<item_product> records = item.item_product.ToList();
                        foreach (var record in records)
                        {
                            db.item_product.Remove(record);
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
                            db.item_product.Remove(record);
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
