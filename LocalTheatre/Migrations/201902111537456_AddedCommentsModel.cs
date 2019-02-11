namespace LocalTheatre.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCommentsModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        CommentId = c.Int(nullable: false, identity: true),
                        CommentBody = c.String(nullable: false),
                        CommentDate = c.DateTime(nullable: false),
                        CommentAuthor = c.String(),
                        AnnouncementId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CommentId)
                .ForeignKey("dbo.Announcements", t => t.AnnouncementId, cascadeDelete: true)
                .Index(t => t.AnnouncementId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "AnnouncementId", "dbo.Announcements");
            DropIndex("dbo.Comments", new[] { "AnnouncementId" });
            DropTable("dbo.Comments");
        }
    }
}
