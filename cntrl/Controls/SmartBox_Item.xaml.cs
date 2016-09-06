using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        public bool Exclude_OutOfStock
        {
            get { return _Exclude_OutOfStock; }
            set
            {
                if (_Exclude_OutOfStock != value)
                {
                    _Exclude_OutOfStock = value;
                    RaisePropertyChanged("Exclude_OutOfStock");
                }
            }
        }
        bool _Exclude_OutOfStock;

        public bool can_New
        {
            get { return _can_new; }
            set
            {
                entity.Brillo.Security Sec = new entity.Brillo.Security(entity.App.Names.Items);
                if (Sec.create)
                {
                    _can_new = value;
                    RaisePropertyChanged("can_New");
                }
                else
                {
                    _can_new = false;
                    RaisePropertyChanged("can_New");
                }
            }
        }
        bool _can_new;

        public bool can_Edit
        {
            get { return _can_new; }
            set
            {
                entity.Brillo.Security Sec = new entity.Brillo.Security(entity.App.Names.Items);
                if (Sec.edit)
                {
                    _can_edit = value;
                    RaisePropertyChanged("can_Edit");
                }
                else
                {
                    _can_edit = false;
                    RaisePropertyChanged("can_Edit");
                }
            }
        }
        bool _can_edit;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SmartBox_Item));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public event RoutedEventHandler Select;
        private void ItemGrid_MouseDoubleClick(object sender, EventArgs e)
        {
            if (itemViewSource != null)
            {
                if (itemViewSource.View != null)
                {
                    entity.BrilloQuery.Item Item = itemViewSource.View.CurrentItem as entity.BrilloQuery.Item;

                    if (Item != null)
                    {
                        ItemID = Item.ID;
                        ItemPopUp.IsOpen = false;
                        Text = Item.Name;
                    }
                    else
                    {
                        ItemID = 0;
                        Text = tbxSearch.Text;
                    }

                    tbxSearch.SelectAll();
                }
            }

            if (Select != null)
            { Select(this, new RoutedEventArgs()); }
        }

        public int ItemID { get; set; }
        public entity.item.item_type? item_types { get; set; }

        public IQueryable<entity.BrilloQuery.Item> Items { get; set; }

        Task taskSearch;
        CancellationTokenSource tokenSource;
        CancellationToken token;

        CollectionViewSource itemViewSource;

        public SmartBox_Item()
        {
            InitializeComponent();

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            Task task = Task.Factory.StartNew(() => LoadData());
            this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(LoginControl_IsVisibleChanged);
            itemViewSource = ((CollectionViewSource)(FindResource("itemViewSource")));
        }

        private void LoadData()
        {
            Items = null;
            using (entity.BrilloQuery.GetItems Execute = new entity.BrilloQuery.GetItems())
            {
                Dispatcher.BeginInvoke(
               DispatcherPriority.ContextIdle,
               new Action(delegate()
               {
                   Items = Execute.Items.AsQueryable();
               }));
            }
        }

        void LoginControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                Dispatcher.BeginInvoke(
                DispatcherPriority.ContextIdle,
                new Action(delegate()
                {
                    tbxSearch.Focus();
                }));
            }
        }

        private void StartSearch(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ItemGrid_MouseDoubleClick(sender, e);
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

                if(SearchText.Count() >= 1)
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
            var predicate = PredicateBuilder.True<entity.BrilloQuery.Item>();

            if (smartBoxItemSetting.Default.ExactSearch)
            {
                predicate = (x => x.IsActive && x.ComapnyID == entity.CurrentSession.Id_Company && ( x.Code == SearchText ));
            }
            else
            {
                predicate = (x => x.IsActive && x.ComapnyID == entity.CurrentSession.Id_Company &&
                    (
                        x.Code.ToLower().Contains(SearchText.ToLower()) ||
                        x.Name.ToLower().Contains(SearchText.ToLower()) ||
                        x.Brand.ToLower().Contains(SearchText.ToLower())
                    ));

                if (item_types != null)
                {
                    predicate = predicate.And(x => x.Type == item_types);
                }
            }

            Dispatcher.InvokeAsync(new Action(() =>
            {
                itemViewSource.Source = Items.Where(predicate).OrderBy(x => x.Name).ToList();

                ItemPopUp.IsOpen = true;
                progBar.IsActive = false;
            }));
        }

        private void Add_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            entity.Brillo.Security Sec = new entity.Brillo.Security(entity.App.Names.Items);
            if (Sec.create)
            {
                cntrl.Curd.item item = new Curd.item();
                item.itemobject = new entity.item();
                popCrud.IsOpen = true;
                popCrud.Visibility = System.Windows.Visibility.Visible;
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
            popupCustomize.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Label_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            popupCustomize.IsOpen = true;
            popupCustomize.Visibility = System.Windows.Visibility.Visible;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            //if (Controls.smartBoxItemSetting.Default.SearchFilter != null)
            //{
            //    Controls.smartBoxItemSetting.Default.SearchFilter.Clear();
            //}

            //if (rbtnCode.IsChecked == true)
            //{
            //    Controls.smartBoxItemSetting.Default.SearchFilter.Add("Code");
            //}
            //if (rbtnName.IsChecked == true)
            //{
            //    Controls.smartBoxItemSetting.Default.SearchFilter.Add("Name");
            //}
            //if (rbtnTag.IsChecked == true)
            //{
            //    Controls.smartBoxItemSetting.Default.SearchFilter.Add("Tag");
            //}
            //if (rbtnExactCode.IsChecked == true)
            //{
            //    Controls.smartBoxItemSetting.Default.SearchFilter.Add("ExactCode");
            //}

            Controls.smartBoxItemSetting.Default.Save();
        }

        private void Refresh_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

            Task task = Task.Factory.StartNew(() => LoadData());
        }
    }
}
