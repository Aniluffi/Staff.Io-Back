using System.Security.Cryptography;
using System.Text;

namespace StaffIo.Service.Common
{
    public static class StringHashExtensions
    {

        /// <summary>
        /// Вычисляет хеш SHA256 для заданной строки и возвращает его в виде шестнадцатеричной строки.
        /// </summary>
        /// <param name="source">Исходная строка для хеширования.</param>
        /// <returns>Хеш SHA256 в виде строки.</returns>
        public static string ToSha256Hash(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }

            // 1. Создаем экземпляр SHA256
            using (var sha256 = SHA256.Create())
            {
                // 2. Преобразуем исходную строку в массив байтов (UTF8)
                byte[] sourceBytes = Encoding.UTF8.GetBytes(source);

                // 3. Вычисляем хеш
                byte[] hashBytes = sha256.ComputeHash(sourceBytes);

                // 4. Преобразуем байты хеша в шестнадцатеричную строку (HEX)
                var builder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    // "x2" форматирует байт как двухзначное шестнадцатеричное число в нижнем регистре
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }

    }
}
