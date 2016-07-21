using entity.Brillo.Document;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace entity
{
    public partial class InventoryDB : BaseDB
    {
        public override int SaveChanges()
        {
            validate_item_inventory();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            validate_item_inventory();
            return base.SaveChangesAsync();
        }

        private void validate_item_inventory()
        {
            NumberOfRecords = 0;

            foreach (item_inventory item_inventory in base.item_inventory.Local)
            {
                if (item_inventory.IsSelected)
                {
                    if (item_inventory.State == EntityState.Added)
                    {
                        item_inventory.timestamp = DateTime.Now;
                        item_inventory.State = EntityState.Unchanged;
                        Entry(item_inventory).State = EntityState.Added;
                    }
                    else if (item_inventory.State == EntityState.Modified)
                    {
                        item_inventory.timestamp = DateTime.Now;
                        item_inventory.State = EntityState.Unchanged;
                        Entry(item_inventory).State = EntityState.Modified;
                    }
                    else if (item_inventory.State == EntityState.Deleted)
                    {
                        item_inventory.timestamp = DateTime.Now;
                        item_inventory.State = EntityState.Unchanged;
                        base.item_inventory.Remove(item_inventory);
                    }


                    NumberOfRecords += 1;

                }
                else if (item_inventory.State > 0)
                {
                    if (item_inventory.State != EntityState.Unchanged)
                    {
                        Entry(item_inventory).State = EntityState.Unchanged;
                    }
                }
            }
        }

        private void validate_item_inventory_detail(item_inventory_detail item_inventory_detail)
        {
            NumberOfRecords = 0;

            if (item_inventory_detail.State == EntityState.Added)
            {
                item_inventory_detail.timestamp = DateTime.Now;
                item_inventory_detail.State = EntityState.Unchanged;
                Entry(item_inventory_detail).State = EntityState.Added;

                NumberOfRecords += 1;
            }
            else if (item_inventory_detail.State == EntityState.Modified)
            {
                item_inventory_detail.timestamp = DateTime.Now;
                item_inventory_detail.State = EntityState.Unchanged;
                Entry(item_inventory_detail).State = EntityState.Modified;

                NumberOfRecords += 1;
            }
            else if (item_inventory_detail.State == EntityState.Deleted)
            {
                item_inventory_detail.timestamp = DateTime.Now;
                item_inventory_detail.State = EntityState.Unchanged;
                base.item_inventory_detail.Remove(item_inventory_detail);

                NumberOfRecords += 1;
            }

            if (item_inventory_detail.State != EntityState.Unchanged)
            {
                Entry(item_inventory_detail).State = EntityState.Unchanged;
            }
        }

        public bool Approve()
        {
            NumberOfRecords = 0;

            foreach (item_inventory item_inventory in base.item_inventory.Local.Where(x => x.status != Status.Documents.Issued && x.IsSelected))
            {
                if (item_inventory.id_inventory == 0)
                {
                    SaveChanges();
                }

                List<item_movement> item_movementLIST = new List<item_movement>();

                foreach (item_inventory_detail item_inventory_detail in item_inventory.item_inventory_detail)
                {
                    if (item_inventory_detail.item_product.item.name.Contains("KIT KAT 4 FINGER"))
                    {
                        NumberOfRecords = NumberOfRecords;
                    }

                    if (item_inventory_detail.item_inventory_dimension.Count() > 0)
                    {
                        item_movement item_movement = new item_movement();
                        item_movement.id_item_product = item_inventory_detail.item_product.id_item_product;
                        item_movement.id_location = item_inventory_detail.app_location.id_location;
                        item_movement.comment = Brillo.Localize.Text<string>("Inventory") + ": " + item_inventory_detail.comment;
                        item_movement.status = Status.Stock.InStock;
                        item_movement.debit = 0;
                        item_movement.credit = item_inventory_detail.value_counted;
                        item_movement.trans_date = item_inventory.trans_date;
                        item_movement.id_inventory_detail = item_inventory_detail.id_inventory_detail;
                        item_movement.timestamp = DateTime.Now;

                        if (item_inventory_detail.unit_value > 0 && item_inventory_detail.id_currencyfx > 0)
                        {
                            item_movement_value item_movement_value = new item_movement_value();
                            item_movement_value.unit_value = item_inventory_detail.unit_value;
                            item_movement_value.id_currencyfx = item_inventory_detail.id_currencyfx;
                            item_movement_value.comment = Brillo.Localize.Text<string>("Inventory") + ": " + item_inventory_detail.comment;
                            item_movement.item_movement_value.Add(item_movement_value);
                        }

                        foreach (item_inventory_dimension item_inventory_dimension in item_inventory_detail.item_inventory_dimension)
                        {
                            item_movement_dimension item_movement_dimension = new item_movement_dimension();
                            item_movement_dimension.value = item_inventory_dimension.value;
                            item_movement_dimension.id_dimension = item_inventory_dimension.id_dimension;
                            //item_movement_dimension.id_measurement = item_inventory_dimension.id_measurement;
                            item_movement.item_movement_dimension.Add(item_movement_dimension);
                        }

                        item_movementLIST.Add(item_movement);

                        NumberOfRecords += 1;
                    }
                    else
                    {
                        if (item_inventory_detail.IsSelected)
                        {
                            decimal delta = 0;

                            if (item_inventory_detail.value_system != item_inventory_detail.value_counted)
                            {
                                //Negative
                                delta = item_inventory_detail.value_counted - item_inventory_detail.value_system;
                            }

                            if (delta != 0)
                            {
                                item_movement item_movement = new item_movement();
                                item_movement.id_item_product = item_inventory_detail.item_product.id_item_product;
                                item_movement.id_location = item_inventory_detail.app_location.id_location;
                                item_movement.comment = Brillo.Localize.Text<string>("Inventory") + ": " + item_inventory_detail.comment;
                                item_movement.status = Status.Stock.InStock;
                                item_movement.credit = delta > 0 ? delta : 0;
                                item_movement.debit = delta < 0 ? Math.Abs(delta) : 0;
                                item_movement.trans_date = item_inventory.trans_date;
                                item_movement.id_inventory_detail = item_inventory_detail.id_inventory_detail;
                                item_movement.timestamp = DateTime.Now;

                                if (item_inventory_detail.unit_value > 0 && item_inventory_detail.id_currencyfx > 0)
                                {
                                    item_movement_value item_movement_value = new item_movement_value();
                                    item_movement_value.unit_value = item_inventory_detail.unit_value;
                                    item_movement_value.id_currencyfx = item_inventory_detail.id_currencyfx;
                                    item_movement.item_movement_value.Add(item_movement_value);
                                }
                                else
                                {
                                    item_inventory_detail parent_inventory = base.item_inventory_detail.Where(x => x.id_item_product == item_inventory_detail.id_item_product && x.id_location == item_inventory_detail.id_location && x.unit_value > 0).FirstOrDefault();
                                    if (parent_inventory!=null)
                                    {
                                        item_movement_value item_movement_value = new item_movement_value();
                                    item_movement_value.unit_value = parent_inventory.unit_value;
                                    item_movement_value.id_currencyfx = parent_inventory.id_currencyfx;
                                    item_movement.item_movement_value.Add(item_movement_value);
                                    }
                                }

                                item_movementLIST.Add(item_movement);
                                NumberOfRecords += 1;

                            }
                        }
                    }
                }

                base.item_movement.AddRange(item_movementLIST);

                item_inventory.status = Status.Documents.Issued;

                //string PathFull = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CogntivoERP\\TemplateFiles\\Inventory.rdlc";
                //if (Directory.Exists(PathFull) == false)
                //{
                //    string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CogntivoERP";

                //    //If path (CognitivoERP) does not exist, create path.
                //    if (Directory.Exists(path) == false)
                //    {
                //        Directory.CreateDirectory(path);
                //    }

                //    string SubFolder = "\\TemplateFiles";

                //    //If path (TemplateFiles) does not exist, create path
                //    if (!Directory.Exists(path + SubFolder))
                //    {
                //        Directory.CreateDirectory(path + SubFolder);
                //    }

                //    //If file does not exist, create file.
                //    if (!File.Exists(path + SubFolder + "\\Inventory.rdlc"))
                //    {
                //        //Add Logic
                //        if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Inventory.rdlc"))
                //        {
                //            File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\Inventory.rdlc",
                //                   path + SubFolder + "\\Inventory.rdlc");
                //        }

                //    }
                //}

               
                //DataSource DataSource = new DataSource();

                //DocumentViewr DocumentViewr = new DocumentViewr();
                //DocumentViewr.reportViewer.LocalReport.ReportPath = PathFull; // Path of the rdlc file
                //DocumentViewr.reportViewer.LocalReport.DataSources.Add(DataSource.Create(item_inventory));
                //DocumentViewr.reportViewer.RefreshReport();

                //Window window = new Window
                //{
                //    Title = "Report",
                //    Content = DocumentViewr
                //};

                //window.ShowDialog();
                SaveChanges();
            }

            return true;
        }
    }
}



