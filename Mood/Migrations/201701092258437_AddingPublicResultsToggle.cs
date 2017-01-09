namespace Mood.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingPublicResultsToggle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Surveys", "PublicResults", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Surveys", "PublicResults");
        }
    }
}
