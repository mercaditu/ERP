using System;
using System.Data;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFLocalizeExtension.Extensions;
using entity.Brillo;

namespace Cognitivo.Menu
{
    public class AppList
    {
        public DataTable dtApp { get; set; }
        public DataTable dtModule { get; set; }

        enum Namespace
        {
            Form,
            Transaction,
            Financial,
            Inventory,
            Importation,
            Exports,

            Reports,
            Documents,
            Company,
            Preferences
        }

        public AppList()
        {
            //Create Datatable
            dtApp = new DataTable();

            dtApp.Columns.Add("module");
            dtApp.Columns.Add("namespace");
            dtApp.Columns.Add("app");
            dtApp.Columns.Add("name");
            dtApp.Columns.Add("img");
            dtApp.Columns.Add("entity.CurrentSession.Versions");

            //Sales        //Module                 //Namespace      //App                 //Name                   //Img
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Form, "Commercial.Contact", entity.App.Names.Contact, "Contact", entity.CurrentSession.Versions.Basic);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Form, "Commercial.ContactSubscription", entity.App.Names.Subscription, "Contact", entity.CurrentSession.Versions.Full);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Transaction, "Sales.Budget", entity.App.Names.SalesBudget, "", entity.CurrentSession.Versions.Full);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Transaction, "Sales.Order", entity.App.Names.SalesOrder, "SalesOrder", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Transaction, "Sales.Invoice", entity.App.Names.SalesInvoice, "Sales", entity.CurrentSession.Versions.Basic);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Transaction, "Sales.PointofSale", entity.App.Names.PointOfSale, "PointofSale", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Transaction, "Sales.Return", entity.App.Names.SalesReturn, "SalesReturn", entity.CurrentSession.Versions.Basic);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Financial, "Configs.AccountUtility", entity.App.Names.AccountUtility, "Accounts", entity.CurrentSession.Versions.Basic);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Financial, "Commercial.AccountsRecievable", entity.App.Names.AccountsReceivable, "Money", entity.CurrentSession.Versions.Basic);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Inventory, "Sales.PackingList", entity.App.Names.PackingList, "ProductSend", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Inventory, "Sales.Packing", entity.App.Names.PackingList, "", entity.CurrentSession.Versions.Medium);

            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.SalesBudgetSummary", entity.App.Names.SalesBudget, "Reports", entity.CurrentSession.Versions.Full);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.SalesOrderSummary", entity.App.Names.SalesOrder, "Reports", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.SalesInvoiceSummary", entity.App.Names.SalesInvoice, "Reports", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.SalesByItem", entity.App.Names.SalesByItem, "Reports", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.CostOfGoodsSold", entity.App.Names.CostOfGoodsSold, "Reports", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.SalesbySalesman", entity.App.Names.SalesbySalesman, "Reports", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.DeliveryByCustomer", entity.App.Names.DeliveryByCustomer, "Reports", entity.CurrentSession.Versions.Full);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.SalesReturnDetail", entity.App.Names.SalesReturn, "Reports", entity.CurrentSession.Versions.Full);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.SalesByVAT", entity.App.Names.SalesByVAT, "Reports", entity.CurrentSession.Versions.Lite);


            //Purchase        //Module                 //Namespace      //App                 //Name                   //Img
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Form, "Commercial.Contact", entity.App.Names.Contact, "Contact", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Inventory, "Purchase.PackingList", entity.App.Names.PurchasePacking, "ProductSend", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Transaction, "Purchase.Tender", entity.App.Names.PurchaseTender, "PurchaseTender", entity.CurrentSession.Versions.Full);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Transaction, "Purchase.Order", entity.App.Names.PurchaseOrder, "PurchaseOrder", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Transaction, "Purchase.Invoice", entity.App.Names.PurchaseInvoice, "Purchase", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Transaction, "Purchase.Return", entity.App.Names.PurchaseReturn, "PurchaseReturn", entity.CurrentSession.Versions.Basic);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Financial, "Configs.AccountUtility", entity.App.Names.AccountUtility, "Accounts", entity.CurrentSession.Versions.Basic);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Financial, "Commercial.AccountsPayable", entity.App.Names.AccountsPayable, "Money", entity.CurrentSession.Versions.Basic);

            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Reports, "Reporting.Views.PurchaseTenderSummary", entity.App.Names.PurchaseTenderSummary, "Reports", entity.CurrentSession.Versions.Full);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Reports, "Reporting.Views.PurchaseOrderSummary", entity.App.Names.PurchaseOrderSummary, "Reports", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Reports, "Reporting.Views.PurchaseInvoiceSummary", entity.App.Names.PurchaseInvoiceSummary, "Reports", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Reports, "Reporting.Views.Contacts", entity.App.Names.Contact, "Reports", entity.CurrentSession.Versions.Lite);

            //Human Resources        //Module                 //Namespace      //App                 //Name                   //Img
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespace.Transaction, "HumanResource.Talent", entity.App.Names.Talent, "EmployeeTalent", entity.CurrentSession.Versions.Full);
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespace.Form, "HumanResource.Employee", entity.App.Names.Employee, "EmployeeID", entity.CurrentSession.Versions.Full);
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespace.Financial, "Configs.Hr_coefficient", entity.App.Names.Hr_coefficient, "EntryExit", entity.CurrentSession.Versions.Full);

            //Stock
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Form, "Product.Item", entity.App.Names.Items, "Item", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Form, "Product.FixedAssets", entity.App.Names.FixedAssets, "FixedAsset", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Form, "Product.Recipe", entity.App.Names.Recipe, "Recipe", entity.CurrentSession.Versions.Basic);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Form, "Product.Stock", entity.App.Names.Stock, "ProductStock", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Form, "Product.Promotions", entity.App.Names.ItemPromotion, "Promotion", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Form, "Product.BranchWalkin", entity.App.Names.FootTraffic, "FootTraffic", entity.CurrentSession.Versions.Basic);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transaction, "Product.Inventory", entity.App.Names.Inventory, "ProductStockAdjust", entity.CurrentSession.Versions.Basic);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transaction, "Product.Movement", entity.App.Names.Movement, "BranchLocation", entity.CurrentSession.Versions.Basic);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transaction, "Product.Transfer", entity.App.Names.Transfer, "ProductSend", entity.CurrentSession.Versions.Basic);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transaction, "Product.Request", entity.App.Names.RequestManagement, "ProjectPlan", entity.CurrentSession.Versions.Basic);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transaction, "Product.SuppliesRequest", entity.App.Names.RequestResource, "ProjectPlan", entity.CurrentSession.Versions.Basic);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transaction, "Product.Maintainance", entity.App.Names.Maintainance, "Maintainance", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Stock, entity.App.Modules.Configuration, "Product.ItemTag", entity.App.Names.ItemTag, "ProductTag", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Stock, entity.App.Modules.Configuration, "Product.PriceList", entity.App.Names.PriceList, "ProductPriceList", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Inventory, "Sales.PackingList", "PackingList", "ProductSend", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Reports, "Reporting.Views.Inventory", entity.App.Names.Stock, "Reports", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Reports, "Reporting.Views.InventorySummary", entity.App.Names.Inventory, "Reports", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Reports, "Reporting.Views.StockFlow", entity.App.Names.StockFlow, "Reports", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Reports, "Reporting.Views.StockFlowDimension", entity.App.Names.StockFlowDimension, "Reports", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Reports, "Reporting.Views.ItemPriceList", entity.App.Names.ItemPriceList, "Reports", entity.CurrentSession.Versions.Lite);


            //Impex
            dtApp.Rows.Add(entity.App.Modules.Impex, Namespace.Exports, "Sales.Export", entity.App.Names.Export, "Export", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Impex, Namespace.Importation, "Purchase.Import", entity.App.Names.Imports, "Import", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Impex, entity.App.Modules.Configuration, "Commercial.Incoterm", entity.App.Names.Incoterm, "IncotermContract", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Impex, entity.App.Modules.Configuration, "Configs.IncotermCondition", entity.App.Names.IncotermCondition, "", entity.CurrentSession.Versions.Medium);

            //Finance
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Form, "Configs.AccountUtility", entity.App.Names.AccountUtility, "Accounts", entity.CurrentSession.Versions.Basic);
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Transaction, "Commercial.AccountsPayable", entity.App.Names.AccountsPayable, "Money", entity.CurrentSession.Versions.Basic);
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Transaction, "Commercial.AccountsRecievable", entity.App.Names.AccountsReceivable, "Money", entity.CurrentSession.Versions.Basic);
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Transaction, "Commercial.Payments", entity.App.Names.Payment, "", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Transaction, "Commercial.PromissoryNote", entity.App.Names.PromissoryNote, "IOU", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Transaction, "Commercial.Reconciliation", entity.App.Names.Reconciliation, "", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Finance, entity.App.Modules.Configuration, "Configs.Currency", entity.App.Names.Currency, "Currency", entity.CurrentSession.Versions.Basic);
            dtApp.Rows.Add(entity.App.Modules.Finance, entity.App.Modules.Configuration, "Commercial.PaymentType", entity.App.Names.PaymentType, "", entity.CurrentSession.Versions.Basic);
            dtApp.Rows.Add(entity.App.Modules.Finance, entity.App.Modules.Configuration, "Configs.Bank", entity.App.Names.Bank, "BankAccount", entity.CurrentSession.Versions.Basic);
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Reports, "Reporting.Views.CurrentAccount_Customer", entity.App.Names.CurrentAccountCustomer, "Reports", entity.CurrentSession.Versions.Basic);
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Reports, "Reporting.Views.PendingSalesDocs", entity.App.Names.SalesDocuments, "Reports", entity.CurrentSession.Versions.Basic);
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Reports, "Reporting.Views.AccountsRecievableAnalisys", entity.App.Names.AnalisysofAccountsReceivable, "Reports", entity.CurrentSession.Versions.Medium);

            //Projects Printing Press
            dtApp.Rows.Add(entity.App.Modules.Project_PrintingPress, Namespace.Form, "Project.PrintingPress.Template", entity.App.Names.Template, "ProjectCategory", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Project_PrintingPress, Namespace.Transaction, "Project.PrintingPress.Estimate", "Costing", "ProjectTaskWizard", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Project_PrintingPress, Namespace.Reports, "Reporting.Views.Project", entity.App.Names.ActivityPlan, "Reports", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Project_PrintingPress, Namespace.Reports, "Reporting.Views.ProjectExecution", entity.App.Names.ProjectExecution, "Reports", entity.CurrentSession.Versions.Medium);

            //Projects Event
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Form, "Project.EventType", entity.App.Names.Template, "ProjectCategory", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Transaction, "Project.EventCosting", entity.App.Names.EventManagement, "ProjectTaskWizard", entity.CurrentSession.Versions.Medium);

            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Transaction, "Project.Development.TaskView", entity.App.Names.ActivityPlan, "ProjectTask", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Inventory, "Project.Development.Logistics", entity.App.Names.Logistics, "Logistics", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Financial, "Project.ProjectFinance", entity.App.Names.ProjectFinance, "ProjectSalesOrder", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Transaction, "Project.ProjectExecution", entity.App.Names.ProjectExecution, "", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Reports, "Reporting.Views.Project", entity.App.Names.ActivityPlan, "Reports", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Reports, "Reporting.Views.ProjectExecution", entity.App.Names.ProjectExecution, "Reports", entity.CurrentSession.Versions.Medium);

            //Projects Plain
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Form, "Project.Development.ProjectType", entity.App.Names.Template, "ProjectCategory", entity.CurrentSession.Versions.Enterprise);
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Form, "Project.Development.Project", entity.App.Names.Project, "Project", entity.CurrentSession.Versions.Enterprise);
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Transaction, "Project.Development.TaskView", entity.App.Names.ActivityPlan, "ProjectTask", entity.CurrentSession.Versions.Enterprise);
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Inventory, "Project.Development.Logistics", entity.App.Names.Logistics, "Logistics", entity.CurrentSession.Versions.Enterprise);
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Transaction, "Project.ProjectExecution", entity.App.Names.ProjectExecution, "Plan", entity.CurrentSession.Versions.Enterprise);
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Financial, "Project.ProjectFinance", entity.App.Names.ProjectFinance, "ProjectSalesOrder", entity.CurrentSession.Versions.Enterprise);
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Reports, "Reporting.Views.Project", entity.App.Names.ActivityPlan, "Reports", entity.CurrentSession.Versions.Enterprise);
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Reports, "Reporting.Views.ProjectExecution", entity.App.Names.ProjectExecution, "Reports", entity.CurrentSession.Versions.Enterprise);

            //Production
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Form, "Production.Line", entity.App.Names.Line, "Line", entity.CurrentSession.Versions.Full);
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Transaction, "Production.Order", entity.App.Names.ProductionOrder, "ProductionOrder", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Transaction, "Production.FractionOrder", entity.App.Names.ProductionbyFraction, "Fraction", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Transaction, "Production.Execution", entity.App.Names.ProductionExecution, "ProductionExecution", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Reports, "Reporting.Views.EmployeesInProduction", entity.App.Names.EmployeesInProduction, "Reports", entity.CurrentSession.Versions.Medium);
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Reports, "Reporting.Views.ProductionOrderStatus", entity.App.Names.ProductionOrderStatus, "Reports", entity.CurrentSession.Versions.Medium);

            //Accounting
            dtApp.Rows.Add(entity.App.Modules.Accounting, Namespace.Transaction, "Accounting.DebeHaberLogin", entity.App.Names.DebeHaberIntegration, "", entity.CurrentSession.Versions.Lite);

            //Application
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Configs.Language", "Language", "Language", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Setup.Migration.MigrationAssistant", "MigrationAssistant", "MigrationAssistant", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Setup.Migration.Cogent.MigrationGUI", "MigrationGUI", "MigrationGUI", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Configs.ConnectionBuilder", "ConnectionBuilder", "Network", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Configs.Settings", "Settings", "", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Configs.KeyGestureSettings", "KeyGestureSettings", "", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Configs.Wallpaper", "Wallpaper", "Wallpaper", entity.CurrentSession.Versions.Lite);

            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.Company", "Company", "HomeCompany", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.Branch", "Branch", "Branch", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.Terminal", "Terminal", "Terminal", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.Geography", "Geography", "Location", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.VAT", "VAT", "ProductPromotion", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.VATGroup", "VATGroup", "ProductPromotion", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Security.User", "User", "User", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Security.UserRole", "UserRole", "UserRole", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.Department", "Department", "", entity.CurrentSession.Versions.Lite);

            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.Currency", "Currency", "Currency", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.AccountingCurrency", "AccountingCurrency", "Currency", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Commercial.PaymentType", "PaymentType", "PaymentType", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.Bank", "Bank", "BankAccount", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.Account", "Account", "BankAccountTrans", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.FiscalPeriod", "FiscalPeriod", "BankAccountTrans", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.Hr_coefficient", "Hr_coefficient", "", entity.CurrentSession.Versions.Lite);

            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transaction, "Configs.Condition", "Condition", "Condition", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transaction, "Configs.Contract", "Contract", "Contract", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transaction, "Configs.Status", "Status", "Status", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transaction, "Configs.Document", "Document", "Document", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transaction, "Configs.DocumentRange", "DocumentRange", "DocumentRange", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transaction, "Configs.DocumentDesigner", "DocumentDesigner", "DocumentDesigner", entity.CurrentSession.Versions.Lite);

            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Purchase, "Configs.CostCenter", "CostCenter", "AccountingChart", entity.CurrentSession.Versions.Lite);

            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Sales, "Sales.Salesman", "Salesman", "SalesRep", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Sales, "Commercial.ContactRole", "ContactRole", "ContactRole", entity.CurrentSession.Versions.Lite);

            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.Location", "Location", "BranchLocation", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.MeasurementType", "MeasurementType", "Measurement", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.Measurement", "Measurement", "Measurement", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.ItemBrand", "ItemBrand", "Product", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.Dimension", "Dimension", "", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.Property", "Property", "Product", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Product.ItemTag", "ItemTag", "Product", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Product.ItemTemplate", "ItemTemplate", "Product", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Sales, "Contact.ContactTag", "ContactTag", "Contact", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Sales, "Configs.ItemAssetGroup", "ItemAssetGroup", "ItemAssetGroup", entity.CurrentSession.Versions.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Project_Dev, "Project.ProjectTag", "ProjectTag", "Project", entity.CurrentSession.Versions.Lite);
        }

        public cntrl.applicationIcon get_AppIcon(DataRow app)
        {
            string _namespace = app["namespace"].ToString();
            string _app = app["app"].ToString();
            string _name = app["name"].ToString();
            string _img = app["img"].ToString();
            string _description = "desc_" + app["name"].ToString();

            cntrl.applicationIcon appIcon = new cntrl.applicationIcon();

            ///AssemblyCheck. If reporting exists, don't add Cognitivo.
            appIcon.Tag = "Cognitivo." + _app;

            ///Security Check.
            try
            {
                entity.Brillo.Security security = new entity.Brillo.Security((entity.App.Names)Enum.Parse(typeof(entity.App.Names), _name, true));
                if (security.view == false)
                {
                    appIcon.IsEnabled = false;
                }
            }
            catch
            {
                appIcon.IsEnabled = false;
            }

            var appLocApplicationName = new LocTextExtension("Cognitivo:local:" + _name + "").SetBinding(appIcon, cntrl.applicationIcon.ApplicationNameProperty);

            //Incase img is not set, set it to DefaultIcon
            if (_img == "") { _img = "DefaultIcon"; }

            _img = "../Images/Application/128/" + _img + ".png";
            appIcon.imgSource = new BitmapImage(new Uri(_img, UriKind.Relative));

            return appIcon;
        }

        public StackPanel get_ConfigIcon(DataRow app)
        {
            string _app = app["app"].ToString();
            string _namespace = app["namespace"].ToString();
            string _img = app["img"].ToString();
            string _description = app["desc"].ToString();
            //int module = (int)app["id"];

            TextBlock txtApp = new TextBlock();
            txtApp.Style = (Style)Application.Current.FindResource("text_HyperLinks");
            var appLocTextExtension = new LocTextExtension("COGNITIVO:local:" + _app + "").SetBinding(txtApp, TextBlock.TextProperty);
            txtApp.FontSize = 16;
            Thickness margin_txtApp = txtApp.Margin;
            margin_txtApp.Left = 5;
            txtApp.Margin = margin_txtApp;

            Image img = new Image();
            //Incase img is not set, set it to DefaultIcon
            _img = "../Images/Application/128/" + (_img == "" ? _img : "DefaultIcon") + ".png";
            img.Stretch = Stretch.UniformToFill;
            img.Width = 26; img.Height = 26;
            img.Source = new BitmapImage(new Uri(_img, UriKind.Relative));

            StackPanel stckIcon = new StackPanel();
            stckIcon.Orientation = Orientation.Horizontal;
            stckIcon.Children.Add(img);
            stckIcon.Children.Add(txtApp);

            StackPanel stckApp = new StackPanel();
            stckApp.Children.Add(stckIcon);

            if (_description != "")
            {
                TextBlock txtDesc = new TextBlock();
                txtDesc.Style = (Style)Application.Current.FindResource("text_Instructions");
                var descLocTextExtension = new LocTextExtension("COGNITIVO:local:" + _description + "").
                SetBinding(txtDesc, TextBlock.TextProperty);
                Thickness margin_txtDesc = txtDesc.Margin;
                margin_txtDesc.Left = 31;
                txtDesc.Margin = margin_txtDesc;
                stckApp.Children.Add(txtDesc);
            }

            Thickness margin_stckApp = stckApp.Margin;
            margin_stckApp.Left = 32;
            margin_stckApp.Right = 32;
            margin_stckApp.Top = 32;
            margin_stckApp.Bottom = 32;
            txtApp.Margin = margin_stckApp;
            stckApp.Tag = _namespace + "." + _app;
            //stckApp.Uid = module.ToString();
            return stckApp;
        }
    }
}
