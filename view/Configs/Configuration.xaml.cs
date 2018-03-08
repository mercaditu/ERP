using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using entity;

namespace Cognitivo.Configs
{
    /// <summary>
    /// Interaction logic for Configuration.xaml
    /// </summary>
    public partial class Configuration : Page
    {
        public Configuration()
        {
            InitializeComponent();
        }

        private void Set_ContactPref(object sender, RoutedEventArgs e)
        {
            Sales.Settings.Default.Default_Customer = sbxContact.ContactID;
            Sales.Settings.Default.Save();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Sales.Settings.Default.Save();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (db db= new db())
            {
                int ContactID = Sales.Settings.Default.Default_Customer;
                contact contact = await db.contacts.FindAsync(ContactID);
                if (contact!=null)
                {
                    sbxContact.Text = contact.name;
                }
            }
        }
        private void btnSalesCost_Clicked(object sender, RoutedEventArgs e)
        {
            Utilities.SalesInvoice SI = new Utilities.SalesInvoice();
            MessageBox.Show(SI.Update_SalesCost() + " Records Updated", "Cognitivo ERP", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void btnMovementChiled_Clicked(object sender, RoutedEventArgs e)
        {
            using (db db = new db())
            {
                List<item_movement> itemMovementListparent = db.item_movement.Where(x => x.parent != null).ToList();
                foreach (item_movement item_movement in itemMovementListparent)
                {
                    item_movement.id_movement_value_rel = item_movement.parent.id_movement_value_rel;
                    item_movement.code = item_movement.parent.code;
                    item_movement.expire_date = item_movement.parent.expire_date;
                }

                db.SaveChanges();
            }
        }
        private void btnMovementValue_Clicked(object sender, RoutedEventArgs e)
        {
            using (db db = new db())
            {
                List<item_movement> parentlessMovements = db.item_movement.Where(x => x.parent == null).ToList();

                foreach (item_movement parentlessMovement in parentlessMovements.Where(x => x.id_movement_value_rel == null))
                {
                    item_movement_value_rel item_movement_value_rel = new item_movement_value_rel();

                    foreach (item_movement_value item_movement_value in parentlessMovement.item_movement_value)
                    {
                        item_movement_value_detail item_movement_value_detail = new item_movement_value_detail
                        {
                            unit_value = item_movement_value.unit_value,
                            comment = item_movement_value.comment
                        };

                        item_movement_value_rel.item_movement_value_detail.Add(item_movement_value_detail);
                    }

                    item_movement_value_rel.item_movement.Add(parentlessMovement);
                    parentlessMovement.item_movement_value_rel = item_movement_value_rel;
                }
                db.SaveChanges();
            }

            using (db db = new db())
            {
                List<item_movement> itemMovementListparent = db.item_movement.Where(x => x.parent != null).ToList();

                foreach (item_movement item_movement in itemMovementListparent)
                {
                    item_movement.id_movement_value_rel = item_movement.parent.id_movement_value_rel;
                }

                db.SaveChanges();
            }

        }



        private void AddDimension_Click(object sender, RoutedEventArgs e)
        {
           
            using (db db = new db())
            {
                foreach (production_order_detail production_order_detail in db.production_order_detail.Where(x => x.id_project_task != null).ToList())
                {
                    project_task project_task = db.project_task.Where(x => x.id_project_task == production_order_detail.id_project_task).First();
                    if (project_task != null)
                    {
                        if (production_order_detail.production_order_dimension.Count() == 0)
                        {
                            foreach (project_task_dimension project_task_dimension in project_task.project_task_dimension)
                            {
                                production_order_dimension production_order_dimension = new production_order_dimension();
                                production_order_dimension.id_dimension = project_task_dimension.id_dimension;
                                production_order_dimension.value = project_task_dimension.value;
                                production_order_dimension.id_measurement = project_task_dimension.id_measurement;
                                production_order_detail.production_order_dimension.Add(production_order_dimension);
                            }
                        }
                    }
                }
                db.SaveChanges();
            }

          

        }
    }
}
