namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Movement_Packinglist : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "item_movement_value_rel",
                c => new
                    {
                        id_movement_value_rel = c.Long(nullable: false, identity: true),
                        is_estimate = c.Boolean(nullable: false),
                        batch_code = c.String(unicode: false),
                        entry_date = c.DateTime(precision: 0),
                        total_value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_movement_value_rel)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_movement_value_detail",
                c => new
                    {
                        id_movement_value_detail = c.Long(nullable: false, identity: true),
                        id_movement_value_rel = c.Long(nullable: false),
                        unit_value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        comment = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.id_movement_value_detail)                
                .ForeignKey("item_movement_value_rel", t => t.id_movement_value_rel, cascadeDelete: true)
                .Index(t => t.id_movement_value_rel);
            
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
            AddColumn("item_movement", "id_movement_value_rel", c => c.Long());
            AddColumn("purchase_packing_detail", "parent_id_purchase_packing_detail", c => c.Int());
            AddColumn("sales_packing_detail", "parent_id_sales_packing_detail", c => c.Int());
            AddColumn("app_comment", "ref_id", c => c.Int(nullable: false));
            CreateIndex("item_movement", "id_movement_value_rel");
            CreateIndex("purchase_packing_detail", "parent_id_purchase_packing_detail");
            CreateIndex("sales_packing_detail", "parent_id_sales_packing_detail");
            AddForeignKey("purchase_packing_detail", "parent_id_purchase_packing_detail", "purchase_packing_detail", "id_purchase_packing_detail");
            AddForeignKey("sales_packing_detail", "parent_id_sales_packing_detail", "sales_packing_detail", "id_sales_packing_detail");
            AddForeignKey("item_movement", "id_movement_value_rel", "item_movement_value_rel", "id_movement_value_rel");
        }
        
        public override void Down()
        {
            DropForeignKey("app_notification", "security_user_id_user", "security_user");
            DropForeignKey("app_notification", "notified_user_id_user", "security_user");
            DropForeignKey("app_notification", "notified_department_id_department", "app_department");
            DropForeignKey("app_notification", "id_company", "app_company");
            DropForeignKey("item_movement_value_rel", "id_user", "security_user");
            DropForeignKey("item_movement_value_detail", "id_movement_value_rel", "item_movement_value_rel");
            DropForeignKey("item_movement", "id_movement_value_rel", "item_movement_value_rel");
            DropForeignKey("item_movement_value_rel", "id_company", "app_company");
            DropForeignKey("sales_packing_detail", "parent_id_sales_packing_detail", "sales_packing_detail");
            DropForeignKey("purchase_packing_detail", "parent_id_purchase_packing_detail", "purchase_packing_detail");
            DropIndex("app_notification", new[] { "security_user_id_user" });
            DropIndex("app_notification", new[] { "notified_user_id_user" });
            DropIndex("app_notification", new[] { "notified_department_id_department" });
            DropIndex("app_notification", new[] { "id_company" });
            DropIndex("item_movement_value_detail", new[] { "id_movement_value_rel" });
            DropIndex("item_movement_value_rel", new[] { "id_user" });
            DropIndex("item_movement_value_rel", new[] { "id_company" });
            DropIndex("sales_packing_detail", new[] { "parent_id_sales_packing_detail" });
            DropIndex("purchase_packing_detail", new[] { "parent_id_purchase_packing_detail" });
            DropIndex("item_movement", new[] { "id_movement_value_rel" });
            DropColumn("app_comment", "ref_id");
            DropColumn("sales_packing_detail", "parent_id_sales_packing_detail");
            DropColumn("purchase_packing_detail", "parent_id_purchase_packing_detail");
            DropColumn("item_movement", "id_movement_value_rel");
            DropColumn("app_department", "type");
            DropTable("app_notification");
            DropTable("item_movement_value_detail");
            DropTable("item_movement_value_rel");
        }
    }
}
