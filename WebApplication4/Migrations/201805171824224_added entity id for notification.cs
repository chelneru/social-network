namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedentityidfornotification : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "EntityId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Notifications", "EntityId");
        }
    }
}
