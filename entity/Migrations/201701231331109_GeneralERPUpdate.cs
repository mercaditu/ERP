namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GeneralERPUpdate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "payment_approve_detail",
                c => new
                    {
                        id_payment_approve_detail = c.Int(nullable: false),
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
                .ForeignKey("payment_schedual", t => t.id_payment_approve_detail)
                .ForeignKey("payment_type", t => t.id_payment_type, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_payment_approve_detail)
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
                "production_service_account",
                c => new
                    {
                        id_production_service_account = c.Int(nullable: false, identity: true),
                        id_contact = c.Int(),
                        id_item = c.Int(nullable: false),
                        id_order_detail = c.Int(),
                        id_purchase_order_detail = c.Int(),
                        id_purchase_invoice_detail = c.Int(),
                        status = c.Int(),
                        unit_cost = c.Decimal(nullable: false, precision: 20, scale: 9),
                        debit = c.Decimal(nullable: false, precision: 20, scale: 9),
                        credit = c.Decimal(nullable: false, precision: 20, scale: 9),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        exp_date = c.DateTime(precision: 0),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        parent_id_production_service_account = c.Int(),
                    })
                .PrimaryKey(t => t.id_production_service_account)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("production_service_account", t => t.parent_id_production_service_account)
                .ForeignKey("contacts", t => t.id_contact)
                .ForeignKey("items", t => t.id_item, cascadeDelete: true)
                .ForeignKey("production_order_detail", t => t.id_order_detail)
                .ForeignKey("purchase_invoice_detail", t => t.id_purchase_invoice_detail)
                .ForeignKey("purchase_order_detail", t => t.id_purchase_order_detail)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_contact)
                .Index(t => t.id_item)
                .Index(t => t.id_order_detail)
                .Index(t => t.id_purchase_order_detail)
                .Index(t => t.id_purchase_invoice_detail)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.parent_id_production_service_account);
            
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
            AddColumn("app_branch", "address", c => c.String(unicode: false));
            AddColumn("app_bank", "can_transfer", c => c.Boolean(nullable: false));
            AddColumn("app_bank", "branch", c => c.String(unicode: false));
            AddColumn("app_bank", "city", c => c.String(unicode: false));
            AddColumn("app_bank", "country", c => c.String(unicode: false));
            AddColumn("app_bank", "swift_code", c => c.String(unicode: false));
            AddColumn("app_bank", "intermediary_bank", c => c.String(unicode: false));
            AddColumn("app_bank", "intermediary_city", c => c.String(unicode: false));
            AddColumn("app_bank", "intermediary_country", c => c.String(unicode: false));
            AddColumn("app_bank", "intermediary_swift", c => c.String(unicode: false));
            AddColumn("hr_position", "id_contact", c => c.Int());
            AddColumn("item_movement", "id_purchase_packing_detail", c => c.Int());
            AddColumn("app_contract", "is_purchase", c => c.Boolean(nullable: false));
            AddColumn("app_contract", "is_sales", c => c.Boolean(nullable: false));
            AddColumn("production_execution_detail", "id_service_account", c => c.Int());
            AddColumn("production_execution_detail", "production_service_account_id_production_service_account", c => c.Int());
            AddColumn("payment_type_detail", "id_payment_approve_detail", c => c.Int());
            AddColumn("purchase_invoice_detail", "batch_code", c => c.String(unicode: false));
            AddColumn("purchase_invoice_detail", "expire_date", c => c.DateTime(precision: 0));
            AddColumn("purchase_order_detail", "expire_date", c => c.DateTime(precision: 0));
            AddColumn("purchase_order_detail", "batch_code", c => c.String(unicode: false));
            AddColumn("sales_rep", "monthly_goal", c => c.Decimal(nullable: false, precision: 20, scale: 9));
            AddColumn("purchase_return_detail", "movement_id", c => c.Int());
            AddColumn("purchase_return_detail", "batch_code", c => c.String(unicode: false));
            AddColumn("purchase_return_detail", "expire_date", c => c.DateTime(precision: 0));
            AddColumn("sales_budget_detail", "movement_id", c => c.Int());
            AddColumn("sales_budget_detail", "expire_date", c => c.DateTime(precision: 0));
            AddColumn("sales_budget_detail", "batch_code", c => c.String(unicode: false));
            AddColumn("sales_budget_detail", "id_sales_promotion", c => c.Int());
            AddColumn("sales_order_detail", "expire_date", c => c.DateTime(precision: 0));
            AddColumn("sales_order_detail", "batch_code", c => c.String(unicode: false));
            AddColumn("sales_order_detail", "id_sales_promotion", c => c.Int());
            AddColumn("sales_invoice_detail", "expire_date", c => c.DateTime(precision: 0));
            AddColumn("sales_invoice_detail", "batch_code", c => c.String(unicode: false));
            AddColumn("sales_invoice_detail", "id_sales_promotion", c => c.Int());
            AddColumn("sales_packing_detail", "id_movement", c => c.Long());
            AddColumn("sales_packing_detail", "expire_date", c => c.DateTime(precision: 0));
            AddColumn("sales_packing_detail", "batch_code", c => c.String(unicode: false));
            AddColumn("sales_packing_detail", "gross_weight", c => c.Decimal(precision: 20, scale: 9));
            AddColumn("sales_packing_detail", "net_weight", c => c.Decimal(precision: 20, scale: 9));
            AddColumn("sales_packing_detail", "volume", c => c.Decimal(precision: 20, scale: 9));
            AddColumn("sales_packing_detail", "measurement_volume_id_measurement", c => c.Int());
            AddColumn("sales_packing_detail", "measurement_weight_id_measurement", c => c.Int());
            AddColumn("sales_packing", "id_item_asset", c => c.Int());
            AddColumn("sales_packing", "eta", c => c.DateTime(precision: 0));
            AddColumn("sales_packing", "etd", c => c.DateTime(precision: 0));
            AddColumn("sales_packing", "driver", c => c.String(unicode: false));
            AddColumn("sales_packing", "licence_no", c => c.String(unicode: false));
            AddColumn("sales_packing", "avg_distance", c => c.String(unicode: false));
            AddColumn("sales_return_detail", "expire_date", c => c.DateTime(precision: 0));
            AddColumn("sales_return_detail", "batch_code", c => c.String(unicode: false));
            AddColumn("sales_return_detail", "movement_id", c => c.Int());
            AddColumn("sales_return_detail", "id_sales_promotion", c => c.Int());
            AddColumn("item_transfer_detail", "status_dest", c => c.Int(nullable: false));
            AddColumn("item_transfer_detail", "gross_weight", c => c.Decimal(precision: 20, scale: 9));
            AddColumn("item_transfer_detail", "net_weight", c => c.Decimal(precision: 20, scale: 9));
            AddColumn("item_transfer_detail", "volume", c => c.Decimal(precision: 20, scale: 9));
            AddColumn("item_transfer_detail", "expire_date", c => c.DateTime(precision: 0));
            AddColumn("item_transfer_detail", "batch_code", c => c.String(unicode: false));
            AddColumn("item_transfer_detail", "measurement_volume_id_measurement", c => c.Int());
            AddColumn("item_transfer_detail", "measurement_weight_id_measurement", c => c.Int());
            AddColumn("item_transfer", "id_item_asset", c => c.Int());
            AddColumn("item_transfer", "eta", c => c.DateTime(precision: 0));
            AddColumn("item_transfer", "etd", c => c.DateTime(precision: 0));
            AddColumn("item_transfer", "driver", c => c.String(unicode: false));
            AddColumn("item_transfer", "licence_no", c => c.String(unicode: false));
            AddColumn("item_transfer", "avg_distance", c => c.String(unicode: false));
            AddColumn("item_inventory_detail", "expire_date", c => c.DateTime(precision: 0));
            AddColumn("item_inventory_detail", "batch_code", c => c.String(unicode: false));
            AddColumn("item_inventory_detail", "movement_id", c => c.Int());
            AddColumn("impexes", "id_currency", c => c.Int(nullable: false));
            AddColumn("impexes", "fx_rate", c => c.Decimal(nullable: false, precision: 20, scale: 9));
            AddColumn("impexes", "est_shipping_date", c => c.DateTime(precision: 0));
            AddColumn("impexes", "real_shipping_date", c => c.DateTime(precision: 0));
            AddColumn("impexes", "est_landed_date", c => c.DateTime(precision: 0));
            AddColumn("impexes", "real_landed_date", c => c.DateTime(precision: 0));
            AddColumn("impexes", "real_arrival_date", c => c.DateTime(precision: 0));
            AddColumn("impex_incoterm_condition", "type", c => c.Int(nullable: false));
            AddColumn("purchase_packing", "id_item_asset", c => c.Int());
            AddColumn("purchase_packing", "eta", c => c.DateTime(precision: 0));
            AddColumn("purchase_packing", "etd", c => c.DateTime(precision: 0));
            AddColumn("purchase_packing", "driver", c => c.String(unicode: false));
            AddColumn("purchase_packing", "licence_no", c => c.String(unicode: false));
            AddColumn("purchase_packing", "avg_distance", c => c.String(unicode: false));
            AddColumn("purchase_packing_detail", "expire_date", c => c.DateTime(precision: 0));
            AddColumn("purchase_packing_detail", "batch_code", c => c.String(unicode: false));
            AddColumn("purchase_packing_detail", "gross_weight", c => c.Decimal(precision: 20, scale: 9));
            AddColumn("purchase_packing_detail", "net_weight", c => c.Decimal(precision: 20, scale: 9));
            AddColumn("purchase_packing_detail", "volume", c => c.Decimal(precision: 20, scale: 9));
            AddColumn("purchase_packing_detail", "measurement_volume_id_measurement", c => c.Int());
            AddColumn("purchase_packing_detail", "measurement_weight_id_measurement", c => c.Int());
            AlterColumn("production_execution_detail", "expiry_date", c => c.DateTime(precision: 0));
            CreateIndex("app_account_detail", "id_payment_approve_detail");
            CreateIndex("hr_position", "id_contact");
            CreateIndex("item_movement", "id_purchase_packing_detail");
            CreateIndex("payment_type_detail", "id_payment_approve_detail");
            CreateIndex("item_transfer", "id_item_asset");
            CreateIndex("item_transfer_detail", "measurement_volume_id_measurement");
            CreateIndex("item_transfer_detail", "measurement_weight_id_measurement");
            CreateIndex("purchase_packing", "id_item_asset");
            CreateIndex("purchase_packing_detail", "measurement_volume_id_measurement");
            CreateIndex("purchase_packing_detail", "measurement_weight_id_measurement");
            CreateIndex("production_execution_detail", "production_service_account_id_production_service_account");
            CreateIndex("sales_packing", "id_item_asset");
            CreateIndex("sales_budget_detail", "id_sales_promotion");
            CreateIndex("sales_order_detail", "id_sales_promotion");
            CreateIndex("sales_invoice_detail", "id_sales_promotion");
            CreateIndex("sales_return_detail", "id_sales_promotion");
            CreateIndex("sales_packing_detail", "measurement_volume_id_measurement");
            CreateIndex("sales_packing_detail", "measurement_weight_id_measurement");
            AddForeignKey("app_account_detail", "id_payment_approve_detail", "payment_approve_detail", "id_payment_approve_detail");
            AddForeignKey("item_transfer", "id_item_asset", "item_asset", "id_item_asset");
            AddForeignKey("item_transfer_detail", "measurement_volume_id_measurement", "app_measurement", "id_measurement");
            AddForeignKey("item_transfer_detail", "measurement_weight_id_measurement", "app_measurement", "id_measurement");
            AddForeignKey("purchase_packing", "id_item_asset", "item_asset", "id_item_asset");
            AddForeignKey("item_movement", "id_purchase_packing_detail", "purchase_packing_detail", "id_purchase_packing_detail");
            AddForeignKey("purchase_packing_detail", "measurement_volume_id_measurement", "app_measurement", "id_measurement");
            AddForeignKey("purchase_packing_detail", "measurement_weight_id_measurement", "app_measurement", "id_measurement");
            AddForeignKey("production_execution_detail", "production_service_account_id_production_service_account", "production_service_account", "id_production_service_account");
            AddForeignKey("sales_return_detail", "id_sales_promotion", "sales_promotion", "id_sales_promotion");
            AddForeignKey("sales_packing_detail", "measurement_volume_id_measurement", "app_measurement", "id_measurement");
            AddForeignKey("sales_packing_detail", "measurement_weight_id_measurement", "app_measurement", "id_measurement");
            AddForeignKey("sales_invoice_detail", "id_sales_promotion", "sales_promotion", "id_sales_promotion");
            AddForeignKey("sales_order_detail", "id_sales_promotion", "sales_promotion", "id_sales_promotion");
            AddForeignKey("sales_budget_detail", "id_sales_promotion", "sales_promotion", "id_sales_promotion");
            AddForeignKey("sales_packing", "id_item_asset", "item_asset", "id_item_asset");
            AddForeignKey("payment_type_detail", "id_payment_approve_detail", "payment_approve_detail", "id_payment_approve_detail");
            AddForeignKey("hr_position", "id_contact", "contacts", "id_contact");
            DropColumn("purchase_invoice_detail", "lot_number");
            DropColumn("purchase_invoice_detail", "expiration_date");
            DropColumn("purchase_order_detail", "lot_number");
            DropColumn("purchase_order_detail", "expiration_date");
            DropColumn("purchase_return_detail", "lot_number");
            DropColumn("purchase_return_detail", "expiration_date");
        }
        
        public override void Down()
        {
            AddColumn("purchase_return_detail", "expiration_date", c => c.DateTime(precision: 0));
            AddColumn("purchase_return_detail", "lot_number", c => c.String(unicode: false));
            AddColumn("purchase_order_detail", "expiration_date", c => c.DateTime(precision: 0));
            AddColumn("purchase_order_detail", "lot_number", c => c.String(unicode: false));
            AddColumn("purchase_invoice_detail", "expiration_date", c => c.DateTime(precision: 0));
            AddColumn("purchase_invoice_detail", "lot_number", c => c.String(unicode: false));
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
            DropForeignKey("hr_position", "id_contact", "contacts");
            DropForeignKey("payment_approve_detail", "id_user", "security_user");
            DropForeignKey("payment_type_detail", "id_payment_approve_detail", "payment_approve_detail");
            DropForeignKey("payment_approve_detail", "id_payment_type", "payment_type");
            DropForeignKey("payment_approve_detail", "id_payment_approve_detail", "payment_schedual");
            DropForeignKey("sales_packing", "id_item_asset", "item_asset");
            DropForeignKey("sales_budget_detail", "id_sales_promotion", "sales_promotion");
            DropForeignKey("sales_order_detail", "id_sales_promotion", "sales_promotion");
            DropForeignKey("sales_invoice_detail", "id_sales_promotion", "sales_promotion");
            DropForeignKey("sales_packing_detail", "measurement_weight_id_measurement", "app_measurement");
            DropForeignKey("sales_packing_detail", "measurement_volume_id_measurement", "app_measurement");
            DropForeignKey("sales_return_detail", "id_sales_promotion", "sales_promotion");
            DropForeignKey("production_service_account", "id_user", "security_user");
            DropForeignKey("production_service_account", "id_purchase_order_detail", "purchase_order_detail");
            DropForeignKey("production_service_account", "id_purchase_invoice_detail", "purchase_invoice_detail");
            DropForeignKey("production_service_account", "id_order_detail", "production_order_detail");
            DropForeignKey("production_execution_detail", "production_service_account_id_production_service_account", "production_service_account");
            DropForeignKey("production_service_account", "id_item", "items");
            DropForeignKey("production_service_account", "id_contact", "contacts");
            DropForeignKey("production_service_account", "parent_id_production_service_account", "production_service_account");
            DropForeignKey("production_service_account", "id_company", "app_company");
            DropForeignKey("purchase_packing_detail", "measurement_weight_id_measurement", "app_measurement");
            DropForeignKey("purchase_packing_detail", "measurement_volume_id_measurement", "app_measurement");
            DropForeignKey("item_movement", "id_purchase_packing_detail", "purchase_packing_detail");
            DropForeignKey("purchase_packing", "id_item_asset", "item_asset");
            DropForeignKey("item_transfer_detail", "measurement_weight_id_measurement", "app_measurement");
            DropForeignKey("item_transfer_detail", "measurement_volume_id_measurement", "app_measurement");
            DropForeignKey("item_transfer", "id_item_asset", "item_asset");
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
            DropIndex("sales_packing_detail", new[] { "measurement_weight_id_measurement" });
            DropIndex("sales_packing_detail", new[] { "measurement_volume_id_measurement" });
            DropIndex("sales_return_detail", new[] { "id_sales_promotion" });
            DropIndex("sales_invoice_detail", new[] { "id_sales_promotion" });
            DropIndex("sales_order_detail", new[] { "id_sales_promotion" });
            DropIndex("sales_budget_detail", new[] { "id_sales_promotion" });
            DropIndex("sales_packing", new[] { "id_item_asset" });
            DropIndex("production_service_account", new[] { "parent_id_production_service_account" });
            DropIndex("production_service_account", new[] { "id_user" });
            DropIndex("production_service_account", new[] { "id_company" });
            DropIndex("production_service_account", new[] { "id_purchase_invoice_detail" });
            DropIndex("production_service_account", new[] { "id_purchase_order_detail" });
            DropIndex("production_service_account", new[] { "id_order_detail" });
            DropIndex("production_service_account", new[] { "id_item" });
            DropIndex("production_service_account", new[] { "id_contact" });
            DropIndex("production_execution_detail", new[] { "production_service_account_id_production_service_account" });
            DropIndex("purchase_packing_detail", new[] { "measurement_weight_id_measurement" });
            DropIndex("purchase_packing_detail", new[] { "measurement_volume_id_measurement" });
            DropIndex("purchase_packing", new[] { "id_item_asset" });
            DropIndex("item_transfer_detail", new[] { "measurement_weight_id_measurement" });
            DropIndex("item_transfer_detail", new[] { "measurement_volume_id_measurement" });
            DropIndex("item_transfer", new[] { "id_item_asset" });
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
            DropIndex("payment_approve_detail", new[] { "id_payment_approve_detail" });
            DropIndex("payment_type_detail", new[] { "id_payment_approve_detail" });
            DropIndex("item_movement", new[] { "id_purchase_packing_detail" });
            DropIndex("hr_position", new[] { "id_contact" });
            DropIndex("app_account_detail", new[] { "id_payment_approve_detail" });
            AlterColumn("production_execution_detail", "expiry_date", c => c.DateTime(nullable: false, precision: 0));
            DropColumn("purchase_packing_detail", "measurement_weight_id_measurement");
            DropColumn("purchase_packing_detail", "measurement_volume_id_measurement");
            DropColumn("purchase_packing_detail", "volume");
            DropColumn("purchase_packing_detail", "net_weight");
            DropColumn("purchase_packing_detail", "gross_weight");
            DropColumn("purchase_packing_detail", "batch_code");
            DropColumn("purchase_packing_detail", "expire_date");
            DropColumn("purchase_packing", "avg_distance");
            DropColumn("purchase_packing", "licence_no");
            DropColumn("purchase_packing", "driver");
            DropColumn("purchase_packing", "etd");
            DropColumn("purchase_packing", "eta");
            DropColumn("purchase_packing", "id_item_asset");
            DropColumn("impex_incoterm_condition", "type");
            DropColumn("impexes", "real_arrival_date");
            DropColumn("impexes", "real_landed_date");
            DropColumn("impexes", "est_landed_date");
            DropColumn("impexes", "real_shipping_date");
            DropColumn("impexes", "est_shipping_date");
            DropColumn("impexes", "fx_rate");
            DropColumn("impexes", "id_currency");
            DropColumn("item_inventory_detail", "movement_id");
            DropColumn("item_inventory_detail", "batch_code");
            DropColumn("item_inventory_detail", "expire_date");
            DropColumn("item_transfer", "avg_distance");
            DropColumn("item_transfer", "licence_no");
            DropColumn("item_transfer", "driver");
            DropColumn("item_transfer", "etd");
            DropColumn("item_transfer", "eta");
            DropColumn("item_transfer", "id_item_asset");
            DropColumn("item_transfer_detail", "measurement_weight_id_measurement");
            DropColumn("item_transfer_detail", "measurement_volume_id_measurement");
            DropColumn("item_transfer_detail", "batch_code");
            DropColumn("item_transfer_detail", "expire_date");
            DropColumn("item_transfer_detail", "volume");
            DropColumn("item_transfer_detail", "net_weight");
            DropColumn("item_transfer_detail", "gross_weight");
            DropColumn("item_transfer_detail", "status_dest");
            DropColumn("sales_return_detail", "id_sales_promotion");
            DropColumn("sales_return_detail", "movement_id");
            DropColumn("sales_return_detail", "batch_code");
            DropColumn("sales_return_detail", "expire_date");
            DropColumn("sales_packing", "avg_distance");
            DropColumn("sales_packing", "licence_no");
            DropColumn("sales_packing", "driver");
            DropColumn("sales_packing", "etd");
            DropColumn("sales_packing", "eta");
            DropColumn("sales_packing", "id_item_asset");
            DropColumn("sales_packing_detail", "measurement_weight_id_measurement");
            DropColumn("sales_packing_detail", "measurement_volume_id_measurement");
            DropColumn("sales_packing_detail", "volume");
            DropColumn("sales_packing_detail", "net_weight");
            DropColumn("sales_packing_detail", "gross_weight");
            DropColumn("sales_packing_detail", "batch_code");
            DropColumn("sales_packing_detail", "expire_date");
            DropColumn("sales_packing_detail", "id_movement");
            DropColumn("sales_invoice_detail", "id_sales_promotion");
            DropColumn("sales_invoice_detail", "batch_code");
            DropColumn("sales_invoice_detail", "expire_date");
            DropColumn("sales_order_detail", "id_sales_promotion");
            DropColumn("sales_order_detail", "batch_code");
            DropColumn("sales_order_detail", "expire_date");
            DropColumn("sales_budget_detail", "id_sales_promotion");
            DropColumn("sales_budget_detail", "batch_code");
            DropColumn("sales_budget_detail", "expire_date");
            DropColumn("sales_budget_detail", "movement_id");
            DropColumn("purchase_return_detail", "expire_date");
            DropColumn("purchase_return_detail", "batch_code");
            DropColumn("purchase_return_detail", "movement_id");
            DropColumn("sales_rep", "monthly_goal");
            DropColumn("purchase_order_detail", "batch_code");
            DropColumn("purchase_order_detail", "expire_date");
            DropColumn("purchase_invoice_detail", "expire_date");
            DropColumn("purchase_invoice_detail", "batch_code");
            DropColumn("payment_type_detail", "id_payment_approve_detail");
            DropColumn("production_execution_detail", "production_service_account_id_production_service_account");
            DropColumn("production_execution_detail", "id_service_account");
            DropColumn("app_contract", "is_sales");
            DropColumn("app_contract", "is_purchase");
            DropColumn("item_movement", "id_purchase_packing_detail");
            DropColumn("hr_position", "id_contact");
            DropColumn("app_bank", "intermediary_swift");
            DropColumn("app_bank", "intermediary_country");
            DropColumn("app_bank", "intermediary_city");
            DropColumn("app_bank", "intermediary_bank");
            DropColumn("app_bank", "swift_code");
            DropColumn("app_bank", "country");
            DropColumn("app_bank", "city");
            DropColumn("app_bank", "branch");
            DropColumn("app_bank", "can_transfer");
            DropColumn("app_branch", "address");
            DropColumn("app_account_detail", "id_payment_approve_detail");
            DropTable("loyalty_tier");
            DropTable("loyalty_member_detail");
            DropTable("loyalty_member");
            DropTable("crm_schedual");
            DropTable("production_service_account");
            DropTable("payment_approve");
            DropTable("payment_approve_detail");
        }
    }
}
