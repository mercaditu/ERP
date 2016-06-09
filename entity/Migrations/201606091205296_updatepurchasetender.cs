namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatepurchasetender : DbMigration
    {
        public override void Up()
        {
            CreateIndex("purchase_tender", "id_range");
            CreateIndex("purchase_tender", "id_terminal");
            AddForeignKey("purchase_tender", "id_range", "app_document_range", "id_range");
            AddForeignKey("purchase_tender", "id_terminal", "app_terminal", "id_terminal");
        }
        
        public override void Down()
        {
            DropForeignKey("purchase_tender", "id_terminal", "app_terminal");
            DropForeignKey("purchase_tender", "id_range", "app_document_range");
            DropIndex("purchase_tender", new[] { "id_terminal" });
            DropIndex("purchase_tender", new[] { "id_range" });
        }
    }
}
