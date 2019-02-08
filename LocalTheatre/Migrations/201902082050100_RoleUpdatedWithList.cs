namespace LocalTheatre.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoleUpdatedWithList : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ExpandedUsers", "RoleName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ExpandedUsers", "RoleName", c => c.String());
        }
    }
}
