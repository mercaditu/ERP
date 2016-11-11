using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace cntrl
{
    public class ReportColumns
    {
        public string Columname { get; set; }
        public bool IsVisibility { get; set; }
    }

    public partial class ReportPanel : UserControl
    {
        public static DependencyProperty ReportTitleProperty = DependencyProperty.Register("ReportTitle", typeof(string), typeof(ReportPanel));
        public string ReportTitle
        {
            get { return (string)GetValue(ReportTitleProperty); }
            set { SetValue(ReportTitleProperty, value); }
        }

        public static DependencyProperty ReportDescriptionProperty = DependencyProperty.Register("ReportDescription", typeof(string), typeof(ReportPanel));
        public string ReportDescription
        {
            get { return (string)GetValue(ReportDescriptionProperty); }
            set { SetValue(ReportDescriptionProperty, value); }
        }

        public DateTime StartDate
        {
            get { return AbsoluteDate.Start(_StartDate); }
            set { _StartDate = value; Data_Update(null, null); }
        }
        private DateTime _StartDate = AbsoluteDate.Start(DateTime.Now.AddMonths(-1));

        public DateTime EndDate
        {
            get { return AbsoluteDate.End(_EndDate); }
            set { _EndDate = value; Data_Update(null, null); }
        }
        private DateTime _EndDate = AbsoluteDate.End(DateTime.Now);

        public CalendarSelectionMode CalendarSelectionMode { get; set; }

        public DataTable ReportDt
        {
            get
            {
                return _ReportDt;
            }
            set
            {
                _ReportDt = value;
                Filterdt = value;

                stpFilter.Children.Clear();
                foreach (DataColumn item in value.Columns)
                {
                    if (item.DataType == typeof(string))
                    {
                        StackPanel stackcolumn = new StackPanel();
                        stackcolumn.Name = "stp" + item.ColumnName;
                        Label desccolumn = new Label();
                        desccolumn.Name = item.ColumnName;
                        desccolumn.Content = item.ColumnName;
                        stpFilter.Children.Add(desccolumn);
                        ComboBox ComboBox = new ComboBox();
                        DataView view = new DataView(value);
                        ComboBox.ItemsSource = view.ToTable(true, item.ColumnName).DefaultView;
                        ComboBox.SelectedValuePath = item.ColumnName;
                        ComboBox.DisplayMemberPath = item.ColumnName;
                        ComboBox.Name = "cbx" + item.ColumnName;
                        ComboBox.SelectionChanged += Cmb_SelectionChanged;
                        ComboBox.IsTextSearchEnabled = true;

                        stpFilter.Children.Add(ComboBox);
                        stpFilter.Children.Add(stackcolumn);
                    }
                }



            }
        }

        public DataTable _ReportDt;

        public DataTable Filterdt
        {
            get
            {
                return _Filterdt;
            }
            set
            {
                _Filterdt = value;

              
                foreach (DataColumn item in value.Columns)
                {

                    if (item.DataType == typeof(System.String))
                    {
                        if (stpFilter.FindName("cbx" + item.ColumnName) !=null)
                        {
                            ComboBox combocolumndata = stpFilter.FindName("cbx" + item.ColumnName) as ComboBox;
                            DataView view = new DataView(value);
                            combocolumndata.ItemsSource = view.ToTable(true, item.ColumnName).DefaultView;
                        }
                    }
                }
            }
        }

        public DataTable _Filterdt;

        public List<ReportColumns> ReportColumn
        {
            get
            {
                return _ReportColumn;
            }
            set
            {
                _ReportColumn = value;
                foreach (ReportColumns ReportColumns in _ReportColumn)
                {
                    CheckBox chkbox = new CheckBox();
                    chkbox.Content = ReportColumns.Columname;
                    chkbox.IsChecked = ReportColumns.IsVisibility;
                    stpColumn.Children.Add(chkbox);
                    chkbox.Checked += CheckBox_Checked;
                    chkbox.Unchecked += CheckBox_Checked;
                }
            }
        }
        List<ReportColumns> _ReportColumn;
        public event RoutedEventHandler Update;
        private void Data_Update(object sender, RoutedEventArgs e)
        {
            Update(this, new RoutedEventArgs());
        }

        public event RoutedEventHandler Filter;
        private void Data_Filter(object sender, RoutedEventArgs e)
        {
            Filter(this, new RoutedEventArgs());
        }

        public ReportPanel()
        {
            InitializeComponent();
        }
        private void Cmb_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ComboBox comboobox = sender as ComboBox;

            string filter = comboobox.DisplayMemberPath + "='" + comboobox.SelectedValue + "'";

            if (ReportDt.Select(filter).CopyToDataTable().Rows.Count > 0)
            {
                Filterdt = ReportDt.Select(filter).CopyToDataTable();
            }
            Data_Filter(null, null);

        }


        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;

            if (chk != null)
            {
                string name = chk.Content.ToString();
                ReportColumns ReportColumns = ReportColumn.Where(x => x.Columname.Contains(name)).FirstOrDefault();

                if (chk.IsChecked == true)
                {
                    ReportColumns.IsVisibility = true;
                }
                else
                {
                    ReportColumns.IsVisibility = false;
                }
            }
            Data_Update(null, null);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Data_Update(null, null);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Data_Update(null, null);
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            var calendar = sender as Calendar;

            StartDate = calendar.SelectedDates.Min();
            EndDate = calendar.SelectedDates.Max();
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
