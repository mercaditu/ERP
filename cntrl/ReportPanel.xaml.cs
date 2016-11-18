using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Media;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Reporting.WinForms;
using System.Windows.Data;
using entity.BrilloQuery;

namespace cntrl
{
    public class ReportColumns
    {
        public string Columname { get; set; }
        public bool IsVisibility { get; set; }
    }

    public partial class ReportPanel : UserControl
    {
        CollectionViewSource ReportViewSource;


        public DateTime StartDate
        {
            get { return AbsoluteDate.Start(_StartDate); }
            set { _StartDate = value; }
        }
        private DateTime _StartDate = AbsoluteDate.Start(DateTime.Now.AddMonths(-1));

        public DateTime EndDate
        {
            get { return AbsoluteDate.End(_EndDate); }
            set { _EndDate = value; }
        }
        private DateTime _EndDate = AbsoluteDate.End(DateTime.Now);

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
                        Label Label = new Label();
                        Label.Name = item.ColumnName;
                        Label.Content = item.ColumnName;
                        Label.Foreground = Brushes.White;

                        stpFilter.Children.Add(Label);
                        ComboBox ComboBox = new ComboBox();
                        DataView view = new DataView(value);
                        ComboBox.ItemsSource = view.ToTable(true, item.ColumnName).DefaultView;
                        ComboBox.SelectedValuePath = item.ColumnName;
                        ComboBox.DisplayMemberPath = item.ColumnName;
                        ComboBox.Name = "cbx" + item.ColumnName;
                        ComboBox.SelectionChanged += Cmb_SelectionChanged;
                        //    ComboBox.Background.Opacity = 16;
                        ComboBox.BorderBrush = Brushes.Transparent;
                        ComboBox.Foreground = Brushes.White;
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
                    if (item.DataType == typeof(string))
                    {
                        if (stpFilter.FindName("cbx" + item.ColumnName) != null)
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

        public void Fill()
        {
            this.reportViewer.Reset();
            Class.Report Report = ReportViewSource.View.CurrentItem as Class.Report;
            ReportDataSource reportDataSource1 = new ReportDataSource();


            DataTable dt = new DataTable();

            string query = System.IO.File.ReadAllText(@Report.QueryPath);
            dt = QueryExecutor.DT(query);


            reportViewer.LocalReport.SetParameters(new ReportParameter("ShowVAT", true.ToString()));

            reportViewer.LocalReport.SetParameters(new ReportParameter("ShowProfit", true.ToString()));

            reportViewer.LocalReport.SetParameters(new ReportParameter("ShowMarkup", true.ToString()));

            reportViewer.LocalReport.SetParameters(new ReportParameter("ShowMargin", true.ToString()));

            reportViewer.LocalReport.SetParameters(new ReportParameter("ShowCost", true.ToString()));


            reportViewer.LocalReport.SetParameters(new ReportParameter("ShowVATTotal", true.ToString()));


            reportDataSource1.Name = "SalesInvoiceSummary"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = dt; //SalesDB.SalesByDate;

            reportViewer.LocalReport.DataSources.Add(reportDataSource1);

            reportViewer.LocalReport.ReportEmbeddedResource = Report.Path;



            reportViewer.Refresh();
            reportViewer.RefreshReport();
        }

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

        private void DateRange_DateChanged(object sender, RoutedEventArgs e)
        {
            Data_Update(null, null);
        }

        private void this_Loaded(object sender, RoutedEventArgs e)
        {
            Class.Generate Generate = new Class.Generate();
            Generate.GenerateReportList();
            ReportViewSource = (CollectionViewSource)FindResource("ReportViewSource");
            ReportViewSource.Source = Generate.ReportList;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Fill();
        }
    }
}
