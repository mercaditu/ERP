namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateContacts : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("purchase_packing_detail", "id_location", "app_location");
            DropIndex("purchase_packing_detail", new[] { "id_location" });
            AddColumn("contacts", "comment", c => c.String(unicode: false));
            AlterColumn("purchase_packing_detail", "id_location", c => c.Int());
            CreateIndex("purchase_packing_detail", "id_location");
            AddForeignKey("purchase_packing_detail", "id_location", "app_location", "id_location");
        }
        
        public override void Down()
        {
            DropForeignKey("purchase_packing_detail", "id_location", "app_location");
            DropIndex("purchase_packing_detail", new[] { "id_location" });
            AlterColumn("purchase_packing_detail", "id_location", c => c.Int(nullable: false));
            DropColumn("contacts", "comment");
            CreateIndex("purchase_packing_detail", "id_location");
            AddForeignKey("purchase_packing_detail", "id_location", "app_location", "id_location", cascadeDelete: true);
        }
    }
}
