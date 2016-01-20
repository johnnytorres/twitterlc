namespace TwitterCapture.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class db10 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TwitterProfiles", "Lang", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TwitterProfiles", "Lang", c => c.Boolean(nullable: false));
        }
    }
}
