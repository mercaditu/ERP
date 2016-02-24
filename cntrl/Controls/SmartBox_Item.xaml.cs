using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace cntrl.Controls
{
    public partial class SmartBox_Item : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SmartBox_Item));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public event RoutedEventHandler Select;
        private void ItemGrid_MouseDoubleClick(object sender, EventArgs e)
        {
            if (itemViewSource.View != null)
            {
                Item = itemViewSource.View.CurrentItem as entity.item;

                if (Item != null)
                {
                    ItemID = Item.id_item;
                    ContactPopUp.IsOpen = false;
                    Text = Item.name;
                }
                else
                {
                    ItemID = 0;
                    Text = tbxSearch.Text;
                }
            }

            if (Select != null)
            { Select(this, new RoutedEventArgs()); }
        }

        public int ItemID { get; set; }
        public entity.item Item { get; set; }
        public entity.item.item_type? item_types { get; set; }
        int company_ID = entity.Properties.Settings.Default.company_ID;

        Task taskSearch;
        CancellationTokenSource tokenSource;
        CancellationToken token;

        CollectionViewSource itemViewSource;

        public SmartBox_Item()
        {
            InitializeComponent();
            itemViewSource = ((CollectionViewSource)(FindResource("itemViewSource")));
        }

        private void StartSearch(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ItemGrid_MouseDoubleClick(sender, e);
            }

            else if (e.Key == Key.Up)
            {
                itemViewSource.View.MoveCurrentToPrevious();
                itemViewSource.View.Refresh();
            }
            else if (e.Key == Key.Down)
            {
                itemViewSource.View.MoveCurrentToNext();
                itemViewSource.View.Refresh();
            }
            else
            {

                string SearchText = tbxSearch.Text;

                if (SearchText.Count() >= 3)
                {
                    if (taskSearch != null)
                    {
                        if (taskSearch.Status == TaskStatus.Running)
                        {
                            tokenSource.Cancel();
                        }
                    }

                    progBar.IsActive = true;

                    tokenSource = new CancellationTokenSource();
                    token = tokenSource.Token;
                    taskSearch = Task.Factory.StartNew(() => Search_OnThread(SearchText), token);
                }
                else
                {
                    //contactViewSource.Source = null;
                }
            }
        }

        private void Search_OnThread(string SearchText)
        {
            using (entity.db db = new entity.db())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.AutoDetectChangesEnabled = false;

                List<entity.item> results;
                if (item_types == null)
                {
                    results = db.items
                               .Where(x =>
                                         x.id_company == company_ID &&
                                         (
                                             x.code.Contains(SearchText) ||
                                             x.name.Contains(SearchText) ||
                                             x.item_brand.name.Contains(SearchText)
                                         ) &&
                                        x.is_active == true
                                       )
                               .OrderBy(x => x.name)
                               .ToList();
                }
                else
                {
                    results = db.items
                                  .Where(x =>
                                            x.id_company == company_ID && x.id_item_type==item_types &&
                                            (
                                                x.code.Contains(SearchText) ||
                                                x.name.Contains(SearchText) ||
                                                x.item_brand.name.Contains(SearchText)
                                            ) &&
                                           x.is_active == true
                                          )
                                  .OrderBy(x => x.name)
                                  .ToList();

                }

                Dispatcher.InvokeAsync(new Action(() =>
                {
                    //itemViewSource = ((CollectionViewSource)(FindResource("itemViewSource")));
                    itemViewSource.Source = results;
                    Item = itemViewSource.View.CurrentItem as entity.item;

                    ContactPopUp.IsOpen = true;
                    progBar.IsActive = false;
                }));
            }
        }
    }
}
