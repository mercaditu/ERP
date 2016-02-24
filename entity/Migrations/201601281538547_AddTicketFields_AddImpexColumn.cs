namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTicketFields_AddImpexColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("projects", "code", c => c.String(unicode: false));
            AddColumn("purchase_invoice", "is_impex", c => c.Boolean(nullable: false));
            AddColumn("purchase_order", "is_impex", c => c.Boolean(nullable: false));
            AddColumn("purchase_return", "is_impex", c => c.Boolean(nullable: false));
            AddColumn("sales_invoice", "is_impex", c => c.Boolean(nullable: false));
            AddColumn("sales_budget", "is_impex", c => c.Boolean(nullable: false));
            AddColumn("sales_order", "is_impex", c => c.Boolean(nullable: false));
            AddColumn("sales_return", "is_impex", c => c.Boolean(nullable: false));
            AddColumn("item_transfer", "id_project", c => c.Int());
            AddColumn("item_transfer", "id_range", c => c.Int());
            AddColumn("item_transfer", "number", c => c.String(unicode: false));
            AddColumn("item_transfer_detail", "id_project_task", c => c.Int());
            CreateIndex("item_transfer", "id_project");
            CreateIndex("item_transfer", "id_range");
            CreateIndex("item_transfer_detail", "id_project_task");
            AddForeignKey("item_transfer", "id_range", "app_document_range", "id_range");
            AddForeignKey("item_transfer_detail", "id_project_task", "project_task", "id_project_task");
            AddForeignKey("item_transfer", "id_project", "projects", "id_project");
        }
        
        public override void Down()
        {
            DropForeignKey("item_transfer", "id_project", "projects");
            DropForeignKey("item_transfer_detail", "id_project_task", "project_task");
            DropForeignKey("item_transfer", "id_range", "app_document_range");
            DropIndex("item_transfer_detail", new[] { "id_project_task" });
            DropIndex("item_transfer", new[] { "id_range" });
            DropIndex("item_transfer", new[] { "id_project" });
            DropColumn("item_transfer_detail", "id_project_task");
            DropColumn("item_transfer", "number");
            DropColumn("item_transfer", "id_range");
            DropColumn("item_transfer", "id_project");
            DropColumn("sales_return", "is_impex");
            DropColumn("sales_order", "is_impex");
            DropColumn("sales_budget", "is_impex");
            DropColumn("sales_invoice", "is_impex");
            DropColumn("purchase_return", "is_impex");
            DropColumn("purchase_order", "is_impex");
            DropColumn("purchase_invoice", "is_impex");
            DropColumn("projects", "code");
        }
    }
}
