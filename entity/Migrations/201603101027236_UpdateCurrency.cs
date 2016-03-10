namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCurrency : DbMigration
    {
        public override void Up()
        {
            AddColumn("app_currency", "has_rounding", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("app_currency", "has_rounding");
        }
    }
}
