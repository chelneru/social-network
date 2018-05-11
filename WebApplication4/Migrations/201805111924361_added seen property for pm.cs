namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedseenpropertyforpm : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PrivateMessages", "Seen", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PrivateMessages", "Seen");
        }
    }
}
