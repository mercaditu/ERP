namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatemovement1 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("item_movement", "id_inventory_detail");
            AddForeignKey("item_movement", "id_inventory_detail", "item_inventory_detail", "id_inventory_detail");
        }
        
        public override void Down()
        {
            DropForeignKey("item_movement", "id_inventory_detail", "item_inventory_detail");
            DropIndex("item_movement", new[] { "id_inventory_detail" });
        }
    }
}
