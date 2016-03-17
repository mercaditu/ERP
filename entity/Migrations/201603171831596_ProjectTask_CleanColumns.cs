namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectTask_CleanColumns : DbMigration
    {
        public override void Up()
        {
            DropColumn("project_task", "ProjectStatus");
        }
        
        public override void Down()
        {
            AddColumn("project_task", "ProjectStatus", c => c.Int());
        }
    }
}
