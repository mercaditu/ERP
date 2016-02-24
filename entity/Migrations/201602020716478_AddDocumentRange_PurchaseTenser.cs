namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDocumentRange_PurchaseTenser : DbMigration
    {
        public override void Up()
        {
            AddColumn("purchase_tender", "id_range", c => c.Int());
            AddColumn("purchase_tender", "id_terminal", c => c.Int());
            AddColumn("purchase_packing", "number", c => c.String(unicode: false));
            DropColumn("purchase_packing", "packing_number");
        }
        
        public override void Down()
        {
            AddColumn("purchase_packing", "packing_number", c => c.String(unicode: false));
            DropColumn("purchase_packing", "number");
            DropColumn("purchase_tender", "id_terminal");
            DropColumn("purchase_tender", "id_range");
        }
    }
}
