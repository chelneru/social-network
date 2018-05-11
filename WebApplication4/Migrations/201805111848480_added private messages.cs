namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedprivatemessages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PrivateMessages",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        InitiatorUserProfileId = c.Guid(nullable: false),
                        TargetUserProfileId = c.Guid(nullable: false),
                        Content = c.String(),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PrivateMessages");
        }
    }
}
