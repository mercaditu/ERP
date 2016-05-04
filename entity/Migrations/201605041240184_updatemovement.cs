namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatemovement : DbMigration
    {
        public override void Up()
        {
            AddColumn("item_movement", "id_transfer_detail", c => c.Int());
            AddColumn("item_movement", "id_production_execution_detail", c => c.Int());
            AddColumn("item_movement", "id_purchase_invoice_detail", c => c.Int());
            AddColumn("item_movement", "id_purchase_return_detail", c => c.Int());
            AddColumn("item_movement", "id_sales_invoice_detail", c => c.Int());
            AddColumn("item_movement", "id_sales_return_detail", c => c.Int());
            AddColumn("item_movement", "id_inventory_detail", c => c.Int());
            AddColumn("item_movement", "id_sales_packing_detail", c => c.Int());
            AddColumn("item_movement", "production_execution_detail_id_execution_detail", c => c.Int());
            AddColumn("item_movement", "sales_invoice_detail_id_sales_invoice_detail", c => c.Long());
            CreateIndex("item_movement", "id_transfer_detail");
            CreateIndex("item_movement", "id_purchase_invoice_detail");
            CreateIndex("item_movement", "id_purchase_return_detail");
            CreateIndex("item_movement", "id_sales_return_detail");
            CreateIndex("item_movement", "id_sales_packing_detail");
            CreateIndex("item_movement", "production_execution_detail_id_execution_detail");
            CreateIndex("item_movement", "sales_invoice_detail_id_sales_invoice_detail");
            AddForeignKey("item_movement", "production_execution_detail_id_execution_detail", "production_execution_detail", "id_execution_detail");
            AddForeignKey("item_movement", "id_purchase_invoice_detail", "purchase_invoice_detail", "id_purchase_invoice_detail");
            AddForeignKey("item_movement", "sales_invoice_detail_id_sales_invoice_detail", "sales_invoice_detail", "id_sales_invoice_detail");
            AddForeignKey("item_movement", "id_sales_packing_detail", "sales_packing_detail", "id_sales_packing_detail");
            AddForeignKey("item_movement", "id_sales_return_detail", "sales_return_detail", "id_sales_return_detail");
            AddForeignKey("item_movement", "id_purchase_return_detail", "purchase_return_detail", "id_purchase_return_detail");
            AddForeignKey("item_movement", "id_transfer_detail", "item_transfer_detail", "id_transfer_detail");
        }
        
        public override void Down()
        {
            DropForeignKey("item_movement", "id_transfer_detail", "item_transfer_detail");
            DropForeignKey("item_movement", "id_purchase_return_detail", "purchase_return_detail");
            DropForeignKey("item_movement", "id_sales_return_detail", "sales_return_detail");
            DropForeignKey("item_movement", "id_sales_packing_detail", "sales_packing_detail");
            DropForeignKey("item_movement", "sales_invoice_detail_id_sales_invoice_detail", "sales_invoice_detail");
            DropForeignKey("item_movement", "id_purchase_invoice_detail", "purchase_invoice_detail");
            DropForeignKey("item_movement", "production_execution_detail_id_execution_detail", "production_execution_detail");
            DropIndex("item_movement", new[] { "sales_invoice_detail_id_sales_invoice_detail" });
            DropIndex("item_movement", new[] { "production_execution_detail_id_execution_detail" });
            DropIndex("item_movement", new[] { "id_sales_packing_detail" });
            DropIndex("item_movement", new[] { "id_sales_return_detail" });
            DropIndex("item_movement", new[] { "id_purchase_return_detail" });
            DropIndex("item_movement", new[] { "id_purchase_invoice_detail" });
            DropIndex("item_movement", new[] { "id_transfer_detail" });
            DropColumn("item_movement", "sales_invoice_detail_id_sales_invoice_detail");
            DropColumn("item_movement", "production_execution_detail_id_execution_detail");
            DropColumn("item_movement", "id_sales_packing_detail");
            DropColumn("item_movement", "id_inventory_detail");
            DropColumn("item_movement", "id_sales_return_detail");
            DropColumn("item_movement", "id_sales_invoice_detail");
            DropColumn("item_movement", "id_purchase_return_detail");
            DropColumn("item_movement", "id_purchase_invoice_detail");
            DropColumn("item_movement", "id_production_execution_detail");
            DropColumn("item_movement", "id_transfer_detail");
        }
    }
}
