namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddItem_inventory_dimension : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "item_inventory_dimension",
                c => new
                    {
                        id_item_inventory_dimension = c.Int(nullable: false, identity: true),
                        id_inventory = c.Int(nullable: false),
                        id_dimension = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 4),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.id_item_inventory_dimension)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_dimension", t => t.id_dimension, cascadeDelete: true)
                .ForeignKey("item_inventory", t => t.id_inventory, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_inventory)
                .Index(t => t.id_dimension)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            AddColumn("app_currencyfx", "is_reverse", c => c.Boolean(nullable: false));
            AddColumn("app_currency", "is_reverse", c => c.Boolean(nullable: false));
            AddColumn("app_contract", "is_promissory", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("item_inventory_dimension", "id_user", "security_user");
            DropForeignKey("item_inventory_dimension", "id_inventory", "item_inventory");
            DropForeignKey("item_inventory_dimension", "id_dimension", "app_dimension");
            DropForeignKey("item_inventory_dimension", "id_company", "app_company");
            DropIndex("item_inventory_dimension", new[] { "id_user" });
            DropIndex("item_inventory_dimension", new[] { "id_company" });
            DropIndex("item_inventory_dimension", new[] { "id_dimension" });
            DropIndex("item_inventory_dimension", new[] { "id_inventory" });
            DropColumn("app_contract", "is_promissory");
            DropColumn("app_currency", "is_reverse");
            DropColumn("app_currencyfx", "is_reverse");
            DropTable("item_inventory_dimension");
        }
    }
}
