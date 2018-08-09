namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updateproductionproject : DbMigration
    {
        public override void Up()
        {
            AddColumn("app_contract", "cloud_id", c => c.Int(nullable: false));
            AddColumn("project_task", "sequence", c => c.Int());
            AddColumn("app_vat_group", "cloud_id", c => c.Int(nullable: false));
            AddColumn("production_order_detail", "sequence", c => c.Int());
            AddColumn("sales_promotion", "cloud_id", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("sales_promotion", "cloud_id");
            DropColumn("production_order_detail", "sequence");
            DropColumn("app_vat_group", "cloud_id");
            DropColumn("project_task", "sequence");
            DropColumn("app_contract", "cloud_id");
        }
    }
}
