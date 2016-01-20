namespace TwitterCapture.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class db5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TwitterTweets", "RetweetersIds", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TwitterTweets", "RetweetersIds");
        }
    }
}
