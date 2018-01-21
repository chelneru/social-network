namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedlikevalue : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Likes", "Value", c => c.Short(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Likes", "Value", c => c.Boolean(nullable: false));
        }
    }
}
