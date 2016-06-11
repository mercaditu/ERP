namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Changesin_productionorder : DbMigration
    {
        public override void Up()
        {
            AddColumn("production_execution_detail", "movement_id", c => c.Int());
            AddColumn("production_order_detail", "movement_id", c => c.Int());
            AddColumn("sales_order_detail", "movement_id", c => c.Int());
            AddColumn("sales_invoice_detail", "movement_id", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("sales_invoice_detail", "movement_id");
            DropColumn("sales_order_detail", "movement_id");
            DropColumn("production_order_detail", "movement_id");
            DropColumn("production_execution_detail", "movement_id");
        }
    }
}
