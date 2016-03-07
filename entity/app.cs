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
            /// Last Used : 31
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
            [LocalizedDescription("AccountsPayable")]
            AccountsPayable = 9,
            [LocalizedDescription("AccountsRecievable")]
            AccountsRecievable = 27,
            [LocalizedDescription("AccountJournal")]
            AccountJournal = 29,
            [LocalizedDescription("Contact")]
            Contact = 10,
            [LocalizedDescription("PaymentUtility")]
            PaymentUtility = 16,
            [LocalizedDescription("PaymentType")]
            PaymentType=29,
            [LocalizedDescription("ItemTransfer")]
            PaymentWithHolding = 31,

            //Stock
            [LocalizedDescription("Item")]
            Item = 11,
            [LocalizedDescription("Adjust")]
            Adjust = 12,
            [LocalizedDescription("Movement")]
            Movement = 13,
            [LocalizedDescription("ImportExport")]
            ImportExport = 14,
            [LocalizedDescription("ItemRequest")]
            ItemRequest = 18,
            [LocalizedDescription("Inventory")]
            Inventory = 26,
            [LocalizedDescription("ItemTransfer")]
            ItemTransfer = 30,

            //Production
            [LocalizedDescription("ProductionExecustion")]
            ProductionExecustion =17,
            [LocalizedDescription("ProductionLine")]
            ProductionLine=19,
            [LocalizedDescription("ProductionOrder")]
            ProductionOrder=20,
            [LocalizedDescription("ProductionTemplate")]
            ProductionTemplate=21,

            //Project
            [LocalizedDescription("Project")]
            Project = 22,
            [LocalizedDescription("ActivityPlan")]
            ActivityPlan = 28,
            [LocalizedDescription("ProjectExecustion")]
            ProjectExecustion = 23,
            [LocalizedDescription("ProjectType")]
            ProjectType = 24,
            [LocalizedDescription("ProjectLogistics")]
            ProjectLogistics = 25,


        }
    }
}
