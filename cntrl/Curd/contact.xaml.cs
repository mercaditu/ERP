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
        entity.dbContext entity = new entity.dbContext();

        private entity.contact _contactobject = null;
        public entity.contact contactobject { get { return _contactobject; } set { _contactobject = value; } }
        public List<entity.contact> contactList { get; set; }
        
        public contact()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                List<entity.contact> contactList = new List<global::entity.contact>();

                cbPriceList.ItemsSource = entity.db.item_price_list.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).AsNoTracking().ToList();

                cbCostCenter.ItemsSource = entity.db.app_cost_center.Where(a => a.is_active == true && a.id_company == CurrentSession.Id_Company).OrderBy(a => a.name).AsNoTracking().ToList();

                cbxRole.ItemsSource = entity.db.contact_role.Where(a => a.id_company == CurrentSession.Id_Company && a.is_active == true).OrderBy(a => a.name).AsNoTracking().ToList();

                CollectionViewSource contactViewSource = (CollectionViewSource)this.FindResource("contactViewSource");
                if (contactobject != null)
                {
                    contactList.Add(contactobject);
                   
                }
             
                contactViewSource.Source = contactList;
                contactViewSource.View.Refresh();
                contactViewSource.View.MoveCurrentToFirst();
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
