namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PackinglistNotificationchanges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "app_notification",
                c => new
                    {
                        id_notification = c.Int(nullable: false, identity: true),
                        id_application = c.Int(nullable: false),
                        ref_id = c.Int(nullable: false),
                        comment = c.String(nullable: false, unicode: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        notified_department_id_department = c.Int(),
                        notified_user_id_user = c.Int(),
                        security_user_id_user = c.Int(),
                    })
                .PrimaryKey(t => t.id_notification)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_department", t => t.notified_department_id_department)
                .ForeignKey("security_user", t => t.notified_user_id_user)
                .ForeignKey("security_user", t => t.security_user_id_user)
                .Index(t => t.id_company)
                .Index(t => t.notified_department_id_department)
                .Index(t => t.notified_user_id_user)
                .Index(t => t.security_user_id_user);
            
            AddColumn("app_department", "type", c => c.Int(nullable: false));
            AddColumn("purchase_packing_detail", "parent_id_purchase_packing_detail", c => c.Int());
            AddColumn("sales_packing_detail", "parent_id_sales_packing_detail", c => c.Int());
            AddColumn("app_comment", "ref_id", c => c.Int(nullable: false));
            CreateIndex("purchase_packing_detail", "parent_id_purchase_packing_detail");
            CreateIndex("sales_packing_detail", "parent_id_sales_packing_detail");
            AddForeignKey("purchase_packing_detail", "parent_id_purchase_packing_detail", "purchase_packing_detail", "id_purchase_packing_detail");
            AddForeignKey("sales_packing_detail", "parent_id_sales_packing_detail", "sales_packing_detail", "id_sales_packing_detail");
        }
        
        public override void Down()
        {
            DropForeignKey("app_notification", "security_user_id_user", "security_user");
            DropForeignKey("app_notification", "notified_user_id_user", "security_user");
            DropForeignKey("app_notification", "notified_department_id_department", "app_department");
            DropForeignKey("app_notification", "id_company", "app_company");
            DropForeignKey("sales_packing_detail", "parent_id_sales_packing_detail", "sales_packing_detail");
            DropForeignKey("purchase_packing_detail", "parent_id_purchase_packing_detail", "purchase_packing_detail");
            DropIndex("app_notification", new[] { "security_user_id_user" });
            DropIndex("app_notification", new[] { "notified_user_id_user" });
            DropIndex("app_notification", new[] { "notified_department_id_department" });
            DropIndex("app_notification", new[] { "id_company" });
            DropIndex("sales_packing_detail", new[] { "parent_id_sales_packing_detail" });
            DropIndex("purchase_packing_detail", new[] { "parent_id_purchase_packing_detail" });
            DropColumn("app_comment", "ref_id");
            DropColumn("sales_packing_detail", "parent_id_sales_packing_detail");
            DropColumn("purchase_packing_detail", "parent_id_purchase_packing_detail");
            DropColumn("app_department", "type");
            DropTable("app_notification");
        }
    }
}
