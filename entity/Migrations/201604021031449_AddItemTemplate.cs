namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddItemTemplate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "item_template",
                c => new
                    {
                        id_template = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, unicode: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.id_template)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_template_detail",
                c => new
                    {
                        id_template_detail = c.Int(nullable: false, identity: true),
                        id_template = c.Int(nullable: false),
                        question = c.String(nullable: false, unicode: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.id_template_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("item_template", t => t.id_template, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_template)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
        }
        
        public override void Down()
        {
            DropForeignKey("item_template", "id_user", "security_user");
            DropForeignKey("item_template_detail", "id_user", "security_user");
            DropForeignKey("item_template_detail", "id_template", "item_template");
            DropForeignKey("item_template_detail", "id_company", "app_company");
            DropForeignKey("item_template", "id_company", "app_company");
            DropIndex("item_template_detail", new[] { "id_user" });
            DropIndex("item_template_detail", new[] { "id_company" });
            DropIndex("item_template_detail", new[] { "id_template" });
            DropIndex("item_template", new[] { "id_user" });
            DropIndex("item_template", new[] { "id_company" });
            DropTable("item_template_detail");
            DropTable("item_template");
        }
    }
}
