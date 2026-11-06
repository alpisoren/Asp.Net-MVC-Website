using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CorporateWebsite.Models.Model
{
    [Table("Yorum")]
    public class Yorum
    {
        [Key]
        public int YorumId { get; set; }

        [Required, StringLength(50, ErrorMessage = "50 Karakter olmalıdır")]
        [DisplayName("Ad Soyad")]
        public string AdSoyad { get; set; }

        [DisplayName("Yorum")]
        public string Icerik { get; set; }

        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [DisplayName("E-posta")]
        public string Eposta { get; set; }

        [DisplayName("Onaylı")]

        public bool Onayli { get; set; }

        [DisplayName("Yazı")]

        public int? BlogId { get; set; }
        public Blog Blog { get; set; }

    }

}