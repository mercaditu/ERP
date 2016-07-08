namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventManagement : DbMigration
    {
        public override void Up()
        {
            AddColumn("project_event", "status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("project_event", "status");
        }
    }
}
