namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSecurity_roleanditem_movement_value : DbMigration
    {
        public override void Up()
        {
            AddColumn("security_role", "is_master", c => c.Boolean(nullable: false));
            AddColumn("item_movement_value", "is_estimate", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("item_movement_value", "is_estimate");
            DropColumn("security_role", "is_master");
        }
    }
}
