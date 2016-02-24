namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSalesPackingRelation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("sales_packing_relation", "id_sales_invoice", "sales_invoice");
            DropIndex("sales_packing_relation", new[] { "id_sales_invoice" });
            AddColumn("purchase_tender", "number", c => c.String(unicode: false));
            AddColumn("sales_packing_relation", "id_sales_invoice_detail", c => c.Long(nullable: false));
            CreateIndex("sales_packing_relation", "id_sales_invoice_detail");
            AddForeignKey("sales_packing_relation", "id_sales_invoice_detail", "sales_invoice_detail", "id_sales_invoice_detail", cascadeDelete: true);
            DropColumn("sales_packing_relation", "id_sales_invoice");
        }
        
        public override void Down()
        {
            AddColumn("sales_packing_relation", "id_sales_invoice", c => c.Int(nullable: false));
            DropForeignKey("sales_packing_relation", "id_sales_invoice_detail", "sales_invoice_detail");
            DropIndex("sales_packing_relation", new[] { "id_sales_invoice_detail" });
            DropColumn("sales_packing_relation", "id_sales_invoice_detail");
            DropColumn("purchase_tender", "number");
            CreateIndex("sales_packing_relation", "id_sales_invoice");
            AddForeignKey("sales_packing_relation", "id_sales_invoice", "sales_invoice", "id_sales_invoice", cascadeDelete: true);
        }
    }
}
