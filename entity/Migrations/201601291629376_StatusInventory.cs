namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StatusInventory : DbMigration
    {
        public override void Up()
        {
            AddColumn("item_inventory", "status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("item_inventory", "status");
        }
    }
}
