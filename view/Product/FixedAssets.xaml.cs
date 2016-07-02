using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using entity;
using System.IO;
using System.Data.Entity;

namespace Cognitivo.Product
{
    public partial class FixedAssets : Page
    {
        ItemDB ItemDB = new ItemDB();
        CollectionViewSource
            itemViewSource,
            itemitem_capitalViewSource, item_asset_maintainanceViewSource;

        public FixedAssets()
        {
            InitializeComponent();

            itemViewSource = FindResource("itemViewSource") as CollectionViewSource;
            itemitem_capitalViewSource = FindResource("itemitem_capitalViewSource") as CollectionViewSource;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ItemDB.items.Where(i => i.is_active && i.id_company == CurrentSession.Id_Company && i.id_item_type == item.item_type.FixedAssets).ToList();
            itemViewSource.Source = ItemDB.items.Local;

            item_asset_maintainanceViewSource = ((CollectionViewSource)(FindResource("item_asset_maintainanceViewSource")));

            cbxBranch.ItemsSource = ItemDB.app_branch.Where(b => b.id_company == CurrentSession.Id_Company && b.is_active).OrderBy(b => b.name).ToList();

            cbxassetGroup.ItemsSource = ItemDB.item_asset_group.Where(b => b.id_company == CurrentSession.Id_Company).OrderBy(b => b.name).ToList();
            cbxType.ItemsSource = Enum.GetValues(typeof(item_asset_maintainance.MaintainanceTypes));

            CollectionViewSource app_departmentViewSource = ((CollectionViewSource)(FindResource("app_departmentViewSource")));
            app_departmentViewSource.Source = ItemDB.app_department.Where(x => x.id_company == CurrentSession.Id_Company).ToList();
            CollectionViewSource item_brandViewSource = ((CollectionViewSource)(FindResource("item_brandViewSource")));
            item_brandViewSource.Source = ItemDB.item_brand.Where(x => x.id_company == CurrentSession.Id_Company).ToList();
            CollectionViewSource contactViewSource = ((CollectionViewSource)(FindResource("contactViewSource")));
            contactViewSource.Source = ItemDB.contacts.Where(x => x.id_company == CurrentSession.Id_Company && x.is_employee).ToList();
            cmbdeactive.ItemsSource = Enum.GetValues(typeof(item_asset.DeActiveTypes)).OfType<item_asset.DeActiveTypes>().ToList();
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
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            ItemDB.CancelAllChanges();
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
    }
}
