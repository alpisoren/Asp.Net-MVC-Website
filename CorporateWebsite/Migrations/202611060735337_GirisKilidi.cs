namespace CorporateWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GirisKilidi : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Admin", "HataliGirisSayisi", c => c.Int(nullable: false));
            AddColumn("dbo.Admin", "KilitBitis", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Admin", "KilitBitis");
            DropColumn("dbo.Admin", "HataliGirisSayisi");
        }
    }
}
