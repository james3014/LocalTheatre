namespace LocalTheatre.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedCommentsModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "CommentTitle", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Comments", "CommentTitle");
        }
    }
}
