namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSecurityPrillage : DbMigration
    {
        public override void Up()
        {
            AlterColumn("security_privilage", "name", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("security_privilage", "name", c => c.String(unicode: false));
        }
    }
}
