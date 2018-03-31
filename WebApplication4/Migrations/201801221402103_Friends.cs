namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Friends : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "UserProfile_Id", c => c.Guid());
            CreateIndex("dbo.AspNetUsers", "UserProfile_Id");
            AddForeignKey("dbo.AspNetUsers", "UserProfile_Id", "dbo.UserProfiles", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "UserProfile_Id", "dbo.UserProfiles");
            DropIndex("dbo.AspNetUsers", new[] { "UserProfile_Id" });
            DropColumn("dbo.AspNetUsers", "UserProfile_Id");
        }
    }
}
