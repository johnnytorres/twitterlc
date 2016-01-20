namespace TwitterCapture.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class db3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TwitterTweets",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        ProfileId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        IsRetweet = c.Boolean(nullable: false),
                        FavouriteCount = c.Long(nullable: false),
                        RetweetCount = c.Long(nullable: false),
                        Text = c.String(),
                        Json = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TwitterProfiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TwitterTweets", "ProfileId", "dbo.TwitterProfiles");
            DropIndex("dbo.TwitterTweets", new[] { "ProfileId" });
            DropTable("dbo.TwitterTweets");
        }
    }
}
