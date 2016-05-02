namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updateitemvaluedimension : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("item_inventory_dimension", "id_inventory", "item_inventory");
            DropIndex("item_inventory_dimension", new[] { "id_inventory" });
            AddColumn("item_inventory_detail", "id_currencyfx", c => c.Int(nullable: false));
            AddColumn("item_inventory_detail", "unit_value", c => c.Decimal(nullable: false, precision: 20, scale: 4));
            AddColumn("item_inventory_dimension", "id_inventory_detail", c => c.Int(nullable: false));
            CreateIndex("item_inventory_dimension", "id_inventory_detail");
            AddForeignKey("item_inventory_dimension", "id_inventory_detail", "item_inventory_detail", "id_inventory_detail", cascadeDelete: true);
            DropColumn("item_inventory_dimension", "id_inventory");
        }
        
        public override void Down()
        {
            AddColumn("item_inventory_dimension", "id_inventory", c => c.Int(nullable: false));
            DropForeignKey("item_inventory_dimension", "id_inventory_detail", "item_inventory_detail");
            DropIndex("item_inventory_dimension", new[] { "id_inventory_detail" });
            DropColumn("item_inventory_dimension", "id_inventory_detail");
            DropColumn("item_inventory_detail", "unit_value");
            DropColumn("item_inventory_detail", "id_currencyfx");
            CreateIndex("item_inventory_dimension", "id_inventory");
            AddForeignKey("item_inventory_dimension", "id_inventory", "item_inventory", "id_inventory", cascadeDelete: true);
        }
    }
}
