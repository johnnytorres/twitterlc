namespace TwitterCapture.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class db12 : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.TwitterProfiles", "IsLeader", c => c.Boolean(nullable: false));
            //DropColumn("dbo.TwitterProfiles", "Json");
            //DropColumn("dbo.TwitterProfiles", "HasMoreHistoricalTweets");
        }
        
        public override void Down()
        {
            //AddColumn("dbo.TwitterProfiles", "HasMoreHistoricalTweets", c => c.Boolean(nullable: false));
            //AddColumn("dbo.TwitterProfiles", "Json", c => c.String());
            //DropColumn("dbo.TwitterProfiles", "IsLeader");
        }
    }
}
