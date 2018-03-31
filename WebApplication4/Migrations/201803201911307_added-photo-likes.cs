namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedphotolikes : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Likes", "Photo_Id", "dbo.Photos");
            DropIndex("dbo.Likes", new[] { "Photo_Id" });
            //RenameColumn(table: "dbo.PhotoLikes", name: "Photo_Id", newName: "PhotoId");
            CreateTable(
                "dbo.PhotoLikes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        PhotoId = c.Guid(nullable: false),
                        Value = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Photos", t => t.PhotoId, cascadeDelete: false)
                .Index(t => t.UserId)
                .Index(t => t.PhotoId);
            
            DropColumn("dbo.Likes", "PhotoLike");
            DropColumn("dbo.Likes", "Photo_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Likes", "Photo_Id", c => c.Guid());
            AddColumn("dbo.Likes", "PhotoLike", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.PhotoLikes", "PhotoId", "dbo.Photos");
            DropForeignKey("dbo.PhotoLikes", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.PhotoLikes", new[] { "PhotoId" });
            DropIndex("dbo.PhotoLikes", new[] { "UserId" });
            DropTable("dbo.PhotoLikes");
            RenameColumn(table: "dbo.PhotoLikes", name: "PhotoId", newName: "Photo_Id");
            CreateIndex("dbo.Likes", "Photo_Id");
            AddForeignKey("dbo.Likes", "Photo_Id", "dbo.Photos", "Id");
        }
    }
}
