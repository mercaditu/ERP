using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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

            MessageBox.Show("Done");
        }

        private void btnMovementValue_Clicked(object sender, RoutedEventArgs e)
        {
            db db = new db();

            int count = db.item_movement.Where(x => x.parent == null).Count();
            for (int i = 0; i < count; i = i + 100)
            {
                List<item_movement> parentlessMovements = db.item_movement
                    .Where(x => x.parent == null).OrderBy(x => x.id_movement).Skip(i).Take(100).ToList();
                AddItemMovementValue(ref db, parentlessMovements);
            }

            MessageBox.Show("50% proccess Done");

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
                                production_order_dimension production_order_dimension = new production_order_dimension
                                {
                                    id_dimension = project_task_dimension.id_dimension,
                                    value = project_task_dimension.value,
                                    id_measurement = project_task_dimension.id_measurement
                                };

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
                    //Get list of credits that do not have balance.
                    List<entity.Brillo.StockList> ItemsWithoutBalance = CurrentItems.GetListwithoutstock(CurrentSession.Id_Branch).ToList();

                    //Get list of credits that have balance.
                    List<entity.Brillo.StockList> ItemsWithBalance = CurrentItems.getProducts_InStock_GroupBy(CurrentSession.Id_Branch, DateTime.Now, true).ToList();

                    db.Database.ExecuteSqlCommand("update item_movement set  parent_id_movement=null where" +
                        " id_movement in (" + ItemsWithBalance.Select(x => x.MovementID).ToArray() + ")");

                    //Make parent null for items with balance. So that we can remove the 0 balance 
                    //foreach (entity.Brillo.StockList item in ItemsWithBalance.Where(x => x.ParentID > 0))
                    //{
                    //    item_movement im = db.item_movement.Find(item.MovementID);
                    //    if (im != null)
                    //    {
                    //        im.parent = null;
                    //        db.SaveChanges();
                    //    }
                    //}





                    //Take all the 0 Balance items, and move it to archive table. Then delete the detail.
                    foreach (entity.Brillo.StockList item in ItemsWithoutBalance)
                    {
                        item_movement im = db.item_movement.Where(x => x.id_movement == item.MovementID).FirstOrDefault();
                        if (im != null)
                        {
                            item_movement_archive item_movement_archive = AddMovement(im);
                            item_movement_archiveList.Add(item_movement_archive);

                            //I don't like this query. Don't call it by parent, but call it by child using navigation property.
                            List<item_movement> item_movementList = im.child.ToList();

                            foreach (item_movement item_movement in item_movementList)
                            {
                                item_movement_archive item_movement_archivechild = AddMovement(item_movement);
                                item_movement_archivechild.parent = item_movement_archive;
                                item_movement_archiveList.Add(item_movement_archivechild);
                                item_movementListDeletechild.Add(item_movement);
                            }

                            item_movementListDeleteparent.Add(im);
                        }

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
            item_movement_archive item_movement_archive = new item_movement_archive
            {
                id_item_product = im.id_item_product,
                id_transfer_detail = im.id_transfer_detail,
                id_execution_detail = im.id_execution_detail,
                id_purchase_invoice_detail = im.id_purchase_invoice_detail,
                id_purchase_return_detail = im.id_purchase_return_detail,
                id_sales_invoice_detail = im.id_sales_invoice_detail,
                id_sales_return_detail = im.id_sales_return_detail,
                id_inventory_detail = im.id_inventory_detail,
                id_sales_packing_detail = im.id_sales_packing_detail,
                id_purchase_packing_detail = im.id_purchase_packing_detail,
                id_location = im.id_location,
                id_movement_value_rel = im.id_movement_value_rel,
                status = im.status,
                debit = im.debit,
                credit = im.credit,
                comment = im.comment,
                code = im.code,
                expire_date = im.expire_date,
                trans_date = im.trans_date,
                id_company = im.id_company,
                id_user = im.id_user,
                is_head = im.is_head,
                timestamp = im.timestamp,
                is_read = im.is_read
            };

            return item_movement_archive;
        }

        private void AddSequence_click(object sender, RoutedEventArgs e)
        {
            using (db db = new db())
            {
                List<project> Projects = db.projects.ToList();
                foreach (project project in Projects)
                {
                    int sequence = 0;
                    foreach (project_task parent in project.project_task.Where(x => x.parent == null))
                    {

                        sequence = sequence + 1;
                        parent.SetSequence(sequence);



                    }

                }
                db.SaveChanges();
            }
        }

        private void AddAccontdetail_click(object sender, RoutedEventArgs e)
        {
            using (db db = new db())
            {
                //List<int?> payment_detailid = db.app_account_detail.Where(x => x.id_payment_detail > 0).Select(x => x.id_payment_detail).ToList();

                //List<payment_detail> payment_detailList = db.payment_detail
                //    .Where(x => !payment_detailid.Contains(x.id_payment_detail) && (x.id_payment_type == 1 || x.id_payment_type == 2))
                //    .Include(x=>x.payment_schedual).ToList();

                List<payment_detail> payment_details = db.payment_detail.Where(x => x.payment_type.payment_behavior == payment_type.payment_behaviours.Normal).Include(x => x.payment_schedual)
                    .Where(x => x.app_account_detail.Count() == 0)
                    .Where(x => x.payment.status != Status.Documents_General.Annulled)
                    .ToList();

                foreach (payment_detail payment_detail in payment_details)
                {
                    if (payment_detail.payment_schedual.Count() > 0)
                    {
                        app_account_detail app_account_detail = new app_account_detail();
                        app_account_session app_account_session = db.app_account_session.Where(x => x.op_date >= payment_detail.trans_date && x.cl_date <= payment_detail.trans_date).FirstOrDefault();

                        if (app_account_session != null)
                        {
                            app_account_detail.id_session = app_account_session.id_session;
                        }

                        app_account_detail.id_payment_detail = payment_detail.id_payment_detail;
                        app_account_detail.id_account = (int)payment_detail.id_account;
                        app_account_detail.id_currencyfx = payment_detail.id_currencyfx;
                        app_account_detail.id_payment_type = payment_detail.id_payment_type;
                        app_account_detail.status = payment_detail.payment_type.is_direct ? Status.Documents_General.Approved : Status.Documents_General.Pending;

                        if (payment_detail.payment_schedual.FirstOrDefault().id_purchase_invoice > 0 || payment_detail.payment_schedual.FirstOrDefault().id_purchase_order > 0)
                        {
                            app_account_detail.debit = Convert.ToDecimal(payment_detail.value);
                        }
                        else if (payment_detail.payment_schedual.FirstOrDefault().id_sales_invoice > 0 || payment_detail.payment_schedual.FirstOrDefault().id_sales_order > 0)
                        {
                            app_account_detail.credit = Convert.ToDecimal(payment_detail.value);
                        }

                        app_account_detail.comment = "Insert Through Configuration";
                        db.app_account_detail.Add(app_account_detail);
                    }
                  
                }

                db.SaveChanges();
            }

        }
    }
}
