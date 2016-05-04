namespace entity.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public partial class updateimpex : DbMigration
    {
        public override void Up()
        {
          
            AlterColumn("impexes", "status", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            AlterColumn("impexes", "status", c => c.String(unicode: false));
        }
    }
}
