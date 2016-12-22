namespace Mood.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovingOwnerUsernameProperty : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Surveys", name: "OwnerUserName", newName: "Owner_Id");
            RenameIndex(table: "dbo.Surveys", name: "IX_OwnerUserName", newName: "IX_Owner_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Surveys", name: "IX_Owner_Id", newName: "IX_OwnerUserName");
            RenameColumn(table: "dbo.Surveys", name: "Owner_Id", newName: "OwnerUserName");
        }
    }
}
