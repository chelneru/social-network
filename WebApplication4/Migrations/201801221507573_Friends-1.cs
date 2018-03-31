namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Friends1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "UserProfile_Id", "dbo.UserProfiles");
            DropIndex("dbo.AspNetUsers", new[] { "UserProfile_Id" });
            CreateTable(
                "dbo.Friends",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserProfile_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfile_Id)
                .Index(t => t.UserProfile_Id);
            
            AddColumn("dbo.UserProfiles", "Friends_Id", c => c.Guid());
            CreateIndex("dbo.UserProfiles", "Friends_Id");
            AddForeignKey("dbo.UserProfiles", "Friends_Id", "dbo.Friends", "Id");
            DropColumn("dbo.AspNetUsers", "UserProfile_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "UserProfile_Id", c => c.Guid());
            DropForeignKey("dbo.Friends", "UserProfile_Id", "dbo.UserProfiles");
            DropForeignKey("dbo.UserProfiles", "Friends_Id", "dbo.Friends");
            DropIndex("dbo.UserProfiles", new[] { "Friends_Id" });
            DropIndex("dbo.Friends", new[] { "UserProfile_Id" });
            DropColumn("dbo.UserProfiles", "Friends_Id");
            DropTable("dbo.Friends");
            CreateIndex("dbo.AspNetUsers", "UserProfile_Id");
            AddForeignKey("dbo.AspNetUsers", "UserProfile_Id", "dbo.UserProfiles", "Id");
        }
    }
}
