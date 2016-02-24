using System.ComponentModel;
namespace entity
{
    public class App
    {
        public enum Modules
        {
         //   [Description(Brillo.Localize.Text<string>("Sales"))]
            Sales,
           // [Description(Brillo.Localize.Text<string>("Purchase"))]
            Purchase,
           // [Description(Brillo.Localize.Text<string>("Stock"))]
            Stock,
            Impex,
            Finance,
            Accounting,
            HumanResources,
            Project_Dev,
            Project_Event,
            Project_PrintingPress,
            Production,
            Configuration,
        }

        public enum Names
        {
            /// <summary>
            /// Last Used : 29
            /// </summary>

            //Sales
            SalesOrder = 1,
            SalesInvoice = 2,
            SalesReturn = 3,
            PackingList = 4,
            SalesBudget = 15,

            //Purchase
            PurchaseTender = 5,
            PurchaseOrder = 6,
            PurchaseInvoice = 7,
            PurchaseReturn = 8,

            //Commercial
            AccountsPayable = 9,
            AccountsRecievable = 27,
            Contact = 10,
            PaymentUtility = 16,
            PaymentType=29,

            //Stock
            Item = 11,
            Adjust = 12,
            Movement = 13,
            ImportExport = 14,
            ItemRequest = 18,
            Inventory = 26,

            //Production
            ProductionExecustion =17,
            ProductionLine=19,
            ProductionOrder=20,
            ProductionTemplate=21,

            //Project
            Project = 22,
            ActivityPlan = 28,
            ProjectExecustion = 23,
            ProjectType = 24,
            ProjectLogistics = 25,


        }
    }
}
