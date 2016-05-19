using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Data.Entity;
using entity;
using System.Data;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Data.Entity.Validation;
using System.Windows.Media;
using System.Windows.Documents;

namespace Cognitivo.Product
{
    public partial class Maintainance : Page
    {
        dbContext db = new dbContext();
        CollectionViewSource item_asset_maintainanceViewSource, app_currencyfxViewSource, item_asset_maintainanceitem_asset_maintainance_detailViewSource;
        public Maintainance()
        {
            InitializeComponent();
        }

        private void toolBar_btnNew_Click(object sender)
        {
            item_asset_maintainance item_asset_maintainance = new item_asset_maintainance();
            item_asset_maintainance.IsSelected = true;
            item_asset_maintainance.State = EntityState.Added;
            db.db.Entry(item_asset_maintainance).State = EntityState.Added;

            item_asset_maintainanceViewSource.View.MoveCurrentToLast();
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            try
            {
                if (MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    item_asset_maintainance item_asset_maintainance = item_asset_maintainanceViewSource.View.CurrentItem as item_asset_maintainance;
                    item_asset_maintainance.is_head = false;
                    item_asset_maintainance.State = EntityState.Deleted;
                    item_asset_maintainance.IsSelected = true;
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void toolBar_btnSave_Click(object sender)
        {
            db.db.SaveChanges();
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            db.CancelChanges();
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            item_asset_maintainance item_asset_maintainance = item_asset_maintainanceViewSource.View.CurrentItem as item_asset_maintainance;
            item_asset_maintainance.IsSelected = true;
            item_asset_maintainance.State = EntityState.Modified;
            db.db.Entry(item_asset_maintainance).State = EntityState.Modified;

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            item_asset_maintainanceitem_asset_maintainance_detailViewSource = ((CollectionViewSource)(FindResource("item_asset_maintainanceitem_asset_maintainance_detailViewSource")));
            item_asset_maintainanceViewSource = ((CollectionViewSource)(FindResource("item_asset_maintainanceViewSource")));
            db.db.item_asset_maintainance.Load();
            item_asset_maintainanceViewSource.Source = db.db.item_asset_maintainance.Local;

            app_currencyfxViewSource = ((CollectionViewSource)(FindResource("app_currencyfxViewSource")));
            db.db.app_currencyfx.Where(x => x.is_active).Load();
            app_currencyfxViewSource.Source = db.db.app_currencyfx.Local;
        }

        private void item_Select(object sender, RoutedEventArgs e)
        {
            if (sbxFixedasset.ItemID > 0)
            {
                item item = db.db.items.Where(x => x.id_item == sbxFixedasset.ItemID).FirstOrDefault();
                item_asset_maintainance item_asset_maintainance = item_asset_maintainanceViewSource.View.CurrentItem as item_asset_maintainance;
                if (item.item_asset.FirstOrDefault() != null)
                {
                    item_asset_maintainance.id_item_asset = item.item_asset.FirstOrDefault().id_item_asset;
                }

            }


        }

        private void sbxitem_Select(object sender, RoutedEventArgs e)
        {
            if (sbxitem.ItemID > 0)
            {
                item item = db.db.items.Where(x => x.id_item == sbxitem.ItemID).FirstOrDefault();
                item_asset_maintainance item_asset_maintainance = item_asset_maintainanceViewSource.View.CurrentItem as item_asset_maintainance;
                if (item_asset_maintainance.item_asset_maintainance_detail.Where(a => a.id_item == item.id_item).FirstOrDefault() == null)
                {
                    item_asset_maintainance_detail item_asset_maintainance_detail = new item_asset_maintainance_detail();
                    item_asset_maintainance_detail.item = item;
                    item_asset_maintainance_detail.id_item = item.id_item;
                    item_asset_maintainance_detail.quantity = 1;
                    item_asset_maintainance_detail.unit_cost =(decimal)item.unit_cost;
                    item_asset_maintainance.item_asset_maintainance_detail.Add(item_asset_maintainance_detail);
                }
            }
            item_asset_maintainanceViewSource.View.Refresh();
            item_asset_maintainanceitem_asset_maintainance_detailViewSource.View.Refresh();
        }
    }
}
