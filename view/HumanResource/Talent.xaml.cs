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
using entity;
using System.Data.Entity;

namespace Cognitivo.HumanResource
{
    /// <summary>
    /// Interaction logic for talent.xaml
    /// </summary>
    public partial class Talent : Page
    {
       
        //entity.dbContext dbContext = new entity.dbContext();
        entity.hr_TalentDB dbContext = new entity.hr_TalentDB();
       CollectionViewSource hr_talentViewSource;
        public Talent()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            hr_talentViewSource = (CollectionViewSource)this.FindResource("hr_talentViewSource");

            dbContext.hr_talent.Where(a => a.is_active == true
            && a.id_company == entity.Properties.Settings.Default.company_ID).OrderBy(a => a.name).Load();
            hr_talentViewSource.Source = dbContext.hr_talent.Local;


          
        }

        private void toolBar_btnSearch_Click(object sender, string query)
        {
            if (!string.IsNullOrEmpty(query) && hr_talentViewSource != null)
            {
                    hr_talentViewSource.View.Filter = i =>
                    {
                        hr_talent hr_talent = i as hr_talent;

                        if (hr_talent != null)
                        {
                            //Protect the code against null values.
                          
                            string customer = hr_talent.name;

                            if ((customer.ToLower().Contains(query.ToLower()))
                                )
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }

                        }
                        else
                        {
                            return false;
                        }
                    };
            }
            else
            {
                hr_talentViewSource.View.Filter = null;
            }         
        }

        private void Save_Click(object sender)
        {
            try
            {
                dbContext.SaveChanges();
            }
            catch
            {
                //toolBar.msgError(ex);
            }
        }

        private void toolBar_btnDelete_Click(object sender)
        {

        }

        private void toolBar_btnCancel_Click(object sender)
        {
           
            dbContext.CancelAllChanges();
            hr_talent hr_talent = (hr_talent)hr_talentDataGrid.SelectedItem;
            hr_talent.State = EntityState.Unchanged;
        }

        private void New_Click(object sender)
        {
            hr_talent hr_talent = new hr_talent();
            hr_talent.State = EntityState.Added;
            hr_talent.IsSelected = true;
            dbContext.Entry(hr_talent).State = EntityState.Added;
            hr_talentViewSource.View.MoveCurrentToLast();
        }
        private void toolBar_btnEdit_Click(object sender)
       {
            // add_MissingRecords();
           if (hr_talentDataGrid.SelectedItem != null)
            {
                hr_talent hr_talent = (hr_talent)hr_talentDataGrid.SelectedItem;
                hr_talent.IsSelected = true;
                hr_talent.State = EntityState.Modified;
                dbContext.Entry(hr_talent).State = EntityState.Modified;
            }
            else
            {
                //toolBar.msgWarning("Please Select a record");
            }

        }
                
      
    }
}

