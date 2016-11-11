using System;
using System.Windows;
using System.Windows.Controls;

namespace cntrl
{
    public partial class DateRange : UserControl
    {

        public static DependencyProperty StartDateProperty = DependencyProperty.Register("StartDate", typeof(DateTime), typeof(DateRange));
        public DateTime StartDate
        {
            get { return (DateTime)GetValue(StartDateProperty); }
            set { SetValue(StartDateProperty, value); }
        }

        public static DependencyProperty EndDateProperty = DependencyProperty.Register("EndDate", typeof(DateTime), typeof(DateRange));
        public DateTime EndDate
        {
            get { return (DateTime)GetValue(EndDateProperty); }
            set { SetValue(EndDateProperty, value); }
        }
        
        public DateRange()
        {
            InitializeComponent();
        }

        private void btnYear_Click(object sender, RoutedEventArgs e)
        {
            if (StartDate.Year == EndDate.Year)
            {
                lblDateHeader.Content = StartDate.Year.ToString();
            }
            else
            {
                lblDateHeader.Content = string.Format("{0} - {1}", StartDate.Year.ToString(), EndDate.Year.ToString());
            }
        }

        private void btnMonth_Click(object sender, RoutedEventArgs e)
        {
            if (StartDate.Month == EndDate.Month)
            {
                lblDateHeader.Content = string.Format("{0} - {1}", StartDate.Month.ToString(), EndDate.Year.ToString());
            }
            else
            {
                lblDateHeader.Content = string.Format("{0} {1} - {2} {3}", StartDate.Month.ToString("MMMM"), StartDate.Year.ToString(), EndDate.Month.ToString("MMMM"), EndDate.Year.ToString());
            }
        }

        private void btnDay_Click(object sender, RoutedEventArgs e)
        {
            lblDateHeader.Content = StartDate.Date.ToShortDateString() + " - " + EndDate.Date.ToShortDateString();
        }

        private void btnDateForward(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void btnDateBack(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }

    public static class AbsoluteDate
    {
        /// <summary>
        /// Gets the 12:00:00 instance of a DateTime
        /// </summary>
        public static DateTime Start(this DateTime dateTime)
        {
            return dateTime.Date;
        }

        /// <summary>
        /// Gets the 11:59:59 instance of a DateTime
        /// </summary>
        public static DateTime End(this DateTime dateTime)
        {
            return Start(dateTime).AddDays(1).AddTicks(-1);
        }
    }
}
