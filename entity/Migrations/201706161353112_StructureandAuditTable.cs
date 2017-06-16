namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StructureandAuditTable : DbMigration
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
                "AuditLogs",
                c => new
                    {
                        AuditLogId = c.Long(nullable: false, identity: true),
                        UserName = c.String(unicode: false),
                        EventDateUTC = c.DateTime(nullable: false, precision: 0),
                        EventType = c.Int(nullable: false),
                        TypeFullName = c.String(nullable: false, maxLength: 512, storeType: "nvarchar"),
                        RecordId = c.String(nullable: false, maxLength: 256, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.AuditLogId)                ;
            
            CreateTable(
                "AuditLogDetails",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PropertyName = c.String(nullable: false, maxLength: 256, storeType: "nvarchar"),
                        OriginalValue = c.String(unicode: false),
                        NewValue = c.String(unicode: false),
                        AuditLogId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)                
                .ForeignKey("AuditLogs", t => t.AuditLogId, cascadeDelete: true)
                .Index(t => t.AuditLogId);
            
            CreateTable(
                "LogMetadata",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AuditLogId = c.Long(nullable: false),
                        Key = c.String(unicode: false),
                        Value = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id)                
                .ForeignKey("AuditLogs", t => t.AuditLogId, cascadeDelete: true)
                .Index(t => t.AuditLogId);
            
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
            DropForeignKey("LogMetadata", "AuditLogId", "AuditLogs");
            DropForeignKey("AuditLogDetails", "AuditLogId", "AuditLogs");
            DropForeignKey("app_comment", "id_user", "security_user");
            DropForeignKey("app_comment", "id_company", "app_company");
            DropIndex("LogMetadata", new[] { "AuditLogId" });
            DropIndex("AuditLogDetails", new[] { "AuditLogId" });
            DropIndex("app_comment", new[] { "id_user" });
            DropIndex("app_comment", new[] { "id_company" });
            DropColumn("item_product", "code");
            DropColumn("item_product", "variation");
            DropColumn("app_company", "serial");
            DropTable("LogMetadata");
            DropTable("AuditLogDetails");
            DropTable("AuditLogs");
            DropTable("app_comment");
        }
    }
}
