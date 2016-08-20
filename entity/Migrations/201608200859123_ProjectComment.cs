namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectComment : DbMigration
    {
        public override void Up()
        {
            AddColumn("app_company", "hash_debehaber", c => c.String(unicode: false));
            AddColumn("app_company", "version", c => c.String(unicode: false));
            AddColumn("app_company", "seats", c => c.String(unicode: false));
            AddColumn("projects", "comment", c => c.String(unicode: false));
            AddColumn("impex_expense", "id_currency", c => c.Int());
            AddColumn("impex_expense", "currency_rate", c => c.Decimal(nullable: false, precision: 20, scale: 4));
            CreateIndex("impex_expense", "id_currency");
            AddForeignKey("impex_expense", "id_currency", "app_currency", "id_currency");
        }
        
        public override void Down()
        {
            DropForeignKey("impex_expense", "id_currency", "app_currency");
            DropIndex("impex_expense", new[] { "id_currency" });
            DropColumn("impex_expense", "currency_rate");
            DropColumn("impex_expense", "id_currency");
            DropColumn("projects", "comment");
            DropColumn("app_company", "seats");
            DropColumn("app_company", "version");
            DropColumn("app_company", "hash_debehaber");
        }
    }
}
