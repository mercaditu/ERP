using System;
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
    public partial class SmartBox_PurchaseReturn : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SmartBox_PurchaseReturn));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty ContactIDProperty = DependencyProperty.Register("ContactID", typeof(int), typeof(SmartBox_PurchaseReturn));

        public int ContactID
        {
            get { return Convert.ToInt32(GetValue(ContactIDProperty)); }
            set { SetValue(ContactIDProperty, value); }
        }

        public decimal Balance { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public event RoutedEventHandler Select;

        private void ReturnGrid_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (returnViewSource.View != null)
            {
                entity.BrilloQuery.Return Purchase_return = returnViewSource.View.CurrentItem as entity.BrilloQuery.Return;

                if (Purchase_return != null)
                {
                    ReturnID = Purchase_return.ID;
                    Balance = Purchase_return.Balance;
                    //Text = Purchase_return.code;

                    popContact.IsOpen = false;

                    if (Select != null)
                    { Select(this, new RoutedEventArgs()); }
                }
            }
        }

        public int ReturnID { get; set; }
        public IQueryable<entity.BrilloQuery.Return> ReturnList { get; set; }

        private Task taskSearch;
        private CancellationTokenSource tokenSource;
        private CancellationToken token;

        private CollectionViewSource returnViewSource;

        public SmartBox_PurchaseReturn()
        {
            InitializeComponent();

            ///Exists code if in design view.
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            returnViewSource = ((CollectionViewSource)(FindResource("returnViewSource")));

            LoadData();

            this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(LoginControl_IsVisibleChanged);
            Controls.smartBoxContactSetting.Default.SearchFilter.Add("Name");

            Controls.smartBoxContactSetting.Default.SearchFilter.Add("code");

            Controls.smartBoxContactSetting.Default.SearchFilter.Add("number");

            Controls.smartBoxContactSetting.Default.SearchFilter.Add("comment");
        }

        public void LoadData()
        {
            progBar.Visibility = Visibility.Visible;
            Task task = Task.Factory.StartNew(() => LoadData_Thread());
        }

        private void LoadData_Thread()
        {
            ReturnList = null;
            Dispatcher.BeginInvoke(
                  DispatcherPriority.ContextIdle,
                  new Action(delegate ()
                  {
                      using (entity.BrilloQuery.PurchaseReturnQuery Execute = new entity.BrilloQuery.PurchaseReturnQuery(ContactID))
                      {
                          ReturnList = Execute.List.AsQueryable();
                      }
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

        private void StartSearch(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ReturnGrid_MouseDoubleClick(sender, e);
            }
            else if (e.Key == Key.Up)
            {
                if (returnViewSource != null)
                {
                    if (returnViewSource.View != null)
                    {
                        returnViewSource.View.MoveCurrentToPrevious();
                        returnViewSource.View.Refresh();
                    }
                }
            }
            else if (e.Key == Key.Down)
            {
                if (returnViewSource != null)
                {
                    if (returnViewSource.View != null)
                    {
                        returnViewSource.View.MoveCurrentToNext();
                        returnViewSource.View.Refresh();
                    }
                }
            }
            else
            {
                string SearchText = tbxSearch.Text;

                if (SearchText.Count() >= 1)
                {
                    if (taskSearch != null)
                    {
                        if (taskSearch.Status == TaskStatus.Running)
                        {
                            tokenSource.Cancel();
                        }
                    }

                    tokenSource = new CancellationTokenSource();
                    token = tokenSource.Token;
                    taskSearch = Task.Factory.StartNew(() => Search_OnThread(SearchText), token);
                }
            }
        }

        private void Search_OnThread(string SearchText)
        {
            SearchText = SearchText.ToUpper();
            var predicate = PredicateBuilder.True<entity.BrilloQuery.Return>();

            var predicateOR = PredicateBuilder.False<entity.BrilloQuery.Return>();
            var param = smartBoxContactSetting.Default.SearchFilter;

            if (param.Contains("Name"))
            {
                predicateOR = predicateOR.Or(x => x.Name.ToUpper().Contains(SearchText));
            }
            if (param.Contains("code"))
            {
                predicateOR = predicateOR.Or(x => x.Code.ToUpper().Contains(SearchText));
            }

            if (param.Contains("number"))
            {
                predicateOR = predicateOR.Or(x => x.Number.ToUpper().Contains(SearchText));
            }

            if (param.Contains("comment"))
            {
                predicateOR = predicateOR.Or(x => x.Comment.ToUpper().Contains(SearchText));
            }

            predicate = predicate.And
            (
                predicateOR
            );

            Dispatcher.InvokeAsync(new Action(() =>
            {
                if (popContact.IsOpen == false)
                {
                    popContact.IsOpen = true;
                }
                try
                {
                    returnViewSource.Source = ReturnList.Where(predicate).ToList();
                }
                catch (Exception)
                {
                    throw;
                }
            }));
        }
    }
}