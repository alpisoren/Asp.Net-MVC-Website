namespace CorporateWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IletisimEposta : DbMigration
    {
        public override void Up()
        {
            // E-posta adresi tarihsel olarak "Fax" kolonunda tutuluyordu. EF iskeleti ekle+sil üretir (veri kaybolur);
            // bunun yerine kolon yeniden adlandırılır, mevcut adres korunur.
            RenameColumn("dbo.Iletisim", "Fax", "Eposta");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.Iletisim", "Eposta", "Fax");
        }
    }
}
