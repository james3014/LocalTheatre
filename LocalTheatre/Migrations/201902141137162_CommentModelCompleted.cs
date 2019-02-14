namespace LocalTheatre.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommentModelCompleted : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Announcements", "User_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Comments", "User_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Announcements", "User_Id");
            CreateIndex("dbo.Comments", "User_Id");
            AddForeignKey("dbo.Comments", "User_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Announcements", "User_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Announcements", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Comments", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Comments", new[] { "User_Id" });
            DropIndex("dbo.Announcements", new[] { "User_Id" });
            DropColumn("dbo.Comments", "User_Id");
            DropColumn("dbo.Announcements", "User_Id");
        }
    }
}
