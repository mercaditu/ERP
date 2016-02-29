namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcontacttag : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "contact_tag",
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
            
            CreateTable(
                "contact_tag_detail",
                c => new
                    {
                        id_contact_tag_detail = c.Int(nullable: false, identity: true),
                        id_contact = c.Int(nullable: false),
                        id_tag = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.id_contact_tag_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("contacts", t => t.id_contact, cascadeDelete: true)
                .ForeignKey("contact_tag", t => t.id_tag, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_contact)
                .Index(t => t.id_tag)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
        }
        
        public override void Down()
        {
            DropForeignKey("contact_tag", "id_user", "security_user");
            DropForeignKey("contact_tag_detail", "id_user", "security_user");
            DropForeignKey("contact_tag_detail", "id_tag", "contact_tag");
            DropForeignKey("contact_tag_detail", "id_contact", "contacts");
            DropForeignKey("contact_tag_detail", "id_company", "app_company");
            DropForeignKey("contact_tag", "id_company", "app_company");
            DropIndex("contact_tag_detail", new[] { "id_user" });
            DropIndex("contact_tag_detail", new[] { "id_company" });
            DropIndex("contact_tag_detail", new[] { "id_tag" });
            DropIndex("contact_tag_detail", new[] { "id_contact" });
            DropIndex("contact_tag", new[] { "id_user" });
            DropIndex("contact_tag", new[] { "id_company" });
            DropTable("contact_tag_detail");
            DropTable("contact_tag");
        }
    }
}
