using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using entity;
using System.IO;
using cntrl.Controls;


namespace Cognitivo.Report
{
    public partial class ReportPage : MetroWindow
    {
        public entity.App.Names Reports { get; set; }
        db db = new db();

        public app_geography Geography { get; set; }
        public contact Contact { get; set; }
        public item Item { get; set; }
       
        public DateTime start_Range
        {
            get { return _start_Range; }
            set
            {
                if (_start_Range != value)
                {
                    _start_Range = value;
                }
            }
        }
        private DateTime _start_Range = DateTime.Now.AddMonths(-1);


        public DateTime end_Range
        {
            get { return _end_Range; }
            set
            {
                if (_end_Range != value)
                {
                    _end_Range = value;
                }
            }
        }
        private DateTime _end_Range = DateTime.Now;

        /// <summary>
        /// Condition KeyWord Array.
        /// </summary>
        public string[] ConditionArray { get; set; }
        public string tbxCondition
        {
            get
            {
                return _tbxCondition;
            }
            set
            {
                if (_tbxCondition != value)
                {
                    _tbxCondition = value;
                    if (value=="")
                    {
                        Array.Clear(ConditionArray, 0, ConditionArray.Length);
                    }
                    else
                    {
                        ConditionArray = _tbxCondition.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                    }
                   
                }
            }
        }
        private string _tbxCondition;

        /// <summary>
        /// Contract KeyWord Array.
        /// </summary>
        public string[] ContractArray { get; set; }
        public string tbxContract
        {
            get
            {
                return _tbxContract;
            }
            set
            {
                if (_tbxContract != value)
                {
                    _tbxContract = value;
                    if (value == "")
                    {
                        Array.Clear(ContractArray, 0, ConditionArray.Length);
                    }
                    else
                    {
                        ContractArray = _tbxContract.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                    }
                  
                
                }
            }
        }
        private string _tbxContract;

        /// <summary>
        /// Tag KeyWord Array.
        /// </summary>
        public string TagArray { get; set; }
        public string tbxTag
        {
            get
            {
                return _tbxTag;
            }
            set
            {
                if (_tbxTag != value)
                {
                    _tbxTag = value;
                    TagArray = _tbxTag;
                }
            }
        }
        private string _tbxTag;
    

        /// <summary>
        /// Brand KeyWord Array.
        /// </summary>
        public string[] BrandArray { get; set; }
        public string tbxBrand
        {
            get
            {
                return _tbxBrand;
            }
            set
            {
                if (_tbxBrand != value)
                {
                    _tbxBrand = value;
                    if (value == "")
                    {
                        Array.Clear(BrandArray, 0, ConditionArray.Length);
                    }
                    else
                    {
                        BrandArray = _tbxBrand.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                    }
                
                }
            }
        }
        private string _tbxBrand;

        public ReportPage()
        {
            InitializeComponent();
        }

        private void btnGridSearch(object sender, RoutedEventArgs e)
        {
            ListBoxItem ListBoxItem = NavList.SelectedItem as ListBoxItem;

            if (ListBoxItem != null)
            {
                string ReportName = "Cognitivo.Report." + ListBoxItem.Tag + "_Report";

                try
                {
                    Page objPage = default(Page);
                    Type PageInstanceType = null;

                    PageInstanceType = Type.GetType(ReportName, false, true);
                    objPage = (Page)Activator.CreateInstance(PageInstanceType);
                    rptFrame.Navigate(objPage);
                    Cursor = Cursors.Arrow;
                }
                catch { }
            }
        }

        private void ListBoxItemSales_Selected(object sender, RoutedEventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            path = path + "\\CogntivoERP";
            string SubFolder = "";
            SubFolder = "\\TemplateFiles";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Directory.CreateDirectory(path + SubFolder);
            }
            else if (!Directory.Exists(path + SubFolder))
            {
                Directory.CreateDirectory(path + SubFolder);

            }

            var predicate = PredicateBuilder.True<entity.sales_invoice>();

            if (ConditionArray != null)
            {
                if (ConditionArray.Count() > 0)
                {
                    predicate = predicate.And(x => ConditionArray.Contains(x.app_condition.name));
                }
            }

            if (ContractArray != null)
            {
                if (ContractArray.Count() > 0)
                {
                    predicate = predicate.And(x => ContractArray.Contains(x.app_contract.name));
                }
            }

