namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateContactwithvat : DbMigration
    {
        public override void Up()
        {
            AddColumn("contact_subscription", "id_vat_group", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("contact_subscription", "id_vat_group");
        }
    }
}
