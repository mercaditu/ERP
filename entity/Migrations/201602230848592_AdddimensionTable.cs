namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdddimensionTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "purchase_order_dimension",
                c => new
                    {
                        id_order_property = c.Long(nullable: false, identity: true),
                        id_purchase_order_detail = c.Long(nullable: false),
                        id_dimension = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 4),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        purchase_order_detail_id_purchase_order_detail = c.Int(),
                    })
                .PrimaryKey(t => t.id_order_property)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_dimension", t => t.id_dimension, cascadeDelete: true)
                .ForeignKey("purchase_order_detail", t => t.purchase_order_detail_id_purchase_order_detail)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_dimension)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.purchase_order_detail_id_purchase_order_detail);
            
            CreateTable(
                "purchase_tender_dimension",
                c => new
                    {
                        id_tender_property = c.Long(nullable: false, identity: true),
                        id_purchase_tender_item = c.Int(nullable: false),
                        id_dimension = c.Int(nullable: false),
                        id_measurement = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 4),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.id_tender_property)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_dimension", t => t.id_dimension, cascadeDelete: true)
                .ForeignKey("app_measurement", t => t.id_measurement, cascadeDelete: true)
                .ForeignKey("purchase_tender_item", t => t.id_purchase_tender_item, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_purchase_tender_item)
                .Index(t => t.id_dimension)
                .Index(t => t.id_measurement)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "purchase_return_dimension",
                c => new
                    {
                        id_return_property = c.Long(nullable: false, identity: true),
                        id_purchase_return_detail = c.Long(nullable: false),
                        id_dimension = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 4),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        purchase_return_detail_id_purchase_return_detail = c.Int(),
                    })
                .PrimaryKey(t => t.id_return_property)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_dimension", t => t.id_dimension, cascadeDelete: true)
                .ForeignKey("purchase_return_detail", t => t.purchase_return_detail_id_purchase_return_detail)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_dimension)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.purchase_return_detail_id_purchase_return_detail);
            
            CreateTable(
                "purchase_invoice_dimension",
                c => new
                    {
                        id_invoice_property = c.Long(nullable: false, identity: true),
                        id_purchase_invoice_detail = c.Long(nullable: false),
                        id_dimension = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 4),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        purchase_invoice_detail_id_purchase_invoice_detail = c.Int(),
                    })
                .PrimaryKey(t => t.id_invoice_property)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_dimension", t => t.id_dimension, cascadeDelete: true)
                .ForeignKey("purchase_invoice_detail", t => t.purchase_invoice_detail_id_purchase_invoice_detail)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_dimension)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.purchase_invoice_detail_id_purchase_invoice_detail);
            
            CreateTable(
                "item_request_dimension",
                c => new
                    {
                        id_request_property = c.Long(nullable: false, identity: true),
                        id_item_request_detail = c.Long(nullable: false),
                        id_dimension = c.Int(nullable: false),
                        id_measurement = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 4),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        item_request_detail_id_item_request_detail = c.Int(),
                    })
                .PrimaryKey(t => t.id_request_property)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_dimension", t => t.id_dimension, cascadeDelete: true)
                .ForeignKey("app_measurement", t => t.id_measurement, cascadeDelete: true)
                .ForeignKey("item_request_detail", t => t.item_request_detail_id_item_request_detail)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_dimension)
                .Index(t => t.id_measurement)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.item_request_detail_id_item_request_detail);
            
            CreateTable(
                "item_transfer_dimension",
                c => new
                    {
                        id_transfer_property = c.Long(nullable: false, identity: true),
                        id_transfer_detail = c.Long(nullable: false),
                        id_dimension = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 4),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        item_transfer_detail_id_transfer_detail = c.Int(),
                    })
                .PrimaryKey(t => t.id_transfer_property)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_dimension", t => t.id_dimension, cascadeDelete: true)
                .ForeignKey("item_transfer_detail", t => t.item_transfer_detail_id_transfer_detail)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_dimension)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.item_transfer_detail_id_transfer_detail);
            
            CreateTable(
                "purchase_packing_dimension",
                c => new
                    {
                        id_packing_property = c.Long(nullable: false, identity: true),
                        id_purchase_packing_detail = c.Long(nullable: false),
                        id_dimension = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 4),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        purchase_packing_detail_id_purchase_packing_detail = c.Int(),
                    })
                .PrimaryKey(t => t.id_packing_property)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_dimension", t => t.id_dimension, cascadeDelete: true)
                .ForeignKey("purchase_packing_detail", t => t.purchase_packing_detail_id_purchase_packing_detail)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_dimension)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.purchase_packing_detail_id_purchase_packing_detail);
            
        }
        
        public override void Down()
        {
            DropForeignKey("purchase_packing_dimension", "id_user", "security_user");
            DropForeignKey("purchase_packing_dimension", "purchase_packing_detail_id_purchase_packing_detail", "purchase_packing_detail");
            DropForeignKey("purchase_packing_dimension", "id_dimension", "app_dimension");
            DropForeignKey("purchase_packing_dimension", "id_company", "app_company");
            DropForeignKey("item_transfer_dimension", "id_user", "security_user");
            DropForeignKey("item_transfer_dimension", "item_transfer_detail_id_transfer_detail", "item_transfer_detail");
            DropForeignKey("item_transfer_dimension", "id_dimension", "app_dimension");
            DropForeignKey("item_transfer_dimension", "id_company", "app_company");
            DropForeignKey("item_request_dimension", "id_user", "security_user");
            DropForeignKey("item_request_dimension", "item_request_detail_id_item_request_detail", "item_request_detail");
            DropForeignKey("item_request_dimension", "id_measurement", "app_measurement");
            DropForeignKey("item_request_dimension", "id_dimension", "app_dimension");
            DropForeignKey("item_request_dimension", "id_company", "app_company");
            DropForeignKey("purchase_invoice_dimension", "id_user", "security_user");
            DropForeignKey("purchase_invoice_dimension", "purchase_invoice_detail_id_purchase_invoice_detail", "purchase_invoice_detail");
            DropForeignKey("purchase_invoice_dimension", "id_dimension", "app_dimension");
            DropForeignKey("purchase_invoice_dimension", "id_company", "app_company");
            DropForeignKey("purchase_return_dimension", "id_user", "security_user");
            DropForeignKey("purchase_return_dimension", "purchase_return_detail_id_purchase_return_detail", "purchase_return_detail");
            DropForeignKey("purchase_return_dimension", "id_dimension", "app_dimension");
            DropForeignKey("purchase_return_dimension", "id_company", "app_company");
            DropForeignKey("purchase_tender_dimension", "id_user", "security_user");
            DropForeignKey("purchase_tender_dimension", "id_purchase_tender_item", "purchase_tender_item");
            DropForeignKey("purchase_tender_dimension", "id_measurement", "app_measurement");
            DropForeignKey("purchase_tender_dimension", "id_dimension", "app_dimension");
            DropForeignKey("purchase_tender_dimension", "id_company", "app_company");
            DropForeignKey("purchase_order_dimension", "id_user", "security_user");
            DropForeignKey("purchase_order_dimension", "purchase_order_detail_id_purchase_order_detail", "purchase_order_detail");
            DropForeignKey("purchase_order_dimension", "id_dimension", "app_dimension");
            DropForeignKey("purchase_order_dimension", "id_company", "app_company");
            DropIndex("purchase_packing_dimension", new[] { "purchase_packing_detail_id_purchase_packing_detail" });
            DropIndex("purchase_packing_dimension", new[] { "id_user" });
            DropIndex("purchase_packing_dimension", new[] { "id_company" });
            DropIndex("purchase_packing_dimension", new[] { "id_dimension" });
            DropIndex("item_transfer_dimension", new[] { "item_transfer_detail_id_transfer_detail" });
            DropIndex("item_transfer_dimension", new[] { "id_user" });
            DropIndex("item_transfer_dimension", new[] { "id_company" });
            DropIndex("item_transfer_dimension", new[] { "id_dimension" });
            DropIndex("item_request_dimension", new[] { "item_request_detail_id_item_request_detail" });
            DropIndex("item_request_dimension", new[] { "id_user" });
            DropIndex("item_request_dimension", new[] { "id_company" });
            DropIndex("item_request_dimension", new[] { "id_measurement" });
            DropIndex("item_request_dimension", new[] { "id_dimension" });
            DropIndex("purchase_invoice_dimension", new[] { "purchase_invoice_detail_id_purchase_invoice_detail" });
            DropIndex("purchase_invoice_dimension", new[] { "id_user" });
            DropIndex("purchase_invoice_dimension", new[] { "id_company" });
            DropIndex("purchase_invoice_dimension", new[] { "id_dimension" });
            DropIndex("purchase_return_dimension", new[] { "purchase_return_detail_id_purchase_return_detail" });
            DropIndex("purchase_return_dimension", new[] { "id_user" });
            DropIndex("purchase_return_dimension", new[] { "id_company" });
            DropIndex("purchase_return_dimension", new[] { "id_dimension" });
            DropIndex("purchase_tender_dimension", new[] { "id_user" });
            DropIndex("purchase_tender_dimension", new[] { "id_company" });
            DropIndex("purchase_tender_dimension", new[] { "id_measurement" });
            DropIndex("purchase_tender_dimension", new[] { "id_dimension" });
            DropIndex("purchase_tender_dimension", new[] { "id_purchase_tender_item" });
            DropIndex("purchase_order_dimension", new[] { "purchase_order_detail_id_purchase_order_detail" });
            DropIndex("purchase_order_dimension", new[] { "id_user" });
            DropIndex("purchase_order_dimension", new[] { "id_company" });
            DropIndex("purchase_order_dimension", new[] { "id_dimension" });
            DropTable("purchase_packing_dimension");
            DropTable("item_transfer_dimension");
            DropTable("item_request_dimension");
            DropTable("purchase_invoice_dimension");
            DropTable("purchase_return_dimension");
            DropTable("purchase_tender_dimension");
            DropTable("purchase_order_dimension");
        }
    }
}
