using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using entity;
using System.IO;
using System.Data.Entity;
using System.Windows.Input;

namespace Cognitivo.Product
{
    public partial class FixedAssets : Page
    {
        ItemDB ItemDB = new ItemDB();
        CollectionViewSource
            itemViewSource,
            itemitem_capitalViewSource, item_asset_maintainanceViewSource,
            itemitem_tagdetailViewSource;

        public FixedAssets()
        {
            InitializeComponent();

            itemViewSource = FindResource("itemViewSource") as CollectionViewSource;
            itemitem_capitalViewSource = FindResource("itemitem_capitalViewSource") as CollectionViewSource;
            itemitem_tagdetailViewSource = FindResource("itemitem_tagdetailViewSource") as CollectionViewSource;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ItemDB.items.Where(i => i.id_company == CurrentSession.Id_Company && i.id_item_type == item.item_type.FixedAssets).Load();
            itemViewSource.Source = ItemDB.items.Local;

            item_asset_maintainanceViewSource = ((CollectionViewSource)(FindResource("item_asset_maintainanceViewSource")));

            cbxBranch.ItemsSource = ItemDB.app_branch.Where(b => b.id_company == CurrentSession.Id_Company && b.is_active).OrderBy(b => b.name).ToList();

            cbxassetGroup.ItemsSource = ItemDB.item_asset_group.Where(b => b.id_company == CurrentSession.Id_Company).OrderBy(b => b.name).ToList();
            cbxType.ItemsSource = Enum.GetValues(typeof(item_asset_maintainance.MaintainanceTypes));

            CollectionViewSource app_departmentViewSource = ((CollectionViewSource)(FindResource("app_departmentViewSource")));
            app_departmentViewSource.Source = ItemDB.app_department.Where(x => x.id_company == CurrentSession.Id_Company).OrderBy(x => x.name).ToList();
            
            CollectionViewSource item_brandViewSource = ((CollectionViewSource)(FindResource("item_brandViewSource")));
            ItemDB.item_brand.Where(x => x.id_company == CurrentSession.Id_Company).OrderBy(x => x.name).ToList();
            item_brandViewSource.Source = ItemDB.item_brand.Local;
            CollectionViewSource contactViewSource = ((CollectionViewSource)(FindResource("contactViewSource")));
            contactViewSource.Source = ItemDB.contacts.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company && x.is_employee).OrderBy(x => x.name).ToList();
            cmbdeactive.ItemsSource = Enum.GetValues(typeof(item_asset.DeActiveTypes)).OfType<item_asset.DeActiveTypes>().ToList();

            CollectionViewSource app_vat_groupViewSource = FindResource("app_vat_groupViewSource") as CollectionViewSource;
             ItemDB.app_vat_group
                    .Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company)
                    .OrderBy(a => a.name).Load();
            //await Dispatcher.InvokeAsync(new Action(() =>
            //{
                app_vat_groupViewSource.Source = ItemDB.app_vat_group.Local;
            //}));

            CollectionViewSource item_price_listViewSource = FindResource("item_price_listViewSource") as CollectionViewSource;
             ItemDB.item_price_list
                   .Where(a => a.is_active && a.id_company == CurrentSession.Id_Company)
                   .OrderBy(a => a.name).Load();
            //await Dispatcher.InvokeAsync(new Action(() =>
            //{
                item_price_listViewSource.Source = ItemDB.item_price_list.Local;
            //}));

             ItemDB.app_currency
                    .Where(a => a.is_active && a.id_company == CurrentSession.Id_Company)
                    .OrderBy(a => a.name).ToList();
           // await Dispatcher.InvokeAsync(new Action(() =>
           // {
                CollectionViewSource app_currencyViewSource = ((CollectionViewSource)(FindResource("app_currencyViewSource")));
                app_currencyViewSource.Source = ItemDB.app_currency.Local;
           // }));

