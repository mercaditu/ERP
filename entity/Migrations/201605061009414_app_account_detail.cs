namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class app_account_detail : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "app_account_session",
                c => new
                    {
                        id_session = c.Int(nullable: false, identity: true),
                        id_payment_detail = c.Int(),
                        op_date = c.DateTime(nullable: false, precision: 0),
                        cl_date = c.DateTime(nullable: false, precision: 0),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.id_session)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            AddColumn("app_account_detail", "id_session", c => c.Int());
            AddColumn("app_account_detail", "tran_type", c => c.Int());
            CreateIndex("app_account_detail", "id_session");
            AddForeignKey("app_account_detail", "id_session", "app_account_session", "id_session");
        }
        
        public override void Down()
        {
            DropForeignKey("app_account_session", "id_user", "security_user");
            DropForeignKey("app_account_session", "id_company", "app_company");
            DropForeignKey("app_account_detail", "id_session", "app_account_session");
            DropIndex("app_account_session", new[] { "id_user" });
            DropIndex("app_account_session", new[] { "id_company" });
            DropIndex("app_account_detail", new[] { "id_session" });
            DropColumn("app_account_detail", "tran_type");
            DropColumn("app_account_detail", "id_session");
            DropTable("app_account_session");
        }
    }
}
