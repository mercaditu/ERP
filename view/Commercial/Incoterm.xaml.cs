using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using entity;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace Cognitivo.Commercial
{
    /// <summary>
    /// Interaction logic for Incoterm.xaml
    /// </summary>
    public partial class Incoterm : Page
    {
        //entity.dbContext entity = new entity.dbContext();
        IncotermDB dbContext = new IncotermDB();
        CollectionViewSource impex_incotermViewSource = null;

        public Incoterm()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //int module;
            //if (Tag == null)
            //{
            //    module = 0;
            //}
            //else { module = (int)this.Tag; }
            //if (module > 0)
            //{
            //    entity.Properties.Settings _setting = new entity.Properties.Settings();
            //    int user_id = _setting.user_ID;
            //    int id_role = entity.db.security_user.Where(x => x.id_user == user_id).FirstOrDefault().id_role;
            //    security_curd security_curd = entity.db.security_curd.Where(x => x.id_role == id_role && x.id_module == module).FirstOrDefault();
            //    if (security_curd != null)
            //    {
            //        toolBar.canadd = security_curd.can_insert;
            //        toolBar.canedit = security_curd.can_update;
            //        toolBar.candelete = security_curd.can_delete;
            //    }
            //    else
            //    {
            //        toolBar.canadd = true;
            //        toolBar.canedit = true;
            //        toolBar.candelete = true;
            //    }

            //}
            //else
            //{
            //    toolBar.canadd = true;
            //    toolBar.canedit = true;
            //    toolBar.candelete = true;
            //}

            try
            {
                impex_incotermViewSource = this.FindResource("impex_incotermViewSource") as CollectionViewSource;
                dbContext.impex_incoterm.Where(a => a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).Load();
                impex_incotermViewSource.Source = dbContext.impex_incoterm.Local;

                CollectionViewSource incoterm_conditionViewSource = this.FindResource("incoterm_conditionViewSource") as CollectionViewSource;
                incoterm_conditionViewSource.Source = dbContext.impex_incoterm_condition.Where(a => a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).ToList();
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }

        }

        private void toolBar_btnCancel_Click(object sender)
        {
            dbContext.CancelAllChanges();
        }

        private void toolBar_btnDelete_Click(object sender)
        {
            try
            {
                if (MessageBox.Show("Are you sure want to Delete?", "Cognitivo", MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.Yes)
                {
                    dbContext.impex_incoterm.Remove((impex_incoterm)impex_incotermDataGrid.SelectedItem);
                    toolBar_btnSave_Click(sender);
                }
            }
            catch (Exception ex)
            {
                toolBar.msgError(ex);
            }
        }

        private void toolBar_btnNew_Click(object sender)
        {
            impex_incoterm impex_incoterm = new impex_incoterm();
            impex_incoterm.State = EntityState.Added;
            impex_incoterm.IsSelected = true;
            dbContext.Entry(impex_incoterm).State = EntityState.Added;

            impex_incotermViewSource.View.MoveCurrentToLast();
       }

        private void toolBar_btnSave_Click(object sender)
        {
            if (dbContext.SaveChanges() > 0)
            {
                impex_incotermViewSource.View.Refresh();
                toolBar.msgSaved(dbContext.NumberOfRecords);
            }
        }

        private void toolBar_btnEdit_Click(object sender)
        {
            if (impex_incotermDataGrid.SelectedItem != null)
            {
                impex_incoterm impex_incoterm = (impex_incoterm)impex_incotermDataGrid.SelectedItem;
                impex_incoterm.IsSelected = true;
                impex_incoterm.State = EntityState.Modified;
                dbContext.Entry(impex_incoterm).State = EntityState.Modified;
            }
            else
            {
                toolBar.msgWarning("Please Select a Record");
            }
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    impex_incotermViewSource.View.Filter = i =>
                    {
                        impex_incoterm impex_incoterm = i as impex_incoterm;
                        if (impex_incoterm.name.ToLower().Contains(query.ToLower()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    };
                }
                else
                {
                    impex_incotermViewSource.View.Filter = null;
                }
            }
            catch(Exception)
            {

            }
        }
    }
}
