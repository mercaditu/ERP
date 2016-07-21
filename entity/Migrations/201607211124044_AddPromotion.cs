namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPromotion : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "sales_promotion",
                c => new
                    {
                        id_sales_promotion = c.Int(nullable: false, identity: true),
                        types = c.Int(nullable: false),
                        name = c.String(unicode: false),
                        reference = c.Int(nullable: false),
                        date_start = c.DateTime(nullable: false, precision: 0),
                        date_end = c.DateTime(nullable: false, precision: 0),
                        quantity_min = c.Decimal(nullable: false, precision: 20, scale: 4),
                        quantity_max = c.Decimal(nullable: false, precision: 20, scale: 4),
                        quantity_step = c.Decimal(nullable: false, precision: 20, scale: 4),
                        is_percentage = c.Boolean(nullable: false),
                        result_value = c.Decimal(nullable: false, precision: 20, scale: 4),
                        result_step = c.Decimal(nullable: false, precision: 20, scale: 4),
                        reference_bonus = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_sales_promotion)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            AddColumn("production_order_detail", "code", c => c.String(unicode: false));
            AddColumn("item_tag_detail", "is_default", c => c.Boolean(nullable: false));
            CreateIndex("item_asset", "id_contact");
            AddForeignKey("item_asset", "id_contact", "contacts", "id_contact");
        }
        
        public override void Down()
        {
            DropForeignKey("sales_promotion", "id_user", "security_user");
            DropForeignKey("sales_promotion", "id_company", "app_company");
            DropForeignKey("item_asset", "id_contact", "contacts");
            DropIndex("sales_promotion", new[] { "id_user" });
            DropIndex("sales_promotion", new[] { "id_company" });
            DropIndex("item_asset", new[] { "id_contact" });
            DropColumn("item_tag_detail", "is_default");
            DropColumn("production_order_detail", "code");
            DropTable("sales_promotion");
        }
    }
}
