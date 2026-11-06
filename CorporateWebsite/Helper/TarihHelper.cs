using System.Globalization;

namespace CorporateWebsite.Helper
{
    public static class TarihHelper
    {
        /// <summary>Tarihleri "6 Kasım 2026" biçiminde yazmak için: tarih.ToString("d MMMM yyyy", TarihHelper.Tr)</summary>
        public static readonly CultureInfo Tr = CultureInfo.GetCultureInfo("tr-TR");
    }
}
