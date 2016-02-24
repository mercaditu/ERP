namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatestatusTaskview : DbMigration
    {
        public override void Up()
        {
            AddColumn("project_task", "ProjectStatus", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("project_task", "ProjectStatus");
        }
    }
}
