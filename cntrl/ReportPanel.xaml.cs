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

        public string ReportDescription { get; set; }

        public bool ShowBranch { get; set; }
        public bool ShowTag { get; set; }
        public bool ShowProduct { get; set; }
        public bool ShowSupplier { get; set; }
        public bool ShowCustomer { get; set; }

        public int BranchID 
        { 
            get
            {
                return (Branch.SelectedItem as entity.app_branch).id_branch;
            }
        }

        public int TagID
        {
            get
            {
                return (Tag.SelectedItem as entity.item_tag).id_tag;
            }
        }

        public int ProductID
        {
            get
            {
                return (Tag.SelectedItem as entity.item_tag).id_tag;
            }
        }

        public int SupplierID
        {
            get
            {
                return (Tag.SelectedItem as entity.item_tag).id_tag;
            }
        }

        public int CustomerID
        {
            get
            {
                return (Tag.SelectedItem as entity.item_tag).id_tag;
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
            InitializeComponent();
        }

        private void cbxBranch_Checked(object sender, RoutedEventArgs e)
        {
            cbxBranch.DataContext = entity.CurrentSession.Get_Branch();
        }

        private void cbxTag_Checked(object sender, RoutedEventArgs e)
        {
            using (entity.db db = new entity.db())
            {
                cbxTag.DataContext = db.item_tag.Where(x => x.id_company == entity.CurrentSession.Id_Company && x.is_active);
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (true)
            {
                
            }
        }
    }
}
