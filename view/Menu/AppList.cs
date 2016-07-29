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
            Starter,
            Basic,
            Medium,
            Full,
            Enterprise,
            EventManagement,
            PrintingPress
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
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Transaction, "Sales.PointofSale", entity.App.Names.PointOfSale, "PointofSale", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Transaction, "Sales.Return", entity.App.Names.SalesReturn, "SalesReturn", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Financial, "Configs.AccountUtility", entity.App.Names.AccountUtility, "BankAccountTrans", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Financial, "Commercial.AccountsRecievable", entity.App.Names.AccountsReceivable, "Money", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Inventory, "Sales.PackingList", entity.App.Names.PackingList, "ProductSend", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Inventory, "Sales.Packing", entity.App.Names.PackingList, "", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.SalesByDate", entity.App.Names.SalesbyDate, "Reports", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.SalesByItem", entity.App.Names.SalesByItem, "Reports", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.SalesByBrand", entity.App.Names.SalesByBrand, "Reports", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.SalesByTag", entity.App.Names.SalesByTag, "Reports", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.CostOfGoodsSold", entity.App.Names.CostOfGoodsSold, "Reports", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.Contacts", entity.App.Names.Contact, "Reports", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.SalesBySalesRep", entity.App.Names.SalesbySalesman, "Reports", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.SalesBudgetList", entity.App.Names.SalesBudget, "Reports", Version.Full);
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.DeliveryByCustomer", entity.App.Names.DeliveryByCustomer, "Reports", Version.Full);

            //Purchase        //Module                 //Namespace      //App                 //Name                   //Img
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Form, "Commercial.Contact", entity.App.Names.Contact, "Contact", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Inventory, "Purchase.PackingList", entity.App.Names.PurchasePacking, "ProductSend", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Transaction, "Purchase.Tender", entity.App.Names.PurchaseTender, "PurchaseTender", Version.Full);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Transaction, "Purchase.Order", entity.App.Names.PurchaseOrder, "PurchaseOrder", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Transaction, "Purchase.Invoice", entity.App.Names.PurchaseInvoice, "Purchase", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Transaction, "Purchase.Return", entity.App.Names.PurchaseReturn, "PurchaseReturn", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Financial, "Configs.AccountUtility", entity.App.Names.AccountUtility, "BankAccountTrans", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Financial, "Commercial.AccountsPayable", entity.App.Names.AccountsPayable, "Money", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Reports, "Reporting.Views.Contacts", entity.App.Names.Contact, "Reports", Version.Starter);

            //Human Resources        //Module                 //Namespace      //App                 //Name                   //Img
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespace.Transaction, "HumanResource.Clock", "Clock", "Clock", Version.Full);
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespace.Transaction, "HumanResource.Interview", "EmployeeTalent", "EmployeeTalent", Version.Full);
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespace.Transaction, "HumanResource.Talent", entity.App.Names.Talent, "EmployeeTalent", Version.Full);
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespace.Form, "HumanResource.Employee", entity.App.Names.Employee, "EmployeeID", Version.Full);
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespace.Financial, "HumanResource.Payroll", "Payroll", "Money", Version.Full);
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespace.Financial, "Configs.Hr_coefficient", entity.App.Names.HourCoeficient, "EntryExit", Version.Full);
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespace.Financial, "Commercial.EmpRecievable", entity.App.Names.AccountsReceivable, "Money", Version.Full);

            //Stock
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Form, "Product.Item", entity.App.Names.Items, "Product", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Form, "Product.FixedAssets", entity.App.Names.FixedAssets, "", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Form, "Product.Recipe", entity.App.Names.Recipe, "Recipe", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Form, "Product.Stock", entity.App.Names.Stock, "ProductStock", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Form, "Product.BranchWalkin", entity.App.Names.FootTraffic, "", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transaction, "Product.Inventory", entity.App.Names.Inventory, "ProductStockAdjust", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transaction, "Product.Movement", entity.App.Names.Movement, "BranchLocation", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transaction, "Product.Transfer", entity.App.Names.Transfer, "ProductSend", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transaction, "Product.Request", entity.App.Names.RequestManagement, "ProjectPlan", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transaction, "Product.SuppliesRequest", entity.App.Names.RequestResource, "ProjectPlan", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transaction, "Product.Maintainance", entity.App.Names.Maintainance, "", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Stock, entity.App.Modules.Configuration, "Product.ItemTag", entity.App.Names.ItemTag, "ProductTag", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Stock, entity.App.Modules.Configuration, "Product.PriceList", entity.App.Names.PriceList, "ProductPriceList", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Inventory, "Sales.PackingList", "PackingList", "ProductSend", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Reports, "Reporting.Views.Inventory", entity.App.Names.Inventory, "Reports", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Reports, "Reporting.Views.StockFlow", entity.App.Names.Inventory, "Reports", Version.Starter);

            //Impex
            dtApp.Rows.Add(entity.App.Modules.Impex, Namespace.Exports, "Sales.Export", entity.App.Names.Export, "Export", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Impex, Namespace.Importation, "Purchase.Import", entity.App.Names.Imports, "Import", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Impex, entity.App.Modules.Configuration, "Commercial.Incoterm", entity.App.Names.Incoterm, "", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Impex, entity.App.Modules.Configuration, "Configs.IncotermCondition", entity.App.Names.IncotermCondition, "", Version.Medium);

            //Finance
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Form, "Configs.AccountUtility", entity.App.Names.AccountUtility, "BankAccountTrans", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Transaction, "Commercial.AccountsPayable", entity.App.Names.AccountsPayable, "Money", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Transaction, "Commercial.AccountsRecievable", entity.App.Names.AccountsReceivable, "Money", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Transaction, "Commercial.Payments", entity.App.Names.Payment, "", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Transaction, "Commercial.PromissoryNote", entity.App.Names.PromissoryNote, "", Version.Medium);
            dtApp.Rows.Add(entity.App.Modules.Finance, entity.App.Modules.Configuration, "Configs.Currency", entity.App.Names.Currency, "Currency", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Finance, entity.App.Modules.Configuration, "Commercial.PaymentType", entity.App.Names.PaymentType, "", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Finance, entity.App.Modules.Configuration, "Configs.Bank", entity.App.Names.Bank, "BankAccount", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Reports, "Reporting.Views.CurrentAccount_Customer", entity.App.Names.CurrentAccountCustomer, "Reports", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Reports, "Reporting.Views.PendingSalesDocs", entity.App.Names.SalesDocuments, "Reports", Version.Basic);
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Reports, "Reporting.Views.AccountsRecievableAnalisys", entity.App.Names.AnalisysofAccountsReceivable, "Reports", Version.Medium);

            //Projects Printing Press
            dtApp.Rows.Add(entity.App.Modules.Project_PrintingPress, Namespace.Form, "Project.PrintingPress.Template", entity.App.Names.Template, "ProjectCategory", Version.PrintingPress);
            dtApp.Rows.Add(entity.App.Modules.Project_PrintingPress, Namespace.Transaction, "Project.PrintingPress.Estimate", "Costing", "ProjectTaskWizard", Version.PrintingPress);

            //Projects Event
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Form, "Project.EventType", entity.App.Names.Template, "ProjectCategory", Version.EventManagement);
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Transaction, "Project.EventCosting", entity.App.Names.EventManagement, "ProjectTaskWizard", Version.EventManagement);

            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Transaction, "Project.Development.TaskView", entity.App.Names.ActivityPlan, "ProjectTask", Version.EventManagement);
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Inventory, "Project.Development.Logistics", entity.App.Names.Logistics, "Logistics", Version.EventManagement);
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Transaction, "Project.ProjectExecution", entity.App.Names.ProjectExecution, "", Version.EventManagement);
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Financial, "Project.ProjectFinance", entity.App.Names.ProjectFinance, "ProjectSalesOrder", Version.EventManagement);

            //Projects Plain
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Form, "Project.Development.ProjectType", entity.App.Names.Template, "ProjectCategory", Version.Enterprise);
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Form, "Project.Development.Project", entity.App.Names.Project, "Project", Version.Enterprise);
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Transaction, "Project.Development.TaskView", entity.App.Names.ActivityPlan, "ProjectTask", Version.Enterprise);
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Inventory, "Project.Development.Logistics", entity.App.Names.Logistics, "Logistics", Version.Enterprise);
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Transaction, "Project.ProjectExecution", entity.App.Names.ProjectExecution, "Plan", Version.Enterprise);
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Financial, "Project.ProjectFinance", entity.App.Names.ProjectFinance, "ProjectSalesOrder", Version.Enterprise);
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Reports, "Reporting.Views.ProjectExecution", entity.App.Names.ProjectExecution, "Reports", Version.Enterprise);
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Reports, "Reporting.Views.Project", entity.App.Names.ActivityPlan, "Reports", Version.Enterprise);

            //Production
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Form, "Production.Line", entity.App.Names.Line, "", Version.Full);
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Transaction, "Production.Order", entity.App.Names.ProductionOrder, "ProductionOrder", Version.Full);
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Transaction, "Production.FractionOrder", entity.App.Names.ProductionbyFraction, "", Version.Full);
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Transaction, "Production.Execution", entity.App.Names.ProductionExecution, "ProductionExecution", Version.Full);
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Reports, "Reporting.Views.EmployeesInProduction", entity.App.Names.EmployeesInProduction, "Reports", Version.Full);

            //Accounting
            dtApp.Rows.Add(entity.App.Modules.Accounting, Namespace.Transaction, "Accounting.DebeHaberLogin", entity.App.Names.DebeHaberIntegration, "", Version.Starter);

            //Application
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Configs.Language", "Language", "Language", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Setup.Migration.MigrationAssistant", "MigrationAssistant", "MigrationAssistant", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Setup.Migration.Cogent.MigrationGUI", "MigrationGUI", "MigrationGUI", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Configs.ConnectionBuilder", "ConnectionBuilder", "Network", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Configs.Settings", "Settings", "", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Configs.KeyGestureSettings", "KeyGestureSettings", "", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Configs.Wallpaper", "Wallpaper", "Wallpaper", Version.Starter);

            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.Company", "Company", "HomeCompany", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.Branch", "Branch", "Branch", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.Terminal", "Terminal", "Terminal", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.Geography", "Geography", "Location", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.VAT", "VAT", "ProductPromotion", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.VATGroup", "VATGroup", "ProductPromotion", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Security.User", "User", "User", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Security.UserRole", "UserRole", "UserRole", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.Department", "Department", "", Version.Starter);

            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.Currency", "Currency", "Currency", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.AccountingCurrency", "AccountingCurrency", "Currency", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Commercial.PaymentType", "PaymentType", "PaymentType", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.Bank", "Bank", "BankAccount", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.Account", "Account", "BankAccountTrans", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.FiscalPeriod", "FiscalPeriod", "BankAccountTrans", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.Hr_coefficient", "Hr_coefficient", "", Version.Starter);

            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transaction, "Configs.Condition", "Condition", "Condition", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transaction, "Configs.Contract", "Contract", "Contract", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transaction, "Configs.Status", "Status", "Status", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transaction, "Configs.Document", "Document", "Document", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transaction, "Configs.DocumentRange", "DocumentRange", "DocumentRange", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transaction, "Configs.DocumentDesigner", "DocumentDesigner", "DocumentDesigner", Version.Starter);

            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Purchase, "Configs.CostCenter", "CostCenter", "AccountingChart", Version.Starter);

            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Sales, "Sales.Salesman", "Salesman", "SalesRep", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Sales, "Commercial.ContactRole", "ContactRole", "ContactRole", Version.Starter);

            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.Location", "Location", "BranchLocation", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.MeasurementType", "MeasurementType", "Measurement", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.Measurement", "Measurement", "Measurement", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.ItemBrand", "ItemBrand", "Product", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.Dimension", "Dimension", "", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.Property", "Property", "Product", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Product.ItemTag", "ItemTag", "Product", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Product.ItemTemplate", "ItemTemplate", "Product", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Sales, "Contact.ContactTag", "ContactTag", "Contact", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Sales, "Configs.ItemAssetGroup", "ItemAssetGroup", "ItemAssetGroup", Version.Starter);
            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Project_Dev, "Project.ProjectTag", "ProjectTag", "Project", Version.Starter);
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

            if (_description != "")
            { var appLocApplicationDescription = new LocTextExtension("Cognitivo:local:" + _description + "").SetBinding(appIcon, cntrl.applicationIcon.ApplicationDescriptionProperty); }
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
