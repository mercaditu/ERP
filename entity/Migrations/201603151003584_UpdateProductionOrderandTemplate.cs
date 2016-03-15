namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProductionOrderandTemplate : DbMigration
    {
        public override void Up()
        {
            AddColumn("production_order", "id_branch", c => c.Int(nullable: false));
            AddColumn("production_order", "id_terminal", c => c.Int(nullable: false));
            AddColumn("production_order", "id_range", c => c.Int());
            AddColumn("project_template", "code", c => c.String(unicode: false));
            CreateIndex("production_order", "id_range");
            AddForeignKey("production_order", "id_range", "app_document_range", "id_range");
            DropColumn("project_template", "comment");
        }
        
        public override void Down()
        {
            AddColumn("project_template", "comment", c => c.String(unicode: false));
            DropForeignKey("production_order", "id_range", "app_document_range");
            DropIndex("production_order", new[] { "id_range" });
            DropColumn("project_template", "code");
            DropColumn("production_order", "id_range");
            DropColumn("production_order", "id_terminal");
            DropColumn("production_order", "id_branch");
        }
    }
}
