namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UdateAccounting : DbMigration
    {
        public override void Up()
        {
            AddColumn("payments", "is_accounted", c => c.Boolean(nullable: false));
            AddColumn("purchase_invoice", "is_accounted", c => c.Boolean(nullable: false));
            AddColumn("sales_invoice", "is_accounted", c => c.Boolean(nullable: false));
            AddColumn("sales_return", "is_accounted", c => c.Boolean(nullable: false));
            AddColumn("purchase_return", "is_accounted", c => c.Boolean(nullable: false));
            AddColumn("project_event", "status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("project_event", "status");
            DropColumn("purchase_return", "is_accounted");
            DropColumn("sales_return", "is_accounted");
            DropColumn("sales_invoice", "is_accounted");
            DropColumn("purchase_invoice", "is_accounted");
            DropColumn("payments", "is_accounted");
        }
    }
}
