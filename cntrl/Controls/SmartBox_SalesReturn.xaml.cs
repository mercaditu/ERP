using cntrl.Controls;
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
    public partial class SmartBox_SalesReturn : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SmartBox_SalesReturn));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

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
                entity.BrilloQuery.Return sales_return = returnViewSource.View.CurrentItem as entity.BrilloQuery.Return;

                if (sales_return != null)
                {
                    ReturnID = sales_return.ID;
                    Text = sales_return.code;

                    popContact.IsOpen = false;

                    if (Select != null)
                    { Select(this, new RoutedEventArgs()); }
                }
            }
        }
        public int ReturnID { get; set; }
        public IQueryable<entity.BrilloQuery.Return> ReturnList { get; set; }


        Task taskSearch;
        CancellationTokenSource tokenSource;
        CancellationToken token;

        CollectionViewSource returnViewSource;

        public SmartBox_SalesReturn()
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
            using (entity.BrilloQuery.SalesRetrunQuery Execute = new entity.BrilloQuery.SalesRetrunQuery())
            {
                ReturnList = Execute.List.AsQueryable();
            }

            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate () { progBar.Visibility = Visibility.Collapsed; }));
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

           

            if (param.Contains("code"))
            {
                predicateOR = predicateOR.Or(x => x.code.ToUpper().Contains(SearchText));
            }

            if (param.Contains("number"))
            {
                predicateOR = predicateOR.Or(x => x.number.ToUpper().Contains(SearchText));
            }

            if (param.Contains("comment"))
            {
                predicateOR = predicateOR.Or(x => x.comment.ToUpper().Contains(SearchText));
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
