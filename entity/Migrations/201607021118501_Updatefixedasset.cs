namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updatefixedasset : DbMigration
    {
        public override void Up()
        {
            AddColumn("item_asset", "deactivetype", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("item_asset", "deactivetype");
        }
    }
}
