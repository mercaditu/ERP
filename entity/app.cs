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
        }
        
       // [TypeConverter(typeof(EnumDescriptionTypeConverter))]
        public enum Names
        {
            /// <summary>
            /// Last Used : 53
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
            [LocalizedDescription("Contact")]
            Contact = 10,
            [LocalizedDescription("PaymentUtility")]
            PaymentUtility = 16,
            [LocalizedDescription("PaymentType")]
            PaymentType=51,
            [LocalizedDescription("ItemTransfer")]
            PaymentWithHolding = 31,
            [LocalizedDescription("AccountUtility")]
            AccountUtility = 44,
            [LocalizedDescription("Currency")]
            Currency = 46,
            [LocalizedDescription("Currency")]
            Bank = 47,

            //Stock
            [LocalizedDescription("Item")]
            Item = 11,
            [LocalizedDescription("Adjust")]
            Adjust = 12,
            [LocalizedDescription("Movement")]
            Movement = 13,
            [LocalizedDescription("Import")]
            Import= 14,
            [LocalizedDescription("IncotermCondition")]
            IncotermCondition = 45,
            [LocalizedDescription("Incoterm")]
            Incoterm = 33,
            [LocalizedDescription("Export")]
            Export = 34,
            [LocalizedDescription("RequestResource")]
            RequestResource = 18,
            [LocalizedDescription("Inventory")]
            Inventory = 26,
            [LocalizedDescription("Transfer")]
            Transfer = 30,
            [LocalizedDescription("ItemReceipe")]
            Receipe = 32,
            [LocalizedDescription("Stock")]
            Stock = 41,
            [LocalizedDescription("ItemTag")]
            ItemTag = 42,
            [LocalizedDescription("PriceList")]
            PriceList = 43,

            //Production
            [LocalizedDescription("ProductionExecution")]
            ProductionExecution = 17,
            [LocalizedDescription("ProductionLine")]
            Line=19,
            [LocalizedDescription("ProductionOrder")]
            ProductionOrder=20,
            [LocalizedDescription("ProductionTemplate")]
            ProductionTemplate=21,

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
            //[LocalizedDescription("ProjectLogistics")]
            //EventType = 39,
            [LocalizedDescription("EventCost")]
            EventCost = 39,
            [LocalizedDescription("PrintingPresstemplate")]
            PrintingPresstemplate = 40,
            [LocalizedDescription("ProjectInvoice")]
            ProjectInvoice = 53,

            //HR
            [LocalizedDescription("Employee")]
            Employee = 35,
            [LocalizedDescription("Talent")]
            Talent = 36,
            [LocalizedDescription("HourCoeficient")]
            HourCoeficient = 48,
        }
    }
}
