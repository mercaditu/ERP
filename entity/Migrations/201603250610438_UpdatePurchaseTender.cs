namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePurchaseTender : DbMigration
    {
        public override void Up()
        {
            AddColumn("purchase_tender_detail", "id_vat_group", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("purchase_tender_detail", "id_vat_group");
        }
    }
}
