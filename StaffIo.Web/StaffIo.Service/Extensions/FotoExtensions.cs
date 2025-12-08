using StaffIo.Service.Constans;

namespace StaffIo.Service.Extensions
{
    public static class FotoExtensions
    {
        public static string GetUrl(this string fotoId)
        {
            return string.Format(FotoConstans.BaseFotoUrl , fotoId);
        }
    }
}
