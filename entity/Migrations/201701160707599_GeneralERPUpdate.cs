namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GeneralERPUpdate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("production_account", "id_execution_detail", "production_execution_detail", "id_execution_detail");
            DropForeignKey("production_account", "id_execution_detail", "production_execution_detail");
            DropIndex("production_account", new[] { "id_execution_detail" });
            CreateTable(
                "payment_approve_detail",
                c => new
                    {
                        id_payment_approve_detail = c.Int(nullable: false, identity: true),
                        id_bank = c.Int(),
                        id_payment_approve = c.Int(),
                        id_sales_return = c.Int(),
                        id_purchase_return = c.Int(),
                        id_account = c.Int(),
                        id_currency = c.Int(nullable: false),
                        id_payment_type = c.Int(nullable: false),
                        payment_type_ref = c.Short(),
                        value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        id_range = c.Int(),
                        payment_type_number = c.String(unicode: false),
                        comment = c.String(unicode: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_payment_approve_detail)                
                .ForeignKey("app_account", t => t.id_account)
                .ForeignKey("app_bank", t => t.id_bank)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_currency", t => t.id_currency, cascadeDelete: true)
                .ForeignKey("app_document_range", t => t.id_range)
                .ForeignKey("payment_approve", t => t.id_payment_approve)
                .ForeignKey("payment_type", t => t.id_payment_type, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_bank)
                .Index(t => t.id_payment_approve)
                .Index(t => t.id_account)
                .Index(t => t.id_currency)
                .Index(t => t.id_payment_type)
                .Index(t => t.id_range)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "payment_approve",
                c => new
                    {
                        id_payment_approve = c.Int(nullable: false, identity: true),
                        id_contact = c.Int(),
                        status = c.Int(nullable: false),
                        id_range = c.Int(),
                        number = c.String(unicode: false),
                        id_branch = c.Int(),
                        id_terminal = c.Int(),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_payment_approve)                
                .ForeignKey("app_branch", t => t.id_branch)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_document_range", t => t.id_range)
                .ForeignKey("app_terminal", t => t.id_terminal)
                .ForeignKey("contacts", t => t.id_contact)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_contact)
                .Index(t => t.id_range)
                .Index(t => t.id_branch)
                .Index(t => t.id_terminal)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "crm_schedual",
                c => new
                    {
                        id_schedual = c.Int(nullable: false, identity: true),
                        id_sales_rep = c.Int(),
                        id_opportunity = c.Int(),
                        id_contact = c.Int(),
                        type = c.Int(nullable: false),
                        ref_id = c.Int(nullable: false),
                        start_date = c.DateTime(nullable: false, precision: 0),
                        end_date = c.DateTime(nullable: false, precision: 0),
                        comment = c.String(unicode: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_schedual)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("contacts", t => t.id_contact)
                .ForeignKey("crm_opportunity", t => t.id_opportunity)
                .ForeignKey("sales_rep", t => t.id_sales_rep)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_sales_rep)
                .Index(t => t.id_opportunity)
                .Index(t => t.id_contact)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "loyalty_member",
                c => new
                    {
                        id_member = c.Int(nullable: false, identity: true),
                        id_tier = c.Int(nullable: false),
                        id_contact = c.Int(),
                        number = c.String(nullable: false, unicode: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_member)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("contacts", t => t.id_contact)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .ForeignKey("loyalty_tier", t => t.id_tier, cascadeDelete: true)
                .Index(t => t.id_tier)
                .Index(t => t.id_contact)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "loyalty_member_detail",
                c => new
                    {
                        id_member_detail = c.Int(nullable: false, identity: true),
                        id_member = c.Int(nullable: false),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        expire_date = c.DateTime(nullable: false, precision: 0),
                        debit = c.Decimal(nullable: false, precision: 20, scale: 9),
                        credit = c.Decimal(nullable: false, precision: 20, scale: 9),
                        comment = c.String(unicode: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_member_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("loyalty_member", t => t.id_member, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_member)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "loyalty_tier",
                c => new
                    {
                        id_tier = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, unicode: false),
                        min_value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_tier)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            AddColumn("app_account_detail", "id_payment_approve_detail", c => c.Int());
            AddColumn("app_bank", "for_supplier", c => c.Boolean(nullable: false));
            AddColumn("app_bank", "branch_name", c => c.String(unicode: false));
            AddColumn("app_bank", "branch_address", c => c.String(unicode: false));
            AddColumn("app_bank", "country", c => c.String(unicode: false));
            AddColumn("app_bank", "swift_code", c => c.String(unicode: false));
            AddColumn("app_bank", "intermediary_bank", c => c.String(unicode: false));
            AddColumn("app_contract", "is_purchase", c => c.Boolean(nullable: false));
            AddColumn("app_contract", "is_sales", c => c.Boolean(nullable: false));
            AddColumn("production_execution_detail", "id_production_account", c => c.Int());
            AddColumn("production_execution_detail", "production_account_id_production_account", c => c.Int());
            AddColumn("production_execution_detail", "production_account_id_production_account1", c => c.Int());
            AddColumn("production_account", "Oldproduction_execution_detail_id_execution_detail", c => c.Int());
            AddColumn("payment_type_detail", "id_payment_approve_detail", c => c.Int(nullable: false));
            AddColumn("payment_schedual", "id_payment_approve_detail", c => c.Int());
            AddColumn("sales_rep", "monthly_goal", c => c.Decimal(nullable: false, precision: 20, scale: 9));
            AddColumn("sales_budget_detail", "id_sales_promotion", c => c.Int());
            AddColumn("sales_order_detail", "id_sales_promotion", c => c.Int());
            AddColumn("sales_invoice_detail", "id_sales_promotion", c => c.Int());
            AddColumn("sales_packing_detail", "id_movement", c => c.Long());
            AddColumn("sales_return_detail", "id_sales_promotion", c => c.Int());
            AddColumn("item_transfer_detail", "status_dest", c => c.Int(nullable: false));
            AddColumn("impexes", "est_shipping_date", c => c.DateTime(precision: 0));
            AddColumn("impexes", "real_shipping_date", c => c.DateTime(precision: 0));
            AddColumn("impexes", "est_landed_date", c => c.DateTime(precision: 0));
            AddColumn("impexes", "real_landed_date", c => c.DateTime(precision: 0));
            AddColumn("impexes", "real_arrival_date", c => c.DateTime(precision: 0));
            AddColumn("impex_incoterm_condition", "type", c => c.Int(nullable: false));
            AddColumn("purchase_packing_detail", "expire_date", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("purchase_packing_detail", "batch_code", c => c.String(unicode: false));
            CreateIndex("app_account_detail", "id_payment_approve_detail");
            CreateIndex("production_execution_detail", "production_account_id_production_account");
            CreateIndex("production_execution_detail", "production_account_id_production_account1");
            CreateIndex("production_account", "Oldproduction_execution_detail_id_execution_detail");
            CreateIndex("payment_type_detail", "id_payment_approve_detail");
            CreateIndex("payment_schedual", "id_payment_approve_detail");
            CreateIndex("sales_budget_detail", "id_sales_promotion");
            CreateIndex("sales_invoice_detail", "id_sales_promotion");
            CreateIndex("sales_order_detail", "id_sales_promotion");
            CreateIndex("sales_return_detail", "id_sales_promotion");
            AddForeignKey("production_account", "Oldproduction_execution_detail_id_execution_detail", "production_execution_detail", "id_execution_detail");
            AddForeignKey("app_account_detail", "id_payment_approve_detail", "payment_approve_detail", "id_payment_approve_detail");
            AddForeignKey("payment_schedual", "id_payment_approve_detail", "payment_approve_detail", "id_payment_approve_detail");
            AddForeignKey("sales_order_detail", "id_sales_promotion", "sales_promotion", "id_sales_promotion");
            AddForeignKey("sales_invoice_detail", "id_sales_promotion", "sales_promotion", "id_sales_promotion");
            AddForeignKey("sales_return_detail", "id_sales_promotion", "sales_promotion", "id_sales_promotion");
            AddForeignKey("sales_budget_detail", "id_sales_promotion", "sales_promotion", "id_sales_promotion");
            AddForeignKey("payment_type_detail", "id_payment_approve_detail", "payment_approve_detail", "id_payment_approve_detail", cascadeDelete: true);
            AddForeignKey("production_execution_detail", "production_account_id_production_account1", "production_account", "id_production_account");
            AddForeignKey("production_execution_detail", "production_account_id_production_account", "production_account", "id_production_account");
        }
        
        public override void Down()
        {
            DropForeignKey("production_execution_detail", "production_account_id_production_account", "production_account");
            DropForeignKey("production_execution_detail", "production_account_id_production_account1", "production_account");
            DropForeignKey("loyalty_tier", "id_user", "security_user");
            DropForeignKey("loyalty_member", "id_tier", "loyalty_tier");
            DropForeignKey("loyalty_tier", "id_company", "app_company");
            DropForeignKey("loyalty_member", "id_user", "security_user");
            DropForeignKey("loyalty_member_detail", "id_user", "security_user");
            DropForeignKey("loyalty_member_detail", "id_member", "loyalty_member");
            DropForeignKey("loyalty_member_detail", "id_company", "app_company");
            DropForeignKey("loyalty_member", "id_contact", "contacts");
            DropForeignKey("loyalty_member", "id_company", "app_company");
            DropForeignKey("crm_schedual", "id_user", "security_user");
            DropForeignKey("crm_schedual", "id_sales_rep", "sales_rep");
            DropForeignKey("crm_schedual", "id_opportunity", "crm_opportunity");
            DropForeignKey("crm_schedual", "id_contact", "contacts");
            DropForeignKey("crm_schedual", "id_company", "app_company");
            DropForeignKey("payment_approve_detail", "id_user", "security_user");
            DropForeignKey("payment_type_detail", "id_payment_approve_detail", "payment_approve_detail");
            DropForeignKey("payment_approve_detail", "id_payment_type", "payment_type");
            DropForeignKey("sales_budget_detail", "id_sales_promotion", "sales_promotion");
            DropForeignKey("sales_return_detail", "id_sales_promotion", "sales_promotion");
            DropForeignKey("sales_invoice_detail", "id_sales_promotion", "sales_promotion");
            DropForeignKey("sales_order_detail", "id_sales_promotion", "sales_promotion");
            DropForeignKey("payment_schedual", "id_payment_approve_detail", "payment_approve_detail");
            DropForeignKey("payment_approve", "id_user", "security_user");
            DropForeignKey("payment_approve_detail", "id_payment_approve", "payment_approve");
            DropForeignKey("payment_approve", "id_contact", "contacts");
            DropForeignKey("payment_approve", "id_terminal", "app_terminal");
            DropForeignKey("payment_approve", "id_range", "app_document_range");
            DropForeignKey("payment_approve", "id_company", "app_company");
            DropForeignKey("payment_approve", "id_branch", "app_branch");
            DropForeignKey("payment_approve_detail", "id_range", "app_document_range");
            DropForeignKey("payment_approve_detail", "id_currency", "app_currency");
            DropForeignKey("payment_approve_detail", "id_company", "app_company");
            DropForeignKey("payment_approve_detail", "id_bank", "app_bank");
            DropForeignKey("app_account_detail", "id_payment_approve_detail", "payment_approve_detail");
            DropForeignKey("payment_approve_detail", "id_account", "app_account");
            DropForeignKey("production_account", "Oldproduction_execution_detail_id_execution_detail", "production_execution_detail");
            DropIndex("loyalty_tier", new[] { "id_user" });
            DropIndex("loyalty_tier", new[] { "id_company" });
            DropIndex("loyalty_member_detail", new[] { "id_user" });
            DropIndex("loyalty_member_detail", new[] { "id_company" });
            DropIndex("loyalty_member_detail", new[] { "id_member" });
            DropIndex("loyalty_member", new[] { "id_user" });
            DropIndex("loyalty_member", new[] { "id_company" });
            DropIndex("loyalty_member", new[] { "id_contact" });
            DropIndex("loyalty_member", new[] { "id_tier" });
            DropIndex("crm_schedual", new[] { "id_user" });
            DropIndex("crm_schedual", new[] { "id_company" });
            DropIndex("crm_schedual", new[] { "id_contact" });
            DropIndex("crm_schedual", new[] { "id_opportunity" });
            DropIndex("crm_schedual", new[] { "id_sales_rep" });
            DropIndex("sales_return_detail", new[] { "id_sales_promotion" });
            DropIndex("sales_order_detail", new[] { "id_sales_promotion" });
            DropIndex("sales_invoice_detail", new[] { "id_sales_promotion" });
            DropIndex("sales_budget_detail", new[] { "id_sales_promotion" });
            DropIndex("payment_schedual", new[] { "id_payment_approve_detail" });
            DropIndex("payment_approve", new[] { "id_user" });
            DropIndex("payment_approve", new[] { "id_company" });
            DropIndex("payment_approve", new[] { "id_terminal" });
            DropIndex("payment_approve", new[] { "id_branch" });
            DropIndex("payment_approve", new[] { "id_range" });
            DropIndex("payment_approve", new[] { "id_contact" });
            DropIndex("payment_approve_detail", new[] { "id_user" });
            DropIndex("payment_approve_detail", new[] { "id_company" });
            DropIndex("payment_approve_detail", new[] { "id_range" });
            DropIndex("payment_approve_detail", new[] { "id_payment_type" });
            DropIndex("payment_approve_detail", new[] { "id_currency" });
            DropIndex("payment_approve_detail", new[] { "id_account" });
            DropIndex("payment_approve_detail", new[] { "id_payment_approve" });
            DropIndex("payment_approve_detail", new[] { "id_bank" });
            DropIndex("payment_type_detail", new[] { "id_payment_approve_detail" });
            DropIndex("production_account", new[] { "Oldproduction_execution_detail_id_execution_detail" });
            DropIndex("production_execution_detail", new[] { "production_account_id_production_account1" });
            DropIndex("production_execution_detail", new[] { "production_account_id_production_account" });
            DropIndex("app_account_detail", new[] { "id_payment_approve_detail" });
            DropColumn("purchase_packing_detail", "batch_code");
            DropColumn("purchase_packing_detail", "expire_date");
            DropColumn("impex_incoterm_condition", "type");
            DropColumn("impexes", "real_arrival_date");
            DropColumn("impexes", "real_landed_date");
            DropColumn("impexes", "est_landed_date");
            DropColumn("impexes", "real_shipping_date");
            DropColumn("impexes", "est_shipping_date");
            DropColumn("item_transfer_detail", "status_dest");
            DropColumn("sales_return_detail", "id_sales_promotion");
            DropColumn("sales_packing_detail", "id_movement");
            DropColumn("sales_invoice_detail", "id_sales_promotion");
            DropColumn("sales_order_detail", "id_sales_promotion");
            DropColumn("sales_budget_detail", "id_sales_promotion");
            DropColumn("sales_rep", "monthly_goal");
            DropColumn("payment_schedual", "id_payment_approve_detail");
            DropColumn("payment_type_detail", "id_payment_approve_detail");
            DropColumn("production_account", "Oldproduction_execution_detail_id_execution_detail");
            DropColumn("production_execution_detail", "production_account_id_production_account1");
            DropColumn("production_execution_detail", "production_account_id_production_account");
            DropColumn("production_execution_detail", "id_production_account");
            DropColumn("app_contract", "is_sales");
            DropColumn("app_contract", "is_purchase");
            DropColumn("app_bank", "intermediary_bank");
            DropColumn("app_bank", "swift_code");
            DropColumn("app_bank", "country");
            DropColumn("app_bank", "branch_address");
            DropColumn("app_bank", "branch_name");
            DropColumn("app_bank", "for_supplier");
            DropColumn("app_account_detail", "id_payment_approve_detail");
            DropTable("loyalty_tier");
            DropTable("loyalty_member_detail");
            DropTable("loyalty_member");
            DropTable("crm_schedual");
            DropTable("payment_approve");
            DropTable("payment_approve_detail");
            CreateIndex("production_account", "id_execution_detail");
            AddForeignKey("production_account", "id_execution_detail", "production_execution_detail", "id_execution_detail");
        }
    }
}
