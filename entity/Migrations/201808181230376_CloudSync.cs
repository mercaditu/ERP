namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CloudSync : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "app_cloud_sync",
                c => new
                    {
                        id_cloud_sync = c.Int(nullable: false, identity: true),
                        type = c.Int(nullable: false),
                        comment = c.String(nullable: false, unicode: false),
                        status = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_cloud_sync)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            AddColumn("app_account_detail", "is_accounted", c => c.Boolean(nullable: false));
            AddColumn("app_account_detail", "is_archieved", c => c.Boolean(nullable: false));
            AddColumn("app_contract", "cloud_id", c => c.Int());
            AddColumn("items", "ref_id", c => c.Long());
            AddColumn("items", "chart_id", c => c.Long());
            AddColumn("project_task", "sequence", c => c.Int());
            AddColumn("project_task", "parent_child", c => c.Int(nullable: false));
            AddColumn("project_task", "revision", c => c.Short());
            AddColumn("item_asset", "quantity", c => c.Int());
            AddColumn("item_asset", "id_currency", c => c.Int());
            AddColumn("app_vat_group", "cloud_id", c => c.Int());
            AddColumn("production_order_detail", "sequence", c => c.Int());
            AddColumn("production_order_detail", "parent_child", c => c.Int(nullable: false));
            AddColumn("production_order_detail", "revision", c => c.Short());
            AddColumn("sales_promotion", "cloud_id", c => c.Int());
            CreateIndex("item_asset", "id_currency");
            AddForeignKey("item_asset", "id_currency", "app_currency", "id_currency");
        }
        
        public override void Down()
        {
            DropForeignKey("app_cloud_sync", "id_user", "security_user");
            DropForeignKey("app_cloud_sync", "id_company", "app_company");
            DropForeignKey("item_asset", "id_currency", "app_currency");
            DropIndex("app_cloud_sync", new[] { "id_user" });
            DropIndex("app_cloud_sync", new[] { "id_company" });
            DropIndex("item_asset", new[] { "id_currency" });
            DropColumn("sales_promotion", "cloud_id");
            DropColumn("production_order_detail", "revision");
            DropColumn("production_order_detail", "parent_child");
            DropColumn("production_order_detail", "sequence");
            DropColumn("app_vat_group", "cloud_id");
            DropColumn("item_asset", "id_currency");
            DropColumn("item_asset", "quantity");
            DropColumn("project_task", "revision");
            DropColumn("project_task", "parent_child");
            DropColumn("project_task", "sequence");
            DropColumn("items", "chart_id");
            DropColumn("items", "ref_id");
            DropColumn("app_contract", "cloud_id");
            DropColumn("app_account_detail", "is_archieved");
            DropColumn("app_account_detail", "is_accounted");
            DropTable("app_cloud_sync");
        }
    }
}
