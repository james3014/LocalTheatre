namespace LocalTheatre.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdentityModelDbContext : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExpandedUsers",
                c => new
                    {
                        UserName = c.String(nullable: false, maxLength: 128),
                        Email = c.String(),
                        Password = c.String(),
                        LockoutEndDate = c.DateTime(),
                        AccessFailedCount = c.Int(nullable: false),
                        PhoneNumber = c.String(),
                    })
                .PrimaryKey(t => t.UserName);
            
            AddColumn("dbo.AspNetUsers", "FirstName", c => c.String(nullable: false));
            AddColumn("dbo.AspNetUsers", "Surname", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Surname");
            DropColumn("dbo.AspNetUsers", "FirstName");
            DropTable("dbo.ExpandedUsers");
        }
    }
}
