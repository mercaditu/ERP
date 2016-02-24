namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDocumentRange : DbMigration
    {
        public override void Up()
        {
            AddColumn("app_document_range", "id_branch", c => c.Int());
            AddColumn("app_document_range", "id_terminal", c => c.Int());
            CreateIndex("app_document_range", "id_branch");
            CreateIndex("app_document_range", "id_terminal");
            AddForeignKey("app_document_range", "id_branch", "app_branch", "id_branch");
            AddForeignKey("app_document_range", "id_terminal", "app_terminal", "id_terminal");
        }
        
        public override void Down()
        {
            DropForeignKey("app_document_range", "id_terminal", "app_terminal");
            DropForeignKey("app_document_range", "id_branch", "app_branch");
            DropIndex("app_document_range", new[] { "id_terminal" });
            DropIndex("app_document_range", new[] { "id_branch" });
            DropColumn("app_document_range", "id_terminal");
            DropColumn("app_document_range", "id_branch");
        }
    }
}
