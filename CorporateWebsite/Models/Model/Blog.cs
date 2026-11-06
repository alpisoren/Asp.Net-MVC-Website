using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CorporateWebsite.Models.Model
{
    [Table("Blog")]
    public class Blog
    {

        [Key]
        public int BlogId { get; set; }

        [DisplayName("Başlık")]

        public string Baslik { get; set; }

        [DisplayName("İçerik")]

        public string Icerik { get; set; }

        [DisplayName("Resim")]

        public string ResimURL { get; set; }


        [DisplayName("Kategori")]


        public int? KategoriId { get; set; }
        public Kategori Kategori { get; set; }

        /// <summary>Sitede gösterilen tarih. İleri bir tarih verilirse yazı o ana kadar sitede görünmez (planlı yayın).</summary>
        [DisplayName("Yayın Tarihi")]
        public DateTime YayinTarihi { get; set; }

        [DisplayName("Son Güncelleme")]
        public DateTime? GuncellemeTarihi { get; set; }

        [DisplayName("Yazar")]
        [StringLength(100)]
        public string Yazar { get; set; }

        /// <summary>Virgülle ayrılmış etiketler ("seo, web tasarım"). Bkz. EtiketHelper.</summary>
        [DisplayName("Etiketler")]
        [StringLength(250)]
        public string Etiketler { get; set; }

        public ICollection<Yorum> Yorumlar { get; set; }

    }
}