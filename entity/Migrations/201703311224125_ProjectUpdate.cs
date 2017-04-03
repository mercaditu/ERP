namespace entity.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class ProjectUpdate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "security_request",
                c => new
                {
                    id_request = c.Int(nullable: false, identity: true),
                    id_application = c.Int(nullable: false),
                    state = c.Int(nullable: false),
                    type = c.Int(nullable: false),
                    value = c.Decimal(precision: 20, scale: 9),
                    request_date = c.DateTime(nullable: false, precision: 0),
                    approve_date = c.DateTime(nullable: false, precision: 0),
                    comment = c.String(unicode: false),
                    id_company = c.Int(nullable: false),
                    id_user = c.Int(nullable: false),
                    is_head = c.Boolean(nullable: false),
                    timestamp = c.DateTime(nullable: false, precision: 0),
                    is_read = c.Boolean(nullable: false),
                    approve_user_id_user = c.Int(),
                    request_user_id_user = c.Int(),
                    security_user_id_user = c.Int(),
                })
                .PrimaryKey(t => t.id_request)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.approve_user_id_user)
                .ForeignKey("security_user", t => t.request_user_id_user)
                .ForeignKey("security_user", t => t.security_user_id_user)
                .Index(t => t.id_company)
                .Index(t => t.approve_user_id_user)
                .Index(t => t.request_user_id_user)
                .Index(t => t.security_user_id_user);

            AddColumn("contacts", "method", c => c.Int());
            AddColumn("contacts", "geo_longlat", c => c.String(unicode: false));
            AddColumn("contacts", "code_verif", c => c.String(unicode: false));
            AddColumn("items", "is_archived", c => c.Boolean(nullable: false));
            AddColumn("item_product", "show_batch", c => c.Boolean(nullable: false));
            AddColumn("project_task", "importance", c => c.Decimal(nullable: false, precision: 20, scale: 9));
            AddColumn("project_task", "is_archived", c => c.Boolean(nullable: false));
            AddColumn("app_document_range", "is_archived", c => c.Boolean(nullable: false));
            AddColumn("purchase_invoice", "method", c => c.Int());
            AddColumn("impex_import", "is_archived", c => c.Boolean(nullable: false));
            AddColumn("impexes", "type", c => c.Int());
            AddColumn("impex_export", "is_archived", c => c.Boolean(nullable: false));
            AddColumn("sales_invoice", "method", c => c.Int());
            AddColumn("sales_budget", "method", c => c.Int());
            AddColumn("item_asset_maintainance", "is_archived", c => c.Boolean(nullable: false));
            AddColumn("production_order_detail", "is_archived", c => c.Boolean(nullable: false));
            AddColumn("production_order_detail", "importance", c => c.Decimal(nullable: false, precision: 20, scale: 9));
            AddColumn("purchase_invoice_detail", "id_purchase_packing_detail", c => c.Int());
            AddColumn("purchase_invoice_detail", "vat", c => c.Decimal(nullable: false, precision: 20, scale: 9));
            AddColumn("purchase_order", "method", c => c.Int());
            AddColumn("purchase_tender", "is_archived", c => c.Boolean(nullable: false));
            AddColumn("sales_return", "method", c => c.Int());
            AddColumn("sales_order", "method", c => c.Int());
            AddColumn("production_line", "is_active", c => c.Boolean(nullable: false));
            AddColumn("purchase_return", "method", c => c.Int());
            AlterColumn("purchase_packing_detail", "verified_by", c => c.Int());
            CreateIndex("purchase_invoice_detail", "id_purchase_packing_detail");
            AddForeignKey("purchase_invoice_detail", "id_purchase_packing_detail", "purchase_packing_detail", "id_purchase_packing_detail");
            DropColumn("production_order", "completed");
            DropColumn("purchase_packing_detail", "user_verified");
        }

        public override void Down()
        {
            AddColumn("purchase_packing_detail", "user_verified", c => c.Boolean(nullable: false));
            AddColumn("production_order", "completed", c => c.Decimal(nullable: false, precision: 20, scale: 9));
            DropForeignKey("security_request", "security_user_id_user", "security_user");
            DropForeignKey("security_request", "request_user_id_user", "security_user");
            DropForeignKey("security_request", "approve_user_id_user", "security_user");
            DropForeignKey("security_request", "id_company", "app_company");
            DropForeignKey("purchase_invoice_detail", "id_purchase_packing_detail", "purchase_packing_detail");
            DropIndex("security_request", new[] { "security_user_id_user" });
            DropIndex("security_request", new[] { "request_user_id_user" });
            DropIndex("security_request", new[] { "approve_user_id_user" });
            DropIndex("security_request", new[] { "id_company" });
            DropIndex("purchase_invoice_detail", new[] { "id_purchase_packing_detail" });
            AlterColumn("purchase_packing_detail", "verified_by", c => c.Decimal(precision: 20, scale: 9));
            DropColumn("purchase_return", "method");
            DropColumn("production_line", "is_active");
            DropColumn("sales_order", "method");
            DropColumn("sales_return", "method");
            DropColumn("purchase_tender", "is_archived");
            DropColumn("purchase_order", "method");
            DropColumn("purchase_invoice_detail", "vat");
            DropColumn("purchase_invoice_detail", "id_purchase_packing_detail");
            DropColumn("production_order_detail", "importance");
            DropColumn("production_order_detail", "is_archived");
            DropColumn("item_asset_maintainance", "is_archived");
            DropColumn("sales_budget", "method");
            DropColumn("sales_invoice", "method");
            DropColumn("impex_export", "is_archived");
            DropColumn("impexes", "type");
            DropColumn("impex_import", "is_archived");
            DropColumn("purchase_invoice", "method");
            DropColumn("app_document_range", "is_archived");
            DropColumn("project_task", "is_archived");
            DropColumn("project_task", "importance");
            DropColumn("item_product", "show_batch");
            DropColumn("items", "is_archived");
            DropColumn("contacts", "code_verif");
            DropColumn("contacts", "geo_longlat");
            DropColumn("contacts", "method");
            DropTable("security_request");
        }
    }
}