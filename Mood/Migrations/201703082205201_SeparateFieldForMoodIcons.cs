namespace Mood.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeparateFieldForMoodIcons : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Moods", "IconName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Moods", "IconName");
        }
    }
}
