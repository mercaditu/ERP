namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangesinPaymentDetail : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("payment_detail", "id_payment", "payments");
            DropIndex("payment_detail", new[] { "id_payment" });
            AlterColumn("payment_detail", "id_payment", c => c.Int());
            CreateIndex("payment_detail", "id_payment");
            AddForeignKey("payment_detail", "id_payment", "payments", "id_payment");
        }
        
        public override void Down()
        {
            DropForeignKey("payment_detail", "id_payment", "payments");
            DropIndex("payment_detail", new[] { "id_payment" });
            AlterColumn("payment_detail", "id_payment", c => c.Int(nullable: false));
            CreateIndex("payment_detail", "id_payment");
            AddForeignKey("payment_detail", "id_payment", "payments", "id_payment", cascadeDelete: true);
        }
    }
}
