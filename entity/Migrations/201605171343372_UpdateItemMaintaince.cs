namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateItemMaintaince : DbMigration
    {
        public override void Up()
        {
            AddColumn("item_asset_maintainance", "maintainance_type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("item_asset_maintainance", "maintainance_type");
        }
    }
}
