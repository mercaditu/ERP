using entity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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

namespace cntrl.Curd
{
    /// <summary>
    /// Interaction logic for Accounting_template.xaml
    /// </summary>
    public partial class Accounting_template : UserControl
    {
        CollectionViewSource _accounting_templateViewSource = null;
        public CollectionViewSource accounting_templateViewSource { get { return _accounting_templateViewSource; } set { _accounting_templateViewSource = value; } }
        CollectionViewSource _accounting_templatedetailViewSource = null;
        public CollectionViewSource accounting_templatedetailViewSource { get { return _accounting_templatedetailViewSource; } set { _accounting_templatedetailViewSource = value; } }

        private entity.dbContext _entity = null;
        public entity.dbContext entity { get { return _entity; } set { _entity = value; } }

        entity.Properties.Settings _setting = new entity.Properties.Settings();
        public Accounting_template()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            stackMainAc.DataContext = accounting_templateViewSource;
            stpsub.DataContext = accounting_templatedetailViewSource;
            CollectionViewSource accounting_chartViewSource = ((CollectionViewSource)(FindResource("accounting_chartViewSource")));
            accounting_chartViewSource.Source = entity.db.accounting_chart.Where(x=>x.id_company==_setting.company_ID).OrderBy(b => b.name).ToList();

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                accounting_template accounting_template = accounting_templateViewSource.View.CurrentItem as accounting_template;
                if (accounting_template.accounting_template_detail.Sum(x=>x.coefficeint)==100)
                {
                    IEnumerable<DbEntityValidationResult> validationresult = entity.db.GetValidationErrors();
                    if (validationresult.Count() == 0)
                    {
                        entity.db.SaveChanges();
                        Grid parentGrid = (Grid)this.Parent;
                        parentGrid.Children.Clear();
                        parentGrid.Visibility = System.Windows.Visibility.Hidden;
                    }
                    
                }
                else
                {
                    MessageBox.Show("CoEfficient is Equal to 100%");
                }
               
            }
            catch (Exception ex)
            { throw ex; }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                entity.accounting_template objAccount = accounting_templateViewSource.View.CurrentItem as entity.accounting_template;
                objAccount.is_active = false;
                btnSave_Click(sender, e);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                accounting_template_detailDataGrid.CancelEdit();
                // entity.CancelChanges();
                accounting_templateViewSource.View.Refresh();
                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception ex)
            { throw ex; }
        }

        //private void cbxItem_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter && combo.Data != null)
        //    {
        //        int id = Convert.ToInt32(((accounting_chart)cbxChart.Data).id_chart);
        //        if (id > 0)
        //        {
        //           accounting_template_detail accounting_template_detail= new accounting_template_detail();
        //            accounting_template_detail.id_chart=id;
        //            _entity.db.accounting_template_detail.Add(accounting_template_detail);
        //            }
        //        }
            
        //}

        //private void cbxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{

        //}
    }
}
