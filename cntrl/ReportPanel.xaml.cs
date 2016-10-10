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


        public static DependencyProperty ShowBranchProperty = DependencyProperty.Register("ShowBranch", typeof(bool), typeof(ReportPanel));
        public bool ShowBranch
        {
            get { return (bool)GetValue(ShowBranchProperty); }
            set { SetValue(ShowBranchProperty, value); }
        }

        public static DependencyProperty ShowTagProperty = DependencyProperty.Register("ShowTag", typeof(bool), typeof(ReportPanel));
        public bool ShowTag
        {
            get { return (bool)GetValue(ShowTagProperty); }
            set { SetValue(ShowTagProperty, value); }
        }

        public static DependencyProperty ShowProductProperty = DependencyProperty.Register("ShowProduct", typeof(bool), typeof(ReportPanel));
        public bool ShowProduct
        {
            get { return (bool)GetValue(ShowProductProperty); }
            set { SetValue(ShowProductProperty, value); }
        }

        public static DependencyProperty ShowSupplierProperty = DependencyProperty.Register("ShowSupplier", typeof(bool), typeof(ReportPanel));
        public bool ShowSupplier
        {
            get { return (bool)GetValue(ShowSupplierProperty); }
            set { SetValue(ShowSupplierProperty, value); }
        }

        public static DependencyProperty ShowCustomerProperty = DependencyProperty.Register("ShowCustomer", typeof(bool), typeof(ReportPanel));
        public bool ShowCustomer
        {
            get { return (bool)GetValue(ShowCustomerProperty); }
            set { SetValue(ShowCustomerProperty, value); }
        }

        public static DependencyProperty ShowProjectProperty = DependencyProperty.Register("ShowProject", typeof(bool), typeof(ReportPanel));
        public bool ShowProject
        {
            get { return (bool)GetValue(ShowProjectProperty); }
            set { SetValue(ShowProjectProperty, value); }
        }

        public static DependencyProperty ShowCurrencyProperty = DependencyProperty.Register("ShowCurrency", typeof(bool), typeof(ReportPanel));
        public bool ShowCurrency
        {
            get { return (bool)GetValue(ShowCurrencyProperty); }
            set { SetValue(ShowCurrencyProperty, value); }
        }

        public static DependencyProperty ShowDatesProperty = DependencyProperty.Register("ShowDates", typeof(bool), typeof(ReportPanel));
        public bool ShowDates
        {
            get { return (bool)GetValue(ShowDatesProperty); }
            set { SetValue(ShowDatesProperty, value); }
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

        //public entity.app_branch Branch
        //{
        //    get
        //    {
        //        if ((bool)cbxBranch.IsChecked)
        //        {
        //            return (cbBranch.SelectedItem as entity.app_branch);
        //        }
        //        return null;
        //    }
        //}

        //public entity.item_tag ItemTag
        //{
        //    get
        //    {
        //        return (cbTag.SelectedItem as entity.item_tag);
        //    }
        //}

        public int ProductID
        {
            get
            {
                if ((bool)cbxProduct.IsChecked)
                {
                    return sbxItem.ItemID;
                }
                return 0;
            }
        }

        //public int SupplierID
        //{
        //    get
        //    {
        //        if ((bool)cbxSupplier.IsChecked)
        //        {
        //            return sbxSupplier.ContactID;
        //        }
        //        return 0;
        //    }
        //}

        //public int CustomerID
        //{
        //    get
        //    {
        //        if ((bool)cbxCustomer.IsChecked)
        //        {
        //            return sbxCustomer.ContactID;
        //        }
        //        return 0;
        //    }
        //}

        //public entity.app_currency Currency
        //{
        //    get
        //    {
        //        return (cbCurrency.SelectedItem as entity.app_currency);
        //    }
        //}
        public DataTable ReportDt
        {
            get
            {

                return _ReportDt;


            }
            set
            {
             

                if (_ReportDt == null)
                {
                

                    stpColumn.Children.Clear();
                    foreach (DataColumn item in value.Columns)
                    {

                        if (item.DataType == typeof(System.String))
                        {

                            StackPanel stackcolumn = new StackPanel();
                            stackcolumn.Name = "stp" + item.ColumnName;
                            Label desccolumn = new Label();
                            desccolumn.Name = item.ColumnName;
                            desccolumn.Content = item.ColumnName;
                            stackcolumn.Children.Add(desccolumn);
                            ComboBox combocolumndata = new ComboBox();
                            DataView view = new DataView(value);
                            combocolumndata.ItemsSource = view.ToTable(true, item.ColumnName).DefaultView;
                            combocolumndata.SelectedValuePath = item.ColumnName;
                            combocolumndata.DisplayMemberPath = item.ColumnName;
                            combocolumndata.Name = "cbx" + item.ColumnName;
                            combocolumndata.SelectionChanged += Cmb_SelectionChanged;
                            stackcolumn.Children.Add(combocolumndata);
                            stpColumn.Children.Add(stackcolumn);


                        }
                    }
                }

                _ReportDt = value;
                Filterdt = value;
            }
        }

        public DataTable _ReportDt;

        public DataTable Filterdt { get; set; }
        
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

        public ReportPanel()
        {
            InitializeComponent();
        }
        private void Cmb_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ComboBox comboobox = sender as ComboBox;
            string filter = comboobox.DisplayMemberPath + "='" + comboobox.SelectedValue + "'";
            Filterdt = ReportDt.Select(filter).CopyToDataTable();
            Data_Update(null, null);
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

        //private void cbxBranch_Checked(object sender, RoutedEventArgs e)
        //{
        //    cbBranch.ItemsSource = entity.CurrentSession.Get_Branch();
        //}

        //private void cbxTag_Checked(object sender, RoutedEventArgs e)
        //{
        //    using (entity.db db = new entity.db())
        //    {
        //        cbTag.ItemsSource = db.item_tag.Where(x => x.id_company == entity.CurrentSession.Id_Company && x.is_active).ToList();
        //    }
        //}

        private void cbxProject_Checked(object sender, RoutedEventArgs e)
        {
            using (entity.db db = new entity.db())
            {
                cbProject.ItemsSource = db.projects.Where(x => x.id_company == entity.CurrentSession.Id_Company).ToList();
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Data_Update(null, null);
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
