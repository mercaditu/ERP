namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ADDItem_Receipe : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("item_recepie", "item_detail_id_item", "items");
            DropForeignKey("item_recepie", "item_head_id_item", "items");
            DropIndex("item_recepie", new[] { "item_detail_id_item" });
            DropIndex("item_recepie", new[] { "item_head_id_item" });
            CreateTable(
                "item_recepie_detail",
                c => new
                    {
                        id_recepie_detail = c.Int(nullable: false, identity: true),
                        id_recepie = c.Decimal(nullable: false, precision: 20, scale: 4),
                        id_item = c.Decimal(nullable: false, precision: 20, scale: 4),
                        quantity = c.Decimal(nullable: false, precision: 20, scale: 4),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        item_id_item = c.Int(),
                        item_recepie_id_recepie = c.Int(),
                    })
                .PrimaryKey(t => t.id_recepie_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("items", t => t.item_id_item)
                .ForeignKey("item_recepie", t => t.item_recepie_id_recepie)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.item_id_item)
                .Index(t => t.item_recepie_id_recepie);
            
            AddColumn("item_recepie", "id_item", c => c.Decimal(nullable: false, precision: 20, scale: 4));
            DropColumn("item_recepie", "value");
            DropColumn("item_recepie", "item_detail_id_item");
            DropColumn("item_recepie", "item_head_id_item");
        }
        
        public override void Down()
        {
            AddColumn("item_recepie", "item_head_id_item", c => c.Int());
            AddColumn("item_recepie", "item_detail_id_item", c => c.Int());
            AddColumn("item_recepie", "value", c => c.Decimal(nullable: false, precision: 20, scale: 4));
            DropForeignKey("item_recepie_detail", "id_user", "security_user");
            DropForeignKey("item_recepie_detail", "item_recepie_id_recepie", "item_recepie");
            DropForeignKey("item_recepie_detail", "item_id_item", "items");
            DropForeignKey("item_recepie_detail", "id_company", "app_company");
            DropIndex("item_recepie_detail", new[] { "item_recepie_id_recepie" });
            DropIndex("item_recepie_detail", new[] { "item_id_item" });
            DropIndex("item_recepie_detail", new[] { "id_user" });
            DropIndex("item_recepie_detail", new[] { "id_company" });
            DropColumn("item_recepie", "id_item");
            DropTable("item_recepie_detail");
            CreateIndex("item_recepie", "item_head_id_item");
            CreateIndex("item_recepie", "item_detail_id_item");
            AddForeignKey("item_recepie", "item_head_id_item", "items", "id_item");
            AddForeignKey("item_recepie", "item_detail_id_item", "items", "id_item");
        }
    }
}
