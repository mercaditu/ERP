namespace entity.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class ChangesinProject : DbMigration
    {
        public override void Up()
        {
            AddColumn("project_task", "completed", c => c.Decimal(nullable: false, precision: 20, scale: 9));
            AddColumn("purchase_invoice", "is_archived", c => c.Boolean(nullable: false));
            AddColumn("impexes", "is_archived", c => c.Boolean(nullable: false));
            AddColumn("sales_invoice", "is_archived", c => c.Boolean(nullable: false));
            AddColumn("sales_budget", "is_archived", c => c.Boolean(nullable: false));
            AddColumn("projects", "is_archived", c => c.Boolean(nullable: false));
            AddColumn("production_order", "is_archived", c => c.Boolean(nullable: false));
            AddColumn("production_order", "completed", c => c.Decimal(nullable: false, precision: 20, scale: 9));
            AddColumn("item_request", "is_archived", c => c.Boolean(nullable: false));
            AddColumn("item_transfer", "is_archived", c => c.Boolean(nullable: false));
            AddColumn("item_transfer_detail", "verified_by", c => c.Decimal(precision: 20, scale: 9));
            AddColumn("purchase_packing_detail", "verified_quantity", c => c.Decimal(precision: 20, scale: 9));
            AddColumn("purchase_packing_detail", "verified_by", c => c.Decimal(precision: 20, scale: 9));
            AddColumn("production_order_detail", "completed", c => c.Decimal(nullable: false, precision: 20, scale: 9));
            AddColumn("purchase_order", "is_archived", c => c.Boolean(nullable: false));
            AddColumn("sales_packing_detail", "verified_quantity", c => c.Decimal(precision: 20, scale: 9));
            AddColumn("sales_packing_detail", "verified_by", c => c.Decimal(precision: 20, scale: 9));
            AddColumn("sales_return", "is_archived", c => c.Boolean(nullable: false));
            AddColumn("sales_order", "is_archived", c => c.Boolean(nullable: false));
            AddColumn("purchase_return", "is_archived", c => c.Boolean(nullable: false));
            AddColumn("item_inventory", "is_archived", c => c.Boolean(nullable: false));
            AddColumn("payments", "is_archived", c => c.Boolean(nullable: false));
            DropColumn("purchase_packing_detail", "user_quantity");
            DropColumn("sales_packing_detail", "user_quantity");
        }

        public override void Down()
        {
            AddColumn("sales_packing_detail", "user_quantity", c => c.Decimal(precision: 20, scale: 9));
            AddColumn("purchase_packing_detail", "user_quantity", c => c.Decimal(precision: 20, scale: 9));
            DropColumn("payments", "is_archived");
            DropColumn("item_inventory", "is_archived");
            DropColumn("purchase_return", "is_archived");
            DropColumn("sales_order", "is_archived");
            DropColumn("sales_return", "is_archived");
            DropColumn("sales_packing_detail", "verified_by");
            DropColumn("sales_packing_detail", "verified_quantity");
            DropColumn("purchase_order", "is_archived");
            DropColumn("production_order_detail", "completed");
            DropColumn("purchase_packing_detail", "verified_by");
            DropColumn("purchase_packing_detail", "verified_quantity");
            DropColumn("item_transfer_detail", "verified_by");
            DropColumn("item_transfer", "is_archived");
            DropColumn("item_request", "is_archived");
            DropColumn("production_order", "completed");
            DropColumn("production_order", "is_archived");
            DropColumn("projects", "is_archived");
            DropColumn("sales_budget", "is_archived");
            DropColumn("sales_invoice", "is_archived");
            DropColumn("impexes", "is_archived");
            DropColumn("purchase_invoice", "is_archived");
            DropColumn("project_task", "completed");
        }
    }
}