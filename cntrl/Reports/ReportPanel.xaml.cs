using entity;
using entity.BrilloQuery;
using Microsoft.Reporting.WinForms;
using Syncfusion.UI.Xaml.Grid.Converter;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml.Linq;

namespace cntrl
{
	public partial class ReportPanel : UserControl
	{

		private bool RefreshPanel = true;

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
			set { _StartDate = value; RefreshPanel = true; Fill(); /*Filter_Click(null, null);*/ }
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
			set { _EndDate = value; RefreshPanel = true; Fill(); /*Button_Click_1(null, null);*/ }
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
						Label Label = new Label()
						{
							Name = item.ColumnName,
							Content = entity.Brillo.Localize.StringText(item.ColumnName) != string.Empty ? entity.Brillo.Localize.StringText(item.ColumnName) : item.ColumnName,
							Foreground = Brushes.Black
						};

						Style lblStyle = Application.Current.FindResource("input_label") as Style;
						Label.Style = lblStyle;
						stpFilter.Children.Add(Label);

						DataView view = new DataView(ReportDt);
						Style cbxStyle = Application.Current.FindResource("input_combobox") as Style;

						ComboBox ComboBox = new ComboBox()
						{
							SelectedValuePath = item.ColumnName,
							DisplayMemberPath = item.ColumnName,
							Name = "cbx" + item.ColumnName,
							BorderBrush = Brushes.White,
							Foreground = Brushes.Black,
							IsTextSearchEnabled = true,
							IsEditable = true,
							Style = cbxStyle,
							ItemsSource = view.ToTable(true, item.ColumnName).DefaultView
						};
						TextSearch.SetTextPath(ComboBox, item.ColumnName);

