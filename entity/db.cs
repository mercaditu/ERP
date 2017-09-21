namespace entity
{
    using System.Configuration;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.Core.EntityClient;
    using System.Threading;
    using System.Threading.Tasks;
    using Z.EntityFramework.Plus;

    public class db : DbContext
    {
        public override int SaveChanges()
        {
            var audit = new Z.EntityFramework.Plus.Audit();

            audit.CreatedBy = CurrentSession.User != null ? CurrentSession.User.name : "Unkown" ;
            audit.PreSaveChanges(this);
            var rowAffecteds = base.SaveChanges();
            audit.PostSaveChanges();

            if (audit.Configuration.AutoSavePreAction != null)
            {
                audit.Configuration.AutoSavePreAction(this, audit);
                base.SaveChanges();
            }

            return rowAffecteds;
        }

        public override Task<int> SaveChangesAsync()
        {
            return SaveChangesAsync(CancellationToken.None);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            var audit = new Z.EntityFramework.Plus.Audit();
            audit.PreSaveChanges(this);
            var rowAffecteds = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            audit.PostSaveChanges();

            if (audit.Configuration.AutoSavePreAction != null)
            {
                audit.Configuration.AutoSavePreAction(this, audit);
                await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }

            return rowAffecteds;
        }

        public db() : base("name = Cognitivo.Properties.Settings.MySQLconnString")
        {
            Configuration.LazyLoadingEnabled = true;
            AuditManager.DefaultConfiguration.Exclude<item_inventory_detail>();
            AuditManager.DefaultConfiguration.Exclude<item_movement>();
            AuditManager.DefaultConfiguration.Exclude<item_movement_dimension>();
            AuditManager.DefaultConfiguration.Exclude<item_movement_value>();
            AuditManager.DefaultConfiguration.Exclude<payment_schedual>();
            AuditManager.DefaultConfiguration.Exclude<app_notification>();
            AuditManager.DefaultConfiguration.AutoSavePreAction = (context, audit) =>
        // ADD "Where(x => x.AuditEntryID == 0)" to allow multiple SaveChanges with same Audit
        (context as db).AuditEntries.AddRange(audit.Entries);
        }

        public static DbConnection GetSqlConnection()
        {
            // Initialize the EntityConnectionStringBuilder. 
            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();

            var connectionSettings = ConfigurationManager.ConnectionStrings["Cognitivo.Properties.Settings.MySQLconnString"];

            // Set the provider name. 
            entityBuilder.Provider = connectionSettings.ProviderName;

            // Set the provider-specific connection string. 
            entityBuilder.ProviderConnectionString = connectionSettings.ConnectionString;

            // Set the Metadata location. 
            //entityBuilder.Metadata = "res://*/Models.TestModel.csdl|res://*/Models.TestModel.ssdl|res://*/Models.TestModel.msl";

            return new EntityConnection(entityBuilder.ToString());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Properties<decimal>().Configure(c => c.HasPrecision(20, 9));
        }

        public DbSet<AuditEntry> AuditEntries { get; set; }
        public DbSet<AuditEntryProperty> AuditEntryProperties { get; set; }

        public virtual DbSet<app_account> app_account { get; set; }
        public virtual DbSet<app_account_detail> app_account_detail { get; set; }
        public virtual DbSet<app_account_session> app_account_session { get; set; }
        public virtual DbSet<app_attachment> app_attachment { get; set; }
        public virtual DbSet<app_bank> app_bank { get; set; }
        public virtual DbSet<app_branch> app_branch { get; set; }
        public virtual DbSet<app_branch_walkins> app_branch_walkins { get; set; }
        public virtual DbSet<app_company> app_company { get; set; }
        public virtual DbSet<app_comment> app_comment { get; set; }
        public virtual DbSet<app_company_interest> app_company_interest { get; set; }
        public virtual DbSet<app_condition> app_condition { get; set; }
        public virtual DbSet<app_contract> app_contract { get; set; }
        public virtual DbSet<app_contract_detail> app_contract_detail { get; set; }
        public virtual DbSet<app_cost_center> app_cost_center { get; set; }
        public virtual DbSet<app_currency> app_currency { get; set; }
        public virtual DbSet<app_currency_denomination> app_currency_denomination { get; set; }
        public virtual DbSet<app_currencyfx> app_currencyfx { get; set; }
        public virtual DbSet<app_geography> app_geography { get; set; }
        public virtual DbSet<app_field> app_field { get; set; }
        public virtual DbSet<app_document> app_document { get; set; }
        public virtual DbSet<app_dimension> app_dimension { get; set; }
        public virtual DbSet<app_document_range> app_document_range { get; set; }
        public virtual DbSet<app_property> app_property { get; set; }
        public virtual DbSet<app_name_template> app_name_template { get; set; }
        public virtual DbSet<app_name_template_detail> app_name_template_detail { get; set; }
        public virtual DbSet<app_measurement> app_measurement { get; set; }
        public virtual DbSet<app_measurement_type> app_measurement_type { get; set; }
        public virtual DbSet<app_vat> app_vat { get; set; }
        public virtual DbSet<app_weather> app_weather { get; set; }
        public virtual DbSet<app_terminal> app_terminal { get; set; }
        public virtual DbSet<app_location> app_location { get; set; }
        public virtual DbSet<app_configuration> app_configuration { get; set; }
        public virtual DbSet<app_notification> app_notification { get; set; }
        public virtual DbSet<contact> contacts { get; set; }
        public virtual DbSet<contact_field_value> contact_field_value { get; set; }
        public virtual DbSet<contact_role> contact_role { get; set; }
        public virtual DbSet<contact_subscription> contact_subscription { get; set; }
        public virtual DbSet<contact_tag_detail> contact_tag_detail { get; set; }
        public virtual DbSet<contact_tag> contact_tag { get; set; }
        public virtual DbSet<hr_talent> hr_talent { get; set; }
        public virtual DbSet<hr_talent_detail> hr_talent_detail { get; set; }
        public virtual DbSet<hr_timesheet> hr_timesheet { get; set; }
        public virtual DbSet<hr_position> hr_position { get; set; }
        public virtual DbSet<hr_family> hr_family { get; set; }
        public virtual DbSet<hr_education> hr_education { get; set; }
        public virtual DbSet<hr_contract> hr_contract { get; set; }
        public virtual DbSet<hr_time_coefficient> hr_time_coefficient { get; set; }
        public virtual DbSet<impex> impex { get; set; }
        public virtual DbSet<impex_incoterm> impex_incoterm { get; set; }
        public virtual DbSet<impex_incoterm_condition> impex_incoterm_condition { get; set; }
        public virtual DbSet<impex_incoterm_detail> impex_incoterm_detail { get; set; }
        public virtual DbSet<impex_expense> impex_expense { get; set; }
        public virtual DbSet<impex_import> impex_import { get; set; }
        public virtual DbSet<impex_export> impex_export { get; set; }
        public virtual DbSet<item> items { get; set; }
        public virtual DbSet<item_branch_safety> item_branch_safety { get; set; }
        public virtual DbSet<item_request> item_request { get; set; }
        public virtual DbSet<item_request_detail> item_request_detail { get; set; }
        public virtual DbSet<item_request_decision> item_request_decision { get; set; }
        public virtual DbSet<item_asset> item_asset { get; set; }
        public virtual DbSet<item_asset_group> item_asset_group { get; set; }
        public virtual DbSet<item_asset_maintainance> item_asset_maintainance { get; set; }
        public virtual DbSet<item_asset_maintainance_detail> item_asset_maintainance_detail { get; set; }
        public virtual DbSet<item_product> item_product { get; set; }
        public virtual DbSet<item_service> item_service { get; set; }
        public virtual DbSet<item_price> item_price { get; set; }
        public virtual DbSet<item_property> item_property { get; set; }
        public virtual DbSet<item_dimension> item_dimension { get; set; }
        public virtual DbSet<item_conversion_factor> item_conversion_factor { get; set; }
        public virtual DbSet<item_attachment> item_attachment { get; set; }
        public virtual DbSet<item_price_list> item_price_list { get; set; }
        public virtual DbSet<item_movement> item_movement { get; set; }
        public virtual DbSet<item_movement_value> item_movement_value { get; set; }
        public virtual DbSet<item_recepie> item_recepie { get; set; }
        public virtual DbSet<item_recepie_detail> item_recepie_detail { get; set; }
        public virtual DbSet<item_tag_detail> item_tag_detail { get; set; }
        public virtual DbSet<item_tag> item_tag { get; set; }
        public virtual DbSet<item_brand> item_brand { get; set; }
        public virtual DbSet<item_inventory> item_inventory { get; set; }
        public virtual DbSet<item_inventory_detail> item_inventory_detail { get; set; }
        public virtual DbSet<item_transfer> item_transfer { get; set; }
        public virtual DbSet<item_transfer_detail> item_transfer_detail { get; set; }
        public virtual DbSet<item_template> item_template { get; set; }
        public virtual DbSet<item_template_detail> item_template_detail { get; set; }
        public virtual DbSet<payment> payments { get; set; }
        public virtual DbSet<payment_promissory_note> payment_promissory_note { get; set; }
        public virtual DbSet<payment_detail> payment_detail { get; set; }
        public virtual DbSet<payment_approve> payment_approve { get; set; }
        public virtual DbSet<payment_approve_detail> payment_approve_detail { get; set; }
        public virtual DbSet<payment_type> payment_type { get; set; }
        public virtual DbSet<payment_type_detail> payment_type_detail { get; set; }
        public virtual DbSet<payment_schedual> payment_schedual { get; set; }
        public virtual DbSet<payment_withholding_tax> payment_withholding_tax { get; set; }
        public virtual DbSet<payment_withholding_detail> payment_withholding_detail { get; set; }
        public virtual DbSet<production_order> production_order { get; set; }
        public virtual DbSet<production_order_detail> production_order_detail { get; set; }
        public virtual DbSet<production_line> production_line { get; set; }
        public virtual DbSet<project> projects { get; set; }
        public virtual DbSet<project_tag_detail> project_tag_detail { get; set; }
        public virtual DbSet<project_tag> project_tag { get; set; }
        public virtual DbSet<project_template> project_template { get; set; }
        public virtual DbSet<project_template_detail> project_template_detail { get; set; }
        public virtual DbSet<production_execution_detail> production_execution_detail { get; set; }
        public virtual DbSet<production_service_account> production_service_account { get; set; }
        public virtual DbSet<project_task> project_task { get; set; }
        public virtual DbSet<project_task_dimension> project_task_dimension { get; set; }
        public virtual DbSet<crm_opportunity> crm_opportunity { get; set; }
        public virtual DbSet<crm_schedual> crm_schedual { get; set; }
        public virtual DbSet<sales_budget> sales_budget { get; set; }
        public virtual DbSet<sales_budget_detail> sales_budget_detail { get; set; }
        public virtual DbSet<sales_order> sales_order { get; set; }
        public virtual DbSet<sales_order_detail> sales_order_detail { get; set; }
        public virtual DbSet<sales_invoice> sales_invoice { get; set; }
        public virtual DbSet<sales_invoice_detail> sales_invoice_detail { get; set; }
        public virtual DbSet<sales_rep> sales_rep { get; set; }
        public virtual DbSet<sales_return> sales_return { get; set; }
        public virtual DbSet<sales_promotion> sales_promotion { get; set; }
        public virtual DbSet<sales_return_detail> sales_return_detail { get; set; }
        public virtual DbSet<security_user> security_user { get; set; }
        public virtual DbSet<security_request> security_request { get; set; }
        public virtual DbSet<security_role> security_role { get; set; }
        public virtual DbSet<security_crud> security_curd { get; set; }
        public virtual DbSet<security_privilage> security_privilage { get; set; }
        public virtual DbSet<security_role_privilage> security_role_privilage { get; set; }
        public virtual DbSet<security_question> security_question { get; set; }
        public virtual DbSet<purchase_invoice> purchase_invoice { get; set; }
        public virtual DbSet<purchase_invoice_detail> purchase_invoice_detail { get; set; }
        public virtual DbSet<purchase_order> purchase_order { get; set; }
        public virtual DbSet<purchase_order_detail> purchase_order_detail { get; set; }
        public virtual DbSet<purchase_packing> purchase_packing { get; set; }
        public virtual DbSet<purchase_packing_detail> purchase_packing_detail { get; set; }
        public virtual DbSet<purchase_packing_relation> purchase_packing_relation { get; set; }
        public virtual DbSet<purchase_packing_detail_relation> purchase_packing_detail_relation { get; set; }
        public virtual DbSet<purchase_tender> purchase_tender { get; set; }
        public virtual DbSet<purchase_tender_contact> purchase_tender_contact_detail { get; set; }
        public virtual DbSet<purchase_tender_item> purchase_tender_item_detail { get; set; }
        public virtual DbSet<purchase_tender_detail> purchase_tender_detail { get; set; }
        public virtual DbSet<purchase_return> purchase_return { get; set; }
        public virtual DbSet<purchase_return_detail> purchase_return_detail { get; set; }
        public virtual DbSet<purchase_invoice_dimension> purchase_invoice_dimension { get; set; }
        public virtual DbSet<purchase_order_dimension> purchase_order_dimension { get; set; }
        public virtual DbSet<purchase_packing_dimension> purchase_packing_dimension { get; set; }
        public virtual DbSet<purchase_return_dimension> purchase_return_dimension { get; set; }
        public virtual DbSet<purchase_tender_dimension> purchase_tender_dimension { get; set; }
        public virtual DbSet<purchase_tender_detail_dimension> purchase_tender_detail_dimension { get; set; }
        public virtual DbSet<item_movement_dimension> item_movement_dimension { get; set; }
        public virtual DbSet<item_inventory_dimension> item_inventory_dimension { get; set; }
        public virtual DbSet<item_transfer_dimension> item_transfer_dimension { get; set; }
        public virtual DbSet<item_request_dimension> item_request_dimension { get; set; }
        public virtual DbSet<sales_packing> sales_packing { get; set; }
        public virtual DbSet<sales_packing_detail> sales_packing_detail { get; set; }
        public virtual DbSet<sales_packing_relation> sales_packing_relation { get; set; }
        public virtual DbSet<app_vat_group> app_vat_group { get; set; }
        public virtual DbSet<app_vat_group_details> app_vat_group_details { get; set; }
        public virtual DbSet<app_department> app_department { get; set; }
        public virtual DbSet<project_event> project_event { get; set; }
        public virtual DbSet<project_event_variable> project_event_variable { get; set; }
        public virtual DbSet<project_event_fixed> project_event_fixed { get; set; }
        public virtual DbSet<project_event_template> project_event_template { get; set; }
        public virtual DbSet<project_event_template_variable> project_event_template_variable { get; set; }
        public virtual DbSet<project_event_template_fixed> project_event_template_fixed { get; set; }

        public virtual DbSet<loyalty_member> loyalty_member { get; set; }
        public virtual DbSet<loyalty_member_detail> loyalty_member_detail { get; set; }
        public virtual DbSet<loyalty_tier> loyalty_tier { get; set; }
    }
}