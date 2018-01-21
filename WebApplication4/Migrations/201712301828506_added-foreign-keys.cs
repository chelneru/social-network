namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class addedforeignkeys : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Likes", "UserId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Likes", "UserId");
            CreateIndex("dbo.Likes", "PostId");
            AddForeignKey("dbo.Likes", "PostId", "dbo.Posts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Likes", "UserId", "dbo.AspNetUsers", "Id");
        }

        public override void Down()
        {
            DropForeignKey("dbo.Likes", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Likes", "PostId", "dbo.Posts");
            DropIndex("dbo.Likes", new[] { "PostId" });
            DropIndex("dbo.Likes", new[] { "UserId" });
            AlterColumn("dbo.Likes", "UserId", c => c.Guid(nullable: false));
        }
    }
}
