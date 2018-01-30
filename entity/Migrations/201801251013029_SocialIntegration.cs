namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SocialIntegration : DbMigration
    {
        public override void Up()
        {
            AddColumn("payment_detail", "cloud_id", c => c.Long());
            AddColumn("payment_detail", "is_finalize", c => c.Boolean(nullable: false));
            AddColumn("contacts", "cloud_id", c => c.Long());
            AddColumn("contacts", "is_finalize", c => c.Boolean(nullable: false));
            AddColumn("items", "cloud_id", c => c.Long());
            AddColumn("items", "is_finalize", c => c.Boolean(nullable: false));
            AddColumn("purchase_invoice", "cloud_id", c => c.Long());
            AddColumn("purchase_invoice", "is_finalize", c => c.Boolean(nullable: false));
            AddColumn("sales_invoice", "cloud_id", c => c.Long());
            AddColumn("sales_invoice", "is_finalize", c => c.Boolean(nullable: false));
            AddColumn("sales_budget", "cloud_id", c => c.Long());
            AddColumn("sales_budget", "is_finalize", c => c.Boolean(nullable: false));
            AddColumn("purchase_order_detail", "cloud_id", c => c.Long());
            AddColumn("purchase_order_detail", "is_finalize", c => c.Boolean(nullable: false));
            AddColumn("purchase_invoice_detail", "cloud_id", c => c.Long());
            AddColumn("purchase_invoice_detail", "is_finalize", c => c.Boolean(nullable: false));
            AddColumn("purchase_order", "cloud_id", c => c.Long());
            AddColumn("purchase_order", "is_finalize", c => c.Boolean(nullable: false));
            AddColumn("sales_order_detail", "cloud_id", c => c.Long());
            AddColumn("sales_order_detail", "is_finalize", c => c.Boolean(nullable: false));
            AddColumn("sales_budget_detail", "cloud_id", c => c.Long());
            AddColumn("sales_budget_detail", "is_finalize", c => c.Boolean(nullable: false));
            AddColumn("sales_invoice_detail", "cloud_id", c => c.Long());
            AddColumn("sales_invoice_detail", "is_finalize", c => c.Boolean(nullable: false));
            AddColumn("sales_return_detail", "cloud_id", c => c.Long());
            AddColumn("sales_return_detail", "is_finalize", c => c.Boolean(nullable: false));
            AddColumn("sales_return", "cloud_id", c => c.Long());
            AddColumn("sales_return", "is_finalize", c => c.Boolean(nullable: false));
            AddColumn("sales_order", "cloud_id", c => c.Long());
            AddColumn("sales_order", "is_finalize", c => c.Boolean(nullable: false));
            AddColumn("purchase_return", "cloud_id", c => c.Long());
            AddColumn("purchase_return", "is_finalize", c => c.Boolean(nullable: false));
            AddColumn("purchase_return_detail", "cloud_id", c => c.Long());
            AddColumn("purchase_return_detail", "is_finalize", c => c.Boolean(nullable: false));
            AddColumn("payments", "cloud_id", c => c.Long());
            AddColumn("payments", "is_finalize", c => c.Boolean(nullable: false));
            
        }
        
        public override void Down()
        {
           
            DropColumn("payments", "is_finalize");
            DropColumn("payments", "cloud_id");
            DropColumn("purchase_return_detail", "is_finalize");
            DropColumn("purchase_return_detail", "cloud_id");
            DropColumn("purchase_return", "is_finalize");
            DropColumn("purchase_return", "cloud_id");
            DropColumn("sales_order", "is_finalize");
            DropColumn("sales_order", "cloud_id");
            DropColumn("sales_return", "is_finalize");
            DropColumn("sales_return", "cloud_id");
            DropColumn("sales_return_detail", "is_finalize");
            DropColumn("sales_return_detail", "cloud_id");
            DropColumn("sales_invoice_detail", "is_finalize");
            DropColumn("sales_invoice_detail", "cloud_id");
            DropColumn("sales_budget_detail", "is_finalize");
            DropColumn("sales_budget_detail", "cloud_id");
            DropColumn("sales_order_detail", "is_finalize");
            DropColumn("sales_order_detail", "cloud_id");
            DropColumn("purchase_order", "is_finalize");
            DropColumn("purchase_order", "cloud_id");
            DropColumn("purchase_invoice_detail", "is_finalize");
            DropColumn("purchase_invoice_detail", "cloud_id");
            DropColumn("purchase_order_detail", "is_finalize");
            DropColumn("purchase_order_detail", "cloud_id");
            DropColumn("sales_budget", "is_finalize");
            DropColumn("sales_budget", "cloud_id");
            DropColumn("sales_invoice", "is_finalize");
            DropColumn("sales_invoice", "cloud_id");
            DropColumn("purchase_invoice", "is_finalize");
            DropColumn("purchase_invoice", "cloud_id");
            DropColumn("items", "is_finalize");
            DropColumn("items", "cloud_id");
            DropColumn("contacts", "is_finalize");
            DropColumn("contacts", "cloud_id");
            DropColumn("payment_detail", "is_finalize");
            DropColumn("payment_detail", "cloud_id");
        }
    }
}
