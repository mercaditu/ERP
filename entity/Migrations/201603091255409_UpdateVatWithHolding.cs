namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateVatWithHolding : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("payment_withholding_tax", "id_range", "app_document_range");
            DropIndex("payment_withholding_tax", new[] { "id_range" });
            CreateTable(
                "payment_withholding_details",
                c => new
                    {
                        id_withholding_detail = c.Int(nullable: false, identity: true),
                        id_withholding = c.Int(nullable: false),
                        id_sales_invoice = c.Int(),
                        id_purchase_invoice = c.Int(),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.id_withholding_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("payment_withholding_tax", t => t.id_withholding, cascadeDelete: true)
                .ForeignKey("purchase_invoice", t => t.id_purchase_invoice)
                .ForeignKey("sales_invoice", t => t.id_sales_invoice)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_withholding)
                .Index(t => t.id_sales_invoice)
                .Index(t => t.id_purchase_invoice)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            AddColumn("contacts", "id_bank", c => c.Int());
            AddColumn("contact_subscription", "unit_price", c => c.Decimal(nullable: false, precision: 20, scale: 4));
            AlterColumn("contact_subscription", "bill_cycle", c => c.Int(nullable: false));
            AlterColumn("payment_withholding_tax", "id_range", c => c.Int());
            CreateIndex("contacts", "id_bank");
            CreateIndex("payment_withholding_tax", "id_range");
            AddForeignKey("contacts", "id_bank", "app_bank", "id_bank");
            AddForeignKey("payment_withholding_tax", "id_range", "app_document_range", "id_range");
        }
        
        public override void Down()
        {
            DropForeignKey("payment_withholding_tax", "id_range", "app_document_range");
            DropForeignKey("payment_withholding_details", "id_user", "security_user");
            DropForeignKey("payment_withholding_details", "id_sales_invoice", "sales_invoice");
            DropForeignKey("payment_withholding_details", "id_purchase_invoice", "purchase_invoice");
            DropForeignKey("payment_withholding_details", "id_withholding", "payment_withholding_tax");
            DropForeignKey("payment_withholding_details", "id_company", "app_company");
            DropForeignKey("contacts", "id_bank", "app_bank");
            DropIndex("payment_withholding_details", new[] { "id_user" });
            DropIndex("payment_withholding_details", new[] { "id_company" });
            DropIndex("payment_withholding_details", new[] { "id_purchase_invoice" });
            DropIndex("payment_withholding_details", new[] { "id_sales_invoice" });
            DropIndex("payment_withholding_details", new[] { "id_withholding" });
            DropIndex("payment_withholding_tax", new[] { "id_range" });
            DropIndex("contacts", new[] { "id_bank" });
            AlterColumn("payment_withholding_tax", "id_range", c => c.Int(nullable: false));
            AlterColumn("contact_subscription", "bill_cycle", c => c.Short(nullable: false));
            DropColumn("contact_subscription", "unit_price");
            DropColumn("contacts", "id_bank");
            DropTable("payment_withholding_details");
            CreateIndex("payment_withholding_tax", "id_range");
            AddForeignKey("payment_withholding_tax", "id_range", "app_document_range", "id_range", cascadeDelete: true);
        }
    }
}
