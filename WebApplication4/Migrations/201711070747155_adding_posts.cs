namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class adding_posts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Posts",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
                    PostDateTime = c.DateTime(nullable: false),
                    UserId = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.Posts", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Posts", new[] { "UserId" });
            DropTable("dbo.Posts");
        }
    }
}
