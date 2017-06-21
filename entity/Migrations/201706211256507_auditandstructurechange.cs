namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class auditandstructurechange : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "app_comment",
                c => new
                    {
                        id_comment = c.Int(nullable: false, identity: true),
                        id_application = c.Int(nullable: false),
                        comment = c.String(nullable: false, unicode: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_comment)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
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
            
            CreateTable(
                "purchase_packing_detail_relation",
                c => new
                    {
                        id_purchase_packing_detail_relation = c.Int(nullable: false, identity: true),
                        id_purchase_invoice_detail = c.Int(nullable: false),
                        id_purchase_packing_detail = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id_purchase_packing_detail_relation)                
                .ForeignKey("purchase_invoice_detail", t => t.id_purchase_invoice_detail, cascadeDelete: true)
                .ForeignKey("purchase_packing_detail", t => t.id_purchase_packing_detail, cascadeDelete: true)
                .Index(t => t.id_purchase_invoice_detail)
                .Index(t => t.id_purchase_packing_detail);
            
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
            DropForeignKey("purchase_packing_detail_relation", "id_purchase_packing_detail", "purchase_packing_detail");
            DropForeignKey("purchase_packing_detail_relation", "id_purchase_invoice_detail", "purchase_invoice_detail");
            DropForeignKey("AuditEntryProperties", "AuditEntryID", "AuditEntries");
            DropForeignKey("app_comment", "id_user", "security_user");
            DropForeignKey("app_comment", "id_company", "app_company");
            DropIndex("purchase_packing_detail_relation", new[] { "id_purchase_packing_detail" });
            DropIndex("purchase_packing_detail_relation", new[] { "id_purchase_invoice_detail" });
            DropIndex("AuditEntryProperties", new[] { "AuditEntryID" });
            DropIndex("app_comment", new[] { "id_user" });
            DropIndex("app_comment", new[] { "id_company" });
            DropColumn("item_product", "code");
            DropColumn("item_product", "variation");
            DropColumn("app_company", "serial");
            DropTable("purchase_packing_detail_relation");
            DropTable("AuditEntryProperties");
            DropTable("AuditEntries");
            DropTable("app_comment");
        }
    }
}
