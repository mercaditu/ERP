using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using entity;
using System.Data.Entity;
using System.Collections.Generic;

namespace cntrl.Curd
{
    public partial class contact : UserControl
    {
        public entity.dbContext entity { get; set; }

        private entity.contact _contactobject = null;
        public entity.contact contactobject { get { return _contactobject; } set { _contactobject = value; } }
        public List<entity.contact> contactList { get; set; }
        CollectionViewSource contactViewSource;
        public contact()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {


                if (entity != null)
                {
                    contactList = new List<global::entity.contact>();
                    cbPriceList.ItemsSource = entity.db.item_price_list.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).AsNoTracking().ToList();

                    cbCostCenter.ItemsSource = entity.db.app_cost_center.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).AsNoTracking().ToList();

                    cbxRole.ItemsSource = entity.db.contact_role.Where(a => a.id_company == CurrentSession.Id_Company && a.is_active == true).OrderBy(a => a.name).AsNoTracking().ToList();

                    if (contactobject.id_contact == 0)
                    {
                        entity.db.contacts.Add(contactobject);
                    }

                    contactViewSource = (CollectionViewSource)this.FindResource("contactViewSource");
                    entity.db.contacts.Load();

                    contactViewSource.Source = entity.db.contacts.Local;
              
                    contactViewSource.View.MoveCurrentTo(contactobject);

                }

            }
        }

        public event btnSave_ClickedEventHandler btnSave_Click;
        public delegate void btnSave_ClickedEventHandler(object sender);
        public void btnSave_MouseUp(object sender, EventArgs e)
        {

            if (btnSave_Click != null)
            {
                btnSave_Click(sender);
            }
        }

        public event btnCancel_ClickedEventHandler btnCancel_Click;
        public delegate void btnCancel_ClickedEventHandler(object sender);
        private void btnCancel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (btnCancel_Click != null)
            {
                btnCancel_Click(sender);
            }
        }
    }
}
