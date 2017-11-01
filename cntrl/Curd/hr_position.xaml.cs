using entity;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace cntrl
{
    public partial class hr_position : UserControl
    {
        private CollectionViewSource _objCollectionViewSource = null;
        public CollectionViewSource objCollectionViewSource { get { return _objCollectionViewSource; } set { _objCollectionViewSource = value; } }

        public dbContext Entity { get; set; }

        private entity.hr_position _hr_positionobj = null;
        public entity.hr_position hr_positionobject { get => _hr_positionobj; set => _hr_positionobj = value; }

        private Class.clsCommon.Mode _operationMode = 0;
        public Class.clsCommon.Mode OperationMode { get => _operationMode; set => _operationMode = value; }

        public hr_position()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                CollectionViewSource app_departmentViewSource = (CollectionViewSource)FindResource("app_departmentViewSource");
                Entity.db.app_department.Where(a => a.id_company == CurrentSession.Id_Company).Load();
                app_departmentViewSource.Source = Entity.db.app_department.Local;

                Entity.db.hr_position.Where(a => a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).Load();
                objCollectionViewSource.Source = Entity.db.hr_position.Local;

                if (OperationMode == Class.clsCommon.Mode.Add)
                {
                    entity.hr_position hr_position = new entity.hr_position();
                    Entity.db.hr_position.Add(hr_position);

                    objCollectionViewSource.View.Refresh();
                    objCollectionViewSource.View.MoveCurrentToLast();
                }
                else
                {
                    objCollectionViewSource.View.MoveCurrentTo(Entity.db.hr_position.Where(x => x.id_position == hr_positionobject.id_position).FirstOrDefault());
                }
                stackFields.DataContext = objCollectionViewSource;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Entity.CancelChanges();
                objCollectionViewSource.View.Refresh();
                Grid parentGrid = (Grid)this.Parent;
                parentGrid.Children.Clear();
                parentGrid.Visibility = Visibility.Hidden;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<DbEntityValidationResult> validationresult = Entity.db.GetValidationErrors();
                if (validationresult.Count() == 0)
                {
                    Entity.db.SaveChanges();
                    btnCancel_Click(sender, e);
                }
        }

        private void sbxContact_Select(object sender, RoutedEventArgs e)
        {
            entity.hr_position hr_position = objCollectionViewSource.View.CurrentItem as entity.hr_position;
            if (sbxContact.ContactID > 0)
            {
                if (hr_position != null)
                {
                    contact contact = Entity.db.contacts.Where(x => x.id_contact == sbxContact.ContactID).FirstOrDefault();
                    hr_position.id_contact = contact.id_contact;
                    hr_position.contact = contact;
                }
            }
        }
    }
}