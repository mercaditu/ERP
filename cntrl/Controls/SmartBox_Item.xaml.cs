using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace cntrl.Controls
{
    public partial class SmartBox_Item : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string prop)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        //Settings that if Marked True will show Quantity Popup.
        public static readonly DependencyProperty QuantityIntegrationProperty = DependencyProperty.Register("QuantityIntegration", typeof(bool), typeof(SmartBox_Item));

        public bool QuantityIntegration
        {
            get { return (bool)GetValue(QuantityIntegrationProperty); }
            set { SetValue(QuantityIntegrationProperty, value); }
        }

        //Quantity of the Popup control if used.
        public decimal Quantity
        {
            get { return _Quantity; }
            set
            {
                _Quantity = value;
                RaisePropertyChanged("Quantity");
            }
        }

        private decimal _Quantity;

        //Setting that if Marked true, will exclude Out Of Stock.
        private bool _Exclude_OutOfStock;
        public bool Exclude_OutOfStock
        {
            get { return _Exclude_OutOfStock; }
            set
            {
                if (_Exclude_OutOfStock != value)
                {
                    _Exclude_OutOfStock = value;
                    RaisePropertyChanged("Exclude_OutOfStock");

                    //Filters Items list to only include InStock Items or Non-Products
                    if (Items != null)
                    {
                        Items = Items.AsQueryable().Where(x => x.Quantity > 0);
                    }
                }
            }
        }

        private bool _ExactSearch;
        public bool ExactSearch
        {
            get { return _ExactSearch; }
            set
            {

                entity.Brillo.Security Sec = new entity.Brillo.Security(entity.App.Names.Items);
                _ExactSearch = Sec.SpecialSecurity_ReturnsBoolean(entity.Privilage.Privilages.ItemBarcodeSearchOnly) ? value : false;
                RaisePropertyChanged("ExactSearch");
            }
        }

        public bool can_New
        {
            get { return _can_new; }
            set
            {
                _can_new = new entity.Brillo.Security(entity.App.Names.Items).create ? value : false;
                RaisePropertyChanged("can_New");
            }
        }

        private bool _can_new;

        public bool can_Edit
        {
            get { return _can_new; }
            set
            {
                entity.Brillo.Security Sec = new entity.Brillo.Security(entity.App.Names.Items);
                _can_edit = new entity.Brillo.Security(entity.App.Names.Items).edit ? value : false;
                RaisePropertyChanged("can_Edit");
            }
        }

        private bool _can_edit;

        public decimal QuantityInStock { get; set; }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SmartBox_Item));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty LocationIDProperty = DependencyProperty.Register("LocationID", typeof(int), typeof(SmartBox_Item), new FrameworkPropertyMetadata(
            0,
            new PropertyChangedCallback(OnLocationChangeCallBack)));

        public int LocationID
        {
            get { return (int)GetValue(LocationIDProperty); }
            set { SetValue(LocationIDProperty, value); }
        }


        #region "INotifyPropertyChanged"

        private static void OnLocationChangeCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SmartBox_Item c = sender as SmartBox_Item;
            if (c != null)
            {
                c.OnLocationChange((int)e.NewValue);
            }
        }

        protected virtual void OnLocationChange(int newvalue)
        {
            var task = Task.Factory.StartNew(() => LoadData(newvalue));
        }


        #endregion "INotifyPropertyChanged"
        public event RoutedEventHandler Select;

        private void ItemGrid_MouseDoubleClick(object sender, EventArgs e)
        {
            if (itemViewSource != null)
            {
                if (itemViewSource.View != null)
                {
                    entity.Brillo.StockList Item = itemViewSource.View.CurrentItem as entity.Brillo.StockList;

                    if (Item != null)
                    {
                        ItemID = Item.ItemID;
                        if (Item.Quantity!=null)
                        {
                            QuantityInStock = (decimal)Item.Quantity;
                        }
                      
                        ItemPopUp.IsOpen = false;
                        //  Text = Item.Name;
                        if (Quantity <= 1)
                        {
                            Quantity = 1;
                        }
                    }
                    else
                    {
                        ItemID = 0;
                        if (Quantity <= 1)
                        {
                            Quantity = 1;
                        }
                        Text = tbxSearch.Text;
                    }

                }
            }

            Select?.Invoke(this, new RoutedEventArgs());
            Text = "";
            tbxSearch.Focus();
            tbxSearch.SelectAll();
        }

        public int ItemID { get; set; }

        public entity.item.item_type? item_types { get; set; }

        public IQueryable<entity.Brillo.StockList> Items { get; set; }
        //  public IQueryable<entity.BrilloQuery.Item> Items { get; set; }

        //private Task taskSearch;
        private CancellationTokenSource tokenSource;

        private CancellationToken token;

        private CollectionViewSource itemViewSource;

        public SmartBox_Item()
        {
            InitializeComponent();


            entity.Brillo.Security Sec = new entity.Brillo.Security(entity.App.Names.Items);
            Exclude_OutOfStock = Sec.SpecialSecurity_ReturnsBoolean(entity.Privilage.Privilages.Include_OutOfStock) == true ? false : true;

            smartBoxItemSetting Settings = new smartBoxItemSetting();
            if (Exclude_OutOfStock)
            {
                Settings.Exclude_OutOfStock = Exclude_OutOfStock;
            }
            else if (item_types == entity.item.item_type.Product || item_types == entity.item.item_type.RawMaterial)
            {
                Settings.Exclude_OutOfStock = Exclude_OutOfStock;
            }

            if (entity.CurrentSession.Allow_BarCodeSearchOnly)
            {
                Settings.ExactSearch = true;
                ExactSearch = true;
            }
            else
            {
                ExactSearch = false;
            }
            smartBoxItemSetting.Default.Save();

            this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(LoginControl_IsVisibleChanged);
            itemViewSource = ((CollectionViewSource)(FindResource("itemViewSource")));
        }

        private void _SmartBox_Item_Loaded(object sender, RoutedEventArgs e)
        {
            int LocId = LocationID;
            LoadData(LocationID);

            //Basic Data like Salesman, Contracts, VAT, Currencies, etc to speed up Window Load.
            //Load_BasicData(null, null);
            //Load Basic Data into Timer.
            //System.Timers.Timer myTimer = new System.Timers.Timer();
            //myTimer.Elapsed += new ElapsedEventHandler(LoadData);
            //myTimer.Interval = 60000;
            //myTimer.Start();
        }

        private void LoadData(int LocId)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate ()
            {
                progBar.Visibility = Visibility.Visible;
                tbxSearch.IsEnabled = false;
            }));

            LoadData_Thread(LocId);
           // var task = Task.Factory.StartNew(() => LoadData_Thread(LocId, IgnorStock));
        }

        private void LoadData_Thread(int LocID)
        {
            Items = null;

            entity.Brillo.Stock Execute = new entity.Brillo.Stock();
            
            if (LocID == 0)
            {
                Items = Execute.getItems_ByBranch(entity.CurrentSession.Id_Branch).AsQueryable();
            }
            else
            {
                Items = Execute.getItems_ByBranch(entity.CurrentSession.Id_Branch).Where(x => x.LocationID == LocID || x.LocationID == null).AsQueryable();
            }

            if (Exclude_OutOfStock)
            {
                Items = Items.Where(x => x.Quantity > 0).AsQueryable();
            }
            
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate ()
            {
                tbxSearch.IsEnabled = true;
                progBar.Visibility = Visibility.Collapsed;
            }));
        }

        private void LoginControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                Dispatcher.BeginInvoke(
                DispatcherPriority.ContextIdle,
                new Action(delegate ()
                {
                    tbxSearch.Focus();
                }));
            }
        }

        private void Quantity_TextChanged(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (QuantityIntegration)
                {
                    //  Quantity = 1;
                    popQuantity.IsOpen = false;
                    ItemGrid_MouseDoubleClick(sender, e);
                    FocusManager.SetFocusedElement(tbxSearch.Parent, tbxSearch);
                }
            }
        }

        private void StartSearch(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (QuantityIntegration)
                {
                    Quantity = 1;
                    ItemPopUp.IsOpen = false;
                    popQuantity.IsOpen = true;
                    tbxQuantity.Focus();
                }
                else
                {
                    Quantity = 1;
                    ItemGrid_MouseDoubleClick(sender, e);
                }
            }
            else if (e.Key == Key.Up)
            {
                if (itemViewSource != null)
                {
                    if (itemViewSource.View != null)
                    {
                        itemViewSource.View.MoveCurrentToPrevious();
                        itemViewSource.View.Refresh();
                    }
                }
            }
            else if (e.Key == Key.Down)
            {
                if (itemViewSource != null)
                {
                    if (itemViewSource.View != null)
                    {
                        itemViewSource.View.MoveCurrentToNext();
                        itemViewSource.View.Refresh();
                    }
                }
            }
            else
            {
                string SearchText = tbxSearch.Text;

                if (SearchText.Count() >= 1)
                {
                    tokenSource = new CancellationTokenSource();
                    token = tokenSource.Token;
                    Search_OnThread(SearchText);
                }
            }
        }

        private void Search_OnThread(string SearchText)
        {

            var predicate = PredicateBuilder.True<entity.Brillo.StockList>();
            entity.Brillo.Security Sec = new entity.Brillo.Security(entity.App.Names.Items);

            if (Sec.SpecialSecurity_ReturnsBoolean(entity.Privilage.Privilages.ItemBarcodeSearchOnly))
            {

            }

            if (_ExactSearch)
            {
                predicate = (x => (x.CompanyID == entity.CurrentSession.Id_Company) && (x.Code == SearchText));
            }
            else
            {
                predicate = (x => (x.CompanyID == entity.CurrentSession.Id_Company) &&
                    (
                        x.Code.ToLower().Contains(SearchText.ToLower()) 
                        || x.Name.ToLower().Contains(SearchText.ToLower()) 
                        //|| x.Brand.ToLower().Contains(SearchText.ToLower())
                    ));

                if (item_types != null)
                {
                    predicate = predicate.And(x => x.Type == (int)item_types);
                }
                //if (Exclude_OutOfStock == true)
                //{
                //    predicate = predicate.And(x => x.Quantity > 0);
                //}
            }
            List<entity.Brillo.StockList> ItemList = Items.Where(predicate).OrderBy(x => x.Name).ToList();
            itemViewSource.Source = ItemList;
            //       itemViewSource.Source = Items;
            ItemPopUp.IsOpen = true;
        }

        private void Add_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            entity.Brillo.Security Sec = new entity.Brillo.Security(entity.App.Names.Items);
            if (Sec.create)
            {
                Curd.item item = new Curd.item();
                item.itemobject = new entity.item();
                popCrud.IsOpen = true;
                popCrud.Visibility = Visibility.Visible;
                ContactPopUp.Children.Add(item);
            }
        }

        private void crudItem_btnCancel_Click(object sender)
        {
            popCrud.IsOpen = false;
        }

        private void popupCustomize_Closed(object sender, EventArgs e)
        {
            popupCustomize.IsOpen = false;
            popupCustomize.Visibility = Visibility.Collapsed;
        }

        private void Label_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            popupCustomize.IsOpen = true;
            popupCustomize.Visibility = Visibility.Visible;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            smartBoxItemSetting.Default.Save();
        }

        private void Refresh_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            LoadData(LocationID);
        }

        public void SmartBoxItem_Focus()
        {
            tbxSearch.Focus();
        }
    }
}