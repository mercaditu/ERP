namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePayment : DbMigration
    {
        public override void Up()
        {
            AddColumn("project_template_detail", "status", c => c.Int());
            AddColumn("payment_detail", "id_range", c => c.Int());
            AddColumn("payment_detail", "payment_type_number", c => c.String(unicode: false));
            AddColumn("payment_detail", "app_bank_id_bank", c => c.Int());
            AddColumn("payments", "id_range", c => c.Int());
            AddColumn("payments", "number", c => c.String(unicode: false));
            AddColumn("payments", "id_branch", c => c.Int());
            AddColumn("payments", "id_terminal", c => c.Int());
            AddColumn("payment_type", "id_document", c => c.Int());
            AddColumn("payment_type", "has_bank", c => c.Boolean(nullable: false));
            CreateIndex("payment_type", "id_document");
            CreateIndex("payment_detail", "id_range");
            CreateIndex("payment_detail", "app_bank_id_bank");
            CreateIndex("payments", "id_range");
            CreateIndex("payments", "id_branch");
            CreateIndex("payments", "id_terminal");
            AddForeignKey("payment_type", "id_document", "app_document", "id_document");
            AddForeignKey("payment_detail", "app_bank_id_bank", "app_bank", "id_bank");
            AddForeignKey("payment_detail", "id_range", "app_document_range", "id_range");
            AddForeignKey("payments", "id_branch", "app_branch", "id_branch");
            AddForeignKey("payments", "id_range", "app_document_range", "id_range");
            AddForeignKey("payments", "id_terminal", "app_terminal", "id_terminal");
            DropColumn("payments", "payment_number");
        }
        
        public override void Down()
        {
            AddColumn("payments", "payment_number", c => c.String(unicode: false));
            DropForeignKey("payments", "id_terminal", "app_terminal");
            DropForeignKey("payments", "id_range", "app_document_range");
            DropForeignKey("payments", "id_branch", "app_branch");
            DropForeignKey("payment_detail", "id_range", "app_document_range");
            DropForeignKey("payment_detail", "app_bank_id_bank", "app_bank");
            DropForeignKey("payment_type", "id_document", "app_document");
            DropIndex("payments", new[] { "id_terminal" });
            DropIndex("payments", new[] { "id_branch" });
            DropIndex("payments", new[] { "id_range" });
            DropIndex("payment_detail", new[] { "app_bank_id_bank" });
            DropIndex("payment_detail", new[] { "id_range" });
            DropIndex("payment_type", new[] { "id_document" });
            DropColumn("payment_type", "has_bank");
            DropColumn("payment_type", "id_document");
            DropColumn("payments", "id_terminal");
            DropColumn("payments", "id_branch");
            DropColumn("payments", "number");
            DropColumn("payments", "id_range");
            DropColumn("payment_detail", "app_bank_id_bank");
            DropColumn("payment_detail", "payment_type_number");
            DropColumn("payment_detail", "id_range");
            DropColumn("project_template_detail", "status");
        }
    }
}
