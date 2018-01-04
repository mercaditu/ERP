using ClosedXML.Excel;
using entity.Controller.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace entity.Brillo
{
    public class Inventory2Excel
    {
        public bool Create(item_inventory item_inventory, int id_location)
        {
            if (item_inventory != null)
            {
                var wb = new XLWorkbook();
                List<InventoryDetail> DetailList = new List<InventoryDetail>();

                foreach (var inv_detail in item_inventory.item_inventory_detail.Where(x => x.id_location == id_location).OrderBy(x => x.app_location.name).ToList())
                {
                    InventoryDetail Detail = new InventoryDetail()
                    {
                        //Hidden Columns
                        DetailID = inv_detail.id_inventory_detail,
                        LocationID = inv_detail.id_location,
                        ProductID = inv_detail.id_item_product,

                        Location = inv_detail.app_location.name,

                        Brand = inv_detail.item_product.item.item_brand != null ? inv_detail.item_product.item.item_brand.name : "",
                        Code = inv_detail.item_product.item.code,
                        //Probably Trim item name if it is too long
                        Product = inv_detail.item_product.item.name, //.Substring(0, 254),

                        ExpiryDate = inv_detail.expire_date,
                        BatchCode = inv_detail.batch_code,

                        System_Quantity = inv_detail.value_system,
                        Real_Quantity = inv_detail.value_counted,

                        Cost = inv_detail.unit_value,
                        Comments = inv_detail.comment,
                        MovementID = inv_detail.movement_id
                    };

                    DetailList.Add(Detail);
                }

                if (DetailList.Count() > 0)
                {
                    var ws = wb.Worksheets.Add(item_inventory.app_branch.name);
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
                }

                //Save File
                //Add code to show save panel.
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog()
                {
                    FileName = Localize.StringText("Inventory") + " " + item_inventory.app_branch.name + " " + item_inventory.trans_date.Month + "-" + item_inventory.trans_date.Year, // Default file name
                    DefaultExt = ".xlsx", // Default file extension
                    Filter = "Text documents (.xlsx)|*.xlsx" // Filter files by extension
                };

                // Show save file dialog box
                bool? result = dlg.ShowDialog();

                // Process save file dialog box results
                if (result == true)
                {
                    // Save document
                    wb.SaveAs(dlg.FileName);
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
                    List<item_brand> Brands = new List<item_brand>();
                    using (db db = new db())
                    {
                        Brands = db.item_brand.Where(x => x.id_company == CurrentSession.Id_Company).ToList();
                    }

                    //Ignore 1st Row due to Header
                    foreach (IXLRow row in ws.RowsUsed().Where(x => x.RowNumber() > 1))
                    {
                        int LocationID = row.Cell(2).GetValue<int>();
                        int ProductID = row.Cell(3).GetValue<int>();
                        int InvDetailID = row.Cell(1).GetValue<int>();

                        //Reuse or Create the Detail.
                        item_inventory_detail detail = item_inventory.item_inventory_detail.Where(x => x.id_inventory_detail == InvDetailID).FirstOrDefault();

                        //Allow you to create new rows into Excel and load that into the System.
                        if (detail == null)
                        {
                            using (db db = new db())
                            {
                                string ItemCode = row.Cell(2).GetValue<string>();
                                ProductID = db.item_product.Where(x => x.item.code == ItemCode).Select(x => x.id_item_product).FirstOrDefault();
                            }

                            detail = new item_inventory_detail()
                            {
                                id_item_product = ProductID,
                                unit_value = 0,
                                value_system = 0,
                                id_location = LocationID
                            };

                            item_inventory.item_inventory_detail.Add(detail);
                        }

                        if (detail != null)
                        {
                            if (row.Cell(12).Value != null && row.Cell(12).Value.ToString() != "")
                            {
                                detail.value_counted = row.Cell(12).GetValue<decimal>();
                            }

                            detail.unit_value = row.Cell(13).GetValue<decimal>();
                            detail.comment = row.Cell(14).GetValue<string>();

                            //Run Batch Code and Expiration Check if CanExpire is set to true.
                            if (detail.item_product.can_expire)
                            {
                                detail.batch_code = row.Cell(10).GetValue<string>();

                                if (row.Cell(9).Value != null && row.Cell(9).Value.ToString() != "")
                                {
                                    detail.expire_date = row.Cell(9).GetValue<DateTime>();
                                    detail.RaisePropertyChanged("expire_date");
                                }
                            }

                            if (row.Cell(6).Value != null)
                            {
                                string Brand = row.Cell(6).GetValue<string>();

                                if (Brand != "")
                                {
                                    item_brand item_brand = new List<item_brand>().Where(x => x.name == Brand).FirstOrDefault();

                                    if (item_brand != null)
                                    {
                                        detail.item_product.item.id_brand = item_brand.id_brand;
                                    }
                                    else
                                    {
                                        using (db db = new db())
                                        {
                                            item_brand New_item_brand = new item_brand()
                                            {
                                                name = row.Cell(6).GetValue<string>()
                                            };

                                            db.item_brand.Add(New_item_brand);
                                            db.SaveChanges();

                                            //Different Context.
                                            detail.item_product.item.id_brand = New_item_brand.id_brand;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        public bool UploadAStrilloExcel(string Path, item_inventory item_inventory, int id_location, ref InventoryController InventoryController)
        {
            if (string.IsNullOrEmpty(Path) == false && item_inventory != null)
            {
                XLWorkbook workbook = new XLWorkbook(Path);
                app_dimension app_dimensionlargo = InventoryController.db.app_dimension.Where(x => x.id_company == CurrentSession.Id_Company && x.name == "LARGO").FirstOrDefault();
                app_dimension app_dimensionancho = InventoryController.db.app_dimension.Where(x => x.id_company == CurrentSession.Id_Company && x.name == "ANCHO").FirstOrDefault();
                foreach (var ws in workbook.Worksheets)
                {


                    //Ignore 1st Row due to Header
                    foreach (IXLRow row in ws.RowsUsed().Where(x => x.RowNumber() > 1))
                    {
                        int LocationID = id_location;
                        decimal Cost = 0;
                        decimal Stock = 0;
                        decimal Largo = 0;
                        decimal Ancho = 0;
                        if (decimal.TryParse(row.Cell(3).GetValue<string>(), out var s))
                        {
                            Stock = row.Cell(3).GetValue<decimal>();
                        }
                        if (decimal.TryParse(row.Cell(4).GetValue<string>(), out var c))
                        {
                            Cost = row.Cell(4).GetValue<decimal>();
                        }
                        if (decimal.TryParse(row.Cell(6).GetValue<string>(), out var l))
                        {
                            Largo = row.Cell(6).GetValue<decimal>();
                        }
                        if (decimal.TryParse(row.Cell(5).GetValue<string>(), out var a))
                        {
                            Ancho = row.Cell(5).GetValue<decimal>();
                        }





                        //Allow you to create new rows into Excel and load that into the System.

                        item_inventory_detail detail = new item_inventory_detail();
                        string ItemCode = row.Cell(1).GetValue<string>();
                        if (ItemCode != "")
                        {

                            item_product item_product = InventoryController.db.item_product.Where(x => x.item.code == ItemCode).FirstOrDefault();

                            if (item_product == null)
                            {
                                using (db db = new db())
                                {
                                    item item = new item();
                                    item.name = row.Cell(2).GetValue<string>();
                                    item.code = row.Cell(1).GetValue<string>();
                                    item.id_item_type = item.item_type.Product;
                                    app_vat_group app_vat_group = db.app_vat_group.Where(x => x.id_company == CurrentSession.Id_Company && x.is_active).FirstOrDefault();
                                    if (app_vat_group != null)
                                    {
                                        item.id_vat_group = app_vat_group.id_vat_group;
                                    }
                                    else { break; }


                                    item_product _product = new item_product();
                                    item.item_product.Add(_product);
                                    db.items.Add(item);
                                    db.SaveChanges();
                                }
                                item_product = InventoryController.db.item_product.Where(x => x.item.code == ItemCode).FirstOrDefault();
                                detail.item_product = item_product;
                                detail.id_item_product = item_product.id_item_product;
                            }
                            else
                            {
                                detail.item_product = item_product;
                                detail.id_item_product = item_product.id_item_product;
                            }


                            detail.unit_value = 0;
                            detail.value_system = 0;
                            detail.id_location = LocationID;





                            detail.value_counted = Stock;




                            if (Cost > 0)
                            {
                                detail.unit_value = Cost;
                            }

                            if (Largo > 0)
                            {

                                if (app_dimensionlargo != null)
                                {
                                    item_inventory_dimension item_inventory_dimensionlargo = new item_inventory_dimension();
                                    item_inventory_dimensionlargo.id_dimension = app_dimensionlargo.id_dimension;
                                    item_inventory_dimensionlargo.app_dimension = app_dimensionlargo;
                                    item_inventory_dimensionlargo.value = Largo;
                                    detail.item_inventory_dimension.Add(item_inventory_dimensionlargo);
                                }

                                if (app_dimensionancho != null && Ancho > 0)
                                {
                                    item_inventory_dimension item_inventory_dimensionancho = new item_inventory_dimension();
                                    item_inventory_dimensionancho.id_dimension = app_dimensionancho.id_dimension;
                                    item_inventory_dimensionancho.app_dimension = app_dimensionancho;
                                    item_inventory_dimensionancho.value = Ancho;
                                    detail.item_inventory_dimension.Add(item_inventory_dimensionancho);
                                }



                            }
                            detail.item_inventory = item_inventory;
                            item_inventory.item_inventory_detail.Add(detail);
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

        public string Location { get; set; } //4

        public string Tag { get; set; }
        public string Brand { get; set; } //6

        public string Code { get; set; }
        public string Product { get; set; }

        public DateTime? ExpiryDate { get; set; } //9
        public string BatchCode { get; set; } //10

        public decimal System_Quantity { get; set; }
        public decimal? Real_Quantity { get; set; } //12

        public decimal Cost { get; set; } //13

        public string Comments { get; set; } //14
        public int? MovementID { get; set; } //15
    }
}