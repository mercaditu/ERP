namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTransferdetailandrequestdetail : DbMigration
    {
        public override void Up()
        {
            AddColumn("production_execution_detail", "status", c => c.Int());
            AddColumn("item_transfer_detail", "movement_id", c => c.Int());
            AddColumn("item_request_decision", "movement_id", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("item_request_decision", "movement_id");
            DropColumn("item_transfer_detail", "movement_id");
            DropColumn("production_execution_detail", "status");
        }
    }
}
