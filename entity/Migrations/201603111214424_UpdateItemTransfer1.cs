namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateItemTransfer1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("item_transfer", "employee_id_contact", c => c.Int());
            CreateIndex("item_transfer", "employee_id_contact");
            AddForeignKey("item_transfer", "employee_id_contact", "contacts", "id_contact");
        }
        
        public override void Down()
        {
            DropForeignKey("item_transfer", "employee_id_contact", "contacts");
            DropIndex("item_transfer", new[] { "employee_id_contact" });
            DropColumn("item_transfer", "employee_id_contact");
        }
    }
}
