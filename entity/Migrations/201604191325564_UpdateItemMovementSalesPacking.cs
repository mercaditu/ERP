namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateItemMovementSalesPacking : DbMigration
    {
        public override void Up()
        {
            AddColumn("item_movement", "id_sales_packing", c => c.Int());
            CreateIndex("item_movement", "id_sales_packing");
            AddForeignKey("item_movement", "id_sales_packing", "sales_packing", "id_sales_packing");
        }
        
        public override void Down()
        {
            DropForeignKey("item_movement", "id_sales_packing", "sales_packing");
            DropIndex("item_movement", new[] { "id_sales_packing" });
            DropColumn("item_movement", "id_sales_packing");
        }
    }
}
