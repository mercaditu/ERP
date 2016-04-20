namespace entity.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;
    
    public partial class UpdateSalesPacking : DbMigration
    {
        public override void Up()
        {
            db db = new entity.db();
            if (db.sales_packing.Count()>0)
            {
                List<sales_packing> sales_packing=db.sales_packing.ToList();
                db.sales_packing.RemoveRange(sales_packing);
                db.SaveChanges();
            }
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
