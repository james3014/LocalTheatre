namespace LocalTheatre.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedUserRolesExpandedUser : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ExpandedUsers", "LockoutEndDate");
            DropColumn("dbo.ExpandedUsers", "AccessFailedCount");
            DropColumn("dbo.ExpandedUsers", "PhoneNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ExpandedUsers", "PhoneNumber", c => c.String());
            AddColumn("dbo.ExpandedUsers", "AccessFailedCount", c => c.Int(nullable: false));
            AddColumn("dbo.ExpandedUsers", "LockoutEndDate", c => c.DateTime());
        }
    }
}
