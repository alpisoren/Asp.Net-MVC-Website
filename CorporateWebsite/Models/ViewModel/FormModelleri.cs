using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CorporateWebsite.Models.ViewModel
{
    public static class Yetkiler
    {
        public const string Admin = "Admin";
        public const string Editor = "Editor";

        public static readonly IReadOnlyList<string> Tumu = new[] { Admin, Editor };

        public static bool Gecerli(string yetki)
        {
            foreach (var y in Tumu)
            {
                if (y == yetki)
                {
                    return true;
                }
            }
            return false;
        }

        public static SelectList SecimListesi(string secili)
        {
            return new SelectList(new[]
            {
                new { Deger = Admin, Ad = "Admin (tüm yetkiler)" },
                new { Deger = Editor, Ad = "Editör (blog, hizmet, kategori, yorum)" }
            }, "Deger", "Ad", secili);
        }
    }

    /// <summary>Yönetici ekleme/güncelleme formu. Sifre güncellemede boş bırakılabilir.</summary>
    public class YoneticiFormModeli
    {
        public int AdminId { get; set; }

        [DisplayName("E-posta")]
        [Required(ErrorMessage = "E-posta zorunludur")]
        [StringLength(50, ErrorMessage = "En fazla 50 karakter olabilir")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string Eposta { get; set; }

        [DisplayName("Şifre")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Şifre 8-100 karakter olmalıdır")]
        public string Sifre { get; set; }

        [DisplayName("Yetki")]
        [Required(ErrorMessage = "Yetki seçiniz")]
        public string Yetki { get; set; }
    }

    // Dosya input'u da "ResimURL" adını taşıdığı için bu alan model bağlamaya dahil edilmez.
    [Bind(Exclude = "ResimURL")]
    public class BlogFormModeli
    {
        public int BlogId { get; set; }

        [DisplayName("Başlık")]
        [Required(ErrorMessage = "Başlık zorunludur")]
        [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir")]
        public string Baslik { get; set; }

        [DisplayName("İçerik")]
        [AllowHtml]
        public string Icerik { get; set; }

        [DisplayName("Kategori")]
        public int? KategoriId { get; set; }

        /// <summary>Mevcut resmin adresi (sadece gösterim için; formdan bağlanmaz).</summary>
        [DisplayName("Resim")]
        public string ResimURL { get; set; }

        [DisplayName("Yayın Tarihi")]
        [Required(ErrorMessage = "Yayın tarihi zorunludur")]
        [DataType(DataType.Date)]
        public DateTime? YayinTarihi { get; set; }

        [DisplayName("Yazar")]
        [StringLength(100, ErrorMessage = "Yazar en fazla 100 karakter olabilir")]
        public string Yazar { get; set; }

        [DisplayName("Etiketler")]
        [StringLength(250, ErrorMessage = "Etiketler toplamda en fazla 250 karakter olabilir")]
        public string Etiketler { get; set; }
    }

    [Bind(Exclude = "ResimUrl")]
    public class HizmetFormModeli
    {
        public int HizmetId { get; set; }

        [DisplayName("Hizmet Başlık")]
        [Required(ErrorMessage = "Başlık zorunludur")]
        [StringLength(150, ErrorMessage = "150 Karakter olmalıdır")]
        public string Baslik { get; set; }

        [DisplayName("Hizmet Açıklama")]
        [AllowHtml]
        public string Aciklama { get; set; }

        /// <summary>Mevcut resmin adresi (sadece gösterim için; formdan bağlanmaz).</summary>
        [DisplayName("Hizmet Resim")]
        public string ResimUrl { get; set; }
    }
}
