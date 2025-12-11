using StaffIo.Service.Constans;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StaffIo.Service.Extensions
{
    public static class FotoExtensions
    {
        public static string GetUrl(this string fotoPatch)
        {
            return FotoConstans.BaseFotoUrl + "/" + fotoPatch + "?t=" + DateTime.Now;
        }
    }
}
