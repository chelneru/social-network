namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Fix : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.PhotoLikes", name: "PhotoId", newName: "PhotoId");
            RenameIndex(table: "dbo.PhotoLikes", name: "IX_PhotoId", newName: "IX_PhotoId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.PhotoLikes", name: "IX_PhotoId", newName: "IX_PhotoId");
            RenameColumn(table: "dbo.PhotoLikes", name: "PhotoId", newName: "PhotoId");
        }
    }
}
