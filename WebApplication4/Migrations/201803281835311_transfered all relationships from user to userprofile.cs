namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class transferedallrelationshipsfromusertouserprofile : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Posts", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Likes", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Photos", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PhotoLikes", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Likes", new[] { "UserId" });
            DropIndex("dbo.Posts", new[] { "UserId" });
            DropIndex("dbo.PhotoLikes", new[] { "UserId" });
            DropIndex("dbo.Photos", new[] { "UserId" });
            AddColumn("dbo.Likes", "UserProfileId", c => c.Guid(nullable: false));
            AddColumn("dbo.Posts", "UserProfileId", c => c.Guid(nullable: false));
            AddColumn("dbo.PhotoLikes", "UserProfileId", c => c.Guid(nullable: false));
            AddColumn("dbo.Photos", "UserProfileId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Likes", "UserProfileId");
            CreateIndex("dbo.Posts", "UserProfileId");
            CreateIndex("dbo.PhotoLikes", "UserProfileId");
            CreateIndex("dbo.Photos", "UserProfileId");
            AddForeignKey("dbo.Posts", "UserProfileId", "dbo.UserProfiles", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Likes", "UserProfileId", "dbo.UserProfiles", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Photos", "UserProfileId", "dbo.UserProfiles", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PhotoLikes", "UserProfileId", "dbo.UserProfiles", "Id", cascadeDelete: true);
            DropColumn("dbo.Likes", "UserId");
            DropColumn("dbo.Posts", "UserId");
            DropColumn("dbo.PhotoLikes", "UserId");
            DropColumn("dbo.Photos", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Photos", "UserId", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.PhotoLikes", "UserId", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Posts", "UserId", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Likes", "UserId", c => c.String(nullable: false, maxLength: 128));
            DropForeignKey("dbo.PhotoLikes", "UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.Photos", "UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.Likes", "UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.Posts", "UserProfileId", "dbo.UserProfiles");
            DropIndex("dbo.Photos", new[] { "UserProfileId" });
            DropIndex("dbo.PhotoLikes", new[] { "UserProfileId" });
            DropIndex("dbo.Posts", new[] { "UserProfileId" });
            DropIndex("dbo.Likes", new[] { "UserProfileId" });
            DropColumn("dbo.Photos", "UserProfileId");
            DropColumn("dbo.PhotoLikes", "UserProfileId");
            DropColumn("dbo.Posts", "UserProfileId");
            DropColumn("dbo.Likes", "UserProfileId");
            CreateIndex("dbo.Photos", "UserId");
            CreateIndex("dbo.PhotoLikes", "UserId");
            CreateIndex("dbo.Posts", "UserId");
            CreateIndex("dbo.Likes", "UserId");
            AddForeignKey("dbo.PhotoLikes", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Photos", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Likes", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Posts", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
