namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePackingList : DbMigration
    {
        public override void Up()
        {
            AddColumn("sales_packing", "number", c => c.String(unicode: false));
            DropColumn("sales_packing", "packing_number");
        }
        
        public override void Down()
        {
            AddColumn("sales_packing", "packing_number", c => c.String(unicode: false));
            DropColumn("sales_packing", "number");
        }
    }
}
