namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_Maintanaince_Detail : DbMigration
    {
        public override void Up()
        {
            AddColumn("app_currencyfx", "type", c => c.Int(nullable: false));
            AddColumn("item_request_detail", "id_maintainance_detail", c => c.Int());
            AddColumn("item_price_list", "percentcost", c => c.Decimal(nullable: false, precision: 20, scale: 4));
            AddColumn("item_asset_maintainance_detail", "id_time_coefficient", c => c.Int());
            AddColumn("item_asset_maintainance_detail", "id_contact", c => c.Int());
            CreateIndex("item_request_detail", "id_maintainance_detail");
            CreateIndex("item_asset_maintainance_detail", "id_time_coefficient");
            CreateIndex("item_asset_maintainance_detail", "id_contact");
            AddForeignKey("item_asset_maintainance_detail", "id_contact", "contacts", "id_contact");
            AddForeignKey("item_asset_maintainance_detail", "id_time_coefficient", "hr_time_coefficient", "id_time_coefficient");
            AddForeignKey("item_request_detail", "id_maintainance_detail", "item_asset_maintainance_detail", "id_maintainance_detail");
        }
        
        public override void Down()
        {
            DropForeignKey("item_request_detail", "id_maintainance_detail", "item_asset_maintainance_detail");
            DropForeignKey("item_asset_maintainance_detail", "id_time_coefficient", "hr_time_coefficient");
            DropForeignKey("item_asset_maintainance_detail", "id_contact", "contacts");
            DropIndex("item_asset_maintainance_detail", new[] { "id_contact" });
            DropIndex("item_asset_maintainance_detail", new[] { "id_time_coefficient" });
            DropIndex("item_request_detail", new[] { "id_maintainance_detail" });
            DropColumn("item_asset_maintainance_detail", "id_contact");
            DropColumn("item_asset_maintainance_detail", "id_time_coefficient");
            DropColumn("item_price_list", "percentcost");
            DropColumn("item_request_detail", "id_maintainance_detail");
            DropColumn("app_currencyfx", "type");
        }
    }
}
