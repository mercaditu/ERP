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

        public event RoutedEventHandler DateChanged;
        private void DateChanged_Click(object sender, RoutedEventArgs e)
        {
            DateChanged?.Invoke(this, new RoutedEventArgs());
        }

        public DateRange()
        {
            InitializeComponent();
        }

        private void btnYear_Click(object sender, RoutedEventArgs e)
        {
            lblDateHeader_Changed();
            DateChanged_Click(null, null);
        }

        private void btnMonth_Click(object sender, RoutedEventArgs e)
        {
            lblDateHeader_Changed();
            DateChanged_Click(null, null);
        }

        private void btnDay_Click(object sender, RoutedEventArgs e)
        {
            lblDateHeader_Changed();
            DateChanged_Click(null, null);
        }

        private void btnDateForward(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (rbtnYear.IsChecked == true)
            {
                StartDate = StartDate.AddYears(1);
                EndDate = EndDate.AddYears(1);
            }
            else if (rbtnMonth.IsChecked == true)
            {
                StartDate = StartDate.AddMonths(1);
                EndDate = EndDate.AddMonths(1);
            }
            else
            {
                StartDate = StartDate.AddDays(1);
                EndDate = EndDate.AddDays(1);
            }

            lblDateHeader_Changed();
            DateChanged_Click(null, null);
        }

        private void btnDateBack(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (rbtnYear.IsChecked == true)
            {
                StartDate = StartDate.AddYears(-1);
                EndDate = EndDate.AddYears(-1);
            }
            else if (rbtnMonth.IsChecked == true)
            {
                StartDate = StartDate.AddMonths(-1);
                EndDate = EndDate.AddMonths(-1);
            }
            else
            {
                StartDate = StartDate.AddDays(-1);
                EndDate = EndDate.AddDays(-1);
            }

            lblDateHeader_Changed();
            DateChanged_Click(null, null);
        }

        private void lblDateHeader_Changed()
        {
            if (rbtnYear.IsChecked == true)
            {
                StartDate = new DateTime(StartDate.Year, 1, 1);
                EndDate = StartDate.AddYears(1).AddDays(-1);

                if (StartDate.Year == EndDate.Year)
                {
                    lblDateHeader.Content = StartDate.Year.ToString();
                }
                else
                {
                    lblDateHeader.Content = string.Format("{0} - {1}", StartDate.Year.ToString(), EndDate.Year.ToString());
                }
            }
            else if (rbtnMonth.IsChecked == true)
            {
                StartDate = new DateTime(StartDate.Year, StartDate.Month, 1);
                EndDate = StartDate.AddMonths(1).AddDays(-1);

                if (StartDate.Month == EndDate.Month)
                {
                    lblDateHeader.Content = string.Format("{0} - {1}", StartDate.Month.ToString(), EndDate.Year.ToString());
                }
                else
                {
                    lblDateHeader.Content = string.Format("{0} {1} - {2} {3}", StartDate.Month.ToString(), StartDate.Year.ToString(), EndDate.Month.ToString(), EndDate.Year.ToString());
                }
            }
            else
            {
                lblDateHeader.Content = StartDate.Date.ToShortDateString() + " - " + EndDate.Date.ToShortDateString();
            }
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
