namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cascadedelete : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Likes", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserProfiles", "User_Id", "dbo.AspNetUsers");
            AddForeignKey("dbo.Likes", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: false);
            AddForeignKey("dbo.UserProfiles", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserProfiles", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Likes", "UserId", "dbo.AspNetUsers");
            AddForeignKey("dbo.UserProfiles", "User_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Likes", "UserId", "dbo.AspNetUsers", "Id");
        }
    }
}
