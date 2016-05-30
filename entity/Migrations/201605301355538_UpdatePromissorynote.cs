namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePromissorynote : DbMigration
    {
        public override void Up()
        {
            AddColumn("item_price_list", "percent_type", c => c.Int(nullable: false));
            AddColumn("item_price_list", "percent_over", c => c.Decimal(nullable: false, precision: 20, scale: 4));
            AddColumn("item_price_list", "ref_price_list_id_price_list", c => c.Int());
            AddColumn("payment_promissory_note", "id_range", c => c.Int());
            AddColumn("payment_promissory_note", "id_branch", c => c.Int(nullable: false));
            AddColumn("payment_promissory_note", "id_terminal", c => c.Int());
            CreateIndex("item_price_list", "ref_price_list_id_price_list");
            CreateIndex("payment_promissory_note", "id_range");
            CreateIndex("payment_promissory_note", "id_branch");
            CreateIndex("payment_promissory_note", "id_terminal");
            AddForeignKey("item_price_list", "ref_price_list_id_price_list", "item_price_list", "id_price_list");
            AddForeignKey("payment_promissory_note", "id_branch", "app_branch", "id_branch", cascadeDelete: true);
            AddForeignKey("payment_promissory_note", "id_range", "app_document_range", "id_range");
            AddForeignKey("payment_promissory_note", "id_terminal", "app_terminal", "id_terminal");
            DropColumn("item_price_list", "percentcost");
        }
        
        public override void Down()
        {
            AddColumn("item_price_list", "percentcost", c => c.Decimal(nullable: false, precision: 20, scale: 4));
            DropForeignKey("payment_promissory_note", "id_terminal", "app_terminal");
            DropForeignKey("payment_promissory_note", "id_range", "app_document_range");
            DropForeignKey("payment_promissory_note", "id_branch", "app_branch");
            DropForeignKey("item_price_list", "ref_price_list_id_price_list", "item_price_list");
            DropIndex("payment_promissory_note", new[] { "id_terminal" });
            DropIndex("payment_promissory_note", new[] { "id_branch" });
            DropIndex("payment_promissory_note", new[] { "id_range" });
            DropIndex("item_price_list", new[] { "ref_price_list_id_price_list" });
            DropColumn("payment_promissory_note", "id_terminal");
            DropColumn("payment_promissory_note", "id_branch");
            DropColumn("payment_promissory_note", "id_range");
            DropColumn("item_price_list", "ref_price_list_id_price_list");
            DropColumn("item_price_list", "percent_over");
            DropColumn("item_price_list", "percent_type");
        }
    }
}
