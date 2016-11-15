namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Databasechange : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("purchase_return", "id_purchase_invoice", "purchase_invoice");
            DropIndex("purchase_return", new[] { "id_purchase_invoice" });
            AddColumn("production_execution_detail", "geo_lat", c => c.Decimal(precision: 20, scale: 9));
            AddColumn("production_execution_detail", "geo_long", c => c.Decimal(precision: 20, scale: 9));
            AlterColumn("purchase_return", "id_purchase_invoice", c => c.Int());
            CreateIndex("purchase_return", "id_purchase_invoice");
            AddForeignKey("purchase_return", "id_purchase_invoice", "purchase_invoice", "id_purchase_invoice");
        }
        
        public override void Down()
        {
            DropForeignKey("purchase_return", "id_purchase_invoice", "purchase_invoice");
            DropIndex("purchase_return", new[] { "id_purchase_invoice" });
            AlterColumn("purchase_return", "id_purchase_invoice", c => c.Int(nullable: false));
            DropColumn("production_execution_detail", "geo_long");
            DropColumn("production_execution_detail", "geo_lat");
            CreateIndex("purchase_return", "id_purchase_invoice");
            AddForeignKey("purchase_return", "id_purchase_invoice", "purchase_invoice", "id_purchase_invoice", cascadeDelete: true);
        }
    }
}
