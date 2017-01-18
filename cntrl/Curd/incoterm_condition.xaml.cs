using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace cntrl.Curd
{
    /// <summary>
    /// Interaction logic for incoterm_condition.xaml
    /// </summary>
    public partial class incoterm_condition : UserControl
    {
        CollectionViewSource _conditionViewSource = null;
        public CollectionViewSource conditionViewSource { get { return _conditionViewSource; } set { _conditionViewSource = value; } }

        private entity.dbContext _entity = null;
        public entity.dbContext entity { get { return _entity; } set { _entity = value; } }

        public incoterm_condition()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                stackFields.DataContext = conditionViewSource;
                cbxConditionTypes.ItemsSource = Enum.GetValues(typeof(entity.impex_incoterm_condition.incoterm_Types)).OfType<entity.impex_incoterm_condition.incoterm_Types>().ToList();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Save
            try
            {
                IEnumerable<DbEntityValidationResult> validationresult = entity.db.GetValidationErrors();
                if (validationresult.Count() == 0)
                {
                    entity.db.SaveChanges();
                    Image_MouseDown(sender, e);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Image_MouseDown(object sender, RoutedEventArgs e)
        {
            //Cancel
            try
            {
                entity.CancelChanges();
                conditionViewSource.View.Refresh();
                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
