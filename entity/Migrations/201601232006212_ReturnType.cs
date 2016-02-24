namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReturnType : DbMigration
    {
        public override void Up()
        {
            AddColumn("purchase_return", "return_type", c => c.Int(nullable: false));
            AddColumn("sales_return", "return_type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("sales_return", "return_type");
            DropColumn("purchase_return", "return_type");
        }
    }
}
