using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace cntrl
{
    public partial class navPagination : UserControl
    {
        //public event PropertyChangedEventHandler PropertyChanged;

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

		public event btnFirstPage_ClickedEventHandler btnFirstPage_Click;

		public delegate void btnFirstPage_ClickedEventHandler(object sender);

		public void btnFirstPage_mouseup(object sender, EventArgs e)
		{


			btnFirstPage_Click?.Invoke(this);
		}

		public event btnNextPage_ClickedEventHandler btnNextPage_Click;

		public delegate void btnNextPage_ClickedEventHandler(object sender);

		public void btnNextPage_mouseup(object sender, EventArgs e)
		{


			btnNextPage_Click?.Invoke(this);
		}

		public event btnPreviousPage_ClickedEventHandler btnPreviousPage_Click;

		public delegate void btnPreviousPage_ClickedEventHandler(object sender);

		public void btnPreviousPage_mouseup(object sender, EventArgs e)
		{


			btnPreviousPage_Click?.Invoke(this);
		}


		public event btnLastPage_ClickedEventHandler btnLastPage_Click;

		public delegate void btnLastPage_ClickedEventHandler(object sender);

		public void btnLastPage_mouseup(object sender, EventArgs e)
		{


			btnLastPage_Click?.Invoke(this);
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