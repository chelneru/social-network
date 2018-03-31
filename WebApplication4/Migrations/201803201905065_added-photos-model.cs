namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedphotosmodel : DbMigration
    {
        public override void Up()
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
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Likes", "PhotoLike", c => c.Boolean(nullable: false));
            AddColumn("dbo.Likes", "Photo_Id", c => c.Guid());
            CreateIndex("dbo.Likes", "Photo_Id");
            AddForeignKey("dbo.Likes", "Photo_Id", "dbo.Photos", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Photos", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Likes", "Photo_Id", "dbo.Photos");
            DropIndex("dbo.Photos", new[] { "UserId" });
            DropIndex("dbo.Likes", new[] { "Photo_Id" });
            DropColumn("dbo.Likes", "Photo_Id");
            DropColumn("dbo.Likes", "PhotoLike");
            DropTable("dbo.Photos");
        }
    }
}
