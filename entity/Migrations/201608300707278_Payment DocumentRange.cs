namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PaymentDocumentRange : DbMigration
    {
        public override void Up()
        {
            AddColumn("payment_schedual", "number", c => c.String(unicode: false));
            AlterColumn("app_document_range", "range_start", c => c.Int(nullable: false));
            AlterColumn("app_document_range", "range_current", c => c.Int(nullable: false));
            AlterColumn("app_document_range", "range_end", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("app_document_range", "range_end", c => c.Short(nullable: false));
            AlterColumn("app_document_range", "range_current", c => c.Short(nullable: false));
            AlterColumn("app_document_range", "range_start", c => c.Short(nullable: false));
            DropColumn("payment_schedual", "number");
        }
    }
}
