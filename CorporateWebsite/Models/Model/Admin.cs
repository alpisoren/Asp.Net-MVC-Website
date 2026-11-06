using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CorporateWebsite.Models.Model
{
    [Table("Admin")]
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }

        [Required, StringLength(50, ErrorMessage = "50 Karakter olmalıdır")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string Eposta { get; set; }

        // PBKDF2 hash'i 68 karakterdir; eski 50 sınırı yetmez.
        [Required, StringLength(100)]
        public string Sifre { get; set; }

        public string Yetki { get; set; }

        // Şifre sıfırlama: token'ın kendisi değil SHA-256 özeti saklanır.
        [StringLength(64)]
        public string SifreSifirlamaToken { get; set; }

        public DateTime? SifreSifirlamaSonTarih { get; set; }

        // Hesap kilidi veritabanında tutulur: uygulama yeniden başlasa ya da birden fazla sunucu olsa da geçerlidir.
        /// <summary>Son başarılı girişten bu yana art arda hatalı şifre sayısı.</summary>
        public int HataliGirisSayisi { get; set; }

        /// <summary>Doluysa bu ana (UTC) kadar doğru şifreyle bile giriş yapılamaz.</summary>
        public DateTime? KilitBitis { get; set; }
    }
}
