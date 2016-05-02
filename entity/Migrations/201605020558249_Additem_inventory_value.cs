namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Additem_inventory_value : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("item_inventory_dimension", "id_inventory", "item_inventory");
            DropIndex("item_inventory_dimension", new[] { "id_inventory" });
            CreateTable(
                "item_inventory_value",
                c => new
                    {
                        id_item_inventory_value = c.Long(nullable: false, identity: true),
                        id_inventory_detail = c.Long(nullable: false),
                        id_currencyfx = c.Int(nullable: false),
                        unit_value = c.Decimal(nullable: false, precision: 20, scale: 4),
                        comment = c.String(unicode: false),
                        is_estimate = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        item_inventory_detail_id_inventory_detail = c.Int(),
                    })
                .PrimaryKey(t => t.id_item_inventory_value)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_currencyfx", t => t.id_currencyfx, cascadeDelete: true)
                .ForeignKey("item_inventory_detail", t => t.item_inventory_detail_id_inventory_detail)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_currencyfx)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.item_inventory_detail_id_inventory_detail);
            
            AddColumn("item_movement_value", "item_inventory_detail_id_inventory_detail", c => c.Int());
            AddColumn("item_inventory_dimension", "id_inventory_detail", c => c.Int(nullable: false));
            CreateIndex("item_movement_value", "item_inventory_detail_id_inventory_detail");
            CreateIndex("item_inventory_dimension", "id_inventory_detail");
            AddForeignKey("item_inventory_dimension", "id_inventory_detail", "item_inventory_detail", "id_inventory_detail", cascadeDelete: true);
            AddForeignKey("item_movement_value", "item_inventory_detail_id_inventory_detail", "item_inventory_detail", "id_inventory_detail");
            DropColumn("item_inventory_dimension", "id_inventory");
        }
        
        public override void Down()
        {
            AddColumn("item_inventory_dimension", "id_inventory", c => c.Int(nullable: false));
            DropForeignKey("item_inventory_value", "id_user", "security_user");
            DropForeignKey("item_inventory_value", "item_inventory_detail_id_inventory_detail", "item_inventory_detail");
            DropForeignKey("item_inventory_value", "id_currencyfx", "app_currencyfx");
            DropForeignKey("item_inventory_value", "id_company", "app_company");
            DropForeignKey("item_movement_value", "item_inventory_detail_id_inventory_detail", "item_inventory_detail");
            DropForeignKey("item_inventory_dimension", "id_inventory_detail", "item_inventory_detail");
            DropIndex("item_inventory_value", new[] { "item_inventory_detail_id_inventory_detail" });
            DropIndex("item_inventory_value", new[] { "id_user" });
            DropIndex("item_inventory_value", new[] { "id_company" });
            DropIndex("item_inventory_value", new[] { "id_currencyfx" });
            DropIndex("item_inventory_dimension", new[] { "id_inventory_detail" });
            DropIndex("item_movement_value", new[] { "item_inventory_detail_id_inventory_detail" });
            DropColumn("item_inventory_dimension", "id_inventory_detail");
            DropColumn("item_movement_value", "item_inventory_detail_id_inventory_detail");
            DropTable("item_inventory_value");
            CreateIndex("item_inventory_dimension", "id_inventory");
            AddForeignKey("item_inventory_dimension", "id_inventory", "item_inventory", "id_inventory", cascadeDelete: true);
        }
    }
}
