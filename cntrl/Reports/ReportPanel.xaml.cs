using System.Data;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Reporting.WinForms;
using System.Windows.Data;
using entity.BrilloQuery;
using entity;
using System.Linq;
using System.Windows.Media;
using System;
using System.IO;
using System.Xml.Linq;
using cntrl.Properties;
using System.Reflection;

namespace cntrl
{
    public partial class ReportPanel : UserControl
    {
        bool RefreshPanel = true;
        public bool ShowDateRange
        {
            get { return _ShowDateRange; }
            set
            {
                _ShowDateRange = value;
                if (value == true)
                {
                    DateRange.Visibility = Visibility.Visible;
                }
            }
        }
        private bool _ShowDateRange;

        public bool ShowProject
        {
            get { return _ShowProject; }
            set
            {
                if (_ShowProject != value)
                {
                    _ShowProject = value;
                    if (value == true)
                    {
                        stpProject.Visibility = Visibility.Visible;
                        using (db db = new db())
                        {
                            ComboProject.ItemsSource = db.projects.Where(x => x.id_company == CurrentSession.Id_Company).ToList();
                        }
                    }
                }

            }
        }
        private bool _ShowProject;

        private static readonly DependencyProperty ApplicationNameProperty = DependencyProperty.Register("ApplicationName", typeof(App.Names), typeof(ReportPanel));
        public App.Names ApplicationName
        {
            get { return (App.Names)GetValue(ApplicationNameProperty); }
            set { SetValue(ApplicationNameProperty, value); }
        }

        private CollectionViewSource ReportViewSource;
        public DateTime StartDate
        {
            get { return AbsoluteDate.Start(_StartDate); }
            set { _StartDate = value; RefreshPanel = true; Fill(); Button_Click_1(null, null); }
        }
        private DateTime _StartDate = AbsoluteDate.Start(DateTime.Now.AddMonths(-1));

        public int ProjectID
        {
            get { return _ProjectID; }
            set
            {
                _ProjectID = value;
                Fill();
            }
        }
        private int _ProjectID;

        public DateTime EndDate
        {
            get { return AbsoluteDate.End(_EndDate); }
            set { _EndDate = value; RefreshPanel = true; Fill(); Button_Click_1(null, null); }
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

                ClearFilter();
            }
        }

        public DataTable _ReportDt;

        public void ClearFilter()
        {
            if (RefreshPanel)
            {
                stpFilter.Children.Clear();
                foreach (DataColumn item in ReportDt.Columns)
                {
                    if (item.DataType == typeof(string))
                    {
                        Label Label = new Label();
                        Label.Name = item.ColumnName;
                        Label.Content = entity.Brillo.Localize.StringText(item.ColumnName) != string.Empty ? entity.Brillo.Localize.StringText(item.ColumnName) : item.ColumnName;
                        Label.Foreground = Brushes.Black;
                        Style lblStyle = Application.Current.FindResource("input_label") as Style;
                        Label.Style = lblStyle;
                        stpFilter.Children.Add(Label);

                        ComboBox ComboBox = new ComboBox();
                        Style cbxStyle = Application.Current.FindResource("input_combobox") as Style;
                        ComboBox.Style = cbxStyle;
                        DataView view = new DataView(ReportDt);
                        ComboBox.ItemsSource = view.ToTable(true, item.ColumnName).DefaultView;
                        ComboBox.SelectedValuePath = item.ColumnName;
                        ComboBox.DisplayMemberPath = item.ColumnName;
                        ComboBox.Name = "cbx" + item.ColumnName;
                        ComboBox.BorderBrush = Brushes.White;
                        ComboBox.Foreground = Brushes.Black;
                        ComboBox.IsTextSearchEnabled = true;
                        TextSearch.SetTextPath(ComboBox, item.ColumnName);
                        ComboBox.IsEditable = true;
                        stpFilter.Children.Add(ComboBox);
                    }
                    else if (item.DataType == typeof(bool))
                    {
                        Label Label = new Label();
                        Label.Name = item.ColumnName;
                        Label.Content = entity.Brillo.Localize.StringText(item.ColumnName) != string.Empty ? entity.Brillo.Localize.StringText(item.ColumnName) : item.ColumnName;
                        Label.Foreground = Brushes.Black;
                        Style lblStyle = Application.Current.FindResource("input_label") as Style;
                        Label.Style = lblStyle;
                        stpFilter.Children.Add(Label);

                        CheckBox CheckBox = new CheckBox();
                        Style cbxStyle = Application.Current.FindResource("input_checkbox") as Style;
                        CheckBox.Style = cbxStyle;
                        DataView view = new DataView(ReportDt);
                        CheckBox.Name = "cbx" + item.ColumnName;
                        CheckBox.Tag = item.ColumnName;
                        stpFilter.Children.Add(CheckBox);
                    }
                }
            }

        }

