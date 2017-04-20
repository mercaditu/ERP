namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContractPositionFix : DbMigration
    {
        public override void Up()
        {
            AddColumn("item_movement", "barcode", c => c.String(unicode: false));
            AddColumn("project_template_detail", "quantity", c => c.Decimal(nullable: false, precision: 20, scale: 9));
            AddColumn("hr_contract", "id_position", c => c.Int());
            CreateIndex("hr_contract", "id_position");
            AddForeignKey("hr_contract", "id_position", "hr_position", "id_position");
        }
        
        public override void Down()
        {
            DropForeignKey("hr_contract", "id_position", "hr_position");
            DropIndex("hr_contract", new[] { "id_position" });
            DropColumn("hr_contract", "id_position");
            DropColumn("project_template_detail", "quantity");
            DropColumn("item_movement", "barcode");
        }
    }
}
