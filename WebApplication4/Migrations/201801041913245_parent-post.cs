namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class parentpost : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Posts", "ParentPost_Id", c => c.Guid());
            CreateIndex("dbo.Posts", "ParentPost_Id");
            AddForeignKey("dbo.Posts", "ParentPost_Id", "dbo.Posts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Posts", "ParentPost_Id", "dbo.Posts");
            DropIndex("dbo.Posts", new[] { "ParentPost_Id" });
            DropColumn("dbo.Posts", "ParentPost_Id");
        }
    }
}
