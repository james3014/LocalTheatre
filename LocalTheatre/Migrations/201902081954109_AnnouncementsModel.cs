namespace LocalTheatre.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AnnouncementsModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Announcements",
                c => new
                    {
                        AnnouncementId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Announcement = c.String(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Category = c.String(nullable: false),
                        Author = c.String(),
                    })
                .PrimaryKey(t => t.AnnouncementId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Announcements");
        }
    }
}
