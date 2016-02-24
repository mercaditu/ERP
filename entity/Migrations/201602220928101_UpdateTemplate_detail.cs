namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTemplate_detail : DbMigration
    {
        public override void Up()
        {
            AddColumn("accounting_template_detail", "coefficeint", c => c.Decimal(nullable: false, precision: 20, scale: 4));
        }
        
        public override void Down()
        {
            DropColumn("accounting_template_detail", "coefficeint");
        }
    }
}
