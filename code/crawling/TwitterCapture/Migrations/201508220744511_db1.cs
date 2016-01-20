namespace TwitterCapture.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class db1 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.TwitterProfiles");
            AlterColumn("dbo.TwitterProfiles", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.TwitterProfiles", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.TwitterProfiles");
            AlterColumn("dbo.TwitterProfiles", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.TwitterProfiles", "Id");
        }
    }
}