            if (start_Range != Convert.ToDateTime("1/1/0001"))
            {
                predicate = predicate.And(x => x.trans_date >= start_Range);

            }
            if (end_Range != Convert.ToDateTime("1/1/0001"))
            {
                predicate = predicate.And(x => x.trans_date <= end_Range);

            }
            if (Contact != null)
            {
                predicate = predicate.And(x => x.contact == Contact);
            }


            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@path + SubFolder + "\\SalesHechuka.txt", true))
            {

                List<sales_invoice> SaleInvoiceList = db.sales_invoice.Where(predicate).ToList();
                string Line = string.Empty;
                if (file.BaseStream.Length==0)
                {
                     Line = "Contact gov code" + "\t" + "contact name" + "\t" + "number" + "\t" + "date";
                    foreach (app_vat_group app_vat_group in db.app_vat_group.ToList())
                    {
                        Line += "\t" + app_vat_group.name;
                    }
                    file.WriteLine(Line);
                }
              
                foreach (sales_invoice sales_invoice in SaleInvoiceList)
                {
                    Line = string.Empty;
                    Line = sales_invoice.contact.gov_code
                        + "\t" + sales_invoice.contact.name
                        + "\t" + sales_invoice.number
                        + "\t" + sales_invoice.trans_date + "\t" + sales_invoice.app_condition.name + "\t" + sales_invoice.app_contract.name;
                      
                    foreach (app_vat_group app_vat_group in db.app_vat_group.ToList())
                    {
                       Line += "\t" + sales_invoice.sales_invoice_detail.Where(x => x.id_vat_group == app_vat_group.id_vat_group).Sum(x => x.SubTotal_Vat - x.SubTotal);
                    }
                    file.WriteLine(Line);
                }

                var predicatePurchaseReturn = PredicateBuilder.True<entity.purchase_return>();

                if (ConditionArray != null)
                {
                    if (ConditionArray.Count() > 0)
                    {
                        predicatePurchaseReturn = predicatePurchaseReturn.And(x => ConditionArray.Contains(x.app_condition.name));
                    }
                }

                if (ContractArray != null)
                {
                    if (ContractArray.Count() > 0)
                    {
                        predicatePurchaseReturn = predicatePurchaseReturn.And(x => ContractArray.Contains(x.app_contract.name));
                    }
                }

                if (start_Range != Convert.ToDateTime("1/1/0001"))
                {
                    predicatePurchaseReturn = predicatePurchaseReturn.And(x => x.trans_date >= start_Range);

                }

                if (end_Range != Convert.ToDateTime("1/1/0001"))
                {
                    predicatePurchaseReturn = predicatePurchaseReturn.And(x => x.trans_date <= end_Range);

                }

                if (Contact != null)
                {
                    predicatePurchaseReturn = predicatePurchaseReturn.And(x => x.contact == Contact);
                }

                List<purchase_return> PurchaseReturnList = db.purchase_return.Where(predicatePurchaseReturn).ToList();
                foreach (purchase_return purchase_return in PurchaseReturnList)
                {
                    Line = string.Empty;
                    Line = purchase_return.contact.gov_code 
                        + "\t" + purchase_return.contact.name 
                        + "\t" + purchase_return.number
                        + "\t" + purchase_return.trans_date + "\t" + purchase_return.app_condition.name + "\t" + purchase_return.app_contract.name;
                        

                    foreach (app_vat_group app_vat_group in db.app_vat_group.ToList())
                    {
                        Line += "\t" + purchase_return.purchase_return_detail.Where(x => x.id_vat_group == app_vat_group.id_vat_group).Sum(x => x.SubTotal_Vat - x.SubTotal);
                    }
                                        file.WriteLine(Line);

                }
                
                MessageBox.Show("Files Saved...");
            }
        }
        public string GetTag(List<item_tag_detail> item_tag_detail)
        {
            string TagList = "";
            if (item_tag_detail.Count > 0)
            {
                foreach (item_tag_detail _item_tag_detail in item_tag_detail)
                {
                    if (!TagList.Contains(_item_tag_detail.item_tag.name))
                    {
                        TagList = TagList + "," + _item_tag_detail.item_tag.name;
                    }
                }
                return TagList.Remove(0, 1);
            }

            return TagList;
        }
        private void ListBoxItemPurchase_Selected(object sender, RoutedEventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            path = path + "\\CogntivoERP";
            string SubFolder = "";
            SubFolder = "\\TemplateFiles";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Directory.CreateDirectory(path + SubFolder);
            }
            else if (!Directory.Exists(path + SubFolder))
            {
                Directory.CreateDirectory(path + SubFolder);

            }
            var predicate = PredicateBuilder.True<entity.purchase_invoice>();

            if (ConditionArray != null)
            {
                if (ConditionArray.Count() > 0)
                {
                    predicate = predicate.And(x => ConditionArray.Contains(x.app_condition.name));
                }
            }

            if (ContractArray != null)
            {
                if (ContractArray.Count() > 0)
                {
                    predicate = predicate.And(x => ContractArray.Contains(x.app_contract.name));
                }
            }

            if (start_Range != Convert.ToDateTime("1/1/0001"))
            {
                predicate = predicate.And(x => x.trans_date >= start_Range);

            }
            if (end_Range != Convert.ToDateTime("1/1/0001"))
            {
                predicate = predicate.And(x => x.trans_date <= end_Range);

            }
            if (Contact != null)
            {
                predicate = predicate.And(x => x.contact == Contact);
            }


            using (System.IO.StreamWriter file =
           new System.IO.StreamWriter(@path + SubFolder + "\\PurchaseHechuka.txt", true))
            {
               

                List<purchase_invoice> purchase_invoiceList = db.purchase_invoice.Where(predicate).ToList();
                string Line = string.Empty;
                if (file.BaseStream.Length == 0)
                {
                    Line = "Contact gov code" + "\t" + "contact name" + "\t" + "number" + "\t" + "date";
                    foreach (app_vat_group app_vat_group in db.app_vat_group.ToList())
                    {
                        Line += "\t" + app_vat_group.name;
                    }
                    file.WriteLine(Line);
                }
                foreach (purchase_invoice purchase_invoice in purchase_invoiceList)
                {
                    Line = string.Empty;
                    Line = purchase_invoice.contact.gov_code + "\t" + purchase_invoice.contact.name + "\t" + purchase_invoice.number + "\t" + purchase_invoice.trans_date + "\t" + purchase_invoice.app_condition.name + "\t" + purchase_invoice.app_contract.name;

                    foreach (app_vat_group app_vat_group in db.app_vat_group.ToList())
                    {
                        Line += "\t" + purchase_invoice.purchase_invoice_detail.Where(x => x.id_vat_group == app_vat_group.id_vat_group).Sum(x => x.SubTotal_Vat - x.SubTotal);
                    }
                    file.WriteLine(Line);
                }
                var predicateSalesReturn = PredicateBuilder.True<entity.sales_return>();

                if (ConditionArray != null)
                {
                    if (ConditionArray.Count() > 0)
                    {
                        predicateSalesReturn = predicateSalesReturn.And(x => ConditionArray.Contains(x.app_condition.name));
                    }
                }

                if (ContractArray != null)
                {
                    if (ContractArray.Count() > 0)
                    {
                        predicateSalesReturn = predicateSalesReturn.And(x => ContractArray.Contains(x.app_contract.name));
                    }
                }

                if (start_Range != Convert.ToDateTime("1/1/0001"))
                {
                    predicateSalesReturn = predicateSalesReturn.And(x => x.trans_date >= start_Range);

                }
                if (end_Range != Convert.ToDateTime("1/1/0001"))
                {
                    predicateSalesReturn = predicateSalesReturn.And(x => x.trans_date <= end_Range);

                }
                if (Contact != null)
                {
                    predicateSalesReturn = predicateSalesReturn.And(x => x.contact == Contact);
                }
                List<sales_return> sales_returnList = db.sales_return.Where(predicateSalesReturn).ToList();
                foreach (sales_return sales_return in sales_returnList)
                {
                    Line = string.Empty;
                    Line = sales_return.contact.gov_code + "\t" + sales_return.contact.name + "\t" + sales_return.number + "\t" + sales_return.trans_date + "\t" + sales_return.app_condition.name + "\t" + sales_return.app_contract.name;
                    foreach (app_vat_group app_vat_group in db.app_vat_group.ToList())
                    {
                        Line += "\t" + sales_return.sales_return_detail.Where(x => x.id_vat_group == app_vat_group.id_vat_group).Sum(x => x.SubTotal_Vat - x.SubTotal);
                    }
                    file.WriteLine(Line);
                    file.WriteLine(Line);

                }
                MessageBox.Show("Files Saved...");
            }
        }

        private void grid_sbxContact_Select(object sender, RoutedEventArgs e)
        {
            if (grid_sbxContact.ContactID>0)
            {
                Contact = db.contacts.Where(x => x.id_contact == grid_sbxContact.ContactID).FirstOrDefault();
            }
        }

        private void grid_sbxItem_Select(object sender, RoutedEventArgs e)
        {
            if (grid_sbxItem.ItemID > 0)
            {
                Item = db.items.Where(x => x.id_item == grid_sbxItem.ItemID).FirstOrDefault();
            }
        }


    }
}
