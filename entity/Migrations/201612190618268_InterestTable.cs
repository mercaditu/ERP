namespace entity.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class InterestTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("app_attachment", "id_company", "app_company");
            DropForeignKey("app_attachment", "id_user", "security_user");
            DropIndex("app_attachment", new[] { "id_company" });
            DropIndex("app_attachment", new[] { "id_user" });
            CreateTable(
                "app_company_interest",
                c => new
                    {
                        id_interest = c.Int(nullable: false),
                        grace_period = c.Int(nullable: false),
                        interest = c.Decimal(nullable: false, precision: 20, scale: 9),
                        is_forced = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_interest)                
                .ForeignKey("app_company", t => t.id_interest)
                .Index(t => t.id_interest);
            
            AddColumn("app_contract", "surcharge", c => c.Decimal(precision: 20, scale: 9));
            AddColumn("app_vat_group_details", "percentage", c => c.Decimal(nullable: false, precision: 20, scale: 9));
            AddColumn("app_attachment", "reference_id", c => c.Int(nullable: false));
            AddColumn("app_attachment", "is_default", c => c.Boolean(nullable: false));
            AddColumn("app_attachment", "application", c => c.Int(nullable: false));
            DropColumn("app_attachment", "is_active");
            DropColumn("app_attachment", "id_company");
            DropColumn("app_attachment", "id_user");
            DropColumn("app_attachment", "is_head");
            DropColumn("app_attachment", "timestamp");
            DropColumn("app_attachment", "is_read");
        }
        
        public override void Down()
        {
            AddColumn("app_attachment", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_attachment", "timestamp", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("app_attachment", "is_head", c => c.Boolean(nullable: false));
            AddColumn("app_attachment", "id_user", c => c.Int(nullable: false));
            AddColumn("app_attachment", "id_company", c => c.Int(nullable: false));
            AddColumn("app_attachment", "is_active", c => c.Boolean(nullable: false));
            DropForeignKey("app_company_interest", "id_interest", "app_company");
            DropIndex("app_company_interest", new[] { "id_interest" });
            DropColumn("app_attachment", "application");
            DropColumn("app_attachment", "is_default");
            DropColumn("app_attachment", "reference_id");
            DropColumn("app_vat_group_details", "percentage");
            DropColumn("app_contract", "surcharge");
            DropTable("app_company_interest");
            CreateIndex("app_attachment", "id_user");
            CreateIndex("app_attachment", "id_company");
            AddForeignKey("app_attachment", "id_user", "security_user", "id_user", cascadeDelete: true);
            AddForeignKey("app_attachment", "id_company", "app_company", "id_company", cascadeDelete: true);
        }
    }
}
