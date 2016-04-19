namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSalesPacking : DbMigration
    {
        public override void Up()
        {
            AlterColumn("sales_packing_detail", "id_location", c => c.Int());
            CreateIndex("sales_packing_detail", "id_location");
            AddForeignKey("sales_packing_detail", "id_location", "app_location", "id_location");
        }
        
        public override void Down()
        {
            DropForeignKey("sales_packing_detail", "id_location", "app_location");
            DropIndex("sales_packing_detail", new[] { "id_location" });
            AlterColumn("sales_packing_detail", "id_location", c => c.Int(nullable: false));
        }
    }
}
