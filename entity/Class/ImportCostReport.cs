using entity;
using entity.Class;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace entity.Class
{
    public class ImportCostReport
    {

        ImpexDB ImpexDB = new ImpexDB();
        public List<Impex_ItemDetail> GetExpensesForAllIncoterm(purchase_invoice PurchaseInvoice)
        {
            List<Impex_ItemDetail> Impex_ItemDetailLIST = new List<Impex_ItemDetail>();
            List<impex_incoterm> impex_incotermList = ImpexDB.impex_incoterm.ToList();
            List<entity.Class.Impex_Products> Impex_ProductsLIST = new List<entity.Class.Impex_Products>();
       
            foreach (impex_incoterm Incoterm in impex_incotermList)
            {



                List<impex_incoterm_detail> IncotermDetail = ImpexDB.impex_incoterm_detail.Where(i => i.id_incoterm == Incoterm.id_incoterm && i.buyer == true).ToList();
                decimal totalExpense = 0;


                if (PurchaseInvoice != null)
                {
                    decimal GrandTotal = PurchaseInvoice.purchase_invoice_detail.Where(z => z.item != null && z.item.item_product != null).Sum(y => y.SubTotal);
                    foreach (purchase_invoice_detail _purchase_invoice_detail in PurchaseInvoice.purchase_invoice_detail.Where(x => x.item != null && x.item.item_product != null))
                    {
                        foreach (impex_incoterm_detail item in IncotermDetail)
                        {
                            impex_expense impex_expense = new impex_expense();

                            impex_expense _impex_expense = ImpexDB.impex_expense.Where(x => x.id_incoterm_condition == item.id_incoterm_condition && x.id_purchase_invoice == PurchaseInvoice.id_purchase_invoice).FirstOrDefault();
                            if (_impex_expense != null)
                            {
                                totalExpense += (decimal)_impex_expense.value;
                            }
                            else
                            {
                                totalExpense += 0;
                            }



                        }
                        entity.Class.Impex_ItemDetail ImpexImportDetails = new entity.Class.Impex_ItemDetail();
                        ImpexImportDetails.number = _purchase_invoice_detail.purchase_invoice.number;
                        ImpexImportDetails.id_item = (int)_purchase_invoice_detail.id_item;
                        ImpexImportDetails.item_code = ImpexDB.items.Where(a => a.id_item == _purchase_invoice_detail.id_item).FirstOrDefault().code;
                        ImpexImportDetails.item = ImpexDB.items.Where(a => a.id_item == _purchase_invoice_detail.id_item).FirstOrDefault().name;
                        ImpexImportDetails.quantity = _purchase_invoice_detail.quantity;
                        ImpexImportDetails.unit_cost = _purchase_invoice_detail.unit_cost;
                        ImpexImportDetails.sub_total = _purchase_invoice_detail.SubTotal;
                        ImpexImportDetails.id_invoice = _purchase_invoice_detail.id_purchase_invoice;
                        ImpexImportDetails.id_invoice_detail = _purchase_invoice_detail.id_purchase_invoice_detail;

                        if (totalExpense > 0)
                        {
                            ImpexImportDetails.unit_Importcost = Math.Round(((_purchase_invoice_detail.SubTotal / GrandTotal) * totalExpense) / _purchase_invoice_detail.quantity, 2);
                            ImpexImportDetails.prorated_cost = _purchase_invoice_detail.unit_cost + ImpexImportDetails.unit_Importcost;
                        }

                        decimal SubTotal = (_purchase_invoice_detail.quantity * ImpexImportDetails.prorated_cost);
                        ImpexImportDetails.sub_total = Math.Round(SubTotal, 2);
                        ImpexImportDetails.incoterm = Incoterm.name;
                        Impex_ItemDetailLIST.Add(ImpexImportDetails);
                    }

                }
            }







            return Impex_ItemDetailLIST;
        }
        public DataTable ImportDatatable(purchase_invoice PurchaseInvoice)
        {

            return ToDataTable<Impex_ItemDetail>(GetExpensesForAllIncoterm(PurchaseInvoice));
        }
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

    }
}
