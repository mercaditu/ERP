namespace entity.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class ItemSupplier_Name_and_Code : DbMigration
    {
        public override void Up()
        {
            AddColumn("app_account", "credit_line", c => c.Decimal(nullable: false, precision: 20, scale: 9));
            AddColumn("items", "supplier_name", c => c.String(unicode: false));
            AddColumn("items", "supplier_code", c => c.String(unicode: false));
            AddColumn("items", "sku", c => c.String(unicode: false));
            AddColumn("purchase_packing_detail", "id_branch", c => c.Int());
            AddColumn("purchase_packing_detail", "user_verified", c => c.Boolean(nullable: false));
            AddColumn("purchase_packing_detail", "user_quantity", c => c.Decimal(precision: 20, scale: 9));
            AddColumn("production_execution_detail", "is_accounted", c => c.Boolean(nullable: false));
            AddColumn("sales_packing_detail", "id_branch", c => c.Int());
            AddColumn("sales_packing_detail", "user_verified", c => c.Boolean(nullable: false));
            AddColumn("sales_packing_detail", "user_quantity", c => c.Decimal(precision: 20, scale: 9));
            CreateIndex("sales_packing_detail", "id_branch");
            AddForeignKey("sales_packing_detail", "id_branch", "app_branch", "id_branch");
        }

        public override void Down()
        {
            DropForeignKey("sales_packing_detail", "id_branch", "app_branch");
            DropIndex("sales_packing_detail", new[] { "id_branch" });
            DropColumn("sales_packing_detail", "user_quantity");
            DropColumn("sales_packing_detail", "user_verified");
            DropColumn("sales_packing_detail", "id_branch");
            DropColumn("production_execution_detail", "is_accounted");
            DropColumn("purchase_packing_detail", "user_quantity");
            DropColumn("purchase_packing_detail", "user_verified");
            DropColumn("purchase_packing_detail", "id_branch");
            DropColumn("items", "sku");
            DropColumn("items", "supplier_code");
            DropColumn("items", "supplier_name");
            DropColumn("app_account", "credit_line");
        }
    }
}