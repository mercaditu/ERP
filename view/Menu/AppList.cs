using System;
using System.Data;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFLocalizeExtension.Extensions;

namespace Cognitivo.Menu
{
    public class AppList
    {
        public DataTable dtApp { get; set; }
        public DataTable dtModule { get; set; }

        public enum Version
        {
            Lite,       //  //     0 USD //   0 USD
            Basic,      //  // 3,000 USD // 350 USD
            Medium,     //  // 5,000 USD // 550 USD
            Full,       //  // 8,000 USD // 750 USD
            Enterprise, //  //12,000 USD //
            PrintingPress,
            EventManagement
        }

        public enum VersionKey
        {
            Himayuddin_51,
            Bathua_102,
            Mankurad_153,
            Dashehari_204,
            Chausa_255,
            Gulabkhas_306,
            Alphonso_357
        }

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
            dtApp.Columns.Add("version");

            //Sales        //Module                 //Namespace      //App                 //Name                   //Img
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Form, "Commercial.Contact", entity.App.Names.Contact, "Contact", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Form, "Commercial.ContactSubscription", entity.App.Names.Subscription, "Contact", Version.Full);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Transaction, "Sales.Budget", entity.App.Names.SalesBudget, "", Version.Full);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Transaction, "Sales.Order", entity.App.Names.SalesOrder, "SalesOrder", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Transaction, "Sales.Invoice", entity.App.Names.SalesInvoice, "Sales", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Transaction, "Sales.PointofSale", entity.App.Names.PointOfSale, "PointofSale", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Transaction, "Sales.Return", entity.App.Names.SalesReturn, "SalesReturn", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Financial, "Configs.AccountUtility", entity.App.Names.AccountUtility, "Accounts", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Financial, "Commercial.AccountsRecievable", entity.App.Names.AccountsReceivable, "Money", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Inventory, "Sales.PackingList", entity.App.Names.PackingList, "ProductSend", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Inventory, "Sales.Packing", entity.App.Names.PackingList, "", Version.Medium);

            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.SalesBudgetSummary", entity.App.Names.SalesBudgetSummary, "Reports", Version.Full);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.SalesOrderSummary", entity.App.Names.SalesOrderSummary, "Reports", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.SalesInvoiceSummary", entity.App.Names.SalesInvoiceSummary, "Reports", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.SalesByItem", entity.App.Names.SalesByItem, "Reports", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.CostOfGoodsSold", entity.App.Names.CostOfGoodsSold, "Reports", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.Contacts", entity.App.Names.Contact, "Reports", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.SalesBySalesRep", entity.App.Names.SalesbySalesman, "Reports", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.DeliveryByCustomer", entity.App.Names.DeliveryByCustomer, "Reports", Version.Full);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.SalesReturnDetail", entity.App.Names.SalesReturn, "Reports", Version.Full);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.SalesByVAT", entity.App.Names.SalesByVAT, "Reports", Version.Lite);


            //Purchase        //Module                 //Namespace      //App                 //Name                   //Img
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Form, "Commercial.Contact", entity.App.Names.Contact, "Contact", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Inventory, "Purchase.PackingList", entity.App.Names.PurchasePacking, "ProductSend", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Transaction, "Purchase.Tender", entity.App.Names.PurchaseTender, "PurchaseTender", Version.Full);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Transaction, "Purchase.Order", entity.App.Names.PurchaseOrder, "PurchaseOrder", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Transaction, "Purchase.Invoice", entity.App.Names.PurchaseInvoice, "Purchase", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Transaction, "Purchase.Return", entity.App.Names.PurchaseReturn, "PurchaseReturn", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Financial, "Configs.AccountUtility", entity.App.Names.AccountUtility, "Accounts", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Financial, "Commercial.AccountsPayable", entity.App.Names.AccountsPayable, "Money", Version.Basic);

            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Reports, "Reporting.Views.PurchaseTenderSummary", entity.App.Names.PurchaseTenderSummary, "Reports", Version.Full);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Reports, "Reporting.Views.PurchaseOrderSummary", entity.App.Names.PurchaseOrderSummary, "Reports", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Reports, "Reporting.Views.PurchaseInvoiceSummary", entity.App.Names.PurchaseInvoiceSummary, "Reports", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Reports, "Reporting.Views.Contacts", entity.App.Names.Contact, "Reports", Version.Lite);

            //Human Resources        //Module                 //Namespace      //App                 //Name                   //Img
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespace.Transaction, "HumanResource.Talent", entity.App.Names.Talent, "EmployeeTalent", Version.Full);
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespace.Form, "HumanResource.Employee", entity.App.Names.Employee, "EmployeeID", Version.Full);
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespace.Financial, "Configs.Hr_coefficient", entity.App.Names.Hr_coefficient, "EntryExit", Version.Full);

            //Stock
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Form, "Product.Item", entity.App.Names.Items, "Item", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Form, "Product.FixedAssets", entity.App.Names.FixedAssets, "FixedAsset", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Form, "Product.Recipe", entity.App.Names.Recipe, "Recipe", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Form, "Product.Stock", entity.App.Names.Stock, "ProductStock", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Form, "Product.BranchWalkin", entity.App.Names.FootTraffic, "FootTraffic", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transaction, "Product.Inventory", entity.App.Names.Inventory, "ProductStockAdjust", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transaction, "Product.Movement", entity.App.Names.Movement, "BranchLocation", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transaction, "Product.Transfer", entity.App.Names.Transfer, "ProductSend", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transaction, "Product.Request", entity.App.Names.RequestManagement, "ProjectPlan", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transaction, "Product.SuppliesRequest", entity.App.Names.RequestResource, "ProjectPlan", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transaction, "Product.Maintainance", entity.App.Names.Maintainance, "Maintainance", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Stock, entity.App.Modules.Configuration, "Product.ItemTag", entity.App.Names.ItemTag, "ProductTag", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Stock, entity.App.Modules.Configuration, "Product.PriceList", entity.App.Names.PriceList, "ProductPriceList", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Inventory, "Sales.PackingList", "PackingList", "ProductSend", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Reports, "Reporting.Views.Inventory", entity.App.Names.Inventory, "Reports", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Reports, "Reporting.Views.StockFlow", entity.App.Names.StockFlow, "Reports", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Reports, "Reporting.Views.ItemPriceList", entity.App.Names.ItemPriceList, "Reports", Version.Lite);


            //Impex
            dtApp.Rows.Add(entity.App.Modules.Impex, Namespace.Exports, "Sales.Export", entity.App.Names.Export, "Export", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Impex, Namespace.Importation, "Purchase.Import", entity.App.Names.Imports, "Import", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Impex, entity.App.Modules.Configuration, "Commercial.Incoterm", entity.App.Names.Incoterm, "IncotermContract", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Impex, entity.App.Modules.Configuration, "Configs.IncotermCondition", entity.App.Names.IncotermCondition, "", Version.Medium);

            //Finance
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Form, "Configs.AccountUtility", entity.App.Names.AccountUtility, "Accounts", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Transaction, "Commercial.AccountsPayable", entity.App.Names.AccountsPayable, "Money", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Transaction, "Commercial.AccountsRecievable", entity.App.Names.AccountsReceivable, "Money", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Transaction, "Commercial.Payments", entity.App.Names.Payment, "", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Transaction, "Commercial.PromissoryNote", entity.App.Names.PromissoryNote, "IOU", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Finance, entity.App.Modules.Configuration, "Configs.Currency", entity.App.Names.Currency, "Currency", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Finance, entity.App.Modules.Configuration, "Commercial.PaymentType", entity.App.Names.PaymentType, "", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Finance, entity.App.Modules.Configuration, "Configs.Bank", entity.App.Names.Bank, "BankAccount", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Reports, "Reporting.Views.CurrentAccount_Customer", entity.App.Names.CurrentAccountCustomer, "Reports", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Reports, "Reporting.Views.PendingSalesDocs", entity.App.Names.SalesDocuments, "Reports", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Reports, "Reporting.Views.AccountsRecievableAnalisys", entity.App.Names.AnalisysofAccountsReceivable, "Reports", Version.Medium);

            //Projects Printing Press
            dtApp.Rows.Add(entity.App.Modules.Project_PrintingPress, Namespace.Form, "Project.PrintingPress.Template", entity.App.Names.Template, "ProjectCategory", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Project_PrintingPress, Namespace.Transaction, "Project.PrintingPress.Estimate", "Costing", "ProjectTaskWizard", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Project_PrintingPress, Namespace.Reports, "Reporting.Views.Project", entity.App.Names.ActivityPlan, "Reports", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Project_PrintingPress, Namespace.Reports, "Reporting.Views.ProjectExecution", entity.App.Names.ProjectExecution, "Reports", Version.Medium);

            //Projects Event
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Form, "Project.EventType", entity.App.Names.Template, "ProjectCategory", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Transaction, "Project.EventCosting", entity.App.Names.EventManagement, "ProjectTaskWizard", Version.Medium);

            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Transaction, "Project.Development.TaskView", entity.App.Names.ActivityPlan, "ProjectTask", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Inventory, "Project.Development.Logistics", entity.App.Names.Logistics, "Logistics", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Financial, "Project.ProjectFinance", entity.App.Names.ProjectFinance, "ProjectSalesOrder", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Transaction, "Project.ProjectExecution", entity.App.Names.ProjectExecution, "", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Reports, "Reporting.Views.Project", entity.App.Names.ActivityPlan, "Reports", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Reports, "Reporting.Views.ProjectExecution", entity.App.Names.ProjectExecution, "Reports", Version.Medium);

            //Projects Plain
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Form, "Project.Development.ProjectType", entity.App.Names.Template, "ProjectCategory", Version.Enterprise);
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Form, "Project.Development.Project", entity.App.Names.Project, "Project", Version.Enterprise);
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Transaction, "Project.Development.TaskView", entity.App.Names.ActivityPlan, "ProjectTask", Version.Enterprise);
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Inventory, "Project.Development.Logistics", entity.App.Names.Logistics, "Logistics", Version.Enterprise);
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Transaction, "Project.ProjectExecution", entity.App.Names.ProjectExecution, "Plan", Version.Enterprise);
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Financial, "Project.ProjectFinance", entity.App.Names.ProjectFinance, "ProjectSalesOrder", Version.Enterprise);
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Reports, "Reporting.Views.Project", entity.App.Names.ActivityPlan, "Reports", Version.Enterprise);
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Reports, "Reporting.Views.ProjectExecution", entity.App.Names.ProjectExecution, "Reports", Version.Enterprise);

            //Production
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Form, "Production.Line", entity.App.Names.Line, "Line", Version.Full);
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Transaction, "Production.Order", entity.App.Names.ProductionOrder, "ProductionOrder", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Transaction, "Production.FractionOrder", entity.App.Names.ProductionbyFraction, "Fraction", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Transaction, "Production.Execution", entity.App.Names.ProductionExecution, "ProductionExecution", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Reports, "Reporting.Views.EmployeesInProduction", entity.App.Names.EmployeesInProduction, "Reports", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Reports, "Reporting.Views.ProductionOrderStatus", entity.App.Names.ProductionOrderStatus, "Reports", Version.Medium);

            //Accounting
            dtApp.Rows.Add(entity.App.Modules.Accounting, Namespace.Transaction, "Accounting.DebeHaberLogin", entity.App.Names.DebeHaberIntegration, "", Version.Lite);

            //Application
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Configs.Language", "Language", "Language", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Setup.Migration.MigrationAssistant", "MigrationAssistant", "MigrationAssistant", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Setup.Migration.Cogent.MigrationGUI", "MigrationGUI", "MigrationGUI", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Configs.ConnectionBuilder", "ConnectionBuilder", "Network", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Configs.Settings", "Settings", "", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Configs.KeyGestureSettings", "KeyGestureSettings", "", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Configs.Wallpaper", "Wallpaper", "Wallpaper", Version.Lite);

            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.Company", "Company", "HomeCompany", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.Branch", "Branch", "Branch", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.Terminal", "Terminal", "Terminal", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.Geography", "Geography", "Location", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.VAT", "VAT", "ProductPromotion", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.VATGroup", "VATGroup", "ProductPromotion", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Security.User", "User", "User", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Security.UserRole", "UserRole", "UserRole", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.Department", "Department", "", Version.Lite);

            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.Currency", "Currency", "Currency", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.AccountingCurrency", "AccountingCurrency", "Currency", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Commercial.PaymentType", "PaymentType", "PaymentType", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.Bank", "Bank", "BankAccount", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.Account", "Account", "BankAccountTrans", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.FiscalPeriod", "FiscalPeriod", "BankAccountTrans", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.Hr_coefficient", "Hr_coefficient", "", Version.Lite);

            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transaction, "Configs.Condition", "Condition", "Condition", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transaction, "Configs.Contract", "Contract", "Contract", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transaction, "Configs.Status", "Status", "Status", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transaction, "Configs.Document", "Document", "Document", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transaction, "Configs.DocumentRange", "DocumentRange", "DocumentRange", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transaction, "Configs.DocumentDesigner", "DocumentDesigner", "DocumentDesigner", Version.Lite);

            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Purchase, "Configs.CostCenter", "CostCenter", "AccountingChart", Version.Lite);

            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Sales, "Sales.Salesman", "Salesman", "SalesRep", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Sales, "Commercial.ContactRole", "ContactRole", "ContactRole", Version.Lite);

            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.Location", "Location", "BranchLocation", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.MeasurementType", "MeasurementType", "Measurement", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.Measurement", "Measurement", "Measurement", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.ItemBrand", "ItemBrand", "Product", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.Dimension", "Dimension", "", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.Property", "Property", "Product", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Product.ItemTag", "ItemTag", "Product", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Product.ItemTemplate", "ItemTemplate", "Product", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Sales, "Contact.ContactTag", "ContactTag", "Contact", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Sales, "Configs.ItemAssetGroup", "ItemAssetGroup", "ItemAssetGroup", Version.Lite);
            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Project_Dev, "Project.ProjectTag", "ProjectTag", "Project", Version.Lite);
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
