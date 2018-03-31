namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class posteditedattribute : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Posts", "Edited", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Posts", "Edited");
        }
    }
}
