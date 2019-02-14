namespace LocalTheatre.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommentModelAlteredNames : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Comments", "CommentBody", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Comments", "CommentBody", c => c.String(nullable: false));
        }
    }
}
