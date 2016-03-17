namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProductionOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("production_order_detail", "status", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("production_order_detail", "status");
        }
    }
}
