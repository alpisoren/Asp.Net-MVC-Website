using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CorporateWebsite.Models.Model
{
    [Table("Iletisim")]
    public class Iletisim
    {
        [Key]
        public int IletisimId { get; set; }

        [StringLength(250,ErrorMessage ="250 Karakter olmalıdır")]
        [DisplayName("Adres")]
        public string Adres { get; set; }


        [StringLength(250, ErrorMessage = "250 Karakter olmalıdır")]
        [DisplayName("Telefon")]
        public string Telefon { get; set; }

        [DisplayName("E-posta")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string Eposta { get; set; }

        [DisplayName("WhatsApp")]

        public string Whatsapp { get; set; }

        public string Facebook { get; set; }

        [DisplayName("X (Twitter)")]

        public string Twitter { get; set; }

        public string Instagram { get; set; }



    }
}