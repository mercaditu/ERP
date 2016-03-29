namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommericalType_ProjectCurrency : DbMigration
    {
        public override void Up()
        {
            AddColumn("projects", "id_currency", c => c.Int());
            AddColumn("purchase_invoice", "trans_type", c => c.Int(nullable: false));
            AddColumn("sales_invoice", "trans_type", c => c.Int(nullable: false));
            AddColumn("sales_budget", "trans_type", c => c.Int(nullable: false));
            AddColumn("sales_order", "trans_type", c => c.Int(nullable: false));
            AddColumn("sales_return", "trans_type", c => c.Int(nullable: false));
            AddColumn("purchase_order", "trans_type", c => c.Int(nullable: false));
            AddColumn("purchase_return", "trans_type", c => c.Int(nullable: false));
            CreateIndex("projects", "id_currency");
            AddForeignKey("projects", "id_currency", "app_currency", "id_currency");
        }
        
        public override void Down()
        {
            DropForeignKey("projects", "id_currency", "app_currency");
            DropIndex("projects", new[] { "id_currency" });
            DropColumn("purchase_return", "trans_type");
            DropColumn("purchase_order", "trans_type");
            DropColumn("sales_return", "trans_type");
            DropColumn("sales_order", "trans_type");
            DropColumn("sales_budget", "trans_type");
            DropColumn("sales_invoice", "trans_type");
            DropColumn("purchase_invoice", "trans_type");
            DropColumn("projects", "id_currency");
        }
    }
}
