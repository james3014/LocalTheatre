namespace LocalTheatre.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoleModelsUpdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExpandedUsers", "RoleName", c => c.String());
            AddColumn("dbo.ExpandedUsers", "IsSuspended", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExpandedUsers", "IsSuspended");
            DropColumn("dbo.ExpandedUsers", "RoleName");
        }
    }
}
