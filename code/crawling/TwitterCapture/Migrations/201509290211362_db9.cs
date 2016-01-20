namespace TwitterCapture.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class db9 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TwitterProfiles", "Protected", c => c.Boolean(nullable: false, defaultValue:false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TwitterProfiles", "Protected", c => c.String());
        }
    }
}
