namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class autogeneratelinkpreviewid : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.LinkPreviews");
            AlterColumn("dbo.LinkPreviews", "Id", c => c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"));
            AddPrimaryKey("dbo.LinkPreviews", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.LinkPreviews");
            AlterColumn("dbo.LinkPreviews", "Id", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.LinkPreviews", "Id");
        }
    }
}
