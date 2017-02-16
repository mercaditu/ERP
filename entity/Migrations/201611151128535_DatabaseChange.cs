namespace entity.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class DatabaseChange : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("purchase_return", "id_purchase_invoice", "purchase_invoice");
            DropIndex("purchase_return", new[] { "id_purchase_invoice" });
            AddColumn("contacts", "trans_code", c => c.String(unicode: false));
            AddColumn("production_execution_detail", "geo_lat", c => c.Decimal(precision: 20, scale: 9));
            AddColumn("production_execution_detail", "geo_long", c => c.Decimal(precision: 20, scale: 9));
            AlterColumn("purchase_return", "id_purchase_invoice", c => c.Int());
            CreateIndex("purchase_return", "id_purchase_invoice");
            AddForeignKey("purchase_return", "id_purchase_invoice", "purchase_invoice", "id_purchase_invoice");
            DropColumn("sales_return_detail", "id_sales_invoice_detail");
            DropColumn("item_transfer_dimension", "id_transfer_detail");
            DropColumn("item_request_dimension", "id_item_request_detail");
        }

        public override void Down()
        {
            AddColumn("item_request_dimension", "id_item_request_detail", c => c.Long(nullable: false));
            AddColumn("item_transfer_dimension", "id_transfer_detail", c => c.Long(nullable: false));
            AddColumn("sales_return_detail", "id_sales_invoice_detail", c => c.Long());
            DropForeignKey("purchase_return", "id_purchase_invoice", "purchase_invoice");
            DropIndex("purchase_return", new[] { "id_purchase_invoice" });
            AlterColumn("purchase_return", "id_purchase_invoice", c => c.Int(nullable: false));
            DropColumn("production_execution_detail", "geo_long");
            DropColumn("production_execution_detail", "geo_lat");
            DropColumn("contacts", "trans_code");
            CreateIndex("purchase_return", "id_purchase_invoice");
            AddForeignKey("purchase_return", "id_purchase_invoice", "purchase_invoice", "id_purchase_invoice", cascadeDelete: true);
        }
    }
}