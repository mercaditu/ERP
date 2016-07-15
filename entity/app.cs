using entity.Class;
using System.ComponentModel;
namespace entity
{
    public class App
    {
        //[TypeConverter(typeof(EnumDescriptionTypeConverter))]
        public enum Modules
        {
            [LocalizedDescription("Sales")]
            Sales,
            [LocalizedDescription("Purchase")]
            Purchase,
            [LocalizedDescription("Stock")]
            Stock,
            [LocalizedDescription("Impex")]
            Impex,
            [LocalizedDescription("Finance")]
            Finance,
            [LocalizedDescription("Accounting")]
            Accounting,
            [LocalizedDescription("HumanResources")]
            HumanResources,
            [LocalizedDescription("Project_Dev")]
            Project_Dev,
            [LocalizedDescription("Project_Event")]
            Project_Event,
            [LocalizedDescription("Project_PrintingPress")]
            Project_PrintingPress,
            [LocalizedDescription("Production")]
            Production,
            [LocalizedDescription("Configuration")]
            Configuration,
            [LocalizedDescription("Report")]
            Report,
        }

        // [TypeConverter(typeof(EnumDescriptionTypeConverter))]
        public enum Names
        {
            /// <summary>
            /// Last Used : 96
            /// </summary>

            //Sales
            [LocalizedDescription("SalesOrder")]
            SalesOrder = 1,
            [LocalizedDescription("SalesInvoice")]
            SalesInvoice = 2,
            [LocalizedDescription("SalesReturn")]
            SalesReturn = 3,
            [LocalizedDescription("PackingList")]
            PackingList = 4,
            [LocalizedDescription("SalesBudget")]
            SalesBudget = 15,
            [LocalizedDescription("PointOfSale")]
            PointOfSale = 78,

            //Purchase
            [LocalizedDescription("PurchaseTender")]
            PurchaseTender = 5,
            [LocalizedDescription("PurchaseOrder")]
            PurchaseOrder = 6,
            [LocalizedDescription("PurchaseInvoice")]
            PurchaseInvoice = 7,
            [LocalizedDescription("PurchaseReturn")]
            PurchaseReturn = 8,

            //Commercial
            [LocalizedDescription("FiscalPeriod")]
            FiscalPeriod = 37,
            [LocalizedDescription("ChartofAccount")]
            ChartofAccount = 38,
            [LocalizedDescription("AccountsPayable")]
            AccountsPayable = 9,
            [LocalizedDescription("AccountsReceivable")]
            AccountsReceivable = 27,
            [LocalizedDescription("AccountingJournal")]
            AccountingJournal = 29,
            [LocalizedDescription("BookofExpenses")]
            BookofExpenses = 49,
            [LocalizedDescription("BookofIncomes")]
            BookofIncomes = 50,
            DebeHaberSync=90,
            [LocalizedDescription("Contact")]
            Contact = 10,
            [LocalizedDescription("ContactSubscription")]
            ContactSubscription = 92,
            [LocalizedDescription("PaymentUtility")]
            PaymentUtility = 16,
            [LocalizedDescription("Payment")]
            Payment = 93,
            [LocalizedDescription("PaymentType")]
            PaymentType = 51,
            [LocalizedDescription("ItemTransfer")]
            PaymentWithHolding = 31,
            [LocalizedDescription("AccountUtility")]
            AccountUtility = 44,
            [LocalizedDescription("Currency")]
            Currency = 46,
            [LocalizedDescription("Currency")]
            Bank = 47,
            [LocalizedDescription("PromissoryNote")]
            PromissoryNote = 85,
            

            //Stock
            [LocalizedDescription("Items")]
            Items = 11,
            [LocalizedDescription("Adjust")]
            Adjust = 12,
            [LocalizedDescription("Movement")]
            Movement = 13,
            [LocalizedDescription("Import")]
            Import = 14,
            [LocalizedDescription("IncotermCondition")]
            IncotermCondition = 45,
            [LocalizedDescription("Incoterm")]
            Incoterm = 33,
            [LocalizedDescription("Export")]
            Export = 34,
            [LocalizedDescription("RequestResource")]
            RequestResource = 18,
            [LocalizedDescription("SuppliesRequestResource")]
            SuppliesRequestResource = 95,
            [LocalizedDescription("Inventory")]
            Inventory = 26,
            [LocalizedDescription("Transfer")]
            Transfer = 30,
            [LocalizedDescription("Recipe")]
            Recipe = 32,
            [LocalizedDescription("Stock")]
            Stock = 41,
            [LocalizedDescription("ItemTag")]
            ItemTag = 42,
            [LocalizedDescription("PriceList")]
            PriceList = 43,
            [LocalizedDescription("FixedAssets")]
            FixedAssets = 80,
            [LocalizedDescription("Maintainance")]
            Maintainance = 81,
            [LocalizedDescription("ItemBarcode")]
            ItemBarcode = 94,
            [LocalizedDescription("BranchWalkin")]
            BranchWalkin = 96,

