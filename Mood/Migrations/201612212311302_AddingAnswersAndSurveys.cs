namespace Mood.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingAnswersAndSurveys : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Answers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Time = c.DateTime(),
                        SurveyId = c.Guid(),
                        MoodId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Moods", t => t.MoodId)
                .ForeignKey("dbo.Surveys", t => t.SurveyId)
                .Index(t => t.SurveyId)
                .Index(t => t.MoodId);
            
            CreateTable(
                "dbo.Surveys",
                c => new
                    {
                        Id = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        OwnerUserName = c.String(nullable: false, maxLength: 128),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerUserName, cascadeDelete: true)
                .Index(t => t.OwnerUserName);
            
            DropColumn("dbo.Moods", "Count");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Moods", "Count", c => c.Int(nullable: false));
            DropForeignKey("dbo.Answers", "SurveyId", "dbo.Surveys");
            DropForeignKey("dbo.Surveys", "OwnerUserName", "dbo.AspNetUsers");
            DropForeignKey("dbo.Answers", "MoodId", "dbo.Moods");
            DropIndex("dbo.Surveys", new[] { "OwnerUserName" });
            DropIndex("dbo.Answers", new[] { "MoodId" });
            DropIndex("dbo.Answers", new[] { "SurveyId" });
            DropTable("dbo.Surveys");
            DropTable("dbo.Answers");
        }
    }
}