						stpFilter.Children.Add(ComboBox);
					}
					else if (item.DataType == typeof(bool))
					{
						Style lblStyle = Application.Current.FindResource("input_label") as Style;

						Label Label = new Label()
						{
							Name = item.ColumnName,
							Content = entity.Brillo.Localize.StringText(item.ColumnName) != string.Empty ? entity.Brillo.Localize.StringText(item.ColumnName) : item.ColumnName,
							Foreground = Brushes.Black,
							Style = lblStyle
						};

						stpFilter.Children.Add(Label);

						Style cbxStyle = Application.Current.FindResource("input_checkbox") as Style;
						DataView view = new DataView(ReportDt);

						CheckBox CheckBox = new CheckBox()
						{
							Style = cbxStyle,
							Name = "cbx" + item.ColumnName,
							Tag = item.ColumnName
						};

						stpFilter.Children.Add(CheckBox);
					}
				}
			}
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

			ReportParameter ParametersCost = new ReportParameter()
			{
				Name = "ParameterCost"
			};

			ParametersCost.Values.Add(CurrentSession.UserRole.see_cost.ToString());

			ReportParameter Parameters = new ReportParameter()
			{
				Name = "Parameters"
			};

			Parameters.Values.Add(_StartDate.ToString() + " - " + _EndDate.ToString());

			reportViewer.LocalReport.SetParameters(new ReportParameter[] { Parameters, ParametersCost });

			reportViewer.Refresh();
			reportViewer.RefreshReport();
		}

		public void Fill()
		{
			reportViewer.Reset();

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

            if (query.Contains("ProjectID") && stpProject.Visibility != Visibility.Visible)
            {
                stpProject.Visibility = Visibility.Visible;
            }

			query = query.Replace("@CompanyID", CurrentSession.Id_Company.ToString());
			query = query.Replace("@ProjectID", ProjectID.ToString());

			query = query.Replace("@StartDate", StartDate.ToString("yyyy-MM-dd") + " 00:00:00");
			query = query.Replace("@EndDate", EndDate.ToString("yyyy-MM-dd") + " 23:59:59");

			dt = QueryExecutor.DT(query);

			if (dt.Rows.Count > 0)
			{
				if (Report.Name.ToLower() == "HumanResource".ToLower())
				{
					dt = dt.Select("id_item_type = 5 or id_item_type = 3 or id_item_type = 7").CopyToDataTable();
				}
				else if (Report.Name.ToLower() == "RawMaterials".ToLower())
				{
					dt = dt.Select("id_item_type = 5 or id_item_type = 1 or id_item_type = 2 or id_item_type = 6").CopyToDataTable();
				}
			}

			ReportDt = dt;
			sfdatagrid.ItemsSource = dt;
			sfPivotTable.ItemSource = dt;

			reportDataSource1.Name = "DataSet1"; //Name of the report dataset in our .RDLC file
			reportDataSource1.Value = dt; //SalesDB.SalesByDate;

			reportViewer.LocalReport.DataSources.Add(reportDataSource1);
			Assembly assembly = Assembly.GetExecutingAssembly();
			Stream reportStream = assembly.GetManifestResourceStream(Report.Path);

			// translate the report
			if (reportStream != null)
			{
				reportStream = RdlcReportHelper.TranslateReport(reportStream);
				reportViewer.LocalReport.LoadReportDefinition(reportStream);
			}

			ReportParameter ParametersCost = new ReportParameter();
			ParametersCost.Name = "ParameterCost";
			ParametersCost.Values.Add(CurrentSession.UserRole.see_cost.ToString());
			ReportParameter Parameters = new ReportParameter();
			Parameters.Name = "Parameters";
			Parameters.Values.Add(_StartDate.ToString() + " - " + _EndDate.ToString());

			reportViewer.LocalReport.SetParameters(new ReportParameter[] { Parameters, ParametersCost });

			reportViewer.Refresh();
			reportViewer.RefreshReport();
		}

		public ReportPanel()
		{
			InitializeComponent();
		}

		private void Load(object sender, RoutedEventArgs e)
		{
			Class.Generate Generate = new Class.Generate();
			Generate.GenerateReportList();

			string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CogntivoERP\\Reports\\" + ApplicationName + "\\";

			foreach (var Report in Generate.ReportList.Where(x => x.Application == ApplicationName).ToList())
			{
				//Check if Report exists in Path.
				string ReportName = Report.Path.Replace("cntrl.Reports.", "");
				ReportName = ReportName.Remove(0, ReportName.IndexOf(".") + 1);
				entity.Brillo.IO.CreateIfNotExists(path + ReportName);

				if (entity.Brillo.IO.FileExists(path) == false)
				{
					using (Stream resource = GetType().Assembly.GetManifestResourceStream(Report.Path))
					{
						if (resource == null)
						{
							throw new ArgumentException("Resource Not Found", "resourceName");
						}
						using (Stream output = File.OpenWrite(path + ReportName))
						{
							resource.CopyTo(output);
						}
					}
				}

				ListBoxItem LBItem = new ListBoxItem()
				{
					Content = entity.Brillo.Localize.StringText(ReportName),
					Tag = path + ReportName
				};

				//Report.Path = path + ReportName;
			}

			ReportViewSource = FindResource("ReportViewSource") as CollectionViewSource;
			ReportViewSource.Source = Generate.ReportList.Where(x => x.Application == ApplicationName).ToList();
		}

		private void Report_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ReportList.SelectedItem != null)
			{
				Fill();
			}
		}

		private void Refresh_Click(object sender, RoutedEventArgs e)
		{
			Fill();
		}

		private void Filter_Click(object sender, RoutedEventArgs e)
		{
			//Fill();
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
							filter += " and ";
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
							filter += " and ";
						}
						filter += CheckBox.Tag + " = True";
						IsFirst = false;
					}
				}
			}
			if (ReportDt != null)
			{
				if (ReportDt.Rows.Count > 0)
				{
					if (ReportDt.Select(filter).Any())
					{
						ReportDt = ReportDt.Select(filter).CopyToDataTable();
					}
				}

				Filter();
			}
		}

		private void Filter_Cancel(object sender, RoutedEventArgs e)
		{
			RefreshPanel = true;
			Fill();
		}

		private void Export_Click(object sender, RoutedEventArgs e)
		{
			if (sfdatagrid.View != null)
			{
				var options = new ExcelExportingOptions()
				{
                    
					AllowOutlining = false
				};
				var excelEngine = sfdatagrid.ExportToExcel(sfdatagrid.View, options);
				var workBook = excelEngine.Excel.Workbooks[0];
				// Add code to show save panel.
				System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog()
				{
					FileName = "Excel", // Default file name
					DefaultExt = ".xlsx", // Default file extension
					Filter = "Text documents (.xlsx)|*.xlsx" // Filter files by extension
				};

				//Show save file dialog box
				System.Windows.Forms.DialogResult result = dlg.ShowDialog();

				//  Process save file dialog box results
				if (result == System.Windows.Forms.DialogResult.OK)
				{
					// Save document
					workBook.SaveAs(dlg.FileName);
				}
			}
		}

		private void Edit_Click(object sender, RoutedEventArgs e)
		{
			Class.Report Report = ReportViewSource.View.CurrentItem as Class.Report;

			Reports.ReportEditor ReportDesigner = new Reports.ReportEditor()
			{
				Path = Report.Path,
				Application = Report.Application
			};

			Window window = new Window
			{
				Title = "Report Designer",
				Content = ReportDesigner
			};

			window.ShowDialog();
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