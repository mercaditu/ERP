namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_AssetMaintainance : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "app_branch_walkins",
                c => new
                    {
                        id_branch_walkin = c.Int(nullable: false, identity: true),
                        id_branch = c.Int(nullable: false),
                        start_date = c.DateTime(nullable: false, precision: 0),
                        end_date = c.DateTime(nullable: false, precision: 0),
                        quantity = c.Decimal(nullable: false, precision: 20, scale: 4),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.id_branch_walkin)                
                .ForeignKey("app_branch", t => t.id_branch, cascadeDelete: true)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_branch)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_asset_maintainance",
                c => new
                    {
                        id_maintainance = c.Int(nullable: false, identity: true),
                        id_item_asset = c.Int(nullable: false),
                        status = c.Int(nullable: false),
                        start_date = c.DateTime(nullable: false, precision: 0),
                        end_date = c.DateTime(nullable: false, precision: 0),
                        comment = c.String(unicode: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.id_maintainance)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("item_asset", t => t.id_item_asset, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_item_asset)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_asset_maintainance_detail",
                c => new
                    {
                        id_maintainance_detail = c.Int(nullable: false, identity: true),
                        id_maintainance = c.Int(nullable: false),
                        id_item = c.Int(),
                        item_description = c.String(unicode: false),
                        quantity = c.Decimal(nullable: false, precision: 20, scale: 4),
                        unit_cost = c.Decimal(nullable: false, precision: 20, scale: 4),
                        id_currencyfx = c.Int(),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.id_maintainance_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_currencyfx", t => t.id_currencyfx)
                .ForeignKey("items", t => t.id_item)
                .ForeignKey("item_asset_maintainance", t => t.id_maintainance, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_maintainance)
                .Index(t => t.id_item)
                .Index(t => t.id_currencyfx)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            AddColumn("app_branch", "area", c => c.Decimal(precision: 20, scale: 4));
            AddColumn("app_branch", "id_measurement", c => c.Int());
            AddColumn("item_asset", "id_branch", c => c.Int());
            CreateIndex("app_branch", "id_measurement");
            CreateIndex("item_asset", "id_branch");
            AddForeignKey("item_asset", "id_branch", "app_branch", "id_branch");
            AddForeignKey("app_branch", "id_measurement", "app_measurement", "id_measurement");
        }
        
        public override void Down()
        {
            DropForeignKey("item_asset_maintainance_detail", "id_user", "security_user");
            DropForeignKey("item_asset_maintainance_detail", "id_maintainance", "item_asset_maintainance");
            DropForeignKey("item_asset_maintainance_detail", "id_item", "items");
            DropForeignKey("item_asset_maintainance_detail", "id_currencyfx", "app_currencyfx");
            DropForeignKey("item_asset_maintainance_detail", "id_company", "app_company");
            DropForeignKey("item_asset_maintainance", "id_user", "security_user");
            DropForeignKey("item_asset_maintainance", "id_item_asset", "item_asset");
            DropForeignKey("item_asset_maintainance", "id_company", "app_company");
            DropForeignKey("app_branch_walkins", "id_user", "security_user");
            DropForeignKey("app_branch_walkins", "id_company", "app_company");
            DropForeignKey("app_branch_walkins", "id_branch", "app_branch");
            DropForeignKey("app_branch", "id_measurement", "app_measurement");
            DropForeignKey("item_asset", "id_branch", "app_branch");
            DropIndex("item_asset_maintainance_detail", new[] { "id_user" });
            DropIndex("item_asset_maintainance_detail", new[] { "id_company" });
            DropIndex("item_asset_maintainance_detail", new[] { "id_currencyfx" });
            DropIndex("item_asset_maintainance_detail", new[] { "id_item" });
            DropIndex("item_asset_maintainance_detail", new[] { "id_maintainance" });
            DropIndex("item_asset_maintainance", new[] { "id_user" });
            DropIndex("item_asset_maintainance", new[] { "id_company" });
            DropIndex("item_asset_maintainance", new[] { "id_item_asset" });
            DropIndex("app_branch_walkins", new[] { "id_user" });
            DropIndex("app_branch_walkins", new[] { "id_company" });
            DropIndex("app_branch_walkins", new[] { "id_branch" });
            DropIndex("item_asset", new[] { "id_branch" });
            DropIndex("app_branch", new[] { "id_measurement" });
            DropColumn("item_asset", "id_branch");
            DropColumn("app_branch", "id_measurement");
            DropColumn("app_branch", "area");
            DropTable("item_asset_maintainance_detail");
            DropTable("item_asset_maintainance");
            DropTable("app_branch_walkins");
        }
    }
}
