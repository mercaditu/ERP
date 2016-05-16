namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TryingToFix_ItemMovement_ExcessFK : DbMigration
    {
        public override void Up()
        {
         Sql(@" ALTER TABLE `cognitivo`.`item_movement`  "
            + " DROP FOREIGN KEY `FK_item_movement_sales_return_id_sales_return`, "
            + " DROP FOREIGN KEY `FK_item_movement_sales_packing_id_sales_packing`, "
            + " DROP FOREIGN KEY `FK_item_movement_sales_invoice_id_sales_invoice`, "
            + " DROP FOREIGN KEY `FK_item_movement_purchase_return_id_purchase_return`, "
            + " DROP FOREIGN KEY `FK_item_movement_purchase_invoice_id_purchase_invoice`, "
            + " DROP FOREIGN KEY `FK_item_movement_production_execution_id_production_execution`; "
            + " ALTER TABLE `cognitivo`.`item_movement`  "
            + " DROP COLUMN `id_sales_packing`, "
            + " DROP COLUMN `id_inventory`, "
            + " DROP COLUMN `id_sales_return`, "
            + " DROP COLUMN `id_sales_invoice`, "
            + " DROP COLUMN `id_purchase_return`, "
            + " DROP COLUMN `id_purchase_invoice`, "
            + " DROP COLUMN `id_production_execution`, "
            + " DROP INDEX `IX_id_sales_packing` , "
            + " DROP INDEX `IX_id_sales_return` , "
            + " DROP INDEX `IX_id_sales_invoice` , "
            + " DROP INDEX `IX_id_purchase_return` , "
            + " DROP INDEX `IX_id_purchase_invoice` , "
            + " DROP INDEX `IX_id_production_execution` ;");
        }
        
        public override void Down()
        {
            AddColumn("item_movement", "transaction_id", c => c.Int(nullable: false));
            AddColumn("item_movement", "id_inventory", c => c.Int());
            AddColumn("item_movement", "id_sales_invoice", c => c.Int());
            RenameIndex(table: "dbo.item_movement", name: "IX_item_transfer_id_transfer", newName: "IX_id_transfer");
            RenameIndex(table: "dbo.item_movement", name: "IX_purchase_return_id_purchase_return", newName: "IX_id_purchase_return");
            RenameIndex(table: "dbo.item_movement", name: "IX_sales_return_id_sales_return", newName: "IX_id_sales_return");
            RenameIndex(table: "dbo.item_movement", name: "IX_sales_packing_id_sales_packing", newName: "IX_id_sales_packing");
            RenameIndex(table: "dbo.item_movement", name: "IX_purchase_invoice_id_purchase_invoice", newName: "IX_id_purchase_invoice");
            RenameIndex(table: "dbo.item_movement", name: "IX_production_execution_id_production_execution", newName: "IX_id_production_execution");
            RenameColumn(table: "item_movement", name: "item_transfer_id_transfer", newName: "id_transfer");
            RenameColumn(table: "item_movement", name: "purchase_return_id_purchase_return", newName: "id_purchase_return");
            RenameColumn(table: "item_movement", name: "sales_return_id_sales_return", newName: "id_sales_return");
            RenameColumn(table: "item_movement", name: "sales_packing_id_sales_packing", newName: "id_sales_packing");
            RenameColumn(table: "item_movement", name: "purchase_invoice_id_purchase_invoice", newName: "id_purchase_invoice");
            RenameColumn(table: "item_movement", name: "production_execution_id_production_execution", newName: "id_production_execution");
            CreateIndex("item_movement", "id_sales_invoice");
            AddForeignKey("item_movement", "id_sales_invoice", "sales_invoice", "id_sales_invoice");
        }
    }
}
