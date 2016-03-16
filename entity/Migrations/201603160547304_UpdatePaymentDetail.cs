namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePaymentDetail : DbMigration
    {
        public override void Up()
        {
            AddColumn("payment_detail", "comment", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("payment_detail", "comment");
        }
    }
}
