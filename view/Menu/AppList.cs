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

            //Sales        //Module                 //Namespace      //App                 //Name                   //Img
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Form, "Commercial.Contact", entity.App.Names.Contact, "Contact");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Transaction, "Sales.Budget", entity.App.Names.SalesBudget, "");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Transaction, "Sales.Order", entity.App.Names.SalesOrder, "SalesOrder");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Transaction, "Sales.Invoice", entity.App.Names.SalesInvoice, "Sales");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Transaction, "Sales.PointofSale", entity.App.Names.PointOfSale, "PointofSale");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Transaction, "Sales.Return", entity.App.Names.SalesReturn, "SalesReturn");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Financial, "Configs.AccountUtility", entity.App.Names.AccountUtility, "BankAccountTrans");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Financial, "Commercial.AccountsRecievable", entity.App.Names.AccountsReceivable, "Money");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Inventory, "Sales.PackingList", entity.App.Names.PackingList, "ProductSend");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Inventory, "Sales.Packing", entity.App.Names.PackingList, "");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.SalesByDate", entity.App.Names.SalesbyDate, "Reports");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.SalesByItem", entity.App.Names.SalesByItem, "Reports");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.CostOfGoodsSold", entity.App.Names.CostOfGoodsSold, "Reports");
            dtApp.Rows.Add(entity.App.Modules.Sales, Namespace.Reports, "Reporting.Views.Contacts", entity.App.Names.Contact, "Reports");

            //Purchase        //Module                 //Namespace      //App                 //Name                   //Img
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Form, "Commercial.Contact", entity.App.Names.Contact, "Contact");
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Transaction, "Purchase.Tender", entity.App.Names.PurchaseTender, "PurchaseTender");
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Transaction, "Purchase.Order", entity.App.Names.PurchaseOrder, "PurchaseOrder");
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Transaction, "Purchase.Invoice", entity.App.Names.PurchaseInvoice, "Purchase");
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Transaction, "Purchase.Return", entity.App.Names.PurchaseReturn, "PurchaseReturn");
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Financial, "Configs.AccountUtility", entity.App.Names.AccountUtility, "BankAccountTrans");
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Financial, "Commercial.AccountsPayable", entity.App.Names.AccountsPayable, "Money");
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Inventory, "Purchase.PackingList", "PackingList", "ProductRecieve");
            dtApp.Rows.Add(entity.App.Modules.Purchase, Namespace.Reports, "Reporting.Views.Contacts", entity.App.Names.Contact, "Reports");

            //Human Resources        //Module                 //Namespace      //App                 //Name                   //Img
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespace.Transaction, "HumanResource.Clock", "Clock", "Clock");
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespace.Transaction, "HumanResource.Interview", "EmployeeTalent", "EmployeeTalent");
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespace.Transaction, "HumanResource.Talent", entity.App.Names.Talent, "EmployeeTalent");
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespace.Form, "HumanResource.Employee", entity.App.Names.Employee, "EmployeeID");
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespace.Financial, "HumanResource.CurrentAccount", "CurrentAccount", "");
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespace.Financial, "HumanResource.Payroll", "Payroll", "Money");
            dtApp.Rows.Add(entity.App.Modules.HumanResources, Namespace.Financial, "Configs.Hr_coefficient", entity.App.Names.HourCoeficient, "EntryExit");

            //Stock
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Form, "Product.Item", entity.App.Names.Item, "Product");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Form, "Product.FixedAssets", entity.App.Names.FixedAssets, "");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Form, "Product.Recipe", entity.App.Names.Recipe, "Recipe");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Form, "Product.Stock", entity.App.Names.Stock, "ProductStock");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transaction, "Product.Inventory", entity.App.Names.Inventory, "ProductStockAdjust");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transaction, "Product.Movement", entity.App.Names.Movement, "BranchLocation");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transaction, "Product.Transfer", entity.App.Names.Transfer, "ProductSend");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transaction, "Product.Request", entity.App.Names.RequestResource, "ProjectPlan");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transaction, "Product.SuppliesRequest", entity.App.Names.RequestResource, "ProjectPlan");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Transaction, "Product.Maintainance", entity.App.Names.Maintainance, "");
            dtApp.Rows.Add(entity.App.Modules.Stock, entity.App.Modules.Configuration, "Product.ItemTag", entity.App.Names.ItemTag, "ProductTag");
            dtApp.Rows.Add(entity.App.Modules.Stock, entity.App.Modules.Configuration, "Product.PriceList", entity.App.Names.PriceList, "ProductPriceList");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Inventory, "Sales.PackingList", "PackingList", "ProductSend");
            dtApp.Rows.Add(entity.App.Modules.Stock, Namespace.Reports, "Reporting.Views.Inventory", entity.App.Names.Inventory, "Reports");

            //Impex
            dtApp.Rows.Add(entity.App.Modules.Impex, Namespace.Exports, "Sales.Export", entity.App.Names.Export, "Export");
            dtApp.Rows.Add(entity.App.Modules.Impex, Namespace.Importation, "Purchase.Import", entity.App.Names.Import, "Import");
            dtApp.Rows.Add(entity.App.Modules.Impex, entity.App.Modules.Configuration, "Commercial.Incoterm", entity.App.Names.Incoterm, "");
            dtApp.Rows.Add(entity.App.Modules.Impex, entity.App.Modules.Configuration, "Configs.IncotermCondition", entity.App.Names.IncotermCondition, "");

            //Finance
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Form, "Configs.AccountUtility", entity.App.Names.AccountUtility, "BankAccountTrans");
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Transaction, "Commercial.AccountsPayable", entity.App.Names.AccountsPayable, "Money");
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Transaction, "Commercial.AccountsRecievable", entity.App.Names.AccountsReceivable, "Money");
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Transaction, "Commercial.Payments", entity.App.Names.Payments, "");
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Transaction, "Commercial.PromissoryNote", entity.App.Names.PromissoryNote, "");
            dtApp.Rows.Add(entity.App.Modules.Finance, entity.App.Modules.Configuration, "Configs.Currency", entity.App.Names.Currency, "Currency");
            dtApp.Rows.Add(entity.App.Modules.Finance, entity.App.Modules.Configuration, "Commercial.PaymentType", entity.App.Names.PaymentType, "");
            dtApp.Rows.Add(entity.App.Modules.Finance, entity.App.Modules.Configuration, "Configs.Bank", entity.App.Names.Bank, "BankAccount");
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Reports, "Reporting.Views.CurrentAccount_Customer", entity.App.Names.CurrentAccount_Customer, "Reports");
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Reports, "Reporting.Views.PendingSalesDocs", entity.App.Names.PendingSalesDocs, "Reports");
            dtApp.Rows.Add(entity.App.Modules.Finance, Namespace.Reports, "Reporting.Views.AccountsRecievableAnalisys", entity.App.Names.PendingSalesDocs, "Reports");

            //Projects Printing Press
            dtApp.Rows.Add(entity.App.Modules.Project_PrintingPress, Namespace.Form, "Project.PrintingPress.Template", entity.App.Names.Template, "ProjectCategory");
            dtApp.Rows.Add(entity.App.Modules.Project_PrintingPress, Namespace.Transaction, "Project.PrintingPress.Estimate", "Costing", "ProjectTaskWizard");

            //Projects Event
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Form, "Project.EventType", entity.App.Names.Template, "ProjectCategory");
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Transaction, "Project.EventCosting", entity.App.Names.EventManagement, "ProjectTaskWizard");

            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Transaction, "Project.Development.TaskView", entity.App.Names.ActivityPlan, "ProjectTask");
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Transaction, "Project.Development.Logistics", entity.App.Names.Logistics, "Logistics");
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Transaction, "Project.ProjectExecution", entity.App.Names.ProjectExecution, "");
            dtApp.Rows.Add(entity.App.Modules.Project_Event, Namespace.Transaction, "Project.ProjectFinance", entity.App.Names.ProjectFinance, "ProjectSalesOrder");


            //Projects Plain
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Form, "Project.Development.ProjectType", entity.App.Names.Template, "ProjectCategory");
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Form, "Project.Development.Project", entity.App.Names.Project, "Project");
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Transaction, "Project.Development.TaskView", entity.App.Names.ActivityPlan, "ProjectTask");
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Transaction, "Project.Development.Logistics", entity.App.Names.Logistics, "Logistics");
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Transaction, "Project.ProjectExecution", entity.App.Names.ProjectExecution, "Plan");
            dtApp.Rows.Add(entity.App.Modules.Project_Dev, Namespace.Transaction, "Project.ProjectFinance", entity.App.Names.ProjectFinance, "ProjectSalesOrder");

            //Production
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Form, "Production.Line", entity.App.Names.Line, "");
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Transaction, "Production.Order", entity.App.Names.ProductionOrder, "ProductionOrder");
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Transaction, "Production.FractionOrder", entity.App.Names.ProductionOrder, "ProductionOrder");
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Transaction, "Production.Execution", entity.App.Names.ProductionExecution, "ProductionExecution");
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Transaction, "Production.FractionExecution", entity.App.Names.ProductionExecution, "ProductionExecution");
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Transaction, "Production.QualityAssurance", "QualityAssurance", "");
            dtApp.Rows.Add(entity.App.Modules.Production, Namespace.Transaction, "Production.ShopFloor", "Factory", "");

            //Accounting
            dtApp.Rows.Add(entity.App.Modules.Accounting, Namespace.Form, "Accounting.Cycle", entity.App.Names.FiscalPeriod, "AccountingPeriod");
            dtApp.Rows.Add(entity.App.Modules.Accounting, Namespace.Form, "Accounting.ChartOfAccounts", entity.App.Names.ChartofAccount, "AccountingChart");
            dtApp.Rows.Add(entity.App.Modules.Accounting, Namespace.Transaction, "Accounting.Journal", entity.App.Names.AccountingJournal, "AccountingTransaction");
            dtApp.Rows.Add(entity.App.Modules.Accounting, Namespace.Transaction, "Accounting.ExpenseJournal", entity.App.Names.BookofExpenses, "ExpenseBook");
            dtApp.Rows.Add(entity.App.Modules.Accounting, Namespace.Transaction, "Accounting.IncomeJournal", entity.App.Names.BookofIncomes, "IncomeBook");
            dtApp.Rows.Add(entity.App.Modules.Accounting, Namespace.Transaction, "Accounting.DebeHaberSync", entity.App.Names.DebeHaberSync, "");

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
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.AccountingCurrency", "AccountingCurrency", "Currency");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Commercial.PaymentType", "PaymentType", "PaymentType");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.Bank", "Bank", "BankAccount");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.Account", "Account", "BankAccountTrans");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.FiscalPeriod", "FiscalPeriod", "BankAccountTrans");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Financial, "Configs.Hr_coefficient", "Hr_coefficient", "");
        
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transaction, "Configs.Condition", "Condition", "Condition");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transaction, "Configs.Contract", "Contract", "Contract");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transaction, "Configs.Status", "Status", "Status");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transaction, "Configs.Document", "Document", "Document");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transaction, "Configs.DocumentRange", "DocumentRange", "DocumentRange");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Transaction, "Configs.DocumentDesigner", "DocumentDesigner", "DocumentDesigner");

            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Purchase, "Configs.CostCenter", "CostCenter", "AccountingChart");

            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Sales, "Sales.Salesman", "Salesman", "SalesRep");
            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Sales, "Commercial.ContactRole", "ContactRole", "ContactRole");

            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.Location", "Location", "BranchLocation");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.MeasurementType", "MeasurementType", "Measurement");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.Measurement", "Measurement", "Measurement");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.ItemBrand", "ItemBrand", "Product");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.Dimension", "Dimension", "");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Configs.Property", "Property", "");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Product.ItemTag", "ItemTag", "Product");
            dtApp.Rows.Add(entity.App.Modules.Configuration, Namespace.Inventory, "Product.ItemTemplate", "ItemTemplate", "Product");
            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Sales, "Contact.ContactTag", "ContactTag", "Contact");
            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Sales, "Configs.ItemAssetGroup", "ItemAssetGroup", "ItemAssetGroup");
            dtApp.Rows.Add(entity.App.Modules.Configuration, entity.App.Modules.Project_Dev, "Project.ProjectTag", "ProjectTag", "Project");
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
