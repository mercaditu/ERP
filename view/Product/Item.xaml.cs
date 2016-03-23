
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Data.Entity;
using System.Data;
using entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Threading.Tasks;

namespace Cognitivo.Product
{
    public partial class Item : Page, Menu.ApplicationWindow.ICanClose
    {
        ItemDB dbContext = new ItemDB();

        CollectionViewSource itemViewSource,
            itemitem_priceViewSource,
            //itemitem_propertyViewSource,
            itemitem_dimentionViewSource,
            itemitem_capitalViewSource,
            item_brandViewSource,
            app_vat_groupViewSource,
            item_price_listViewSource,
            itemitem_productViewSource,
            app_dimentionViewSource,
            itemitem_tagdetailViewSource,
            hr_talentViewSource,
            itemitem_serviceViewSource;
            //app_propertyViewSource;

        public Item()
        {
            InitializeComponent();

            itemViewSource = FindResource("itemViewSource") as CollectionViewSource;
            itemitem_priceViewSource = FindResource("itemitem_priceViewSource") as CollectionViewSource;
            //itemitem_propertyViewSource = FindResource("itemitem_propertyViewSource") as CollectionViewSource;
            itemitem_dimentionViewSource = FindResource("itemitem_dimentionViewSource") as CollectionViewSource;
            itemitem_productViewSource = FindResource("itemitem_productViewSource") as CollectionViewSource;
            itemitem_capitalViewSource = FindResource("itemitem_capitalViewSource") as CollectionViewSource;
            itemitem_tagdetailViewSource = FindResource("itemitem_tagdetailViewSource") as CollectionViewSource;
            item_brandViewSource = ((CollectionViewSource)(FindResource("item_brandViewSource")));
            item_price_listViewSource = ((CollectionViewSource)(FindResource("item_price_listViewSource")));
            app_vat_groupViewSource = ((CollectionViewSource)(FindResource("app_vat_groupViewSource")));
            //app_propertyViewSource = ((CollectionViewSource)(FindResource("app_propertyViewSource")));
            app_dimentionViewSource = (CollectionViewSource)FindResource("app_dimentionViewSource");
            hr_talentViewSource = (CollectionViewSource)FindResource("hr_talentViewSource");
            itemitem_serviceViewSource = (CollectionViewSource)FindResource("itemitem_serviceViewSource");
        }

        private void load_PrimaryData()
        {
            Task task_PrimaryData = Task.Factory.StartNew(() => load_PrimaryDataThread());
            task_PrimaryData.Wait();
            Task thread_SecondaryData = Task.Factory.StartNew(() => load_SecondaryDataThread());
        }

        private async void load_PrimaryDataThread()
        {
            ProductSettings _pref_Product = new ProductSettings();

            if (_pref_Product.Product)
            {
                dbContext.items.Where(i => i.is_active && i.id_company == CurrentSession.Id_Company && i.id_item_type == item.item_type.Product).Load();
            }

            if (_pref_Product.RawMaterial)
            {
                dbContext.items.Where(i => i.is_active && i.id_company == CurrentSession.Id_Company && i.id_item_type == item.item_type.RawMaterial).Load();
            }

            if (_pref_Product.FixedAsset)
            {
                dbContext.items.Where(i => i.is_active && i.id_company == CurrentSession.Id_Company && i.id_item_type == item.item_type.FixedAssets).Load();
            }

            if (_pref_Product.Service)
            {
                dbContext.items.Where(i => i.is_active && i.id_company == CurrentSession.Id_Company && i.id_item_type == item.item_type.Service).Load();
            }

            if (_pref_Product.Task)
            {
                dbContext.items.Where(i => i.is_active && i.id_company == CurrentSession.Id_Company && i.id_item_type == item.item_type.Task).Load();
            }

            if (_pref_Product.Supplies)
            {
                dbContext.items.Where(i => i.is_active && i.id_company == CurrentSession.Id_Company && i.id_item_type == item.item_type.Supplies).Load();
            }

            await Dispatcher.InvokeAsync(new Action(() =>
            {
                itemViewSource.Source = dbContext.items.Local.OrderBy(x => x.name);
            }));
        }