             ItemDB.item_tag
                .Where(x => x.id_company == CurrentSession.Id_Company && x.is_active)
                .OrderBy(x => x.name).Load();
           // await Dispatcher.InvokeAsync(new Action(() =>
          //  {
                CollectionViewSource item_tagViewSource = ((CollectionViewSource)(FindResource("item_tagViewSource")));
                item_tagViewSource.Source = ItemDB.item_tag.Local;
           // }));
        }

        #region Mini ToolBar
        private void toolBar_Mini_btnSave_Click(object sender)
        {
            ItemDB.SaveChanges();
        }

        private void toolBar_Mini_btnEdit_Click(object sender)
        {
            item_asset_maintainance item_asset_maintainance = item_asset_maintainanceViewSource.View.CurrentItem as item_asset_maintainance;
            item_asset_maintainance.IsSelected = true;
            item_asset_maintainance.State = EntityState.Modified;
        }

        private void toolBar_Mini_btnNew_Click(object sender)
        {
            item item = itemViewSource.View.CurrentItem as item;

            if (item != null)
            {
                if (item.item_asset != null)
                {
                    item_asset item_asset = item.item_asset.FirstOrDefault() as item_asset;

                    item_asset_maintainance item_asset_maintainance = new item_asset_maintainance();
                    item_asset_maintainance.IsSelected = true;
                    item_asset_maintainance.State = EntityState.Added;

                    item_asset.item_asset_maintainance.Add(item_asset_maintainance);

                    itemitem_capitalViewSource.View.Refresh();
                    item_asset_maintainanceViewSource.View.Refresh();
                    item_asset_maintainanceViewSource.View.MoveCurrentTo(item_asset_maintainance);
                }
            }
        }
        #endregion

        #region Toolbar

        private void toolBar_btnEdit_Click(object sender)
        {
            if (itemDataGrid.SelectedItem != null)
            {
                item item = itemDataGrid.SelectedItem as item;
                item.IsSelected = true;
                item.State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select an Item");
            }
        }

        private void toolBar_btnSave_Click(object sender)
        {
            if (ItemDB.SaveChanges() > 0)
            {
                toolBar.msgSaved(ItemDB.NumberOfRecords);
                itemViewSource.View.Refresh();
            }
        }

        private void toolBar_btnNew_Click(object sender)
        {
            item item = ItemDB.New();
            item.id_item_type = entity.item.item_type.FixedAssets;
        

            using (db db = new db())
            { item.id_vat_group = db.app_vat_group.Where(x => x.is_default && x.id_company == CurrentSession.Id_Company).FirstOrDefault().id_vat_group; }

            item_asset item_asset = new item_asset();

            item.item_asset.Add(item_asset);
            ItemDB.items.Add(item);

            itemViewSource.View.Refresh();
            itemViewSource.View.MoveCurrentTo(item);
            itemitem_capitalViewSource.View.Refresh();
            itemitem_capitalViewSource.View.MoveCurrentTo(item_asset);
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            // ItemDB.CancelAllChanges();
            item item = (item)itemDataGrid.SelectedItem;
            if (item.State == EntityState.Added)
            {
                ItemDB.Entry(item).State = EntityState.Detached;
            }
            else
            {
                item.is_active = false;
                item.State = EntityState.Unchanged;
                ItemDB.Entry(item).State = EntityState.Unchanged;
            }
            itemViewSource.View.Refresh();
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

        private void AddBrand_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void EditBrand_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void itemserviceComboBox_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (CmbService.ContactID > 0)
            {
                contact contact = ItemDB.contacts.Where(x => x.id_contact == CmbService.ContactID).FirstOrDefault();
                item_asset item_asset = itemitem_capitalViewSource.View.CurrentItem as item_asset;
                item_asset.id_contact = contact.id_contact;
                item_asset.contact = contact;
                
            }

        }

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

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            if (!string.IsNullOrEmpty(query) && itemViewSource != null)
            {
                try
                {
                    itemViewSource.View.Filter = i =>
                    {
                        item item = i as item;
                        if (item != null)
                        {
                            string name = "";
                            string code = "";

                            if (item.name != null)
                            {
                                name = item.name.ToLower();
                            }

                            if (item.code != null)
                            {
                                code = item.code.ToLower();
                            }

                            if (name.Contains(query.ToLower()) || code.Contains(query.ToLower()))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    };
                }
                catch { }
            }
            else
            {
                itemViewSource.View.Filter = null;
            }
        }
    }
}
