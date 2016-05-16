namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class TryingToFix_ItemMovement_ExcessFK : DbMigration
    {
        public override void Up()
        {
            Sql(@" ALTER TABLE `item_movement`  "
         + " DROP FOREIGN KEY `FK_item_movement_sales_return_id_sales_return`, "
         + " DROP FOREIGN KEY `FK_item_movement_sales_packing_id_sales_packing`, "
         + " DROP FOREIGN KEY `FK_item_movement_sales_invoice_id_sales_invoice`, "
         + " DROP FOREIGN KEY `FK_item_movement_purchase_return_id_purchase_return`, "
         + " DROP FOREIGN KEY `FK_item_movement_purchase_invoice_id_purchase_invoice`, "
         + " DROP FOREIGN KEY `FK_item_movement_production_execution_id_production_execution`; "
         + " ALTER TABLE `item_movement`  "
         + " DROP COLUMN `id_sales_packing`, "
         + " DROP COLUMN `id_inventory`, "
         + " DROP COLUMN `id_sales_return`, "
         + " DROP COLUMN `id_sales_invoice`, "
         + " DROP COLUMN `id_purchase_return`, "
         + " DROP COLUMN `id_purchase_invoice`, "
         + " DROP COLUMN `id_production_execution`, "
         + " DROP INDEX `IX_id_sales_packing` , "
         + " DROP INDEX `IX_id_sales_return` , "
         + " DROP INDEX `IX_id_sales_invoice` , "
         + " DROP INDEX `IX_id_purchase_return` , "
         + " DROP INDEX `IX_id_purchase_invoice` , "
         + " DROP INDEX `IX_id_production_execution` ;"
        + " ALTER TABLE `item_movement`  "
        + "DROP FOREIGN KEY `FK_item_movement_item_transfer_id_transfer`;  "
        + "ALTER TABLE `item_movement`  "
        + "DROP COLUMN `id_transfer`, "
        + "DROP INDEX `IX_id_transfer` ; "
        + "ALTER TABLE `item_movement` "
        + "CHANGE COLUMN `id_production_execution_detail` `id_execution_detail` INT(11) NULL DEFAULT NULL ,"
        + "CHANGE COLUMN `id_sales_invoice_detail` `id_sales_invoice_detail` BIGINT(20) NULL DEFAULT NULL ;"
           + " ALTER TABLE `item_movement` "
+ "DROP INDEX `IX_production_execution_detail_id_execution_detail` ,"
+ "ADD INDEX `IX_id_execution_detail` USING HASH (`production_execution_detail_id_execution_detail` ASC);");







            CreateTable(
                "app_branch_walkins",
                c => new
                    {
                        id_branch_walkin = c.Int(nullable: false, identity: true),
                        id_branch = c.Int(nullable: false),
                        start_date = c.DateTime(nullable: false, precision: 0),
                        end_date = c.DateTime(nullable: false, precision: 0),
                        quantity = c.Decimal(nullable: false, precision: 20, scale: 4),
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
                "item_asset_maintainance",
                c => new
                    {
                        id_maintainance = c.Int(nullable: false, identity: true),
                        id_item_asset = c.Int(nullable: false),
                        status = c.Int(nullable: false),
                        start_date = c.DateTime(nullable: false, precision: 0),
                        end_date = c.DateTime(nullable: false, precision: 0),
                        comment = c.String(unicode: false),
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
                "item_asset_maintainance_detail",
                c => new
                    {
                        id_maintainance_detail = c.Int(nullable: false, identity: true),
                        id_maintainance = c.Int(nullable: false),
                        id_item = c.Int(),
                        item_description = c.String(unicode: false),
                        quantity = c.Decimal(nullable: false, precision: 20, scale: 4),
                        unit_cost = c.Decimal(nullable: false, precision: 20, scale: 4),
                        id_currencyfx = c.Int(),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
                        is_read = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id_maintainance_detail)
                .ForeignKey("app_company", t => t.id_company, cascadeDelete: true)
                .ForeignKey("app_currencyfx", t => t.id_currencyfx)
                .ForeignKey("items", t => t.id_item)
                .ForeignKey("item_asset_maintainance", t => t.id_maintainance, cascadeDelete: true)
                .ForeignKey("security_user", t => t.id_user, cascadeDelete: true)
                .Index(t => t.id_maintainance)
                .Index(t => t.id_item)
                .Index(t => t.id_currencyfx)
                .Index(t => t.id_company)
                .Index(t => t.id_user);

            AddColumn("accounting_budget", "is_read", c => c.Boolean(nullable: false));
            AddColumn("accounting_chart", "is_read", c => c.Boolean(nullable: false));
            AddColumn("accounting_journal_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("accounting_journal", "is_read", c => c.Boolean(nullable: false));
            AddColumn("accounting_cycle", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_branch", "area", c => c.Decimal(precision: 20, scale: 4));
            AddColumn("app_branch", "id_measurement", c => c.Int());
            AddColumn("app_branch", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_geography", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_bank", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_account", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_account_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_account_session", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_department", "is_read", c => c.Boolean(nullable: false));
            AddColumn("hr_position", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_request", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_currency", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_currency_denomination", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_currencyfx", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_request_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("items", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_measurement", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_measurement_type", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_conversion_factor", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_product", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_dimension", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_dimension", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_movement_dimension", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_movement", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_location", "is_read", c => c.Boolean(nullable: false));
            AddColumn("contacts", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_contract", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_condition", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_contract_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_cost_center", "is_read", c => c.Boolean(nullable: false));
            AddColumn("contact_field_value", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_field", "is_read", c => c.Boolean(nullable: false));
            AddColumn("contact_role", "is_read", c => c.Boolean(nullable: false));
            AddColumn("contact_subscription", "is_read", c => c.Boolean(nullable: false));
            AddColumn("contact_tag_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("contact_tag", "is_read", c => c.Boolean(nullable: false));
            AddColumn("hr_contract", "is_read", c => c.Boolean(nullable: false));
            AddColumn("hr_education", "is_read", c => c.Boolean(nullable: false));
            AddColumn("hr_family", "is_read", c => c.Boolean(nullable: false));
            AddColumn("hr_talent_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("hr_talent", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_price_list", "is_read", c => c.Boolean(nullable: false));
            AddColumn("production_execution_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("hr_time_coefficient", "is_read", c => c.Boolean(nullable: false));
            AddColumn("production_execution", "is_read", c => c.Boolean(nullable: false));
            AddColumn("production_line", "is_read", c => c.Boolean(nullable: false));
            AddColumn("production_order", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_document_range", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_document", "is_read", c => c.Boolean(nullable: false));
            AddColumn("payment_type", "is_read", c => c.Boolean(nullable: false));
            AddColumn("payment_type_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("payment_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("payments", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_terminal", "is_read", c => c.Boolean(nullable: false));
            AddColumn("payment_schedual", "is_read", c => c.Boolean(nullable: false));
            AddColumn("payment_promissory_note", "is_read", c => c.Boolean(nullable: false));
            AddColumn("purchase_invoice", "is_read", c => c.Boolean(nullable: false));
            AddColumn("payment_withholding_details", "is_read", c => c.Boolean(nullable: false));
            AddColumn("payment_withholding_tax", "is_read", c => c.Boolean(nullable: false));
            AddColumn("payment_withholding_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("sales_invoice", "is_read", c => c.Boolean(nullable: false));
            AddColumn("crm_opportunity", "is_read", c => c.Boolean(nullable: false));
            AddColumn("sales_budget", "is_read", c => c.Boolean(nullable: false));
            AddColumn("projects", "is_read", c => c.Boolean(nullable: false));
            AddColumn("project_tag_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("project_tag", "is_read", c => c.Boolean(nullable: false));
            AddColumn("project_task", "is_read", c => c.Boolean(nullable: false));
            AddColumn("production_order_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("production_order_dimension", "is_read", c => c.Boolean(nullable: false));
            AddColumn("project_task_dimension", "is_read", c => c.Boolean(nullable: false));
            AddColumn("purchase_invoice_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_vat_group", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_vat_group_details", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_vat", "is_read", c => c.Boolean(nullable: false));
            AddColumn("purchase_invoice_dimension", "is_read", c => c.Boolean(nullable: false));
            AddColumn("purchase_order_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("purchase_order", "is_read", c => c.Boolean(nullable: false));
            AddColumn("purchase_tender", "is_read", c => c.Boolean(nullable: false));
            AddColumn("purchase_tender_contact", "is_read", c => c.Boolean(nullable: false));
            AddColumn("purchase_tender_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("purchase_tender_item", "is_read", c => c.Boolean(nullable: false));
            AddColumn("purchase_tender_dimension", "is_read", c => c.Boolean(nullable: false));
            AddColumn("sales_rep", "is_read", c => c.Boolean(nullable: false));
            AddColumn("purchase_order_dimension", "is_read", c => c.Boolean(nullable: false));
            AddColumn("sales_budget_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("sales_order_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("sales_order", "is_read", c => c.Boolean(nullable: false));
            AddColumn("sales_invoice_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("sales_packing_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("sales_packing", "is_read", c => c.Boolean(nullable: false));
            AddColumn("project_template", "is_read", c => c.Boolean(nullable: false));
            AddColumn("project_template_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_tag", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_tag_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("sales_return", "is_read", c => c.Boolean(nullable: false));
            AddColumn("sales_return_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("purchase_return", "is_read", c => c.Boolean(nullable: false));
            AddColumn("purchase_return_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("purchase_return_dimension", "is_read", c => c.Boolean(nullable: false));
            AddColumn("production_execution_dimension", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_movement_value", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_transfer", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_transfer_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_request_dimension", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_asset", "id_branch", c => c.Int());
            AddColumn("item_asset", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_asset_group", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_attachment", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_attachment", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_brand", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_price", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_property", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_recepie", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_recepie_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_service", "is_read", c => c.Boolean(nullable: false));
            AddColumn("project_event", "is_read", c => c.Boolean(nullable: false));
            AddColumn("project_event_fixed", "is_read", c => c.Boolean(nullable: false));
            AddColumn("project_event_template", "is_read", c => c.Boolean(nullable: false));
            AddColumn("project_event_template_fixed", "is_read", c => c.Boolean(nullable: false));
            AddColumn("project_event_template_variable", "is_read", c => c.Boolean(nullable: false));
            AddColumn("project_event_variable", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_request_decision", "is_read", c => c.Boolean(nullable: false));
            AddColumn("accounting_template", "is_read", c => c.Boolean(nullable: false));
            AddColumn("accounting_template_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_configuration", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_name_template", "is_read", c => c.Boolean(nullable: false));
            AddColumn("app_name_template_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("hr_timesheet", "is_read", c => c.Boolean(nullable: false));
            AddColumn("impexes", "is_read", c => c.Boolean(nullable: false));
            AddColumn("impex_expense", "is_read", c => c.Boolean(nullable: false));
            AddColumn("impex_incoterm_condition", "is_read", c => c.Boolean(nullable: false));
            AddColumn("impex_incoterm_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("impex_incoterm", "is_read", c => c.Boolean(nullable: false));
            AddColumn("impex_export", "is_read", c => c.Boolean(nullable: false));
            AddColumn("impex_import", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_inventory", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_inventory_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_inventory_dimension", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_template", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_template_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("item_transfer_dimension", "is_read", c => c.Boolean(nullable: false));
            AddColumn("purchase_packing", "is_read", c => c.Boolean(nullable: false));
            AddColumn("purchase_packing_detail", "is_read", c => c.Boolean(nullable: false));
            AddColumn("purchase_packing_dimension", "is_read", c => c.Boolean(nullable: false));
            CreateIndex("app_branch", "id_measurement");
            CreateIndex("item_movement", "id_sales_invoice_detail");
            CreateIndex("item_asset", "id_branch");
            AddForeignKey("item_asset", "id_branch", "app_branch", "id_branch");
            AddForeignKey("app_branch", "id_measurement", "app_measurement", "id_measurement");
            DropColumn("item_movement", "id_application");

            DropColumn("item_movement", "transaction_id");

            Sql(@"ALTER TABLE `abhi`.`item_movement` 
                    DROP FOREIGN KEY `FK_d7f23589f78a4e15b97260b12c8fbaa1`,
                    DROP FOREIGN KEY `FK_88afdd83a2e44c27b32645c87cd4482e`;
                    ALTER TABLE `abhi`.`item_movement` 
                    DROP COLUMN `sales_invoice_detail_id_sales_invoice_detail`,
                    DROP COLUMN `production_execution_detail_id_execution_detail`,
                    ADD INDEX `FK_item_movement_sales_packing_detail_id_production_execust_idx` (`id_execution_detail` ASC),
                    DROP INDEX `IX_id_execution_detail` ,
                    DROP INDEX `IX_sales_invoice_detail_id_sales_invoice_detail` ;
                    ALTER TABLE `abhi`.`item_movement` 
                    ADD CONSTRAINT `FK_item_movement_sales_packing_detail_id_sales_invoice_detail`
                      FOREIGN KEY (`id_sales_invoice_detail`)
                      REFERENCES `abhi`.`sales_invoice_detail` (`id_sales_invoice_detail`)
                      ON DELETE RESTRICT
                      ON UPDATE RESTRICT,
                    ADD CONSTRAINT `FK_item_movement_id_production_execustion_detail`
                      FOREIGN KEY (`id_execution_detail`)
                      REFERENCES `abhi`.`production_execution_detail` (`id_execution_detail`)
                      ON DELETE RESTRICT
                      ON UPDATE RESTRICT;
                    ");
        }

        public override void Down()
        {
            AddColumn("item_movement", "transaction_id", c => c.Int(nullable: false));
            AddColumn("item_movement", "id_production_execution_detail", c => c.Int());
            AddColumn("item_movement", "id_sales_packing", c => c.Int());
            AddColumn("item_movement", "id_inventory", c => c.Int());
            AddColumn("item_movement", "id_sales_invoice", c => c.Int());
            AddColumn("item_movement", "id_transfer", c => c.Int());
            AddColumn("item_movement", "id_application", c => c.Int(nullable: false));
            DropForeignKey("item_asset_maintainance_detail", "id_user", "security_user");
            DropForeignKey("item_asset_maintainance_detail", "id_maintainance", "item_asset_maintainance");
            DropForeignKey("item_asset_maintainance_detail", "id_item", "items");
            DropForeignKey("item_asset_maintainance_detail", "id_currencyfx", "app_currencyfx");
            DropForeignKey("item_asset_maintainance_detail", "id_company", "app_company");
            DropForeignKey("item_asset_maintainance", "id_user", "security_user");
            DropForeignKey("item_asset_maintainance", "id_item_asset", "item_asset");
            DropForeignKey("item_asset_maintainance", "id_company", "app_company");
            DropForeignKey("app_branch_walkins", "id_user", "security_user");
            DropForeignKey("app_branch_walkins", "id_company", "app_company");
            DropForeignKey("app_branch_walkins", "id_branch", "app_branch");
            DropForeignKey("app_branch", "id_measurement", "app_measurement");
            DropForeignKey("item_asset", "id_branch", "app_branch");
            DropIndex("item_asset_maintainance_detail", new[] { "id_user" });
            DropIndex("item_asset_maintainance_detail", new[] { "id_company" });
            DropIndex("item_asset_maintainance_detail", new[] { "id_currencyfx" });
            DropIndex("item_asset_maintainance_detail", new[] { "id_item" });
            DropIndex("item_asset_maintainance_detail", new[] { "id_maintainance" });
            DropIndex("item_asset_maintainance", new[] { "id_user" });
            DropIndex("item_asset_maintainance", new[] { "id_company" });
            DropIndex("item_asset_maintainance", new[] { "id_item_asset" });
            DropIndex("app_branch_walkins", new[] { "id_user" });
            DropIndex("app_branch_walkins", new[] { "id_company" });
            DropIndex("app_branch_walkins", new[] { "id_branch" });
            DropIndex("item_asset", new[] { "id_branch" });
            DropIndex("item_movement", new[] { "id_sales_invoice_detail" });
            DropIndex("app_branch", new[] { "id_measurement" });
            AlterColumn("item_movement", "id_sales_invoice_detail", c => c.Int());
            DropColumn("purchase_packing_dimension", "is_read");
            DropColumn("purchase_packing_detail", "is_read");
            DropColumn("purchase_packing", "is_read");
            DropColumn("item_transfer_dimension", "is_read");
            DropColumn("item_template_detail", "is_read");
            DropColumn("item_template", "is_read");
            DropColumn("item_inventory_dimension", "is_read");
            DropColumn("item_inventory_detail", "is_read");
            DropColumn("item_inventory", "is_read");
            DropColumn("impex_import", "is_read");
            DropColumn("impex_export", "is_read");
            DropColumn("impex_incoterm", "is_read");
            DropColumn("impex_incoterm_detail", "is_read");
            DropColumn("impex_incoterm_condition", "is_read");
            DropColumn("impex_expense", "is_read");
            DropColumn("impexes", "is_read");
            DropColumn("hr_timesheet", "is_read");
            DropColumn("app_name_template_detail", "is_read");
            DropColumn("app_name_template", "is_read");
            DropColumn("app_configuration", "is_read");
            DropColumn("accounting_template_detail", "is_read");
            DropColumn("accounting_template", "is_read");
            DropColumn("item_request_decision", "is_read");
            DropColumn("project_event_variable", "is_read");
            DropColumn("project_event_template_variable", "is_read");
            DropColumn("project_event_template_fixed", "is_read");
            DropColumn("project_event_template", "is_read");
            DropColumn("project_event_fixed", "is_read");
            DropColumn("project_event", "is_read");
            DropColumn("item_service", "is_read");
            DropColumn("item_recepie_detail", "is_read");
            DropColumn("item_recepie", "is_read");
            DropColumn("item_property", "is_read");
            DropColumn("item_price", "is_read");
            DropColumn("item_brand", "is_read");
            DropColumn("app_attachment", "is_read");
            DropColumn("item_attachment", "is_read");
            DropColumn("item_asset_group", "is_read");
            DropColumn("item_asset", "is_read");
            DropColumn("item_asset", "id_branch");
            DropColumn("item_request_dimension", "is_read");
            DropColumn("item_transfer_detail", "is_read");
            DropColumn("item_transfer", "is_read");
            DropColumn("item_movement_value", "is_read");
            DropColumn("production_execution_dimension", "is_read");
            DropColumn("purchase_return_dimension", "is_read");
            DropColumn("purchase_return_detail", "is_read");
            DropColumn("purchase_return", "is_read");
            DropColumn("sales_return_detail", "is_read");
            DropColumn("sales_return", "is_read");
            DropColumn("item_tag_detail", "is_read");
            DropColumn("item_tag", "is_read");
            DropColumn("project_template_detail", "is_read");
            DropColumn("project_template", "is_read");
            DropColumn("sales_packing", "is_read");
            DropColumn("sales_packing_detail", "is_read");
            DropColumn("sales_invoice_detail", "is_read");
            DropColumn("sales_order", "is_read");
            DropColumn("sales_order_detail", "is_read");
            DropColumn("sales_budget_detail", "is_read");
            DropColumn("purchase_order_dimension", "is_read");
            DropColumn("sales_rep", "is_read");
            DropColumn("purchase_tender_dimension", "is_read");
            DropColumn("purchase_tender_item", "is_read");
            DropColumn("purchase_tender_detail", "is_read");
            DropColumn("purchase_tender_contact", "is_read");
            DropColumn("purchase_tender", "is_read");
            DropColumn("purchase_order", "is_read");
            DropColumn("purchase_order_detail", "is_read");
            DropColumn("purchase_invoice_dimension", "is_read");
            DropColumn("app_vat", "is_read");
            DropColumn("app_vat_group_details", "is_read");
            DropColumn("app_vat_group", "is_read");
            DropColumn("purchase_invoice_detail", "is_read");
            DropColumn("project_task_dimension", "is_read");
            DropColumn("production_order_dimension", "is_read");
            DropColumn("production_order_detail", "is_read");
            DropColumn("project_task", "is_read");
            DropColumn("project_tag", "is_read");
            DropColumn("project_tag_detail", "is_read");
            DropColumn("projects", "is_read");
            DropColumn("sales_budget", "is_read");
            DropColumn("crm_opportunity", "is_read");
            DropColumn("sales_invoice", "is_read");
            DropColumn("payment_withholding_detail", "is_read");
            DropColumn("payment_withholding_tax", "is_read");
            DropColumn("payment_withholding_details", "is_read");
            DropColumn("purchase_invoice", "is_read");
            DropColumn("payment_promissory_note", "is_read");
            DropColumn("payment_schedual", "is_read");
            DropColumn("app_terminal", "is_read");
            DropColumn("payments", "is_read");
            DropColumn("payment_detail", "is_read");
            DropColumn("payment_type_detail", "is_read");
            DropColumn("payment_type", "is_read");
            DropColumn("app_document", "is_read");
            DropColumn("app_document_range", "is_read");
            DropColumn("production_order", "is_read");
            DropColumn("production_line", "is_read");
            DropColumn("production_execution", "is_read");
            DropColumn("hr_time_coefficient", "is_read");
            DropColumn("production_execution_detail", "is_read");
            DropColumn("item_price_list", "is_read");
            DropColumn("hr_talent", "is_read");
            DropColumn("hr_talent_detail", "is_read");
            DropColumn("hr_family", "is_read");
            DropColumn("hr_education", "is_read");
            DropColumn("hr_contract", "is_read");
            DropColumn("contact_tag", "is_read");
            DropColumn("contact_tag_detail", "is_read");
            DropColumn("contact_subscription", "is_read");
            DropColumn("contact_role", "is_read");
            DropColumn("app_field", "is_read");
            DropColumn("contact_field_value", "is_read");
            DropColumn("app_cost_center", "is_read");
            DropColumn("app_contract_detail", "is_read");
            DropColumn("app_condition", "is_read");
            DropColumn("app_contract", "is_read");
            DropColumn("contacts", "is_read");
            DropColumn("app_location", "is_read");
            DropColumn("item_movement", "is_read");
            DropColumn("item_movement_dimension", "is_read");
            DropColumn("app_dimension", "is_read");
            DropColumn("item_dimension", "is_read");
            DropColumn("item_product", "is_read");
            DropColumn("item_conversion_factor", "is_read");
            DropColumn("app_measurement_type", "is_read");
            DropColumn("app_measurement", "is_read");
            DropColumn("items", "is_read");
            DropColumn("item_request_detail", "is_read");
            DropColumn("app_currencyfx", "is_read");
            DropColumn("app_currency_denomination", "is_read");
            DropColumn("app_currency", "is_read");
            DropColumn("item_request", "is_read");
            DropColumn("hr_position", "is_read");
            DropColumn("app_department", "is_read");
            DropColumn("app_account_session", "is_read");
            DropColumn("app_account_detail", "is_read");
            DropColumn("app_account", "is_read");
            DropColumn("app_bank", "is_read");
            DropColumn("app_geography", "is_read");
            DropColumn("app_branch", "is_read");
            DropColumn("app_branch", "id_measurement");
            DropColumn("app_branch", "area");
            DropColumn("accounting_cycle", "is_read");
            DropColumn("accounting_journal", "is_read");
            DropColumn("accounting_journal_detail", "is_read");
            DropColumn("accounting_chart", "is_read");
            DropColumn("accounting_budget", "is_read");
            DropTable("item_asset_maintainance_detail");
            DropTable("item_asset_maintainance");
            DropTable("app_branch_walkins");
            RenameIndex(table: "dbo.item_movement", name: "IX_purchase_return_id_purchase_return", newName: "IX_id_purchase_return");
            RenameIndex(table: "dbo.item_movement", name: "IX_sales_return_id_sales_return", newName: "IX_id_sales_return");
            RenameIndex(table: "dbo.item_movement", name: "IX_purchase_invoice_id_purchase_invoice", newName: "IX_id_purchase_invoice");
            RenameIndex(table: "dbo.item_movement", name: "IX_production_execution_id_production_execution", newName: "IX_id_production_execution");
            RenameIndex(table: "dbo.item_movement", name: "IX_id_execution_detail", newName: "IX_production_execution_detail_id_execution_detail");
            RenameColumn(table: "item_movement", name: "purchase_return_id_purchase_return", newName: "id_purchase_return");
            RenameColumn(table: "item_movement", name: "sales_return_id_sales_return", newName: "id_sales_return");
            RenameColumn(table: "item_movement", name: "purchase_invoice_id_purchase_invoice", newName: "id_purchase_invoice");
            RenameColumn(table: "item_movement", name: "production_execution_id_production_execution", newName: "id_production_execution");
            RenameColumn(table: "item_movement", name: "id_sales_invoice_detail", newName: "sales_invoice_detail_id_sales_invoice_detail");
            RenameColumn(table: "item_movement", name: "id_execution_detail", newName: "production_execution_detail_id_execution_detail");
        
            CreateIndex("item_movement", "id_sales_packing");
            CreateIndex("item_movement", "id_sales_invoice");
            CreateIndex("item_movement", "id_transfer");
            AddForeignKey("item_movement", "id_sales_invoice", "sales_invoice", "id_sales_invoice");
            AddForeignKey("item_movement", "id_transfer", "item_transfer", "id_transfer");
            AddForeignKey("item_movement", "id_sales_packing", "sales_packing", "id_sales_packing");
        }
    }
}
