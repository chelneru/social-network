namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addeduseraddresinprofile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfiles", "UserAddress", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfiles", "UserAddress");
        }
    }
}
