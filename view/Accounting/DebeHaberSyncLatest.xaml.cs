using cntrl;
using Cognitivo.Menu;
using Cognitivo.Properties;
using entity;
using entity.BrilloQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Accounting
{
    public partial class DebeHaberSyncLatest : Page, IDisposable
    {
        DataTable salesdt;
        DataTable salesdetaildt;
        DataTable salesreturndt;
        DataTable salesreturndetaildt;
        DataTable purchasedt;
        DataTable purchasedetaildt;
        DataTable purchasereturndt;
        DataTable purchasereturndetaildt;
        private CollectionViewSource sales_invoiceViewSource;
        private CollectionViewSource sales_returnViewSource;
        private CollectionViewSource purchase_invoiceViewSource;
        private CollectionViewSource purchase_returnViewSource;
        private CollectionViewSource payment_detailViewSource;
        private CollectionViewSource item_assetViewSource;
        private CollectionViewSource production_order_detailViewSource;


        //private db db = new db();
        private dbContext db = new dbContext();

        private string RelationshipHash = string.Empty;
        private string GovCode = string.Empty;

        public DateTime StartDate
        {
            get { return AbsoluteDate.Start(_StartDate); }
            set { _StartDate = value; } //fill(); }
        }

        private DateTime _StartDate = AbsoluteDate.Start(DateTime.Now.AddMonths(-1));

        public DateTime EndDate
        {
            get { return AbsoluteDate.End(_EndDate); }
            set { _EndDate = value; } // fill(); }
        }

        private DateTime _EndDate = AbsoluteDate.End(DateTime.Now);

        public DebeHaberSyncLatest()
        {
            InitializeComponent();

            db.db = new db();

            DatePanel.StartDate = DateTime.Now.AddMonths(-1);
            DatePanel.EndDate = DateTime.Now.Date.AddDays(1).AddTicks(-1);

            sales_invoiceViewSource = FindResource("sales_invoiceViewSource") as CollectionViewSource;
            sales_returnViewSource = FindResource("sales_returnViewSource") as CollectionViewSource;
            purchase_invoiceViewSource = FindResource("purchase_invoiceViewSource") as CollectionViewSource;
            purchase_returnViewSource = FindResource("purchase_returnViewSource") as CollectionViewSource;
            payment_detailViewSource = FindResource("payment_detailViewSource") as CollectionViewSource;
            item_assetViewSource = FindResource("item_assetViewSource") as CollectionViewSource;
            production_order_detailViewSource = FindResource("production_order_detailViewSource") as CollectionViewSource;

            RelationshipHash = db.db.app_company.Where(x => x.id_company == CurrentSession.Id_Company).FirstOrDefault().hash_debehaber;
            GovCode = db.db.app_company.Where(x => x.id_company == CurrentSession.Id_Company).FirstOrDefault().gov_code;
            var timer = new System.Threading.Timer(
                e => btnData_Refresh(null, null),
                null,
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(15));
        }

        private void fill()
        {
            //Dispatcher
            Dispatcher.BeginInvoke((Action)(() =>
            {
                Get_SalesInvoice();
                Get_PurchaseInvoice();
                Get_PurchaseReturnInvoice();
                Get_SalesReturn();
                Get_Payment();
                Get_ProductionExecution();
            }));
            Dispatcher.BeginInvoke((Action)(() => { progBar.IsIndeterminate = false; }));
        }

        private void btnData_Refresh(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => { progBar.IsIndeterminate = true; }));
            Task taskAuth = Task.Factory.StartNew(() => fill());
        }

        #region LoadData

        public void Get_SalesInvoice()
        {
            string query = @" 
  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
  set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
select 0 as IsSelected,
	   
       sales_invoice.id_sales_invoice as id_invoice ,contacts.gov_code as customerTaxID,contacts.name as customerName,app_company.name as supplierName,app_company.gov_code as supplierTaxID,
        app_currency.code as currencyCode,app_currencyfx.buy_value,app_currencyfx.sell_value,app_contract_detail.interval as paymentCondition,Date(sales_invoice.trans_date) as date,sales_invoice.number as number
        ,sales_invoice.comment as comment,sales_invoice.status
											
												
												from sales_invoice 
                                                inner join app_company on sales_invoice.id_company = app_company.id_company
												inner join contacts on sales_invoice.id_contact = contacts.id_contact
												left join app_terminal on sales_invoice.id_terminal = app_terminal.id_terminal
																inner join app_branch on app_branch.id_branch=sales_invoice.id_branch
																inner join app_currencyfx on app_currencyfx.id_currencyfx=sales_invoice.id_currencyfx
																inner join app_currency on app_currency.id_currency=app_currencyfx.id_currency
																inner join app_contract on app_contract.id_contract=sales_invoice.id_contract
                                                                left join app_contract_detail on app_contract_detail.id_contract=app_contract.id_contract
																inner join app_condition on app_condition.id_condition=sales_invoice.id_condition
														 	  where sales_invoice.id_company = @CompanyID and sales_invoice.trans_date between '@startDate' and '@endDate'
                                              order by sales_invoice.trans_date";
            query = query.Replace("@CompanyID", CurrentSession.Id_Company.ToString());
            query = query.Replace("@startDate", DatePanel.StartDate.ToString("yyyy-MM-dd") + " 00:00:00");
            query = query.Replace("@endDate", DatePanel.EndDate.ToString("yyyy-MM-dd") + " 23:59:59");
            salesdt = QueryExecutor.DT(query);

            sales_invoiceViewSource.Source = salesdt;

            string querydetail = @" 
   
  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
  set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
select
	  
sales_invoice_detail.id_sales_invoice_detail as id,
CASE
      WHEN items.id_item_type=1 THEN '" + entity.Brillo.Localize.StringText("Product") + @"'
      WHEN items.id_item_type=2 THEN  '" + entity.Brillo.Localize.StringText("RawMaterial") + @"'
      WHEN items.id_item_type=3 THEN  '" + entity.Brillo.Localize.StringText("Service") + @"'
      WHEN items.id_item_type=4 THEN  '" + entity.Brillo.Localize.StringText("FixedAssets") + @"'
      WHEN items.id_item_type=5 THEN  '" + entity.Brillo.Localize.StringText("Task") + @"'
      WHEN items.id_item_type=6 THEN  '" + entity.Brillo.Localize.StringText("Supplies") + @"'
      WHEN items.id_item_type=7 THEN  '" + entity.Brillo.Localize.StringText("ServiceContract") + @"'
END as chart
,sales_invoice_detail.id_sales_invoice,items.id_item_type,items.id_item,items.description,
												
												sales_invoice_detail.unit_cost as UnitCost,vatco.vat as vat,
												sales_invoice_detail.unit_price as UnitPrice,
                                                vatco.coef as coefficient,
												round(( (sales_invoice_detail.unit_price) * (vatco.coef + 1)),4) as UnitPriceVat,
												round((sales_invoice_detail.quantity * sales_invoice_detail.unit_price),4) as SubTotal,
													round(( (sales_invoice_detail.unit_price) * (vatco.coef + 1)) * sales_invoice_detail.quantity ,2) as value,
												round(sales_invoice_detail.discount, 4) as Discount,
												round((sales_invoice_detail.quantity * (sales_invoice_detail.discount * vatco.coef)),4) as DiscountVat,
												(sales_invoice_detail.quantity * sales_invoice_detail.unit_cost) as SubTotalCost
                                             
                                                
												from sales_invoice_detail
												inner join sales_invoice on sales_invoice_detail.id_sales_invoice=sales_invoice.id_sales_invoice
												left join sales_rep on sales_invoice.id_sales_rep = sales_rep.id_sales_rep
												inner join contacts on sales_invoice.id_contact = contacts.id_contact
												left join app_geography on app_geography.id_geography=contacts.id_geography
												inner join items on sales_invoice_detail.id_item = items.id_item
												left join app_terminal on sales_invoice.id_terminal = app_terminal.id_terminal
													 LEFT OUTER JOIN
															 (SELECT app_vat_group.id_vat_group, SUM(app_vat.coefficient)  AS coef, app_vat_group.name as VAT
																FROM  app_vat_group
																	LEFT OUTER JOIN app_vat_group_details ON app_vat_group.id_vat_group = app_vat_group_details.id_vat_group
																	LEFT OUTER JOIN app_vat ON app_vat_group_details.id_vat = app_vat.id_vat
																GROUP BY app_vat_group.id_vat_group)

																vatco ON vatco.id_vat_group = sales_invoice_detail.id_vat_group
																inner join app_branch on app_branch.id_branch=sales_invoice.id_branch
																inner join app_currencyfx on app_currencyfx.id_currencyfx=sales_invoice.id_currencyfx
																inner join app_currency on app_currency.id_currency=app_currencyfx.id_currency
																inner join app_contract on app_contract.id_contract=sales_invoice.id_contract
																inner join app_condition on app_condition.id_condition=sales_invoice.id_condition
																left join projects on projects.id_project=sales_invoice.id_project
											  where sales_invoice.id_company = @CompanyID and sales_invoice.trans_date between '@startDate' and '@endDate'
                                              order by sales_invoice.trans_date";
            querydetail = querydetail.Replace("@CompanyID", CurrentSession.Id_Company.ToString());
            querydetail = querydetail.Replace("@startDate", DatePanel.StartDate.ToString("yyyy-MM-dd") + " 00:00:00");
            querydetail = querydetail.Replace("@endDate", DatePanel.EndDate.ToString("yyyy-MM-dd") + " 23:59:59");
            salesdetaildt = QueryExecutor.DT(querydetail);



        }

        public async void Get_Payment()
        {
            //x.Is Head replace with Is_Accounted = True.
            payment_detailViewSource.Source = await db.db.payment_detail.Where(x =>
                x.payment.id_company == CurrentSession.Id_Company &&
                x.trans_date >= DatePanel.StartDate && x.trans_date <= DatePanel.EndDate &&
                x.payment.is_accounted == false &&
                x.payment.status == Status.Documents_General.Approved).Include(x => x.payment).ToListAsync();
        }

        public void Get_SalesReturn()
        {
            string query = @" 
  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
  set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
select 0 as IsSelected,
	   
       sales_return.id_sales_return as id_invoice ,contacts.gov_code as customerTaxID,contacts.name as customerName,app_company.name as supplierName,app_company.gov_code as supplierTaxID,
        app_currency.code as currencyCode,app_currencyfx.buy_value,app_currencyfx.sell_value,app_contract_detail.interval as paymentCondition,Date(sales_return.trans_date) as date,sales_return.number as number
        ,sales_return.comment as comment,sales_return.status
											
												
												from sales_return 
                                                inner join app_company on sales_return.id_company = app_company.id_company
												inner join contacts on sales_return.id_contact = contacts.id_contact
												left join app_terminal on sales_return.id_terminal = app_terminal.id_terminal
																inner join app_branch on app_branch.id_branch=sales_return.id_branch
																inner join app_currencyfx on app_currencyfx.id_currencyfx=sales_return.id_currencyfx
																inner join app_currency on app_currency.id_currency=app_currencyfx.id_currency
																inner join app_contract on app_contract.id_contract=sales_return.id_contract
                                                                left join app_contract_detail on app_contract_detail.id_contract=app_contract.id_contract
																inner join app_condition on app_condition.id_condition=sales_return.id_condition
														 	  where sales_return.id_company = @CompanyID and sales_return.trans_date between '@startDate' and '@endDate'
                                              order by sales_return.trans_date";
            query = query.Replace("@CompanyID", CurrentSession.Id_Company.ToString());
            query = query.Replace("@startDate", DatePanel.StartDate.ToString("yyyy-MM-dd") + " 00:00:00");
            query = query.Replace("@endDate", DatePanel.EndDate.ToString("yyyy-MM-dd") + " 23:59:59");
            salesreturndt = QueryExecutor.DT(query);

            sales_returnViewSource.Source = salesreturndt;

            string querydetail = @" 
   
  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
  set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
select
	  
sales_return_detail.id_sales_return_detail as id,
CASE
      WHEN items.id_item_type=1 THEN '" + entity.Brillo.Localize.StringText("Product") + @"'
      WHEN items.id_item_type=2 THEN  '" + entity.Brillo.Localize.StringText("RawMaterial") + @"'
      WHEN items.id_item_type=3 THEN  '" + entity.Brillo.Localize.StringText("Service") + @"'
      WHEN items.id_item_type=4 THEN  '" + entity.Brillo.Localize.StringText("FixedAssets") + @"'
      WHEN items.id_item_type=5 THEN  '" + entity.Brillo.Localize.StringText("Task") + @"'
      WHEN items.id_item_type=6 THEN  '" + entity.Brillo.Localize.StringText("Supplies") + @"'
      WHEN items.id_item_type=7 THEN  '" + entity.Brillo.Localize.StringText("ServiceContract") + @"'
END as chart,
sales_return_detail.id_sales_return,items.id_item_type,items.id_item,items.description,
												
												sales_return_detail.unit_cost as UnitCost,vatco.vat as vat,
												sales_return_detail.unit_price as UnitPrice,
                                                vatco.coef as coefficient,
												round(( (sales_return_detail.unit_price) * (vatco.coef + 1)),4) as UnitPriceVat,
												round((sales_return_detail.quantity * sales_return_detail.unit_price),4) as SubTotal,
													round(( (sales_return_detail.unit_price) * (vatco.coef + 1)) * sales_return_detail.quantity ,2) as value,
												round(sales_return_detail.discount, 4) as Discount,
												round((sales_return_detail.quantity * (sales_return_detail.discount * vatco.coef)),4) as DiscountVat,
												(sales_return_detail.quantity * sales_return_detail.unit_cost) as SubTotalCost
                                             
                                                
												from sales_return_detail
												inner join sales_return on sales_return_detail.id_sales_return=sales_return.id_sales_return
												left join sales_rep on sales_return.id_sales_rep = sales_rep.id_sales_rep
												inner join contacts on sales_return.id_contact = contacts.id_contact
												left join app_geography on app_geography.id_geography=contacts.id_geography
												inner join items on sales_return_detail.id_item = items.id_item
												left join app_terminal on sales_return.id_terminal = app_terminal.id_terminal
													 LEFT OUTER JOIN
															 (SELECT app_vat_group.id_vat_group, SUM(app_vat.coefficient)  AS coef, app_vat_group.name as VAT
																FROM  app_vat_group
																	LEFT OUTER JOIN app_vat_group_details ON app_vat_group.id_vat_group = app_vat_group_details.id_vat_group
																	LEFT OUTER JOIN app_vat ON app_vat_group_details.id_vat = app_vat.id_vat
																GROUP BY app_vat_group.id_vat_group)

																vatco ON vatco.id_vat_group = sales_return_detail.id_vat_group
																inner join app_branch on app_branch.id_branch=sales_return.id_branch
																inner join app_currencyfx on app_currencyfx.id_currencyfx=sales_return.id_currencyfx
																inner join app_currency on app_currency.id_currency=app_currencyfx.id_currency
																inner join app_contract on app_contract.id_contract=sales_return.id_contract
																inner join app_condition on app_condition.id_condition=sales_return.id_condition
																left join projects on projects.id_project=sales_return.id_project
											  where sales_return.id_company = @CompanyID and sales_return.trans_date between '@startDate' and '@endDate'
                                              order by sales_return.trans_date";
            querydetail = querydetail.Replace("@CompanyID", CurrentSession.Id_Company.ToString());
            querydetail = querydetail.Replace("@startDate", DatePanel.StartDate.ToString("yyyy-MM-dd") + " 00:00:00");
            querydetail = querydetail.Replace("@endDate", DatePanel.EndDate.ToString("yyyy-MM-dd") + " 23:59:59");
            salesreturndetaildt = QueryExecutor.DT(querydetail);
        }

        public void Get_PurchaseReturnInvoice()
        {
            //x.Is Head replace with Is_Accounted = True.
            string query = @" 
  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
  set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
select 0 as IsSelected,
	   
       purchase_return.id_purchase_return as id_invoice,contacts.gov_code as supplierTaxID,contacts.name as SupplierName,app_company.name as customerName,app_company.gov_code as customerTaxID,
        app_currency.code as currencyCode,app_currencyfx.buy_value,app_currencyfx.sell_value,app_contract_detail.interval as paymentCondition,Date(purchase_return.trans_date) as date,purchase_return.number as number
        ,purchase_return.comment as comment,purchase_return.status
											
												
												from purchase_return 
                                                inner join app_company on purchase_return.id_company = app_company.id_company
												inner join contacts on purchase_return.id_contact = contacts.id_contact
												left join app_terminal on purchase_return.id_terminal = app_terminal.id_terminal
																inner join app_branch on app_branch.id_branch=purchase_return.id_branch
																inner join app_currencyfx on app_currencyfx.id_currencyfx=purchase_return.id_currencyfx
																inner join app_currency on app_currency.id_currency=app_currencyfx.id_currency
																inner join app_contract on app_contract.id_contract=purchase_return.id_contract
                                                                left join app_contract_detail on app_contract_detail.id_contract=app_contract.id_contract
																inner join app_condition on app_condition.id_condition=purchase_return.id_condition
														 	  where purchase_return.id_company = @CompanyID and purchase_return.trans_date between '@startDate' and '@endDate'
                                              order by purchase_return.trans_date";
            query = query.Replace("@CompanyID", CurrentSession.Id_Company.ToString());
            query = query.Replace("@startDate", DatePanel.StartDate.ToString("yyyy-MM-dd") + " 00:00:00");
            query = query.Replace("@endDate", DatePanel.EndDate.ToString("yyyy-MM-dd") + " 23:59:59");
            purchasereturndt = QueryExecutor.DT(query);

            purchase_returnViewSource.Source = purchasereturndt;

            string querydetail = @" 
   
  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
  set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
select 
	  
purchase_return_detail.id_purchase_return_detail as id,
CASE
      WHEN items.id_item_type=1 THEN '" + entity.Brillo.Localize.StringText("Product") + @"'
      WHEN items.id_item_type=2 THEN  '" + entity.Brillo.Localize.StringText("RawMaterial") + @"'
      WHEN items.id_item_type=3 THEN  '" + entity.Brillo.Localize.StringText("Service") + @"'
      WHEN items.id_item_type=4 THEN  '" + entity.Brillo.Localize.StringText("FixedAssets") + @"'
      WHEN items.id_item_type=5 THEN  '" + entity.Brillo.Localize.StringText("Task") + @"'
      WHEN items.id_item_type=6 THEN  '" + entity.Brillo.Localize.StringText("Supplies") + @"'
      WHEN items.id_item_type=7 THEN  '" + entity.Brillo.Localize.StringText("ServiceContract") + @"'
END as chart
,purchase_return_detail.id_purchase_return,
items.id_item_type,items.id_item,items.description,
												
												purchase_return_detail.unit_cost as UnitCost,vatco.vat as vat,
											    vatco.coef as coefficient,
												round(( (purchase_return_detail.unit_cost) * (vatco.coef + 1)),4) as UnitPriceVat,
												round((purchase_return_detail.quantity * purchase_return_detail.unit_cost),4) as SubTotal,
												round(( (purchase_return_detail.unit_cost) * (vatco.coef + 1)) * purchase_return_detail.quantity ,2) as value,
												round(purchase_return_detail.discount, 4) as Discount,
												round((purchase_return_detail.quantity * (purchase_return_detail.discount * vatco.coef)),4) as DiscountVat,
												(purchase_return_detail.quantity * purchase_return_detail.unit_cost) as SubTotalCost
                                             
                                                
												from purchase_return_detail
												inner join purchase_return on purchase_return_detail.id_purchase_return=purchase_return.id_purchase_return
												inner join contacts on purchase_return.id_contact = contacts.id_contact
												left join app_geography on app_geography.id_geography=contacts.id_geography
												inner join items on purchase_return_detail.id_item = items.id_item
												left join app_terminal on purchase_return.id_terminal = app_terminal.id_terminal
													 LEFT OUTER JOIN
															 (SELECT app_vat_group.id_vat_group, SUM(app_vat.coefficient)  AS coef, app_vat_group.name as VAT
																FROM  app_vat_group
																	LEFT OUTER JOIN app_vat_group_details ON app_vat_group.id_vat_group = app_vat_group_details.id_vat_group
																	LEFT OUTER JOIN app_vat ON app_vat_group_details.id_vat = app_vat.id_vat
																GROUP BY app_vat_group.id_vat_group)

																vatco ON vatco.id_vat_group = purchase_return_detail.id_vat_group
																inner join app_branch on app_branch.id_branch=purchase_return.id_branch
																inner join app_currencyfx on app_currencyfx.id_currencyfx=purchase_return.id_currencyfx
																inner join app_currency on app_currency.id_currency=app_currencyfx.id_currency
																inner join app_contract on app_contract.id_contract=purchase_return.id_contract
																inner join app_condition on app_condition.id_condition=purchase_return.id_condition
																left join projects on projects.id_project=purchase_return.id_project
											  where purchase_return.id_company = @CompanyID and purchase_return.trans_date between '@startDate' and '@endDate'
                                              order by purchase_return.trans_date";
            querydetail = querydetail.Replace("@CompanyID", CurrentSession.Id_Company.ToString());
            querydetail = querydetail.Replace("@startDate", DatePanel.StartDate.ToString("yyyy-MM-dd") + " 00:00:00");
            querydetail = querydetail.Replace("@endDate", DatePanel.EndDate.ToString("yyyy-MM-dd") + " 23:59:59");
            purchasereturndetaildt = QueryExecutor.DT(querydetail);
        }

        public void Get_PurchaseInvoice()
        {
            string query = @" 
  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
  set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
select 0 as IsSelected,
	   
       purchase_invoice.id_purchase_invoice as id_invoice,contacts.gov_code as supplierTaxID,contacts.name as SupplierName,app_company.name as customerName,app_company.gov_code as customerTaxID,
        app_currency.code as currencyCode,app_currencyfx.buy_value,app_currencyfx.sell_value,app_contract_detail.interval as paymentCondition,Date(purchase_invoice.trans_date) as date,purchase_invoice.number as number
        ,purchase_invoice.comment as comment,purchase_invoice.status
											
												
												from purchase_invoice 
                                                inner join app_company on purchase_invoice.id_company = app_company.id_company
												inner join contacts on purchase_invoice.id_contact = contacts.id_contact
												left join app_terminal on purchase_invoice.id_terminal = app_terminal.id_terminal
																inner join app_branch on app_branch.id_branch=purchase_invoice.id_branch
																inner join app_currencyfx on app_currencyfx.id_currencyfx=purchase_invoice.id_currencyfx
																inner join app_currency on app_currency.id_currency=app_currencyfx.id_currency
																inner join app_contract on app_contract.id_contract=purchase_invoice.id_contract
                                                                left join app_contract_detail on app_contract_detail.id_contract=app_contract.id_contract
																inner join app_condition on app_condition.id_condition=purchase_invoice.id_condition
														 	  where purchase_invoice.id_company = @CompanyID and purchase_invoice.trans_date between '@startDate' and '@endDate'
                                              order by purchase_invoice.trans_date";
            query = query.Replace("@CompanyID", CurrentSession.Id_Company.ToString());
            query = query.Replace("@startDate", DatePanel.StartDate.ToString("yyyy-MM-dd") + " 00:00:00");
            query = query.Replace("@endDate", DatePanel.EndDate.ToString("yyyy-MM-dd") + " 23:59:59");
            purchasedt = QueryExecutor.DT(query);

            purchase_invoiceViewSource.Source = purchasedt;

            string querydetail = @" 
   
  set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
  set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
select
	  
purchase_invoice_detail.id_purchase_invoice_detail as id,
CASE
      WHEN items.id_item_type=1 THEN '" + entity.Brillo.Localize.StringText("Product") + @"'
      WHEN items.id_item_type=2 THEN  '" + entity.Brillo.Localize.StringText("RawMaterial") + @"'
      WHEN items.id_item_type=3 THEN  '" + entity.Brillo.Localize.StringText("Service") + @"'
      WHEN items.id_item_type=4 THEN  '" + entity.Brillo.Localize.StringText("FixedAssets") + @"'
      WHEN items.id_item_type=5 THEN  '" + entity.Brillo.Localize.StringText("Task") + @"'
      WHEN items.id_item_type=6 THEN  '" + entity.Brillo.Localize.StringText("Supplies") + @"'
      WHEN items.id_item_type=7 THEN  '" + entity.Brillo.Localize.StringText("ServiceContract") + @"'
END as chart
,purchase_invoice_detail.id_purchase_invoice,
items.id_item_type,items.id_item,items.description,
												
												purchase_invoice_detail.unit_cost as UnitCost,vatco.vat as vat,
											    vatco.coef as coefficient,
												round(( (purchase_invoice_detail.unit_cost) * (vatco.coef + 1)),4) as UnitPriceVat,
												round((purchase_invoice_detail.quantity * purchase_invoice_detail.unit_cost),4) as SubTotal,
												round(( (purchase_invoice_detail.unit_cost) * (vatco.coef + 1)) * purchase_invoice_detail.quantity ,2) as value,
												round(purchase_invoice_detail.discount, 4) as Discount,
												round((purchase_invoice_detail.quantity * (purchase_invoice_detail.discount * vatco.coef)),4) as DiscountVat,
												(purchase_invoice_detail.quantity * purchase_invoice_detail.unit_cost) as SubTotalCost
                                             
                                                
												from purchase_invoice_detail
												inner join purchase_invoice on purchase_invoice_detail.id_purchase_invoice=purchase_invoice.id_purchase_invoice
												inner join contacts on purchase_invoice.id_contact = contacts.id_contact
												left join app_geography on app_geography.id_geography=contacts.id_geography
												inner join items on purchase_invoice_detail.id_item = items.id_item
												left join app_terminal on purchase_invoice.id_terminal = app_terminal.id_terminal
													 LEFT OUTER JOIN
															 (SELECT app_vat_group.id_vat_group, SUM(app_vat.coefficient)  AS coef, app_vat_group.name as VAT
																FROM  app_vat_group
																	LEFT OUTER JOIN app_vat_group_details ON app_vat_group.id_vat_group = app_vat_group_details.id_vat_group
																	LEFT OUTER JOIN app_vat ON app_vat_group_details.id_vat = app_vat.id_vat
																GROUP BY app_vat_group.id_vat_group)

																vatco ON vatco.id_vat_group = purchase_invoice_detail.id_vat_group
																inner join app_branch on app_branch.id_branch=purchase_invoice.id_branch
																inner join app_currencyfx on app_currencyfx.id_currencyfx=purchase_invoice.id_currencyfx
																inner join app_currency on app_currency.id_currency=app_currencyfx.id_currency
																inner join app_contract on app_contract.id_contract=purchase_invoice.id_contract
																inner join app_condition on app_condition.id_condition=purchase_invoice.id_condition
																left join projects on projects.id_project=purchase_invoice.id_project
											  where purchase_invoice.id_company = @CompanyID and purchase_invoice.trans_date between '@startDate' and '@endDate'
                                              order by purchase_invoice.trans_date";
            querydetail = querydetail.Replace("@CompanyID", CurrentSession.Id_Company.ToString());
            querydetail = querydetail.Replace("@startDate", DatePanel.StartDate.ToString("yyyy-MM-dd") + " 00:00:00");
            querydetail = querydetail.Replace("@endDate", DatePanel.EndDate.ToString("yyyy-MM-dd") + " 23:59:59");
            purchasedetaildt = QueryExecutor.DT(querydetail);
        }

        private async void Get_ItemAsset()
        {
            await db.db.item_asset.Where(x =>
                x.id_company == CurrentSession.Id_Company &&
                x.item.is_active == true).ToListAsync();
            item_assetViewSource.Source = db.db.item_asset.Local;
        }

        private async void Get_ProductionExecution()
        {
            //If we bring only low level items, it's easy to calculate the higher level items.
            await db.db.production_order.Where
                (x =>
                x.id_company == CurrentSession.Id_Company
                //x.production_order_detail.Where(y => y.product == false).Count() > 0
                //x.child.Count() == 0
                )
                .Include(z => z.production_order_detail)
                //.Include(a => a.production_execution_detail)
                .ToListAsync();

            production_order_detailViewSource.Source = db.db.production_order.Local;
        }

        #endregion LoadData

        private void btnData_Sync(object sender, RoutedEventArgs e)
        {
            //DebeHaberLogIn DebeHaberLogIn = new DebeHaberLogIn();
            //try
            //{
            //    DebeHaberLogIn.check_api(RelationshipHash, GovCode);
            //}
            //catch (Exception)
            //{

            //    return;
            //}



            //Loops through each set of data and syncs each record individually.
            Sales_Sync();
            Purchase_Sync();
            SalesReturn_Sync();
            PurchaseReturn_Sync();
            PaymentSync();
            Production_Sync();
        }

        #region Sales Sync

        private void Sales_Sync()
        {
            DebeHaber.SyncLatest.Integration Integration = new DebeHaber.SyncLatest.Integration()
            {
                Key = RelationshipHash,
                GovCode = GovCode
            };

            if (salesdt != null && salesdt.Select("IsSelected=1").Count()>0)
            {

                
                salesdt = salesdt.Select("IsSelected=1").CopyToDataTable();

                if (salesdt.Rows.Count > 0)
                {
                    //Loop through
                    foreach (DataRow sales_invoice in salesdt.Rows)
                    {

                        DebeHaber.SyncLatest.Transaction Transaction = new DebeHaber.SyncLatest.Transaction();
                        DebeHaber.SyncLatest.Commercial_Invoice Sales = new DebeHaber.SyncLatest.Commercial_Invoice();
                        //Loads Data from Sales
                        Sales.Fill_By(sales_invoice, DebeHaber.SyncLatest.TransactionTypes.Sales);


                        if (salesdetaildt.Select("id_sales_invoice=" + sales_invoice["id_invoice"].ToString()).Count() > 0)
                        {
                            DataTable dtdetail = salesdetaildt.Select("id_sales_invoice=" + sales_invoice["id_invoice"].ToString()).CopyToDataTable();
                            foreach (DataRow Detail in dtdetail.Rows)
                            {
                                DebeHaber.SyncLatest.CommercialInvoice_Detail CommercialInvoice_Detail = new DebeHaber.SyncLatest.CommercialInvoice_Detail();
                                //Fill and Detail SalesDetail
                                CommercialInvoice_Detail.Fill_By(Detail);
                                Sales.CommercialInvoice_Detail.Add(CommercialInvoice_Detail);
                            }

                        }

                        //Loop through payments made.


                        Transaction.Commercial_Invoices.Add(Sales);
                        Integration.Transactions.Add(Transaction);


                    }
                    int Count = Integration.Transactions.Count;
                    int PageSize = 100;
                    int PageCount = (Count / PageSize) < 1 ? 1 : (int)Math.Ceiling((decimal)Count / PageSize);

                    for (int PageIndex = 0; PageIndex < PageCount; PageIndex++)
                    {

                        try
                        {
                            List<DebeHaber.SyncLatest.Transaction> trans = Integration.Transactions.Skip(PageIndex * PageSize).Take(PageSize).ToList();
                            var Sales_Json = new JavaScriptSerializer() { MaxJsonLength = 86753090 }.Serialize(trans);

                            var obj = Send2API(Sales_Json);

                            List<DebeHaber.SyncLatest.Web_Data> sales_json = new JavaScriptSerializer().Deserialize<List<DebeHaber.SyncLatest.Web_Data>>(obj.ToString());
                            using (db db = new db())
                            {
                                foreach (DebeHaber.SyncLatest.Web_Data data in sales_json)
                                {

                                    sales_invoice sales = db.sales_invoice.Where(x => x.id_sales_invoice == data.ref_id).FirstOrDefault();
                                    sales.cloud_id = data.id;
                                }
                                db.SaveChanges();
                            }


                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("403"))
                            {


                                DebeHaberLogIn page = new DebeHaberLogIn();
                                MainWindow rootWindow = Window.GetWindow(this) as MainWindow;

                                rootWindow.mainFrame.Navigate(page);


                            }
                            else
                            {
                                if (MessageBox.Show("Error. Would you like to save the file for analysis?", "", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                                {
                                    MessageBox.Show(ex.Message, "Error Message");
                                    // Class.ErrorLog.DebeHaber(new JavaScriptSerializer().Serialize(Integration).ToString());
                                }
                            }

                            //Error Sales Invoice keep Is Accounted to False.

                        }
                        finally
                        {
                            db.db.SaveChanges();
                            fill();
                        }
                    }
                }
            }


        }

        #endregion Sales Sync

        #region Purchase Sync

        private void Purchase_Sync()
        {
            DebeHaber.SyncLatest.Integration Integration = new DebeHaber.SyncLatest.Integration()
            {
                Key = RelationshipHash,
                GovCode = GovCode
            };

            if (purchasedt != null && purchasedt.Select("IsSelected=1").Count() > 0)
            {
                purchasedt = purchasedt.Select("IsSelected=1").CopyToDataTable();
                if (purchasedt.Rows.Count > 0)
                {
                    //Loop through
                    foreach (DataRow purchase_invoice in purchasedt.Rows)
                    {

                        DebeHaber.SyncLatest.Transaction Transaction = new DebeHaber.SyncLatest.Transaction();
                        DebeHaber.SyncLatest.Commercial_Invoice Purchase = new DebeHaber.SyncLatest.Commercial_Invoice();
                        //Loads Data from Sales
                        Purchase.Fill_By(purchase_invoice, DebeHaber.SyncLatest.TransactionTypes.Purchase);


                        if (purchasedetaildt.Select("id_purchase_invoice=" + purchase_invoice["id_invoice"].ToString()).Count() > 0)
                        {
                            DataTable dtdetail = purchasedetaildt.Select("id_purchase_invoice=" + purchase_invoice["id_invoice"].ToString()).CopyToDataTable();
                            foreach (DataRow Detail in dtdetail.Rows)
                            {
                                DebeHaber.SyncLatest.CommercialInvoice_Detail CommercialInvoice_Detail = new DebeHaber.SyncLatest.CommercialInvoice_Detail();
                                //Fill and Detail SalesDetail
                                CommercialInvoice_Detail.Fill_By(Detail);
                                Purchase.CommercialInvoice_Detail.Add(CommercialInvoice_Detail);
                            }

                        }

                        //Loop through payments made.


                        Transaction.Commercial_Invoices.Add(Purchase);
                        Integration.Transactions.Add(Transaction);


                    }
                    int Count = Integration.Transactions.Count;
                    int PageSize = 100;
                    int PageCount = (Count / PageSize) < 1 ? 1 : (int)Math.Ceiling((decimal)Count / PageSize);

                    for (int PageIndex = 0; PageIndex < PageCount; PageIndex++)
                    {

                        try
                        {
                            List<DebeHaber.SyncLatest.Transaction> trans = Integration.Transactions.Skip(PageIndex * PageSize).Take(PageSize).ToList();
                            var Purchase_Json = new JavaScriptSerializer() { MaxJsonLength = 86753090 }.Serialize(trans);

                            var obj = Send2API(Purchase_Json);

                            List<DebeHaber.SyncLatest.Web_Data> purchase_json = new JavaScriptSerializer().Deserialize<List<DebeHaber.SyncLatest.Web_Data>>(obj.ToString());
                            using (db db = new db())
                            {
                                foreach (DebeHaber.SyncLatest.Web_Data data in purchase_json)
                                {

                                    purchase_invoice purchase = db.purchase_invoice.Where(x => x.id_purchase_invoice == data.ref_id).FirstOrDefault();
                                    purchase.cloud_id = data.id;
                                }
                                db.SaveChanges();
                            }


                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("403"))
                            {


                                DebeHaberLogIn page = new DebeHaberLogIn();
                                MainWindow rootWindow = Window.GetWindow(this) as MainWindow;

                                rootWindow.mainFrame.Navigate(page);


                            }
                            else
                            {
                                if (MessageBox.Show("Error. Would you like to save the file for analysis?", "", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                                {
                                    MessageBox.Show(ex.Message, "Error Message");
                                    // Class.ErrorLog.DebeHaber(new JavaScriptSerializer().Serialize(Integration).ToString());
                                }
                            }

                            //Error Sales Invoice keep Is Accounted to False.

                        }
                        finally
                        {
                            db.db.SaveChanges();
                            fill();
                        }
                    }

                }
            }
        }

        #endregion Purchase Sync

        #region SalesReturn Sync

        private void SalesReturn_Sync()
        {

            DebeHaber.SyncLatest.Integration Integration = new DebeHaber.SyncLatest.Integration()
            {
                Key = RelationshipHash,
                GovCode = GovCode
            };

            if (salesreturndt != null && salesreturndt.Select("IsSelected=1").Count() > 0)
            {
                salesreturndt = salesreturndt.Select("IsSelected=1").CopyToDataTable();
                if (salesreturndt.Rows.Count > 0)
                {



                    //Loop through
                    foreach (DataRow sales_return in salesreturndt.Rows)
                    {

                        DebeHaber.SyncLatest.Transaction Transaction = new DebeHaber.SyncLatest.Transaction();
                        DebeHaber.SyncLatest.Commercial_Invoice SalesReturn = new DebeHaber.SyncLatest.Commercial_Invoice();
                        //Loads Data from Sales
                        SalesReturn.Fill_By(sales_return, DebeHaber.SyncLatest.TransactionTypes.SalesReturn);


                        if (salesreturndetaildt.Select("id_sales_return=" + sales_return["id_invoice"].ToString()).Count() > 0)
                        {
                            DataTable dtdetail = salesreturndetaildt.Select("id_sales_return=" + sales_return["id_invoice"].ToString()).CopyToDataTable();
                            foreach (DataRow Detail in dtdetail.Rows)
                            {
                                DebeHaber.SyncLatest.CommercialInvoice_Detail CommercialInvoice_Detail = new DebeHaber.SyncLatest.CommercialInvoice_Detail();
                                //Fill and Detail SalesDetail
                                CommercialInvoice_Detail.Fill_By(Detail);
                                SalesReturn.CommercialInvoice_Detail.Add(CommercialInvoice_Detail);
                            }

                        }

                        //Loop through payments made.


                        Transaction.Commercial_Invoices.Add(SalesReturn);
                        Integration.Transactions.Add(Transaction);


                    }
                    int Count = Integration.Transactions.Count;
                    int PageSize = 100;
                    int PageCount = (Count / PageSize) < 1 ? 1 : (int)Math.Ceiling((decimal)Count / PageSize);

                    for (int PageIndex = 0; PageIndex < PageCount; PageIndex++)
                    {

                        try
                        {
                            List<DebeHaber.SyncLatest.Transaction> trans = Integration.Transactions.Skip(PageIndex * PageSize).Take(PageSize).ToList();
                            var Sales_Return_Json = new JavaScriptSerializer() { MaxJsonLength = 86753090 }.Serialize(trans);

                            var obj = Send2API(Sales_Return_Json);

                            List<DebeHaber.SyncLatest.Web_Data> sales_return_json = new JavaScriptSerializer().Deserialize<List<DebeHaber.SyncLatest.Web_Data>>(obj.ToString());
                            using (db db = new db())
                            {
                                foreach (DebeHaber.SyncLatest.Web_Data data in sales_return_json)
                                {

                                    sales_return sales_return = db.sales_return.Where(x => x.id_sales_return == data.ref_id).FirstOrDefault();
                                    sales_return.cloud_id = data.id;
                                }
                                db.SaveChanges();
                            }


                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("403"))
                            {


                                DebeHaberLogIn page = new DebeHaberLogIn();
                                MainWindow rootWindow = Window.GetWindow(this) as MainWindow;

                                rootWindow.mainFrame.Navigate(page);


                            }
                            else
                            {
                                if (MessageBox.Show("Error. Would you like to save the file for analysis?", "", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                                {
                                    MessageBox.Show(ex.Message, "Error Message");
                                    // Class.ErrorLog.DebeHaber(new JavaScriptSerializer().Serialize(Integration).ToString());
                                }
                            }

                            //Error Sales Invoice keep Is Accounted to False.

                        }
                        finally
                        {
                            db.db.SaveChanges();
                            fill();
                        }
                    }
                }
            }
        }

        #endregion SalesReturn Sync

        #region PurchaseReturn Sync

        private void PurchaseReturn_Sync()
        {
            DebeHaber.SyncLatest.Integration Integration = new DebeHaber.SyncLatest.Integration()
            {
                Key = RelationshipHash,
                GovCode = GovCode
            };

            if (purchasereturndt != null && purchasereturndt.Select("IsSelected=1").Count() > 0)
            {
                purchasereturndt = purchasereturndt.Select("IsSelected=1").CopyToDataTable();
                if (purchasereturndt.Rows.Count > 0)
                {

                    //Loop through
                    foreach (DataRow purchase_return in purchasereturndt.Rows)
                    {

                        DebeHaber.SyncLatest.Transaction Transaction = new DebeHaber.SyncLatest.Transaction();
                        DebeHaber.SyncLatest.Commercial_Invoice PurchaseReturn = new DebeHaber.SyncLatest.Commercial_Invoice();
                        //Loads Data from Sales
                        PurchaseReturn.Fill_By(purchase_return, DebeHaber.SyncLatest.TransactionTypes.PurchaseReturn);


                        if (purchasereturndetaildt.Select("id_purchase_return=" + purchase_return["id_invoice"].ToString()).Count() > 0)
                        {
                            DataTable dtdetail = purchasereturndetaildt.Select("id_purchase_return=" + purchase_return["id_invoice"].ToString()).CopyToDataTable();
                            foreach (DataRow Detail in dtdetail.Rows)
                            {
                                DebeHaber.SyncLatest.CommercialInvoice_Detail CommercialInvoice_Detail = new DebeHaber.SyncLatest.CommercialInvoice_Detail();
                                //Fill and Detail SalesDetail
                                CommercialInvoice_Detail.Fill_By(Detail);
                                PurchaseReturn.CommercialInvoice_Detail.Add(CommercialInvoice_Detail);
                            }

                        }

                        //Loop through payments made.


                        Transaction.Commercial_Invoices.Add(PurchaseReturn);
                        Integration.Transactions.Add(Transaction);


                    }
                    int Count = Integration.Transactions.Count;
                    int PageSize = 100;
                    int PageCount = (Count / PageSize) < 1 ? 1 : (int)Math.Ceiling((decimal)Count / PageSize);

                    for (int PageIndex = 0; PageIndex < PageCount; PageIndex++)
                    {

                        try
                        {
                            List<DebeHaber.SyncLatest.Transaction> trans = Integration.Transactions.Skip(PageIndex * PageSize).Take(PageSize).ToList();
                            var Purchase_Json = new JavaScriptSerializer() { MaxJsonLength = 86753090 }.Serialize(trans);

                            var obj = Send2API(Purchase_Json);

                            List<DebeHaber.SyncLatest.Web_Data> purchase_return_json = new JavaScriptSerializer().Deserialize<List<DebeHaber.SyncLatest.Web_Data>>(obj.ToString());
                            using (db db = new db())
                            {
                                foreach (DebeHaber.SyncLatest.Web_Data data in purchase_return_json)
                                {

                                    purchase_return purchase_return = db.purchase_return.Where(x => x.id_purchase_return == data.ref_id).FirstOrDefault();
                                    purchase_return.cloud_id = data.id;
                                }
                                db.SaveChanges();
                            }


                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("403"))
                            {


                                DebeHaberLogIn page = new DebeHaberLogIn();
                                MainWindow rootWindow = Window.GetWindow(this) as MainWindow;

                                rootWindow.mainFrame.Navigate(page);


                            }
                            else
                            {
                                if (MessageBox.Show("Error. Would you like to save the file for analysis?", "", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                                {
                                    MessageBox.Show(ex.Message, "Error Message");
                                    // Class.ErrorLog.DebeHaber(new JavaScriptSerializer().Serialize(Integration).ToString());
                                }
                            }

                            //Error Sales Invoice keep Is Accounted to False.

                        }
                        finally
                        {
                            db.db.SaveChanges();
                            fill();
                        }
                    }
                }
            }
        }

        #endregion PurchaseReturn Sync

        #region Payment Sync

        private void PaymentSync()
        {
            List<payment_detail> PaymentList = db.db.payment_detail.Local.Where(x => x.IsSelected && x.payment.is_accounted == false).ToList();

            //Loop through
            foreach (payment_detail payment_detail in PaymentList)
            {
                DebeHaber.Integration Integration = new DebeHaber.Integration();
                Integration.Key = RelationshipHash;
                Integration.GovCode = GovCode;
                DebeHaber.Transaction Transaction = new DebeHaber.Transaction();

                DebeHaber.Payments Payment = new DebeHaber.Payments();

                //Loads Data from Sales
                payment_schedual schedual = db.db.payment_schedual.Where(x => x.id_payment_detail == payment_detail.id_payment_detail).FirstOrDefault();
                Payment.FillPayments(schedual);

                Transaction.Payments.Add(Payment);
                Integration.Transactions.Add(Transaction);

                try
                {
                    var Sales_Json = new JavaScriptSerializer().Serialize(Integration);
                    Send2API(Sales_Json);

                    payment_detail.IsSelected = false;
                    payment_detail.payment.is_accounted = true;
                }
                catch (Exception ex)
                {

                    if (ex.Message.Contains("403"))
                    {


                        DebeHaberLogIn page = new DebeHaberLogIn();
                        MainWindow rootWindow = Window.GetWindow(this) as MainWindow;

                        rootWindow.mainFrame.Navigate(page);


                    }
                    else
                    {
                        if (MessageBox.Show("Error. Would you like to save the file for analysis?", "", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                        {
                            MessageBox.Show(ex.Message, "Error Message");
                            Class.ErrorLog.DebeHaber(new JavaScriptSerializer().Serialize(Integration).ToString());
                        }
                    }

                    //Error Sales Invoice keep Is Accounted to False.
                    payment_detail.payment.is_accounted = false;
                }
                finally
                {
                    db.db.SaveChanges();
                    fill();
                }
            }
        }

        #endregion Payment Sync

        #region Production_Sync

        private void Production_Sync()
        {
            List<production_order> OrderList = db.db.production_order.Local.Where(x => x.IsSelected).ToList();

            foreach (production_order ProductionOrder in OrderList)
            {
                DebeHaber.Integration Integration = new DebeHaber.Integration();
                Integration.Key = RelationshipHash;
                Integration.GovCode = GovCode;
                DebeHaber.Transaction Transaction = new DebeHaber.Transaction();
                DebeHaber.Production Production = new DebeHaber.Production();

                Production.branch = ProductionOrder.id_branch > 0 ? db.db.app_branch.Find(ProductionOrder.id_branch).name : "";
                Production.name = ProductionOrder.name;

                DateTime productionDate = ProductionOrder.trans_date;

                IQueryable<production_order_detail> ProductionOrder2 = ProductionOrder.production_order_detail.Where(x => x.item.id_item_type != item.item_type.Task).AsQueryable().Include(x => x.production_execution_detail);

                int DetailCount = 0;

                foreach (production_order_detail Detail in ProductionOrder2)
                {
                    if (Detail.production_execution_detail.Where(x => x.is_accounted == false && x.status == Status.Production.Executed).Count() > 0)
                    {
                        DebeHaber.Production_Detail Production_Detail = new DebeHaber.Production_Detail();
                        Production_Detail.Fill_ByExecution(Detail, db.db);
                        Production.Production_Detail.Add(Production_Detail);
                        productionDate = Production_Detail.trans_date;
                        DetailCount += 1;
                    }

                }

                Production.trans_date = productionDate;

                if (DetailCount > 0)
                {
                    Transaction.Production.Add(Production);
                    Integration.Transactions.Add(Transaction);



                    try
                    {
                        var Sales_Json = new JavaScriptSerializer().Serialize(Integration);
                        Send2API(Sales_Json);

                        ProductionOrder.IsSelected = false;

                        foreach (production_order_detail Detail in ProductionOrder.production_order_detail.Where(x => x.item.id_item_type != item.item_type.Task))
                        {
                            foreach (production_execution_detail Exe in Detail.production_execution_detail.Where(x => x.is_accounted == false && x.status == Status.Production.Executed))
                            {
                                Exe.is_accounted = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        if (ex.Message.Contains("403"))
                        {


                            DebeHaberLogIn page = new DebeHaberLogIn();
                            MainWindow rootWindow = Window.GetWindow(this) as MainWindow;

                            rootWindow.mainFrame.Navigate(page);


                        }
                        else
                        {
                            if (MessageBox.Show("Error. Would you like to save the file for analysis?", "", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                            {
                                MessageBox.Show(ex.Message, "Error Message");
                                Class.ErrorLog.DebeHaber(new JavaScriptSerializer().Serialize(Integration).ToString());
                            }
                        }
                    }
                    finally
                    {
                        db.db.SaveChanges();
                        fill();
                    }
                }
            }
        }

        #endregion Production_Sync

        private void FixedAsset(DebeHaber.Transaction Transaction)
        {
            List<item_asset_group> AssetGroupList = db.db.item_asset_group.Where(x => x.id_company == CurrentSession.Id_Company).ToList();

            foreach (item_asset_group item_asset_group in AssetGroupList)
            {
                DebeHaber.FixedAssetGroup FixedAssetGroup = new DebeHaber.FixedAssetGroup();
                FixedAssetGroup.Name = item_asset_group.name;
                FixedAssetGroup.LifespanYears = (decimal)item_asset_group.depreciation_rate;

                //Loop through
                foreach (item_asset item_asset in item_asset_group.item_asset.Where(x => x.IsSelected))
                {
                    DebeHaber.FixedAsset FixedAsset = new DebeHaber.FixedAsset();
                    FixedAsset.Name = item_asset.item.name;
                    FixedAsset.Code = item_asset.item.code;
                    FixedAsset.CurrentCost = (decimal)item_asset.current_value;
                    FixedAsset.PurchaseCost = (decimal)item_asset.purchase_value;
                    FixedAsset.PurchaseDate = (DateTime)item_asset.purchase_date;
                    FixedAsset.Quantity = 1;
                    FixedAsset.CurrencyName = CurrentSession.Currency_Default.name;

                    item_asset.IsSelected = false;
                    FixedAssetGroup.FixedAssets.Add(FixedAsset);
                }

                Transaction.FixedAssetGroups.Add(FixedAssetGroup);
            }
        }

        #region CheckBox Check/UnCheck Methods

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sales_invoiceViewSource.View != null)
            {
                foreach (sales_invoice sales_invoice in sales_invoiceViewSource.View.OfType<sales_invoice>().ToList())
                {
                    sales_invoice.IsSelected = true;
                }
                sales_invoiceViewSource.View.Refresh();
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sales_invoiceViewSource.View != null)
            {
                foreach (sales_invoice sales_invoice in sales_invoiceViewSource.View.OfType<sales_invoice>().ToList())
                {
                    sales_invoice.IsSelected = false;
                }
                sales_invoiceViewSource.View.Refresh();
            }
        }

        private void SalesReturn_Checked(object sender, RoutedEventArgs e)
        {
            if (sales_returnViewSource.View != null)
            {
                foreach (sales_return sales_return in sales_returnViewSource.View.OfType<sales_return>().ToList())
                {
                    sales_return.IsSelected = true;
                }
                sales_returnViewSource.View.Refresh();
            }
        }

        private void SalesReturn_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sales_returnViewSource.View != null)
            {
                foreach (sales_return sales_return in sales_returnViewSource.View.OfType<sales_return>().ToList())
                {
                    sales_return.IsSelected = false;
                }
                sales_returnViewSource.View.Refresh();
            }
        }

        private void Purchase_Checked(object sender, RoutedEventArgs e)
        {
            if (purchase_invoiceViewSource.View != null)
            {
                foreach (purchase_invoice purchase_invoice in purchase_invoiceViewSource.View.OfType<purchase_invoice>().ToList())
                {
                    purchase_invoice.IsSelected = true;
                }
                purchase_invoiceViewSource.View.Refresh();
            }
        }

        private void Purchase_UnChecked(object sender, RoutedEventArgs e)
        {
            if (purchase_invoiceViewSource.View != null)
            {
                foreach (purchase_invoice purchase_invoice in purchase_invoiceViewSource.View.OfType<purchase_invoice>().ToList())
                {
                    purchase_invoice.IsSelected = false;
                }
                purchase_invoiceViewSource.View.Refresh();
            }
        }

        private void PurchaseRetuen_Checked(object sender, RoutedEventArgs e)
        {
            if (purchase_returnViewSource.View != null)
            {
                foreach (purchase_return purchase_return in purchase_returnViewSource.View.OfType<purchase_return>().ToList())
                {
                    purchase_return.IsSelected = true;
                }
                purchase_returnViewSource.View.Refresh();
            }
        }

        private void PurchaseReturn_Unchecked(object sender, RoutedEventArgs e)
        {
            if (purchase_returnViewSource.View != null)
            {
                foreach (purchase_return purchase_return in purchase_returnViewSource.View.OfType<purchase_return>().ToList())
                {
                    purchase_return.IsSelected = false;
                }
                purchase_returnViewSource.View.Refresh();
            }
        }

        private void Payment_Checked(object sender, RoutedEventArgs e)
        {
            if (payment_detailViewSource.View != null)
            {
                foreach (payment_detail payment_detail in payment_detailViewSource.View.OfType<payment_detail>().ToList())
                {
                    payment_detail.IsSelected = true;
                }
                payment_detailViewSource.View.Refresh();
            }
        }

        private void Payment_UnChecked(object sender, RoutedEventArgs e)
        {
            if (payment_detailViewSource.View != null)
            {
                foreach (payment_detail payment_detail in payment_detailViewSource.View.OfType<payment_detail>().ToList())
                {
                    payment_detail.IsSelected = false;
                }
                payment_detailViewSource.View.Refresh();
            }
        }

        #endregion CheckBox Check/UnCheck Methods

        private object Send2API(object Json)
        {
            var webAddr = Settings.Default.DebeHaberConnString + "/api/syncData";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Authorization", "Bearer " + RelationshipHash);

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(Json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();


                if (result.ToString().Contains("Error"))
                {
                    MessageBox.Show(result.ToString());
                    Class.ErrorLog.DebeHaber(Json.ToString());
                }
                return result;
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DebeHaberSync() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support

        private void tbxServer_LostFocus(object sender, RoutedEventArgs e)
        {
            Settings SalesSettings = new Settings();

            SalesSettings.DebeHaberConnString = tbxServer.Content as string;
            Settings.Default.Save();
            SalesSettings = Settings.Default;
        }

        private void btnSalesData_Refresh(object sender, RoutedEventArgs e)
        {
            Get_SalesInvoice();

        }

        private void btnSalesReturnData_Refresh(object sender, RoutedEventArgs e)
        {

            Get_SalesReturn();

        }

        private void btnPurchaseData_Refresh(object sender, RoutedEventArgs e)
        {

            Get_PurchaseInvoice();

        }

        private void btnPurchaseReturnData_Refresh(object sender, RoutedEventArgs e)
        {
            Get_PurchaseReturnInvoice();
        }

        private void btnPaymentData_Refresh(object sender, RoutedEventArgs e)
        {
            Get_Payment();
        }

        private void btnProductionData_Refresh(object sender, RoutedEventArgs e)
        {
            Get_ProductionExecution();
        }
    }
}