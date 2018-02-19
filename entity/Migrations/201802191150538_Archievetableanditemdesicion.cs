namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Archievetableanditemdesicion : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "app_account_detail_archive",
                c => new
                    {
                        id_account_detail = c.Int(nullable: false, identity: true),
                        id_account = c.Int(nullable: false),
                        id_currencyfx = c.Int(nullable: false),
                        id_payment_detail = c.Int(),
                        id_payment_approve_detail = c.Int(),
                        id_payment_type = c.Int(nullable: false),
                        status = c.Int(nullable: false),
                        debit = c.Decimal(nullable: false, precision: 20, scale: 9),
                        credit = c.Decimal(nullable: false, precision: 20, scale: 9),
                        comment = c.String(unicode: false),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        id_session = c.Int(),
                        tran_type = c.Int(),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_account_detail)                
                .ForeignKey("app_account", t => t.id_account, cascadeDelete: true)
                .ForeignKey("app_account_session", t => t.id_session)
                .ForeignKey("payment_detail", t => t.id_payment_detail)
                .ForeignKey("app_currencyfx", t => t.id_currencyfx, cascadeDelete: true)
                .ForeignKey("payment_approve_detail", t => t.id_payment_approve_detail)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("payment_type", t => t.id_payment_type, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_account)
                .Index(t => t.id_currencyfx)
                .Index(t => t.id_payment_detail)
                .Index(t => t.id_payment_approve_detail)
                .Index(t => t.id_payment_type)
                .Index(t => t.id_session)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_movement_archive",
                c => new
                    {
                        id_movement_archieve = c.Long(nullable: false, identity: true),
                        id_item_product = c.Int(nullable: false),
                        id_transfer_detail = c.Int(),
                        id_execution_detail = c.Int(),
                        id_purchase_invoice_detail = c.Int(),
                        id_purchase_return_detail = c.Int(),
                        id_sales_invoice_detail = c.Int(),
                        id_sales_return_detail = c.Int(),
                        id_inventory_detail = c.Int(),
                        id_sales_packing_detail = c.Int(),
                        id_purchase_packing_detail = c.Int(),
                        id_location = c.Int(nullable: false),
                        id_movement_value_rel = c.Long(),
                        status = c.Int(nullable: false),
                        debit = c.Decimal(nullable: false, precision: 20, scale: 9),
                        credit = c.Decimal(nullable: false, precision: 20, scale: 9),
                        comment = c.String(unicode: false),
                        code = c.String(unicode: false),
                        expire_date = c.DateTime(precision: 0),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        barcode = c.String(unicode: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        parent_id_movement_archieve = c.Long(),
                    })
                .PrimaryKey(t => t.id_movement_archieve)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_location", t => t.id_location, cascadeDelete: true)
                .ForeignKey("item_movement_archive", t => t.parent_id_movement_archieve)
                .ForeignKey("production_execution_detail", t => t.id_execution_detail)
                .ForeignKey("purchase_invoice_detail", t => t.id_purchase_invoice_detail)
                .ForeignKey("purchase_packing_detail", t => t.id_purchase_packing_detail)
                .ForeignKey("item_transfer_detail", t => t.id_transfer_detail)
                .ForeignKey("sales_packing_detail", t => t.id_sales_packing_detail)
                .ForeignKey("sales_invoice_detail", t => t.id_sales_invoice_detail)
                .ForeignKey("sales_return_detail", t => t.id_sales_return_detail)
                .ForeignKey("purchase_return_detail", t => t.id_purchase_return_detail)
                .ForeignKey("item_movement_value_rel", t => t.id_movement_value_rel)
                .ForeignKey("item_product", t => t.id_item_product, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_item_product)
                .Index(t => t.id_transfer_detail)
                .Index(t => t.id_execution_detail)
                .Index(t => t.id_purchase_invoice_detail)
                .Index(t => t.id_purchase_return_detail)
                .Index(t => t.id_sales_invoice_detail)
                .Index(t => t.id_sales_return_detail)
                .Index(t => t.id_sales_packing_detail)
                .Index(t => t.id_purchase_packing_detail)
                .Index(t => t.id_location)
                .Index(t => t.id_movement_value_rel)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.parent_id_movement_archieve);
            
            AddColumn("item_movement_dimension", "item_movement_archive_id_movement_archieve", c => c.Long());
            AddColumn("item_request_decision", "item_movement_id_movement", c => c.Long());
            AddColumn("item_request_decision", "item_transfer_id_transfer", c => c.Int());
            AddColumn("item_request_decision", "production_order_id_production_order", c => c.Int());
            AddColumn("item_request_decision", "purchase_tender_id_purchase_tender", c => c.Int());
            AddColumn("item_movement_value", "item_movement_archive_id_movement_archieve", c => c.Long());
            CreateIndex("item_movement_dimension", "item_movement_archive_id_movement_archieve");
            CreateIndex("item_request_decision", "item_movement_id_movement");
            CreateIndex("item_request_decision", "item_transfer_id_transfer");
            CreateIndex("item_request_decision", "production_order_id_production_order");
            CreateIndex("item_request_decision", "purchase_tender_id_purchase_tender");
            CreateIndex("item_movement_value", "item_movement_archive_id_movement_archieve");
            AddForeignKey("item_request_decision", "item_movement_id_movement", "item_movement", "id_movement");
            AddForeignKey("item_request_decision", "item_transfer_id_transfer", "item_transfer", "id_transfer");
            AddForeignKey("item_request_decision", "production_order_id_production_order", "production_order", "id_production_order");
            AddForeignKey("item_request_decision", "purchase_tender_id_purchase_tender", "purchase_tender", "id_purchase_tender");
            AddForeignKey("item_movement_dimension", "item_movement_archive_id_movement_archieve", "item_movement_archive", "id_movement_archieve");
            AddForeignKey("item_movement_value", "item_movement_archive_id_movement_archieve", "item_movement_archive", "id_movement_archieve");
        }
        
        public override void Down()
        {
            DropForeignKey("app_account_detail_archive", "id_user", "security_user");
            DropForeignKey("app_account_detail_archive", "id_payment_type", "payment_type");
            DropForeignKey("app_account_detail_archive", "id_company", "app_company");
            DropForeignKey("item_movement_archive", "id_user", "security_user");
            DropForeignKey("item_movement_archive", "id_item_product", "item_product");
            DropForeignKey("item_movement_archive", "id_movement_value_rel", "item_movement_value_rel");
            DropForeignKey("item_movement_value", "item_movement_archive_id_movement_archieve", "item_movement_archive");
            DropForeignKey("item_movement_dimension", "item_movement_archive_id_movement_archieve", "item_movement_archive");
            DropForeignKey("item_movement_archive", "id_purchase_return_detail", "purchase_return_detail");
            DropForeignKey("item_movement_archive", "id_sales_return_detail", "sales_return_detail");
            DropForeignKey("item_movement_archive", "id_sales_invoice_detail", "sales_invoice_detail");
            DropForeignKey("item_movement_archive", "id_sales_packing_detail", "sales_packing_detail");
            DropForeignKey("item_movement_archive", "id_transfer_detail", "item_transfer_detail");
            DropForeignKey("item_movement_archive", "id_purchase_packing_detail", "purchase_packing_detail");
            DropForeignKey("item_movement_archive", "id_purchase_invoice_detail", "purchase_invoice_detail");
            DropForeignKey("item_movement_archive", "id_execution_detail", "production_execution_detail");
            DropForeignKey("item_request_decision", "purchase_tender_id_purchase_tender", "purchase_tender");
            DropForeignKey("item_request_decision", "production_order_id_production_order", "production_order");
            DropForeignKey("item_request_decision", "item_transfer_id_transfer", "item_transfer");
            DropForeignKey("item_request_decision", "item_movement_id_movement", "item_movement");
            DropForeignKey("app_account_detail_archive", "id_payment_approve_detail", "payment_approve_detail");
            DropForeignKey("item_movement_archive", "parent_id_movement_archieve", "item_movement_archive");
            DropForeignKey("item_movement_archive", "id_location", "app_location");
            DropForeignKey("item_movement_archive", "id_company", "app_company");
            DropForeignKey("app_account_detail_archive", "id_currencyfx", "app_currencyfx");
            DropForeignKey("app_account_detail_archive", "id_payment_detail", "payment_detail");
            DropForeignKey("app_account_detail_archive", "id_session", "app_account_session");
            DropForeignKey("app_account_detail_archive", "id_account", "app_account");
            DropIndex("item_movement_value", new[] { "item_movement_archive_id_movement_archieve" });
            DropIndex("item_request_decision", new[] { "purchase_tender_id_purchase_tender" });
            DropIndex("item_request_decision", new[] { "production_order_id_production_order" });
            DropIndex("item_request_decision", new[] { "item_transfer_id_transfer" });
            DropIndex("item_request_decision", new[] { "item_movement_id_movement" });
            DropIndex("item_movement_dimension", new[] { "item_movement_archive_id_movement_archieve" });
            DropIndex("item_movement_archive", new[] { "parent_id_movement_archieve" });
            DropIndex("item_movement_archive", new[] { "id_user" });
            DropIndex("item_movement_archive", new[] { "id_company" });
            DropIndex("item_movement_archive", new[] { "id_movement_value_rel" });
            DropIndex("item_movement_archive", new[] { "id_location" });
            DropIndex("item_movement_archive", new[] { "id_purchase_packing_detail" });
            DropIndex("item_movement_archive", new[] { "id_sales_packing_detail" });
            DropIndex("item_movement_archive", new[] { "id_sales_return_detail" });
            DropIndex("item_movement_archive", new[] { "id_sales_invoice_detail" });
            DropIndex("item_movement_archive", new[] { "id_purchase_return_detail" });
            DropIndex("item_movement_archive", new[] { "id_purchase_invoice_detail" });
            DropIndex("item_movement_archive", new[] { "id_execution_detail" });
            DropIndex("item_movement_archive", new[] { "id_transfer_detail" });
            DropIndex("item_movement_archive", new[] { "id_item_product" });
            DropIndex("app_account_detail_archive", new[] { "id_user" });
            DropIndex("app_account_detail_archive", new[] { "id_company" });
            DropIndex("app_account_detail_archive", new[] { "id_session" });
            DropIndex("app_account_detail_archive", new[] { "id_payment_type" });
            DropIndex("app_account_detail_archive", new[] { "id_payment_approve_detail" });
            DropIndex("app_account_detail_archive", new[] { "id_payment_detail" });
            DropIndex("app_account_detail_archive", new[] { "id_currencyfx" });
            DropIndex("app_account_detail_archive", new[] { "id_account" });
            DropColumn("item_movement_value", "item_movement_archive_id_movement_archieve");
            DropColumn("item_request_decision", "purchase_tender_id_purchase_tender");
            DropColumn("item_request_decision", "production_order_id_production_order");
            DropColumn("item_request_decision", "item_transfer_id_transfer");
            DropColumn("item_request_decision", "item_movement_id_movement");
            DropColumn("item_movement_dimension", "item_movement_archive_id_movement_archieve");
            DropTable("item_movement_archive");
            DropTable("app_account_detail_archive");
        }
    }
}
