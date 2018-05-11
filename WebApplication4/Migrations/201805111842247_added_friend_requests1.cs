namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_friend_requests1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FriendRequests", "Used", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FriendRequests", "Used");
        }
    }
}
