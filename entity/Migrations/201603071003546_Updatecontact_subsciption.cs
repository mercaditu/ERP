namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updatecontact_subsciption : DbMigration
    {
        public override void Up()
        {
            AlterColumn("contact_subscription", "id_item", c => c.Int(nullable: false));
            CreateIndex("contact_subscription", "id_item");
            AddForeignKey("contact_subscription", "id_item", "items", "id_item", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("contact_subscription", "id_item", "items");
            DropIndex("contact_subscription", new[] { "id_item" });
            AlterColumn("contact_subscription", "id_item", c => c.String(unicode: false));
        }
    }
}
