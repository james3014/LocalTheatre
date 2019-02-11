namespace LocalTheatre.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedAnnouncementModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Announcements", "Category", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Announcements", "Category", c => c.String(nullable: false));
        }
    }
}