            //Production
            [LocalizedDescription("ProductionExecution")]
            ProductionExecution = 17,
            [LocalizedDescription("ProductionLine")]
            Line = 19,
            [LocalizedDescription("ProductionOrder")]
            ProductionOrder = 20,
            [LocalizedDescription("ProductionTemplate")]
            ProductionTemplate = 21,

            //Project
            [LocalizedDescription("Template")]
            Template = 52,
            [LocalizedDescription("Project")]
            Project = 22,
            [LocalizedDescription("ActivityPlan")]
            ActivityPlan = 28,
            [LocalizedDescription("ProjectExecution")]
            ProjectExecution = 23,
            [LocalizedDescription("ProjectType")]
            ProjectType = 24,
            [LocalizedDescription("Logistics")]
            Logistics = 25,
            [LocalizedDescription("EventManagement")]
            EventManagement = 39,
            [LocalizedDescription("PrintingPresstemplate")]
            PrintingPresstemplate = 40,
            [LocalizedDescription("ProjectFinance")]
            ProjectFinance = 53,

            //HR
            [LocalizedDescription("Employee")]
            Employee = 35,
            [LocalizedDescription("Talent")]
            Talent = 36,
            [LocalizedDescription("HourCoeficient")]
            HourCoeficient = 48,
            [LocalizedDescription("EmpRecievable")]
            EmpRecievable = 95,

            //Configuration
            [LocalizedDescription("User")]
            User = 54,
            [LocalizedDescription("UserRole")]
            UserRole = 55,
            [LocalizedDescription("Document")]
            Document = 56,
            [LocalizedDescription("Range")]
            Range = 57,
            [LocalizedDescription("Contract")]
            Contract = 58,
            [LocalizedDescription("Condition")]
            Condition = 59,
            [LocalizedDescription("Account")]
            Account = 60,
            [LocalizedDescription("Terminal")]
            Terminal = 61,
            [LocalizedDescription("VATGrouping")]
            VATGroup = 62,
            [LocalizedDescription("VAT")]
            VAT = 63,
            [LocalizedDescription("Geography")]
            Geography = 64,
            [LocalizedDescription("Department")]
            Department = 65,
            [LocalizedDescription("Company")]
            Company = 66,
            [LocalizedDescription("Branch")]
            Branch = 67,
            [LocalizedDescription("ContactRole")]
            ContactRole = 68,
            [LocalizedDescription("Salesman")]
            Salesman = 69,
            [LocalizedDescription("Location")]
            Location = 70,
            [LocalizedDescription("Measurement")]
            Measurement = 71,
            [LocalizedDescription("Brand")]
            Brand = 72,
            [LocalizedDescription("Tags")]
            Tags = 73,
            [LocalizedDescription("ContactTags")]
            ContactTags = 74,
            [LocalizedDescription("Dimension")]
            Dimension = 75,
            [LocalizedDescription("Type")]
            Type = 76,
            [LocalizedDescription("CostCenter")]
            CostCenter = 77,

            [LocalizedDescription("SalesbyDate")]
            SalesbyDate = 83,
            [LocalizedDescription("CurrentAccount_Customer")]
            CurrentAccount_Customer = 86,
            [LocalizedDescription("SalesByItem")]
            SalesByItem = 86,
            [LocalizedDescription("CostOfGoodsSold")]
            CostOfGoodsSold = 87,
            [LocalizedDescription("CostOfInventory")]
            CostOfInventory = 89,
            [LocalizedDescription("PendingSalesDocs")]
            PendingSalesDocs = 90,
            [LocalizedDescription("SuppliesRequest")]
            SuppliesRequest = 84,
            [LocalizedDescription("AccountsRecievableAnalisys")]
            AccountsRecievableAnalisys = 91,
            [LocalizedDescription("SalesBySalesRep")]
            SalesBySalesRep = 92,
            [LocalizedDescription("EmployeesInProduction")]
            EmployeesInProduction = 93,
            [LocalizedDescription("Fraction")]
            Fraction = 94,
            [LocalizedDescription("DebeHaberLogin")]
            DebeHaberLogin = 95
        }
    }
}
