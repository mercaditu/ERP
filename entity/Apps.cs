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
            [LocalizedDescription("InternationalCommerce")]
            InternationalCommerce,
            [LocalizedDescription("Finance")]
            Finance,
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
            /// Last Used : 136
            /// </summary>


            [LocalizedDescription("DocumentRange")]
            DocumentRange = 110,

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
            [LocalizedDescription("PackingList")]
            PurchasePacking = 106,
            [LocalizedDescription("PendingPayables")]
            PendingPayables = 133,

            //Commercial
            [LocalizedDescription("AccountsPayable")]
            AccountsPayable = 9,
            [LocalizedDescription("AccountsReceivable")]
            AccountsReceivable = 27,
            [LocalizedDescription("DebeHaberSync")]
            DebeHaberSync = 90,
            [LocalizedDescription("Contact")]
            Contact = 10,
            [LocalizedDescription("Subscription")]
            Subscription = 92,
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
            [LocalizedDescription("Reconciliation")]
            Reconciliation = 125,


            //Stock
            [LocalizedDescription("Items")]
            Items = 11,
            [LocalizedDescription("Adjust")]
            Adjust = 12,
            [LocalizedDescription("Movement")]
            Movement = 13,
            [LocalizedDescription("Imports")]
            Imports = 14,
            [LocalizedDescription("IncotermCondition")]
            IncotermCondition = 45,
            [LocalizedDescription("Incoterm")]
            Incoterm = 33,
            [LocalizedDescription("Export")]
            Export = 34,
            [LocalizedDescription("RequestManagement")]
            RequestManagement = 18,
            [LocalizedDescription("RequestResource")]
            RequestResource = 95,
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
            [LocalizedDescription("FootTraffic")]
            FootTraffic = 96,
            [LocalizedDescription("ItemTemplate")]
            ItemTemplate = 108,
            [LocalizedDescription("ItemProperty")]
            Property = 109,
            [LocalizedDescription("ItemAssetGroup")]
            ItemAssetGroup = 112,
            [LocalizedDescription("SalesPromotion")]
            SalesPromotion = 123,
            [LocalizedDescription("CostBreakDown")]
            CostBreakDown = 132,
            [LocalizedDescription("MerchandisewithCredits")]
            MerchandisewithCredits = 134,
            [LocalizedDescription("MerchandisewithDebits")]
            MerchandisewithDebits = 135,
            [LocalizedDescription("InventoryAnalysis")]
            InventoryAnalysis = 136,

            //Production
            [LocalizedDescription("ProductionExecution")]
            ProductionExecution = 17,
            [LocalizedDescription("ProductionLine")]
            Line = 19,
            [LocalizedDescription("ProductionOrder")]
            ProductionOrder = 20,
            [LocalizedDescription("ProductionTemplate")]
            ProductionTemplate = 21,
            [LocalizedDescription("ProductionOrderStatus")]
            ProductionOrderStatus = 131,

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
            [LocalizedDescription("EventFinance")]
            EventFinance = 129,
            [LocalizedDescription("PrintingPresstemplate")]
            PrintingPresstemplate = 40,
            [LocalizedDescription("ProjectFinance")]
            ProjectFinance = 53,
            [LocalizedDescription("ProjectTag")]
            ProjectTag = 111,


            //HR
            [LocalizedDescription("Employee")]
            Employee = 35,
            [LocalizedDescription("Talent")]
            Talent = 36,
            [LocalizedDescription("HourCoeficient")]
            HourCoeficient = 48,
            [LocalizedDescription("Position")]
            Position = 130,
            [LocalizedDescription("EmpRecievable")]
            EmpRecievable = 101,

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
            [LocalizedDescription("MeasurementType")]
            MeasurementType = 107,
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

            [LocalizedDescription("SalesInvoiceSummary")]
            SalesInvoiceSummary = 83,
            [LocalizedDescription("CurrentAccountCustomer")]
            CurrentAccountCustomer = 86,
            [LocalizedDescription("SalesByItem")]
            SalesByItem = 103,
            [LocalizedDescription("SalesByBrand")]
            SalesByBrand = 97,
            [LocalizedDescription("AvgSales")]
            AvgSales = 99,
            [LocalizedDescription("CostOfGoodsSold")]
            CostOfGoodsSold = 87,
            [LocalizedDescription("CostOfInventory")]
            CostOfInventory = 89,
            [LocalizedDescription("SalesDocuments")]
            SalesDocuments = 104,
            [LocalizedDescription("SuppliesRequest")]
            SuppliesRequest = 84,
            [LocalizedDescription("AnalisysofAccountsReceivable")]
            AnalisysofAccountsReceivable = 91,
            [LocalizedDescription("SalesbySalesman")]
            SalesbySalesman = 126,
            [LocalizedDescription("EmployeesInProduction")]
            EmployeesInProduction = 107,
            [LocalizedDescription("ProductionbyFraction")]
            ProductionbyFraction = 100,
            [LocalizedDescription("DebeHaberIntegration")]
            DebeHaberIntegration = 102,
            [LocalizedDescription("DeliveryByCustomer")]
            DeliveryByCustomer = 105,

            [LocalizedDescription("SalesVATDetail")]
            SalesVATDetail = 119,

            [LocalizedDescription("PurchaseVATDetail")]
            PurchaseVATDetail = 118,

            [LocalizedDescription("ItemPriceList")]
            ItemPriceList = 120,
            [LocalizedDescription("InventoryFlow")]
            InventoryFlow = 121,
            [LocalizedDescription("InventoryFlowDimension")]
            InventoryFlowDimension = 124,
            [LocalizedDescription("VATSales")]
            VATSales = 122,
            [LocalizedDescription("VATPurchase")]
            VATPurchase = 127,

            [LocalizedDescription("Version")]
            Version = 128,
            TechnicalReport = 136
        }
    }
}
