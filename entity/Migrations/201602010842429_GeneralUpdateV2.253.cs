namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GeneralUpdateV2253 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "app_name_template",
                c => new
                    {
                        id_name_template = c.Short(nullable: false, identity: true),
                        app_name = c.Int(nullable: false),
                        name = c.String(nullable: false, unicode: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.id_name_template)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "app_name_template_detail",
                c => new
                    {
                        id_name_template_detail = c.Short(nullable: false, identity: true),
                        id_name_template = c.Short(nullable: false),
                        sequence = c.Short(nullable: false),
                        question = c.String(unicode: false),
                        answer_type = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.id_name_template_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_name_template", t => t.id_name_template, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_name_template)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            AddColumn("items", "id_name_template", c => c.Short());
            AddColumn("project_task", "item_description", c => c.String(unicode: false));
            AddColumn("project_template_detail", "item_description", c => c.String(unicode: false));
            AddColumn("purchase_invoice", "id_journal", c => c.Int());
            AddColumn("purchase_invoice", "barcode", c => c.String(unicode: false));
            AddColumn("payments", "id_journal", c => c.Int());
            AddColumn("purchase_order", "barcode", c => c.String(unicode: false));
            AddColumn("purchase_return", "id_journal", c => c.Int());
            AddColumn("purchase_return", "barcode", c => c.String(unicode: false));
            AddColumn("sales_invoice", "id_journal", c => c.Int());
            AddColumn("sales_invoice", "barcode", c => c.String(unicode: false));
            AddColumn("sales_budget", "barcode", c => c.String(unicode: false));
            AddColumn("sales_order", "barcode", c => c.String(unicode: false));
            AddColumn("sales_return", "id_journal", c => c.Int());
            AddColumn("sales_return", "barcode", c => c.String(unicode: false));
            AddColumn("payment_withholding_tax", "id_journal", c => c.Int());
            CreateIndex("purchase_invoice", "id_journal");
            CreateIndex("payments", "id_journal");
            CreateIndex("sales_invoice", "id_journal");
            CreateIndex("sales_return", "id_journal");
            CreateIndex("payment_withholding_tax", "id_journal");
            AddForeignKey("purchase_invoice", "id_journal", "accounting_journal", "id_journal");
            AddForeignKey("payments", "id_journal", "accounting_journal", "id_journal");
            AddForeignKey("sales_invoice", "id_journal", "accounting_journal", "id_journal");
            AddForeignKey("sales_return", "id_journal", "accounting_journal", "id_journal");
            AddForeignKey("payment_withholding_tax", "id_journal", "accounting_journal", "id_journal");
            DropColumn("items", "link_code");
            DropColumn("items", "has_promo");
            DropColumn("items", "is_substitute");
            DropColumn("project_task", "name");
            DropColumn("project_template_detail", "name");
            DropColumn("purchase_invoice", "is_accounted");
            DropColumn("purchase_order", "is_accounted");
            DropColumn("purchase_return", "is_accounted");
            DropColumn("sales_invoice", "is_accounted");
            DropColumn("sales_budget", "is_accounted");
            DropColumn("sales_order", "is_accounted");
            DropColumn("sales_return", "is_accounted");
        }
        
        public override void Down()
        {
            AddColumn("sales_return", "is_accounted", c => c.Boolean(nullable: false));
            AddColumn("sales_order", "is_accounted", c => c.Boolean(nullable: false));
            AddColumn("sales_budget", "is_accounted", c => c.Boolean(nullable: false));
            AddColumn("sales_invoice", "is_accounted", c => c.Boolean(nullable: false));
            AddColumn("purchase_return", "is_accounted", c => c.Boolean(nullable: false));
            AddColumn("purchase_order", "is_accounted", c => c.Boolean(nullable: false));
            AddColumn("purchase_invoice", "is_accounted", c => c.Boolean(nullable: false));
            AddColumn("project_template_detail", "name", c => c.String(unicode: false));
            AddColumn("project_task", "name", c => c.String(nullable: false, unicode: false));
            AddColumn("items", "is_substitute", c => c.Boolean(nullable: false));
            AddColumn("items", "has_promo", c => c.Boolean(nullable: false));
            AddColumn("items", "link_code", c => c.Int());
            DropForeignKey("payment_withholding_tax", "id_journal", "accounting_journal");
            DropForeignKey("app_name_template", "id_user", "security_user");
            DropForeignKey("app_name_template_detail", "id_user", "security_user");
            DropForeignKey("app_name_template_detail", "id_name_template", "app_name_template");
            DropForeignKey("app_name_template_detail", "id_company", "app_company");
            DropForeignKey("app_name_template", "id_company", "app_company");
            DropForeignKey("sales_return", "id_journal", "accounting_journal");
            DropForeignKey("sales_invoice", "id_journal", "accounting_journal");
            DropForeignKey("payments", "id_journal", "accounting_journal");
            DropForeignKey("purchase_invoice", "id_journal", "accounting_journal");
            DropIndex("payment_withholding_tax", new[] { "id_journal" });
            DropIndex("app_name_template_detail", new[] { "id_user" });
            DropIndex("app_name_template_detail", new[] { "id_company" });
            DropIndex("app_name_template_detail", new[] { "id_name_template" });
            DropIndex("app_name_template", new[] { "id_user" });
            DropIndex("app_name_template", new[] { "id_company" });
            DropIndex("sales_return", new[] { "id_journal" });
            DropIndex("sales_invoice", new[] { "id_journal" });
            DropIndex("payments", new[] { "id_journal" });
            DropIndex("purchase_invoice", new[] { "id_journal" });
            DropColumn("payment_withholding_tax", "id_journal");
            DropColumn("sales_return", "barcode");
            DropColumn("sales_return", "id_journal");
            DropColumn("sales_order", "barcode");
            DropColumn("sales_budget", "barcode");
            DropColumn("sales_invoice", "barcode");
            DropColumn("sales_invoice", "id_journal");
            DropColumn("purchase_return", "barcode");
            DropColumn("purchase_return", "id_journal");
            DropColumn("purchase_order", "barcode");
            DropColumn("payments", "id_journal");
            DropColumn("purchase_invoice", "barcode");
            DropColumn("purchase_invoice", "id_journal");
            DropColumn("project_template_detail", "item_description");
            DropColumn("project_task", "item_description");
            DropColumn("items", "id_name_template");
            DropTable("app_name_template_detail");
            DropTable("app_name_template");
        }
    }
}
