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
        cntrl.Curd.ItemRequest ItemRequest;
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
            if (db.db.SaveChanges() > 0)
            {
                toolBar.msgSaved(0);
            }
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
        private void btnRequestResource_Click(object sender, RoutedEventArgs e)
        {
            if (dgvMaintainceDetail.ItemsSource != null)
            {
                List<item_asset_maintainance_detail> item_asset_maintainance_detaillist = db.db.item_asset_maintainance_detail.ToList();
                item_asset_maintainance_detaillist = item_asset_maintainance_detaillist.Where(x => x.IsSelected == true).ToList();

                if (item_asset_maintainance_detaillist.Count() > 0)
                {
                    ItemRequest = new cntrl.Curd.ItemRequest();
                    crud_modal_request.Visibility = Visibility.Visible;
                    ItemRequest.listdepartment = db.db.app_department.ToList();
                    ItemRequest.item_request_Click += item_request_Click;
                    crud_modal_request.Children.Add(ItemRequest);
                }
                else
                {
                    toolBar.msgWarning("Select a Task");
                }
            }
        }


        public void item_request_Click(object sender)
        {
          
            if (dgvMaintainceDetail.ItemsSource != null)
            {
                List<item_asset_maintainance_detail> item_asset_maintainance_detaillist = db.db.item_asset_maintainance_detail.ToList();
                item_asset_maintainance_detaillist = item_asset_maintainance_detaillist.Where(x => x.IsSelected == true).ToList();

                item_request item_request = new item_request();
                item_request.name = ItemRequest.name;
                item_request.comment = ItemRequest.comment;

                item_request.id_department = ItemRequest.id_department;
           

                item_request.request_date = DateTime.Now;

                foreach (item_asset_maintainance_detail data in item_asset_maintainance_detaillist)
                {
                    item_request_detail item_request_detail = new entity.item_request_detail();
                    item_request_detail.date_needed_by = ItemRequest.neededDate;
                    item_request_detail.id_maintainance_detail = data.id_maintainance_detail;
                    item_request_detail.urgency = ItemRequest.Urgencies;
                    int idItem = data.item.id_item;
                    item_request_detail.id_item = idItem;
                    item item = db.db.items.Where(x => x.id_item == idItem).FirstOrDefault();
                    if (item != null)
                    {
                        item_request_detail.item = item;
                        item_request_detail.comment = item_request_detail.item.name;
                    }

                 


                    item_request_detail.quantity = data.quantity;

                    item_request.item_request_detail.Add(item_request_detail);
                }

                db.db.item_request.Add(item_request);
                db.db.SaveChanges();

                //item_requestViewSource.View.Filter = i =>
                //{
                //    item_request _item_request = (item_request)i;
                //    if (_item_request.id_production_order == id_production_order)
                //        return true;
                //    else
                //        return false;
                //};
            }

            crud_modal_request.Children.Clear();
            crud_modal_request.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void toolBar_btnApprove_Click(object sender)
        {
            foreach (item_asset_maintainance item_asset_maintainance in item_asset_maintainanceViewSource.View.OfType<item_asset_maintainance>().ToList())
            {
                item_asset_maintainance.status = entity.item_asset_maintainance.Status.Done;
            }
        }
    }
}
