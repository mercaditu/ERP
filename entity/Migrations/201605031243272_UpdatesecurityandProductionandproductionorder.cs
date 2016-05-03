namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatesecurityandProductionandproductionorder : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "production_order_dimension",
                c => new
                    {
                        id_order_dimension = c.Int(nullable: false, identity: true),
                        id_order_detail = c.Int(nullable: false),
                        id_dimension = c.Int(nullable: false),
                        id_measurement = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 4),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
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
                "production_execution_dimension",
                c => new
                    {
                        id_execution_dimension = c.Int(nullable: false, identity: true),
                        id_execution_detail = c.Int(nullable: false),
                        id_dimension = c.Int(nullable: false),
                        id_measurement = c.Int(nullable: false),
                        value = c.Decimal(nullable: false, precision: 20, scale: 4),
                        id_company = c.Int(nullable: false),
                        id_user = c.Int(nullable: false),
                        is_head = c.Boolean(nullable: false),
                        timestamp = c.DateTime(nullable: false, precision: 0),
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
            
            AddColumn("production_order", "types", c => c.Int(nullable: false));
            AlterColumn("security_privilage", "name", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("production_execution_dimension", "id_user", "security_user");
            DropForeignKey("production_execution_dimension", "id_execution_detail", "production_execution_detail");
            DropForeignKey("production_execution_dimension", "id_measurement", "app_measurement");
            DropForeignKey("production_execution_dimension", "id_dimension", "app_dimension");
            DropForeignKey("production_execution_dimension", "id_company", "app_company");
            DropForeignKey("production_order_dimension", "id_user", "security_user");
            DropForeignKey("production_order_dimension", "id_order_detail", "production_order_detail");
            DropForeignKey("production_order_dimension", "id_measurement", "app_measurement");
            DropForeignKey("production_order_dimension", "id_dimension", "app_dimension");
            DropForeignKey("production_order_dimension", "id_company", "app_company");
            DropIndex("production_execution_dimension", new[] { "id_user" });
            DropIndex("production_execution_dimension", new[] { "id_company" });
            DropIndex("production_execution_dimension", new[] { "id_measurement" });
            DropIndex("production_execution_dimension", new[] { "id_dimension" });
            DropIndex("production_execution_dimension", new[] { "id_execution_detail" });
            DropIndex("production_order_dimension", new[] { "id_user" });
            DropIndex("production_order_dimension", new[] { "id_company" });
            DropIndex("production_order_dimension", new[] { "id_measurement" });
            DropIndex("production_order_dimension", new[] { "id_dimension" });
            DropIndex("production_order_dimension", new[] { "id_order_detail" });
            AlterColumn("security_privilage", "name", c => c.String(unicode: false));
            DropColumn("production_order", "types");
            DropTable("production_execution_dimension");
            DropTable("production_order_dimension");
        }
    }
}
