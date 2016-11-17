namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class receipeandpaymentdetailchanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("item_request", "given_user_id_user", c => c.Int());
            AddColumn("payments", "id_sales_rep", c => c.Int());
            CreateIndex("item_request", "given_user_id_user");
            CreateIndex("payments", "id_sales_rep");
            AddForeignKey("item_request", "given_user_id_user", "security_user", "id_user");
            AddForeignKey("payments", "id_sales_rep", "sales_rep", "id_sales_rep");
        }
        
        public override void Down()
        {
            DropForeignKey("payments", "id_sales_rep", "sales_rep");
            DropForeignKey("item_request", "given_user_id_user", "security_user");
            DropIndex("payments", new[] { "id_sales_rep" });
            DropIndex("item_request", new[] { "given_user_id_user" });
            DropColumn("payments", "id_sales_rep");
            DropColumn("item_request", "given_user_id_user");
        }
    }
}
