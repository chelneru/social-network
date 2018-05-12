namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedusedproperty : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FriendRequests", "Used", c => c.Short(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FriendRequests", "Used", c => c.Boolean(nullable: false));
        }
    }
}
