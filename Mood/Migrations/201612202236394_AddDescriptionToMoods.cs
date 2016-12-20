namespace Mood.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDescriptionToMoods : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Moods", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Moods", "Description");
        }
    }
}
