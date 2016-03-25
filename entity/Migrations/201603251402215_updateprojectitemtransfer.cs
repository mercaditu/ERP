namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateprojectitemtransfer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "project_tag_detail",
                c => new
                    {
                        id_project_tag_detail = c.Int(nullable: false, identity: true),
                        id_project = c.Int(nullable: false),
                        id_tag = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.id_project_tag_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("projects", t => t.id_project, cascadeDelete: true)
                .ForeignKey("project_tag", t => t.id_tag, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_project)
                .Index(t => t.id_tag)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "project_tag",
                c => new
                    {
                        id_tag = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, unicode: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.id_tag)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            AddColumn("project_task", "id_range", c => c.Int());
            AddColumn("project_task", "number", c => c.String(unicode: false));
            AddColumn("purchase_tender_detail", "id_vat_group", c => c.Int(nullable: false));
            AddColumn("item_transfer_detail", "status", c => c.Int(nullable: false));
            CreateIndex("project_task", "id_range");
            AddForeignKey("project_task", "id_range", "app_document_range", "id_range");
        }
        
        public override void Down()
        {
            DropForeignKey("project_task", "id_range", "app_document_range");
            DropForeignKey("project_tag_detail", "id_user", "security_user");
            DropForeignKey("project_tag", "id_user", "security_user");
            DropForeignKey("project_tag_detail", "id_tag", "project_tag");
            DropForeignKey("project_tag", "id_company", "app_company");
            DropForeignKey("project_tag_detail", "id_project", "projects");
            DropForeignKey("project_tag_detail", "id_company", "app_company");
            DropIndex("project_tag", new[] { "id_user" });
            DropIndex("project_tag", new[] { "id_company" });
            DropIndex("project_tag_detail", new[] { "id_user" });
            DropIndex("project_tag_detail", new[] { "id_company" });
            DropIndex("project_tag_detail", new[] { "id_tag" });
            DropIndex("project_tag_detail", new[] { "id_project" });
            DropIndex("project_task", new[] { "id_range" });
            DropColumn("item_transfer_detail", "status");
            DropColumn("purchase_tender_detail", "id_vat_group");
            DropColumn("project_task", "number");
            DropColumn("project_task", "id_range");
            DropTable("project_tag");
            DropTable("project_tag_detail");
        }
    }
}
