namespace TwitterCapture.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class db13 : DbMigration
    {
        public override void Up()
        {
            //RenameColumn(table: "dbo.TwitterProfiles", name: "IsLeader", newName: "Yleader");
        }
        
        public override void Down()
        {
            //RenameColumn(table: "dbo.TwitterProfiles", name: "Yleader", newName: "IsLeader");
        }
    }
}
