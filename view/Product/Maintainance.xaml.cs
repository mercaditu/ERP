using entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Cognitivo.Product
{
    public partial class Maintainance : Page
    {
        private db dbContext = new db();
        private CollectionViewSource item_asset_maintainanceViewSource, item_asset_maintainanceitem_asset_maintainance_detailViewSource;
        private cntrl.Curd.ItemRequest ItemRequest;

        public Maintainance()
        {
            InitializeComponent();
        }

        private void toolBar_btnSave_Click(object sender)
        {
            if (dbContext.SaveChanges() > 0)
            {
                toolBar.msgSaved(0);
            }
        }

        private void toolBar_btnCancel_Click(object sender)
        {
            item_asset_maintainance item_asset_maintainance = item_asset_maintainanceViewSource.View.CurrentItem as item_asset_maintainance;
            if (item_asset_maintainance != null)
            {
                item_asset_maintainance.State = EntityState.Unchanged;

                if (item_asset_maintainance.State == EntityState.Added)
                {
                    dbContext.Entry(item_asset_maintainance).State = EntityState.Detached;
                }
                else
                {
                    item_asset_maintainance.State = EntityState.Unchanged;
                    dbContext.Entry(item_asset_maintainance).State = EntityState.Unchanged;
                }
            }
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            item_asset_maintainance item_asset_maintainance = item_asset_maintainanceViewSource.View.CurrentItem as item_asset_maintainance;
            if (item_asset_maintainance != null)
            {
                item_asset_maintainance.IsSelected = true;
                item_asset_maintainance.State = EntityState.Modified;
                dbContext.Entry(item_asset_maintainance).State = EntityState.Modified;
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            item_asset_maintainanceitem_asset_maintainance_detailViewSource = ((CollectionViewSource)(FindResource("item_asset_maintainanceitem_asset_maintainance_detailViewSource")));
            item_asset_maintainanceViewSource = ((CollectionViewSource)(FindResource("item_asset_maintainanceViewSource")));
            await dbContext.item_asset_maintainance.Where(x => x.id_company == CurrentSession.Id_Company).LoadAsync();
            item_asset_maintainanceViewSource.Source = dbContext.item_asset_maintainance.Local;

            //app_currencyfxViewSource = ((CollectionViewSource)(FindResource("app_currencyfxViewSource")));
            //await dbContext.app_currencyfx.Where(x => x.is_active && x.id_company == CurrentSession.Id_Company).LoadAsync();
            //app_currencyfxViewSource.Source = dbContext.app_currencyfx.Local;
            //sbxFixedasset.item_types = item.item_type.FixedAssets;

        }

        private void sbxitem_Select(object sender, RoutedEventArgs e)
        {
            if (sbxitem.ItemID > 0)
            {
                item item = dbContext.items.Find(sbxitem.ItemID);
                item_asset_maintainance item_asset_maintainance = item_asset_maintainanceViewSource.View.CurrentItem as item_asset_maintainance;

                if (item != null && item_asset_maintainance != null)
                {
                    if (item_asset_maintainance.item_asset_maintainance_detail.Where(a => a.id_item == item.id_item).FirstOrDefault() == null)
                    {
                        item_asset_maintainance_detail item_asset_maintainance_detail = new item_asset_maintainance_detail();
                        item_asset_maintainance_detail.item = item;
                        item_asset_maintainance_detail.id_item = item.id_item;

                        if (dtpstartdate.Text == "")
                        {
                            dtpstartdate.Text = DateTime.Now.ToString();
                        }

                        if (dtpenddate.Text == "")
                        {
                            dtpenddate.Text = DateTime.Now.ToString();
                        }

                        string start_date = string.Format("{0} {1}", dtpstartdate.Text, dtpstarttime.Text);
                        item_asset_maintainance_detail.start_date = Convert.ToDateTime(start_date);

                        string end_date = string.Format("{0} {1}", dtpenddate.Text, dtpendtime.Text);
                        item_asset_maintainance_detail.end_date = Convert.ToDateTime(end_date);

                        item_asset_maintainance_detail.quantity = 1;

                        if (item.unit_cost != null)
                        {
                            item_asset_maintainance_detail.unit_cost = (decimal)item.unit_cost;
                        }
                        if (CmbService.ContactID > 0)
                        {
                            contact contact = dbContext.contacts.Where(x => x.id_contact == CmbService.ContactID).FirstOrDefault();
                            item_asset_maintainance_detail.id_contact = contact.id_contact;
                            item_asset_maintainance_detail.contact = contact;
                        }

                        item_asset_maintainance.item_asset_maintainance_detail.Add(item_asset_maintainance_detail);
                        item_asset_maintainanceViewSource.View.Refresh();
                        item_asset_maintainanceitem_asset_maintainance_detailViewSource.View.Refresh();
                    }
                }
            }
        }

        private void btnRequestResource_Click(object sender, RoutedEventArgs e)
        {
            if (dgvMaintainceDetail.ItemsSource != null)
            {
                List<item_asset_maintainance_detail> item_asset_maintainance_detaillist = item_asset_maintainanceitem_asset_maintainance_detailViewSource.View.OfType<item_asset_maintainance_detail>().ToList();
                item_asset_maintainance_detaillist = item_asset_maintainance_detaillist.Where(x => x.IsSelected == true).ToList();

                if (item_asset_maintainance_detaillist.Count() > 0)
                {
                    ItemRequest = new cntrl.Curd.ItemRequest();
                    crud_modal_request.Visibility = Visibility.Visible;
                    ItemRequest.listdepartment = dbContext.app_department.ToList();
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
                List<item_asset_maintainance_detail> item_asset_maintainance_detaillist = dbContext.item_asset_maintainance_detail.ToList();
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
                    item item = dbContext.items.Where(x => x.id_item == idItem).FirstOrDefault();
                    if (item != null)
                    {
                        item_request_detail.item = item;
                        item_request_detail.comment = item_request_detail.item.name;
                    }

                    item_request_detail.quantity = data.quantity;

                    item_request.item_request_detail.Add(item_request_detail);
                }

                dbContext.item_request.Add(item_request);
                dbContext.SaveChanges();
            }

            crud_modal_request.Children.Clear();
            crud_modal_request.Visibility = Visibility.Collapsed;
        }

        private void toolBar_btnApprove_Click(object sender)
        {
            foreach (item_asset_maintainance item_asset_maintainance in item_asset_maintainanceViewSource.View.OfType<item_asset_maintainance>().ToList())
            {
                item_asset_maintainance.status = item_asset_maintainance.Status.Done;
            }

            dbContext.SaveChanges();
            item_asset_maintainanceViewSource.View.Refresh();
        }

        private void DeleteCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter as item_asset_maintainance_detail != null)
            {
                e.CanExecute = true;
            }
        }

        private void DeleteCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are you sure want to Delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    item_asset_maintainance item_asset_maintainance = item_asset_maintainanceViewSource.View.CurrentItem as item_asset_maintainance;
                    //DeleteDetailGridRow
                    dgvMaintainceDetail.CancelEdit();
                    dbContext.item_asset_maintainance_detail.Remove(e.Parameter as item_asset_maintainance_detail);
                    item_asset_maintainanceitem_asset_maintainance_detailViewSource.View.Refresh();
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }
    }
}