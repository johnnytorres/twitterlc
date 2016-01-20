namespace TwitterCapture.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class db11 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.TwitterTweets", "IsRetweet");
           // DropColumn("dbo.TwitterTweets", "RetweetersIds");
           // DropColumn("dbo.TwitterTweets", "Json");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TwitterTweets", "Json", c => c.String());
            AddColumn("dbo.TwitterTweets", "RetweetersIds", c => c.String());
            AddColumn("dbo.TwitterTweets", "IsRetweet", c => c.Boolean(nullable: false));
        }
    }
}
