using System;
using System.Data;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFLocalizeExtension.Extensions;
using entity;
namespace Cognitivo.Menu
{
    class AppList
    {
        public DataTable dtApp { get; set; }
        public DataTable dtModule { get; set; }

        enum Namespace
        {
            Form,
            Transactions,
            Financial,
            Inventory,
            Imports,
            Exports,

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

            //Sales
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Form, "Commercial.Contact", "Contact", "Contact");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Transactions, "Sales.Budget", "SalesBudget", "");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Transactions, "Sales.Order", "SalesOrder", "SalesOrder");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Transactions, "Sales.Invoice", "SalesInvoice", "Sales");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Transactions, "Sales.PointofSale", "PointofSale", "PointofSale");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Transactions, "Sales.Return", "SalesReturn", "SalesReturn");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Financial, "Configs.AccountUtility", "AccountUtility", "BankAccountTrans");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Financial, "Commercial.AccountsRecievable", "AccountsReceivable", "Money");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Inventory, "Sales.PackingList", "PackingList", "ProductSend");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Inventory, "Sales.Packing", "Packing", "");

            //Purchase
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Form, "Commercial.Contact", "Contact", "Contact");
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Transactions, "Purchase.Tender", "PurchaseTender", "PurchaseTender");
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Transactions, "Purchase.Order", "PurchaseOrder", "PurchaseOrder");
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Transactions, "Purchase.Invoice", "PurchaseInvoice", "Purchase");
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Transactions, "Purchase.Return", "PurchaseReturn", "PurchaseReturn");
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Financial, "Configs.AccountUtility", "AccountUtility", "BankAccountTrans");
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Financial, "Commercial.AccountsPayable", "AccountsPayable", "Money");
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Inventory, "Purchase.PackingList", "PackingList", "ProductRecieve");

            ///Human Resources
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespace.Transactions, "HumanResource.Clock", "Clock", "Clock");
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespace.Transactions, "HumanResource.Interview", "EmployeeTalent", "EmployeeTalent");
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespace.Transactions, "HumanResource.Talent", "Talent", "EmployeeTalent");
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespace.Form, "HumanResource.Employee", "Employee", "EmployeeID");
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespace.Financial, "HumanResource.CurrentAccount", "CurrentAccount", "");
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespace.Financial, "HumanResource.Payroll", "Payroll", "Money");

            //Stock
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Form, "Product.Item", "Item", "Product");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Form, "Product.Recipe", "Recipe", "Recipe");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Form, "Product.Stock", "Stock", "ProductStock");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transactions, "Product.Inventory", "Inventory", "ProductStockAdjust");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transactions, "Product.Movement", "Movement", "");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transactions, "Product.Transfer", "Transfer", "");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transactions, "Product.Request", "RequestResource", "");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transactions, "Product.Maintainance", "Maintainance", "");
            dtApp.Rows.Add(entity.App.Modules.Stock, entity.App.Modules.Configuration, "Product.ItemTag", "ItemTag", "ProductTag");
            dtApp.Rows.Add(entity.App.Modules.Stock, entity.App.Modules.Configuration, "Product.PriceList", "PriceList", "ProductPriceList");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Inventory, "Sales.PackingList", "PackingList", "ProductSend");

            //Impex
            dtApp.Rows.Add(entity.App.Modules.Impex, Namespace.Exports, "Sales.Export", "Export", "Export");
            dtApp.Rows.Add(entity.App.Modules.Impex, Namespace.Imports, "Purchase.Import", "Import", "Import");
            dtApp.Rows.Add(entity.App.Modules.Impex, entity.App.Modules.Configuration, "Commercial.Incoterm", "Incoterm", "");
            dtApp.Rows.Add(entity.App.Modules.Impex, entity.App.Modules.Configuration, "Configs.IncotermCondition", "IncotermCondition", "");

            //Finance
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Form, "Configs.AccountUtility", "AccountUtility", "BankAccountTrans");
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Transactions, "Commercial.AccountsPayable", "AccountsPayable", "Money");
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Transactions, "Commercial.AccountsRecievable", "AccountsReceivable", "Money");
            dtApp.Rows.Add(entity.App.Modules.Finance, entity.App.Modules.Configuration, "Configs.Currency", "Currency", "Currency");
            dtApp.Rows.Add(entity.App.Modules.Finance, entity.App.Modules.Configuration, "Commercial.PaymentType", "PaymentType", "");
            dtApp.Rows.Add(entity.App.Modules.Finance, entity.App.Modules.Configuration, "Configs.Bank", "Bank", "BankAccount");
            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Configuration, "Configs.Hr_coefficient", "Hr_coefficient", "");
           

            //Projects Printing Press
            dtApp.Rows.Add(entity.App.Modules.Project_PrintingPress, Namespace.Form, "Project.PrintingPress.Template", "Template", "ProjectCategory");
            dtApp.Rows.Add(entity.App.Modules.Project_PrintingPress, Namespace.Transactions, "Project.PrintingPress.Estimate", "Costing", "ProjectTaskWizard");

            //Projects Event
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Form, "Project.EventType", "Template", "ProjectCategory");
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Transactions, "Project.EventCosting", "Costing", "ProjectTaskWizard");
          
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Transactions, "Project.Development.TaskView", "ActivityPlan", "ProjectTask");
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Transactions, "Project.Development.Logistics", "Logistics", "Logistics");
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Transactions, "Project.ProjectExecution", "ProjectExecution", "");

            //Projects Plain
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Form, "Project.Development.ProjectType", "Template", "ProjectCategory");
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Transactions, "Project.Development.Project", "Project", "Project");
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Transactions, "Project.Development.TaskView", "ActivityPlan", "ProjectTask");
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Transactions, "Project.Development.Logistics", "Logistics", "Logistics");
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Transactions, "Project.ProjectExecution", "ProjectExecution", "");
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Transactions, "Project.ProjectInvoice", "ProjectInvoice", "");

            //Production
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Transactions, "Production.Line", "Line", "");
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Transactions, "Production.Order", "ProductionOrder", "ProductionOrder");
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Transactions, "Production.Execution", "ProductionExecution", "ProductionExecution");
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Transactions, "Production.QualityAssurance", "QualityAssurance", "");
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Transactions, "Production.ShopFloor", "Factory", "");

            //Accounting
            dtApp.Rows.Add(entity.App.Modules.Accounting, Namespace.Form, "Accounting.Cycle", "FiscalPeriod", "AccountingPeriod");
            dtApp.Rows.Add(entity.App.Modules.Accounting, Namespace.Form, "Accounting.ChartOfAccounts", "ChartofAccount", "AccountingChart");
            dtApp.Rows.Add(entity.App.Modules.Accounting, Namespace.Transactions, "Accounting.Journal", "AccountingJournal", "");
            dtApp.Rows.Add(entity.App.Modules.Accounting, Namespace.Transactions, "Accounting.ExpenseJournal", "BookofExpenses", "ExpenseBook");
            dtApp.Rows.Add(entity.App.Modules.Accounting, Namespace.Transactions, "Accounting.IncomeJournal", "BookofIncomes", "IncomeBook");

            //Application
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Configs.Language", "Language", "Language");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Setup.Migration.MigrationAssistant", "MigrationAssistant", "MigrationAssistant");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Setup.Migration.Cogent.MigrationGUI", "MigrationGUI", "MigrationGUI");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Configs.ConnectionBuilder", "ConnectionBuilder", "Network");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Configs.Settings", "Settings", "");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Configs.KeyGestureSettings", "KeyGestureSettings", "");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Preferences, "Configs.Wallpaper", "Wallpaper", "Wallpaper");

            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.Company", "Company", "HomeCompany");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.Branch", "Branch", "Branch");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.Terminal", "Terminal", "Terminal");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.Geography", "Geography", "Location");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.VAT", "VAT", "ProductPromotion");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.VATGroup", "VATGroup", "ProductPromotion");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Security.User", "User", "User");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Security.UserRole", "UserRole", "UserRole");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Company, "Configs.Department", "Department", "");

            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.Currency", "Currency", "Currency");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Commercial.PaymentType", "PaymentType", "PaymentType");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.Bank", "Bank", "BankAccount");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.Account", "Account", "BankAccountTrans");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.FiscalPeriod", "FiscalPeriod", "BankAccountTrans");

            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transactions, "Configs.Condition", "Condition", "Condition");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transactions, "Configs.Contract", "Contract", "Contract");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transactions, "Configs.Status", "Status", "Status");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transactions, "Configs.Document", "Document", "Document");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transactions, "Configs.DocumentRange", "DocumentRange", "DocumentRange");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transactions, "Configs.DocumentDesigner", "DocumentDesigner", "DocumentDesigner");

            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Purchase, "Configs.CostCenter", "CostCenter", "AccountingChart");

            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Sales, "Sales.Representative", "Representative", "SalesRep");
            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Sales, "Commercial.ContactRole", "ContactRole", "ContactRole");

            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.Location", "Location", "BranchLocation");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.MeasurementType", "MeasurementType", "Measurement");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.Measurement", "Measurement", "Measurement");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.ItemBrand", "ItemBrand", "Product");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.Dimension", "Dimension", "");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.Property", "Property", "");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Product.ItemTag", "ItemTag", "Product");
            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Sales, "Contact.ContactTag", "ContactTag", "Contact");
        }

        public cntrl.applicationIcon get_AppIcon(DataRow app)
        {
            string _namespace = app["namespace"].ToString();
            string _app = app["app"].ToString();
            string _name = app["name"].ToString();
            string _img = app["img"].ToString();
            string _description = "desc_" + app["name"].ToString();
            
            cntrl.applicationIcon appIcon = new cntrl.applicationIcon();
            appIcon.Tag = "Cognitivo." + _app;
                        
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
            if (_img == "") { _img = "DefaultIcon"; }
            _img = "../Images/Application/128/" + _img + ".png";
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
