namespace Mood.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingTextFieldToAnswers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Answers", "Details", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Answers", "Details");
        }
    }
}
