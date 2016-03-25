namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateitem_transfer : DbMigration
    {
        public override void Up()
        {
            AddColumn("item_transfer_detail", "status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("item_transfer_detail", "status");
        }
    }
}
