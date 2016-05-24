using System;
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
        StockDB StockDB;

        CollectionViewSource item_movementViewSource, inventoryViewSource;
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

                calc_Inventory();
            }
        }
        DateTime _InventoryDate = DateTime.Now;

        public Stock()
        {
            InitializeComponent();
        }

        private async void StockPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            StockDB = new StockDB();
            await StockDB.app_branch.Include(x => x.app_location)
                .Where(a => a.can_stock == true
                         && a.is_active == true
                         && a.id_company == entity.CurrentSession.Id_Company)
                         .OrderBy(a => a.name).ToListAsync();
            dgvBranch.ItemsSource = StockDB.app_branch.Local;

            InventoryDate = DateTime.Now;
        }

        private void calc_Inventory()
        {
            app_branch app_branch = (app_branch)dgvBranch.SelectedItem;

            if (app_branch != null && app_branch.id_branch > 0)
            {
                int id_branch = app_branch.id_branch;

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
                 group a by new { a.item_product, a.app_location }
                     into last
                     select new
                     {
                         code = last.Key.item_product.item.code,
                         name = last.Key.item_product.item.name,
                         location = last.Key.app_location.name,
                         itemid = last.Key.item_product.item.id_item,
                         quantity = last.Sum(x => x.credit) - last.Sum(x => x.debit),
                         id_item_product = last.Key.item_product.id_item_product,
                         measurement = last.Key.item_product.item.app_measurement.code_iso,
                         id_location = last.Key.app_location.id_location
                     }).ToList();

                inventoryViewSource = ((CollectionViewSource)(FindResource("inventoryViewSource")));
                inventoryViewSource.Source = movement;

            }
        }

        private void itemDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            calc_Inventory();
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
            dynamic _item_movement = item_inventoryDataGrid.SelectedItem;

            if (_item_movement != null)
            {
                int id_item_product = _item_movement.id_item_product;

                int id_location = _item_movement.id_location;
                using (db db = new db())
                {



                    item_movementViewSource = ((CollectionViewSource)(FindResource("item_movementViewSource")));
                    item_movementViewSource.Source = await db.item_movement.Where(x => x.id_company == entity.CurrentSession.Id_Company
                                                        && x.id_item_product == id_item_product
                                                        && x.app_location.id_location == id_location
                                                        && x.status == Status.Stock.InStock
                                                        && x.trans_date <= InventoryDate
                                                        ).Take(10).AsNoTracking().OrderByDescending(x => x.trans_date).ToListAsync();
                    foreach (item_movement item_movement in item_movementViewSource.View.Cast<item_movement>().ToList())
                    {
                        foreach (item_movement_dimension item_movement_dimension in item_movement.item_movement_dimension)
                        {
                            if (!(item_movement.comment.Contains(item_movement_dimension.app_dimension.name)))
                            {
                                item_movement.comment += " " + item_movement_dimension.app_dimension.name + " : " + item_movement_dimension.value + ",";
                            }
                            
                        }
                        
                    }
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (inventoryViewSource != null)
            {
                if (inventoryViewSource.View != null)
                {
                    inventoryViewSource.View.Filter = i =>
                    {
                        dynamic TmpInventory = (dynamic)i;
                        if (TmpInventory.code.ToUpper().Contains(txtsearch.Text.ToUpper()) || TmpInventory.name.ToUpper().Contains(txtsearch.Text.ToUpper()))
                            return true;
                        else
                            return false;
                    };
                }
            }
        }
    }


}
