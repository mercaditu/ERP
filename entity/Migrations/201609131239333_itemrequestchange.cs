namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class itemrequestchange : DbMigration
    {
        public override void Up()
        {
            AddColumn("security_role", "see_cost", c => c.Boolean(nullable: false));
            AddColumn("item_request", "id_range", c => c.Int());
            AddColumn("item_request", "number", c => c.String(unicode: false));
            AddColumn("hr_contract", "work_type", c => c.Int(nullable: false));
            CreateIndex("item_request", "id_range");
            AddForeignKey("item_request", "id_range", "app_document_range", "id_range");
        }
        
        public override void Down()
        {
            DropForeignKey("item_request", "id_range", "app_document_range");
            DropIndex("item_request", new[] { "id_range" });
            DropColumn("hr_contract", "work_type");
            DropColumn("item_request", "number");
            DropColumn("item_request", "id_range");
            DropColumn("security_role", "see_cost");
        }
    }
}
