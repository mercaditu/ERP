namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProject_task : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("sales_order_detail", "id_project_task", "project_task");
            DropIndex("sales_order_detail", new[] { "id_project_task" });
            AddColumn("project_task", "sales_detail_id_sales_order_detail", c => c.Int());
            AddColumn("sales_order_detail", "project_task_id_project_task", c => c.Int());
            AddColumn("sales_order_detail", "project_task_id_project_task1", c => c.Int());
            CreateIndex("project_task", "sales_detail_id_sales_order_detail");
            CreateIndex("sales_order_detail", "project_task_id_project_task");
            CreateIndex("sales_order_detail", "project_task_id_project_task1");
            AddForeignKey("project_task", "sales_detail_id_sales_order_detail", "sales_order_detail", "id_sales_order_detail");
            AddForeignKey("sales_order_detail", "project_task_id_project_task1", "project_task", "id_project_task");
            AddForeignKey("sales_order_detail", "project_task_id_project_task", "project_task", "id_project_task");
        }
        
        public override void Down()
        {
            DropForeignKey("sales_order_detail", "project_task_id_project_task", "project_task");
            DropForeignKey("sales_order_detail", "project_task_id_project_task1", "project_task");
            DropForeignKey("project_task", "sales_detail_id_sales_order_detail", "sales_order_detail");
            DropIndex("sales_order_detail", new[] { "project_task_id_project_task1" });
            DropIndex("sales_order_detail", new[] { "project_task_id_project_task" });
            DropIndex("project_task", new[] { "sales_detail_id_sales_order_detail" });
            DropColumn("sales_order_detail", "project_task_id_project_task1");
            DropColumn("sales_order_detail", "project_task_id_project_task");
            DropColumn("project_task", "sales_detail_id_sales_order_detail");
            CreateIndex("sales_order_detail", "id_project_task");
            AddForeignKey("sales_order_detail", "id_project_task", "project_task", "id_project_task");
        }
    }
}
