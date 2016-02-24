namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateItemDimension : DbMigration
    {
        public override void Up()
        {
            CreateIndex("item_dimension", "id_measurement");
            AddForeignKey("item_dimension", "id_measurement", "app_measurement", "id_measurement", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("item_dimension", "id_measurement", "app_measurement");
            DropIndex("item_dimension", new[] { "id_measurement" });
        }
    }
}
