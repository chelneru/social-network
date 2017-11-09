namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class generateguid : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Posts");
            AlterColumn("dbo.Posts", "Id", c => c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"));
            AddPrimaryKey("dbo.Posts", "Id");
        }

        public override void Down()
        {
            DropPrimaryKey("dbo.Posts");
            AlterColumn("dbo.Posts", "Id", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.Posts", "Id");
        }
    }
}
