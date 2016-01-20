namespace TwitterCapture.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class db : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TwitterProfiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ScreenName = c.String(),
                        Name = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        StatusesCount = c.Long(nullable: false),
                        FavouritesCount = c.Long(nullable: false),
                        FollowersCount = c.Long(nullable: false),
                        FriendsCount = c.Long(nullable: false),
                        ListedCount = c.Long(nullable: false),
                        Json = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TwitterProfiles");
        }
    }
}
