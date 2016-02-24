namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addbranchitem_transfer : DbMigration
    {
        public override void Up()
        {
            AddColumn("item_transfer", "id_branch", c => c.Int(nullable: false));
            AddColumn("item_transfer", "app_branch_id_branch", c => c.Int());
            CreateIndex("item_transfer", "app_branch_id_branch");
            AddForeignKey("item_transfer", "app_branch_id_branch", "app_branch", "id_branch");
        }
        
        public override void Down()
        {
            DropForeignKey("item_transfer", "app_branch_id_branch", "app_branch");
            DropIndex("item_transfer", new[] { "app_branch_id_branch" });
            DropColumn("item_transfer", "app_branch_id_branch");
            DropColumn("item_transfer", "id_branch");
        }
    }
}
