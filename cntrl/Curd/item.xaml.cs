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
    /// <summary>
    /// Interaction logic for item.xaml
    /// </summary>
    public partial class item : UserControl
    {
        entity.Properties.Settings _settings = new entity.Properties.Settings();
        public bool isValid { get; set; }

        CollectionViewSource _itemViewSource = null;
        public CollectionViewSource itemViewSource { get { return _itemViewSource; } set { _itemViewSource = value; } }

        CollectionViewSource _MainViewSource = null;
        public CollectionViewSource MainViewSource { get { return _MainViewSource; } set { _MainViewSource = value; } }
        public object curObject { get; set; }

        private entity.dbContext __entity = null;
        public entity.dbContext _entity { get { return __entity; } set { __entity = value; } }

        private Class.clsCommon.Mode _operationMode = 0;
        public Class.clsCommon.Mode operationMode { get { return _operationMode; } set { _operationMode = value; } }

        private SearchableTextbox _SearchableTextbox = null;
        public SearchableTextbox STbox { get { return _SearchableTextbox; } set { _SearchableTextbox = value; } }

        private entity.item _itemobject = null;
        public entity.item itemobject { get { return _itemobject; } set { _itemobject = value; } }

        public item()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                CollectionViewSource item_brandViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("item_brandViewSource")));                    
                item_brandViewSource.Source = __entity.db.item_brand.Where(a => a.id_company == _settings.company_ID).OrderBy(a => a.name).ToList();

                CollectionViewSource app_vat_groupViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("app_vat_groupViewSource")));
                app_vat_groupViewSource.Source = __entity.db.app_vat_group.Where(a => a.is_active == true && a.id_company == _settings.company_ID).OrderBy(a => a.name).ToList();

                CollectionViewSource app_currencyViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("app_currencyViewSource")));
                app_currencyViewSource.Source = __entity.db.app_currency.Where(a => a.is_active == true && a.id_company == _settings.company_ID).OrderBy(a => a.name).ToList();

                CollectionViewSource item_price_listViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("item_price_listViewSource")));
                item_price_listViewSource.Source = __entity.db.item_price_list.Where(a => a.is_active == true && a.id_company == _settings.company_ID).OrderBy(a => a.name).ToList();

                MainViewSource.View.MoveCurrentTo(curObject);
                if (operationMode == Class.clsCommon.Mode.Add)
                {
                    entity.item newItem = new entity.item();
                    _entity.db.items.Add(newItem);
                    itemViewSource.View.MoveCurrentToLast();
                }
                else
                {
                    itemViewSource.View.MoveCurrentTo(itemobject);
                    btnDelete.Visibility = System.Windows.Visibility.Visible;
                }
                stackMain.DataContext = itemViewSource;

               cmbitem.ItemsSource = Enum.GetValues(typeof(entity.item.item_type)); 
            }
        }

        private void btnCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!isValid && operationMode == Class.clsCommon.Mode.Add)
            {
                _entity.db.items.Remove(itemViewSource.View.CurrentItem as entity.item);
            }
            else if (!isValid && operationMode == Class.clsCommon.Mode.Edit)
            {
                if (_entity.db.Entry(itemobject).State == EntityState.Modified)
                {
                    _entity.db.Entry(itemobject).State = EntityState.Unchanged;
                }
            }
            itemViewSource.View.Refresh();
            if (itemobject != null)
            {
                STbox.Text = itemobject.name;
                STbox.Data = itemobject;
            }
            MainViewSource.View.Refresh();
            MainViewSource.View.MoveCurrentTo(curObject);
            Grid crud = this.Parent as Grid;
            crud.Children.Clear();
            crud.Visibility = System.Windows.Visibility.Hidden;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                entity.item myitem = itemViewSource.View.CurrentItem as entity.item;
                if (string.IsNullOrEmpty(myitem.Error))
                {
                    isValid = true;
                    btnCancel_MouseDown(sender, null);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                entity.item myitem = itemViewSource.View.CurrentItem as entity.item;
                myitem.is_active = false;

                itemViewSource.View.Filter = i =>
                {
                    entity.item objitem = (entity.item)i;
                    if (objitem.is_active == true)
                        return true;
                    else
                        return false;
                };
            }
        }

        private void cmbitem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            entity.item item = itemViewSource.View.CurrentItem as entity.item;
            if (item != null)
            {
                //Product
                if (item.id_item_type == entity.item.item_type.Product)
                {
                    if (item.item_product.Count == 0)
                    {
                        item_product _product = new item_product();
                        item.item_product.Add(_product);
                        //itemitem_productViewSource.View.Refresh();
                        //itemitem_productViewSource.View.MoveCurrentTo(_product);
                    }
                    if (item.item_asset.Count > 0)
                    {
                        List<item_asset> records = item.item_asset.ToList();
                        foreach (var record in records)
                        {
                            __entity.db.item_asset.Remove(record);
                        }
                    }
                }
                //Searvice
                else if (item.id_item_type == entity.item.item_type.Service)
                {
                    if (item.item_asset.Count > 0)
                    {
                        List<item_asset> records = item.item_asset.ToList();
                        foreach (var record in records)
                        {
                            __entity.db.item_asset.Remove(record);
                        }
                    }
                    if (item.item_product.Count > 0)
                    {
                        List<item_product> records = item.item_product.ToList();
                        foreach (var record in records)
                        {
                            __entity.db.item_product.Remove(record);
                        }
                    }
                }
                //Capital Resource
                else if (item.id_item_type == entity.item.item_type.FixedAssets)
                {
                    if (item.item_asset.Count == 0)
                    {
                        item_asset _capital = new item_asset();
                        item.item_asset.Add(_capital);
                        //itemitem_capitalViewSource.View.Refresh();
                        //itemitem_capitalViewSource.View.MoveCurrentTo(_capital);
                    }
                    if (item.item_product.Count > 0)
                    {
                        List<item_product> records = item.item_product.ToList();
                        foreach (var record in records)
                        {
                            __entity.db.item_product.Remove(record);
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
