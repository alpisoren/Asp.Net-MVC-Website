namespace CorporateWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BlogAlanlari : DbMigration
    {
        public override void Up()
        {
            // Var olan yazılar, migration'ın çalıştığı anın tarihini alır (önceden tarih tutulmuyordu).
            AddColumn("dbo.Blog", "YayinTarihi", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("dbo.Blog", "GuncellemeTarihi", c => c.DateTime());
            AddColumn("dbo.Blog", "Yazar", c => c.String(maxLength: 100));
            AddColumn("dbo.Blog", "Etiketler", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Blog", "Etiketler");
            DropColumn("dbo.Blog", "Yazar");
            DropColumn("dbo.Blog", "GuncellemeTarihi");
            DropColumn("dbo.Blog", "YayinTarihi");
        }
    }
}
