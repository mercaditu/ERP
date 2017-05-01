using entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Cognitivo.Product
{
    public partial class Item : Page, IDisposable
    {
        private entity.Controller.Product.ItemController ItemDB;
        private CollectionViewSource itemViewSource,
            itemitem_priceViewSource,
            itemitem_dimentionViewSource,
            item_brandViewSource,
            app_vat_groupViewSource,
            item_price_listViewSource,
            itemitem_productViewSource,
            app_dimentionViewSource,
            app_propertyViewSource,
            itemitem_tagdetailViewSource,
            hr_talentViewSource,
            itemitem_serviceViewSource,
            item_templateViewSource,
            item_templateitem_template_detaildetailViewSource,
            itemitem_conversion_factorViewSource;

        public bool CanSeeCost { get; set; }

        public Item()
        {
            CanSeeCost = CurrentSession.UserRole.see_cost;

            InitializeComponent();
            ItemDB = FindResource("ItemDB") as entity.Controller.Product.ItemController;
            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {
                //Load Controller.
                ItemDB.Initialize();
            }

            itemViewSource = FindResource("itemViewSource") as CollectionViewSource;
            itemitem_priceViewSource = FindResource("itemitem_priceViewSource") as CollectionViewSource;
            itemitem_dimentionViewSource = FindResource("itemitem_dimentionViewSource") as CollectionViewSource;
            itemitem_productViewSource = FindResource("itemitem_productViewSource") as CollectionViewSource;
            itemitem_tagdetailViewSource = FindResource("itemitem_tagdetailViewSource") as CollectionViewSource;
            item_brandViewSource = FindResource("item_brandViewSource") as CollectionViewSource;
            item_price_listViewSource = FindResource("item_price_listViewSource") as CollectionViewSource;
            app_vat_groupViewSource = FindResource("app_vat_groupViewSource") as CollectionViewSource;
            app_dimentionViewSource = FindResource("app_dimentionViewSource") as CollectionViewSource;
            app_propertyViewSource = FindResource("app_propertyViewSource") as CollectionViewSource;
            hr_talentViewSource = FindResource("hr_talentViewSource") as CollectionViewSource;
            itemitem_serviceViewSource = FindResource("itemitem_serviceViewSource") as CollectionViewSource;
            item_templateViewSource = FindResource("item_templateViewSource") as CollectionViewSource;
            item_templateitem_template_detaildetailViewSource = FindResource("item_templateitem_template_detaildetailViewSource") as CollectionViewSource;
            itemitem_conversion_factorViewSource = FindResource("itemitem_conversion_factorViewSource") as CollectionViewSource;
        }

        private void Load_PrimaryData(object sender, EventArgs e)
        {

            Load_PrimaryDataThread();
            Load_SecondaryDataThread();
        }

        private async void Load_PrimaryDataThread()
        {
            ItemDB.Load(ProductSettings.Default.Product, ProductSettings.Default.RawMaterial, ProductSettings.Default.Supplies,
                         ProductSettings.Default.Task, ProductSettings.Default.Service, ProductSettings.Default.ServiceContract);

            await Dispatcher.InvokeAsync(new Action(() =>
            {
                itemViewSource.Source = ItemDB.db.items.Local;
            }));
        }

        private async void Load_SecondaryDataThread()
        {


            await Dispatcher.InvokeAsync(new Action(() =>
            {
                CollectionViewSource app_measurementViewSource = ((CollectionViewSource)(FindResource("app_measurementViewSource")));
                CollectionViewSource app_measurementViewSourceconvert = ((CollectionViewSource)(FindResource("app_measurementViewSourceconvert")));
                CollectionViewSource app_measurementViewSourcenew = ((CollectionViewSource)(FindResource("app_measurementViewSourcenew")));

                app_measurementViewSource.Source = ItemDB.db.app_measurement.Local;
                app_measurementViewSourceconvert.Source = ItemDB.db.app_measurement.Local;
                app_measurementViewSourcenew.Source = ItemDB.db.app_measurement.Local;
            }));

            await Dispatcher.InvokeAsync(new Action(() =>
            {
                app_dimentionViewSource.Source = ItemDB.db.app_dimension.Local;
                app_propertyViewSource.Source = ItemDB.db.app_property.Local;
                app_vat_groupViewSource.Source = CurrentSession.VAT_Groups;
                CollectionViewSource item_tagViewSource = FindResource("item_tagViewSource") as CollectionViewSource;
                item_tagViewSource.Source = ItemDB.db.item_tag.Local;
                item_templateViewSource = ((CollectionViewSource)(FindResource("item_templateViewSource")));
                item_templateViewSource.Source = ItemDB.db.item_template.Local;
                CollectionViewSource app_currencyViewSource = ((CollectionViewSource)(FindResource("app_currencyViewSource")));
                app_currencyViewSource.Source = CurrentSession.Currencies; //ItemDB.app_currency.Local;
                hr_talentViewSource.Source = ItemDB.db.hr_talent.Local;
                item_price_listViewSource.Source = CurrentSession.PriceLists;
                item_brandViewSource.Source = ItemDB.db.item_brand.Local;
                toolBar.IsEnabled = true;
            }));
        }

        private void Item_Loaded(object sender, RoutedEventArgs e)
        {
            //Loads Primary and Secondary Data
            Load_PrimaryData(null, null);

            cmbitem.ItemsSource = Enum.GetValues(typeof(item.item_type)).OfType<item.item_type>().Where(x => x != item.item_type.FixedAssets).ToList();
        }

        #region Toolbar Events

        private void Cancel_Click(object sender)
        {
            item item = itemDataGrid.SelectedItem as item;

            if (item != null)
            {
                item_priceDataGrid.CancelEdit();
                itemViewSource.View.MoveCurrentToFirst();

                if (item.State == EntityState.Added)
                {
                    ItemDB.db.Entry(item).State = EntityState.Detached;
                }
                else
                {
                    item.State = EntityState.Unchanged;
                    ItemDB.db.Entry(item).State = EntityState.Unchanged;
                }

                itemViewSource.View.Refresh();
            }
        }

        private void Delete_Click(object sender)
        {
            MessageBoxResult res = MessageBox.Show(entity.Brillo.Localize.Question_Delete, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                item item = (item)itemDataGrid.SelectedItem;
                item.is_active = false;
                itemViewSource.View.Filter = i =>
                {
                    item objitem = (item)i;
                    if (objitem.is_active == true)
                        return true;
                    else
                        return false;
                };
            }
        }

        private void Edit_Click(object sender)
        {
            if (itemDataGrid.SelectedItem != null)
            {
                item item = itemDataGrid.SelectedItem as item;
                item.IsSelected = true;
                item.State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning(entity.Brillo.Localize.PleaseSelect);
            }
        }

        private void New_Click(object sender)
        {
            item item = ItemDB.Create();
            ItemDB.db.items.Add(item);

            itemViewSource.View.Refresh();
            itemViewSource.View.MoveCurrentTo(item);
        }

        private void cmbitem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            item item = itemViewSource.View.CurrentItem as item;

            if (item != null)
            {
                //Product
                if (item.id_item_type == item.item_type.Product
                    || item.id_item_type == item.item_type.RawMaterial
                    || item.id_item_type == item.item_type.Supplies)
                {
                    if (item.item_product.Count == 0 || item.item_product == null)
                    {
                        if (itemitem_productViewSource.View != null)
                        {
                            item_product _product = new item_product();
                            item.item_product.Add(_product);
                            itemitem_productViewSource.View.Refresh();
                            itemitem_productViewSource.View.MoveCurrentTo(_product);
                        }
                    }

                    if (item.item_asset.Count > 0)
                    {
                        List<item_asset> records = item.item_asset.ToList();
                        foreach (var record in records)
                        {
                            ItemDB.db.item_asset.Remove(record);
                        }
                    }
                }
                //Service
                else if (item.id_item_type == item.item_type.Service || item.id_item_type == entity.item.item_type.ServiceContract)
                {
                    if (item.item_asset.Count > 0)
                    {
                        List<item_asset> records = item.item_asset.ToList();
                        foreach (var record in records)
                        {
                            ItemDB.db.item_asset.Remove(record);
                        }
                    }
                    if (item.item_product.Count > 0)
                    {
                        List<item_product> records = item.item_product.ToList();
                        foreach (var record in records)
                        {
                            ItemDB.db.item_product.Remove(record);
                        }
                    }
                }
            }
        }

        private void Save_Click(object sender)
        {

            //Check if exact same name exist with the same name. Check if the product is not the same so as not to affect already inserted items.
            item item = itemViewSource.View.CurrentItem as item;

            if (item != null)
            {
                if (ItemDB.db.items.Any(x => x.name == item.name && x.id_item != item.id_item))
                {
                    toolBar.msgWarning("Product: " + item.name + " Already Exists..");
                    return;
                }

                if (ItemDB.SaveChanges_WithValidation())
                {
                    // Save Changes
                    itemViewSource.View.Refresh();
                    toolBar.msgSaved(ItemDB.NumberOfRecords);
                }
            }

        }

        private void Search_Click(object sender, string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                itemViewSource.View.Filter = i =>
                {
                    item item = i as item;
                    if (item.Error == null)
                    {
                        if (item.name.ToLower().Contains(query.ToLower()) || item.code.ToLower().Contains(query.ToLower())) //item.item_tag_detail.Where(x => x.item_tag.name.ToLower().Contains(query.ToLower())).Any()
                        {
                            return true;
                        }
                    }

                    return false;
                };
            }
            else
            {
                itemViewSource.View.Filter = null;
            }
        }

        #endregion Toolbar Events

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as item_price != null)
            {
                e.CanExecute = true;
            }
            if (e.Parameter as item_dimension != null)
            {
                e.CanExecute = true;
            }
            if (e.Parameter as item_tag_detail != null)
            {
                e.CanExecute = true;
            }
            if (e.Parameter as item_conversion_factor != null)
            {
                e.CanExecute = true;
            }
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(entity.Brillo.Localize.Question_Delete, "Cognitivo ERP", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                if (e.Parameter as item_price != null)
                {
                    item_priceDataGrid.CancelEdit();
                    ItemDB.db.item_price.Remove(e.Parameter as item_price);
                    itemitem_priceViewSource.View.Refresh();
                }
                if (e.Parameter as item_dimension != null)
                {
                    item_dimentionDataGrid.CancelEdit();
                    ItemDB.db.item_dimension.Remove(e.Parameter as item_dimension);
                    itemitem_dimentionViewSource.View.Refresh();
                }

                if (e.Parameter as item_tag_detail != null)
                {
                    item_tag_detailDataGrid.CancelEdit();
                    ItemDB.db.item_tag_detail.Remove(e.Parameter as item_tag_detail);
                    itemitem_tagdetailViewSource.View.Refresh();
                }
                if (e.Parameter as item_conversion_factor != null)
                {
                    item_conversion_factorDataGrid.CancelEdit();
                    ItemDB.db.item_conversion_factor.Remove(e.Parameter as item_conversion_factor);
                    itemitem_conversion_factorViewSource.View.Refresh();
                }
            }
        }

        private void hrefCost_Click(object sender, RoutedEventArgs e)
        {
            //TODO WRITE CODE HERE...
        }

        private void ItemTag_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            item item = itemViewSource.View.CurrentItem as item;
            if (item != null)
            {
                foreach (item_tag_detail TagDetial in item.item_tag_detail)
                {

                }
            }
        }

        private void tbCustomize_MouseUp(object sender, MouseButtonEventArgs e)
        {
            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            popupCustomize.StaysOpen = false;
            popupCustomize.IsOpen = true;
        }

        private void popupCustomize_Closed(object sender, EventArgs e)
        {
            popupCustomize.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            ProductSettings.Default.Save();

            ProductSettings _pref_Product = new ProductSettings();
            _pref_Product = ProductSettings.Default;
            popupCustomize.IsOpen = false;

            Load_PrimaryDataThread();

            toolBar.msgWarning("Close and Open Window to see Changes");
        }

        private void item_priceDataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            item item = itemDataGrid.SelectedItem as item;
            item_price item_price = e.NewItem as item_price;
            if (item != null)
            {
                item_price.id_item = item.id_item;
                item_price.item = item;
            }
        }

        #region Config Add/Edit

        private void AddBrand_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.item_brand item_brand = new cntrl.Curd.item_brand()
            {
                item_brandViewSource = item_brandViewSource,
                MainViewSource = itemViewSource,
                curObject = itemViewSource.View.CurrentItem,
                operationMode = cntrl.Class.clsCommon.Mode.Add,
                isExternalCall = true
            };

            crud_modal.Children.Add(item_brand);
        }

        private void EditBrand_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            item_brand _item_brand = cbxItemBrand.SelectedItem as item_brand;
            if (_item_brand != null)
            {
                crud_modal.Visibility = Visibility.Visible;
                cntrl.Curd.item_brand item_brand = new cntrl.Curd.item_brand()
                {
                    item_brandViewSource = item_brandViewSource,
                    MainViewSource = itemViewSource,
                    curObject = itemViewSource.View.CurrentItem,
                    item_brandobject = _item_brand,
                    operationMode = cntrl.Class.clsCommon.Mode.Edit,
                    isExternalCall = true
                };

                crud_modal.Children.Add(item_brand);
            }
        }

        #endregion Config Add/Edit

        private void cbxTag_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Add_Tag();
            }
        }

        private void cbxTag_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Add_Tag();
        }

        private void Add_Tag()
        {
            if (cbxTag.Data != null)
            {
                int id = Convert.ToInt32(((item_tag)cbxTag.Data).id_tag);
                if (id > 0)
                {
                    if (itemViewSource.View.CurrentItem is item item)
                    {
                        item_tag_detail item_tag_detail = new item_tag_detail()
                        {
                            id_tag = ((item_tag)cbxTag.Data).id_tag,
                            item_tag = ((item_tag)cbxTag.Data)
                        };
                        item.item_tag_detail.Add(item_tag_detail);
                        itemitem_tagdetailViewSource.View.Refresh();
                    }
                }
            }
        }

        private void btnadd_Click(object sender, RoutedEventArgs e)
        {
            item item = (item)itemViewSource.View.CurrentItem;
            if (item != null)
            {
                item_service item_service = new item_service()
                {
                    hr_talent = (hr_talent)cmbtalent.SelectionBoxItem,
                    id_talent = (int)cmbtalent.SelectedValue
                };

                item.item_service.Add(item_service);
                itemitem_serviceViewSource.View.Refresh();
                itemitem_serviceViewSource.View.MoveCurrentToLast();
            }
        }

        private async void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            await ItemDB.db.item_brand
              .Where(a => a.id_company == CurrentSession.Id_Company)
              .OrderBy(a => a.name).LoadAsync();

            item_brandViewSource.Source = ItemDB.db.item_brand.Local;
        }

        private void Label_MouseUp(object sender, MouseButtonEventArgs e)
        {
            popupName.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            popupName.StaysOpen = false;
            popupName.IsOpen = true;
        }

        private void popupName_Closed(object sender, EventArgs e)
        {
            if (item_templateViewSource.View.CurrentItem is item_template item_template)
            {
                item item = itemViewSource.View.CurrentItem as item;
                foreach (item_template_detail item_template_detail in item_template.item_template_detail)
                {
                    item.name = item.name + item_template_detail.value;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (ItemDB != null)
            {
                if (disposing)
                {
                    ItemDB.db.Dispose();
                }
            }
        }
    }
}