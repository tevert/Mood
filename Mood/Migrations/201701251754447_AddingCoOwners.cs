namespace Mood.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingCoOwners : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SurveyApplicationUsers",
                c => new
                    {
                        Survey_Id = c.Guid(nullable: false),
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Survey_Id, t.ApplicationUser_Id })
                .ForeignKey("dbo.Surveys", t => t.Survey_Id, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: false)
                .Index(t => t.Survey_Id)
                .Index(t => t.ApplicationUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SurveyApplicationUsers", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.SurveyApplicationUsers", "Survey_Id", "dbo.Surveys");
            DropIndex("dbo.SurveyApplicationUsers", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.SurveyApplicationUsers", new[] { "Survey_Id" });
            DropTable("dbo.SurveyApplicationUsers");
        }
    }
}
