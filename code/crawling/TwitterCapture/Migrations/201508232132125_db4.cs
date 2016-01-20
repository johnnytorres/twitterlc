namespace TwitterCapture.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class db4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TwitterProfiles", "HasMoreHistoricalTweets", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TwitterProfiles", "HasMoreHistoricalTweets");
        }
    }
}
