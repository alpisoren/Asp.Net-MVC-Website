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

        public ICollection<Yorum> Yorumlar { get; set; }

    }
}