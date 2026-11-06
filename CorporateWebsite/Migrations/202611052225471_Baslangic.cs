namespace CorporateWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Baslangic : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Admin",
                c => new
                    {
                        AdminId = c.Int(nullable: false, identity: true),
                        Eposta = c.String(nullable: false, maxLength: 50),
                        Sifre = c.String(nullable: false, maxLength: 100),
                        Yetki = c.String(),
                        SifreSifirlamaToken = c.String(maxLength: 64),
                        SifreSifirlamaSonTarih = c.DateTime(),
                    })
                .PrimaryKey(t => t.AdminId);
            
            CreateTable(
                "dbo.Blog",
                c => new
                    {
                        BlogId = c.Int(nullable: false, identity: true),
                        Baslik = c.String(),
                        Icerik = c.String(),
                        ResimURL = c.String(),
                        KategoriId = c.Int(),
                    })
                .PrimaryKey(t => t.BlogId)
                .ForeignKey("dbo.Kategori", t => t.KategoriId)
                .Index(t => t.KategoriId);
            
            CreateTable(
                "dbo.Kategori",
                c => new
                    {
                        KategoriId = c.Int(nullable: false, identity: true),
                        KategoriAd = c.String(nullable: false, maxLength: 50),
                        Aciklama = c.String(),
                    })
                .PrimaryKey(t => t.KategoriId);
            
            CreateTable(
                "dbo.Yorum",
                c => new
                    {
                        YorumId = c.Int(nullable: false, identity: true),
                        AdSoyad = c.String(nullable: false, maxLength: 50),
                        Icerik = c.String(),
                        Eposta = c.String(),
                        Onayli = c.Boolean(nullable: false),
                        BlogId = c.Int(),
                    })
                .PrimaryKey(t => t.YorumId)
                .ForeignKey("dbo.Blog", t => t.BlogId)
                .Index(t => t.BlogId);
            
            CreateTable(
                "dbo.Hakkimizda",
                c => new
                    {
                        HakkimizdaId = c.Int(nullable: false, identity: true),
                        Aciklama = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.HakkimizdaId);
            
            CreateTable(
                "dbo.Hizmet",
                c => new
                    {
                        HizmetId = c.Int(nullable: false, identity: true),
                        Baslik = c.String(nullable: false, maxLength: 150),
                        Aciklama = c.String(),
                        ResimUrl = c.String(),
                    })
                .PrimaryKey(t => t.HizmetId);
            
            CreateTable(
                "dbo.Iletisim",
                c => new
                    {
                        IletisimId = c.Int(nullable: false, identity: true),
                        Adres = c.String(maxLength: 250),
                        Telefon = c.String(maxLength: 250),
                        Fax = c.String(),
                        Whatsapp = c.String(),
                        Facebook = c.String(),
                        Twitter = c.String(),
                        Instagram = c.String(),
                    })
                .PrimaryKey(t => t.IletisimId);
            
            CreateTable(
                "dbo.Kimlik",
                c => new
                    {
                        Kimlikd = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 100),
                        Keywords = c.String(nullable: false, maxLength: 200),
                        Description = c.String(nullable: false, maxLength: 300),
                        LogoUrl = c.String(),
                        Unvan = c.String(),
                    })
                .PrimaryKey(t => t.Kimlikd);
            
            CreateTable(
                "dbo.Slider",
                c => new
                    {
                        SliderId = c.Int(nullable: false, identity: true),
                        Baslik = c.String(maxLength: 30),
                        Aciklama = c.String(maxLength: 150),
                        ResimURL = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.SliderId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Yorum", "BlogId", "dbo.Blog");
            DropForeignKey("dbo.Blog", "KategoriId", "dbo.Kategori");
            DropIndex("dbo.Yorum", new[] { "BlogId" });
            DropIndex("dbo.Blog", new[] { "KategoriId" });
            DropTable("dbo.Slider");
            DropTable("dbo.Kimlik");
            DropTable("dbo.Iletisim");
            DropTable("dbo.Hizmet");
            DropTable("dbo.Hakkimizda");
            DropTable("dbo.Yorum");
            DropTable("dbo.Kategori");
            DropTable("dbo.Blog");
            DropTable("dbo.Admin");
        }
    }
}
