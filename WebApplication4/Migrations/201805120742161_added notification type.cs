namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addednotificationtype : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "isRequest", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Notifications", "isRequest");
        }
    }
}
