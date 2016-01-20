namespace TwitterCapture.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class db8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TwitterProfiles", "Location", c => c.String());
            AddColumn("dbo.TwitterProfiles", "Description", c => c.String());
            AddColumn("dbo.TwitterProfiles", "Protected", c => c.String());
            AddColumn("dbo.TwitterProfiles", "GeoEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.TwitterProfiles", "Verified", c => c.Boolean(nullable: false));
            AddColumn("dbo.TwitterProfiles", "Lang", c => c.Boolean(nullable: false));
            AddColumn("dbo.TwitterProfiles", "ContributorsEnabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TwitterProfiles", "ContributorsEnabled");
            DropColumn("dbo.TwitterProfiles", "Lang");
            DropColumn("dbo.TwitterProfiles", "Verified");
            DropColumn("dbo.TwitterProfiles", "GeoEnabled");
            DropColumn("dbo.TwitterProfiles", "Protected");
            DropColumn("dbo.TwitterProfiles", "Description");
            DropColumn("dbo.TwitterProfiles", "Location");
        }
    }
}
