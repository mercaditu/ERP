namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updateitemmovement : DbMigration
    {
        public override void Up()
        {
            AddColumn("item_movement", "id_transfer", c => c.Int());
            AddColumn("item_movement", "id_production_execution", c => c.Int());
            AddColumn("item_movement", "id_purchase_invoice", c => c.Int());
            AddColumn("item_movement", "id_purchase_return", c => c.Int());
            AddColumn("item_movement", "id_sales_invoice", c => c.Int());
            AddColumn("item_movement", "id_sales_return", c => c.Int());
            AddColumn("item_movement", "id_inventory", c => c.Int());
            CreateIndex("item_movement", "id_transfer");
            CreateIndex("item_movement", "id_production_execution");
            CreateIndex("item_movement", "id_purchase_invoice");
            CreateIndex("item_movement", "id_purchase_return");
            CreateIndex("item_movement", "id_sales_invoice");
            CreateIndex("item_movement", "id_sales_return");
            AddForeignKey("item_movement", "id_transfer", "item_transfer", "id_transfer");
            AddForeignKey("item_movement", "id_production_execution", "production_execution", "id_production_execution");
            AddForeignKey("item_movement", "id_purchase_invoice", "purchase_invoice", "id_purchase_invoice");
            AddForeignKey("item_movement", "id_purchase_return", "purchase_return", "id_purchase_return");
            AddForeignKey("item_movement", "id_sales_return", "sales_return", "id_sales_return");
            AddForeignKey("item_movement", "id_sales_invoice", "sales_invoice", "id_sales_invoice");
        }
        
        public override void Down()
        {
            DropForeignKey("item_movement", "id_sales_invoice", "sales_invoice");
            DropForeignKey("item_movement", "id_sales_return", "sales_return");
            DropForeignKey("item_movement", "id_purchase_return", "purchase_return");
            DropForeignKey("item_movement", "id_purchase_invoice", "purchase_invoice");
            DropForeignKey("item_movement", "id_production_execution", "production_execution");
            DropForeignKey("item_movement", "id_transfer", "item_transfer");
            DropIndex("item_movement", new[] { "id_sales_return" });
            DropIndex("item_movement", new[] { "id_sales_invoice" });
            DropIndex("item_movement", new[] { "id_purchase_return" });
            DropIndex("item_movement", new[] { "id_purchase_invoice" });
            DropIndex("item_movement", new[] { "id_production_execution" });
            DropIndex("item_movement", new[] { "id_transfer" });
            DropColumn("item_movement", "id_inventory");
            DropColumn("item_movement", "id_sales_return");
            DropColumn("item_movement", "id_sales_invoice");
            DropColumn("item_movement", "id_purchase_return");
            DropColumn("item_movement", "id_purchase_invoice");
            DropColumn("item_movement", "id_production_execution");
            DropColumn("item_movement", "id_transfer");
        }
    }
}
