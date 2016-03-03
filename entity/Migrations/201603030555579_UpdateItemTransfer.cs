namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateItemTransfer : DbMigration
    {
        public override void Up()
        {
            AddColumn("item_transfer", "id_terminal", c => c.Int());
            CreateIndex("item_transfer", "id_terminal");
            AddForeignKey("item_transfer", "id_terminal", "app_terminal", "id_terminal");
        }
        
        public override void Down()
        {
            DropForeignKey("item_transfer", "id_terminal", "app_terminal");
            DropIndex("item_transfer", new[] { "id_terminal" });
            DropColumn("item_transfer", "id_terminal");
        }
    }
}
