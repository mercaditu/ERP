using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace cntrl.Curd
{
    /// <summary>
    /// Interaction logic for contact_role.xaml
    /// </summary>
    public partial class Contact_Role : UserControl
    {
        private CollectionViewSource _objCollectionViewSource = null;
        public CollectionViewSource objCollectionViewSource { get { return _objCollectionViewSource; } set { _objCollectionViewSource = value; } }

        private entity.dbContext _entity = null;
        public entity.dbContext entity { get { return _entity; } set { _entity = value; } }

        public Contact_Role()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                stackFields.DataContext = objCollectionViewSource;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            entity.CancelChanges();
            objCollectionViewSource.View.Refresh();

            Grid parentGrid = Parent as Grid;
            if (parentGrid != null)
            {
                parentGrid.Children.Clear();
                parentGrid.Visibility = Visibility.Hidden;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _entity.SaveChanges();
            Grid parentGrid = Parent as Grid;
            if (parentGrid != null)
            {
                parentGrid.Children.Clear();
                parentGrid.Visibility = Visibility.Hidden;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                entity.contact_role contact_role = objCollectionViewSource.View.CurrentItem as entity.contact_role;
                contact_role.is_active = false;
                btnSave_Click(sender, e);
            }
        }
    }
}