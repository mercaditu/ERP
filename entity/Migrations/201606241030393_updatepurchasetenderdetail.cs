namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatepurchasetenderdetail : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "purchase_tender_detail_dimension",
                c => new
                    {
                        id_tender_detail_property = c.Int(nullable: false, identity: true),
                        id_purchase_tender_detail = c.Int(nullable: false),
                        id_dimension = c.Int(nullable: false),
                        id_measurement = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 4),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_tender_detail_property)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_dimension", t => t.id_dimension, cascadeDelete: true)
                .ForeignKey("app_measurement", t => t.id_measurement, cascadeDelete: true)
                .ForeignKey("purchase_tender_detail", t => t.id_purchase_tender_detail, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_purchase_tender_detail)
                .Index(t => t.id_dimension)
                .Index(t => t.id_measurement)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            AddColumn("item_asset_maintainance_detail", "start_date", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("item_asset_maintainance_detail", "end_date", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("purchase_invoice_dimension", "id_measurement", c => c.Int(nullable: false));
            AddColumn("purchase_order_dimension", "id_measurement", c => c.Int(nullable: false));
            AddColumn("purchase_return_dimension", "id_measurement", c => c.Int(nullable: false));
            AddColumn("purchase_packing_dimension", "id_measurement", c => c.Int(nullable: false));
            CreateIndex("purchase_invoice_dimension", "id_measurement");
            CreateIndex("purchase_order_dimension", "id_measurement");
            CreateIndex("purchase_return_dimension", "id_measurement");
            CreateIndex("item_request_decision", "id_location");
            CreateIndex("purchase_packing_dimension", "id_measurement");
            AddForeignKey("purchase_invoice_dimension", "id_measurement", "app_measurement", "id_measurement", cascadeDelete: true);
            AddForeignKey("purchase_order_dimension", "id_measurement", "app_measurement", "id_measurement", cascadeDelete: true);
            AddForeignKey("purchase_return_dimension", "id_measurement", "app_measurement", "id_measurement", cascadeDelete: true);
            AddForeignKey("item_request_decision", "id_location", "app_location", "id_location");
            AddForeignKey("purchase_packing_dimension", "id_measurement", "app_measurement", "id_measurement", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("purchase_packing_dimension", "id_measurement", "app_measurement");
            DropForeignKey("item_request_decision", "id_location", "app_location");
            DropForeignKey("purchase_return_dimension", "id_measurement", "app_measurement");
            DropForeignKey("purchase_order_dimension", "id_measurement", "app_measurement");
            DropForeignKey("purchase_tender_detail_dimension", "id_user", "security_user");
            DropForeignKey("purchase_tender_detail_dimension", "id_purchase_tender_detail", "purchase_tender_detail");
            DropForeignKey("purchase_tender_detail_dimension", "id_measurement", "app_measurement");
            DropForeignKey("purchase_tender_detail_dimension", "id_dimension", "app_dimension");
            DropForeignKey("purchase_tender_detail_dimension", "id_company", "app_company");
            DropForeignKey("purchase_invoice_dimension", "id_measurement", "app_measurement");
            DropIndex("purchase_packing_dimension", new[] { "id_measurement" });
            DropIndex("item_request_decision", new[] { "id_location" });
            DropIndex("purchase_return_dimension", new[] { "id_measurement" });
            DropIndex("purchase_order_dimension", new[] { "id_measurement" });
            DropIndex("purchase_tender_detail_dimension", new[] { "id_user" });
            DropIndex("purchase_tender_detail_dimension", new[] { "id_company" });
            DropIndex("purchase_tender_detail_dimension", new[] { "id_measurement" });
            DropIndex("purchase_tender_detail_dimension", new[] { "id_dimension" });
            DropIndex("purchase_tender_detail_dimension", new[] { "id_purchase_tender_detail" });
            DropIndex("purchase_invoice_dimension", new[] { "id_measurement" });
            DropColumn("purchase_packing_dimension", "id_measurement");
            DropColumn("purchase_return_dimension", "id_measurement");
            DropColumn("purchase_order_dimension", "id_measurement");
            DropColumn("purchase_invoice_dimension", "id_measurement");
            DropColumn("item_asset_maintainance_detail", "end_date");
            DropColumn("item_asset_maintainance_detail", "start_date");
            DropTable("purchase_tender_detail_dimension");
        }
    }
}
