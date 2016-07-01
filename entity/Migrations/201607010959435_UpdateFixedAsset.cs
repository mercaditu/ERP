namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateFixedAsset : DbMigration
    {
        public override void Up()
        {
            AddColumn("item_asset", "id_department", c => c.Int());
            AddColumn("item_asset", "id_contact", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("item_asset", "id_contact");
            DropColumn("item_asset", "id_department");
        }
    }
}
