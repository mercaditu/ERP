using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
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
            using (db db = new db())
            {
                int ContactID = Sales.Settings.Default.Default_Customer;
                contact contact = await db.contacts.FindAsync(ContactID);
                if (contact != null)
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
            db db = new db();
            int count = db.item_movement.Where(x => x.parent == null).Count();
            for (int i = 0; i < count; i = i + 100)
            {
                List<item_movement> parentlessMovements = db.item_movement
                    .Where(x => x.parent == null).OrderBy(x=>x.id_movement).Skip(i).Take(100).ToList();
                AddItemMovementValue(ref db, parentlessMovements);
            }
            MessageBox.Show("50% process Done");

            count = db.item_movement.Where(x => x.parent != null).Count();
            for (int i = 0; i < count; i = i + 1000)
            {
                List<item_movement> parentMovements = db.item_movement.Where(x => x.parent != null)
                    .OrderBy(x => x.id_movement).Skip(i).Take(1000).ToList();
                changeMovementValue(ref db, parentMovements);
            }


            MessageBox.Show("Done");
        }

        public void AddItemMovementValue(ref db db, List<item_movement> parentlessMovements)
        {

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

        public void changeMovementValue(ref db db, List<item_movement> itemMovementListparent)
        {
            foreach (item_movement item_movement in itemMovementListparent)
            {
                item_movement.id_movement_value_rel = item_movement.parent.id_movement_value_rel;
            }

            db.SaveChanges();
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

        private void AddData_Click(object sender, RoutedEventArgs e)
        {
            if (Microsoft.VisualBasic.Interaction.InputBox("Password?", "Password") == "paraguay")
            {
                int company_id = CurrentSession.Id_Company;
                List<item_movement_archive> item_movement_archiveList = new List<item_movement_archive>();
                List<item_movement> item_movementListDeletechild = new List<item_movement>();
                List<item_movement> item_movementListDeleteparent = new List<item_movement>();
                using (db db = new db())
                {
                    List<entity.Brillo.StockList> StockList = CurrentItems.GetListwithoutstock(CurrentSession.Id_Branch).ToList();
                    foreach (entity.Brillo.StockList item in StockList.Where(x => x.MovementID > 0))
                    {
                        item_movement im = db.item_movement.Where(x => x.id_movement == item.MovementID).First();
                        item_movement_archive item_movement_archive = AddMovement(im);
                        item_movement_archiveList.Add(item_movement_archive);
                        List<item_movement> item_movementList = db.item_movement.Where(x => x.parent.id_movement == item.MovementID).ToList();
                        foreach (item_movement item_movement in item_movementList)
                        {
                            item_movement_archive item_movement_archivechild = AddMovement(item_movement);
                            item_movement_archivechild.parent = item_movement_archive;
                            item_movement_archiveList.Add(item_movement_archivechild);
                            item_movementListDeletechild.Add(item_movement);
                        }
                        item_movementListDeleteparent.Add(im);

                    }
                    db.item_movement.RemoveRange(item_movementListDeletechild);
                    db.SaveChanges();
                    db.item_movement.RemoveRange(item_movementListDeleteparent);
                    db.SaveChanges();
                    db.item_movement_archive.AddRange(item_movement_archiveList);
                    db.SaveChanges();
                }
            }



        }
        public item_movement_archive AddMovement(item_movement im)
        {
            item_movement_archive item_movement_archive = new item_movement_archive();
            item_movement_archive.id_item_product = im.id_item_product;
            item_movement_archive.id_transfer_detail = im.id_transfer_detail;
            item_movement_archive.id_execution_detail = im.id_execution_detail;
            item_movement_archive.id_purchase_invoice_detail = im.id_purchase_invoice_detail;
            item_movement_archive.id_purchase_return_detail = im.id_purchase_return_detail;
            item_movement_archive.id_sales_invoice_detail = im.id_sales_invoice_detail;
            item_movement_archive.id_sales_return_detail = im.id_sales_return_detail;
            item_movement_archive.id_inventory_detail = im.id_inventory_detail;
            item_movement_archive.id_sales_packing_detail = im.id_sales_packing_detail;
            item_movement_archive.id_purchase_packing_detail = im.id_purchase_packing_detail;
            item_movement_archive.id_location = im.id_location;
            item_movement_archive.id_movement_value_rel = im.id_movement_value_rel;
            item_movement_archive.status = im.status;
            item_movement_archive.debit = im.debit;
            item_movement_archive.credit = im.credit;
            item_movement_archive.comment = im.comment;
            item_movement_archive.code = im.code;
            item_movement_archive.expire_date = im.expire_date;
            item_movement_archive.trans_date = im.trans_date;
            item_movement_archive.id_company = im.id_company;
            item_movement_archive.id_user = im.id_user;
            item_movement_archive.is_head = im.is_head;
            item_movement_archive.timestamp = im.timestamp;
            item_movement_archive.is_read = im.is_read;
            return item_movement_archive;
        }







    }
}
