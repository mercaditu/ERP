namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PaymentSchedualUpdate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "item_branch_safety",
                c => new
                    {
                        id_branch_safety = c.Int(nullable: false, identity: true),
                        id_branch = c.Int(nullable: false),
                        id_item_product = c.Int(nullable: false),
                        quantity = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_branch_safety)                
                .ForeignKey("app_branch", t => t.id_branch, cascadeDelete: true)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("item_product", t => t.id_item_product, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_branch)
                .Index(t => t.id_item_product)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            AddColumn("app_company", "has_interest", c => c.Boolean(nullable: false));
            AddColumn("app_currency", "code", c => c.String(unicode: false));
            AddColumn("production_execution_detail", "batch", c => c.String(unicode: false));
            AddColumn("production_execution_detail", "expiry_date", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("payment_schedual", "is_interest", c => c.Boolean(nullable: false));
            AddColumn("sales_rep", "is_collection_agent", c => c.Boolean(nullable: false));
            DropColumn("app_company", "representative_name");
            DropColumn("app_company", "representative_gov_code");
            DropColumn("app_company", "accountant_name");
            DropColumn("app_company", "accountant_gov_code");
        }
        
        public override void Down()
        {
            AddColumn("app_company", "accountant_gov_code", c => c.String(unicode: false));
            AddColumn("app_company", "accountant_name", c => c.String(unicode: false));
            AddColumn("app_company", "representative_gov_code", c => c.String(unicode: false));
            AddColumn("app_company", "representative_name", c => c.String(unicode: false));
            DropForeignKey("item_branch_safety", "id_user", "security_user");
            DropForeignKey("item_branch_safety", "id_item_product", "item_product");
            DropForeignKey("item_branch_safety", "id_company", "app_company");
            DropForeignKey("item_branch_safety", "id_branch", "app_branch");
            DropIndex("item_branch_safety", new[] { "id_user" });
            DropIndex("item_branch_safety", new[] { "id_company" });
            DropIndex("item_branch_safety", new[] { "id_item_product" });
            DropIndex("item_branch_safety", new[] { "id_branch" });
            DropColumn("sales_rep", "is_collection_agent");
            DropColumn("payment_schedual", "is_interest");
            DropColumn("production_execution_detail", "expiry_date");
            DropColumn("production_execution_detail", "batch");
            DropColumn("app_currency", "code");
            DropColumn("app_company", "has_interest");
            DropTable("item_branch_safety");
        }
    }
}
