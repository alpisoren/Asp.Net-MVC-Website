using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace CorporateWebsite.Services
{
    /// <summary>Servis işlemlerinin sonucu: başarılı mı, değilse hangi alanda hangi hata var.</summary>
    public class IslemSonucu
    {
        private readonly List<KeyValuePair<string, string>> hatalar = new List<KeyValuePair<string, string>>();

        public bool Basarili
        {
            get { return hatalar.Count == 0; }
        }

        public IReadOnlyList<KeyValuePair<string, string>> Hatalar
        {
            get { return hatalar; }
        }

        /// <summary>alan boşsa ("") hata forma genel olarak gösterilir.</summary>
        public IslemSonucu HataEkle(string alan, string mesaj)
        {
            hatalar.Add(new KeyValuePair<string, string>(alan ?? "", mesaj));
            return this;
        }

        public void ModelStateAktar(ModelStateDictionary modelState)
        {
            foreach (var h in hatalar)
            {
                modelState.AddModelError(h.Key, h.Value);
            }
        }

        public static IslemSonucu Tamam()
        {
            return new IslemSonucu();
        }

        public static IslemSonucu Hata(string alan, string mesaj)
        {
            return new IslemSonucu().HataEkle(alan, mesaj);
        }
    }
}
