namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatesalesInvoice : DbMigration
    {
        public override void Up()
        {
            AddColumn("sales_invoice", "vatwithholdingpercentage", c => c.Decimal(nullable: false, precision: 20, scale: 4));
        }
        
        public override void Down()
        {
            DropColumn("sales_invoice", "vatwithholdingpercentage");
        }
    }
}
