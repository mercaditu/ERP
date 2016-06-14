namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class promissorynoteUpdate : DbMigration
    {
        public override void Up()
        {
            CreateIndex("payment_promissory_note", "id_currencyfx");
            AddForeignKey("payment_promissory_note", "id_currencyfx", "app_currencyfx", "id_currencyfx", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("payment_promissory_note", "id_currencyfx", "app_currencyfx");
            DropIndex("payment_promissory_note", new[] { "id_currencyfx" });
        }
    }
}
