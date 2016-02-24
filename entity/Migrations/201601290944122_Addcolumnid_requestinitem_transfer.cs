namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addcolumnid_requestinitem_transfer : DbMigration
    {
        public override void Up()
        {
            AddColumn("item_transfer", "id_item_request", c => c.Int());
            CreateIndex("item_transfer", "id_item_request");
            AddForeignKey("item_transfer", "id_item_request", "item_request", "id_item_request");
        }
        
        public override void Down()
        {
            DropForeignKey("item_transfer", "id_item_request", "item_request");
            DropIndex("item_transfer", new[] { "id_item_request" });
            DropColumn("item_transfer", "id_item_request");
        }
    }
}
