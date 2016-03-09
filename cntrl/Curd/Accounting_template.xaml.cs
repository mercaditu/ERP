using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using cntrl;
using entity;
using System.Data.Entity.Validation;

namespace cntrl.Curd
{
    /// <summary>
    /// Interaction logic for Accounting_template.xaml
    /// </summary>
    public partial class Accounting_template : UserControl
    {
        CollectionViewSource accounting_templateViewSource = null;
        CollectionViewSource accounting_templateaccounting_template_detailViewSource = null;

        private dbContext entity = new dbContext();
       // public entity.dbContext entity { get { return _entity; } set { _entity = value; } }

        private Class.clsCommon.Mode _operationMode = 0;
        public Class.clsCommon.Mode operationMode { get { return _operationMode; } set { _operationMode = value; } }


        private entity.accounting_template _accounting_templateobject = null;
        public entity.accounting_template accounting_templateobject { get { return _accounting_templateobject; } set { _accounting_templateobject = value; } }



        public Accounting_template()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            accounting_templateViewSource = ((CollectionViewSource)(FindResource("accounting_templateViewSource")));
            entity.db.accounting_template.Where(a => a.id_company == CurrentSession.Id_Company && a.is_active == true).Include("accounting_template_detail").OrderBy(a => a.name).Load();
            accounting_templateViewSource.Source = entity.db.accounting_template.Local;
           
            CollectionViewSource accounting_chartViewSource = ((CollectionViewSource)(FindResource("accounting_chartViewSource")));
            accounting_chartViewSource.Source = entity.db.accounting_chart.Where(x => x.id_company == CurrentSession.Id_Company).OrderBy(b => b.name).ToList();

            if (operationMode == Class.clsCommon.Mode.Add)
            {
                accounting_template accounting_template = new accounting_template();
                accounting_template.name = "Template";
                accounting_template.is_active = true;
                entity.db.accounting_template.Add(accounting_template);
                //  entity.db.SaveChanges();

                accounting_templateViewSource.View.Refresh();
                accounting_templateViewSource.View.MoveCurrentToLast();
            }
            else
            {
                accounting_templateViewSource.View.MoveCurrentTo(entity.db.accounting_template.Where(x => x.id_template == accounting_templateobject.id_template).FirstOrDefault());
                btnDelete.Visibility = System.Windows.Visibility.Visible;
            }
            //stackMainAc.DataContext = accounting_templateViewSource;
            //stpsub.DataContext = accounting_templateaccounting_template_detailViewSource;
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
                entity.CancelChanges();
              
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
