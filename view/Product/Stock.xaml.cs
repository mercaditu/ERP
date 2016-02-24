using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using System.Data;
using entity;
using System.ComponentModel;

namespace Cognitivo.Product
{
    public partial class Stock : Page, INotifyPropertyChanged
    {
        StockDB StockDB = new StockDB();
        entity.Properties.Settings _entity_Pref = new entity.Properties.Settings();
        CollectionViewSource item_movementViewSource;
        List<int> tag = new List<int>();
        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime InventoryDate
        {
            get { return _InventoryDate; }
            set
            {
                _InventoryDate = value;
                RaisePropertyChanged("InventoryDate");

                slider.Maximum = DateTime.DaysInMonth(_InventoryDate.Year, _InventoryDate.Month);
                slider.Value = InventoryDate.Day;

                calc_Inventory(0);
            }
        }
        DateTime _InventoryDate = DateTime.Now;

        public Stock()
        {
            InitializeComponent();
        }

        private async void StockPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            StockDB.items.Where(a => a.id_company == _entity_Pref.company_ID && a.is_active == true).ToList();

            CollectionViewSource itemViewSource = FindResource("itemViewSource") as CollectionViewSource;
            itemViewSource.Source = StockDB.items.Local;

            await StockDB.app_branch.Include("app_location")
                .Where(a => a.can_stock == true
                         && a.is_active == true
                         && a.id_company == _entity_Pref.company_ID)
                         .OrderBy(a => a.name).ToListAsync();
            dgvBranch.ItemsSource = StockDB.app_branch.Local;

            InventoryDate = DateTime.Now;
        }

        private void calc_Inventory(int id_item)
        {
            app_branch app_branch = (app_branch)dgvBranch.SelectedItem;

            if (app_branch != null && app_branch.id_branch > 0)
            {
                int id_branch = app_branch.id_branch;

                if (id_item > 0)
                {
                    var movement =
                      (from items in StockDB.items
                       join item_product in StockDB.item_product on items.id_item equals item_product.id_item
                           into its
                       from p in its
                       join item_movement in StockDB.item_movement on p.id_item_product equals item_movement.id_item_product
                       into IMS
                       from a in IMS
                       join AM in StockDB.app_branch on a.app_location.id_branch equals AM.id_branch
                       where a.status == Status.Stock.InStock
                          && a.trans_date <= InventoryDate
                          && a.app_location.id_branch == id_branch && a.item_product.id_item == id_item
                       group a by new { a.item_product ,a.app_location}
                           into last
                           select new
                           {
                               code = last.Key.item_product.item.code,
                               name = last.Key.item_product.item.name,
                               location = last.Key.app_location.name,
                               //location = last.OrderBy(m => m.app_location.name),
                               itemid = last.Key.item_product.item.id_item,
                               quntitiy = last.Sum(x => x.credit != null ? x.credit : 0) - last.Sum(x => x.debit != null ? x.debit : 0),
                               id_item_product = last.Key.item_product.id_item_product,
                               measurement = last.Key.item_product.item.id_measurement
                           }).ToList();

                    item_movementDataGrid.ItemsSource = movement;

                }
                else
                {
                    var movement =
                    (from items in StockDB.items
                     join item_product in StockDB.item_product on items.id_item equals item_product.id_item
                         into its
                     from p in its
                     join item_movement in StockDB.item_movement on p.id_item_product equals item_movement.id_item_product
                     into IMS
                     from a in IMS
                     join AM in StockDB.app_branch on a.app_location.id_branch equals AM.id_branch
                     where a.status == Status.Stock.InStock
                        && a.trans_date <= InventoryDate
                        && a.app_location.id_branch == id_branch
                     group a by new { a.item_product,a.app_location }
                         into last
                         select new
                         {
                             code = last.Key.item_product.item.code,
                             name = last.Key.item_product.item.name,
                          
                             location = last.Key.app_location.name,
                             itemid = last.Key.item_product.item.id_item,
                             quntitiy = last.Sum(x => x.credit != null ? x.credit : 0) - last.Sum(x => x.debit != null ? x.debit : 0),
                             id_item_product = last.Key.item_product.id_item_product,
                             measurement = last.Key.item_product.item.id_measurement
                         }).ToList();

                    item_movementDataGrid.ItemsSource = movement;
                }

            }
        }

        private void itemDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            calc_Inventory(0);
        }

        private void slider_ValueChanged(object sender, EventArgs e)
        {
            InventoryDate = InventoryDate.AddDays(slider.Value - InventoryDate.Day);
        }

        private void RRMonth_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            InventoryDate = InventoryDate.AddMonths(-1);
        }

        private void RRDay_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            InventoryDate = InventoryDate.AddDays(-1);
        }

        private void FFDay_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            InventoryDate = InventoryDate.AddDays(1);
        }

        private void FFMonth_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            InventoryDate = InventoryDate.AddMonths(1);
        }

        private void Today_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            InventoryDate = DateTime.Now;
        }

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private async void item_movementDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic _item_movement = item_movementDataGrid.SelectedItem;
            if (_item_movement != null)
            {
                int id_item_product = _item_movement.id_item_product;

                app_branch app_branch = (app_branch)dgvBranch.SelectedItem;
                int id_branch = app_branch.id_branch;

                using (db db = new db())
                {
                    item_movementViewSource = ((CollectionViewSource)(FindResource("item_movementViewSource")));
                    item_movementViewSource.Source = await db.item_movement.Where(x => x.id_company == _entity_Pref.company_ID
                                                        && x.id_item_product == id_item_product
                                                        && x.app_location.id_branch == id_branch
                                                        && x.status == Status.Stock.InStock
                                                        && x.trans_date <= InventoryDate
                                                        ).AsNoTracking().ToListAsync();
                }
            }
        }

        private void itemComboBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (itemComboBox.Data != null)
                {
                    calc_Inventory(((item)itemComboBox.Data).id_item);
                    itemComboBox._focusgrid = false;
                    itemComboBox.Text = ((item)itemComboBox.Data).name;
                }

            }
        }

        private void itemComboBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            calc_Inventory(((item)itemComboBox.Data).id_item);
            itemComboBox._focusgrid = false;
            itemComboBox.Text = ((item)itemComboBox.Data).name;
        }

    }
}
