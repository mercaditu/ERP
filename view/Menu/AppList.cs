using System;
using System.Data;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFLocalizeExtension.Extensions;

namespace Cognitivo.Menu
{
    public class ApplicationClass
    {
        public entity.App.Modules Module { get; set; }
        public AppList.Namespaces Namespace { get; set; }
        public entity.App.Names Application { get; set; }
        public string Name { get; set; }
        public entity.CurrentSession.Versions Version { get; set; }
        public bool HasReport { get; set; }
    }
    public class AppList
    {
        public DataTable dtApp { get; set; }
        public DataTable dtModule { get; set; }

        public enum Namespaces
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
            dtApp.Columns.Add("Version");
            dtApp.Columns.Add("HasReport");

            //Sales        //Module                 //Namespace      //App                 //Name                   //Img
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespaces.Form, "Commercial.Contact", entity.App.Names.Contact, "Contact", entity.CurrentSession.Versions.Basic, "1");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespaces.Form, "Commercial.ContactSubscription", entity.App.Names.Subscription, "Contact", entity.CurrentSession.Versions.Full, "1");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespaces.Transaction, "Sales.Budget", entity.App.Names.SalesBudget, "", entity.CurrentSession.Versions.Full, "1");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespaces.Transaction, "Sales.Order", entity.App.Names.SalesOrder, "SalesOrder", entity.CurrentSession.Versions.Medium, "1");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespaces.Transaction, "Sales.Invoice", entity.App.Names.SalesInvoice, "Sales", entity.CurrentSession.Versions.Basic, "1");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespaces.Transaction, "Sales.PointofSale", entity.App.Names.PointOfSale, "PointofSale", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespaces.Transaction, "Sales.Return", entity.App.Names.SalesReturn, "SalesReturn", entity.CurrentSession.Versions.Basic, "1");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespaces.Financial, "Configs.AccountUtility", entity.App.Names.AccountUtility, "Accounts", entity.CurrentSession.Versions.Basic, "0");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespaces.Financial, "Commercial.AccountsRecievable", entity.App.Names.AccountsReceivable, "Money", entity.CurrentSession.Versions.Basic, "1");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespaces.Inventory, "Sales.PackingList", entity.App.Names.PackingList, "ProductSend", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespaces.Inventory, "Sales.Packing", entity.App.Names.PackingList, "", entity.CurrentSession.Versions.Medium, "0");

            dtApp.Rows.Add(entity.App.Modules.Sales, Namespaces.Reports, "Reporting.Views.SalesByItem", entity.App.Names.SalesByItem, "Reports", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespaces.Reports, "Reporting.Views.SalesbySalesman", entity.App.Names.SalesbySalesman, "Reports", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespaces.Reports, "Reporting.Views.CostOfGoodsSold", entity.App.Names.CostOfGoodsSold, "Reports", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespaces.Reports, "Reporting.Views.DeliveryByCustomer", entity.App.Names.DeliveryByCustomer, "Reports", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespaces.Reports, "Reporting.Views.SalesByVAT", entity.App.Names.VATSales, "Reports", entity.CurrentSession.Versions.Lite, "0");

            //Purchase        //Module                 //Namespace      //App                 //Name                   //Img
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespaces.Form, "Commercial.Contact", entity.App.Names.Contact, "Contact", entity.CurrentSession.Versions.Lite, "1");
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespaces.Inventory, "Purchase.PackingList", entity.App.Names.PackingList, "ProductSend", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespaces.Transaction, "Purchase.Tender", entity.App.Names.PurchaseTender, "PurchaseTender", entity.CurrentSession.Versions.Full, "1");
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespaces.Transaction, "Purchase.Order", entity.App.Names.PurchaseOrder, "PurchaseOrder", entity.CurrentSession.Versions.Medium, "1");
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespaces.Transaction, "Purchase.Invoice", entity.App.Names.PurchaseInvoice, "Purchase", entity.CurrentSession.Versions.Lite, "1");
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespaces.Transaction, "Purchase.Return", entity.App.Names.PurchaseReturn, "PurchaseReturn", entity.CurrentSession.Versions.Basic, "1");
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespaces.Financial, "Configs.AccountUtility", entity.App.Names.AccountUtility, "Accounts", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespaces.Financial, "Commercial.AccountsPayable", entity.App.Names.AccountsPayable, "Money", entity.CurrentSession.Versions.Lite, "1");
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespaces.Reports, "Reporting.Views.PendingPayables", entity.App.Names.PendingPayables, "Reports", entity.CurrentSession.Versions.Lite, "0");


            //Human Resources        //Module                 //Namespace      //App                 //Name                   //Img
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespaces.Transaction, "HumanResource.Talent", entity.App.Names.Talent, "EmployeeTalent", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespaces.Form, "HumanResource.Employee", entity.App.Names.Employee, "EmployeeID", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespaces.Financial, "Configs.Hr_coefficient", entity.App.Names.HourCoeficient, "EntryExit", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespaces.Financial, "Configs.Hr_position", entity.App.Names.Position, "EntryExit", entity.CurrentSession.Versions.Medium, "0");

            //Stock
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespaces.Form, "Product.Item", entity.App.Names.Items, "Item", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespaces.Form, "Product.FixedAssets", entity.App.Names.FixedAssets, "FixedAsset", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespaces.Form, "Product.Recipe", entity.App.Names.Recipe, "Recipe", entity.CurrentSession.Versions.Basic, "0");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespaces.Form, "Product.Stock", entity.App.Names.Stock, "ProductStock", entity.CurrentSession.Versions.Lite, "1");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespaces.Form, "Product.Promotions", entity.App.Names.SalesPromotion, "Promotion", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespaces.Form, "Product.BranchWalkin", entity.App.Names.FootTraffic, "FootTraffic", entity.CurrentSession.Versions.Basic, "0");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespaces.Transaction, "Product.Inventory", entity.App.Names.Inventory, "ProductStockAdjust", entity.CurrentSession.Versions.Basic, "1");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespaces.Transaction, "Product.Movement", entity.App.Names.Movement, "BranchLocation", entity.CurrentSession.Versions.Basic, "1");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespaces.Transaction, "Product.Transfer", entity.App.Names.Transfer, "ProductSend", entity.CurrentSession.Versions.Basic, "1");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespaces.Transaction, "Product.Request", entity.App.Names.RequestManagement, "ProjectPlan", entity.CurrentSession.Versions.Basic, "0");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespaces.Transaction, "Product.SuppliesRequest", entity.App.Names.RequestResource, "ProjectPlan", entity.CurrentSession.Versions.Basic, "0");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespaces.Transaction, "Product.Maintainance", entity.App.Names.Maintainance, "Maintainance", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Stock, entity.App.Modules.Configuration, "Product.ItemTag", entity.App.Names.ItemTag, "ProductTag", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Stock, entity.App.Modules.Configuration, "Product.PriceList", entity.App.Names.PriceList, "ProductPriceList", entity.CurrentSession.Versions.Lite, "1");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespaces.Inventory, "Sales.PackingList", "PackingList", "ProductSend", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespaces.Reports, "Reporting.Views.StockFlow", entity.App.Names.InventoryFlow, "Reports", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespaces.Reports, "Reporting.Views.CostBreakDown", entity.App.Names.CostBreakDown, "Reports", entity.CurrentSession.Versions.Lite, "0");

            //Impex
            dtApp.Rows.Add(entity.App.Modules.Impex, Namespaces.Exports, "Sales.Export", entity.App.Names.Export, "Export", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Impex, Namespaces.Importation, "Purchase.Import", entity.App.Names.Imports, "Import", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Impex, entity.App.Modules.Configuration, "Commercial.Incoterm", entity.App.Names.Incoterm, "IncotermContract", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Impex, entity.App.Modules.Configuration, "Configs.IncotermCondition", entity.App.Names.IncotermCondition, "", entity.CurrentSession.Versions.Medium, "0");

            //Finance
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespaces.Form, "Configs.AccountUtility", entity.App.Names.AccountUtility, "Accounts", entity.CurrentSession.Versions.Basic, "0");
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespaces.Transaction, "Commercial.AccountsPayable", entity.App.Names.AccountsPayable, "Money", entity.CurrentSession.Versions.Basic, "0");
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespaces.Transaction, "Commercial.AccountsRecievable", entity.App.Names.AccountsReceivable, "Money", entity.CurrentSession.Versions.Basic, "0");
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespaces.Transaction, "Commercial.Payments", entity.App.Names.Payment, "", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespaces.Form, "Commercial.PromissoryNote", entity.App.Names.PromissoryNote, "IOU", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespaces.Form, "Commercial.Reconciliation", entity.App.Names.Reconciliation, "", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Finance, entity.App.Modules.Configuration, "Configs.Currency", entity.App.Names.Currency, "Currency", entity.CurrentSession.Versions.Basic, "0");
            dtApp.Rows.Add(entity.App.Modules.Finance, entity.App.Modules.Configuration, "Commercial.PaymentType", entity.App.Names.PaymentType, "", entity.CurrentSession.Versions.Basic, "0");
            dtApp.Rows.Add(entity.App.Modules.Finance, entity.App.Modules.Configuration, "Configs.Bank", entity.App.Names.Bank, "BankAccount", entity.CurrentSession.Versions.Basic, "0");
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespaces.Reports, "Reporting.Views.CurrentAccount_Customer", entity.App.Names.CurrentAccountCustomer, "Reports", entity.CurrentSession.Versions.Basic, "0");

            //Projects Printing Press
            dtApp.Rows.Add(entity.App.Modules.Project_PrintingPress, Namespaces.Form, "Project.PrintingPress.Template", entity.App.Names.Template, "ProjectCategory", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Project_PrintingPress, Namespaces.Transaction, "Project.PrintingPress.Estimate", "Costing", "ProjectTaskWizard", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Project_PrintingPress, Namespaces.Reports, "Reporting.Views.Project", entity.App.Names.ActivityPlan, "Reports", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Project_PrintingPress, Namespaces.Reports, "Reporting.Views.ProjectExecution", entity.App.Names.ProjectExecution, "Reports", entity.CurrentSession.Versions.Medium, "0");

            //Projects Event
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespaces.Form, "Project.EventType", entity.App.Names.Template, "ProjectCategory", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespaces.Transaction, "Project.EventCosting", entity.App.Names.EventManagement, "ProjectTaskWizard", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespaces.Transaction, "Project.Development.TaskView", entity.App.Names.ActivityPlan, "ProjectTask", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespaces.Inventory, "Project.Development.Logistics", entity.App.Names.Logistics, "Logistics", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespaces.Financial, "Project.EventFinance", entity.App.Names.EventFinance, "ProjectSalesOrder", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespaces.Transaction, "Project.ProjectExecution", entity.App.Names.ProjectExecution, "", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespaces.Reports, "Reporting.Views.Project", entity.App.Names.ActivityPlan, "Reports", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespaces.Reports, "Reporting.Views.ProjectExecution", entity.App.Names.ProjectExecution, "Reports", entity.CurrentSession.Versions.Medium, "0");

            //Projects Plain
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespaces.Form, "Project.Development.ProjectType", entity.App.Names.Template, "ProjectCategory", entity.CurrentSession.Versions.Full, "0");
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespaces.Form, "Project.Development.Project", entity.App.Names.Project, "Project", entity.CurrentSession.Versions.Full, "0");
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespaces.Transaction, "Project.Development.TaskView", entity.App.Names.ActivityPlan, "ProjectTask", entity.CurrentSession.Versions.Full, "0");
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespaces.Inventory, "Project.Development.Logistics", entity.App.Names.Logistics, "Logistics", entity.CurrentSession.Versions.Full, "0");
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespaces.Transaction, "Project.ProjectExecution", entity.App.Names.ProjectExecution, "Plan", entity.CurrentSession.Versions.Full, "0");
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespaces.Financial, "Project.ProjectFinance", entity.App.Names.ProjectFinance, "ProjectSalesOrder", entity.CurrentSession.Versions.Full, "0");
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespaces.Reports, "Reporting.Views.Project", entity.App.Names.ActivityPlan, "Reports", entity.CurrentSession.Versions.Full, "0");
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespaces.Reports, "Reporting.Views.ProjectExecution", entity.App.Names.ProjectExecution, "Reports", entity.CurrentSession.Versions.Full, "0");

            //Production
            dtApp.Rows.Add(entity.App.Modules.Production, Namespaces.Form, "Production.Line", entity.App.Names.Line, "Line", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Production, Namespaces.Transaction, "Production.Order", entity.App.Names.ProductionOrder, "ProductionOrder", entity.CurrentSession.Versions.Medium, "1");
            dtApp.Rows.Add(entity.App.Modules.Production, Namespaces.Transaction, "Production.FractionOrder", entity.App.Names.ProductionbyFraction, "Fraction", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Production, Namespaces.Transaction, "Production.Execution", entity.App.Names.ProductionExecution, "ProductionExecution", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Production, Namespaces.Reports, "Reporting.Views.Production_HumanResources", entity.App.Names.EmployeesInProduction, "Reports", entity.CurrentSession.Versions.Medium, "0");
            dtApp.Rows.Add(entity.App.Modules.Production, Namespaces.Reports, "Reporting.Views.ProductionOrderStatus", entity.App.Names.ProductionOrderStatus, "Reports", entity.CurrentSession.Versions.Medium, "0");

            //Application
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Preferences, "Configs.Language", "Language", "Language", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Preferences, "Setup.Migration.MigrationAssistant", "MigrationAssistant", "MigrationAssistant", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Preferences, "Setup.Migration.Cogent.MigrationGUI", "MigrationGUI", "MigrationGUI", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Preferences, "Configs.ConnectionBuilder", "ConnectionBuilder", "Network", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Preferences, "Configs.Settings", "Settings", "", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Preferences, "Configs.KeyGestureSettings", "KeyGestureSettings", "", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Preferences, "Configs.Wallpaper", "Wallpaper", "Wallpaper", entity.CurrentSession.Versions.Lite, "0");

            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Company, "Configs.Company", "Company", "HomeCompany", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Company, "Configs.Branch", "Branch", "Branch", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Company, "Configs.Terminal", "Terminal", "Terminal", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Company, "Configs.Geography", "Geography", "Location", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Company, "Configs.VAT", "VAT", "ProductPromotion", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Company, "Configs.VATGroup", "VATGroup", "ProductPromotion", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Company, "Security.User", "User", "User", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Company, "Security.UserRole", "UserRole", "UserRole", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Company, "Configs.Department", "Department", "", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Company, "Accounting.DebeHaberLogin", "DebeHaberSync", "", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Company, "Configs.Version", "Version", "Version", entity.CurrentSession.Versions.Lite, "0");

            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Financial, "Configs.Currency", "Currency", "Currency", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Financial, "Configs.AccountingCurrency", "AccountingCurrency", "Currency", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Financial, "Commercial.PaymentType", "PaymentType", "PaymentType", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Financial, "Configs.Bank", "Bank", "BankAccount", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Financial, "Configs.Account", "Account", "BankAccountTrans", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Financial, "Configs.FiscalPeriod", "FiscalPeriod", "BankAccountTrans", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Financial, "Configs.Hr_coefficient", "Hr_coefficient", "", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Financial, "Configs.Hr_position", "Hr_position", "", entity.CurrentSession.Versions.Medium, "0");


            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Transaction, "Configs.Condition", "Condition", "Condition", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Transaction, "Configs.Contract", "Contract", "Contract", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Transaction, "Configs.Status", "Status", "Status", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Transaction, "Configs.Document", "Document", "Document", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Transaction, "Configs.DocumentRange", "DocumentRange", "DocumentRange", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Transaction, "Configs.DocumentDesigner", "DocumentDesigner", "DocumentDesigner", entity.CurrentSession.Versions.Lite, "0");

            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Purchase, "Configs.CostCenter", "CostCenter", "AccountingChart", entity.CurrentSession.Versions.Lite, "0");

            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Sales, "Sales.Salesman", "Salesman", "SalesRep", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Sales, "Commercial.ContactRole", "ContactRole", "ContactRole", entity.CurrentSession.Versions.Lite, "0");

            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Inventory, "Configs.Location", "Location", "BranchLocation", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Inventory, "Configs.MeasurementType", "MeasurementType", "Measurement", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Inventory, "Configs.Measurement", "Measurement", "Measurement", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Inventory, "Configs.ItemBrand", "ItemBrand", "Product", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Inventory, "Configs.Dimension", "Dimension", "", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Inventory, "Configs.Property", "Property", "Product", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Inventory, "Product.ItemTag", "ItemTag", "Product", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespaces.Inventory, "Product.ItemTemplate", "ItemTemplate", "Product", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Sales, "Contact.ContactTag", "ContactTag", "Contact", entity.CurrentSession.Versions.Lite, "0");
            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Sales, "Configs.ItemAssetGroup", "ItemAssetGroup", "ItemAssetGroup", entity.CurrentSession.Versions.Lite, "0");

            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Project_Dev, "Project.ProjectTag", "ProjectTag", "Project", entity.CurrentSession.Versions.Lite, "0");

         
        }

        public cntrl.applicationIcon get_AppIcon(DataRow app)
        {
            string _namespace = app["namespace"].ToString();
            string _app = app["app"].ToString();
            string _name = app["name"].ToString();
            string _img = app["img"].ToString();
            string _description = "desc_" + app["name"].ToString();
            string _HasReport = app["HasReport"].ToString();

            cntrl.applicationIcon appIcon = new cntrl.applicationIcon();
            
            //Checks if App has Inbuilt Report to show ReportIcon. This Helps save space.
            appIcon.HasReport = _HasReport == "1" ? true : false;
            
            ///AssemblyCheck. If reporting exists, don't add Cognitivo.
            appIcon.Tag = "Cognitivo." + _app;
            appIcon.Uid = _name;
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
