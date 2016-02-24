namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatecurrencyfxEventcosting : DbMigration
    {
        public override void Up()
        {
            AddColumn("project_event", "id_currencyfx", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("project_event", "id_currencyfx");
        }
    }
}
