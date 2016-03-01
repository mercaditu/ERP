namespace entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSecurity_user : DbMigration
    {
        public override void Up()
        {
            AddColumn("security_user", "trans_date", c => c.DateTime(precision: 0));
            AddColumn("security_user", "id_created_user", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("security_user", "id_created_user");
            DropColumn("security_user", "trans_date");
        }
    }
}
