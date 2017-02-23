using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace entity.Brillo
{
    public class Inventory2Excel
    {
        public bool Create(item_inventory item_inventory)
        {
            if (item_inventory != null)
            {
                var wb = new XLWorkbook();

                List<app_location> LocationList = CurrentSession.Locations.Where(x => x.id_branch == item_inventory.id_branch).ToList();

                foreach (app_location app_location in LocationList)
                {
                    List<InventoryDetail> DetailList = new List<InventoryDetail>();

                    foreach (var inv_detail in item_inventory.item_inventory_detail.Where(x => x.id_location == app_location.id_location).ToList())
                    {
                        InventoryDetail Detail = new InventoryDetail();

                        //Hidden Columns
                        Detail.DetailID = inv_detail.id_inventory_detail;
                        Detail.LocationID = inv_detail.id_location;
                        Detail.ProductID = inv_detail.id_item_product;

                        Detail.Brand = inv_detail.item_product.item.item_brand != null ? inv_detail.item_product.item.item_brand.name : "";
                        Detail.Code = inv_detail.item_product.item.code;
                        Detail.Product = inv_detail.item_product.item.name;

                        Detail.ExpiryDate = inv_detail.expire_date;
                        Detail.BatchCode = inv_detail.batch_code;

                        Detail.System_Quantity = inv_detail.value_system;
                        Detail.Real_Quantity = inv_detail.value_counted;

                        Detail.Cost = inv_detail.unit_value;
                        Detail.Comments = inv_detail.comment;

                        DetailList.Add(Detail);
                    }

                    var ws = wb.Worksheets.Add(app_location.name);
                    //Insert Class into Data
                    ws.Cell(2, 1).InsertData(DetailList);
                    //Hide ID Columns
                    ws.Columns(1, 3).Hide();
                    //Create Headers
                    PropertyInfo[] properties = DetailList.First().GetType().GetProperties();
                    List<string> headerNames = properties.Select(prop => prop.Name).ToList();
                    for (int i = 0; i < headerNames.Count; i++)
                    {
                        ws.Cell(1, i + 1).Value = headerNames[i];
                    }

                    //Save File
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string full_file = "/Inventory " + item_inventory.app_branch.name + " " + DateTime.Now.ToShortDateString();
                    //Excel has an issue with file names being more than 31 characters long. So we need to remove excess.
                    string final_file = full_file.Length > 30 ? full_file.Remove(30) : full_file;


                    wb.SaveAs(path + final_file + ".xlsx");
                    return true;
                }
            }

            return false;
        }

        public bool Read(string Path, item_inventory item_inventory)
        {
            if (string.IsNullOrEmpty(Path) == false && item_inventory != null)
            {
                XLWorkbook workbook = new XLWorkbook(Path);
                foreach (var ws in workbook.Worksheets)
                {
                    //Ignore 1st Row due to Header
                    foreach (IXLRow row in ws.RowsUsed().Where(x => x.RowNumber() > 1))
                    {
                        int LocationID = row.Cell(2).GetValue<int>();
                        int ProductID = row.Cell(3).GetValue<int>();

                        item_inventory_detail detail = item_inventory.item_inventory_detail.Where(x => x.id_location == LocationID && x.id_item_product == ProductID).FirstOrDefault();

                        if (detail != null)
                        {
                            detail.value_counted = row.Cell(11).GetValue<decimal>();
                            detail.unit_value = row.Cell(12).GetValue<decimal>();
                            detail.comment = row.Cell(13).GetValue<string>();

                            if (row.Cell(8).Value != null)
                            {
                                //detail.expire_date = row.Cell(8).GetValue<DateTime>();
                            }

                            if (row.Cell(5).Value != null)
                            {
                                string Brand = row.Cell(5).GetValue<string>();

                                if (Brand != "")
                                {
                                    using (db db = new db())
                                    {
                                        if (db.item_brand.Where(x => x.name == Brand && x.id_company == CurrentSession.Id_Company).Count() == 0)
                                        {
                                            item_brand item_brand = new item_brand();
                                            item_brand.name = row.Cell(5).GetValue<string>();
                                            db.item_brand.Add(item_brand);
                                            db.SaveChanges();

                                            //Different Context.
                                            detail.item_product.item.id_brand = item_brand.id_brand;
                                        }
                                    }
                                }
                            }

                            detail.batch_code = row.Cell(9).GetValue<string>();
                        }
                    }
                }
            }

            return false;
        }
    }

    internal class InventoryDetail
    {
        public int DetailID { get; set; } //1
        public int LocationID { get; set; }
        public int ProductID { get; set; }

        public string Tag { get; set; }
        public string Brand { get; set; } //5

        public string Code { get; set; }
        public string Product { get; set; }

        public DateTime? ExpiryDate { get; set; } //8
        public string BatchCode { get; set; } //9

        public decimal System_Quantity { get; set; }
        public decimal? Real_Quantity { get; set; } //11

        public decimal Cost { get; set; } //12

        public string Comments { get; set; } //13
    }
}