        private async void load_SecondaryDataThread()
        {
            await dbContext.app_measurement
                    .Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company)
                    .OrderBy(a => a.name).LoadAsync();

            await Dispatcher.InvokeAsync(new Action(() =>
            {
                CollectionViewSource app_measurementViewSource = ((CollectionViewSource)(FindResource("app_measurementViewSource")));
                CollectionViewSource app_measurementViewSourceconvert = ((CollectionViewSource)(FindResource("app_measurementViewSourceconvert")));
                CollectionViewSource app_measurementViewSourcenew = ((CollectionViewSource)(FindResource("app_measurementViewSourcenew")));
                app_measurementViewSource.Source = dbContext.app_measurement.Local;
                app_measurementViewSourceconvert.Source = dbContext.app_measurement.Local;
                app_measurementViewSourcenew.Source = dbContext.app_measurement.Local;
            }));

            await dbContext.app_dimension
                .Where(a => a.id_company == CurrentSession.Id_Company)
                .OrderBy(a => a.name).LoadAsync();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                app_dimentionViewSource.Source = dbContext.app_dimension.Local;
            }));

            await dbContext.app_vat_group
                .Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company)
                .OrderBy(a => a.name).LoadAsync();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                app_vat_groupViewSource.Source = dbContext.app_vat_group.Local;
            }));

            await dbContext.item_tag
                .Where(x => x.id_company == CurrentSession.Id_Company && x.is_active == true)
                .OrderBy(x => x.name).LoadAsync();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                CollectionViewSource item_tagViewSource = ((CollectionViewSource)(FindResource("item_tagViewSource")));
                item_tagViewSource.Source = dbContext.item_tag.Local;
            }));

            await dbContext.app_currency
                .Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company)
                .OrderBy(a => a.name).ToListAsync();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                CollectionViewSource app_currencyViewSource = ((CollectionViewSource)(FindResource("app_currencyViewSource")));
                app_currencyViewSource.Source = dbContext.app_currency.Local;
            }));

            await dbContext.hr_talent
                .Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company)
                .OrderBy(a => a.name).LoadAsync();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                hr_talentViewSource.Source = dbContext.hr_talent.Local;
            }));

            await dbContext.item_price_list
               .Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company)
               .OrderBy(a => a.name).LoadAsync();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                item_price_listViewSource.Source = dbContext.item_price_list.Local;
            }));

            await dbContext.item_brand
                .Where(a => a.id_company == CurrentSession.Id_Company)
                .OrderBy(a => a.name).LoadAsync();
            await Dispatcher.InvokeAsync(new Action(() =>
            {
                item_brandViewSource.Source = dbContext.item_brand.Local;
            }));

            await Dispatcher.InvokeAsync(new Action(() =>
            {
                // toolBar.Visibility = Visibility.Visible;
                toolBar.IsEnabled = true;
            }));
        }

        private void Item_Loaded(object sender, RoutedEventArgs e)
        {
            //Loads Primary and Secondary Data
            load_PrimaryData();

            cmbitem.ItemsSource = Enum.GetValues(typeof(item.item_type));
        }

        #region Implementing Interface For CanClose
        public bool CanClose()
        {
            if (dbContext.ChangeTracker.HasChanges())
            {
                MessageBoxResult savechnages = MessageBox.Show("Do you want to save changes?", "Cognitivo", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (savechnages == MessageBoxResult.Yes)
                {
                    IEnumerable<DbEntityValidationResult> validationresult = dbContext.GetValidationErrors();
                    if (validationresult.Count() == 0)
                    {
                        dbContext.item_tag.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).Load();
                        dbContext.SaveChanges();
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Some values are missing. Please fillup all the fields and try again.", "Cognitivo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        return false;
                    }
                }
                else if (savechnages == MessageBoxResult.No)
                {
                    return true;
                }
                else if (savechnages == MessageBoxResult.Cancel)
                {
                    return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region Toolbar Events
        private void toolBar_btnCancel_Click(object sender)
        {
            item item = (item)itemDataGrid.SelectedItem;
            // item_vatDataGrid.CancelEdit();
            item_priceDataGrid.CancelEdit();
            item_dimentionDataGrid.CancelEdit();
            itemViewSource.View.MoveCurrentToFirst();

            if (item.State == EntityState.Added)
            {
                dbContext.Entry(item).State = EntityState.Detached;
            }
            itemViewSource.View.Refresh();
            //SetIsEnable = false;
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                item item = (item)itemDataGrid.SelectedItem;
                item.is_active = false;
                //mycntrl._item =item;
                itemViewSource.View.Filter = i =>
                {
                    entity.item objitem = (item)i;
                    if (objitem.is_active == true)
                        return true;
                    else
                        return false;
                };
            }
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (itemDataGrid.SelectedItem != null)
            {
                item item = (item)itemDataGrid.SelectedItem;
                item.IsSelected = true;
                item.State = System.Data.Entity.EntityState.Modified;
                dbContext.Entry(item).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select an Item");
            }
        }

        private void toolBar_btnNew_Click(object sender)
        {
            item _item = new item();
            item_price _item_price = new item_price();

            _item.IsSelected = true;
            _item.item_price.Add(_item_price);

            _item.State = System.Data.Entity.EntityState.Added;
            dbContext.items.Add(_item);

            itemViewSource.View.Refresh();
            itemViewSource.View.MoveCurrentToLast();
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
                            dbContext.item_asset.Remove(record);
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
                            dbContext.item_asset.Remove(record);
                        }
                    }
                    if (item.item_product.Count > 0)
                    {
                        List<item_product> records = item.item_product.ToList();
                        foreach (var record in records)
                        {
                            dbContext.item_product.Remove(record);
                        }
                    }
                }
                //Capital Resource
                else if (item.id_item_type == item.item_type.FixedAssets)
                {
                    if (item.item_asset.Count == 0)
                    {
                        item_asset _capital = new item_asset();
                        item.item_asset.Add(_capital);
                        if (itemitem_capitalViewSource.View != null)
                        {
                            itemitem_capitalViewSource.View.Refresh();
                            itemitem_capitalViewSource.View.MoveCurrentTo(_capital);
                        }

                    }
                    if (item.item_product.Count > 0)
                    {
                        List<item_product> records = item.item_product.ToList();
                        foreach (var record in records)
                        {
                            dbContext.item_product.Remove(record);
                        }
                    }
                }
            }
            //Product
        }

        private void toolBar_btnSave_Click(object sender)
        {
            IEnumerable<DbEntityValidationResult> validationresult = dbContext.GetValidationErrors();
            if (validationresult.Count() == 0)
            {
                //Check if exact same name exist with the same name. Check if the product is not the same so as not to affect already inserted items.
                item item = itemViewSource.View.CurrentItem as item;
                if (dbContext.items.Any(x => x.name.Contains(item.name) && x.id_item != item.id_item))
                {
                    toolBar.msgWarning("Product Already Exist..");
                    return;
                }

                //Save Changes
                dbContext.SaveChanges();

                itemViewSource.View.Refresh();
                toolBar.msgSaved();

            }
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                itemViewSource.View.Filter = i =>
                {
                    item item = i as item;
                    if (item.Error == null)
                    {
                        if (item.name.ToLower().Contains(query.ToLower()) || item.code.ToLower().Contains(query.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
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
        #endregion

        private void StackPanel_Drop(object sender, DragEventArgs e)
        {
            var data = e.Data as DataObject;
            if (data.ContainsFileDropList())
            {
                var files = data.GetFileDropList();
                string extension = System.IO.Path.GetExtension(files[0]);
                if (!string.IsNullOrEmpty(extension) &&
                    (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif" || extension == ".bmp"))
                    imageViewer.Source = LoadImageFromFile(files[0]);
                else
                    MessageBox.Show("Images with .jpg, .jpeg, .png, .gif, .bmp extensions are only allowed.");
            }
        }

        private BitmapImage LoadImageFromFile(string filename)
        {
            using (var fs = File.OpenRead(filename))
            {
                var img = new BitmapImage();
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                //Downscaling to keep the memory footprint low
                img.DecodePixelWidth = (int)SystemParameters.PrimaryScreenWidth;
                img.StreamSource = fs;
                img.EndInit();
                return img;
            }
        }

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

            if (e.Parameter as item_price != null)
            {
                //item_price item_price = e.Parameter as item_price;
                //if (string.IsNullOrEmpty(item_price.Error))
                //{
                e.CanExecute = true;
                //}
            }
            if (e.Parameter as item_dimension != null)
            {
                //item_dimension item_dimension = e.Parameter as item_dimension;
                //if (string.IsNullOrEmpty(item_dimension.Error))
                //{
                e.CanExecute = true;
                //}
            }
            if (e.Parameter as item_tag_detail != null)
            {
                e.CanExecute = true;
            }
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    //DeleteDetailGridRow
                    if (e.Parameter as item_price != null)
                    {
                        item_priceDataGrid.CancelEdit();
                        dbContext.item_price.Remove(e.Parameter as item_price);
                        itemitem_priceViewSource.View.Refresh();
                    }
                    if (e.Parameter as item_dimension != null)
                    {
                        item_dimentionDataGrid.CancelEdit();
                        dbContext.item_dimension.Remove(e.Parameter as item_dimension);
                        itemitem_dimentionViewSource.View.Refresh();
                    }

                    if (e.Parameter as item_tag_detail != null)
                    {
                        item_tag_detailDataGrid.CancelEdit();
                        dbContext.item_tag_detail.Remove(e.Parameter as item_tag_detail);
                        itemitem_tagdetailViewSource.View.Refresh();
                    }
                }
            }
            catch
            {

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

            Task task_PrimaryData = Task.Factory.StartNew(() => load_PrimaryDataThread());

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
            cntrl.Curd.item_brand item_brand = new cntrl.Curd.item_brand();
            item_brand.item_brandViewSource = item_brandViewSource;
            item_brand.MainViewSource = itemViewSource;
            item_brand.curObject = itemViewSource.View.CurrentItem;
            //item_brand._entity = dbContext;
            item_brand.operationMode = cntrl.Class.clsCommon.Mode.Add;
            item_brand.isExternalCall = true;
            crud_modal.Children.Add(item_brand);
        }

        private void EditBrand_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            item_brand _item_brand = cbxItemBrand.SelectedItem as item_brand;
            if (_item_brand != null)
            {
                crud_modal.Visibility = Visibility.Visible;
                cntrl.Curd.item_brand item_brand = new cntrl.Curd.item_brand();
                item_brand.item_brandViewSource = item_brandViewSource;
                item_brand.MainViewSource = itemViewSource;
                item_brand.curObject = itemViewSource.View.CurrentItem;
                //item_brand._entity = dbContext;
                item_brand.item_brandobject = _item_brand;
                item_brand.operationMode = cntrl.Class.clsCommon.Mode.Edit;
                item_brand.isExternalCall = true;
                crud_modal.Children.Add(item_brand);
            }
        }

        private void AddVAT_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.vat_group _vat_group = new cntrl.Curd.vat_group();
            _vat_group.app_vat_groupViewSource = app_vat_groupViewSource;
            _vat_group.MainViewSource = itemViewSource;
            _vat_group.curObject = itemViewSource.View.CurrentItem;
            //_vat_group._entity = dbContext;
            _vat_group.operationMode = cntrl.Class.clsCommon.Mode.Add;
            _vat_group.isExternalCall = true;
            crud_modal.Children.Add(_vat_group);
        }

        private void EditVAT_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            app_vat_group app_vat_group = cmbvat.SelectedItem as app_vat_group;
            //app_vat_group app_vat_group = item_vat.app_vat_group;
            if (app_vat_group != null)
            {
                crud_modal.Visibility = Visibility.Visible;
                cntrl.Curd.vat_group _vat_group = new cntrl.Curd.vat_group();
                _vat_group.app_vat_groupViewSource = app_vat_groupViewSource;
                _vat_group.MainViewSource = itemViewSource;
                _vat_group.curObject = itemViewSource.View.CurrentItem;
                //_vat_group._entity = dbContext;
                _vat_group.vat_groupObject = app_vat_group;
                _vat_group.operationMode = cntrl.Class.clsCommon.Mode.Edit;
                _vat_group.isExternalCall = true;
                crud_modal.Children.Add(_vat_group);
            }
        }

        private void AddPricelist_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.price_list _price_list = new cntrl.Curd.price_list();
            _price_list.item_price_listViewSource = item_price_listViewSource;
            _price_list.MainViewSource = itemViewSource;
            _price_list.curMainObject = itemViewSource.View.CurrentItem;
            //_price_list._entity = dbContext;
            _price_list.operationMode = cntrl.Class.clsCommon.Mode.Add;
            _price_list.isExternalCall = true;
            crud_modal.Children.Add(_price_list);
        }

        private void EditPricelist_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            item_price item_price = item_priceDataGrid.SelectedItem as item_price;
            item_price_list item_price_list = item_price.item_price_list;
            if (item_price_list != null)
            {
                crud_modal.Visibility = Visibility.Visible;
                cntrl.Curd.price_list _price_list = new cntrl.Curd.price_list();
                _price_list.item_price_listViewSource = item_price_listViewSource;
                _price_list.MainViewSource = itemViewSource;
                _price_list.curMainObject = itemViewSource.View.CurrentItem;
                // _price_list._entity = dbContext;
                _price_list.price_listobject = item_price_list;
                _price_list.operationMode = cntrl.Class.clsCommon.Mode.Edit;
                _price_list.isExternalCall = true;
                crud_modal.Children.Add(_price_list);
            }
        }

        private void AddDimention_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            crud_modal.Visibility = Visibility.Visible;
            cntrl.Curd.dimension _dimension = new cntrl.Curd.dimension();
            _dimension.app_dimensionViewSource = app_dimentionViewSource;
            _dimension.MainViewSource = itemViewSource;
            _dimension.curObject = itemViewSource.View.CurrentItem;
            //_dimension._entity = dbContext;
            _dimension.operationMode = cntrl.Class.clsCommon.Mode.Add;
            _dimension.isExternalCall = true;
            crud_modal.Children.Add(_dimension);
        }

        private void EditDimention_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            item_dimension item_dimension = item_dimentionDataGrid.SelectedItem as item_dimension;
            app_dimension app_dimension = item_dimension.app_dimension;
            if (app_dimension != null)
            {
                crud_modal.Visibility = Visibility.Visible;
                cntrl.Curd.dimension _dimension = new cntrl.Curd.dimension();
                _dimension.app_dimensionViewSource = app_dimentionViewSource;
                _dimension.MainViewSource = itemViewSource;
                _dimension.curObject = itemViewSource.View.CurrentItem;
                //_dimension._entity = dbContext;
                _dimension.objapp_dimension = app_dimension;
                _dimension.operationMode = cntrl.Class.clsCommon.Mode.Edit;
                _dimension.isExternalCall = true;
                crud_modal.Children.Add(_dimension);
            }
        }

        #endregion

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

        void Add_Tag()
        {
            if (cbxTag.Data != null)
            {
                int id = Convert.ToInt32(((item_tag)cbxTag.Data).id_tag);
                if (id > 0)
                {
                    item item = itemViewSource.View.CurrentItem as item;
                    if (item != null)
                    {
                        item_tag_detail item_tag_detail = new item_tag_detail();
                        item_tag_detail.id_tag = ((item_tag)cbxTag.Data).id_tag;
                        item_tag_detail.item_tag = ((item_tag)cbxTag.Data);
                        item.item_tag_detail.Add(item_tag_detail);
                        itemitem_tagdetailViewSource.View.Refresh();

                    }
                }
            }
        }

        private void btnadd_Click(object sender, RoutedEventArgs e)
        {
            item item = (item)itemViewSource.View.CurrentItem;
            item_service item_service = new item_service();
            item_service.hr_talent = (hr_talent)cmbtalent.SelectionBoxItem;
            item_service.id_talent = (int)cmbtalent.SelectedValue;
            item.item_service.Add(item_service);
            itemitem_serviceViewSource.View.Refresh();
            itemitem_serviceViewSource.View.MoveCurrentToLast();
        }

        private void crud_modal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            dbContext.item_brand
              .Where(a => a.id_company == CurrentSession.Id_Company)
              .OrderBy(a => a.name).Load();

            item_brandViewSource.Source = dbContext.item_brand.Local;

        }
    }
}
