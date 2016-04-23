namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProject_TaskwithPurchaseTender : DbMigration
    {
        public override void Up()
        {
            AddColumn("purchase_tender_detail", "status", c => c.Int(nullable: false));
            AddColumn("purchase_tender_item", "id_project_task", c => c.Int());
            CreateIndex("purchase_tender_item", "id_project_task");
            AddForeignKey("purchase_tender_item", "id_project_task", "project_task", "id_project_task");
        }
        
        public override void Down()
        {
            DropForeignKey("purchase_tender_item", "id_project_task", "project_task");
            DropIndex("purchase_tender_item", new[] { "id_project_task" });
            DropColumn("purchase_tender_item", "id_project_task");
            DropColumn("purchase_tender_detail", "status");
        }
    }
}
