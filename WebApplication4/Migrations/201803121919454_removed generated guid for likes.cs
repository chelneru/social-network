namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Removedgeneratedguidforlikes : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Likes");
            AlterColumn("dbo.Likes", "Id", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.Likes", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Likes");
            AlterColumn("dbo.Likes", "Id", c => c.Guid(nullable: false, identity: true));
            AddPrimaryKey("dbo.Likes", "Id");
        }
    }
}
