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
using System.Data.Entity;
using System.Data.Entity.Validation;
using entity;

namespace cntrl.Curd
{
    /// <summary>
    /// Interaction logic for user_role.xaml
    /// </summary>
    public partial class user_role : UserControl
    {
        entity.dbContext entity = new entity.dbContext();
        CollectionViewSource security_roleViewSource = null;
       /// entity.Properties.Settings _entity = new entity.Properties.Settings();

        public user_role()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
             if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
             {
                 security_roleViewSource = this.FindResource("security_roleViewSource") as CollectionViewSource;
                 entity.db.security_role.Where(a=>a.is_active == true && a.id_company == CurrentSession.Id_Company).Load();
                 security_roleViewSource.Source = entity.db.security_role.Local;
                 addNew();
             }
        }
        private void addNew()
        {
            entity.security_role security_role = new entity.security_role();
            entity.db.security_role.Add(security_role);
            security_roleViewSource.View.MoveCurrentToLast();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //entity.CancelChanges();
            Grid parentGrid = (Grid)this.Parent;
            parentGrid.Children.Clear();
            parentGrid.Visibility = System.Windows.Visibility.Hidden;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IEnumerable<DbEntityValidationResult> validationresult = entity.db.GetValidationErrors();
                if (validationresult.Count() == 0)
                {
                    entity.SaveChanges();
                    btnCancel_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
