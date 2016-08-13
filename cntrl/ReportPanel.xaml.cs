using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cntrl
{
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

        public DateTime StartDate
        {
            get 
            {
                _StartDate = Convert.ToDateTime(_StartDate.Date.ToString() + " 00:00:00");
                return _StartDate; 
            }
            set 
            { 
                _StartDate = value; 
            }
        }
        private DateTime _StartDate = DateTime.Now.AddMonths(-1);

        public DateTime EndDate
        {
            get 
            {
                _EndDate = Convert.ToDateTime(_EndDate.Date.ToString() + " 23:59:59");
                return _EndDate; 
            }
            set { _EndDate = value; }
        }
        private DateTime _EndDate = DateTime.Now;


        public entity.app_branch Branch
        { 
            get
            {
                return (cbBranch.SelectedItem as entity.app_branch);
            }
        }

        public entity.item_tag ItemTag
        {
            get
            {
                return (cbTag.SelectedItem as entity.item_tag);
            }
        }

        public int ProductID
        {
            get
            {
                return sbxItem.ItemID;
            }
        }

        public int SupplierID
        {
            get
            {
                return sbxSupplier.ContactID;
            }
        }

        public int CustomerID
        {
            get
            {
                return sbxCustomer.ContactID;
            }
        }

        public event RoutedEventHandler Update;
        private void Data_Update(object sender, RoutedEventArgs e)
        {
            if (Update != null)
            { Update(this, new RoutedEventArgs()); }
        }

        public ReportPanel()
        {
            ShowBranch = false;
            InitializeComponent();
        }

        private void cbxBranch_Checked(object sender, RoutedEventArgs e)
        {
            cbBranch.ItemsSource = entity.CurrentSession.Get_Branch();
        }

        private void cbxTag_Checked(object sender, RoutedEventArgs e)
        {
            using (entity.db db = new entity.db())
            {
                cbTag.ItemsSource = db.item_tag.Where(x => x.id_company == entity.CurrentSession.Id_Company && x.is_active);
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Data_Update(null, null);
        }
    }
}
