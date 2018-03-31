namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedphotoandphotolikesmodels : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Photos", "UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.PhotoLikes", "PhotoId", "dbo.Photos");
            DropForeignKey("dbo.PhotoLikes", "UserProfileId", "dbo.UserProfiles");
            DropIndex("dbo.PhotoLikes", new[] { "UserProfileId" });
            DropIndex("dbo.PhotoLikes", new[] { "PhotoId" });
            DropIndex("dbo.Photos", new[] { "UserProfileId" });
            AddColumn("dbo.Posts", "PhotoLink", c => c.String());
            AddColumn("dbo.Posts", "VideoLink", c => c.String());
            AddColumn("dbo.Posts", "ShareLink", c => c.String());
            DropTable("dbo.PhotoLikes");
            DropTable("dbo.Photos");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Photos",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        LocalPath = c.String(nullable: false),
                        Description = c.String(),
                        Edited = c.Boolean(nullable: false),
                        PostDateTime = c.DateTime(nullable: false),
                        UserProfileId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PhotoLikes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserProfileId = c.Guid(nullable: false),
                        PhotoId = c.Guid(nullable: false),
                        Value = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.Posts", "ShareLink");
            DropColumn("dbo.Posts", "VideoLink");
            DropColumn("dbo.Posts", "PhotoLink");
            CreateIndex("dbo.Photos", "UserProfileId");
            CreateIndex("dbo.PhotoLikes", "PhotoId");
            CreateIndex("dbo.PhotoLikes", "UserProfileId");
            AddForeignKey("dbo.PhotoLikes", "UserProfileId", "dbo.UserProfiles", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PhotoLikes", "PhotoId", "dbo.Photos", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Photos", "UserProfileId", "dbo.UserProfiles", "Id", cascadeDelete: true);
        }
    }
}
