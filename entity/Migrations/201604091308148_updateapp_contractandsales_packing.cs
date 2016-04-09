namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateapp_contractandsales_packing : DbMigration
    {
        public override void Up()
        {
            AddColumn("app_contract", "is_default", c => c.Boolean(nullable: false));
            AddColumn("sales_packing", "packing_type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("sales_packing", "packing_type");
            DropColumn("app_contract", "is_default");
        }
    }
}
