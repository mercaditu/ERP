namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "accounting_budget",
                c => new
                    {
                        id_budget = c.Int(nullable: false, identity: true),
                        id_chart = c.Int(nullable: false),
                        id_cycle = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_budget)                
                .ForeignKey("accounting_cycle", t => t.id_cycle, cascadeDelete: true)
                .ForeignKey("accounting_chart", t => t.id_chart, cascadeDelete: true)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_chart)
                .Index(t => t.id_cycle)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "accounting_chart",
                c => new
                    {
                        id_chart = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, unicode: false),
                        code = c.String(nullable: false, unicode: false),
                        chart_type = c.Int(nullable: false),
                        chartsub_type = c.Int(nullable: false),
                        id_account = c.Int(),
                        id_contact = c.Int(),
                        id_tag = c.Int(),
                        id_item_asset_group = c.Int(),
                        id_vat = c.Int(),
                        id_department = c.Int(),
                        id_cost_center = c.Int(),
                        is_generic = c.Boolean(nullable: false),
                        is_active = c.Boolean(nullable: false),
                        can_transact = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        parent_id_chart = c.Int(),
                    })
                .PrimaryKey(t => t.id_chart)                
                .ForeignKey("app_account", t => t.id_account)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_cost_center", t => t.id_cost_center)
                .ForeignKey("app_department", t => t.id_department)
                .ForeignKey("app_vat", t => t.id_vat)
                .ForeignKey("accounting_chart", t => t.parent_id_chart)
                .ForeignKey("contacts", t => t.id_contact)
                .ForeignKey("item_asset_group", t => t.id_item_asset_group)
                .ForeignKey("item_tag", t => t.id_tag)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_account)
                .Index(t => t.id_contact)
                .Index(t => t.id_tag)
                .Index(t => t.id_item_asset_group)
                .Index(t => t.id_vat)
                .Index(t => t.id_department)
                .Index(t => t.id_cost_center)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.parent_id_chart);
            
            CreateTable(
                "accounting_journal_detail",
                c => new
                    {
                        id_journal_detail = c.Int(nullable: false, identity: true),
                        id_journal = c.Int(nullable: false),
                        id_application = c.Int(nullable: false),
                        id_chart = c.Int(nullable: false),
                        id_currencyfx = c.Int(nullable: false),
                        debit = c.Decimal(nullable: false, precision: 20, scale: 9),
                        credit = c.Decimal(nullable: false, precision: 20, scale: 9),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_journal_detail)                
                .ForeignKey("accounting_chart", t => t.id_chart, cascadeDelete: true)
                .ForeignKey("accounting_journal", t => t.id_journal, cascadeDelete: true)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_currencyfx", t => t.id_currencyfx, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_journal)
                .Index(t => t.id_chart)
                .Index(t => t.id_currencyfx)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "accounting_journal",
                c => new
                    {
                        id_journal = c.Int(nullable: false, identity: true),
                        id_branch = c.Int(nullable: false),
                        id_cycle = c.Int(nullable: false),
                        type = c.Int(nullable: false),
                        code = c.Int(nullable: false),
                        comment = c.String(unicode: false),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        status = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_journal)                
                .ForeignKey("accounting_cycle", t => t.id_cycle, cascadeDelete: true)
                .ForeignKey("app_branch", t => t.id_branch, cascadeDelete: true)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_branch)
                .Index(t => t.id_cycle)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "accounting_cycle",
                c => new
                    {
                        id_cycle = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, unicode: false),
                        start_date = c.DateTime(nullable: false, precision: 0),
                        end_date = c.DateTime(nullable: false, precision: 0),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_cycle)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "app_company",
                c => new
                    {
                        id_company = c.Int(nullable: false, identity: true),
                        id_geography = c.Int(),
                        name = c.String(nullable: false, unicode: false),
                        alias = c.String(unicode: false),
                        gov_code = c.String(nullable: false, unicode: false),
                        address = c.String(nullable: false, unicode: false),
                        domain = c.String(unicode: false),
                        hash_debehaber = c.String(unicode: false),
                        representative_name = c.String(unicode: false),
                        representative_gov_code = c.String(unicode: false),
                        accountant_name = c.String(unicode: false),
                        accountant_gov_code = c.String(unicode: false),
                        is_active = c.Boolean(nullable: false),
                        version = c.String(unicode: false),
                        seats = c.String(unicode: false),
                        email_imap = c.String(unicode: false),
                        email_smtp = c.String(unicode: false),
                        email_port_out = c.Short(nullable: false),
                        email_port_in = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.id_company)                ;
            
            CreateTable(
                "app_branch",
                c => new
                    {
                        id_branch = c.Int(nullable: false, identity: true),
                        id_geography = c.Int(),
                        id_vat = c.Int(),
                        name = c.String(nullable: false, unicode: false),
                        code = c.String(nullable: false, unicode: false),
                        area = c.Decimal(precision: 20, scale: 9),
                        id_measurement = c.Int(),
                        geo_lat = c.Decimal(precision: 20, scale: 9),
                        geo_long = c.Decimal(precision: 20, scale: 9),
                        can_stock = c.Boolean(nullable: false),
                        can_invoice = c.Boolean(nullable: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_branch)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_geography", t => t.id_geography)
                .ForeignKey("app_measurement", t => t.id_measurement)
                .ForeignKey("app_vat", t => t.id_vat)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_geography)
                .Index(t => t.id_vat)
                .Index(t => t.id_measurement)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "app_geography",
                c => new
                    {
                        id_geography = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, unicode: false),
                        code = c.String(unicode: false),
                        type = c.Int(nullable: false),
                        geo_long = c.Decimal(precision: 20, scale: 9),
                        geo_lat = c.Decimal(precision: 20, scale: 9),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        parent_id_geography = c.Int(),
                    })
                .PrimaryKey(t => t.id_geography)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_geography", t => t.parent_id_geography)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.parent_id_geography);
            
            CreateTable(
                "app_bank",
                c => new
                    {
                        id_bank = c.Int(nullable: false, identity: true),
                        id_geography = c.Int(),
                        name = c.String(nullable: false, unicode: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_bank)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_geography", t => t.id_geography)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_geography)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "app_account",
                c => new
                    {
                        id_account = c.Int(nullable: false, identity: true),
                        id_account_type = c.Int(nullable: false),
                        id_bank = c.Int(),
                        id_currency = c.Int(),
                        id_terminal = c.Int(),
                        name = c.String(nullable: false, unicode: false),
                        code = c.String(unicode: false),
                        initial_amount = c.Decimal(precision: 20, scale: 9),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_account)                
                .ForeignKey("app_terminal", t => t.id_terminal)
                .ForeignKey("app_bank", t => t.id_bank)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_currency", t => t.id_currency)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_bank)
                .Index(t => t.id_currency)
                .Index(t => t.id_terminal)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "app_account_detail",
                c => new
                    {
                        id_account_detail = c.Int(nullable: false, identity: true),
                        id_account = c.Int(nullable: false),
                        id_currencyfx = c.Int(nullable: false),
                        id_payment_detail = c.Int(),
                        id_payment_type = c.Int(nullable: false),
                        status = c.Int(nullable: false),
                        debit = c.Decimal(nullable: false, precision: 20, scale: 9),
                        credit = c.Decimal(nullable: false, precision: 20, scale: 9),
                        comment = c.String(unicode: false),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        id_session = c.Int(),
                        tran_type = c.Int(),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_account_detail)                
                .ForeignKey("app_account", t => t.id_account, cascadeDelete: true)
                .ForeignKey("app_account_session", t => t.id_session)
                .ForeignKey("payment_detail", t => t.id_payment_detail)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_currencyfx", t => t.id_currencyfx, cascadeDelete: true)
                .ForeignKey("payment_type", t => t.id_payment_type, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_account)
                .Index(t => t.id_currencyfx)
                .Index(t => t.id_payment_detail)
                .Index(t => t.id_payment_type)
                .Index(t => t.id_session)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "app_account_session",
                c => new
                    {
                        id_session = c.Int(nullable: false, identity: true),
                        op_date = c.DateTime(nullable: false, precision: 0),
                        cl_date = c.DateTime(precision: 0),
                        id_account = c.Int(nullable: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_session)                
                .ForeignKey("app_account", t => t.id_account, cascadeDelete: true)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_account)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "security_user",
                c => new
                    {
                        id_user = c.Int(nullable: false, identity: true),
                        id_company = c.Int(),
                        id_contact = c.Int(),
                        id_question = c.Int(),
                        id_role = c.Int(nullable: false),
                        name = c.String(nullable: false, unicode: false),
                        password = c.String(nullable: false, unicode: false),
                        name_full = c.String(unicode: false),
                        email = c.String(unicode: false),
                        email_username = c.String(unicode: false),
                        email_password = c.String(unicode: false),
                        code = c.String(unicode: false),
                        security_answer = c.String(unicode: false),
                        is_active = c.Boolean(nullable: false),
                        trans_date = c.DateTime(precision: 0),
                        id_created_user = c.Int(),
                        email_imap = c.String(unicode: false),
                        email_smtp = c.String(unicode: false),
                        email_port_out = c.Short(nullable: false),
                        email_port_in = c.Short(nullable: false),
                        parent_id_user = c.Int(),
                    })
                .PrimaryKey(t => t.id_user)                
                .ForeignKey("app_company", t => t.id_company)
                .ForeignKey("security_user", t => t.parent_id_user)
                .ForeignKey("security_question", t => t.id_question)
                .ForeignKey("security_role", t => t.id_role, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_question)
                .Index(t => t.id_role)
                .Index(t => t.parent_id_user);
            
            CreateTable(
                "security_question",
                c => new
                    {
                        id_question = c.Int(nullable: false, identity: true),
                        question = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.id_question)                ;
            
            CreateTable(
                "security_role",
                c => new
                    {
                        id_role = c.Int(nullable: false, identity: true),
                        id_company = c.Int(nullable: false),
                        id_department = c.Int(),
                        name = c.String(nullable: false, unicode: false),
                        see_cost = c.Boolean(nullable: false),
                        is_active = c.Boolean(nullable: false),
                        is_master = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_role)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_department", t => t.id_department)
                .Index(t => t.id_company)
                .Index(t => t.id_department);
            
            CreateTable(
                "app_department",
                c => new
                    {
                        id_department = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, unicode: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_department)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "hr_position",
                c => new
                    {
                        id_position = c.Int(nullable: false, identity: true),
                        id_department = c.Int(nullable: false),
                        name = c.String(unicode: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        parent_id_position = c.Int(),
                    })
                .PrimaryKey(t => t.id_position)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_department", t => t.id_department, cascadeDelete: true)
                .ForeignKey("hr_position", t => t.parent_id_position)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_department)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.parent_id_position);
            
            CreateTable(
                "item_request",
                c => new
                    {
                        id_item_request = c.Int(nullable: false, identity: true),
                        id_project = c.Int(),
                        id_sales_order = c.Int(),
                        id_production_order = c.Int(),
                        id_currency = c.Int(),
                        id_branch = c.Int(),
                        id_department = c.Int(),
                        request_date = c.DateTime(nullable: false, precision: 0),
                        name = c.String(unicode: false),
                        comment = c.String(unicode: false),
                        status = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        request_user_id_user = c.Int(),
                        security_user_id_user = c.Int(),
                    })
                .PrimaryKey(t => t.id_item_request)                
                .ForeignKey("app_branch", t => t.id_branch)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("production_order", t => t.id_production_order)
                .ForeignKey("app_currency", t => t.id_currency)
                .ForeignKey("app_department", t => t.id_department)
                .ForeignKey("projects", t => t.id_project)
                .ForeignKey("security_user", t => t.request_user_id_user)
                .ForeignKey("sales_order", t => t.id_sales_order)
                .ForeignKey("security_user", t => t.security_user_id_user)
                .Index(t => t.id_project)
                .Index(t => t.id_sales_order)
                .Index(t => t.id_production_order)
                .Index(t => t.id_currency)
                .Index(t => t.id_branch)
                .Index(t => t.id_department)
                .Index(t => t.id_company)
                .Index(t => t.request_user_id_user)
                .Index(t => t.security_user_id_user);
            
            CreateTable(
                "app_currency",
                c => new
                    {
                        id_currency = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, unicode: false),
                        id_country = c.Int(),
                        is_priority = c.Boolean(nullable: false),
                        is_active = c.Boolean(nullable: false),
                        has_rounding = c.Boolean(nullable: false),
                        is_reverse = c.Boolean(nullable: false),
                        id_company = c.Int(),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_currency)                
                .ForeignKey("app_company", t => t.id_company)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "app_currency_denomination",
                c => new
                    {
                        id_denomination = c.Int(nullable: false, identity: true),
                        id_currency = c.Int(nullable: false),
                        name = c.String(nullable: false, unicode: false),
                        is_bill = c.Boolean(nullable: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_denomination)                
                .ForeignKey("app_company", t => t.id_company)
                .ForeignKey("app_currency", t => t.id_currency, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_currency)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "app_currencyfx",
                c => new
                    {
                        id_currencyfx = c.Int(nullable: false, identity: true),
                        id_currency = c.Int(nullable: false),
                        buy_value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        sell_value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        is_active = c.Boolean(nullable: false),
                        is_reverse = c.Boolean(nullable: false),
                        type = c.Int(nullable: false),
                        id_company = c.Int(),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_currencyfx)                
                .ForeignKey("app_company", t => t.id_company)
                .ForeignKey("app_currency", t => t.id_currency, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_currency)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "impex_expense",
                c => new
                    {
                        id_impex_expense = c.Int(nullable: false, identity: true),
                        id_impex = c.Int(nullable: false),
                        id_purchase_invoice = c.Int(),
                        id_incoterm_condition = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_currencyfx = c.Int(nullable: false),
                        id_currency = c.Int(),
                        currency_rate = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_impex_expense)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_currency", t => t.id_currency)
                .ForeignKey("impexes", t => t.id_impex, cascadeDelete: true)
                .ForeignKey("impex_incoterm_condition", t => t.id_incoterm_condition, cascadeDelete: true)
                .ForeignKey("purchase_invoice", t => t.id_purchase_invoice)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_impex)
                .Index(t => t.id_purchase_invoice)
                .Index(t => t.id_incoterm_condition)
                .Index(t => t.id_currency)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "impexes",
                c => new
                    {
                        id_impex = c.Int(nullable: false, identity: true),
                        id_incoterm = c.Int(nullable: false),
                        id_contact = c.Int(nullable: false),
                        status = c.Int(nullable: false),
                        number = c.String(unicode: false),
                        impex_type = c.Int(nullable: false),
                        etd = c.DateTime(nullable: false, precision: 0),
                        eta = c.DateTime(nullable: false, precision: 0),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_impex)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("contacts", t => t.id_contact, cascadeDelete: true)
                .ForeignKey("impex_incoterm", t => t.id_incoterm, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_incoterm)
                .Index(t => t.id_contact)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "contacts",
                c => new
                    {
                        id_contact = c.Int(nullable: false, identity: true),
                        id_contact_role = c.Int(nullable: false),
                        id_contract = c.Int(),
                        id_currency = c.Int(),
                        id_cost_center = c.Int(),
                        id_price_list = c.Int(),
                        id_sales_rep = c.Int(),
                        id_geography = c.Int(),
                        id_bank = c.Int(),
                        name = c.String(nullable: false, unicode: false),
                        alias = c.String(unicode: false),
                        code = c.String(unicode: false),
                        gov_code = c.String(unicode: false),
                        telephone = c.String(unicode: false),
                        email = c.String(unicode: false),
                        address = c.String(unicode: false),
                        credit_limit = c.Decimal(precision: 20, scale: 9),
                        lead_time = c.Int(),
                        geo_lat = c.Decimal(precision: 20, scale: 9),
                        geo_long = c.Decimal(precision: 20, scale: 9),
                        is_customer = c.Boolean(nullable: false),
                        is_supplier = c.Boolean(nullable: false),
                        is_employee = c.Boolean(nullable: false),
                        is_sales_rep = c.Boolean(nullable: false),
                        is_active = c.Boolean(nullable: false),
                        comment = c.String(unicode: false),
                        is_person = c.Boolean(nullable: false),
                        date_birth = c.DateTime(precision: 0),
                        gender = c.Int(),
                        social_code = c.String(unicode: false),
                        blood_type = c.Int(),
                        marital_status = c.Int(),
                        id_company = c.Int(),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        parent_id_contact = c.Int(),
                    })
                .PrimaryKey(t => t.id_contact)                
                .ForeignKey("app_bank", t => t.id_bank)
                .ForeignKey("app_company", t => t.id_company)
                .ForeignKey("app_contract", t => t.id_contract)
                .ForeignKey("app_cost_center", t => t.id_cost_center)
                .ForeignKey("app_currency", t => t.id_currency)
                .ForeignKey("app_geography", t => t.id_geography)
                .ForeignKey("contacts", t => t.parent_id_contact)
                .ForeignKey("contact_role", t => t.id_contact_role, cascadeDelete: true)
                .ForeignKey("item_price_list", t => t.id_price_list)
                .ForeignKey("sales_rep", t => t.id_sales_rep)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_contact_role)
                .Index(t => t.id_contract)
                .Index(t => t.id_currency)
                .Index(t => t.id_cost_center)
                .Index(t => t.id_price_list)
                .Index(t => t.id_sales_rep)
                .Index(t => t.id_geography)
                .Index(t => t.id_bank)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.parent_id_contact);
            
            CreateTable(
                "app_contract",
                c => new
                    {
                        id_contract = c.Int(nullable: false, identity: true),
                        id_condition = c.Int(nullable: false),
                        name = c.String(nullable: false, unicode: false),
                        is_active = c.Boolean(nullable: false),
                        is_default = c.Boolean(nullable: false),
                        is_promissory = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_contract)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_condition", t => t.id_condition, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_condition)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "app_condition",
                c => new
                    {
                        id_condition = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, unicode: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_condition)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "app_contract_detail",
                c => new
                    {
                        id_contract_detail = c.Int(nullable: false, identity: true),
                        id_contract = c.Int(nullable: false),
                        coefficient = c.Decimal(nullable: false, precision: 20, scale: 9),
                        interval = c.Short(nullable: false),
                        is_order = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_contract_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_contract", t => t.id_contract, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_contract)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "app_cost_center",
                c => new
                    {
                        id_cost_center = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, unicode: false),
                        is_administrative = c.Boolean(nullable: false),
                        is_product = c.Boolean(nullable: false),
                        is_fixedasset = c.Boolean(nullable: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_cost_center)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "contact_field_value",
                c => new
                    {
                        id_contact_field = c.Short(nullable: false, identity: true),
                        id_contact = c.Int(nullable: false),
                        id_field = c.Short(nullable: false),
                        value = c.String(unicode: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_contact_field)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_field", t => t.id_field, cascadeDelete: true)
                .ForeignKey("contacts", t => t.id_contact, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_contact)
                .Index(t => t.id_field)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "app_field",
                c => new
                    {
                        id_field = c.Short(nullable: false, identity: true),
                        name = c.String(nullable: false, unicode: false),
                        field_type = c.Int(nullable: false),
                        mask = c.String(unicode: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_field)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "contact_role",
                c => new
                    {
                        id_contact_role = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, unicode: false),
                        is_principal = c.Boolean(nullable: false),
                        can_transact = c.Boolean(nullable: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_contact_role)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "contact_subscription",
                c => new
                    {
                        id_subscription = c.Int(nullable: false, identity: true),
                        id_contact = c.Int(nullable: false),
                        id_item = c.Int(nullable: false),
                        id_contract = c.Int(nullable: false),
                        id_vat_group = c.Int(nullable: false),
                        unit_price = c.Decimal(nullable: false, precision: 20, scale: 9),
                        start_date = c.DateTime(nullable: false, precision: 0),
                        end_date = c.DateTime(precision: 0),
                        bill_cycle = c.Int(nullable: false),
                        bill_on = c.Short(nullable: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_subscription)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("contacts", t => t.id_contact, cascadeDelete: true)
                .ForeignKey("items", t => t.id_item, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_contact)
                .Index(t => t.id_item)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "items",
                c => new
                    {
                        id_item = c.Int(nullable: false, identity: true),
                        id_item_type = c.Int(nullable: false),
                        id_vat_group = c.Int(nullable: false),
                        id_brand = c.Int(),
                        id_measurement = c.Int(),
                        id_name_template = c.Short(),
                        name = c.String(nullable: false, unicode: false),
                        code = c.String(nullable: false, unicode: false),
                        variation = c.String(unicode: false),
                        description = c.String(unicode: false),
                        unit_cost = c.Decimal(precision: 20, scale: 9),
                        is_autorecepie = c.Boolean(nullable: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_item)                
                .ForeignKey("app_company", t => t.id_company)
                .ForeignKey("app_measurement", t => t.id_measurement)
                .ForeignKey("app_vat_group", t => t.id_vat_group, cascadeDelete: true)
                .ForeignKey("item_brand", t => t.id_brand)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_vat_group)
                .Index(t => t.id_brand)
                .Index(t => t.id_measurement)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "app_measurement",
                c => new
                    {
                        id_measurement = c.Int(nullable: false, identity: true),
                        id_measurement_type = c.Short(nullable: false),
                        name = c.String(nullable: false, unicode: false),
                        code_iso = c.String(unicode: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_measurement)                
                .ForeignKey("app_company", t => t.id_company)
                .ForeignKey("app_measurement_type", t => t.id_measurement_type, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_measurement_type)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "app_measurement_type",
                c => new
                    {
                        id_measurement_type = c.Short(nullable: false, identity: true),
                        name = c.String(nullable: false, unicode: false),
                        id_company = c.Int(),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_measurement_type)                
                .ForeignKey("app_company", t => t.id_company)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_conversion_factor",
                c => new
                    {
                        id_item_conversion = c.Int(nullable: false, identity: true),
                        id_measurement = c.Int(nullable: false),
                        id_item_product = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_item_conversion)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_measurement", t => t.id_measurement, cascadeDelete: true)
                .ForeignKey("item_product", t => t.id_item_product, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_measurement)
                .Index(t => t.id_item_product)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_product",
                c => new
                    {
                        id_item_product = c.Int(nullable: false, identity: true),
                        id_item = c.Int(nullable: false),
                        stock_min = c.Decimal(precision: 20, scale: 9),
                        stock_max = c.Decimal(precision: 20, scale: 9),
                        can_expire = c.Boolean(nullable: false),
                        is_weigted = c.Boolean(nullable: false),
                        cogs_type = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_item_product)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("items", t => t.id_item, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_item)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_movement",
                c => new
                    {
                        id_movement = c.Long(nullable: false, identity: true),
                        id_item_product = c.Int(nullable: false),
                        id_transfer_detail = c.Int(),
                        id_execution_detail = c.Int(),
                        id_purchase_invoice_detail = c.Int(),
                        id_purchase_return_detail = c.Int(),
                        id_sales_invoice_detail = c.Int(),
                        id_sales_return_detail = c.Int(),
                        id_inventory_detail = c.Int(),
                        id_sales_packing_detail = c.Int(),
                        id_location = c.Int(nullable: false),
                        status = c.Int(nullable: false),
                        debit = c.Decimal(nullable: false, precision: 20, scale: 9),
                        credit = c.Decimal(nullable: false, precision: 20, scale: 9),
                        comment = c.String(unicode: false),
                        code = c.String(unicode: false),
                        expire_date = c.DateTime(precision: 0),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        parent_id_movement = c.Long(),
                    })
                .PrimaryKey(t => t.id_movement)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_location", t => t.id_location, cascadeDelete: true)
                .ForeignKey("item_movement", t => t.parent_id_movement)
                .ForeignKey("sales_invoice_detail", t => t.id_sales_invoice_detail)
                .ForeignKey("sales_packing_detail", t => t.id_sales_packing_detail)
                .ForeignKey("production_execution_detail", t => t.id_execution_detail)
                .ForeignKey("sales_return_detail", t => t.id_sales_return_detail)
                .ForeignKey("purchase_invoice_detail", t => t.id_purchase_invoice_detail)
                .ForeignKey("purchase_return_detail", t => t.id_purchase_return_detail)
                .ForeignKey("item_product", t => t.id_item_product, cascadeDelete: true)
                .ForeignKey("item_transfer_detail", t => t.id_transfer_detail)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_item_product)
                .Index(t => t.id_transfer_detail)
                .Index(t => t.id_execution_detail)
                .Index(t => t.id_purchase_invoice_detail)
                .Index(t => t.id_purchase_return_detail)
                .Index(t => t.id_sales_invoice_detail)
                .Index(t => t.id_sales_return_detail)
                .Index(t => t.id_sales_packing_detail)
                .Index(t => t.id_location)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.parent_id_movement);
            
            CreateTable(
                "app_location",
                c => new
                    {
                        id_location = c.Int(nullable: false, identity: true),
                        id_branch = c.Int(nullable: false),
                        id_contact = c.Int(),
                        name = c.String(nullable: false, unicode: false),
                        is_active = c.Boolean(nullable: false),
                        is_default = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_location)                
                .ForeignKey("app_branch", t => t.id_branch, cascadeDelete: true)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("contacts", t => t.id_contact)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_branch)
                .Index(t => t.id_contact)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_movement_dimension",
                c => new
                    {
                        id_movement_dimension = c.Long(nullable: false, identity: true),
                        id_movement = c.Long(nullable: false),
                        id_dimension = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_movement_dimension)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_dimension", t => t.id_dimension, cascadeDelete: true)
                .ForeignKey("item_movement", t => t.id_movement, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_movement)
                .Index(t => t.id_dimension)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "app_dimension",
                c => new
                    {
                        id_dimension = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, unicode: false),
                        id_company = c.Int(),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_dimension)                
                .ForeignKey("app_company", t => t.id_company)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_dimension",
                c => new
                    {
                        id_item_dimension = c.Int(nullable: false, identity: true),
                        id_item = c.Int(nullable: false),
                        id_app_dimension = c.Int(nullable: false),
                        id_measurement = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        app_dimension_id_dimension = c.Int(),
                    })
                .PrimaryKey(t => t.id_item_dimension)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_dimension", t => t.app_dimension_id_dimension)
                .ForeignKey("app_measurement", t => t.id_measurement, cascadeDelete: true)
                .ForeignKey("items", t => t.id_item, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_item)
                .Index(t => t.id_measurement)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.app_dimension_id_dimension);
            
            CreateTable(
                "project_task_dimension",
                c => new
                    {
                        id_task_dimension = c.Int(nullable: false, identity: true),
                        id_project_task = c.Int(nullable: false),
                        id_dimension = c.Int(nullable: false),
                        id_measurement = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_task_dimension)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_dimension", t => t.id_dimension, cascadeDelete: true)
                .ForeignKey("app_measurement", t => t.id_measurement, cascadeDelete: true)
                .ForeignKey("project_task", t => t.id_project_task, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_project_task)
                .Index(t => t.id_dimension)
                .Index(t => t.id_measurement)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "project_task",
                c => new
                    {
                        id_project_task = c.Int(nullable: false, identity: true),
                        id_project = c.Int(nullable: false),
                        status = c.Int(),
                        id_item = c.Int(),
                        item_description = c.String(unicode: false),
                        code = c.String(unicode: false),
                        quantity_est = c.Decimal(precision: 20, scale: 9),
                        unit_cost_est = c.Decimal(precision: 20, scale: 9),
                        start_date_est = c.DateTime(precision: 0),
                        end_date_est = c.DateTime(precision: 0),
                        trans_date = c.DateTime(precision: 0),
                        is_active = c.Boolean(nullable: false),
                        id_range = c.Int(),
                        number = c.String(unicode: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        parent_id_project_task = c.Int(),
                        sales_detail_id_sales_order_detail = c.Int(),
                    })
                .PrimaryKey(t => t.id_project_task)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("projects", t => t.id_project, cascadeDelete: true)
                .ForeignKey("app_document_range", t => t.id_range)
                .ForeignKey("project_task", t => t.parent_id_project_task)
                .ForeignKey("items", t => t.id_item)
                .ForeignKey("sales_order_detail", t => t.sales_detail_id_sales_order_detail)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_project)
                .Index(t => t.id_item)
                .Index(t => t.id_range)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.parent_id_project_task)
                .Index(t => t.sales_detail_id_sales_order_detail);
            
            CreateTable(
                "app_document_range",
                c => new
                    {
                        id_range = c.Int(nullable: false, identity: true),
                        id_document = c.Int(nullable: false),
                        id_branch = c.Int(),
                        id_terminal = c.Int(),
                        range_start = c.Int(nullable: false),
                        range_current = c.Int(nullable: false),
                        range_end = c.Int(nullable: false),
                        range_padding = c.String(unicode: false),
                        range_template = c.String(nullable: false, unicode: false),
                        code = c.String(unicode: false),
                        expire_date = c.DateTime(precision: 0),
                        use_default_printer = c.Boolean(nullable: false),
                        printer_name = c.String(unicode: false),
                        can_print = c.Boolean(nullable: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_range)                
                .ForeignKey("app_branch", t => t.id_branch)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_document", t => t.id_document, cascadeDelete: true)
                .ForeignKey("app_terminal", t => t.id_terminal)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_document)
                .Index(t => t.id_branch)
                .Index(t => t.id_terminal)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "app_document",
                c => new
                    {
                        id_document = c.Int(nullable: false, identity: true),
                        id_application = c.Int(nullable: false),
                        name = c.String(nullable: false, unicode: false),
                        designer_name = c.String(unicode: false),
                        style_reciept = c.Boolean(nullable: false),
                        style_printer = c.Boolean(nullable: false),
                        filterby_branch = c.Boolean(nullable: false),
                        filterby_tearminal = c.Boolean(nullable: false),
                        is_active = c.Boolean(nullable: false),
                        line_limit = c.Int(),
                        reciept_header = c.String(unicode: false),
                        reciept_body = c.String(unicode: false),
                        reciept_footer = c.String(unicode: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_document)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "payment_type",
                c => new
                    {
                        id_payment_type = c.Int(nullable: false, identity: true),
                        id_document = c.Int(),
                        name = c.String(nullable: false, unicode: false),
                        payment_behavior = c.Int(nullable: false),
                        is_direct = c.Boolean(nullable: false),
                        is_default = c.Boolean(nullable: false),
                        is_active = c.Boolean(nullable: false),
                        has_bank = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_payment_type)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_document", t => t.id_document)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_document)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "payment_type_detail",
                c => new
                    {
                        id_payment_type_detail = c.Int(nullable: false, identity: true),
                        id_payment_type = c.Int(nullable: false),
                        id_payment_detail = c.Int(nullable: false),
                        id_field = c.Short(nullable: false),
                        value = c.String(unicode: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_payment_type_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_field", t => t.id_field, cascadeDelete: true)
                .ForeignKey("payment_detail", t => t.id_payment_detail, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .ForeignKey("payment_type", t => t.id_payment_type, cascadeDelete: true)
                .Index(t => t.id_payment_type)
                .Index(t => t.id_payment_detail)
                .Index(t => t.id_field)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "payment_detail",
                c => new
                    {
                        id_payment_detail = c.Int(nullable: false, identity: true),
                        id_payment = c.Int(),
                        id_sales_return = c.Int(),
                        id_purchase_return = c.Int(),
                        id_account = c.Int(),
                        id_currencyfx = c.Int(nullable: false),
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
                        app_bank_id_bank = c.Int(),
                    })
                .PrimaryKey(t => t.id_payment_detail)                
                .ForeignKey("app_account", t => t.id_account)
                .ForeignKey("app_bank", t => t.app_bank_id_bank)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_currencyfx", t => t.id_currencyfx, cascadeDelete: true)
                .ForeignKey("app_document_range", t => t.id_range)
                .ForeignKey("payments", t => t.id_payment)
                .ForeignKey("payment_type", t => t.id_payment_type, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_payment)
                .Index(t => t.id_account)
                .Index(t => t.id_currencyfx)
                .Index(t => t.id_payment_type)
                .Index(t => t.id_range)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.app_bank_id_bank);
            
            CreateTable(
                "payments",
                c => new
                    {
                        id_payment = c.Int(nullable: false, identity: true),
                        id_weather = c.Int(),
                        id_contact = c.Int(),
                        is_accounted = c.Boolean(nullable: false),
                        id_journal = c.Int(),
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
                .PrimaryKey(t => t.id_payment)                
                .ForeignKey("accounting_journal", t => t.id_journal)
                .ForeignKey("app_branch", t => t.id_branch)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_document_range", t => t.id_range)
                .ForeignKey("app_terminal", t => t.id_terminal)
                .ForeignKey("contacts", t => t.id_contact)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_contact)
                .Index(t => t.id_journal)
                .Index(t => t.id_range)
                .Index(t => t.id_branch)
                .Index(t => t.id_terminal)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "app_terminal",
                c => new
                    {
                        id_terminal = c.Int(nullable: false, identity: true),
                        id_branch = c.Int(nullable: false),
                        name = c.String(nullable: false, unicode: false),
                        code = c.String(unicode: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_terminal)                
                .ForeignKey("app_branch", t => t.id_branch, cascadeDelete: true)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_branch)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "payment_schedual",
                c => new
                    {
                        id_payment_schedual = c.Int(nullable: false, identity: true),
                        id_purchase_invoice = c.Int(),
                        id_purchase_return = c.Int(),
                        id_sales_invoice = c.Int(),
                        id_sales_return = c.Int(),
                        id_sales_order = c.Int(),
                        id_purchase_order = c.Int(),
                        id_note = c.Int(),
                        id_payment_detail = c.Int(),
                        status = c.Int(nullable: false),
                        id_contact = c.Int(nullable: false),
                        id_currencyfx = c.Int(nullable: false),
                        number = c.String(unicode: false),
                        debit = c.Decimal(nullable: false, precision: 20, scale: 9),
                        credit = c.Decimal(nullable: false, precision: 20, scale: 9),
                        can_calculate = c.Boolean(nullable: false),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        expire_date = c.DateTime(nullable: false, precision: 0),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        parent_id_payment_schedual = c.Int(),
                    })
                .PrimaryKey(t => t.id_payment_schedual)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_currencyfx", t => t.id_currencyfx, cascadeDelete: true)
                .ForeignKey("payment_schedual", t => t.parent_id_payment_schedual)
                .ForeignKey("contacts", t => t.id_contact, cascadeDelete: true)
                .ForeignKey("payment_detail", t => t.id_payment_detail)
                .ForeignKey("payment_promissory_note", t => t.id_note)
                .ForeignKey("purchase_invoice", t => t.id_purchase_invoice)
                .ForeignKey("sales_return", t => t.id_sales_return)
                .ForeignKey("sales_invoice", t => t.id_sales_invoice)
                .ForeignKey("purchase_order", t => t.id_purchase_order)
                .ForeignKey("purchase_return", t => t.id_purchase_return)
                .ForeignKey("sales_order", t => t.id_sales_order)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_purchase_invoice)
                .Index(t => t.id_purchase_return)
                .Index(t => t.id_sales_invoice)
                .Index(t => t.id_sales_return)
                .Index(t => t.id_sales_order)
                .Index(t => t.id_purchase_order)
                .Index(t => t.id_note)
                .Index(t => t.id_payment_detail)
                .Index(t => t.id_contact)
                .Index(t => t.id_currencyfx)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.parent_id_payment_schedual);
            
            CreateTable(
                "payment_promissory_note",
                c => new
                    {
                        id_note = c.Int(nullable: false, identity: true),
                        id_contact = c.Int(nullable: false),
                        id_range = c.Int(),
                        note_number = c.String(unicode: false),
                        id_branch = c.Int(nullable: false),
                        id_terminal = c.Int(),
                        status = c.Int(nullable: false),
                        id_currencyfx = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        interest = c.Decimal(nullable: false, precision: 20, scale: 9),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        expiry_date = c.DateTime(nullable: false, precision: 0),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_note)                
                .ForeignKey("app_branch", t => t.id_branch, cascadeDelete: true)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_currencyfx", t => t.id_currencyfx, cascadeDelete: true)
                .ForeignKey("app_document_range", t => t.id_range)
                .ForeignKey("app_terminal", t => t.id_terminal)
                .ForeignKey("contacts", t => t.id_contact, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_contact)
                .Index(t => t.id_range)
                .Index(t => t.id_branch)
                .Index(t => t.id_terminal)
                .Index(t => t.id_currencyfx)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "purchase_invoice",
                c => new
                    {
                        id_purchase_invoice = c.Int(nullable: false, identity: true),
                        id_purchase_order = c.Int(),
                        id_department = c.Int(),
                        id_currencyfx = c.Int(nullable: false),
                        is_accounted = c.Boolean(nullable: false),
                        id_journal = c.Int(),
                        id_contact = c.Int(nullable: false),
                        id_sales_rep = c.Int(),
                        id_weather = c.Int(),
                        id_branch = c.Int(nullable: false),
                        id_terminal = c.Int(),
                        id_contract = c.Int(nullable: false),
                        id_condition = c.Int(nullable: false),
                        id_range = c.Int(),
                        id_project = c.Int(),
                        status = c.Int(nullable: false),
                        number = c.String(unicode: false),
                        code = c.String(unicode: false),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        is_impex = c.Boolean(nullable: false),
                        is_issued = c.Boolean(nullable: false),
                        comment = c.String(unicode: false),
                        barcode = c.String(unicode: false),
                        trans_type = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        contact_id_contact = c.Int(),
                        contact_ref_id_contact = c.Int(),
                        newer_id_purchase_invoice = c.Int(),
                    })
                .PrimaryKey(t => t.id_purchase_invoice)                
                .ForeignKey("accounting_journal", t => t.id_journal)
                .ForeignKey("app_branch", t => t.id_branch, cascadeDelete: true)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_condition", t => t.id_condition, cascadeDelete: true)
                .ForeignKey("app_contract", t => t.id_contract, cascadeDelete: true)
                .ForeignKey("app_currencyfx", t => t.id_currencyfx, cascadeDelete: true)
                .ForeignKey("app_department", t => t.id_department)
                .ForeignKey("app_document_range", t => t.id_range)
                .ForeignKey("app_terminal", t => t.id_terminal)
                .ForeignKey("app_weather", t => t.id_weather)
                .ForeignKey("contacts", t => t.contact_id_contact)
                .ForeignKey("contacts", t => t.contact_ref_id_contact)
                .ForeignKey("purchase_invoice", t => t.newer_id_purchase_invoice)
                .ForeignKey("projects", t => t.id_project)
                .ForeignKey("purchase_order", t => t.id_purchase_order)
                .ForeignKey("sales_rep", t => t.id_sales_rep)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_purchase_order)
                .Index(t => t.id_department)
                .Index(t => t.id_currencyfx)
                .Index(t => t.id_journal)
                .Index(t => t.id_sales_rep)
                .Index(t => t.id_weather)
                .Index(t => t.id_branch)
                .Index(t => t.id_terminal)
                .Index(t => t.id_contract)
                .Index(t => t.id_condition)
                .Index(t => t.id_range)
                .Index(t => t.id_project)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.contact_id_contact)
                .Index(t => t.contact_ref_id_contact)
                .Index(t => t.newer_id_purchase_invoice);
            
            CreateTable(
                "app_weather",
                c => new
                    {
                        id_weather = c.Int(nullable: false, identity: true),
                        id_branch = c.Int(nullable: false),
                        temp = c.Decimal(precision: 20, scale: 9),
                        temp_min = c.Decimal(precision: 20, scale: 9),
                        temp_max = c.Decimal(precision: 20, scale: 9),
                        pressure = c.Decimal(precision: 20, scale: 9),
                        humidity = c.Decimal(precision: 20, scale: 9),
                        wind_speed = c.Decimal(precision: 20, scale: 9),
                        wind_type = c.Decimal(precision: 20, scale: 9),
                        wind_direction = c.Decimal(precision: 20, scale: 9),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.id_weather)                
                .ForeignKey("app_branch", t => t.id_branch, cascadeDelete: true)
                .Index(t => t.id_branch);
            
            CreateTable(
                "payment_withholding_details",
                c => new
                    {
                        id_withholding_detail = c.Int(nullable: false, identity: true),
                        id_withholding = c.Int(nullable: false),
                        id_sales_invoice = c.Int(),
                        id_purchase_invoice = c.Int(),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_withholding_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("sales_invoice", t => t.id_sales_invoice)
                .ForeignKey("payment_withholding_tax", t => t.id_withholding, cascadeDelete: true)
                .ForeignKey("purchase_invoice", t => t.id_purchase_invoice)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_withholding)
                .Index(t => t.id_sales_invoice)
                .Index(t => t.id_purchase_invoice)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "payment_withholding_tax",
                c => new
                    {
                        id_withholding = c.Int(nullable: false, identity: true),
                        status = c.Int(nullable: false),
                        id_contact = c.Int(nullable: false),
                        id_range = c.Int(),
                        id_currencyfx = c.Int(nullable: false),
                        id_journal = c.Int(),
                        withholding_number = c.String(unicode: false),
                        code = c.String(unicode: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        expire_date = c.DateTime(nullable: false, precision: 0),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_withholding)                
                .ForeignKey("accounting_journal", t => t.id_journal)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_currencyfx", t => t.id_currencyfx, cascadeDelete: true)
                .ForeignKey("app_document_range", t => t.id_range)
                .ForeignKey("contacts", t => t.id_contact, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_contact)
                .Index(t => t.id_range)
                .Index(t => t.id_currencyfx)
                .Index(t => t.id_journal)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "payment_withholding_detail",
                c => new
                    {
                        id_withholding_detail = c.Int(nullable: false, identity: true),
                        id_withholding = c.Int(nullable: false),
                        id_sales_invoice = c.Int(nullable: false),
                        id_purchase_invoice = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_withholding_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("payment_withholding_tax", t => t.id_withholding, cascadeDelete: true)
                .ForeignKey("purchase_invoice", t => t.id_purchase_invoice, cascadeDelete: true)
                .ForeignKey("sales_invoice", t => t.id_sales_invoice, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_withholding)
                .Index(t => t.id_sales_invoice)
                .Index(t => t.id_purchase_invoice)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "sales_invoice",
                c => new
                    {
                        id_sales_invoice = c.Int(nullable: false, identity: true),
                        id_sales_order = c.Int(),
                        id_opportunity = c.Int(nullable: false),
                        is_accounted = c.Boolean(nullable: false),
                        id_journal = c.Int(),
                        id_currencyfx = c.Int(nullable: false),
                        id_contact = c.Int(nullable: false),
                        id_sales_rep = c.Int(),
                        id_weather = c.Int(),
                        id_branch = c.Int(nullable: false),
                        id_terminal = c.Int(),
                        id_contract = c.Int(nullable: false),
                        id_condition = c.Int(nullable: false),
                        id_range = c.Int(),
                        id_project = c.Int(),
                        status = c.Int(nullable: false),
                        number = c.String(unicode: false),
                        code = c.String(unicode: false),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        is_impex = c.Boolean(nullable: false),
                        is_issued = c.Boolean(nullable: false),
                        comment = c.String(unicode: false),
                        barcode = c.String(unicode: false),
                        trans_type = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        contact_id_contact = c.Int(),
                        contact_ref_id_contact = c.Int(),
                        newer_id_sales_invoice = c.Int(),
                    })
                .PrimaryKey(t => t.id_sales_invoice)                
                .ForeignKey("accounting_journal", t => t.id_journal)
                .ForeignKey("app_branch", t => t.id_branch, cascadeDelete: true)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_condition", t => t.id_condition, cascadeDelete: true)
                .ForeignKey("app_contract", t => t.id_contract, cascadeDelete: true)
                .ForeignKey("app_currencyfx", t => t.id_currencyfx, cascadeDelete: true)
                .ForeignKey("app_document_range", t => t.id_range)
                .ForeignKey("app_terminal", t => t.id_terminal)
                .ForeignKey("app_weather", t => t.id_weather)
                .ForeignKey("contacts", t => t.contact_id_contact)
                .ForeignKey("contacts", t => t.contact_ref_id_contact)
                .ForeignKey("sales_order", t => t.id_sales_order)
                .ForeignKey("crm_opportunity", t => t.id_opportunity, cascadeDelete: true)
                .ForeignKey("sales_invoice", t => t.newer_id_sales_invoice)
                .ForeignKey("projects", t => t.id_project)
                .ForeignKey("sales_rep", t => t.id_sales_rep)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_sales_order)
                .Index(t => t.id_opportunity)
                .Index(t => t.id_journal)
                .Index(t => t.id_currencyfx)
                .Index(t => t.id_sales_rep)
                .Index(t => t.id_weather)
                .Index(t => t.id_branch)
                .Index(t => t.id_terminal)
                .Index(t => t.id_contract)
                .Index(t => t.id_condition)
                .Index(t => t.id_range)
                .Index(t => t.id_project)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.contact_id_contact)
                .Index(t => t.contact_ref_id_contact)
                .Index(t => t.newer_id_sales_invoice);
            
            CreateTable(
                "crm_opportunity",
                c => new
                    {
                        id_opportunity = c.Int(nullable: false, identity: true),
                        id_contact = c.Int(nullable: false),
                        id_currency = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_opportunity)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("contacts", t => t.id_contact, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_contact)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "sales_budget",
                c => new
                    {
                        id_sales_budget = c.Int(nullable: false, identity: true),
                        id_opportunity = c.Int(nullable: false),
                        id_currencyfx = c.Int(nullable: false),
                        delivery_date = c.DateTime(precision: 0),
                        valid_date = c.DateTime(precision: 0),
                        id_contact = c.Int(nullable: false),
                        id_sales_rep = c.Int(),
                        id_weather = c.Int(),
                        id_branch = c.Int(nullable: false),
                        id_terminal = c.Int(),
                        id_contract = c.Int(nullable: false),
                        id_condition = c.Int(nullable: false),
                        id_range = c.Int(),
                        id_project = c.Int(),
                        status = c.Int(nullable: false),
                        number = c.String(unicode: false),
                        code = c.String(unicode: false),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        is_impex = c.Boolean(nullable: false),
                        is_issued = c.Boolean(nullable: false),
                        comment = c.String(unicode: false),
                        barcode = c.String(unicode: false),
                        trans_type = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        contact_id_contact = c.Int(),
                        contact_ref_id_contact = c.Int(),
                        newer_id_sales_budget = c.Int(),
                    })
                .PrimaryKey(t => t.id_sales_budget)                
                .ForeignKey("app_branch", t => t.id_branch, cascadeDelete: true)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_condition", t => t.id_condition, cascadeDelete: true)
                .ForeignKey("app_contract", t => t.id_contract, cascadeDelete: true)
                .ForeignKey("app_currencyfx", t => t.id_currencyfx, cascadeDelete: true)
                .ForeignKey("app_document_range", t => t.id_range)
                .ForeignKey("app_terminal", t => t.id_terminal)
                .ForeignKey("app_weather", t => t.id_weather)
                .ForeignKey("contacts", t => t.contact_id_contact)
                .ForeignKey("contacts", t => t.contact_ref_id_contact)
                .ForeignKey("crm_opportunity", t => t.id_opportunity, cascadeDelete: true)
                .ForeignKey("sales_budget", t => t.newer_id_sales_budget)
                .ForeignKey("projects", t => t.id_project)
                .ForeignKey("sales_rep", t => t.id_sales_rep)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_opportunity)
                .Index(t => t.id_currencyfx)
                .Index(t => t.id_sales_rep)
                .Index(t => t.id_weather)
                .Index(t => t.id_branch)
                .Index(t => t.id_terminal)
                .Index(t => t.id_contract)
                .Index(t => t.id_condition)
                .Index(t => t.id_range)
                .Index(t => t.id_project)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.contact_id_contact)
                .Index(t => t.contact_ref_id_contact)
                .Index(t => t.newer_id_sales_budget);
            
            CreateTable(
                "projects",
                c => new
                    {
                        id_project = c.Int(nullable: false, identity: true),
                        id_project_template = c.Int(),
                        id_branch = c.Int(),
                        id_contact = c.Int(),
                        id_currency = c.Int(),
                        name = c.String(nullable: false, unicode: false),
                        code = c.String(unicode: false),
                        comment = c.String(unicode: false),
                        est_start_date = c.DateTime(precision: 0),
                        est_end_date = c.DateTime(precision: 0),
                        priority = c.Int(nullable: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_project)                
                .ForeignKey("app_branch", t => t.id_branch)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_currency", t => t.id_currency)
                .ForeignKey("contacts", t => t.id_contact)
                .ForeignKey("project_template", t => t.id_project_template)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_project_template)
                .Index(t => t.id_branch)
                .Index(t => t.id_contact)
                .Index(t => t.id_currency)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "production_order",
                c => new
                    {
                        id_production_order = c.Int(nullable: false, identity: true),
                        id_production_line = c.Int(nullable: false),
                        id_weather = c.Int(),
                        id_project = c.Int(),
                        id_branch = c.Int(nullable: false),
                        id_terminal = c.Int(nullable: false),
                        id_range = c.Int(),
                        work_number = c.String(unicode: false),
                        project_cost_center = c.String(unicode: false),
                        status = c.Int(),
                        name = c.String(unicode: false),
                        barcode = c.String(unicode: false),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        start_date_est = c.DateTime(precision: 0),
                        end_date_est = c.DateTime(precision: 0),
                        types = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_production_order)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_document_range", t => t.id_range)
                .ForeignKey("production_line", t => t.id_production_line, cascadeDelete: true)
                .ForeignKey("projects", t => t.id_project)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_production_line)
                .Index(t => t.id_project)
                .Index(t => t.id_range)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "production_line",
                c => new
                    {
                        id_production_line = c.Int(nullable: false, identity: true),
                        id_location = c.Int(nullable: false),
                        name = c.String(unicode: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_production_line)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_location", t => t.id_location, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_location)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "production_order_detail",
                c => new
                    {
                        id_order_detail = c.Int(nullable: false, identity: true),
                        id_production_order = c.Int(nullable: false),
                        id_project_task = c.Int(),
                        id_item = c.Int(),
                        movement_id = c.Int(),
                        name = c.String(unicode: false),
                        quantity = c.Decimal(nullable: false, precision: 20, scale: 9),
                        status = c.Int(),
                        code = c.String(unicode: false),
                        is_input = c.Boolean(nullable: false),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        start_date_est = c.DateTime(precision: 0),
                        end_date_est = c.DateTime(precision: 0),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        parent_id_order_detail = c.Int(),
                    })
                .PrimaryKey(t => t.id_order_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("production_order_detail", t => t.parent_id_order_detail)
                .ForeignKey("items", t => t.id_item)
                .ForeignKey("production_order", t => t.id_production_order, cascadeDelete: true)
                .ForeignKey("project_task", t => t.id_project_task)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_production_order)
                .Index(t => t.id_project_task)
                .Index(t => t.id_item)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.parent_id_order_detail);
            
            CreateTable(
                "item_request_detail",
                c => new
                    {
                        id_item_request_detail = c.Int(nullable: false, identity: true),
                        id_item_request = c.Int(nullable: false),
                        id_project_task = c.Int(),
                        id_sales_order_detail = c.Int(),
                        id_order_detail = c.Int(),
                        id_maintainance_detail = c.Int(),
                        id_item = c.Int(nullable: false),
                        max_value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        quantity = c.Decimal(nullable: false, precision: 20, scale: 9),
                        date_needed_by = c.DateTime(nullable: false, precision: 0),
                        comment = c.String(unicode: false),
                        urgency = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_item_request_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("items", t => t.id_item, cascadeDelete: true)
                .ForeignKey("item_asset_maintainance_detail", t => t.id_maintainance_detail)
                .ForeignKey("item_request", t => t.id_item_request, cascadeDelete: true)
                .ForeignKey("production_order_detail", t => t.id_order_detail)
                .ForeignKey("project_task", t => t.id_project_task)
                .ForeignKey("sales_order_detail", t => t.id_sales_order_detail)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_item_request)
                .Index(t => t.id_project_task)
                .Index(t => t.id_sales_order_detail)
                .Index(t => t.id_order_detail)
                .Index(t => t.id_maintainance_detail)
                .Index(t => t.id_item)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_asset_maintainance_detail",
                c => new
                    {
                        id_maintainance_detail = c.Int(nullable: false, identity: true),
                        id_maintainance = c.Int(nullable: false),
                        id_item = c.Int(),
                        item_description = c.String(unicode: false),
                        quantity = c.Decimal(nullable: false, precision: 20, scale: 9),
                        unit_cost = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_currencyfx = c.Int(),
                        id_time_coefficient = c.Int(),
                        id_contact = c.Int(),
                        start_date = c.DateTime(nullable: false, precision: 0),
                        end_date = c.DateTime(nullable: false, precision: 0),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_maintainance_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_currencyfx", t => t.id_currencyfx)
                .ForeignKey("contacts", t => t.id_contact)
                .ForeignKey("hr_time_coefficient", t => t.id_time_coefficient)
                .ForeignKey("items", t => t.id_item)
                .ForeignKey("item_asset_maintainance", t => t.id_maintainance, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_maintainance)
                .Index(t => t.id_item)
                .Index(t => t.id_currencyfx)
                .Index(t => t.id_time_coefficient)
                .Index(t => t.id_contact)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "hr_time_coefficient",
                c => new
                    {
                        id_time_coefficient = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, unicode: false),
                        coefficient = c.Decimal(nullable: false, precision: 20, scale: 9),
                        start_time = c.DateTime(nullable: false, precision: 0),
                        end_time = c.DateTime(nullable: false, precision: 0),
                        weekend_only = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_time_coefficient)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_asset_maintainance",
                c => new
                    {
                        id_maintainance = c.Int(nullable: false, identity: true),
                        id_item_asset = c.Int(nullable: false),
                        status = c.Int(nullable: false),
                        start_date = c.DateTime(nullable: false, precision: 0),
                        end_date = c.DateTime(nullable: false, precision: 0),
                        comment = c.String(unicode: false),
                        maintainance_type = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_maintainance)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("item_asset", t => t.id_item_asset, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_item_asset)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_asset",
                c => new
                    {
                        id_item_asset = c.Int(nullable: false, identity: true),
                        id_item = c.Int(nullable: false),
                        id_branch = c.Int(),
                        id_item_asset_group = c.Int(),
                        manufacture_date = c.DateTime(precision: 0),
                        purchase_date = c.DateTime(precision: 0),
                        purchase_value = c.Decimal(precision: 20, scale: 9),
                        current_value = c.Decimal(precision: 20, scale: 9),
                        id_department = c.Int(),
                        id_contact = c.Int(),
                        speed = c.Decimal(precision: 20, scale: 9),
                        deactivetype = c.Int(nullable: false),
                        dieset_price = c.Decimal(precision: 20, scale: 9),
                        min_length = c.Decimal(precision: 20, scale: 9),
                        max_length = c.Decimal(precision: 20, scale: 9),
                        min_width = c.Decimal(precision: 20, scale: 9),
                        max_width = c.Decimal(precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_item_asset)                
                .ForeignKey("app_branch", t => t.id_branch)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("contacts", t => t.id_contact)
                .ForeignKey("items", t => t.id_item, cascadeDelete: true)
                .ForeignKey("item_asset_group", t => t.id_item_asset_group)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_item)
                .Index(t => t.id_branch)
                .Index(t => t.id_item_asset_group)
                .Index(t => t.id_contact)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_asset_group",
                c => new
                    {
                        id_item_asset_group = c.Int(nullable: false, identity: true),
                        name = c.String(unicode: false),
                        depreciation_rate = c.Decimal(precision: 20, scale: 9),
                        depreciation_run = c.DateTime(precision: 0),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_item_asset_group)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_request_decision",
                c => new
                    {
                        id_item_request_decision = c.Int(nullable: false, identity: true),
                        id_item_request_detail = c.Int(nullable: false),
                        id_location = c.Int(),
                        decision = c.Int(nullable: false),
                        quantity = c.Decimal(nullable: false, precision: 20, scale: 9),
                        movement_id = c.Int(),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_item_request_decision)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_location", t => t.id_location)
                .ForeignKey("item_request_detail", t => t.id_item_request_detail, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_item_request_detail)
                .Index(t => t.id_location)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_request_dimension",
                c => new
                    {
                        id_request_property = c.Long(nullable: false, identity: true),
                        id_item_request_detail = c.Long(nullable: false),
                        id_dimension = c.Int(nullable: false),
                        id_measurement = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        item_request_detail_id_item_request_detail = c.Int(),
                    })
                .PrimaryKey(t => t.id_request_property)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_dimension", t => t.id_dimension, cascadeDelete: true)
                .ForeignKey("app_measurement", t => t.id_measurement, cascadeDelete: true)
                .ForeignKey("item_request_detail", t => t.item_request_detail_id_item_request_detail)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_dimension)
                .Index(t => t.id_measurement)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.item_request_detail_id_item_request_detail);
            
            CreateTable(
                "sales_order_detail",
                c => new
                    {
                        id_sales_order_detail = c.Int(nullable: false, identity: true),
                        id_sales_order = c.Int(nullable: false),
                        movement_id = c.Int(),
                        id_sales_budget_detail = c.Int(),
                        id_location = c.Int(),
                        id_project_task = c.Int(),
                        id_item = c.Int(nullable: false),
                        item_description = c.String(unicode: false),
                        quantity = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_vat_group = c.Int(nullable: false),
                        unit_price = c.Decimal(nullable: false, precision: 20, scale: 9),
                        unit_cost = c.Decimal(nullable: false, precision: 20, scale: 9),
                        comment = c.String(unicode: false),
                        discount = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        project_task_id_project_task = c.Int(),
                        project_task_id_project_task1 = c.Int(),
                    })
                .PrimaryKey(t => t.id_sales_order_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_location", t => t.id_location)
                .ForeignKey("app_vat_group", t => t.id_vat_group, cascadeDelete: true)
                .ForeignKey("items", t => t.id_item, cascadeDelete: true)
                .ForeignKey("project_task", t => t.project_task_id_project_task)
                .ForeignKey("sales_budget_detail", t => t.id_sales_budget_detail)
                .ForeignKey("sales_order", t => t.id_sales_order, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .ForeignKey("project_task", t => t.project_task_id_project_task1)
                .Index(t => t.id_sales_order)
                .Index(t => t.id_sales_budget_detail)
                .Index(t => t.id_location)
                .Index(t => t.id_item)
                .Index(t => t.id_vat_group)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.project_task_id_project_task)
                .Index(t => t.project_task_id_project_task1);
            
            CreateTable(
                "app_vat_group",
                c => new
                    {
                        id_vat_group = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, unicode: false),
                        is_active = c.Boolean(nullable: false),
                        is_default = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_vat_group)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "app_vat_group_details",
                c => new
                    {
                        id_vat_group_detail = c.Int(nullable: false, identity: true),
                        id_vat_group = c.Int(nullable: false),
                        id_vat = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_vat_group_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_vat", t => t.id_vat, cascadeDelete: true)
                .ForeignKey("app_vat_group", t => t.id_vat_group, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_vat_group)
                .Index(t => t.id_vat)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "app_vat",
                c => new
                    {
                        id_vat = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, unicode: false),
                        coefficient = c.Decimal(nullable: false, precision: 20, scale: 9),
                        on_product = c.Boolean(nullable: false),
                        on_branch = c.Boolean(nullable: false),
                        on_destination = c.Boolean(nullable: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_vat)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "sales_budget_detail",
                c => new
                    {
                        id_sales_budget_detail = c.Int(nullable: false, identity: true),
                        id_sales_budget = c.Int(nullable: false),
                        id_location = c.Int(),
                        id_project_task = c.Int(),
                        id_item = c.Int(nullable: false),
                        item_description = c.String(unicode: false),
                        quantity = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_vat_group = c.Int(nullable: false),
                        unit_price = c.Decimal(nullable: false, precision: 20, scale: 9),
                        unit_cost = c.Decimal(nullable: false, precision: 20, scale: 9),
                        comment = c.String(unicode: false),
                        discount = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_sales_budget_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_location", t => t.id_location)
                .ForeignKey("app_vat_group", t => t.id_vat_group, cascadeDelete: true)
                .ForeignKey("items", t => t.id_item, cascadeDelete: true)
                .ForeignKey("project_task", t => t.id_project_task)
                .ForeignKey("sales_budget", t => t.id_sales_budget, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_sales_budget)
                .Index(t => t.id_location)
                .Index(t => t.id_project_task)
                .Index(t => t.id_item)
                .Index(t => t.id_vat_group)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "sales_invoice_detail",
                c => new
                    {
                        id_sales_invoice_detail = c.Int(nullable: false, identity: true),
                        id_sales_invoice = c.Int(nullable: false),
                        movement_id = c.Int(),
                        id_sales_order_detail = c.Int(),
                        id_location = c.Int(),
                        id_project_task = c.Int(),
                        id_item = c.Int(nullable: false),
                        item_description = c.String(unicode: false),
                        quantity = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_vat_group = c.Int(nullable: false),
                        unit_price = c.Decimal(nullable: false, precision: 20, scale: 9),
                        unit_cost = c.Decimal(nullable: false, precision: 20, scale: 9),
                        comment = c.String(unicode: false),
                        discount = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_sales_invoice_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_location", t => t.id_location)
                .ForeignKey("app_vat_group", t => t.id_vat_group, cascadeDelete: true)
                .ForeignKey("items", t => t.id_item, cascadeDelete: true)
                .ForeignKey("project_task", t => t.id_project_task)
                .ForeignKey("sales_invoice", t => t.id_sales_invoice, cascadeDelete: true)
                .ForeignKey("sales_order_detail", t => t.id_sales_order_detail)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_sales_invoice)
                .Index(t => t.id_sales_order_detail)
                .Index(t => t.id_location)
                .Index(t => t.id_project_task)
                .Index(t => t.id_item)
                .Index(t => t.id_vat_group)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "sales_packing_relation",
                c => new
                    {
                        id_sales_packing_relation = c.Int(nullable: false, identity: true),
                        id_sales_invoice_detail = c.Long(nullable: false),
                        id_sales_packing_detail = c.Int(nullable: false),
                        sales_invoice_detail_id_sales_invoice_detail = c.Int(),
                    })
                .PrimaryKey(t => t.id_sales_packing_relation)                
                .ForeignKey("sales_invoice_detail", t => t.sales_invoice_detail_id_sales_invoice_detail)
                .ForeignKey("sales_packing_detail", t => t.id_sales_packing_detail, cascadeDelete: true)
                .Index(t => t.id_sales_packing_detail)
                .Index(t => t.sales_invoice_detail_id_sales_invoice_detail);
            
            CreateTable(
                "sales_packing_detail",
                c => new
                    {
                        id_sales_packing_detail = c.Int(nullable: false, identity: true),
                        id_sales_packing = c.Int(nullable: false),
                        id_sales_order_detail = c.Int(),
                        id_location = c.Int(),
                        id_item = c.Int(nullable: false),
                        quantity = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_sales_packing_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_location", t => t.id_location)
                .ForeignKey("items", t => t.id_item, cascadeDelete: true)
                .ForeignKey("sales_order_detail", t => t.id_sales_order_detail)
                .ForeignKey("sales_packing", t => t.id_sales_packing, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_sales_packing)
                .Index(t => t.id_sales_order_detail)
                .Index(t => t.id_location)
                .Index(t => t.id_item)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "sales_packing",
                c => new
                    {
                        id_sales_packing = c.Int(nullable: false, identity: true),
                        id_opportunity = c.Int(nullable: false),
                        id_contact = c.Int(nullable: false),
                        id_branch = c.Int(nullable: false),
                        id_terminal = c.Int(nullable: false),
                        id_range = c.Int(),
                        number = c.String(unicode: false),
                        comment = c.String(unicode: false),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        packing_type = c.Int(nullable: false),
                        status = c.Int(nullable: false),
                        is_issued = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        newer_id_sales_packing = c.Int(),
                    })
                .PrimaryKey(t => t.id_sales_packing)                
                .ForeignKey("app_branch", t => t.id_branch, cascadeDelete: true)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_document_range", t => t.id_range)
                .ForeignKey("app_terminal", t => t.id_terminal, cascadeDelete: true)
                .ForeignKey("contacts", t => t.id_contact, cascadeDelete: true)
                .ForeignKey("crm_opportunity", t => t.id_opportunity, cascadeDelete: true)
                .ForeignKey("sales_packing", t => t.newer_id_sales_packing)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_opportunity)
                .Index(t => t.id_contact)
                .Index(t => t.id_branch)
                .Index(t => t.id_terminal)
                .Index(t => t.id_range)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.newer_id_sales_packing);
            
            CreateTable(
                "sales_order",
                c => new
                    {
                        id_sales_order = c.Int(nullable: false, identity: true),
                        id_sales_budget = c.Int(),
                        id_opportunity = c.Int(nullable: false),
                        id_currencyfx = c.Int(nullable: false),
                        delivery_date = c.DateTime(precision: 0),
                        valid_date = c.DateTime(precision: 0),
                        id_contact = c.Int(nullable: false),
                        id_sales_rep = c.Int(),
                        id_weather = c.Int(),
                        id_branch = c.Int(nullable: false),
                        id_terminal = c.Int(),
                        id_contract = c.Int(nullable: false),
                        id_condition = c.Int(nullable: false),
                        id_range = c.Int(),
                        id_project = c.Int(),
                        status = c.Int(nullable: false),
                        number = c.String(unicode: false),
                        code = c.String(unicode: false),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        is_impex = c.Boolean(nullable: false),
                        is_issued = c.Boolean(nullable: false),
                        comment = c.String(unicode: false),
                        barcode = c.String(unicode: false),
                        trans_type = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        contact_id_contact = c.Int(),
                        contact_ref_id_contact = c.Int(),
                        newer_id_sales_order = c.Int(),
                    })
                .PrimaryKey(t => t.id_sales_order)                
                .ForeignKey("app_branch", t => t.id_branch, cascadeDelete: true)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_condition", t => t.id_condition, cascadeDelete: true)
                .ForeignKey("app_contract", t => t.id_contract, cascadeDelete: true)
                .ForeignKey("app_currencyfx", t => t.id_currencyfx, cascadeDelete: true)
                .ForeignKey("app_document_range", t => t.id_range)
                .ForeignKey("app_terminal", t => t.id_terminal)
                .ForeignKey("app_weather", t => t.id_weather)
                .ForeignKey("contacts", t => t.contact_id_contact)
                .ForeignKey("contacts", t => t.contact_ref_id_contact)
                .ForeignKey("crm_opportunity", t => t.id_opportunity, cascadeDelete: true)
                .ForeignKey("sales_order", t => t.newer_id_sales_order)
                .ForeignKey("projects", t => t.id_project)
                .ForeignKey("sales_budget", t => t.id_sales_budget)
                .ForeignKey("sales_rep", t => t.id_sales_rep)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_sales_budget)
                .Index(t => t.id_opportunity)
                .Index(t => t.id_currencyfx)
                .Index(t => t.id_sales_rep)
                .Index(t => t.id_weather)
                .Index(t => t.id_branch)
                .Index(t => t.id_terminal)
                .Index(t => t.id_contract)
                .Index(t => t.id_condition)
                .Index(t => t.id_range)
                .Index(t => t.id_project)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.contact_id_contact)
                .Index(t => t.contact_ref_id_contact)
                .Index(t => t.newer_id_sales_order);
            
            CreateTable(
                "sales_rep",
                c => new
                    {
                        id_sales_rep = c.Int(nullable: false, identity: true),
                        id_contact = c.Int(nullable: false),
                        enum_type = c.Int(nullable: false),
                        name = c.String(nullable: false, unicode: false),
                        code = c.String(unicode: false),
                        commision_base = c.Decimal(precision: 20, scale: 9),
                        commision_calculation = c.String(unicode: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_sales_rep)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "production_execution_detail",
                c => new
                    {
                        id_execution_detail = c.Int(nullable: false, identity: true),
                        id_order_detail = c.Int(),
                        id_project_task = c.Int(),
                        id_time_coefficient = c.Int(),
                        id_contact = c.Int(),
                        id_item = c.Int(),
                        movement_id = c.Int(),
                        name = c.String(unicode: false),
                        status = c.Int(),
                        quantity = c.Decimal(nullable: false, precision: 20, scale: 9),
                        start_date = c.DateTime(nullable: false, precision: 0),
                        end_date = c.DateTime(nullable: false, precision: 0),
                        unit_cost = c.Decimal(nullable: false, precision: 20, scale: 9),
                        is_input = c.Boolean(nullable: false),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        parent_id_execution_detail = c.Int(),
                    })
                .PrimaryKey(t => t.id_execution_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("production_execution_detail", t => t.parent_id_execution_detail)
                .ForeignKey("contacts", t => t.id_contact)
                .ForeignKey("hr_time_coefficient", t => t.id_time_coefficient)
                .ForeignKey("items", t => t.id_item)
                .ForeignKey("production_order_detail", t => t.id_order_detail)
                .ForeignKey("project_task", t => t.id_project_task)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_order_detail)
                .Index(t => t.id_project_task)
                .Index(t => t.id_time_coefficient)
                .Index(t => t.id_contact)
                .Index(t => t.id_item)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.parent_id_execution_detail);
            
            CreateTable(
                "production_execution_dimension",
                c => new
                    {
                        id_execution_dimension = c.Int(nullable: false, identity: true),
                        id_execution_detail = c.Int(nullable: false),
                        id_dimension = c.Int(nullable: false),
                        id_measurement = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_execution_dimension)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_dimension", t => t.id_dimension, cascadeDelete: true)
                .ForeignKey("app_measurement", t => t.id_measurement, cascadeDelete: true)
                .ForeignKey("production_execution_detail", t => t.id_execution_detail, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_execution_detail)
                .Index(t => t.id_dimension)
                .Index(t => t.id_measurement)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "production_order_dimension",
                c => new
                    {
                        id_order_dimension = c.Int(nullable: false, identity: true),
                        id_order_detail = c.Int(nullable: false),
                        id_dimension = c.Int(nullable: false),
                        id_measurement = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_order_dimension)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_dimension", t => t.id_dimension, cascadeDelete: true)
                .ForeignKey("app_measurement", t => t.id_measurement, cascadeDelete: true)
                .ForeignKey("production_order_detail", t => t.id_order_detail, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_order_detail)
                .Index(t => t.id_dimension)
                .Index(t => t.id_measurement)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "project_tag_detail",
                c => new
                    {
                        id_project_tag_detail = c.Int(nullable: false, identity: true),
                        id_project = c.Int(nullable: false),
                        id_tag = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_project_tag_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("projects", t => t.id_project, cascadeDelete: true)
                .ForeignKey("project_tag", t => t.id_tag, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_project)
                .Index(t => t.id_tag)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "project_tag",
                c => new
                    {
                        id_tag = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, unicode: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_tag)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "project_template",
                c => new
                    {
                        id_project_template = c.Int(nullable: false, identity: true),
                        id_item_output = c.Int(nullable: false),
                        name = c.String(unicode: false),
                        code = c.String(unicode: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_project_template)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "project_template_detail",
                c => new
                    {
                        id_template_detail = c.Int(nullable: false, identity: true),
                        id_project_template = c.Int(nullable: false),
                        sequence = c.Int(),
                        id_item = c.Int(),
                        status = c.Int(),
                        id_tag = c.Int(),
                        item_description = c.String(unicode: false),
                        code = c.String(unicode: false),
                        logic = c.String(unicode: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        parent_id_template_detail = c.Int(),
                    })
                .PrimaryKey(t => t.id_template_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("project_template_detail", t => t.parent_id_template_detail)
                .ForeignKey("items", t => t.id_item)
                .ForeignKey("item_tag", t => t.id_tag)
                .ForeignKey("project_template", t => t.id_project_template, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_project_template)
                .Index(t => t.id_item)
                .Index(t => t.id_tag)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.parent_id_template_detail);
            
            CreateTable(
                "item_tag",
                c => new
                    {
                        id_tag = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, unicode: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_tag)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_tag_detail",
                c => new
                    {
                        id_item_tag_detail = c.Int(nullable: false, identity: true),
                        id_item = c.Int(nullable: false),
                        id_tag = c.Int(nullable: false),
                        is_default = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_item_tag_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("items", t => t.id_item, cascadeDelete: true)
                .ForeignKey("item_tag", t => t.id_tag, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_item)
                .Index(t => t.id_tag)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "sales_return",
                c => new
                    {
                        id_sales_return = c.Int(nullable: false, identity: true),
                        id_opportunity = c.Int(nullable: false),
                        id_sales_invoice = c.Int(),
                        return_type = c.Int(nullable: false),
                        id_currencyfx = c.Int(nullable: false),
                        is_accounted = c.Boolean(nullable: false),
                        id_journal = c.Int(),
                        id_contact = c.Int(nullable: false),
                        id_sales_rep = c.Int(),
                        id_weather = c.Int(),
                        id_branch = c.Int(nullable: false),
                        id_terminal = c.Int(),
                        id_contract = c.Int(nullable: false),
                        id_condition = c.Int(nullable: false),
                        id_range = c.Int(),
                        id_project = c.Int(),
                        status = c.Int(nullable: false),
                        number = c.String(unicode: false),
                        code = c.String(unicode: false),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        is_impex = c.Boolean(nullable: false),
                        is_issued = c.Boolean(nullable: false),
                        comment = c.String(unicode: false),
                        barcode = c.String(unicode: false),
                        trans_type = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        contact_id_contact = c.Int(),
                        contact_ref_id_contact = c.Int(),
                        newer_id_sales_return = c.Int(),
                    })
                .PrimaryKey(t => t.id_sales_return)                
                .ForeignKey("accounting_journal", t => t.id_journal)
                .ForeignKey("app_branch", t => t.id_branch, cascadeDelete: true)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_condition", t => t.id_condition, cascadeDelete: true)
                .ForeignKey("app_contract", t => t.id_contract, cascadeDelete: true)
                .ForeignKey("app_currencyfx", t => t.id_currencyfx, cascadeDelete: true)
                .ForeignKey("app_document_range", t => t.id_range)
                .ForeignKey("app_terminal", t => t.id_terminal)
                .ForeignKey("app_weather", t => t.id_weather)
                .ForeignKey("contacts", t => t.contact_id_contact)
                .ForeignKey("contacts", t => t.contact_ref_id_contact)
                .ForeignKey("crm_opportunity", t => t.id_opportunity, cascadeDelete: true)
                .ForeignKey("sales_return", t => t.newer_id_sales_return)
                .ForeignKey("projects", t => t.id_project)
                .ForeignKey("sales_invoice", t => t.id_sales_invoice)
                .ForeignKey("sales_rep", t => t.id_sales_rep)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_opportunity)
                .Index(t => t.id_sales_invoice)
                .Index(t => t.id_currencyfx)
                .Index(t => t.id_journal)
                .Index(t => t.id_sales_rep)
                .Index(t => t.id_weather)
                .Index(t => t.id_branch)
                .Index(t => t.id_terminal)
                .Index(t => t.id_contract)
                .Index(t => t.id_condition)
                .Index(t => t.id_range)
                .Index(t => t.id_project)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.contact_id_contact)
                .Index(t => t.contact_ref_id_contact)
                .Index(t => t.newer_id_sales_return);
            
            CreateTable(
                "sales_return_detail",
                c => new
                    {
                        id_sales_return_detail = c.Int(nullable: false, identity: true),
                        id_sales_return = c.Int(nullable: false),
                        id_sales_invoice_detail = c.Long(),
                        has_return = c.Boolean(nullable: false),
                        id_location = c.Int(),
                        id_project_task = c.Int(),
                        id_item = c.Int(nullable: false),
                        item_description = c.String(unicode: false),
                        quantity = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_vat_group = c.Int(nullable: false),
                        unit_price = c.Decimal(nullable: false, precision: 20, scale: 9),
                        unit_cost = c.Decimal(nullable: false, precision: 20, scale: 9),
                        comment = c.String(unicode: false),
                        discount = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        sales_invoice_detail_id_sales_invoice_detail = c.Int(),
                    })
                .PrimaryKey(t => t.id_sales_return_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_location", t => t.id_location)
                .ForeignKey("app_vat_group", t => t.id_vat_group, cascadeDelete: true)
                .ForeignKey("items", t => t.id_item, cascadeDelete: true)
                .ForeignKey("project_task", t => t.id_project_task)
                .ForeignKey("sales_invoice_detail", t => t.sales_invoice_detail_id_sales_invoice_detail)
                .ForeignKey("sales_return", t => t.id_sales_return, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_sales_return)
                .Index(t => t.id_location)
                .Index(t => t.id_project_task)
                .Index(t => t.id_item)
                .Index(t => t.id_vat_group)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.sales_invoice_detail_id_sales_invoice_detail);
            
            CreateTable(
                "purchase_invoice_detail",
                c => new
                    {
                        id_purchase_invoice_detail = c.Int(nullable: false, identity: true),
                        id_purchase_invoice = c.Int(nullable: false),
                        id_purchase_order_detail = c.Int(),
                        id_location = c.Int(),
                        id_cost_center = c.Int(nullable: false),
                        id_item = c.Int(),
                        item_description = c.String(unicode: false),
                        quantity = c.Decimal(nullable: false, precision: 20, scale: 9),
                        lot_number = c.String(unicode: false),
                        expiration_date = c.DateTime(precision: 0),
                        id_vat_group = c.Int(nullable: false),
                        unit_cost = c.Decimal(nullable: false, precision: 20, scale: 9),
                        comment = c.String(unicode: false),
                        discount = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        project_task_id_project_task = c.Int(),
                    })
                .PrimaryKey(t => t.id_purchase_invoice_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_cost_center", t => t.id_cost_center, cascadeDelete: true)
                .ForeignKey("app_location", t => t.id_location)
                .ForeignKey("app_vat_group", t => t.id_vat_group, cascadeDelete: true)
                .ForeignKey("items", t => t.id_item)
                .ForeignKey("purchase_invoice", t => t.id_purchase_invoice, cascadeDelete: true)
                .ForeignKey("purchase_order_detail", t => t.id_purchase_order_detail)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .ForeignKey("project_task", t => t.project_task_id_project_task)
                .Index(t => t.id_purchase_invoice)
                .Index(t => t.id_purchase_order_detail)
                .Index(t => t.id_location)
                .Index(t => t.id_cost_center)
                .Index(t => t.id_item)
                .Index(t => t.id_vat_group)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.project_task_id_project_task);
            
            CreateTable(
                "purchase_invoice_dimension",
                c => new
                    {
                        id_invoice_property = c.Long(nullable: false, identity: true),
                        id_purchase_invoice_detail = c.Long(nullable: false),
                        id_dimension = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_measurement = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        purchase_invoice_detail_id_purchase_invoice_detail = c.Int(),
                    })
                .PrimaryKey(t => t.id_invoice_property)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_dimension", t => t.id_dimension, cascadeDelete: true)
                .ForeignKey("app_measurement", t => t.id_measurement, cascadeDelete: true)
                .ForeignKey("purchase_invoice_detail", t => t.purchase_invoice_detail_id_purchase_invoice_detail)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_dimension)
                .Index(t => t.id_measurement)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.purchase_invoice_detail_id_purchase_invoice_detail);
            
            CreateTable(
                "purchase_order_detail",
                c => new
                    {
                        id_purchase_order_detail = c.Int(nullable: false, identity: true),
                        id_purchase_order = c.Int(nullable: false),
                        id_purchase_tender_detail = c.Int(),
                        id_location = c.Int(),
                        id_cost_center = c.Int(nullable: false),
                        id_item = c.Int(),
                        item_description = c.String(unicode: false),
                        quantity = c.Decimal(nullable: false, precision: 20, scale: 9),
                        lot_number = c.String(unicode: false),
                        expiration_date = c.DateTime(precision: 0),
                        id_vat_group = c.Int(nullable: false),
                        unit_cost = c.Decimal(nullable: false, precision: 20, scale: 9),
                        comment = c.String(unicode: false),
                        discount = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_purchase_order_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_cost_center", t => t.id_cost_center, cascadeDelete: true)
                .ForeignKey("app_location", t => t.id_location)
                .ForeignKey("app_vat_group", t => t.id_vat_group, cascadeDelete: true)
                .ForeignKey("items", t => t.id_item)
                .ForeignKey("purchase_order", t => t.id_purchase_order, cascadeDelete: true)
                .ForeignKey("purchase_tender_detail", t => t.id_purchase_tender_detail)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_purchase_order)
                .Index(t => t.id_purchase_tender_detail)
                .Index(t => t.id_location)
                .Index(t => t.id_cost_center)
                .Index(t => t.id_item)
                .Index(t => t.id_vat_group)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "purchase_order",
                c => new
                    {
                        id_purchase_order = c.Int(nullable: false, identity: true),
                        id_purchase_tender = c.Int(),
                        id_department = c.Int(),
                        id_currencyfx = c.Int(nullable: false),
                        recieve_date_est = c.DateTime(precision: 0),
                        id_contact = c.Int(nullable: false),
                        id_sales_rep = c.Int(),
                        id_weather = c.Int(),
                        id_branch = c.Int(nullable: false),
                        id_terminal = c.Int(),
                        id_contract = c.Int(nullable: false),
                        id_condition = c.Int(nullable: false),
                        id_range = c.Int(),
                        id_project = c.Int(),
                        status = c.Int(nullable: false),
                        number = c.String(unicode: false),
                        code = c.String(unicode: false),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        is_impex = c.Boolean(nullable: false),
                        is_issued = c.Boolean(nullable: false),
                        comment = c.String(unicode: false),
                        barcode = c.String(unicode: false),
                        trans_type = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        contact_id_contact = c.Int(),
                        contact_ref_id_contact = c.Int(),
                        newer_id_purchase_order = c.Int(),
                    })
                .PrimaryKey(t => t.id_purchase_order)                
                .ForeignKey("app_branch", t => t.id_branch, cascadeDelete: true)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_condition", t => t.id_condition, cascadeDelete: true)
                .ForeignKey("app_contract", t => t.id_contract, cascadeDelete: true)
                .ForeignKey("app_currencyfx", t => t.id_currencyfx, cascadeDelete: true)
                .ForeignKey("app_department", t => t.id_department)
                .ForeignKey("app_document_range", t => t.id_range)
                .ForeignKey("app_terminal", t => t.id_terminal)
                .ForeignKey("app_weather", t => t.id_weather)
                .ForeignKey("contacts", t => t.contact_id_contact)
                .ForeignKey("contacts", t => t.contact_ref_id_contact)
                .ForeignKey("purchase_order", t => t.newer_id_purchase_order)
                .ForeignKey("projects", t => t.id_project)
                .ForeignKey("purchase_tender", t => t.id_purchase_tender)
                .ForeignKey("sales_rep", t => t.id_sales_rep)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_purchase_tender)
                .Index(t => t.id_department)
                .Index(t => t.id_currencyfx)
                .Index(t => t.id_sales_rep)
                .Index(t => t.id_weather)
                .Index(t => t.id_branch)
                .Index(t => t.id_terminal)
                .Index(t => t.id_contract)
                .Index(t => t.id_condition)
                .Index(t => t.id_range)
                .Index(t => t.id_project)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.contact_id_contact)
                .Index(t => t.contact_ref_id_contact)
                .Index(t => t.newer_id_purchase_order);
            
            CreateTable(
                "purchase_tender",
                c => new
                    {
                        id_purchase_tender = c.Int(nullable: false, identity: true),
                        id_branch = c.Int(nullable: false),
                        status = c.Int(nullable: false),
                        id_department = c.Int(),
                        id_project = c.Int(),
                        id_weather = c.Int(),
                        name = c.String(unicode: false),
                        code = c.Short(nullable: false),
                        comment = c.String(unicode: false),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        id_range = c.Int(),
                        id_terminal = c.Int(),
                        number = c.String(unicode: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_purchase_tender)                
                .ForeignKey("app_branch", t => t.id_branch, cascadeDelete: true)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_document_range", t => t.id_range)
                .ForeignKey("app_terminal", t => t.id_terminal)
                .ForeignKey("projects", t => t.id_project)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_branch)
                .Index(t => t.id_project)
                .Index(t => t.id_range)
                .Index(t => t.id_terminal)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "purchase_tender_contact",
                c => new
                    {
                        id_purchase_tender_contact = c.Int(nullable: false, identity: true),
                        id_purchase_tender = c.Int(nullable: false),
                        id_contact = c.Int(nullable: false),
                        id_contract = c.Int(nullable: false),
                        id_condition = c.Int(nullable: false),
                        id_currencyfx = c.Int(nullable: false),
                        comment = c.String(unicode: false),
                        recieve_date_est = c.DateTime(precision: 0),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_purchase_tender_contact)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_condition", t => t.id_condition, cascadeDelete: true)
                .ForeignKey("app_contract", t => t.id_contract, cascadeDelete: true)
                .ForeignKey("app_currencyfx", t => t.id_currencyfx, cascadeDelete: true)
                .ForeignKey("contacts", t => t.id_contact, cascadeDelete: true)
                .ForeignKey("purchase_tender", t => t.id_purchase_tender, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_purchase_tender)
                .Index(t => t.id_contact)
                .Index(t => t.id_contract)
                .Index(t => t.id_condition)
                .Index(t => t.id_currencyfx)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "purchase_tender_detail",
                c => new
                    {
                        id_purchase_tender_detail = c.Int(nullable: false, identity: true),
                        id_purchase_tender_contact = c.Int(nullable: false),
                        id_purchase_tender_item = c.Int(nullable: false),
                        status = c.Int(nullable: false),
                        item_description = c.String(unicode: false),
                        quantity = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_vat_group = c.Int(nullable: false),
                        unit_cost = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_purchase_tender_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("purchase_tender_contact", t => t.id_purchase_tender_contact, cascadeDelete: true)
                .ForeignKey("purchase_tender_item", t => t.id_purchase_tender_item, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_purchase_tender_contact)
                .Index(t => t.id_purchase_tender_item)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "purchase_tender_detail_dimension",
                c => new
                    {
                        id_tender_detail_property = c.Int(nullable: false, identity: true),
                        id_purchase_tender_detail = c.Int(nullable: false),
                        id_dimension = c.Int(nullable: false),
                        id_measurement = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_tender_detail_property)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_dimension", t => t.id_dimension, cascadeDelete: true)
                .ForeignKey("app_measurement", t => t.id_measurement, cascadeDelete: true)
                .ForeignKey("purchase_tender_detail", t => t.id_purchase_tender_detail, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_purchase_tender_detail)
                .Index(t => t.id_dimension)
                .Index(t => t.id_measurement)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "purchase_tender_item",
                c => new
                    {
                        id_purchase_tender_item = c.Int(nullable: false, identity: true),
                        id_purchase_tender = c.Int(nullable: false),
                        id_item = c.Int(),
                        id_project_task = c.Int(),
                        item_description = c.String(unicode: false),
                        quantity = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_purchase_tender_item)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("items", t => t.id_item)
                .ForeignKey("project_task", t => t.id_project_task)
                .ForeignKey("purchase_tender", t => t.id_purchase_tender, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_purchase_tender)
                .Index(t => t.id_item)
                .Index(t => t.id_project_task)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "purchase_tender_dimension",
                c => new
                    {
                        id_tender_property = c.Long(nullable: false, identity: true),
                        id_purchase_tender_item = c.Int(nullable: false),
                        id_dimension = c.Int(nullable: false),
                        id_measurement = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_tender_property)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_dimension", t => t.id_dimension, cascadeDelete: true)
                .ForeignKey("app_measurement", t => t.id_measurement, cascadeDelete: true)
                .ForeignKey("purchase_tender_item", t => t.id_purchase_tender_item, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_purchase_tender_item)
                .Index(t => t.id_dimension)
                .Index(t => t.id_measurement)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "purchase_order_dimension",
                c => new
                    {
                        id_order_property = c.Long(nullable: false, identity: true),
                        id_purchase_order_detail = c.Long(nullable: false),
                        id_dimension = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_measurement = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        purchase_order_detail_id_purchase_order_detail = c.Int(),
                    })
                .PrimaryKey(t => t.id_order_property)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_dimension", t => t.id_dimension, cascadeDelete: true)
                .ForeignKey("app_measurement", t => t.id_measurement, cascadeDelete: true)
                .ForeignKey("purchase_order_detail", t => t.purchase_order_detail_id_purchase_order_detail)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_dimension)
                .Index(t => t.id_measurement)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.purchase_order_detail_id_purchase_order_detail);
            
            CreateTable(
                "purchase_return",
                c => new
                    {
                        id_purchase_return = c.Int(nullable: false, identity: true),
                        id_purchase_invoice = c.Int(nullable: false),
                        return_type = c.Int(nullable: false),
                        id_currencyfx = c.Int(nullable: false),
                        is_accounted = c.Boolean(nullable: false),
                        id_journal = c.Int(),
                        id_contact = c.Int(nullable: false),
                        id_sales_rep = c.Int(),
                        id_weather = c.Int(),
                        id_branch = c.Int(nullable: false),
                        id_terminal = c.Int(),
                        id_contract = c.Int(nullable: false),
                        id_condition = c.Int(nullable: false),
                        id_range = c.Int(),
                        id_project = c.Int(),
                        status = c.Int(nullable: false),
                        number = c.String(unicode: false),
                        code = c.String(unicode: false),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        is_impex = c.Boolean(nullable: false),
                        is_issued = c.Boolean(nullable: false),
                        comment = c.String(unicode: false),
                        barcode = c.String(unicode: false),
                        trans_type = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        contact_id_contact = c.Int(),
                        contact_ref_id_contact = c.Int(),
                        newer_id_purchase_return = c.Int(),
                    })
                .PrimaryKey(t => t.id_purchase_return)                
                .ForeignKey("app_branch", t => t.id_branch, cascadeDelete: true)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_condition", t => t.id_condition, cascadeDelete: true)
                .ForeignKey("app_contract", t => t.id_contract, cascadeDelete: true)
                .ForeignKey("app_currencyfx", t => t.id_currencyfx, cascadeDelete: true)
                .ForeignKey("app_document_range", t => t.id_range)
                .ForeignKey("app_terminal", t => t.id_terminal)
                .ForeignKey("app_weather", t => t.id_weather)
                .ForeignKey("contacts", t => t.contact_id_contact)
                .ForeignKey("contacts", t => t.contact_ref_id_contact)
                .ForeignKey("purchase_return", t => t.newer_id_purchase_return)
                .ForeignKey("projects", t => t.id_project)
                .ForeignKey("purchase_invoice", t => t.id_purchase_invoice, cascadeDelete: true)
                .ForeignKey("sales_rep", t => t.id_sales_rep)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_purchase_invoice)
                .Index(t => t.id_currencyfx)
                .Index(t => t.id_sales_rep)
                .Index(t => t.id_weather)
                .Index(t => t.id_branch)
                .Index(t => t.id_terminal)
                .Index(t => t.id_contract)
                .Index(t => t.id_condition)
                .Index(t => t.id_range)
                .Index(t => t.id_project)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.contact_id_contact)
                .Index(t => t.contact_ref_id_contact)
                .Index(t => t.newer_id_purchase_return);
            
            CreateTable(
                "purchase_return_detail",
                c => new
                    {
                        id_purchase_return_detail = c.Int(nullable: false, identity: true),
                        id_purchase_return = c.Int(nullable: false),
                        id_purchase_invoice_detail = c.Int(),
                        has_return = c.Boolean(nullable: false),
                        discount = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_location = c.Int(),
                        id_cost_center = c.Int(nullable: false),
                        id_item = c.Int(),
                        item_description = c.String(unicode: false),
                        quantity = c.Decimal(nullable: false, precision: 20, scale: 9),
                        lot_number = c.String(unicode: false),
                        expiration_date = c.DateTime(precision: 0),
                        id_vat_group = c.Int(nullable: false),
                        unit_cost = c.Decimal(nullable: false, precision: 20, scale: 9),
                        comment = c.String(unicode: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_purchase_return_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_cost_center", t => t.id_cost_center, cascadeDelete: true)
                .ForeignKey("app_location", t => t.id_location)
                .ForeignKey("app_vat_group", t => t.id_vat_group, cascadeDelete: true)
                .ForeignKey("items", t => t.id_item)
                .ForeignKey("purchase_invoice_detail", t => t.id_purchase_invoice_detail)
                .ForeignKey("purchase_return", t => t.id_purchase_return, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_purchase_return)
                .Index(t => t.id_purchase_invoice_detail)
                .Index(t => t.id_location)
                .Index(t => t.id_cost_center)
                .Index(t => t.id_item)
                .Index(t => t.id_vat_group)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "purchase_return_dimension",
                c => new
                    {
                        id_return_property = c.Long(nullable: false, identity: true),
                        id_purchase_return_detail = c.Long(nullable: false),
                        id_dimension = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_measurement = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        purchase_return_detail_id_purchase_return_detail = c.Int(),
                    })
                .PrimaryKey(t => t.id_return_property)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_dimension", t => t.id_dimension, cascadeDelete: true)
                .ForeignKey("app_measurement", t => t.id_measurement, cascadeDelete: true)
                .ForeignKey("purchase_return_detail", t => t.purchase_return_detail_id_purchase_return_detail)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_dimension)
                .Index(t => t.id_measurement)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.purchase_return_detail_id_purchase_return_detail);
            
            CreateTable(
                "item_movement_value",
                c => new
                    {
                        id_movement_value = c.Long(nullable: false, identity: true),
                        id_movement = c.Long(nullable: false),
                        id_currencyfx = c.Int(nullable: false),
                        unit_value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        comment = c.String(unicode: false),
                        is_estimate = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_movement_value)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_currencyfx", t => t.id_currencyfx, cascadeDelete: true)
                .ForeignKey("item_movement", t => t.id_movement, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_movement)
                .Index(t => t.id_currencyfx)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_transfer_detail",
                c => new
                    {
                        id_transfer_detail = c.Int(nullable: false, identity: true),
                        status = c.Int(nullable: false),
                        id_transfer = c.Int(nullable: false),
                        id_project_task = c.Int(),
                        id_item_product = c.Int(nullable: false),
                        movement_id = c.Int(),
                        quantity_origin = c.Decimal(nullable: false, precision: 20, scale: 9),
                        quantity_destination = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_transfer_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("item_product", t => t.id_item_product, cascadeDelete: true)
                .ForeignKey("item_transfer", t => t.id_transfer, cascadeDelete: true)
                .ForeignKey("project_task", t => t.id_project_task)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_transfer)
                .Index(t => t.id_project_task)
                .Index(t => t.id_item_product)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_transfer",
                c => new
                    {
                        id_transfer = c.Int(nullable: false, identity: true),
                        id_weather = c.Int(),
                        status = c.Int(nullable: false),
                        id_item_request = c.Int(),
                        id_project = c.Int(),
                        id_department = c.Int(),
                        id_range = c.Int(),
                        number = c.String(unicode: false),
                        id_terminal = c.Int(),
                        comment = c.String(unicode: false),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        transfer_type = c.Int(nullable: false),
                        id_branch = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        app_branch_id_branch = c.Int(),
                        app_branch_destination_id_branch = c.Int(),
                        app_branch_origin_id_branch = c.Int(),
                        app_location_destination_id_location = c.Int(),
                        app_location_origin_id_location = c.Int(),
                        employee_id_contact = c.Int(),
                        security_user_id_user = c.Int(),
                        user_given_id_user = c.Int(),
                        user_requested_id_user = c.Int(),
                    })
                .PrimaryKey(t => t.id_transfer)                
                .ForeignKey("app_branch", t => t.app_branch_id_branch)
                .ForeignKey("app_branch", t => t.app_branch_destination_id_branch)
                .ForeignKey("app_branch", t => t.app_branch_origin_id_branch)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_department", t => t.id_department)
                .ForeignKey("app_document_range", t => t.id_range)
                .ForeignKey("app_location", t => t.app_location_destination_id_location)
                .ForeignKey("app_location", t => t.app_location_origin_id_location)
                .ForeignKey("app_terminal", t => t.id_terminal)
                .ForeignKey("app_weather", t => t.id_weather)
                .ForeignKey("contacts", t => t.employee_id_contact)
                .ForeignKey("item_request", t => t.id_item_request)
                .ForeignKey("projects", t => t.id_project)
                .ForeignKey("security_user", t => t.security_user_id_user)
                .ForeignKey("security_user", t => t.user_given_id_user)
                .ForeignKey("security_user", t => t.user_requested_id_user)
                .Index(t => t.id_weather)
                .Index(t => t.id_item_request)
                .Index(t => t.id_project)
                .Index(t => t.id_department)
                .Index(t => t.id_range)
                .Index(t => t.id_terminal)
                .Index(t => t.id_company)
                .Index(t => t.app_branch_id_branch)
                .Index(t => t.app_branch_destination_id_branch)
                .Index(t => t.app_branch_origin_id_branch)
                .Index(t => t.app_location_destination_id_location)
                .Index(t => t.app_location_origin_id_location)
                .Index(t => t.employee_id_contact)
                .Index(t => t.security_user_id_user)
                .Index(t => t.user_given_id_user)
                .Index(t => t.user_requested_id_user);
            
            CreateTable(
                "item_transfer_dimension",
                c => new
                    {
                        id_transfer_property = c.Long(nullable: false, identity: true),
                        id_transfer_detail = c.Long(nullable: false),
                        id_dimension = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        item_transfer_detail_id_transfer_detail = c.Int(),
                    })
                .PrimaryKey(t => t.id_transfer_property)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_dimension", t => t.id_dimension, cascadeDelete: true)
                .ForeignKey("item_transfer_detail", t => t.item_transfer_detail_id_transfer_detail)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_dimension)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.item_transfer_detail_id_transfer_detail);
            
            CreateTable(
                "item_attachment",
                c => new
                    {
                        id_item_attachment = c.Int(nullable: false, identity: true),
                        id_item = c.Int(nullable: false),
                        id_attachment = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_item_attachment)                
                .ForeignKey("app_attachment", t => t.id_attachment, cascadeDelete: true)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("items", t => t.id_item, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_item)
                .Index(t => t.id_attachment)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "app_attachment",
                c => new
                    {
                        id_attachment = c.Int(nullable: false, identity: true),
                        file = c.Binary(),
                        mime = c.String(unicode: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_attachment)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_brand",
                c => new
                    {
                        id_brand = c.Int(nullable: false, identity: true),
                        id_contact = c.Int(),
                        name = c.String(nullable: false, unicode: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_brand)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("contacts", t => t.id_contact)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_contact)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_price",
                c => new
                    {
                        id_item_price = c.Int(nullable: false, identity: true),
                        id_item = c.Int(nullable: false),
                        id_price_list = c.Int(nullable: false),
                        id_currency = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        min_quantity = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_item_price)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("items", t => t.id_item, cascadeDelete: true)
                .ForeignKey("item_price_list", t => t.id_price_list, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_item)
                .Index(t => t.id_price_list)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_price_list",
                c => new
                    {
                        id_price_list = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, unicode: false),
                        is_default = c.Boolean(nullable: false),
                        is_active = c.Boolean(nullable: false),
                        percent_type = c.Int(nullable: false),
                        percent_over = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        ref_price_list_id_price_list = c.Int(),
                    })
                .PrimaryKey(t => t.id_price_list)                
                .ForeignKey("app_company", t => t.id_company)
                .ForeignKey("item_price_list", t => t.ref_price_list_id_price_list)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.ref_price_list_id_price_list);
            
            CreateTable(
                "item_property",
                c => new
                    {
                        id_item_property = c.Int(nullable: false, identity: true),
                        id_item = c.Int(nullable: false),
                        id_property = c.Int(nullable: false),
                        id_measurement = c.Int(),
                        value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_item_property)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_property", t => t.id_property, cascadeDelete: true)
                .ForeignKey("items", t => t.id_item, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_item)
                .Index(t => t.id_property)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "app_property",
                c => new
                    {
                        id_property = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.id_property)                ;
            
            CreateTable(
                "item_recepie",
                c => new
                    {
                        id_recepie = c.Int(nullable: false, identity: true),
                        id_item = c.Decimal(nullable: false, precision: 20, scale: 9),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        item_id_item = c.Int(),
                    })
                .PrimaryKey(t => t.id_recepie)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("items", t => t.item_id_item)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.item_id_item);
            
            CreateTable(
                "item_recepie_detail",
                c => new
                    {
                        id_recepie_detail = c.Int(nullable: false, identity: true),
                        id_recepie = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_item = c.Decimal(nullable: false, precision: 20, scale: 9),
                        quantity = c.Decimal(nullable: false, precision: 20, scale: 9),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        item_id_item = c.Int(),
                        item_recepie_id_recepie = c.Int(),
                    })
                .PrimaryKey(t => t.id_recepie_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("items", t => t.item_id_item)
                .ForeignKey("item_recepie", t => t.item_recepie_id_recepie)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.item_id_item)
                .Index(t => t.item_recepie_id_recepie);
            
            CreateTable(
                "item_service",
                c => new
                    {
                        id_item_service = c.Int(nullable: false, identity: true),
                        id_item = c.Int(nullable: false),
                        id_talent = c.Int(),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_item_service)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("hr_talent", t => t.id_talent)
                .ForeignKey("items", t => t.id_item, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_item)
                .Index(t => t.id_talent)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "hr_talent",
                c => new
                    {
                        id_talent = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, unicode: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_talent)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "hr_talent_detail",
                c => new
                    {
                        id_talent_detail = c.Int(nullable: false, identity: true),
                        id_talent = c.Int(nullable: false),
                        id_contact = c.Int(nullable: false),
                        experience = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_talent_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("contacts", t => t.id_contact, cascadeDelete: true)
                .ForeignKey("hr_talent", t => t.id_talent, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_talent)
                .Index(t => t.id_contact)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "project_event",
                c => new
                    {
                        id_project_event = c.Int(nullable: false, identity: true),
                        id_project_event_template = c.Int(nullable: false),
                        id_item = c.Int(nullable: false),
                        id_contact = c.Int(),
                        name = c.String(nullable: false, unicode: false),
                        quantity_adult = c.Int(nullable: false),
                        quantity_child = c.Int(nullable: false),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        is_active = c.Boolean(nullable: false),
                        id_currencyfx = c.Int(nullable: false),
                        status = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_project_event)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("contacts", t => t.id_contact)
                .ForeignKey("items", t => t.id_item, cascadeDelete: true)
                .ForeignKey("project_event_template", t => t.id_project_event_template, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_project_event_template)
                .Index(t => t.id_item)
                .Index(t => t.id_contact)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "project_event_fixed",
                c => new
                    {
                        id_project_event_fixed = c.Int(nullable: false, identity: true),
                        id_project_event = c.Int(nullable: false),
                        id_tag = c.Int(nullable: false),
                        id_item = c.Int(nullable: false),
                        consumption = c.Decimal(nullable: false, precision: 20, scale: 9),
                        is_included = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_project_event_fixed)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("items", t => t.id_item, cascadeDelete: true)
                .ForeignKey("item_tag", t => t.id_tag, cascadeDelete: true)
                .ForeignKey("project_event", t => t.id_project_event, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_project_event)
                .Index(t => t.id_tag)
                .Index(t => t.id_item)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "project_event_template",
                c => new
                    {
                        id_project_event_template = c.Int(nullable: false, identity: true),
                        id_tag = c.Int(nullable: false),
                        name = c.String(nullable: false, unicode: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_project_event_template)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("item_tag", t => t.id_tag, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_tag)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "project_event_template_fixed",
                c => new
                    {
                        id_project_event_template_fixed = c.Int(nullable: false, identity: true),
                        id_project_event_template = c.Int(nullable: false),
                        id_tag = c.Int(nullable: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_project_event_template_fixed)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("item_tag", t => t.id_tag, cascadeDelete: true)
                .ForeignKey("project_event_template", t => t.id_project_event_template, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_project_event_template)
                .Index(t => t.id_tag)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "project_event_template_variable",
                c => new
                    {
                        id_project_event_template_variable = c.Int(nullable: false, identity: true),
                        id_project_event_template = c.Int(nullable: false),
                        id_tag = c.Int(nullable: false),
                        adult_consumption = c.Decimal(nullable: false, precision: 20, scale: 9),
                        child_consumption = c.Decimal(nullable: false, precision: 20, scale: 9),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_project_event_template_variable)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("item_tag", t => t.id_tag, cascadeDelete: true)
                .ForeignKey("project_event_template", t => t.id_project_event_template, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_project_event_template)
                .Index(t => t.id_tag)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "project_event_variable",
                c => new
                    {
                        id_project_event_variable = c.Int(nullable: false, identity: true),
                        id_project_event = c.Int(nullable: false),
                        id_tag = c.Int(nullable: false),
                        id_item = c.Int(nullable: false),
                        adult_consumption = c.Decimal(nullable: false, precision: 20, scale: 9),
                        child_consumption = c.Decimal(nullable: false, precision: 20, scale: 9),
                        is_included = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_project_event_variable)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("items", t => t.id_item, cascadeDelete: true)
                .ForeignKey("item_tag", t => t.id_tag, cascadeDelete: true)
                .ForeignKey("project_event", t => t.id_project_event, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_project_event)
                .Index(t => t.id_tag)
                .Index(t => t.id_item)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "contact_tag_detail",
                c => new
                    {
                        id_contact_tag_detail = c.Int(nullable: false, identity: true),
                        id_contact = c.Int(nullable: false),
                        id_tag = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_contact_tag_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("contacts", t => t.id_contact, cascadeDelete: true)
                .ForeignKey("contact_tag", t => t.id_tag, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_contact)
                .Index(t => t.id_tag)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "contact_tag",
                c => new
                    {
                        id_tag = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, unicode: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_tag)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "hr_contract",
                c => new
                    {
                        id_hr_contract = c.Int(nullable: false, identity: true),
                        id_contact = c.Int(nullable: false),
                        id_branch = c.Int(),
                        id_department = c.Int(),
                        id_currency = c.Int(),
                        base_salary = c.Decimal(nullable: false, precision: 20, scale: 9),
                        codigo = c.String(unicode: false),
                        start_date = c.DateTime(nullable: false, precision: 0),
                        end_date = c.DateTime(nullable: false, precision: 0),
                        end_trial_period = c.DateTime(nullable: false, precision: 0),
                        comment = c.String(unicode: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_hr_contract)                
                .ForeignKey("app_branch", t => t.id_branch)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_currency", t => t.id_currency)
                .ForeignKey("app_department", t => t.id_department)
                .ForeignKey("contacts", t => t.id_contact, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_contact)
                .Index(t => t.id_branch)
                .Index(t => t.id_department)
                .Index(t => t.id_currency)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "hr_education",
                c => new
                    {
                        id_education = c.Int(nullable: false, identity: true),
                        id_contact = c.Int(nullable: false),
                        institution = c.String(nullable: false, unicode: false),
                        education_level = c.Int(),
                        start_date = c.DateTime(precision: 0),
                        end_date = c.DateTime(precision: 0),
                        average = c.Decimal(precision: 20, scale: 9),
                        comment = c.String(unicode: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_education)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("contacts", t => t.id_contact, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_contact)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "hr_family",
                c => new
                    {
                        id_family = c.Int(nullable: false, identity: true),
                        id_contact = c.Int(nullable: false),
                        name = c.String(nullable: false, unicode: false),
                        relationship = c.Int(nullable: false),
                        date_birth = c.DateTime(nullable: false, precision: 0),
                        telephone_emergency = c.String(unicode: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_family)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("contacts", t => t.id_contact, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_contact)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "impex_export",
                c => new
                    {
                        id_export = c.Short(nullable: false, identity: true),
                        id_impex = c.Int(nullable: false),
                        id_sales_invoice = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_export)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("impexes", t => t.id_impex, cascadeDelete: true)
                .ForeignKey("sales_invoice", t => t.id_sales_invoice, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_impex)
                .Index(t => t.id_sales_invoice)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "impex_import",
                c => new
                    {
                        id_import = c.Short(nullable: false, identity: true),
                        id_impex = c.Int(nullable: false),
                        id_purchase_invoice = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_import)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("impexes", t => t.id_impex, cascadeDelete: true)
                .ForeignKey("purchase_invoice", t => t.id_purchase_invoice, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_impex)
                .Index(t => t.id_purchase_invoice)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "impex_incoterm",
                c => new
                    {
                        id_incoterm = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, unicode: false),
                        code_iso = c.String(unicode: false),
                        is_priority = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_incoterm)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "impex_incoterm_detail",
                c => new
                    {
                        id_incoterm_detail = c.Int(nullable: false, identity: true),
                        id_incoterm = c.Int(nullable: false),
                        id_incoterm_condition = c.Int(nullable: false),
                        buyer = c.Boolean(nullable: false),
                        seller = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_incoterm_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("impex_incoterm", t => t.id_incoterm, cascadeDelete: true)
                .ForeignKey("impex_incoterm_condition", t => t.id_incoterm_condition, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_incoterm)
                .Index(t => t.id_incoterm_condition)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "impex_incoterm_condition",
                c => new
                    {
                        id_incoterm_condition = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, unicode: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_incoterm_condition)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "security_crud",
                c => new
                    {
                        id_crud = c.Int(nullable: false, identity: true),
                        id_role = c.Int(nullable: false),
                        id_application = c.Int(nullable: false),
                        can_create = c.Boolean(nullable: false),
                        can_read = c.Boolean(nullable: false),
                        can_update = c.Boolean(nullable: false),
                        can_delete = c.Boolean(nullable: false),
                        can_approve = c.Boolean(nullable: false),
                        can_annul = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_crud)                
                .ForeignKey("security_role", t => t.id_role, cascadeDelete: true)
                .Index(t => t.id_role);
            
            CreateTable(
                "security_role_privilage",
                c => new
                    {
                        id_role_privilage = c.Int(nullable: false, identity: true),
                        id_role = c.Int(nullable: false),
                        id_privilage = c.Int(nullable: false),
                        has_privilage = c.Boolean(nullable: false),
                        value_max = c.Decimal(precision: 20, scale: 9),
                    })
                .PrimaryKey(t => t.id_role_privilage)                
                .ForeignKey("security_privilage", t => t.id_privilage, cascadeDelete: true)
                .ForeignKey("security_role", t => t.id_role, cascadeDelete: true)
                .Index(t => t.id_role)
                .Index(t => t.id_privilage);
            
            CreateTable(
                "security_privilage",
                c => new
                    {
                        id_privilage = c.Int(nullable: false, identity: true),
                        id_application = c.Int(nullable: false),
                        name = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id_privilage)                ;
            
            CreateTable(
                "accounting_template",
                c => new
                    {
                        id_template = c.Int(nullable: false, identity: true),
                        name = c.String(unicode: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_template)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "accounting_template_detail",
                c => new
                    {
                        id_template_detail = c.Int(nullable: false, identity: true),
                        id_template = c.Int(nullable: false),
                        id_chart = c.Int(nullable: false),
                        coefficeint = c.Decimal(nullable: false, precision: 20, scale: 9),
                        is_debit = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_template_detail)                
                .ForeignKey("accounting_chart", t => t.id_chart, cascadeDelete: true)
                .ForeignKey("accounting_template", t => t.id_template, cascadeDelete: true)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_template)
                .Index(t => t.id_chart)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "app_branch_walkins",
                c => new
                    {
                        id_branch_walkin = c.Int(nullable: false, identity: true),
                        id_branch = c.Int(nullable: false),
                        start_date = c.DateTime(nullable: false, precision: 0),
                        end_date = c.DateTime(nullable: false, precision: 0),
                        quantity = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_branch_walkin)                
                .ForeignKey("app_branch", t => t.id_branch, cascadeDelete: true)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_branch)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "app_configuration",
                c => new
                    {
                        id_configuration = c.Int(nullable: false, identity: true),
                        application = c.Int(nullable: false),
                        configuration = c.Int(nullable: false),
                        value = c.String(unicode: false),
                        type = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_configuration)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "app_name_template",
                c => new
                    {
                        id_name_template = c.Short(nullable: false, identity: true),
                        app_name = c.Int(nullable: false),
                        name = c.String(nullable: false, unicode: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_name_template)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "app_name_template_detail",
                c => new
                    {
                        id_name_template_detail = c.Short(nullable: false, identity: true),
                        id_name_template = c.Short(nullable: false),
                        sequence = c.Short(nullable: false),
                        question = c.String(unicode: false),
                        answer_type = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_name_template_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_name_template", t => t.id_name_template, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_name_template)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "bi_chart_report",
                c => new
                    {
                        id_bi_chart_report = c.Int(nullable: false, identity: true),
                        id_bi_report = c.Int(nullable: false),
                        chart_type = c.Int(nullable: false),
                        is_default = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_bi_chart_report)                
                .ForeignKey("bi_report", t => t.id_bi_report, cascadeDelete: true)
                .Index(t => t.id_bi_report);
            
            CreateTable(
                "bi_report",
                c => new
                    {
                        id_bi_report = c.Int(nullable: false, identity: true),
                        name = c.String(unicode: false),
                        short_description = c.String(unicode: false),
                        long_description = c.String(unicode: false),
                        query = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.id_bi_report)                ;
            
            CreateTable(
                "bi_report_detail",
                c => new
                    {
                        id_bi_report_detail = c.Int(nullable: false, identity: true),
                        id_bi_report = c.Int(nullable: false),
                        name_column = c.String(unicode: false),
                        display_column = c.String(unicode: false),
                        format_column = c.Int(nullable: false),
                        is_output = c.Boolean(nullable: false),
                        group = c.Int(),
                        order_by = c.Int(),
                        filter_by = c.Int(),
                        aggregate_by = c.Int(),
                        is_runningtotal = c.Boolean(nullable: false),
                        is_header = c.Boolean(nullable: false),
                        is_footer = c.Boolean(nullable: false),
                        is_drilldown = c.Boolean(nullable: false),
                        chart_axis = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.id_bi_report_detail)                
                .ForeignKey("bi_report", t => t.id_bi_report, cascadeDelete: true)
                .Index(t => t.id_bi_report);
            
            CreateTable(
                "bi_tag",
                c => new
                    {
                        id_bi_tag = c.Int(nullable: false, identity: true),
                        name = c.String(unicode: false),
                        id_module = c.Int(),
                        behavior = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_bi_tag)                ;
            
            CreateTable(
                "bi_tag_report",
                c => new
                    {
                        id_bi_tag_report = c.Int(nullable: false, identity: true),
                        id_bi_tag = c.Int(nullable: false),
                        id_bi_report = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id_bi_tag_report)                
                .ForeignKey("bi_report", t => t.id_bi_report, cascadeDelete: true)
                .ForeignKey("bi_tag", t => t.id_bi_tag, cascadeDelete: true)
                .Index(t => t.id_bi_tag)
                .Index(t => t.id_bi_report);
            
            CreateTable(
                "bi_tag_role",
                c => new
                    {
                        id_bi_tag_role = c.Int(nullable: false, identity: true),
                        id_bi_tag = c.Int(nullable: false),
                        id_role = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id_bi_tag_role)                
                .ForeignKey("bi_tag", t => t.id_bi_tag, cascadeDelete: true)
                .ForeignKey("security_role", t => t.id_role, cascadeDelete: true)
                .Index(t => t.id_bi_tag)
                .Index(t => t.id_role);
            
            CreateTable(
                "hr_timesheet",
                c => new
                    {
                        id_timesheet = c.Int(nullable: false, identity: true),
                        id_contact = c.Int(nullable: false),
                        id_branch = c.Int(nullable: false),
                        is_entry = c.Boolean(nullable: false),
                        time = c.DateTime(nullable: false, precision: 0),
                        comment = c.String(unicode: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_timesheet)                
                .ForeignKey("app_branch", t => t.id_branch, cascadeDelete: true)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("contacts", t => t.id_contact, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_contact)
                .Index(t => t.id_branch)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_inventory",
                c => new
                    {
                        id_inventory = c.Int(nullable: false, identity: true),
                        id_branch = c.Int(nullable: false),
                        code = c.Int(nullable: false),
                        comment = c.String(unicode: false),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        status = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_inventory)                
                .ForeignKey("app_branch", t => t.id_branch, cascadeDelete: true)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_branch)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_inventory_detail",
                c => new
                    {
                        id_inventory_detail = c.Int(nullable: false, identity: true),
                        id_inventory = c.Int(nullable: false),
                        id_location = c.Int(nullable: false),
                        id_item_product = c.Int(nullable: false),
                        status = c.Int(nullable: false),
                        value_system = c.Decimal(nullable: false, precision: 20, scale: 9),
                        value_counted = c.Decimal(precision: 20, scale: 9),
                        comment = c.String(unicode: false),
                        id_currencyfx = c.Int(nullable: false),
                        unit_value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_inventory_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_location", t => t.id_location, cascadeDelete: true)
                .ForeignKey("item_inventory", t => t.id_inventory, cascadeDelete: true)
                .ForeignKey("item_product", t => t.id_item_product, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_inventory)
                .Index(t => t.id_location)
                .Index(t => t.id_item_product)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_inventory_dimension",
                c => new
                    {
                        id_item_inventory_dimension = c.Int(nullable: false, identity: true),
                        id_inventory_detail = c.Int(nullable: false),
                        id_dimension = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_item_inventory_dimension)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_dimension", t => t.id_dimension, cascadeDelete: true)
                .ForeignKey("item_inventory_detail", t => t.id_inventory_detail, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_inventory_detail)
                .Index(t => t.id_dimension)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_template",
                c => new
                    {
                        id_template = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, unicode: false),
                        is_active = c.Boolean(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_template)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "item_template_detail",
                c => new
                    {
                        id_template_detail = c.Int(nullable: false, identity: true),
                        id_template = c.Int(nullable: false),
                        question = c.String(nullable: false, unicode: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_template_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("item_template", t => t.id_template, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_template)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "purchase_packing",
                c => new
                    {
                        id_purchase_packing = c.Int(nullable: false, identity: true),
                        id_contact = c.Int(nullable: false),
                        id_branch = c.Int(nullable: false),
                        id_range = c.Int(),
                        number = c.String(unicode: false),
                        comment = c.String(unicode: false),
                        trans_date = c.DateTime(nullable: false, precision: 0),
                        status = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        app_terminal_id_terminal = c.Int(),
                        newer_id_purchase_packing = c.Int(),
                    })
                .PrimaryKey(t => t.id_purchase_packing)                
                .ForeignKey("app_branch", t => t.id_branch, cascadeDelete: true)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_terminal", t => t.app_terminal_id_terminal)
                .ForeignKey("contacts", t => t.id_contact, cascadeDelete: true)
                .ForeignKey("purchase_packing", t => t.newer_id_purchase_packing)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_contact)
                .Index(t => t.id_branch)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.app_terminal_id_terminal)
                .Index(t => t.newer_id_purchase_packing);
            
            CreateTable(
                "purchase_packing_detail",
                c => new
                    {
                        id_purchase_packing_detail = c.Int(nullable: false, identity: true),
                        id_purchase_packing = c.Int(nullable: false),
                        id_purchase_order_detail = c.Int(),
                        id_location = c.Int(),
                        id_item = c.Int(nullable: false),
                        quantity = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_purchase_packing_detail)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_location", t => t.id_location)
                .ForeignKey("items", t => t.id_item, cascadeDelete: true)
                .ForeignKey("purchase_order_detail", t => t.id_purchase_order_detail)
                .ForeignKey("purchase_packing", t => t.id_purchase_packing, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_purchase_packing)
                .Index(t => t.id_purchase_order_detail)
                .Index(t => t.id_location)
                .Index(t => t.id_item)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
            CreateTable(
                "purchase_packing_dimension",
                c => new
                    {
                        id_packing_property = c.Long(nullable: false, identity: true),
                        id_purchase_packing_detail = c.Long(nullable: false),
                        id_dimension = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        id_measurement = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                        purchase_packing_detail_id_purchase_packing_detail = c.Int(),
                    })
                .PrimaryKey(t => t.id_packing_property)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_dimension", t => t.id_dimension, cascadeDelete: true)
                .ForeignKey("app_measurement", t => t.id_measurement, cascadeDelete: true)
                .ForeignKey("purchase_packing_detail", t => t.purchase_packing_detail_id_purchase_packing_detail)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_dimension)
                .Index(t => t.id_measurement)
                .Index(t => t.id_company)
                .Index(t => t.id_user)
                .Index(t => t.purchase_packing_detail_id_purchase_packing_detail);
            
            CreateTable(
                "purchase_packing_relation",
                c => new
                    {
                        id_purchase_packing_relation = c.Int(nullable: false, identity: true),
                        id_purchase_invoice = c.Int(nullable: false),
                        id_purchase_packing = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id_purchase_packing_relation)                
                .ForeignKey("purchase_invoice", t => t.id_purchase_invoice, cascadeDelete: true)
                .ForeignKey("purchase_packing", t => t.id_purchase_packing, cascadeDelete: true)
                .Index(t => t.id_purchase_invoice)
                .Index(t => t.id_purchase_packing);
            
            CreateTable(
                "sales_promotion",
                c => new
                    {
                        id_sales_promotion = c.Int(nullable: false, identity: true),
                        types = c.Int(nullable: false),
                        name = c.String(unicode: false),
                        reference = c.Int(nullable: false),
                        date_start = c.DateTime(nullable: false, precision: 0),
                        date_end = c.DateTime(nullable: false, precision: 0),
                        quantity_min = c.Decimal(nullable: false, precision: 20, scale: 9),
                        quantity_max = c.Decimal(nullable: false, precision: 20, scale: 9),
                        quantity_step = c.Decimal(nullable: false, precision: 20, scale: 9),
                        is_percentage = c.Boolean(nullable: false),
                        result_value = c.Decimal(nullable: false, precision: 20, scale: 9),
                        result_step = c.Decimal(nullable: false, precision: 20, scale: 9),
                        reference_bonus = c.Int(nullable: false),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_sales_promotion)                
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_company)
                .Index(t => t.id_user);
            
        }
        
        public override void Down()
        {
            DropForeignKey("sales_promotion", "id_user", "security_user");
            DropForeignKey("sales_promotion", "id_company", "app_company");
            DropForeignKey("purchase_packing", "id_user", "security_user");
            DropForeignKey("purchase_packing_relation", "id_purchase_packing", "purchase_packing");
            DropForeignKey("purchase_packing_relation", "id_purchase_invoice", "purchase_invoice");
            DropForeignKey("purchase_packing_detail", "id_user", "security_user");
            DropForeignKey("purchase_packing_dimension", "id_user", "security_user");
            DropForeignKey("purchase_packing_dimension", "purchase_packing_detail_id_purchase_packing_detail", "purchase_packing_detail");
            DropForeignKey("purchase_packing_dimension", "id_measurement", "app_measurement");
            DropForeignKey("purchase_packing_dimension", "id_dimension", "app_dimension");
            DropForeignKey("purchase_packing_dimension", "id_company", "app_company");
            DropForeignKey("purchase_packing_detail", "id_purchase_packing", "purchase_packing");
            DropForeignKey("purchase_packing_detail", "id_purchase_order_detail", "purchase_order_detail");
            DropForeignKey("purchase_packing_detail", "id_item", "items");
            DropForeignKey("purchase_packing_detail", "id_location", "app_location");
            DropForeignKey("purchase_packing_detail", "id_company", "app_company");
            DropForeignKey("purchase_packing", "newer_id_purchase_packing", "purchase_packing");
            DropForeignKey("purchase_packing", "id_contact", "contacts");
            DropForeignKey("purchase_packing", "app_terminal_id_terminal", "app_terminal");
            DropForeignKey("purchase_packing", "id_company", "app_company");
            DropForeignKey("purchase_packing", "id_branch", "app_branch");
            DropForeignKey("item_template", "id_user", "security_user");
            DropForeignKey("item_template_detail", "id_user", "security_user");
            DropForeignKey("item_template_detail", "id_template", "item_template");
            DropForeignKey("item_template_detail", "id_company", "app_company");
            DropForeignKey("item_template", "id_company", "app_company");
            DropForeignKey("item_inventory", "id_user", "security_user");
            DropForeignKey("item_inventory_detail", "id_user", "security_user");
            DropForeignKey("item_inventory_detail", "id_item_product", "item_product");
            DropForeignKey("item_inventory_dimension", "id_user", "security_user");
            DropForeignKey("item_inventory_dimension", "id_inventory_detail", "item_inventory_detail");
            DropForeignKey("item_inventory_dimension", "id_dimension", "app_dimension");
            DropForeignKey("item_inventory_dimension", "id_company", "app_company");
            DropForeignKey("item_inventory_detail", "id_inventory", "item_inventory");
            DropForeignKey("item_inventory_detail", "id_location", "app_location");
            DropForeignKey("item_inventory_detail", "id_company", "app_company");
            DropForeignKey("item_inventory", "id_company", "app_company");
            DropForeignKey("item_inventory", "id_branch", "app_branch");
            DropForeignKey("hr_timesheet", "id_user", "security_user");
            DropForeignKey("hr_timesheet", "id_contact", "contacts");
            DropForeignKey("hr_timesheet", "id_company", "app_company");
            DropForeignKey("hr_timesheet", "id_branch", "app_branch");
            DropForeignKey("bi_tag_role", "id_role", "security_role");
            DropForeignKey("bi_tag_role", "id_bi_tag", "bi_tag");
            DropForeignKey("bi_tag_report", "id_bi_tag", "bi_tag");
            DropForeignKey("bi_tag_report", "id_bi_report", "bi_report");
            DropForeignKey("bi_report_detail", "id_bi_report", "bi_report");
            DropForeignKey("bi_chart_report", "id_bi_report", "bi_report");
            DropForeignKey("app_name_template", "id_user", "security_user");
            DropForeignKey("app_name_template_detail", "id_user", "security_user");
            DropForeignKey("app_name_template_detail", "id_name_template", "app_name_template");
            DropForeignKey("app_name_template_detail", "id_company", "app_company");
            DropForeignKey("app_name_template", "id_company", "app_company");
            DropForeignKey("app_configuration", "id_user", "security_user");
            DropForeignKey("app_configuration", "id_company", "app_company");
            DropForeignKey("app_branch_walkins", "id_user", "security_user");
            DropForeignKey("app_branch_walkins", "id_company", "app_company");
            DropForeignKey("app_branch_walkins", "id_branch", "app_branch");
            DropForeignKey("accounting_template", "id_user", "security_user");
            DropForeignKey("accounting_template", "id_company", "app_company");
            DropForeignKey("accounting_template_detail", "id_user", "security_user");
            DropForeignKey("accounting_template_detail", "id_company", "app_company");
            DropForeignKey("accounting_template_detail", "id_template", "accounting_template");
            DropForeignKey("accounting_template_detail", "id_chart", "accounting_chart");
            DropForeignKey("accounting_budget", "id_user", "security_user");
            DropForeignKey("accounting_budget", "id_company", "app_company");
            DropForeignKey("accounting_budget", "id_chart", "accounting_chart");
            DropForeignKey("accounting_chart", "id_user", "security_user");
            DropForeignKey("accounting_chart", "id_tag", "item_tag");
            DropForeignKey("accounting_chart", "id_item_asset_group", "item_asset_group");
            DropForeignKey("accounting_chart", "id_contact", "contacts");
            DropForeignKey("accounting_chart", "parent_id_chart", "accounting_chart");
            DropForeignKey("accounting_chart", "id_vat", "app_vat");
            DropForeignKey("accounting_chart", "id_department", "app_department");
            DropForeignKey("accounting_chart", "id_cost_center", "app_cost_center");
            DropForeignKey("accounting_chart", "id_company", "app_company");
            DropForeignKey("accounting_chart", "id_account", "app_account");
            DropForeignKey("accounting_journal_detail", "id_user", "security_user");
            DropForeignKey("accounting_journal_detail", "id_currencyfx", "app_currencyfx");
            DropForeignKey("accounting_journal_detail", "id_company", "app_company");
            DropForeignKey("accounting_journal", "id_user", "security_user");
            DropForeignKey("accounting_journal", "id_company", "app_company");
            DropForeignKey("accounting_journal", "id_branch", "app_branch");
            DropForeignKey("accounting_journal_detail", "id_journal", "accounting_journal");
            DropForeignKey("accounting_journal", "id_cycle", "accounting_cycle");
            DropForeignKey("accounting_cycle", "id_user", "security_user");
            DropForeignKey("accounting_cycle", "id_company", "app_company");
            DropForeignKey("app_branch", "id_user", "security_user");
            DropForeignKey("app_branch", "id_vat", "app_vat");
            DropForeignKey("app_branch", "id_measurement", "app_measurement");
            DropForeignKey("app_geography", "id_user", "security_user");
            DropForeignKey("app_geography", "parent_id_geography", "app_geography");
            DropForeignKey("app_geography", "id_company", "app_company");
            DropForeignKey("app_branch", "id_geography", "app_geography");
            DropForeignKey("app_bank", "id_user", "security_user");
            DropForeignKey("app_bank", "id_geography", "app_geography");
            DropForeignKey("app_bank", "id_company", "app_company");
            DropForeignKey("app_account", "id_user", "security_user");
            DropForeignKey("app_account", "id_currency", "app_currency");
            DropForeignKey("app_account", "id_company", "app_company");
            DropForeignKey("app_account", "id_bank", "app_bank");
            DropForeignKey("app_account_detail", "id_user", "security_user");
            DropForeignKey("app_account_detail", "id_payment_type", "payment_type");
            DropForeignKey("app_account_detail", "id_currencyfx", "app_currencyfx");
            DropForeignKey("app_account_detail", "id_company", "app_company");
            DropForeignKey("app_account_session", "id_user", "security_user");
            DropForeignKey("security_user", "id_role", "security_role");
            DropForeignKey("security_role_privilage", "id_role", "security_role");
            DropForeignKey("security_role_privilage", "id_privilage", "security_privilage");
            DropForeignKey("security_crud", "id_role", "security_role");
            DropForeignKey("security_role", "id_department", "app_department");
            DropForeignKey("app_department", "id_user", "security_user");
            DropForeignKey("item_request", "security_user_id_user", "security_user");
            DropForeignKey("item_request", "id_sales_order", "sales_order");
            DropForeignKey("item_request", "request_user_id_user", "security_user");
            DropForeignKey("item_request", "id_project", "projects");
            DropForeignKey("item_request", "id_department", "app_department");
            DropForeignKey("app_currency", "id_user", "security_user");
            DropForeignKey("item_request", "id_currency", "app_currency");
            DropForeignKey("impex_expense", "id_user", "security_user");
            DropForeignKey("impex_expense", "id_purchase_invoice", "purchase_invoice");
            DropForeignKey("impexes", "id_user", "security_user");
            DropForeignKey("impexes", "id_incoterm", "impex_incoterm");
            DropForeignKey("impex_incoterm", "id_user", "security_user");
            DropForeignKey("impex_incoterm_detail", "id_user", "security_user");
            DropForeignKey("impex_incoterm_condition", "id_user", "security_user");
            DropForeignKey("impex_incoterm_detail", "id_incoterm_condition", "impex_incoterm_condition");
            DropForeignKey("impex_expense", "id_incoterm_condition", "impex_incoterm_condition");
            DropForeignKey("impex_incoterm_condition", "id_company", "app_company");
            DropForeignKey("impex_incoterm_detail", "id_incoterm", "impex_incoterm");
            DropForeignKey("impex_incoterm_detail", "id_company", "app_company");
            DropForeignKey("impex_incoterm", "id_company", "app_company");
            DropForeignKey("impex_import", "id_user", "security_user");
            DropForeignKey("impex_import", "id_purchase_invoice", "purchase_invoice");
            DropForeignKey("impex_import", "id_impex", "impexes");
            DropForeignKey("impex_import", "id_company", "app_company");
            DropForeignKey("impex_export", "id_user", "security_user");
            DropForeignKey("impex_export", "id_sales_invoice", "sales_invoice");
            DropForeignKey("impex_export", "id_impex", "impexes");
            DropForeignKey("impex_export", "id_company", "app_company");
            DropForeignKey("impex_expense", "id_impex", "impexes");
            DropForeignKey("impexes", "id_contact", "contacts");
            DropForeignKey("contacts", "id_user", "security_user");
            DropForeignKey("contacts", "id_sales_rep", "sales_rep");
            DropForeignKey("contacts", "id_price_list", "item_price_list");
            DropForeignKey("hr_family", "id_user", "security_user");
            DropForeignKey("hr_family", "id_contact", "contacts");
            DropForeignKey("hr_family", "id_company", "app_company");
            DropForeignKey("hr_education", "id_user", "security_user");
            DropForeignKey("hr_education", "id_contact", "contacts");
            DropForeignKey("hr_education", "id_company", "app_company");
            DropForeignKey("hr_contract", "id_user", "security_user");
            DropForeignKey("hr_contract", "id_contact", "contacts");
            DropForeignKey("hr_contract", "id_department", "app_department");
            DropForeignKey("hr_contract", "id_currency", "app_currency");
            DropForeignKey("hr_contract", "id_company", "app_company");
            DropForeignKey("hr_contract", "id_branch", "app_branch");
            DropForeignKey("contact_tag_detail", "id_user", "security_user");
            DropForeignKey("contact_tag", "id_user", "security_user");
            DropForeignKey("contact_tag_detail", "id_tag", "contact_tag");
            DropForeignKey("contact_tag", "id_company", "app_company");
            DropForeignKey("contact_tag_detail", "id_contact", "contacts");
            DropForeignKey("contact_tag_detail", "id_company", "app_company");
            DropForeignKey("contact_subscription", "id_user", "security_user");
            DropForeignKey("items", "id_user", "security_user");
            DropForeignKey("project_event", "id_user", "security_user");
            DropForeignKey("project_event_variable", "id_user", "security_user");
            DropForeignKey("project_event_variable", "id_project_event", "project_event");
            DropForeignKey("project_event_variable", "id_tag", "item_tag");
            DropForeignKey("project_event_variable", "id_item", "items");
            DropForeignKey("project_event_variable", "id_company", "app_company");
            DropForeignKey("project_event_template", "id_user", "security_user");
            DropForeignKey("project_event_template_variable", "id_user", "security_user");
            DropForeignKey("project_event_template_variable", "id_project_event_template", "project_event_template");
            DropForeignKey("project_event_template_variable", "id_tag", "item_tag");
            DropForeignKey("project_event_template_variable", "id_company", "app_company");
            DropForeignKey("project_event_template_fixed", "id_user", "security_user");
            DropForeignKey("project_event_template_fixed", "id_project_event_template", "project_event_template");
            DropForeignKey("project_event_template_fixed", "id_tag", "item_tag");
            DropForeignKey("project_event_template_fixed", "id_company", "app_company");
            DropForeignKey("project_event", "id_project_event_template", "project_event_template");
            DropForeignKey("project_event_template", "id_tag", "item_tag");
            DropForeignKey("project_event_template", "id_company", "app_company");
            DropForeignKey("project_event_fixed", "id_user", "security_user");
            DropForeignKey("project_event_fixed", "id_project_event", "project_event");
            DropForeignKey("project_event_fixed", "id_tag", "item_tag");
            DropForeignKey("project_event_fixed", "id_item", "items");
            DropForeignKey("project_event_fixed", "id_company", "app_company");
            DropForeignKey("project_event", "id_item", "items");
            DropForeignKey("project_event", "id_contact", "contacts");
            DropForeignKey("project_event", "id_company", "app_company");
            DropForeignKey("item_service", "id_user", "security_user");
            DropForeignKey("item_service", "id_item", "items");
            DropForeignKey("item_service", "id_talent", "hr_talent");
            DropForeignKey("hr_talent", "id_user", "security_user");
            DropForeignKey("hr_talent_detail", "id_user", "security_user");
            DropForeignKey("hr_talent_detail", "id_talent", "hr_talent");
            DropForeignKey("hr_talent_detail", "id_contact", "contacts");
            DropForeignKey("hr_talent_detail", "id_company", "app_company");
            DropForeignKey("hr_talent", "id_company", "app_company");
            DropForeignKey("item_service", "id_company", "app_company");
            DropForeignKey("item_recepie", "id_user", "security_user");
            DropForeignKey("item_recepie_detail", "id_user", "security_user");
            DropForeignKey("item_recepie_detail", "item_recepie_id_recepie", "item_recepie");
            DropForeignKey("item_recepie_detail", "item_id_item", "items");
            DropForeignKey("item_recepie_detail", "id_company", "app_company");
            DropForeignKey("item_recepie", "item_id_item", "items");
            DropForeignKey("item_recepie", "id_company", "app_company");
            DropForeignKey("item_property", "id_user", "security_user");
            DropForeignKey("item_property", "id_item", "items");
            DropForeignKey("item_property", "id_property", "app_property");
            DropForeignKey("item_property", "id_company", "app_company");
            DropForeignKey("item_price", "id_user", "security_user");
            DropForeignKey("item_price", "id_price_list", "item_price_list");
            DropForeignKey("item_price_list", "id_user", "security_user");
            DropForeignKey("item_price_list", "ref_price_list_id_price_list", "item_price_list");
            DropForeignKey("item_price_list", "id_company", "app_company");
            DropForeignKey("item_price", "id_item", "items");
            DropForeignKey("item_price", "id_company", "app_company");
            DropForeignKey("items", "id_brand", "item_brand");
            DropForeignKey("item_brand", "id_user", "security_user");
            DropForeignKey("item_brand", "id_contact", "contacts");
            DropForeignKey("item_brand", "id_company", "app_company");
            DropForeignKey("item_attachment", "id_user", "security_user");
            DropForeignKey("item_attachment", "id_item", "items");
            DropForeignKey("item_attachment", "id_company", "app_company");
            DropForeignKey("app_attachment", "id_user", "security_user");
            DropForeignKey("item_attachment", "id_attachment", "app_attachment");
            DropForeignKey("app_attachment", "id_company", "app_company");
            DropForeignKey("contact_subscription", "id_item", "items");
            DropForeignKey("items", "id_vat_group", "app_vat_group");
            DropForeignKey("items", "id_measurement", "app_measurement");
            DropForeignKey("app_measurement", "id_user", "security_user");
            DropForeignKey("item_conversion_factor", "id_user", "security_user");
            DropForeignKey("item_product", "id_user", "security_user");
            DropForeignKey("item_movement", "id_user", "security_user");
            DropForeignKey("item_transfer_detail", "id_user", "security_user");
            DropForeignKey("item_transfer_detail", "id_project_task", "project_task");
            DropForeignKey("item_transfer_dimension", "id_user", "security_user");
            DropForeignKey("item_transfer_dimension", "item_transfer_detail_id_transfer_detail", "item_transfer_detail");
            DropForeignKey("item_transfer_dimension", "id_dimension", "app_dimension");
            DropForeignKey("item_transfer_dimension", "id_company", "app_company");
            DropForeignKey("item_transfer", "user_requested_id_user", "security_user");
            DropForeignKey("item_transfer", "user_given_id_user", "security_user");
            DropForeignKey("item_transfer", "security_user_id_user", "security_user");
            DropForeignKey("item_transfer", "id_project", "projects");
            DropForeignKey("item_transfer_detail", "id_transfer", "item_transfer");
            DropForeignKey("item_transfer", "id_item_request", "item_request");
            DropForeignKey("item_transfer", "employee_id_contact", "contacts");
            DropForeignKey("item_transfer", "id_weather", "app_weather");
            DropForeignKey("item_transfer", "id_terminal", "app_terminal");
            DropForeignKey("item_transfer", "app_location_origin_id_location", "app_location");
            DropForeignKey("item_transfer", "app_location_destination_id_location", "app_location");
            DropForeignKey("item_transfer", "id_range", "app_document_range");
            DropForeignKey("item_transfer", "id_department", "app_department");
            DropForeignKey("item_transfer", "id_company", "app_company");
            DropForeignKey("item_transfer", "app_branch_origin_id_branch", "app_branch");
            DropForeignKey("item_transfer", "app_branch_destination_id_branch", "app_branch");
            DropForeignKey("item_transfer", "app_branch_id_branch", "app_branch");
            DropForeignKey("item_transfer_detail", "id_item_product", "item_product");
            DropForeignKey("item_movement", "id_transfer_detail", "item_transfer_detail");
            DropForeignKey("item_transfer_detail", "id_company", "app_company");
            DropForeignKey("item_movement", "id_item_product", "item_product");
            DropForeignKey("item_movement_value", "id_user", "security_user");
            DropForeignKey("item_movement_value", "id_movement", "item_movement");
            DropForeignKey("item_movement_value", "id_currencyfx", "app_currencyfx");
            DropForeignKey("item_movement_value", "id_company", "app_company");
            DropForeignKey("item_movement_dimension", "id_user", "security_user");
            DropForeignKey("item_movement_dimension", "id_movement", "item_movement");
            DropForeignKey("app_dimension", "id_user", "security_user");
            DropForeignKey("project_task_dimension", "id_user", "security_user");
            DropForeignKey("project_task", "id_user", "security_user");
            DropForeignKey("sales_order_detail", "project_task_id_project_task1", "project_task");
            DropForeignKey("project_task", "sales_detail_id_sales_order_detail", "sales_order_detail");
            DropForeignKey("purchase_invoice_detail", "project_task_id_project_task", "project_task");
            DropForeignKey("project_task_dimension", "id_project_task", "project_task");
            DropForeignKey("project_task", "id_item", "items");
            DropForeignKey("project_task", "parent_id_project_task", "project_task");
            DropForeignKey("project_task", "id_range", "app_document_range");
            DropForeignKey("app_document_range", "id_user", "security_user");
            DropForeignKey("app_document_range", "id_terminal", "app_terminal");
            DropForeignKey("app_document", "id_user", "security_user");
            DropForeignKey("payment_type", "id_user", "security_user");
            DropForeignKey("payment_type_detail", "id_payment_type", "payment_type");
            DropForeignKey("payment_type_detail", "id_user", "security_user");
            DropForeignKey("payment_detail", "id_user", "security_user");
            DropForeignKey("payment_type_detail", "id_payment_detail", "payment_detail");
            DropForeignKey("payment_detail", "id_payment_type", "payment_type");
            DropForeignKey("payment_schedual", "id_user", "security_user");
            DropForeignKey("payment_schedual", "id_sales_order", "sales_order");
            DropForeignKey("purchase_return", "id_user", "security_user");
            DropForeignKey("purchase_return", "id_sales_rep", "sales_rep");
            DropForeignKey("purchase_return_detail", "id_user", "security_user");
            DropForeignKey("purchase_return_dimension", "id_user", "security_user");
            DropForeignKey("purchase_return_dimension", "purchase_return_detail_id_purchase_return_detail", "purchase_return_detail");
            DropForeignKey("purchase_return_dimension", "id_measurement", "app_measurement");
            DropForeignKey("purchase_return_dimension", "id_dimension", "app_dimension");
            DropForeignKey("purchase_return_dimension", "id_company", "app_company");
            DropForeignKey("purchase_return_detail", "id_purchase_return", "purchase_return");
            DropForeignKey("purchase_return_detail", "id_purchase_invoice_detail", "purchase_invoice_detail");
            DropForeignKey("item_movement", "id_purchase_return_detail", "purchase_return_detail");
            DropForeignKey("purchase_return_detail", "id_item", "items");
            DropForeignKey("purchase_return_detail", "id_vat_group", "app_vat_group");
            DropForeignKey("purchase_return_detail", "id_location", "app_location");
            DropForeignKey("purchase_return_detail", "id_cost_center", "app_cost_center");
            DropForeignKey("purchase_return_detail", "id_company", "app_company");
            DropForeignKey("purchase_return", "id_purchase_invoice", "purchase_invoice");
            DropForeignKey("purchase_return", "id_project", "projects");
            DropForeignKey("payment_schedual", "id_purchase_return", "purchase_return");
            DropForeignKey("purchase_return", "newer_id_purchase_return", "purchase_return");
            DropForeignKey("purchase_return", "contact_ref_id_contact", "contacts");
            DropForeignKey("purchase_return", "contact_id_contact", "contacts");
            DropForeignKey("purchase_return", "id_weather", "app_weather");
            DropForeignKey("purchase_return", "id_terminal", "app_terminal");
            DropForeignKey("purchase_return", "id_range", "app_document_range");
            DropForeignKey("purchase_return", "id_currencyfx", "app_currencyfx");
            DropForeignKey("purchase_return", "id_contract", "app_contract");
            DropForeignKey("purchase_return", "id_condition", "app_condition");
            DropForeignKey("purchase_return", "id_company", "app_company");
            DropForeignKey("purchase_return", "id_branch", "app_branch");
            DropForeignKey("payment_schedual", "id_purchase_order", "purchase_order");
            DropForeignKey("purchase_invoice", "id_user", "security_user");
            DropForeignKey("purchase_invoice", "id_sales_rep", "sales_rep");
            DropForeignKey("purchase_invoice_detail", "id_user", "security_user");
            DropForeignKey("purchase_order_detail", "id_user", "security_user");
            DropForeignKey("purchase_order_detail", "id_purchase_tender_detail", "purchase_tender_detail");
            DropForeignKey("purchase_order_dimension", "id_user", "security_user");
            DropForeignKey("purchase_order_dimension", "purchase_order_detail_id_purchase_order_detail", "purchase_order_detail");
            DropForeignKey("purchase_order_dimension", "id_measurement", "app_measurement");
            DropForeignKey("purchase_order_dimension", "id_dimension", "app_dimension");
            DropForeignKey("purchase_order_dimension", "id_company", "app_company");
            DropForeignKey("purchase_order", "id_user", "security_user");
            DropForeignKey("purchase_order", "id_sales_rep", "sales_rep");
            DropForeignKey("purchase_order", "id_purchase_tender", "purchase_tender");
            DropForeignKey("purchase_tender", "id_user", "security_user");
            DropForeignKey("purchase_tender_contact", "id_user", "security_user");
            DropForeignKey("purchase_tender_detail", "id_user", "security_user");
            DropForeignKey("purchase_tender_detail", "id_purchase_tender_item", "purchase_tender_item");
            DropForeignKey("purchase_tender_item", "id_user", "security_user");
            DropForeignKey("purchase_tender_dimension", "id_user", "security_user");
            DropForeignKey("purchase_tender_dimension", "id_purchase_tender_item", "purchase_tender_item");
            DropForeignKey("purchase_tender_dimension", "id_measurement", "app_measurement");
            DropForeignKey("purchase_tender_dimension", "id_dimension", "app_dimension");
            DropForeignKey("purchase_tender_dimension", "id_company", "app_company");
            DropForeignKey("purchase_tender_item", "id_purchase_tender", "purchase_tender");
            DropForeignKey("purchase_tender_item", "id_project_task", "project_task");
            DropForeignKey("purchase_tender_item", "id_item", "items");
            DropForeignKey("purchase_tender_item", "id_company", "app_company");
            DropForeignKey("purchase_tender_detail_dimension", "id_user", "security_user");
            DropForeignKey("purchase_tender_detail_dimension", "id_purchase_tender_detail", "purchase_tender_detail");
            DropForeignKey("purchase_tender_detail_dimension", "id_measurement", "app_measurement");
            DropForeignKey("purchase_tender_detail_dimension", "id_dimension", "app_dimension");
            DropForeignKey("purchase_tender_detail_dimension", "id_company", "app_company");
            DropForeignKey("purchase_tender_detail", "id_purchase_tender_contact", "purchase_tender_contact");
            DropForeignKey("purchase_tender_detail", "id_company", "app_company");
            DropForeignKey("purchase_tender_contact", "id_purchase_tender", "purchase_tender");
            DropForeignKey("purchase_tender_contact", "id_contact", "contacts");
            DropForeignKey("purchase_tender_contact", "id_currencyfx", "app_currencyfx");
            DropForeignKey("purchase_tender_contact", "id_contract", "app_contract");
            DropForeignKey("purchase_tender_contact", "id_condition", "app_condition");
            DropForeignKey("purchase_tender_contact", "id_company", "app_company");
            DropForeignKey("purchase_tender", "id_project", "projects");
            DropForeignKey("purchase_tender", "id_terminal", "app_terminal");
            DropForeignKey("purchase_tender", "id_range", "app_document_range");
            DropForeignKey("purchase_tender", "id_company", "app_company");
            DropForeignKey("purchase_tender", "id_branch", "app_branch");
            DropForeignKey("purchase_order_detail", "id_purchase_order", "purchase_order");
            DropForeignKey("purchase_invoice", "id_purchase_order", "purchase_order");
            DropForeignKey("purchase_order", "id_project", "projects");
            DropForeignKey("purchase_order", "newer_id_purchase_order", "purchase_order");
            DropForeignKey("purchase_order", "contact_ref_id_contact", "contacts");
            DropForeignKey("purchase_order", "contact_id_contact", "contacts");
            DropForeignKey("purchase_order", "id_weather", "app_weather");
            DropForeignKey("purchase_order", "id_terminal", "app_terminal");
            DropForeignKey("purchase_order", "id_range", "app_document_range");
            DropForeignKey("purchase_order", "id_department", "app_department");
            DropForeignKey("purchase_order", "id_currencyfx", "app_currencyfx");
            DropForeignKey("purchase_order", "id_contract", "app_contract");
            DropForeignKey("purchase_order", "id_condition", "app_condition");
            DropForeignKey("purchase_order", "id_company", "app_company");
            DropForeignKey("purchase_order", "id_branch", "app_branch");
            DropForeignKey("purchase_invoice_detail", "id_purchase_order_detail", "purchase_order_detail");
            DropForeignKey("purchase_order_detail", "id_item", "items");
            DropForeignKey("purchase_order_detail", "id_vat_group", "app_vat_group");
            DropForeignKey("purchase_order_detail", "id_location", "app_location");
            DropForeignKey("purchase_order_detail", "id_cost_center", "app_cost_center");
            DropForeignKey("purchase_order_detail", "id_company", "app_company");
            DropForeignKey("purchase_invoice_dimension", "id_user", "security_user");
            DropForeignKey("purchase_invoice_dimension", "purchase_invoice_detail_id_purchase_invoice_detail", "purchase_invoice_detail");
            DropForeignKey("purchase_invoice_dimension", "id_measurement", "app_measurement");
            DropForeignKey("purchase_invoice_dimension", "id_dimension", "app_dimension");
            DropForeignKey("purchase_invoice_dimension", "id_company", "app_company");
            DropForeignKey("purchase_invoice_detail", "id_purchase_invoice", "purchase_invoice");
            DropForeignKey("item_movement", "id_purchase_invoice_detail", "purchase_invoice_detail");
            DropForeignKey("purchase_invoice_detail", "id_item", "items");
            DropForeignKey("purchase_invoice_detail", "id_vat_group", "app_vat_group");
            DropForeignKey("purchase_invoice_detail", "id_location", "app_location");
            DropForeignKey("purchase_invoice_detail", "id_cost_center", "app_cost_center");
            DropForeignKey("purchase_invoice_detail", "id_company", "app_company");
            DropForeignKey("purchase_invoice", "id_project", "projects");
            DropForeignKey("payment_withholding_details", "id_user", "security_user");
            DropForeignKey("payment_withholding_details", "id_purchase_invoice", "purchase_invoice");
            DropForeignKey("payment_withholding_tax", "id_user", "security_user");
            DropForeignKey("payment_withholding_details", "id_withholding", "payment_withholding_tax");
            DropForeignKey("payment_withholding_detail", "id_user", "security_user");
            DropForeignKey("payment_withholding_detail", "id_sales_invoice", "sales_invoice");
            DropForeignKey("sales_invoice", "id_user", "security_user");
            DropForeignKey("sales_invoice", "id_sales_rep", "sales_rep");
            DropForeignKey("sales_invoice", "id_project", "projects");
            DropForeignKey("payment_withholding_details", "id_sales_invoice", "sales_invoice");
            DropForeignKey("payment_schedual", "id_sales_invoice", "sales_invoice");
            DropForeignKey("sales_invoice", "newer_id_sales_invoice", "sales_invoice");
            DropForeignKey("crm_opportunity", "id_user", "security_user");
            DropForeignKey("sales_return", "id_user", "security_user");
            DropForeignKey("sales_return_detail", "id_user", "security_user");
            DropForeignKey("sales_return_detail", "id_sales_return", "sales_return");
            DropForeignKey("sales_return_detail", "sales_invoice_detail_id_sales_invoice_detail", "sales_invoice_detail");
            DropForeignKey("sales_return_detail", "id_project_task", "project_task");
            DropForeignKey("item_movement", "id_sales_return_detail", "sales_return_detail");
            DropForeignKey("sales_return_detail", "id_item", "items");
            DropForeignKey("sales_return_detail", "id_vat_group", "app_vat_group");
            DropForeignKey("sales_return_detail", "id_location", "app_location");
            DropForeignKey("sales_return_detail", "id_company", "app_company");
            DropForeignKey("sales_return", "id_sales_rep", "sales_rep");
            DropForeignKey("sales_return", "id_sales_invoice", "sales_invoice");
            DropForeignKey("sales_return", "id_project", "projects");
            DropForeignKey("payment_schedual", "id_sales_return", "sales_return");
            DropForeignKey("sales_return", "newer_id_sales_return", "sales_return");
            DropForeignKey("sales_return", "id_opportunity", "crm_opportunity");
            DropForeignKey("sales_return", "contact_ref_id_contact", "contacts");
            DropForeignKey("sales_return", "contact_id_contact", "contacts");
            DropForeignKey("sales_return", "id_weather", "app_weather");
            DropForeignKey("sales_return", "id_terminal", "app_terminal");
            DropForeignKey("sales_return", "id_range", "app_document_range");
            DropForeignKey("sales_return", "id_currencyfx", "app_currencyfx");
            DropForeignKey("sales_return", "id_contract", "app_contract");
            DropForeignKey("sales_return", "id_condition", "app_condition");
            DropForeignKey("sales_return", "id_company", "app_company");
            DropForeignKey("sales_return", "id_branch", "app_branch");
            DropForeignKey("sales_return", "id_journal", "accounting_journal");
            DropForeignKey("sales_invoice", "id_opportunity", "crm_opportunity");
            DropForeignKey("sales_budget", "id_user", "security_user");
            DropForeignKey("sales_budget", "id_sales_rep", "sales_rep");
            DropForeignKey("sales_budget", "id_project", "projects");
            DropForeignKey("projects", "id_user", "security_user");
            DropForeignKey("projects", "id_project_template", "project_template");
            DropForeignKey("project_template", "id_user", "security_user");
            DropForeignKey("project_template_detail", "id_user", "security_user");
            DropForeignKey("project_template_detail", "id_project_template", "project_template");
            DropForeignKey("project_template_detail", "id_tag", "item_tag");
            DropForeignKey("item_tag", "id_user", "security_user");
            DropForeignKey("item_tag_detail", "id_user", "security_user");
            DropForeignKey("item_tag_detail", "id_tag", "item_tag");
            DropForeignKey("item_tag_detail", "id_item", "items");
            DropForeignKey("item_tag_detail", "id_company", "app_company");
            DropForeignKey("item_tag", "id_company", "app_company");
            DropForeignKey("project_template_detail", "id_item", "items");
            DropForeignKey("project_template_detail", "parent_id_template_detail", "project_template_detail");
            DropForeignKey("project_template_detail", "id_company", "app_company");
            DropForeignKey("project_template", "id_company", "app_company");
            DropForeignKey("project_task", "id_project", "projects");
            DropForeignKey("project_tag_detail", "id_user", "security_user");
            DropForeignKey("project_tag", "id_user", "security_user");
            DropForeignKey("project_tag_detail", "id_tag", "project_tag");
            DropForeignKey("project_tag", "id_company", "app_company");
            DropForeignKey("project_tag_detail", "id_project", "projects");
            DropForeignKey("project_tag_detail", "id_company", "app_company");
            DropForeignKey("production_order", "id_user", "security_user");
            DropForeignKey("production_order", "id_project", "projects");
            DropForeignKey("production_order_detail", "id_user", "security_user");
            DropForeignKey("production_order_detail", "id_project_task", "project_task");
            DropForeignKey("production_order_dimension", "id_user", "security_user");
            DropForeignKey("production_order_dimension", "id_order_detail", "production_order_detail");
            DropForeignKey("production_order_dimension", "id_measurement", "app_measurement");
            DropForeignKey("production_order_dimension", "id_dimension", "app_dimension");
            DropForeignKey("production_order_dimension", "id_company", "app_company");
            DropForeignKey("production_order_detail", "id_production_order", "production_order");
            DropForeignKey("production_execution_detail", "id_user", "security_user");
            DropForeignKey("production_execution_detail", "id_project_task", "project_task");
            DropForeignKey("production_execution_detail", "id_order_detail", "production_order_detail");
            DropForeignKey("production_execution_dimension", "id_user", "security_user");
            DropForeignKey("production_execution_dimension", "id_execution_detail", "production_execution_detail");
            DropForeignKey("production_execution_dimension", "id_measurement", "app_measurement");
            DropForeignKey("production_execution_dimension", "id_dimension", "app_dimension");
            DropForeignKey("production_execution_dimension", "id_company", "app_company");
            DropForeignKey("item_movement", "id_execution_detail", "production_execution_detail");
            DropForeignKey("production_execution_detail", "id_item", "items");
            DropForeignKey("production_execution_detail", "id_time_coefficient", "hr_time_coefficient");
            DropForeignKey("production_execution_detail", "id_contact", "contacts");
            DropForeignKey("production_execution_detail", "parent_id_execution_detail", "production_execution_detail");
            DropForeignKey("production_execution_detail", "id_company", "app_company");
            DropForeignKey("item_request_detail", "id_user", "security_user");
            DropForeignKey("item_request_detail", "id_sales_order_detail", "sales_order_detail");
            DropForeignKey("sales_order_detail", "id_user", "security_user");
            DropForeignKey("sales_order", "id_user", "security_user");
            DropForeignKey("sales_order", "id_sales_rep", "sales_rep");
            DropForeignKey("sales_rep", "id_user", "security_user");
            DropForeignKey("sales_rep", "id_company", "app_company");
            DropForeignKey("sales_order_detail", "id_sales_order", "sales_order");
            DropForeignKey("sales_invoice", "id_sales_order", "sales_order");
            DropForeignKey("sales_order", "id_sales_budget", "sales_budget");
            DropForeignKey("sales_order", "id_project", "projects");
            DropForeignKey("sales_order", "newer_id_sales_order", "sales_order");
            DropForeignKey("sales_order", "id_opportunity", "crm_opportunity");
            DropForeignKey("sales_order", "contact_ref_id_contact", "contacts");
            DropForeignKey("sales_order", "contact_id_contact", "contacts");
            DropForeignKey("sales_order", "id_weather", "app_weather");
            DropForeignKey("sales_order", "id_terminal", "app_terminal");
            DropForeignKey("sales_order", "id_range", "app_document_range");
            DropForeignKey("sales_order", "id_currencyfx", "app_currencyfx");
            DropForeignKey("sales_order", "id_contract", "app_contract");
            DropForeignKey("sales_order", "id_condition", "app_condition");
            DropForeignKey("sales_order", "id_company", "app_company");
            DropForeignKey("sales_order", "id_branch", "app_branch");
            DropForeignKey("sales_invoice_detail", "id_user", "security_user");
            DropForeignKey("sales_packing_detail", "id_user", "security_user");
            DropForeignKey("sales_packing_relation", "id_sales_packing_detail", "sales_packing_detail");
            DropForeignKey("sales_packing", "id_user", "security_user");
            DropForeignKey("sales_packing_detail", "id_sales_packing", "sales_packing");
            DropForeignKey("sales_packing", "newer_id_sales_packing", "sales_packing");
            DropForeignKey("sales_packing", "id_opportunity", "crm_opportunity");
            DropForeignKey("sales_packing", "id_contact", "contacts");
            DropForeignKey("sales_packing", "id_terminal", "app_terminal");
            DropForeignKey("sales_packing", "id_range", "app_document_range");
            DropForeignKey("sales_packing", "id_company", "app_company");
            DropForeignKey("sales_packing", "id_branch", "app_branch");
            DropForeignKey("sales_packing_detail", "id_sales_order_detail", "sales_order_detail");
            DropForeignKey("item_movement", "id_sales_packing_detail", "sales_packing_detail");
            DropForeignKey("sales_packing_detail", "id_item", "items");
            DropForeignKey("sales_packing_detail", "id_location", "app_location");
            DropForeignKey("sales_packing_detail", "id_company", "app_company");
            DropForeignKey("sales_packing_relation", "sales_invoice_detail_id_sales_invoice_detail", "sales_invoice_detail");
            DropForeignKey("sales_invoice_detail", "id_sales_order_detail", "sales_order_detail");
            DropForeignKey("sales_invoice_detail", "id_sales_invoice", "sales_invoice");
            DropForeignKey("sales_invoice_detail", "id_project_task", "project_task");
            DropForeignKey("item_movement", "id_sales_invoice_detail", "sales_invoice_detail");
            DropForeignKey("sales_invoice_detail", "id_item", "items");
            DropForeignKey("sales_invoice_detail", "id_vat_group", "app_vat_group");
            DropForeignKey("sales_invoice_detail", "id_location", "app_location");
            DropForeignKey("sales_invoice_detail", "id_company", "app_company");
            DropForeignKey("sales_budget_detail", "id_user", "security_user");
            DropForeignKey("sales_order_detail", "id_sales_budget_detail", "sales_budget_detail");
            DropForeignKey("sales_budget_detail", "id_sales_budget", "sales_budget");
            DropForeignKey("sales_budget_detail", "id_project_task", "project_task");
            DropForeignKey("sales_budget_detail", "id_item", "items");
            DropForeignKey("sales_budget_detail", "id_vat_group", "app_vat_group");
            DropForeignKey("sales_budget_detail", "id_location", "app_location");
            DropForeignKey("sales_budget_detail", "id_company", "app_company");
            DropForeignKey("sales_order_detail", "project_task_id_project_task", "project_task");
            DropForeignKey("sales_order_detail", "id_item", "items");
            DropForeignKey("sales_order_detail", "id_vat_group", "app_vat_group");
            DropForeignKey("app_vat_group", "id_user", "security_user");
            DropForeignKey("app_vat_group_details", "id_user", "security_user");
            DropForeignKey("app_vat_group_details", "id_vat_group", "app_vat_group");
            DropForeignKey("app_vat_group_details", "id_vat", "app_vat");
            DropForeignKey("app_vat", "id_user", "security_user");
            DropForeignKey("app_vat", "id_company", "app_company");
            DropForeignKey("app_vat_group_details", "id_company", "app_company");
            DropForeignKey("app_vat_group", "id_company", "app_company");
            DropForeignKey("sales_order_detail", "id_location", "app_location");
            DropForeignKey("sales_order_detail", "id_company", "app_company");
            DropForeignKey("item_request_detail", "id_project_task", "project_task");
            DropForeignKey("item_request_detail", "id_order_detail", "production_order_detail");
            DropForeignKey("item_request_dimension", "id_user", "security_user");
            DropForeignKey("item_request_dimension", "item_request_detail_id_item_request_detail", "item_request_detail");
            DropForeignKey("item_request_dimension", "id_measurement", "app_measurement");
            DropForeignKey("item_request_dimension", "id_dimension", "app_dimension");
            DropForeignKey("item_request_dimension", "id_company", "app_company");
            DropForeignKey("item_request_decision", "id_user", "security_user");
            DropForeignKey("item_request_decision", "id_item_request_detail", "item_request_detail");
            DropForeignKey("item_request_decision", "id_location", "app_location");
            DropForeignKey("item_request_decision", "id_company", "app_company");
            DropForeignKey("item_request_detail", "id_item_request", "item_request");
            DropForeignKey("item_asset_maintainance_detail", "id_user", "security_user");
            DropForeignKey("item_request_detail", "id_maintainance_detail", "item_asset_maintainance_detail");
            DropForeignKey("item_asset_maintainance", "id_user", "security_user");
            DropForeignKey("item_asset_maintainance_detail", "id_maintainance", "item_asset_maintainance");
            DropForeignKey("item_asset", "id_user", "security_user");
            DropForeignKey("item_asset_maintainance", "id_item_asset", "item_asset");
            DropForeignKey("item_asset", "id_item_asset_group", "item_asset_group");
            DropForeignKey("item_asset_group", "id_user", "security_user");
            DropForeignKey("item_asset_group", "id_company", "app_company");
            DropForeignKey("item_asset", "id_item", "items");
            DropForeignKey("item_asset", "id_contact", "contacts");
            DropForeignKey("item_asset", "id_company", "app_company");
            DropForeignKey("item_asset", "id_branch", "app_branch");
            DropForeignKey("item_asset_maintainance", "id_company", "app_company");
            DropForeignKey("item_asset_maintainance_detail", "id_item", "items");
            DropForeignKey("hr_time_coefficient", "id_user", "security_user");
            DropForeignKey("item_asset_maintainance_detail", "id_time_coefficient", "hr_time_coefficient");
            DropForeignKey("hr_time_coefficient", "id_company", "app_company");
            DropForeignKey("item_asset_maintainance_detail", "id_contact", "contacts");
            DropForeignKey("item_asset_maintainance_detail", "id_currencyfx", "app_currencyfx");
            DropForeignKey("item_asset_maintainance_detail", "id_company", "app_company");
            DropForeignKey("item_request_detail", "id_item", "items");
            DropForeignKey("item_request_detail", "id_company", "app_company");
            DropForeignKey("production_order_detail", "id_item", "items");
            DropForeignKey("production_order_detail", "parent_id_order_detail", "production_order_detail");
            DropForeignKey("production_order_detail", "id_company", "app_company");
            DropForeignKey("production_line", "id_user", "security_user");
            DropForeignKey("production_order", "id_production_line", "production_line");
            DropForeignKey("production_line", "id_location", "app_location");
            DropForeignKey("production_line", "id_company", "app_company");
            DropForeignKey("item_request", "id_production_order", "production_order");
            DropForeignKey("production_order", "id_range", "app_document_range");
            DropForeignKey("production_order", "id_company", "app_company");
            DropForeignKey("projects", "id_contact", "contacts");
            DropForeignKey("projects", "id_currency", "app_currency");
            DropForeignKey("projects", "id_company", "app_company");
            DropForeignKey("projects", "id_branch", "app_branch");
            DropForeignKey("sales_budget", "newer_id_sales_budget", "sales_budget");
            DropForeignKey("sales_budget", "id_opportunity", "crm_opportunity");
            DropForeignKey("sales_budget", "contact_ref_id_contact", "contacts");
            DropForeignKey("sales_budget", "contact_id_contact", "contacts");
            DropForeignKey("sales_budget", "id_weather", "app_weather");
            DropForeignKey("sales_budget", "id_terminal", "app_terminal");
            DropForeignKey("sales_budget", "id_range", "app_document_range");
            DropForeignKey("sales_budget", "id_currencyfx", "app_currencyfx");
            DropForeignKey("sales_budget", "id_contract", "app_contract");
            DropForeignKey("sales_budget", "id_condition", "app_condition");
            DropForeignKey("sales_budget", "id_company", "app_company");
            DropForeignKey("sales_budget", "id_branch", "app_branch");
            DropForeignKey("crm_opportunity", "id_contact", "contacts");
            DropForeignKey("crm_opportunity", "id_company", "app_company");
            DropForeignKey("sales_invoice", "contact_ref_id_contact", "contacts");
            DropForeignKey("sales_invoice", "contact_id_contact", "contacts");
            DropForeignKey("sales_invoice", "id_weather", "app_weather");
            DropForeignKey("sales_invoice", "id_terminal", "app_terminal");
            DropForeignKey("sales_invoice", "id_range", "app_document_range");
            DropForeignKey("sales_invoice", "id_currencyfx", "app_currencyfx");
            DropForeignKey("sales_invoice", "id_contract", "app_contract");
            DropForeignKey("sales_invoice", "id_condition", "app_condition");
            DropForeignKey("sales_invoice", "id_company", "app_company");
            DropForeignKey("sales_invoice", "id_branch", "app_branch");
            DropForeignKey("sales_invoice", "id_journal", "accounting_journal");
            DropForeignKey("payment_withholding_detail", "id_purchase_invoice", "purchase_invoice");
            DropForeignKey("payment_withholding_detail", "id_withholding", "payment_withholding_tax");
            DropForeignKey("payment_withholding_detail", "id_company", "app_company");
            DropForeignKey("payment_withholding_tax", "id_contact", "contacts");
            DropForeignKey("payment_withholding_tax", "id_range", "app_document_range");
            DropForeignKey("payment_withholding_tax", "id_currencyfx", "app_currencyfx");
            DropForeignKey("payment_withholding_tax", "id_company", "app_company");
            DropForeignKey("payment_withholding_tax", "id_journal", "accounting_journal");
            DropForeignKey("payment_withholding_details", "id_company", "app_company");
            DropForeignKey("payment_schedual", "id_purchase_invoice", "purchase_invoice");
            DropForeignKey("purchase_invoice", "newer_id_purchase_invoice", "purchase_invoice");
            DropForeignKey("purchase_invoice", "contact_ref_id_contact", "contacts");
            DropForeignKey("purchase_invoice", "contact_id_contact", "contacts");
            DropForeignKey("purchase_invoice", "id_weather", "app_weather");
            DropForeignKey("app_weather", "id_branch", "app_branch");
            DropForeignKey("purchase_invoice", "id_terminal", "app_terminal");
            DropForeignKey("purchase_invoice", "id_range", "app_document_range");
            DropForeignKey("purchase_invoice", "id_department", "app_department");
            DropForeignKey("purchase_invoice", "id_currencyfx", "app_currencyfx");
            DropForeignKey("purchase_invoice", "id_contract", "app_contract");
            DropForeignKey("purchase_invoice", "id_condition", "app_condition");
            DropForeignKey("purchase_invoice", "id_company", "app_company");
            DropForeignKey("purchase_invoice", "id_branch", "app_branch");
            DropForeignKey("purchase_invoice", "id_journal", "accounting_journal");
            DropForeignKey("payment_promissory_note", "id_user", "security_user");
            DropForeignKey("payment_schedual", "id_note", "payment_promissory_note");
            DropForeignKey("payment_promissory_note", "id_contact", "contacts");
            DropForeignKey("payment_promissory_note", "id_terminal", "app_terminal");
            DropForeignKey("payment_promissory_note", "id_range", "app_document_range");
            DropForeignKey("payment_promissory_note", "id_currencyfx", "app_currencyfx");
            DropForeignKey("payment_promissory_note", "id_company", "app_company");
            DropForeignKey("payment_promissory_note", "id_branch", "app_branch");
            DropForeignKey("payment_schedual", "id_payment_detail", "payment_detail");
            DropForeignKey("payment_schedual", "id_contact", "contacts");
            DropForeignKey("payment_schedual", "parent_id_payment_schedual", "payment_schedual");
            DropForeignKey("payment_schedual", "id_currencyfx", "app_currencyfx");
            DropForeignKey("payment_schedual", "id_company", "app_company");
            DropForeignKey("payments", "id_user", "security_user");
            DropForeignKey("payment_detail", "id_payment", "payments");
            DropForeignKey("payments", "id_contact", "contacts");
            DropForeignKey("payments", "id_terminal", "app_terminal");
            DropForeignKey("app_terminal", "id_user", "security_user");
            DropForeignKey("app_terminal", "id_company", "app_company");
            DropForeignKey("app_terminal", "id_branch", "app_branch");
            DropForeignKey("app_account", "id_terminal", "app_terminal");
            DropForeignKey("payments", "id_range", "app_document_range");
            DropForeignKey("payments", "id_company", "app_company");
            DropForeignKey("payments", "id_branch", "app_branch");
            DropForeignKey("payments", "id_journal", "accounting_journal");
            DropForeignKey("payment_detail", "id_range", "app_document_range");
            DropForeignKey("payment_detail", "id_currencyfx", "app_currencyfx");
            DropForeignKey("payment_detail", "id_company", "app_company");
            DropForeignKey("payment_detail", "app_bank_id_bank", "app_bank");
            DropForeignKey("app_account_detail", "id_payment_detail", "payment_detail");
            DropForeignKey("payment_detail", "id_account", "app_account");
            DropForeignKey("payment_type_detail", "id_field", "app_field");
            DropForeignKey("payment_type_detail", "id_company", "app_company");
            DropForeignKey("payment_type", "id_document", "app_document");
            DropForeignKey("payment_type", "id_company", "app_company");
            DropForeignKey("app_document_range", "id_document", "app_document");
            DropForeignKey("app_document", "id_company", "app_company");
            DropForeignKey("app_document_range", "id_company", "app_company");
            DropForeignKey("app_document_range", "id_branch", "app_branch");
            DropForeignKey("project_task", "id_company", "app_company");
            DropForeignKey("project_task_dimension", "id_measurement", "app_measurement");
            DropForeignKey("project_task_dimension", "id_dimension", "app_dimension");
            DropForeignKey("project_task_dimension", "id_company", "app_company");
            DropForeignKey("item_movement_dimension", "id_dimension", "app_dimension");
            DropForeignKey("item_dimension", "id_user", "security_user");
            DropForeignKey("item_dimension", "id_item", "items");
            DropForeignKey("item_dimension", "id_measurement", "app_measurement");
            DropForeignKey("item_dimension", "app_dimension_id_dimension", "app_dimension");
            DropForeignKey("item_dimension", "id_company", "app_company");
            DropForeignKey("app_dimension", "id_company", "app_company");
            DropForeignKey("item_movement_dimension", "id_company", "app_company");
            DropForeignKey("item_movement", "parent_id_movement", "item_movement");
            DropForeignKey("app_location", "id_user", "security_user");
            DropForeignKey("item_movement", "id_location", "app_location");
            DropForeignKey("app_location", "id_contact", "contacts");
            DropForeignKey("app_location", "id_company", "app_company");
            DropForeignKey("app_location", "id_branch", "app_branch");
            DropForeignKey("item_movement", "id_company", "app_company");
            DropForeignKey("item_conversion_factor", "id_item_product", "item_product");
            DropForeignKey("item_product", "id_item", "items");
            DropForeignKey("item_product", "id_company", "app_company");
            DropForeignKey("item_conversion_factor", "id_measurement", "app_measurement");
            DropForeignKey("item_conversion_factor", "id_company", "app_company");
            DropForeignKey("app_measurement_type", "id_user", "security_user");
            DropForeignKey("app_measurement", "id_measurement_type", "app_measurement_type");
            DropForeignKey("app_measurement_type", "id_company", "app_company");
            DropForeignKey("app_measurement", "id_company", "app_company");
            DropForeignKey("items", "id_company", "app_company");
            DropForeignKey("contact_subscription", "id_contact", "contacts");
            DropForeignKey("contact_subscription", "id_company", "app_company");
            DropForeignKey("contact_role", "id_user", "security_user");
            DropForeignKey("contacts", "id_contact_role", "contact_role");
            DropForeignKey("contact_role", "id_company", "app_company");
            DropForeignKey("contact_field_value", "id_user", "security_user");
            DropForeignKey("contact_field_value", "id_contact", "contacts");
            DropForeignKey("contact_field_value", "id_field", "app_field");
            DropForeignKey("app_field", "id_user", "security_user");
            DropForeignKey("app_field", "id_company", "app_company");
            DropForeignKey("contact_field_value", "id_company", "app_company");
            DropForeignKey("contacts", "parent_id_contact", "contacts");
            DropForeignKey("contacts", "id_geography", "app_geography");
            DropForeignKey("contacts", "id_currency", "app_currency");
            DropForeignKey("contacts", "id_cost_center", "app_cost_center");
            DropForeignKey("app_cost_center", "id_user", "security_user");
            DropForeignKey("app_cost_center", "id_company", "app_company");
            DropForeignKey("contacts", "id_contract", "app_contract");
            DropForeignKey("app_contract", "id_user", "security_user");
            DropForeignKey("app_contract_detail", "id_user", "security_user");
            DropForeignKey("app_contract_detail", "id_contract", "app_contract");
            DropForeignKey("app_contract_detail", "id_company", "app_company");
            DropForeignKey("app_condition", "id_user", "security_user");
            DropForeignKey("app_contract", "id_condition", "app_condition");
            DropForeignKey("app_condition", "id_company", "app_company");
            DropForeignKey("app_contract", "id_company", "app_company");
            DropForeignKey("contacts", "id_company", "app_company");
            DropForeignKey("contacts", "id_bank", "app_bank");
            DropForeignKey("impexes", "id_company", "app_company");
            DropForeignKey("impex_expense", "id_currency", "app_currency");
            DropForeignKey("impex_expense", "id_company", "app_company");
            DropForeignKey("app_currencyfx", "id_user", "security_user");
            DropForeignKey("app_currencyfx", "id_currency", "app_currency");
            DropForeignKey("app_currencyfx", "id_company", "app_company");
            DropForeignKey("app_currency_denomination", "id_user", "security_user");
            DropForeignKey("app_currency_denomination", "id_currency", "app_currency");
            DropForeignKey("app_currency_denomination", "id_company", "app_company");
            DropForeignKey("app_currency", "id_company", "app_company");
            DropForeignKey("item_request", "id_company", "app_company");
            DropForeignKey("item_request", "id_branch", "app_branch");
            DropForeignKey("hr_position", "id_user", "security_user");
            DropForeignKey("hr_position", "parent_id_position", "hr_position");
            DropForeignKey("hr_position", "id_department", "app_department");
            DropForeignKey("hr_position", "id_company", "app_company");
            DropForeignKey("app_department", "id_company", "app_company");
            DropForeignKey("security_role", "id_company", "app_company");
            DropForeignKey("security_user", "id_question", "security_question");
            DropForeignKey("security_user", "parent_id_user", "security_user");
            DropForeignKey("security_user", "id_company", "app_company");
            DropForeignKey("app_account_session", "id_company", "app_company");
            DropForeignKey("app_account_detail", "id_session", "app_account_session");
            DropForeignKey("app_account_session", "id_account", "app_account");
            DropForeignKey("app_account_detail", "id_account", "app_account");
            DropForeignKey("app_branch", "id_company", "app_company");
            DropForeignKey("accounting_budget", "id_cycle", "accounting_cycle");
            DropForeignKey("accounting_journal_detail", "id_chart", "accounting_chart");
            DropIndex("sales_promotion", new[] { "id_user" });
            DropIndex("sales_promotion", new[] { "id_company" });
            DropIndex("purchase_packing_relation", new[] { "id_purchase_packing" });
            DropIndex("purchase_packing_relation", new[] { "id_purchase_invoice" });
            DropIndex("purchase_packing_dimension", new[] { "purchase_packing_detail_id_purchase_packing_detail" });
            DropIndex("purchase_packing_dimension", new[] { "id_user" });
            DropIndex("purchase_packing_dimension", new[] { "id_company" });
            DropIndex("purchase_packing_dimension", new[] { "id_measurement" });
            DropIndex("purchase_packing_dimension", new[] { "id_dimension" });
            DropIndex("purchase_packing_detail", new[] { "id_user" });
            DropIndex("purchase_packing_detail", new[] { "id_company" });
            DropIndex("purchase_packing_detail", new[] { "id_item" });
            DropIndex("purchase_packing_detail", new[] { "id_location" });
            DropIndex("purchase_packing_detail", new[] { "id_purchase_order_detail" });
            DropIndex("purchase_packing_detail", new[] { "id_purchase_packing" });
            DropIndex("purchase_packing", new[] { "newer_id_purchase_packing" });
            DropIndex("purchase_packing", new[] { "app_terminal_id_terminal" });
            DropIndex("purchase_packing", new[] { "id_user" });
            DropIndex("purchase_packing", new[] { "id_company" });
            DropIndex("purchase_packing", new[] { "id_branch" });
            DropIndex("purchase_packing", new[] { "id_contact" });
            DropIndex("item_template_detail", new[] { "id_user" });
            DropIndex("item_template_detail", new[] { "id_company" });
            DropIndex("item_template_detail", new[] { "id_template" });
            DropIndex("item_template", new[] { "id_user" });
            DropIndex("item_template", new[] { "id_company" });
            DropIndex("item_inventory_dimension", new[] { "id_user" });
            DropIndex("item_inventory_dimension", new[] { "id_company" });
            DropIndex("item_inventory_dimension", new[] { "id_dimension" });
            DropIndex("item_inventory_dimension", new[] { "id_inventory_detail" });
            DropIndex("item_inventory_detail", new[] { "id_user" });
            DropIndex("item_inventory_detail", new[] { "id_company" });
            DropIndex("item_inventory_detail", new[] { "id_item_product" });
            DropIndex("item_inventory_detail", new[] { "id_location" });
            DropIndex("item_inventory_detail", new[] { "id_inventory" });
            DropIndex("item_inventory", new[] { "id_user" });
            DropIndex("item_inventory", new[] { "id_company" });
            DropIndex("item_inventory", new[] { "id_branch" });
            DropIndex("hr_timesheet", new[] { "id_user" });
            DropIndex("hr_timesheet", new[] { "id_company" });
            DropIndex("hr_timesheet", new[] { "id_branch" });
            DropIndex("hr_timesheet", new[] { "id_contact" });
            DropIndex("bi_tag_role", new[] { "id_role" });
            DropIndex("bi_tag_role", new[] { "id_bi_tag" });
            DropIndex("bi_tag_report", new[] { "id_bi_report" });
            DropIndex("bi_tag_report", new[] { "id_bi_tag" });
            DropIndex("bi_report_detail", new[] { "id_bi_report" });
            DropIndex("bi_chart_report", new[] { "id_bi_report" });
            DropIndex("app_name_template_detail", new[] { "id_user" });
            DropIndex("app_name_template_detail", new[] { "id_company" });
            DropIndex("app_name_template_detail", new[] { "id_name_template" });
            DropIndex("app_name_template", new[] { "id_user" });
            DropIndex("app_name_template", new[] { "id_company" });
            DropIndex("app_configuration", new[] { "id_user" });
            DropIndex("app_configuration", new[] { "id_company" });
            DropIndex("app_branch_walkins", new[] { "id_user" });
            DropIndex("app_branch_walkins", new[] { "id_company" });
            DropIndex("app_branch_walkins", new[] { "id_branch" });
            DropIndex("accounting_template_detail", new[] { "id_user" });
            DropIndex("accounting_template_detail", new[] { "id_company" });
            DropIndex("accounting_template_detail", new[] { "id_chart" });
            DropIndex("accounting_template_detail", new[] { "id_template" });
            DropIndex("accounting_template", new[] { "id_user" });
            DropIndex("accounting_template", new[] { "id_company" });
            DropIndex("security_role_privilage", new[] { "id_privilage" });
            DropIndex("security_role_privilage", new[] { "id_role" });
            DropIndex("security_crud", new[] { "id_role" });
            DropIndex("impex_incoterm_condition", new[] { "id_user" });
            DropIndex("impex_incoterm_condition", new[] { "id_company" });
            DropIndex("impex_incoterm_detail", new[] { "id_user" });
            DropIndex("impex_incoterm_detail", new[] { "id_company" });
            DropIndex("impex_incoterm_detail", new[] { "id_incoterm_condition" });
            DropIndex("impex_incoterm_detail", new[] { "id_incoterm" });
            DropIndex("impex_incoterm", new[] { "id_user" });
            DropIndex("impex_incoterm", new[] { "id_company" });
            DropIndex("impex_import", new[] { "id_user" });
            DropIndex("impex_import", new[] { "id_company" });
            DropIndex("impex_import", new[] { "id_purchase_invoice" });
            DropIndex("impex_import", new[] { "id_impex" });
            DropIndex("impex_export", new[] { "id_user" });
            DropIndex("impex_export", new[] { "id_company" });
            DropIndex("impex_export", new[] { "id_sales_invoice" });
            DropIndex("impex_export", new[] { "id_impex" });
            DropIndex("hr_family", new[] { "id_user" });
            DropIndex("hr_family", new[] { "id_company" });
            DropIndex("hr_family", new[] { "id_contact" });
            DropIndex("hr_education", new[] { "id_user" });
            DropIndex("hr_education", new[] { "id_company" });
            DropIndex("hr_education", new[] { "id_contact" });
            DropIndex("hr_contract", new[] { "id_user" });
            DropIndex("hr_contract", new[] { "id_company" });
            DropIndex("hr_contract", new[] { "id_currency" });
            DropIndex("hr_contract", new[] { "id_department" });
            DropIndex("hr_contract", new[] { "id_branch" });
            DropIndex("hr_contract", new[] { "id_contact" });
            DropIndex("contact_tag", new[] { "id_user" });
            DropIndex("contact_tag", new[] { "id_company" });
            DropIndex("contact_tag_detail", new[] { "id_user" });
            DropIndex("contact_tag_detail", new[] { "id_company" });
            DropIndex("contact_tag_detail", new[] { "id_tag" });
            DropIndex("contact_tag_detail", new[] { "id_contact" });
            DropIndex("project_event_variable", new[] { "id_user" });
            DropIndex("project_event_variable", new[] { "id_company" });
            DropIndex("project_event_variable", new[] { "id_item" });
            DropIndex("project_event_variable", new[] { "id_tag" });
            DropIndex("project_event_variable", new[] { "id_project_event" });
            DropIndex("project_event_template_variable", new[] { "id_user" });
            DropIndex("project_event_template_variable", new[] { "id_company" });
            DropIndex("project_event_template_variable", new[] { "id_tag" });
            DropIndex("project_event_template_variable", new[] { "id_project_event_template" });
            DropIndex("project_event_template_fixed", new[] { "id_user" });
            DropIndex("project_event_template_fixed", new[] { "id_company" });
            DropIndex("project_event_template_fixed", new[] { "id_tag" });
            DropIndex("project_event_template_fixed", new[] { "id_project_event_template" });
            DropIndex("project_event_template", new[] { "id_user" });
            DropIndex("project_event_template", new[] { "id_company" });
            DropIndex("project_event_template", new[] { "id_tag" });
            DropIndex("project_event_fixed", new[] { "id_user" });
            DropIndex("project_event_fixed", new[] { "id_company" });
            DropIndex("project_event_fixed", new[] { "id_item" });
            DropIndex("project_event_fixed", new[] { "id_tag" });
            DropIndex("project_event_fixed", new[] { "id_project_event" });
            DropIndex("project_event", new[] { "id_user" });
            DropIndex("project_event", new[] { "id_company" });
            DropIndex("project_event", new[] { "id_contact" });
            DropIndex("project_event", new[] { "id_item" });
            DropIndex("project_event", new[] { "id_project_event_template" });
            DropIndex("hr_talent_detail", new[] { "id_user" });
            DropIndex("hr_talent_detail", new[] { "id_company" });
            DropIndex("hr_talent_detail", new[] { "id_contact" });
            DropIndex("hr_talent_detail", new[] { "id_talent" });
            DropIndex("hr_talent", new[] { "id_user" });
            DropIndex("hr_talent", new[] { "id_company" });
            DropIndex("item_service", new[] { "id_user" });
            DropIndex("item_service", new[] { "id_company" });
            DropIndex("item_service", new[] { "id_talent" });
            DropIndex("item_service", new[] { "id_item" });
            DropIndex("item_recepie_detail", new[] { "item_recepie_id_recepie" });
            DropIndex("item_recepie_detail", new[] { "item_id_item" });
            DropIndex("item_recepie_detail", new[] { "id_user" });
            DropIndex("item_recepie_detail", new[] { "id_company" });
            DropIndex("item_recepie", new[] { "item_id_item" });
            DropIndex("item_recepie", new[] { "id_user" });
            DropIndex("item_recepie", new[] { "id_company" });
            DropIndex("item_property", new[] { "id_user" });
            DropIndex("item_property", new[] { "id_company" });
            DropIndex("item_property", new[] { "id_property" });
            DropIndex("item_property", new[] { "id_item" });
            DropIndex("item_price_list", new[] { "ref_price_list_id_price_list" });
            DropIndex("item_price_list", new[] { "id_user" });
            DropIndex("item_price_list", new[] { "id_company" });
            DropIndex("item_price", new[] { "id_user" });
            DropIndex("item_price", new[] { "id_company" });
            DropIndex("item_price", new[] { "id_price_list" });
            DropIndex("item_price", new[] { "id_item" });
            DropIndex("item_brand", new[] { "id_user" });
            DropIndex("item_brand", new[] { "id_company" });
            DropIndex("item_brand", new[] { "id_contact" });
            DropIndex("app_attachment", new[] { "id_user" });
            DropIndex("app_attachment", new[] { "id_company" });
            DropIndex("item_attachment", new[] { "id_user" });
            DropIndex("item_attachment", new[] { "id_company" });
            DropIndex("item_attachment", new[] { "id_attachment" });
            DropIndex("item_attachment", new[] { "id_item" });
            DropIndex("item_transfer_dimension", new[] { "item_transfer_detail_id_transfer_detail" });
            DropIndex("item_transfer_dimension", new[] { "id_user" });
            DropIndex("item_transfer_dimension", new[] { "id_company" });
            DropIndex("item_transfer_dimension", new[] { "id_dimension" });
            DropIndex("item_transfer", new[] { "user_requested_id_user" });
            DropIndex("item_transfer", new[] { "user_given_id_user" });
            DropIndex("item_transfer", new[] { "security_user_id_user" });
            DropIndex("item_transfer", new[] { "employee_id_contact" });
            DropIndex("item_transfer", new[] { "app_location_origin_id_location" });
            DropIndex("item_transfer", new[] { "app_location_destination_id_location" });
            DropIndex("item_transfer", new[] { "app_branch_origin_id_branch" });
            DropIndex("item_transfer", new[] { "app_branch_destination_id_branch" });
            DropIndex("item_transfer", new[] { "app_branch_id_branch" });
            DropIndex("item_transfer", new[] { "id_company" });
            DropIndex("item_transfer", new[] { "id_terminal" });
            DropIndex("item_transfer", new[] { "id_range" });
            DropIndex("item_transfer", new[] { "id_department" });
            DropIndex("item_transfer", new[] { "id_project" });
            DropIndex("item_transfer", new[] { "id_item_request" });
            DropIndex("item_transfer", new[] { "id_weather" });
            DropIndex("item_transfer_detail", new[] { "id_user" });
            DropIndex("item_transfer_detail", new[] { "id_company" });
            DropIndex("item_transfer_detail", new[] { "id_item_product" });
            DropIndex("item_transfer_detail", new[] { "id_project_task" });
            DropIndex("item_transfer_detail", new[] { "id_transfer" });
            DropIndex("item_movement_value", new[] { "id_user" });
            DropIndex("item_movement_value", new[] { "id_company" });
            DropIndex("item_movement_value", new[] { "id_currencyfx" });
            DropIndex("item_movement_value", new[] { "id_movement" });
            DropIndex("purchase_return_dimension", new[] { "purchase_return_detail_id_purchase_return_detail" });
            DropIndex("purchase_return_dimension", new[] { "id_user" });
            DropIndex("purchase_return_dimension", new[] { "id_company" });
            DropIndex("purchase_return_dimension", new[] { "id_measurement" });
            DropIndex("purchase_return_dimension", new[] { "id_dimension" });
            DropIndex("purchase_return_detail", new[] { "id_user" });
            DropIndex("purchase_return_detail", new[] { "id_company" });
            DropIndex("purchase_return_detail", new[] { "id_vat_group" });
            DropIndex("purchase_return_detail", new[] { "id_item" });
            DropIndex("purchase_return_detail", new[] { "id_cost_center" });
            DropIndex("purchase_return_detail", new[] { "id_location" });
            DropIndex("purchase_return_detail", new[] { "id_purchase_invoice_detail" });
            DropIndex("purchase_return_detail", new[] { "id_purchase_return" });
            DropIndex("purchase_return", new[] { "newer_id_purchase_return" });
            DropIndex("purchase_return", new[] { "contact_ref_id_contact" });
            DropIndex("purchase_return", new[] { "contact_id_contact" });
            DropIndex("purchase_return", new[] { "id_user" });
            DropIndex("purchase_return", new[] { "id_company" });
            DropIndex("purchase_return", new[] { "id_project" });
            DropIndex("purchase_return", new[] { "id_range" });
            DropIndex("purchase_return", new[] { "id_condition" });
            DropIndex("purchase_return", new[] { "id_contract" });
            DropIndex("purchase_return", new[] { "id_terminal" });
            DropIndex("purchase_return", new[] { "id_branch" });
            DropIndex("purchase_return", new[] { "id_weather" });
            DropIndex("purchase_return", new[] { "id_sales_rep" });
            DropIndex("purchase_return", new[] { "id_currencyfx" });
            DropIndex("purchase_return", new[] { "id_purchase_invoice" });
            DropIndex("purchase_order_dimension", new[] { "purchase_order_detail_id_purchase_order_detail" });
            DropIndex("purchase_order_dimension", new[] { "id_user" });
            DropIndex("purchase_order_dimension", new[] { "id_company" });
            DropIndex("purchase_order_dimension", new[] { "id_measurement" });
            DropIndex("purchase_order_dimension", new[] { "id_dimension" });
            DropIndex("purchase_tender_dimension", new[] { "id_user" });
            DropIndex("purchase_tender_dimension", new[] { "id_company" });
            DropIndex("purchase_tender_dimension", new[] { "id_measurement" });
            DropIndex("purchase_tender_dimension", new[] { "id_dimension" });
            DropIndex("purchase_tender_dimension", new[] { "id_purchase_tender_item" });
            DropIndex("purchase_tender_item", new[] { "id_user" });
            DropIndex("purchase_tender_item", new[] { "id_company" });
            DropIndex("purchase_tender_item", new[] { "id_project_task" });
            DropIndex("purchase_tender_item", new[] { "id_item" });
            DropIndex("purchase_tender_item", new[] { "id_purchase_tender" });
            DropIndex("purchase_tender_detail_dimension", new[] { "id_user" });
            DropIndex("purchase_tender_detail_dimension", new[] { "id_company" });
            DropIndex("purchase_tender_detail_dimension", new[] { "id_measurement" });
            DropIndex("purchase_tender_detail_dimension", new[] { "id_dimension" });
            DropIndex("purchase_tender_detail_dimension", new[] { "id_purchase_tender_detail" });
            DropIndex("purchase_tender_detail", new[] { "id_user" });
            DropIndex("purchase_tender_detail", new[] { "id_company" });
            DropIndex("purchase_tender_detail", new[] { "id_purchase_tender_item" });
            DropIndex("purchase_tender_detail", new[] { "id_purchase_tender_contact" });
            DropIndex("purchase_tender_contact", new[] { "id_user" });
            DropIndex("purchase_tender_contact", new[] { "id_company" });
            DropIndex("purchase_tender_contact", new[] { "id_currencyfx" });
            DropIndex("purchase_tender_contact", new[] { "id_condition" });
            DropIndex("purchase_tender_contact", new[] { "id_contract" });
            DropIndex("purchase_tender_contact", new[] { "id_contact" });
            DropIndex("purchase_tender_contact", new[] { "id_purchase_tender" });
            DropIndex("purchase_tender", new[] { "id_user" });
            DropIndex("purchase_tender", new[] { "id_company" });
            DropIndex("purchase_tender", new[] { "id_terminal" });
            DropIndex("purchase_tender", new[] { "id_range" });
            DropIndex("purchase_tender", new[] { "id_project" });
            DropIndex("purchase_tender", new[] { "id_branch" });
            DropIndex("purchase_order", new[] { "newer_id_purchase_order" });
            DropIndex("purchase_order", new[] { "contact_ref_id_contact" });
            DropIndex("purchase_order", new[] { "contact_id_contact" });
            DropIndex("purchase_order", new[] { "id_user" });
            DropIndex("purchase_order", new[] { "id_company" });
            DropIndex("purchase_order", new[] { "id_project" });
            DropIndex("purchase_order", new[] { "id_range" });
            DropIndex("purchase_order", new[] { "id_condition" });
            DropIndex("purchase_order", new[] { "id_contract" });
            DropIndex("purchase_order", new[] { "id_terminal" });
            DropIndex("purchase_order", new[] { "id_branch" });
            DropIndex("purchase_order", new[] { "id_weather" });
            DropIndex("purchase_order", new[] { "id_sales_rep" });
            DropIndex("purchase_order", new[] { "id_currencyfx" });
            DropIndex("purchase_order", new[] { "id_department" });
            DropIndex("purchase_order", new[] { "id_purchase_tender" });
            DropIndex("purchase_order_detail", new[] { "id_user" });
            DropIndex("purchase_order_detail", new[] { "id_company" });
            DropIndex("purchase_order_detail", new[] { "id_vat_group" });
            DropIndex("purchase_order_detail", new[] { "id_item" });
            DropIndex("purchase_order_detail", new[] { "id_cost_center" });
            DropIndex("purchase_order_detail", new[] { "id_location" });
            DropIndex("purchase_order_detail", new[] { "id_purchase_tender_detail" });
            DropIndex("purchase_order_detail", new[] { "id_purchase_order" });
            DropIndex("purchase_invoice_dimension", new[] { "purchase_invoice_detail_id_purchase_invoice_detail" });
            DropIndex("purchase_invoice_dimension", new[] { "id_user" });
            DropIndex("purchase_invoice_dimension", new[] { "id_company" });
            DropIndex("purchase_invoice_dimension", new[] { "id_measurement" });
            DropIndex("purchase_invoice_dimension", new[] { "id_dimension" });
            DropIndex("purchase_invoice_detail", new[] { "project_task_id_project_task" });
            DropIndex("purchase_invoice_detail", new[] { "id_user" });
            DropIndex("purchase_invoice_detail", new[] { "id_company" });
            DropIndex("purchase_invoice_detail", new[] { "id_vat_group" });
            DropIndex("purchase_invoice_detail", new[] { "id_item" });
            DropIndex("purchase_invoice_detail", new[] { "id_cost_center" });
            DropIndex("purchase_invoice_detail", new[] { "id_location" });
            DropIndex("purchase_invoice_detail", new[] { "id_purchase_order_detail" });
            DropIndex("purchase_invoice_detail", new[] { "id_purchase_invoice" });
            DropIndex("sales_return_detail", new[] { "sales_invoice_detail_id_sales_invoice_detail" });
            DropIndex("sales_return_detail", new[] { "id_user" });
            DropIndex("sales_return_detail", new[] { "id_company" });
            DropIndex("sales_return_detail", new[] { "id_vat_group" });
            DropIndex("sales_return_detail", new[] { "id_item" });
            DropIndex("sales_return_detail", new[] { "id_project_task" });
            DropIndex("sales_return_detail", new[] { "id_location" });
            DropIndex("sales_return_detail", new[] { "id_sales_return" });
            DropIndex("sales_return", new[] { "newer_id_sales_return" });
            DropIndex("sales_return", new[] { "contact_ref_id_contact" });
            DropIndex("sales_return", new[] { "contact_id_contact" });
            DropIndex("sales_return", new[] { "id_user" });
            DropIndex("sales_return", new[] { "id_company" });
            DropIndex("sales_return", new[] { "id_project" });
            DropIndex("sales_return", new[] { "id_range" });
            DropIndex("sales_return", new[] { "id_condition" });
            DropIndex("sales_return", new[] { "id_contract" });
            DropIndex("sales_return", new[] { "id_terminal" });
            DropIndex("sales_return", new[] { "id_branch" });
            DropIndex("sales_return", new[] { "id_weather" });
            DropIndex("sales_return", new[] { "id_sales_rep" });
            DropIndex("sales_return", new[] { "id_journal" });
            DropIndex("sales_return", new[] { "id_currencyfx" });
            DropIndex("sales_return", new[] { "id_sales_invoice" });
            DropIndex("sales_return", new[] { "id_opportunity" });
            DropIndex("item_tag_detail", new[] { "id_user" });
            DropIndex("item_tag_detail", new[] { "id_company" });
            DropIndex("item_tag_detail", new[] { "id_tag" });
            DropIndex("item_tag_detail", new[] { "id_item" });
            DropIndex("item_tag", new[] { "id_user" });
            DropIndex("item_tag", new[] { "id_company" });
            DropIndex("project_template_detail", new[] { "parent_id_template_detail" });
            DropIndex("project_template_detail", new[] { "id_user" });
            DropIndex("project_template_detail", new[] { "id_company" });
            DropIndex("project_template_detail", new[] { "id_tag" });
            DropIndex("project_template_detail", new[] { "id_item" });
            DropIndex("project_template_detail", new[] { "id_project_template" });
            DropIndex("project_template", new[] { "id_user" });
            DropIndex("project_template", new[] { "id_company" });
            DropIndex("project_tag", new[] { "id_user" });
            DropIndex("project_tag", new[] { "id_company" });
            DropIndex("project_tag_detail", new[] { "id_user" });
            DropIndex("project_tag_detail", new[] { "id_company" });
            DropIndex("project_tag_detail", new[] { "id_tag" });
            DropIndex("project_tag_detail", new[] { "id_project" });
            DropIndex("production_order_dimension", new[] { "id_user" });
            DropIndex("production_order_dimension", new[] { "id_company" });
            DropIndex("production_order_dimension", new[] { "id_measurement" });
            DropIndex("production_order_dimension", new[] { "id_dimension" });
            DropIndex("production_order_dimension", new[] { "id_order_detail" });
            DropIndex("production_execution_dimension", new[] { "id_user" });
            DropIndex("production_execution_dimension", new[] { "id_company" });
            DropIndex("production_execution_dimension", new[] { "id_measurement" });
            DropIndex("production_execution_dimension", new[] { "id_dimension" });
            DropIndex("production_execution_dimension", new[] { "id_execution_detail" });
            DropIndex("production_execution_detail", new[] { "parent_id_execution_detail" });
            DropIndex("production_execution_detail", new[] { "id_user" });
            DropIndex("production_execution_detail", new[] { "id_company" });
            DropIndex("production_execution_detail", new[] { "id_item" });
            DropIndex("production_execution_detail", new[] { "id_contact" });
            DropIndex("production_execution_detail", new[] { "id_time_coefficient" });
            DropIndex("production_execution_detail", new[] { "id_project_task" });
            DropIndex("production_execution_detail", new[] { "id_order_detail" });
            DropIndex("sales_rep", new[] { "id_user" });
            DropIndex("sales_rep", new[] { "id_company" });
            DropIndex("sales_order", new[] { "newer_id_sales_order" });
            DropIndex("sales_order", new[] { "contact_ref_id_contact" });
            DropIndex("sales_order", new[] { "contact_id_contact" });
            DropIndex("sales_order", new[] { "id_user" });
            DropIndex("sales_order", new[] { "id_company" });
            DropIndex("sales_order", new[] { "id_project" });
            DropIndex("sales_order", new[] { "id_range" });
            DropIndex("sales_order", new[] { "id_condition" });
            DropIndex("sales_order", new[] { "id_contract" });
            DropIndex("sales_order", new[] { "id_terminal" });
            DropIndex("sales_order", new[] { "id_branch" });
            DropIndex("sales_order", new[] { "id_weather" });
            DropIndex("sales_order", new[] { "id_sales_rep" });
            DropIndex("sales_order", new[] { "id_currencyfx" });
            DropIndex("sales_order", new[] { "id_opportunity" });
            DropIndex("sales_order", new[] { "id_sales_budget" });
            DropIndex("sales_packing", new[] { "newer_id_sales_packing" });
            DropIndex("sales_packing", new[] { "id_user" });
            DropIndex("sales_packing", new[] { "id_company" });
            DropIndex("sales_packing", new[] { "id_range" });
            DropIndex("sales_packing", new[] { "id_terminal" });
            DropIndex("sales_packing", new[] { "id_branch" });
            DropIndex("sales_packing", new[] { "id_contact" });
            DropIndex("sales_packing", new[] { "id_opportunity" });
            DropIndex("sales_packing_detail", new[] { "id_user" });
            DropIndex("sales_packing_detail", new[] { "id_company" });
            DropIndex("sales_packing_detail", new[] { "id_item" });
            DropIndex("sales_packing_detail", new[] { "id_location" });
            DropIndex("sales_packing_detail", new[] { "id_sales_order_detail" });
            DropIndex("sales_packing_detail", new[] { "id_sales_packing" });
            DropIndex("sales_packing_relation", new[] { "sales_invoice_detail_id_sales_invoice_detail" });
            DropIndex("sales_packing_relation", new[] { "id_sales_packing_detail" });
            DropIndex("sales_invoice_detail", new[] { "id_user" });
            DropIndex("sales_invoice_detail", new[] { "id_company" });
            DropIndex("sales_invoice_detail", new[] { "id_vat_group" });
            DropIndex("sales_invoice_detail", new[] { "id_item" });
            DropIndex("sales_invoice_detail", new[] { "id_project_task" });
            DropIndex("sales_invoice_detail", new[] { "id_location" });
            DropIndex("sales_invoice_detail", new[] { "id_sales_order_detail" });
            DropIndex("sales_invoice_detail", new[] { "id_sales_invoice" });
            DropIndex("sales_budget_detail", new[] { "id_user" });
            DropIndex("sales_budget_detail", new[] { "id_company" });
            DropIndex("sales_budget_detail", new[] { "id_vat_group" });
            DropIndex("sales_budget_detail", new[] { "id_item" });
            DropIndex("sales_budget_detail", new[] { "id_project_task" });
            DropIndex("sales_budget_detail", new[] { "id_location" });
            DropIndex("sales_budget_detail", new[] { "id_sales_budget" });
            DropIndex("app_vat", new[] { "id_user" });
            DropIndex("app_vat", new[] { "id_company" });
            DropIndex("app_vat_group_details", new[] { "id_user" });
            DropIndex("app_vat_group_details", new[] { "id_company" });
            DropIndex("app_vat_group_details", new[] { "id_vat" });
            DropIndex("app_vat_group_details", new[] { "id_vat_group" });
            DropIndex("app_vat_group", new[] { "id_user" });
            DropIndex("app_vat_group", new[] { "id_company" });
            DropIndex("sales_order_detail", new[] { "project_task_id_project_task1" });
            DropIndex("sales_order_detail", new[] { "project_task_id_project_task" });
            DropIndex("sales_order_detail", new[] { "id_user" });
            DropIndex("sales_order_detail", new[] { "id_company" });
            DropIndex("sales_order_detail", new[] { "id_vat_group" });
            DropIndex("sales_order_detail", new[] { "id_item" });
            DropIndex("sales_order_detail", new[] { "id_location" });
            DropIndex("sales_order_detail", new[] { "id_sales_budget_detail" });
            DropIndex("sales_order_detail", new[] { "id_sales_order" });
            DropIndex("item_request_dimension", new[] { "item_request_detail_id_item_request_detail" });
            DropIndex("item_request_dimension", new[] { "id_user" });
            DropIndex("item_request_dimension", new[] { "id_company" });
            DropIndex("item_request_dimension", new[] { "id_measurement" });
            DropIndex("item_request_dimension", new[] { "id_dimension" });
            DropIndex("item_request_decision", new[] { "id_user" });
            DropIndex("item_request_decision", new[] { "id_company" });
            DropIndex("item_request_decision", new[] { "id_location" });
            DropIndex("item_request_decision", new[] { "id_item_request_detail" });
            DropIndex("item_asset_group", new[] { "id_user" });
            DropIndex("item_asset_group", new[] { "id_company" });
            DropIndex("item_asset", new[] { "id_user" });
            DropIndex("item_asset", new[] { "id_company" });
            DropIndex("item_asset", new[] { "id_contact" });
            DropIndex("item_asset", new[] { "id_item_asset_group" });
            DropIndex("item_asset", new[] { "id_branch" });
            DropIndex("item_asset", new[] { "id_item" });
            DropIndex("item_asset_maintainance", new[] { "id_user" });
            DropIndex("item_asset_maintainance", new[] { "id_company" });
            DropIndex("item_asset_maintainance", new[] { "id_item_asset" });
            DropIndex("hr_time_coefficient", new[] { "id_user" });
            DropIndex("hr_time_coefficient", new[] { "id_company" });
            DropIndex("item_asset_maintainance_detail", new[] { "id_user" });
            DropIndex("item_asset_maintainance_detail", new[] { "id_company" });
            DropIndex("item_asset_maintainance_detail", new[] { "id_contact" });
            DropIndex("item_asset_maintainance_detail", new[] { "id_time_coefficient" });
            DropIndex("item_asset_maintainance_detail", new[] { "id_currencyfx" });
            DropIndex("item_asset_maintainance_detail", new[] { "id_item" });
            DropIndex("item_asset_maintainance_detail", new[] { "id_maintainance" });
            DropIndex("item_request_detail", new[] { "id_user" });
            DropIndex("item_request_detail", new[] { "id_company" });
            DropIndex("item_request_detail", new[] { "id_item" });
            DropIndex("item_request_detail", new[] { "id_maintainance_detail" });
            DropIndex("item_request_detail", new[] { "id_order_detail" });
            DropIndex("item_request_detail", new[] { "id_sales_order_detail" });
            DropIndex("item_request_detail", new[] { "id_project_task" });
            DropIndex("item_request_detail", new[] { "id_item_request" });
            DropIndex("production_order_detail", new[] { "parent_id_order_detail" });
            DropIndex("production_order_detail", new[] { "id_user" });
            DropIndex("production_order_detail", new[] { "id_company" });
            DropIndex("production_order_detail", new[] { "id_item" });
            DropIndex("production_order_detail", new[] { "id_project_task" });
            DropIndex("production_order_detail", new[] { "id_production_order" });
            DropIndex("production_line", new[] { "id_user" });
            DropIndex("production_line", new[] { "id_company" });
            DropIndex("production_line", new[] { "id_location" });
            DropIndex("production_order", new[] { "id_user" });
            DropIndex("production_order", new[] { "id_company" });
            DropIndex("production_order", new[] { "id_range" });
            DropIndex("production_order", new[] { "id_project" });
            DropIndex("production_order", new[] { "id_production_line" });
            DropIndex("projects", new[] { "id_user" });
            DropIndex("projects", new[] { "id_company" });
            DropIndex("projects", new[] { "id_currency" });
            DropIndex("projects", new[] { "id_contact" });
            DropIndex("projects", new[] { "id_branch" });
            DropIndex("projects", new[] { "id_project_template" });
            DropIndex("sales_budget", new[] { "newer_id_sales_budget" });
            DropIndex("sales_budget", new[] { "contact_ref_id_contact" });
            DropIndex("sales_budget", new[] { "contact_id_contact" });
            DropIndex("sales_budget", new[] { "id_user" });
            DropIndex("sales_budget", new[] { "id_company" });
            DropIndex("sales_budget", new[] { "id_project" });
            DropIndex("sales_budget", new[] { "id_range" });
            DropIndex("sales_budget", new[] { "id_condition" });
            DropIndex("sales_budget", new[] { "id_contract" });
            DropIndex("sales_budget", new[] { "id_terminal" });
            DropIndex("sales_budget", new[] { "id_branch" });
            DropIndex("sales_budget", new[] { "id_weather" });
            DropIndex("sales_budget", new[] { "id_sales_rep" });
            DropIndex("sales_budget", new[] { "id_currencyfx" });
            DropIndex("sales_budget", new[] { "id_opportunity" });
            DropIndex("crm_opportunity", new[] { "id_user" });
            DropIndex("crm_opportunity", new[] { "id_company" });
            DropIndex("crm_opportunity", new[] { "id_contact" });
            DropIndex("sales_invoice", new[] { "newer_id_sales_invoice" });
            DropIndex("sales_invoice", new[] { "contact_ref_id_contact" });
            DropIndex("sales_invoice", new[] { "contact_id_contact" });
            DropIndex("sales_invoice", new[] { "id_user" });
            DropIndex("sales_invoice", new[] { "id_company" });
            DropIndex("sales_invoice", new[] { "id_project" });
            DropIndex("sales_invoice", new[] { "id_range" });
            DropIndex("sales_invoice", new[] { "id_condition" });
            DropIndex("sales_invoice", new[] { "id_contract" });
            DropIndex("sales_invoice", new[] { "id_terminal" });
            DropIndex("sales_invoice", new[] { "id_branch" });
            DropIndex("sales_invoice", new[] { "id_weather" });
            DropIndex("sales_invoice", new[] { "id_sales_rep" });
            DropIndex("sales_invoice", new[] { "id_currencyfx" });
            DropIndex("sales_invoice", new[] { "id_journal" });
            DropIndex("sales_invoice", new[] { "id_opportunity" });
            DropIndex("sales_invoice", new[] { "id_sales_order" });
            DropIndex("payment_withholding_detail", new[] { "id_user" });
            DropIndex("payment_withholding_detail", new[] { "id_company" });
            DropIndex("payment_withholding_detail", new[] { "id_purchase_invoice" });
            DropIndex("payment_withholding_detail", new[] { "id_sales_invoice" });
            DropIndex("payment_withholding_detail", new[] { "id_withholding" });
            DropIndex("payment_withholding_tax", new[] { "id_user" });
            DropIndex("payment_withholding_tax", new[] { "id_company" });
            DropIndex("payment_withholding_tax", new[] { "id_journal" });
            DropIndex("payment_withholding_tax", new[] { "id_currencyfx" });
            DropIndex("payment_withholding_tax", new[] { "id_range" });
            DropIndex("payment_withholding_tax", new[] { "id_contact" });
            DropIndex("payment_withholding_details", new[] { "id_user" });
            DropIndex("payment_withholding_details", new[] { "id_company" });
            DropIndex("payment_withholding_details", new[] { "id_purchase_invoice" });
            DropIndex("payment_withholding_details", new[] { "id_sales_invoice" });
            DropIndex("payment_withholding_details", new[] { "id_withholding" });
            DropIndex("app_weather", new[] { "id_branch" });
            DropIndex("purchase_invoice", new[] { "newer_id_purchase_invoice" });
            DropIndex("purchase_invoice", new[] { "contact_ref_id_contact" });
            DropIndex("purchase_invoice", new[] { "contact_id_contact" });
            DropIndex("purchase_invoice", new[] { "id_user" });
            DropIndex("purchase_invoice", new[] { "id_company" });
            DropIndex("purchase_invoice", new[] { "id_project" });
            DropIndex("purchase_invoice", new[] { "id_range" });
            DropIndex("purchase_invoice", new[] { "id_condition" });
            DropIndex("purchase_invoice", new[] { "id_contract" });
            DropIndex("purchase_invoice", new[] { "id_terminal" });
            DropIndex("purchase_invoice", new[] { "id_branch" });
            DropIndex("purchase_invoice", new[] { "id_weather" });
            DropIndex("purchase_invoice", new[] { "id_sales_rep" });
            DropIndex("purchase_invoice", new[] { "id_journal" });
            DropIndex("purchase_invoice", new[] { "id_currencyfx" });
            DropIndex("purchase_invoice", new[] { "id_department" });
            DropIndex("purchase_invoice", new[] { "id_purchase_order" });
            DropIndex("payment_promissory_note", new[] { "id_user" });
            DropIndex("payment_promissory_note", new[] { "id_company" });
            DropIndex("payment_promissory_note", new[] { "id_currencyfx" });
            DropIndex("payment_promissory_note", new[] { "id_terminal" });
            DropIndex("payment_promissory_note", new[] { "id_branch" });
            DropIndex("payment_promissory_note", new[] { "id_range" });
            DropIndex("payment_promissory_note", new[] { "id_contact" });
            DropIndex("payment_schedual", new[] { "parent_id_payment_schedual" });
            DropIndex("payment_schedual", new[] { "id_user" });
            DropIndex("payment_schedual", new[] { "id_company" });
            DropIndex("payment_schedual", new[] { "id_currencyfx" });
            DropIndex("payment_schedual", new[] { "id_contact" });
            DropIndex("payment_schedual", new[] { "id_payment_detail" });
            DropIndex("payment_schedual", new[] { "id_note" });
            DropIndex("payment_schedual", new[] { "id_purchase_order" });
            DropIndex("payment_schedual", new[] { "id_sales_order" });
            DropIndex("payment_schedual", new[] { "id_sales_return" });
            DropIndex("payment_schedual", new[] { "id_sales_invoice" });
            DropIndex("payment_schedual", new[] { "id_purchase_return" });
            DropIndex("payment_schedual", new[] { "id_purchase_invoice" });
            DropIndex("app_terminal", new[] { "id_user" });
            DropIndex("app_terminal", new[] { "id_company" });
            DropIndex("app_terminal", new[] { "id_branch" });
            DropIndex("payments", new[] { "id_user" });
            DropIndex("payments", new[] { "id_company" });
            DropIndex("payments", new[] { "id_terminal" });
            DropIndex("payments", new[] { "id_branch" });
            DropIndex("payments", new[] { "id_range" });
            DropIndex("payments", new[] { "id_journal" });
            DropIndex("payments", new[] { "id_contact" });
            DropIndex("payment_detail", new[] { "app_bank_id_bank" });
            DropIndex("payment_detail", new[] { "id_user" });
            DropIndex("payment_detail", new[] { "id_company" });
            DropIndex("payment_detail", new[] { "id_range" });
            DropIndex("payment_detail", new[] { "id_payment_type" });
            DropIndex("payment_detail", new[] { "id_currencyfx" });
            DropIndex("payment_detail", new[] { "id_account" });
            DropIndex("payment_detail", new[] { "id_payment" });
            DropIndex("payment_type_detail", new[] { "id_user" });
            DropIndex("payment_type_detail", new[] { "id_company" });
            DropIndex("payment_type_detail", new[] { "id_field" });
            DropIndex("payment_type_detail", new[] { "id_payment_detail" });
            DropIndex("payment_type_detail", new[] { "id_payment_type" });
            DropIndex("payment_type", new[] { "id_user" });
            DropIndex("payment_type", new[] { "id_company" });
            DropIndex("payment_type", new[] { "id_document" });
            DropIndex("app_document", new[] { "id_user" });
            DropIndex("app_document", new[] { "id_company" });
            DropIndex("app_document_range", new[] { "id_user" });
            DropIndex("app_document_range", new[] { "id_company" });
            DropIndex("app_document_range", new[] { "id_terminal" });
            DropIndex("app_document_range", new[] { "id_branch" });
            DropIndex("app_document_range", new[] { "id_document" });
            DropIndex("project_task", new[] { "sales_detail_id_sales_order_detail" });
            DropIndex("project_task", new[] { "parent_id_project_task" });
            DropIndex("project_task", new[] { "id_user" });
            DropIndex("project_task", new[] { "id_company" });
            DropIndex("project_task", new[] { "id_range" });
            DropIndex("project_task", new[] { "id_item" });
            DropIndex("project_task", new[] { "id_project" });
            DropIndex("project_task_dimension", new[] { "id_user" });
            DropIndex("project_task_dimension", new[] { "id_company" });
            DropIndex("project_task_dimension", new[] { "id_measurement" });
            DropIndex("project_task_dimension", new[] { "id_dimension" });
            DropIndex("project_task_dimension", new[] { "id_project_task" });
            DropIndex("item_dimension", new[] { "app_dimension_id_dimension" });
            DropIndex("item_dimension", new[] { "id_user" });
            DropIndex("item_dimension", new[] { "id_company" });
            DropIndex("item_dimension", new[] { "id_measurement" });
            DropIndex("item_dimension", new[] { "id_item" });
            DropIndex("app_dimension", new[] { "id_user" });
            DropIndex("app_dimension", new[] { "id_company" });
            DropIndex("item_movement_dimension", new[] { "id_user" });
            DropIndex("item_movement_dimension", new[] { "id_company" });
            DropIndex("item_movement_dimension", new[] { "id_dimension" });
            DropIndex("item_movement_dimension", new[] { "id_movement" });
            DropIndex("app_location", new[] { "id_user" });
            DropIndex("app_location", new[] { "id_company" });
            DropIndex("app_location", new[] { "id_contact" });
            DropIndex("app_location", new[] { "id_branch" });
            DropIndex("item_movement", new[] { "parent_id_movement" });
            DropIndex("item_movement", new[] { "id_user" });
            DropIndex("item_movement", new[] { "id_company" });
            DropIndex("item_movement", new[] { "id_location" });
            DropIndex("item_movement", new[] { "id_sales_packing_detail" });
            DropIndex("item_movement", new[] { "id_sales_return_detail" });
            DropIndex("item_movement", new[] { "id_sales_invoice_detail" });
            DropIndex("item_movement", new[] { "id_purchase_return_detail" });
            DropIndex("item_movement", new[] { "id_purchase_invoice_detail" });
            DropIndex("item_movement", new[] { "id_execution_detail" });
            DropIndex("item_movement", new[] { "id_transfer_detail" });
            DropIndex("item_movement", new[] { "id_item_product" });
            DropIndex("item_product", new[] { "id_user" });
            DropIndex("item_product", new[] { "id_company" });
            DropIndex("item_product", new[] { "id_item" });
            DropIndex("item_conversion_factor", new[] { "id_user" });
            DropIndex("item_conversion_factor", new[] { "id_company" });
            DropIndex("item_conversion_factor", new[] { "id_item_product" });
            DropIndex("item_conversion_factor", new[] { "id_measurement" });
            DropIndex("app_measurement_type", new[] { "id_user" });
            DropIndex("app_measurement_type", new[] { "id_company" });
            DropIndex("app_measurement", new[] { "id_user" });
            DropIndex("app_measurement", new[] { "id_company" });
            DropIndex("app_measurement", new[] { "id_measurement_type" });
            DropIndex("items", new[] { "id_user" });
            DropIndex("items", new[] { "id_company" });
            DropIndex("items", new[] { "id_measurement" });
            DropIndex("items", new[] { "id_brand" });
            DropIndex("items", new[] { "id_vat_group" });
            DropIndex("contact_subscription", new[] { "id_user" });
            DropIndex("contact_subscription", new[] { "id_company" });
            DropIndex("contact_subscription", new[] { "id_item" });
            DropIndex("contact_subscription", new[] { "id_contact" });
            DropIndex("contact_role", new[] { "id_user" });
            DropIndex("contact_role", new[] { "id_company" });
            DropIndex("app_field", new[] { "id_user" });
            DropIndex("app_field", new[] { "id_company" });
            DropIndex("contact_field_value", new[] { "id_user" });
            DropIndex("contact_field_value", new[] { "id_company" });
            DropIndex("contact_field_value", new[] { "id_field" });
            DropIndex("contact_field_value", new[] { "id_contact" });
            DropIndex("app_cost_center", new[] { "id_user" });
            DropIndex("app_cost_center", new[] { "id_company" });
            DropIndex("app_contract_detail", new[] { "id_user" });
            DropIndex("app_contract_detail", new[] { "id_company" });
            DropIndex("app_contract_detail", new[] { "id_contract" });
            DropIndex("app_condition", new[] { "id_user" });
            DropIndex("app_condition", new[] { "id_company" });
            DropIndex("app_contract", new[] { "id_user" });
            DropIndex("app_contract", new[] { "id_company" });
            DropIndex("app_contract", new[] { "id_condition" });
            DropIndex("contacts", new[] { "parent_id_contact" });
            DropIndex("contacts", new[] { "id_user" });
            DropIndex("contacts", new[] { "id_company" });
            DropIndex("contacts", new[] { "id_bank" });
            DropIndex("contacts", new[] { "id_geography" });
            DropIndex("contacts", new[] { "id_sales_rep" });
            DropIndex("contacts", new[] { "id_price_list" });
            DropIndex("contacts", new[] { "id_cost_center" });
            DropIndex("contacts", new[] { "id_currency" });
            DropIndex("contacts", new[] { "id_contract" });
            DropIndex("contacts", new[] { "id_contact_role" });
            DropIndex("impexes", new[] { "id_user" });
            DropIndex("impexes", new[] { "id_company" });
            DropIndex("impexes", new[] { "id_contact" });
            DropIndex("impexes", new[] { "id_incoterm" });
            DropIndex("impex_expense", new[] { "id_user" });
            DropIndex("impex_expense", new[] { "id_company" });
            DropIndex("impex_expense", new[] { "id_currency" });
            DropIndex("impex_expense", new[] { "id_incoterm_condition" });
            DropIndex("impex_expense", new[] { "id_purchase_invoice" });
            DropIndex("impex_expense", new[] { "id_impex" });
            DropIndex("app_currencyfx", new[] { "id_user" });
            DropIndex("app_currencyfx", new[] { "id_company" });
            DropIndex("app_currencyfx", new[] { "id_currency" });
            DropIndex("app_currency_denomination", new[] { "id_user" });
            DropIndex("app_currency_denomination", new[] { "id_company" });
            DropIndex("app_currency_denomination", new[] { "id_currency" });
            DropIndex("app_currency", new[] { "id_user" });
            DropIndex("app_currency", new[] { "id_company" });
            DropIndex("item_request", new[] { "security_user_id_user" });
            DropIndex("item_request", new[] { "request_user_id_user" });
            DropIndex("item_request", new[] { "id_company" });
            DropIndex("item_request", new[] { "id_department" });
            DropIndex("item_request", new[] { "id_branch" });
            DropIndex("item_request", new[] { "id_currency" });
            DropIndex("item_request", new[] { "id_production_order" });
            DropIndex("item_request", new[] { "id_sales_order" });
            DropIndex("item_request", new[] { "id_project" });
            DropIndex("hr_position", new[] { "parent_id_position" });
            DropIndex("hr_position", new[] { "id_user" });
            DropIndex("hr_position", new[] { "id_company" });
            DropIndex("hr_position", new[] { "id_department" });
            DropIndex("app_department", new[] { "id_user" });
            DropIndex("app_department", new[] { "id_company" });
            DropIndex("security_role", new[] { "id_department" });
            DropIndex("security_role", new[] { "id_company" });
            DropIndex("security_user", new[] { "parent_id_user" });
            DropIndex("security_user", new[] { "id_role" });
            DropIndex("security_user", new[] { "id_question" });
            DropIndex("security_user", new[] { "id_company" });
            DropIndex("app_account_session", new[] { "id_user" });
            DropIndex("app_account_session", new[] { "id_company" });
            DropIndex("app_account_session", new[] { "id_account" });
            DropIndex("app_account_detail", new[] { "id_user" });
            DropIndex("app_account_detail", new[] { "id_company" });
            DropIndex("app_account_detail", new[] { "id_session" });
            DropIndex("app_account_detail", new[] { "id_payment_type" });
            DropIndex("app_account_detail", new[] { "id_payment_detail" });
            DropIndex("app_account_detail", new[] { "id_currencyfx" });
            DropIndex("app_account_detail", new[] { "id_account" });
            DropIndex("app_account", new[] { "id_user" });
            DropIndex("app_account", new[] { "id_company" });
            DropIndex("app_account", new[] { "id_terminal" });
            DropIndex("app_account", new[] { "id_currency" });
            DropIndex("app_account", new[] { "id_bank" });
            DropIndex("app_bank", new[] { "id_user" });
            DropIndex("app_bank", new[] { "id_company" });
            DropIndex("app_bank", new[] { "id_geography" });
            DropIndex("app_geography", new[] { "parent_id_geography" });
            DropIndex("app_geography", new[] { "id_user" });
            DropIndex("app_geography", new[] { "id_company" });
            DropIndex("app_branch", new[] { "id_user" });
            DropIndex("app_branch", new[] { "id_company" });
            DropIndex("app_branch", new[] { "id_measurement" });
            DropIndex("app_branch", new[] { "id_vat" });
            DropIndex("app_branch", new[] { "id_geography" });
            DropIndex("accounting_cycle", new[] { "id_user" });
            DropIndex("accounting_cycle", new[] { "id_company" });
            DropIndex("accounting_journal", new[] { "id_user" });
            DropIndex("accounting_journal", new[] { "id_company" });
            DropIndex("accounting_journal", new[] { "id_cycle" });
            DropIndex("accounting_journal", new[] { "id_branch" });
            DropIndex("accounting_journal_detail", new[] { "id_user" });
            DropIndex("accounting_journal_detail", new[] { "id_company" });
            DropIndex("accounting_journal_detail", new[] { "id_currencyfx" });
            DropIndex("accounting_journal_detail", new[] { "id_chart" });
            DropIndex("accounting_journal_detail", new[] { "id_journal" });
            DropIndex("accounting_chart", new[] { "parent_id_chart" });
            DropIndex("accounting_chart", new[] { "id_user" });
            DropIndex("accounting_chart", new[] { "id_company" });
            DropIndex("accounting_chart", new[] { "id_cost_center" });
            DropIndex("accounting_chart", new[] { "id_department" });
            DropIndex("accounting_chart", new[] { "id_vat" });
            DropIndex("accounting_chart", new[] { "id_item_asset_group" });
            DropIndex("accounting_chart", new[] { "id_tag" });
            DropIndex("accounting_chart", new[] { "id_contact" });
            DropIndex("accounting_chart", new[] { "id_account" });
            DropIndex("accounting_budget", new[] { "id_user" });
            DropIndex("accounting_budget", new[] { "id_company" });
            DropIndex("accounting_budget", new[] { "id_cycle" });
            DropIndex("accounting_budget", new[] { "id_chart" });
            DropTable("sales_promotion");
            DropTable("purchase_packing_relation");
            DropTable("purchase_packing_dimension");
            DropTable("purchase_packing_detail");
            DropTable("purchase_packing");
            DropTable("item_template_detail");
            DropTable("item_template");
            DropTable("item_inventory_dimension");
            DropTable("item_inventory_detail");
            DropTable("item_inventory");
            DropTable("hr_timesheet");
            DropTable("bi_tag_role");
            DropTable("bi_tag_report");
            DropTable("bi_tag");
            DropTable("bi_report_detail");
            DropTable("bi_report");
            DropTable("bi_chart_report");
            DropTable("app_name_template_detail");
            DropTable("app_name_template");
            DropTable("app_configuration");
            DropTable("app_branch_walkins");
            DropTable("accounting_template_detail");
            DropTable("accounting_template");
            DropTable("security_privilage");
            DropTable("security_role_privilage");
            DropTable("security_crud");
            DropTable("impex_incoterm_condition");
            DropTable("impex_incoterm_detail");
            DropTable("impex_incoterm");
            DropTable("impex_import");
            DropTable("impex_export");
            DropTable("hr_family");
            DropTable("hr_education");
            DropTable("hr_contract");
            DropTable("contact_tag");
            DropTable("contact_tag_detail");
            DropTable("project_event_variable");
            DropTable("project_event_template_variable");
            DropTable("project_event_template_fixed");
            DropTable("project_event_template");
            DropTable("project_event_fixed");
            DropTable("project_event");
            DropTable("hr_talent_detail");
            DropTable("hr_talent");
            DropTable("item_service");
            DropTable("item_recepie_detail");
            DropTable("item_recepie");
            DropTable("app_property");
            DropTable("item_property");
            DropTable("item_price_list");
            DropTable("item_price");
            DropTable("item_brand");
            DropTable("app_attachment");
            DropTable("item_attachment");
            DropTable("item_transfer_dimension");
            DropTable("item_transfer");
            DropTable("item_transfer_detail");
            DropTable("item_movement_value");
            DropTable("purchase_return_dimension");
            DropTable("purchase_return_detail");
            DropTable("purchase_return");
            DropTable("purchase_order_dimension");
            DropTable("purchase_tender_dimension");
            DropTable("purchase_tender_item");
            DropTable("purchase_tender_detail_dimension");
            DropTable("purchase_tender_detail");
            DropTable("purchase_tender_contact");
            DropTable("purchase_tender");
            DropTable("purchase_order");
            DropTable("purchase_order_detail");
            DropTable("purchase_invoice_dimension");
            DropTable("purchase_invoice_detail");
            DropTable("sales_return_detail");
            DropTable("sales_return");
            DropTable("item_tag_detail");
            DropTable("item_tag");
            DropTable("project_template_detail");
            DropTable("project_template");
            DropTable("project_tag");
            DropTable("project_tag_detail");
            DropTable("production_order_dimension");
            DropTable("production_execution_dimension");
            DropTable("production_execution_detail");
            DropTable("sales_rep");
            DropTable("sales_order");
            DropTable("sales_packing");
            DropTable("sales_packing_detail");
            DropTable("sales_packing_relation");
            DropTable("sales_invoice_detail");
            DropTable("sales_budget_detail");
            DropTable("app_vat");
            DropTable("app_vat_group_details");
            DropTable("app_vat_group");
            DropTable("sales_order_detail");
            DropTable("item_request_dimension");
            DropTable("item_request_decision");
            DropTable("item_asset_group");
            DropTable("item_asset");
            DropTable("item_asset_maintainance");
            DropTable("hr_time_coefficient");
            DropTable("item_asset_maintainance_detail");
            DropTable("item_request_detail");
            DropTable("production_order_detail");
            DropTable("production_line");
            DropTable("production_order");
            DropTable("projects");
            DropTable("sales_budget");
            DropTable("crm_opportunity");
            DropTable("sales_invoice");
            DropTable("payment_withholding_detail");
            DropTable("payment_withholding_tax");
            DropTable("payment_withholding_details");
            DropTable("app_weather");
            DropTable("purchase_invoice");
            DropTable("payment_promissory_note");
            DropTable("payment_schedual");
            DropTable("app_terminal");
            DropTable("payments");
            DropTable("payment_detail");
            DropTable("payment_type_detail");
            DropTable("payment_type");
            DropTable("app_document");
            DropTable("app_document_range");
            DropTable("project_task");
            DropTable("project_task_dimension");
            DropTable("item_dimension");
            DropTable("app_dimension");
            DropTable("item_movement_dimension");
            DropTable("app_location");
            DropTable("item_movement");
            DropTable("item_product");
            DropTable("item_conversion_factor");
            DropTable("app_measurement_type");
            DropTable("app_measurement");
            DropTable("items");
            DropTable("contact_subscription");
            DropTable("contact_role");
            DropTable("app_field");
            DropTable("contact_field_value");
            DropTable("app_cost_center");
            DropTable("app_contract_detail");
            DropTable("app_condition");
            DropTable("app_contract");
            DropTable("contacts");
            DropTable("impexes");
            DropTable("impex_expense");
            DropTable("app_currencyfx");
            DropTable("app_currency_denomination");
            DropTable("app_currency");
            DropTable("item_request");
            DropTable("hr_position");
            DropTable("app_department");
            DropTable("security_role");
            DropTable("security_question");
            DropTable("security_user");
            DropTable("app_account_session");
            DropTable("app_account_detail");
            DropTable("app_account");
            DropTable("app_bank");
            DropTable("app_geography");
            DropTable("app_branch");
            DropTable("app_company");
            DropTable("accounting_cycle");
            DropTable("accounting_journal");
            DropTable("accounting_journal_detail");
            DropTable("accounting_chart");
            DropTable("accounting_budget");
        }
    }
}
