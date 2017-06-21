namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class auditandstructurechange : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "AuditEntries",
                c => new
                    {
                        AuditEntryID = c.Int(nullable: false, identity: true),
                        EntitySetName = c.String(maxLength: 255, storeType: "nvarchar"),
                        EntityTypeName = c.String(maxLength: 255, storeType: "nvarchar"),
                        State = c.Int(nullable: false),
                        StateName = c.String(maxLength: 255, storeType: "nvarchar"),
                        CreatedBy = c.String(maxLength: 255, storeType: "nvarchar"),
                        CreatedDate = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.AuditEntryID)                ;
            
            CreateTable(
                "AuditEntryProperties",
                c => new
                    {
                        AuditEntryPropertyID = c.Int(nullable: false, identity: true),
                        AuditEntryID = c.Int(nullable: false),
                        RelationName = c.String(maxLength: 255, storeType: "nvarchar"),
                        PropertyName = c.String(maxLength: 255, storeType: "nvarchar"),
                        OldValue = c.String(unicode: false),
                        NewValue = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.AuditEntryPropertyID)                
                .ForeignKey("AuditEntries", t => t.AuditEntryID, cascadeDelete: true)
                .Index(t => t.AuditEntryID);
            
            AddColumn("app_company", "serial", c => c.String(unicode: false));
            AddColumn("item_product", "variation", c => c.String(unicode: false));
            AddColumn("item_product", "code", c => c.String(unicode: false));
            DropColumn("app_company", "version");
            DropColumn("purchase_invoice_dimension", "id_purchase_invoice_detail");
            DropColumn("purchase_order_dimension", "id_purchase_order_detail");
            DropColumn("purchase_packing_dimension", "id_purchase_packing_detail");
            DropColumn("sales_packing_relation", "id_sales_invoice_detail");
            DropColumn("purchase_return_dimension", "id_purchase_return_detail");
            DropColumn("item_recepie", "id_item");
            DropColumn("item_recepie_detail", "id_recepie");
            DropColumn("item_recepie_detail", "id_item");
        }
        
        public override void Down()
        {
            AddColumn("item_recepie_detail", "id_item", c => c.Decimal(nullable: false, precision: 20, scale: 9));
            AddColumn("item_recepie_detail", "id_recepie", c => c.Decimal(nullable: false, precision: 20, scale: 9));
            AddColumn("item_recepie", "id_item", c => c.Decimal(nullable: false, precision: 20, scale: 9));
            AddColumn("purchase_return_dimension", "id_purchase_return_detail", c => c.Long(nullable: false));
            AddColumn("sales_packing_relation", "id_sales_invoice_detail", c => c.Long(nullable: false));
            AddColumn("purchase_packing_dimension", "id_purchase_packing_detail", c => c.Long(nullable: false));
            AddColumn("purchase_order_dimension", "id_purchase_order_detail", c => c.Long(nullable: false));
            AddColumn("purchase_invoice_dimension", "id_purchase_invoice_detail", c => c.Long(nullable: false));
            AddColumn("app_company", "version", c => c.String(unicode: false));
            DropForeignKey("AuditEntryProperties", "AuditEntryID", "AuditEntries");
            DropIndex("AuditEntryProperties", new[] { "AuditEntryID" });
            DropColumn("item_product", "code");
            DropColumn("item_product", "variation");
            DropColumn("app_company", "serial");
            DropTable("AuditEntryProperties");
            DropTable("AuditEntries");
        }
    }
}
