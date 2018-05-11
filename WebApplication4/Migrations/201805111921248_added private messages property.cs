namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedprivatemessagesproperty : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.PrivateMessages");
            AlterColumn("dbo.PrivateMessages", "Id", c => c.Guid(nullable: false, identity: true));
            AddPrimaryKey("dbo.PrivateMessages", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.PrivateMessages");
            AlterColumn("dbo.PrivateMessages", "Id", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.PrivateMessages", "Id");
        }
    }
}
