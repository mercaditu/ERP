namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSalesPackingRelationV2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("sales_packing_relation", "id_sales_packing", "sales_packing");
            DropIndex("sales_packing_relation", new[] { "id_sales_packing" });
            AddColumn("sales_packing_relation", "id_sales_packing_detail", c => c.Int(nullable: false));
            CreateIndex("sales_packing_relation", "id_sales_packing_detail");
            AddForeignKey("sales_packing_relation", "id_sales_packing_detail", "sales_packing_detail", "id_sales_packing_detail", cascadeDelete: true);
            DropColumn("sales_packing_relation", "id_sales_packing");
        }
        
        public override void Down()
        {
            AddColumn("sales_packing_relation", "id_sales_packing", c => c.Int(nullable: false));
            DropForeignKey("sales_packing_relation", "id_sales_packing_detail", "sales_packing_detail");
            DropIndex("sales_packing_relation", new[] { "id_sales_packing_detail" });
            DropColumn("sales_packing_relation", "id_sales_packing_detail");
            CreateIndex("sales_packing_relation", "id_sales_packing");
            AddForeignKey("sales_packing_relation", "id_sales_packing", "sales_packing", "id_sales_packing", cascadeDelete: true);
        }
    }
}
