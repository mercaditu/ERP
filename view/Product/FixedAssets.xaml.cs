using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using entity;

namespace Cognitivo.Product
{
    public partial class FixedAssets : Page
    {
        entity.ItemDB ItemDB = new entity.ItemDB();
        CollectionViewSource 
            itemViewSource,
            itemitem_capitalViewSource;

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

            cbxType.ItemsSource = Enum.GetValues(typeof(item.item_type));
        }

        #region Mini ToolBar
        private void toolBar_Mini_btnSave_Click(object sender)
        {

        }

        private void toolBar_Mini_btnEdit_Click(object sender)
        {

        }

        private void toolBar_Mini_btnNew_Click(object sender)
        {

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
            if (ItemDB.SaveChanges() == 1)
            {
                toolBar.msgSaved();
                itemViewSource.View.Refresh();
            }
        }

        private void toolBar_btnNew_Click(object sender)
        {
            item item = ItemDB.New();
            item.id_item_type = entity.item.item_type.FixedAssets;
            item_asset _capital = new item_asset();
            item.item_asset.Add(_capital);

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

        }

        private void toolBar_btnApprove_Click(object sender)
        {

        }
       
        #endregion

        private void StackPanel_Drop(object sender, DragEventArgs e)
        {

        }
    }
}
