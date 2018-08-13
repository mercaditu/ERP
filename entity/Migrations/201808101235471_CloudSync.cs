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
            
            AddColumn("app_contract", "cloud_id", c => c.Int());
            AddColumn("project_task", "sequence", c => c.Int());
            AddColumn("project_task", "revision", c => c.Short());
            AddColumn("app_vat_group", "cloud_id", c => c.Int());
            AddColumn("production_order_detail", "sequence", c => c.Int());
            AddColumn("production_order_detail", "revision", c => c.Short());
            AddColumn("sales_promotion", "cloud_id", c => c.Int());
        }
        
        public override void Down()
        {
            DropForeignKey("app_cloud_sync", "id_user", "security_user");
            DropForeignKey("app_cloud_sync", "id_company", "app_company");
            DropIndex("app_cloud_sync", new[] { "id_user" });
            DropIndex("app_cloud_sync", new[] { "id_company" });
            DropColumn("sales_promotion", "cloud_id");
            DropColumn("production_order_detail", "revision");
            DropColumn("production_order_detail", "sequence");
            DropColumn("app_vat_group", "cloud_id");
            DropColumn("project_task", "revision");
            DropColumn("project_task", "sequence");
            DropColumn("app_contract", "cloud_id");
            DropTable("app_cloud_sync");
        }
    }
}
