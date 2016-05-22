using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows;

namespace cntrl
{
    public partial class navPagination : UserControl
    {
      
        public event PropertyChangedEventHandler PropertyChanged;

        public static readonly DependencyProperty PageIndexProperty =
               DependencyProperty.Register("PageIndex", typeof(int), typeof(navPagination));
        public int PageIndex
        {
            get { return int.Parse(GetValue(PageIndexProperty).ToString()); }
            set { SetValue(PageIndexProperty, value); }
        }

        public static readonly DependencyProperty NumberOfPagesProperty =
                DependencyProperty.Register("NumberOfPages", typeof(int), typeof(navPagination));
        public int NumberOfPages
        {
            get { return int.Parse(GetValue(NumberOfPagesProperty).ToString()); }
            set { SetValue(NumberOfPagesProperty, value); }
        }

        public navPagination()
        {
            InitializeComponent();
        }

        private void btnFirstPage_Click(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnPreviousPage_Click(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnNextPage_Click(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnLastPage_Click(object sender, MouseButtonEventArgs e)
        {

        }

        public static readonly RoutedEvent NextPageClick = EventManager.RegisterRoutedEvent("Next", RoutingStrategy.Bubble,
        typeof(RoutedEventHandler), typeof(navPagination));
        public event RoutedEventHandler NextPage
        {
            add { AddHandler(NextPageClick, value); }
            remove { RemoveHandler(NextPageClick, value); }
        }
    }
}
