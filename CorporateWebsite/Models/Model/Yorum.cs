using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CorporateWebsite.Models.Model
{
    [Table("Yorum")]
    public class Yorum
    {
        [Key]
        public int YorumId { get; set; }

        [Required, StringLength(50, ErrorMessage = "50 Karakter olmalıdır")]
        public string AdSoyad { get; set; }

        [DisplayName("Icerik")]
        public string Icerik { get; set; }

        public string Eposta { get; set; }

        public bool Onayli { get; set; }

        public int? BlogId { get; set; }
        public Blog Blog { get; set; }

    }

}