        public void Fill()
        {
            this.reportViewer.Reset();

            ReportDataSource reportDataSource1 = new ReportDataSource();
            Class.Report Report = ReportViewSource.View.CurrentItem as Class.Report;

            if (Report.Parameters.Where(x => x == Class.Report.Types.Project).Count() > 0)
            {
                ShowProject = true;
            }
            if (Report.Parameters.Where(x => x == Class.Report.Types.StartDate || x == Class.Report.Types.EndDate).Count() > 0)
            {
                ShowDateRange = true;
            }

            DataTable dt = new DataTable();

            string query = Report.Query;
            if (Report.ReplaceString != null && Report.ReplaceWithString != null)
            {
                query = query.Replace(Report.ReplaceString, Report.ReplaceWithString);
            }

            query = query.Replace("@CompanyID", CurrentSession.Id_Company.ToString());
            query = query.Replace("@StartDate", StartDate.ToString("yyyy-MM-dd"));
            query = query.Replace("@EndDate", EndDate.AddDays(1).ToString("yyyy-MM-dd"));
            query = query.Replace("@ProjectID", ProjectID.ToString());
            dt = QueryExecutor.DT(query);

            ReportDt = dt;

            reportDataSource1.Name = "DataSet1"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = dt; //SalesDB.SalesByDate;
            //dgvData.ItemsSource = dt; //DataGrid (BETA).

            reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream reportStream = assembly.GetManifestResourceStream(Report.Path);
            // translate the report
            reportStream = RdlcReportHelper.TranslateReport(reportStream);

            reportViewer.LocalReport.LoadReportDefinition(reportStream);


       

            //if (Report.Path=="cntrl.Reports.Productions.ProductionStatus.rdlc")
            //{
            //    ReportParameter Parameters = new ReportParameter("Parameters", _StartDate.ToString() + _EndDate.ToString());
            //    ReportParameter ParametersCost = new ReportParameter("ParametersCost", CurrentSession.UserRole.see_cost.ToString());
            //    reportViewer.LocalReport.SetParameters(new ReportParameter[] { Parameters, ParametersCost });
            //}
            //else
            //{
                ReportParameter Parameters = new ReportParameter("Parameters", _StartDate.ToString() + _EndDate.ToString());
              
                reportViewer.LocalReport.SetParameters(new ReportParameter[] { Parameters});
          //  }

         


            reportViewer.Refresh();
            reportViewer.RefreshReport();
        }
       
        public void Filter()
        {
            this.reportViewer.Reset();

            ReportDataSource reportDataSource1 = new ReportDataSource();
            Class.Report Report = ReportViewSource.View.CurrentItem as Class.Report;

            reportDataSource1.Name = "DataSet1"; //Name of the report dataset in our .RDLC file
            reportDataSource1.Value = ReportDt; //SalesDB.SalesByDate;
            reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream reportStream = assembly.GetManifestResourceStream(Report.Path);
            // translate the report
            reportStream = RdlcReportHelper.TranslateReport(reportStream);

            reportViewer.LocalReport.LoadReportDefinition(reportStream);

            //if (Report.Path == "cntrl.Reports.Productions.ProductionStatus.rdlc")
            //{
            //    ReportParameter Parameters = new ReportParameter("Parameters", _StartDate.ToString() + _EndDate.ToString());
            //    ReportParameter ParametersCost = new ReportParameter("ParametersCost", CurrentSession.UserRole.see_cost.ToString());
            //    reportViewer.LocalReport.SetParameters(new ReportParameter[] { Parameters, ParametersCost });
            //}
            //else
            //{
                ReportParameter Parameters = new ReportParameter("Parameters", _StartDate.ToString() + _EndDate.ToString());

                reportViewer.LocalReport.SetParameters(new ReportParameter[] { Parameters });
           // }

            reportViewer.Refresh();
            reportViewer.RefreshReport();
        }

        public ReportPanel()
        {
            InitializeComponent();
        }

        private void this_Loaded(object sender, RoutedEventArgs e)
        {
            Class.Generate Generate = new Class.Generate();
            Generate.GenerateReportList();
            ReportViewSource = (CollectionViewSource)FindResource("ReportViewSource");
            ReportViewSource.Source = Generate.ReportList.Where(x => x.Application == ApplicationName).ToList();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Fill();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Fill();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            RefreshPanel = false;
            string filter = "";

            bool IsFirst = true;

            foreach (object Control in stpFilter.Children)
            {


                if (Control.GetType() == typeof(ComboBox))
                {

                    ComboBox comboobox = Control as ComboBox;
                    if (comboobox.SelectedValue != null)
                    {
                        if (IsFirst == false)
                        {
                            filter += " or ";
                        }
                        filter += comboobox.DisplayMemberPath + "='" + comboobox.SelectedValue + "'";
                        IsFirst = false;
                    }


                }
                else if (Control.GetType() == typeof(CheckBox))
                {

                    CheckBox CheckBox = Control as CheckBox;

                    if (CheckBox.IsChecked == true)
                    {
                        if (IsFirst == false)
                        {
                            filter += " or ";
                        }
                        filter += CheckBox.Tag + " = True";
                        IsFirst = false;
                    }

                }
            }

            if (ReportDt.Rows.Count > 0)
            {
                if (ReportDt.Select(filter).Any())
                {
                    ReportDt = ReportDt.Select(filter).CopyToDataTable();
                }
            }

            Filter();
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            RefreshPanel = true;

            Fill();
          
           
        }
    }

    public static class RdlcReportHelper
    {
        public static Stream TranslateReport(Stream reportStream)
        {
            XDocument reportXml = XDocument.Load(reportStream);

            foreach (var element in reportXml.Descendants(XName.Get("Value", @"http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")))
            {
                XAttribute attribute = element.Attribute(XName.Get("LocID", @"http://schemas.microsoft.com/SQLServer/reporting/reportdesigner"));

                if (attribute != null)
                {
                    string translatedValue = entity.Brillo.Localize.StringText(attribute.Value);
                    element.Value = string.IsNullOrEmpty(translatedValue) ? element.Value : translatedValue;
                }
            }

            Stream ms = new MemoryStream();
            reportXml.Save(ms, SaveOptions.OmitDuplicateNamespaces);
            ms.Position = 0;

            return ms;
        }
    }
}
