namespace Grant.Utils.Extensions
{
    using System;
    using System.Text;

    /// <summary>
    /// Методы расширения для объектов типа byte[]
    /// </summary>
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Преобразование массива байт в строку с указанной кодировкой (по умолчанию - UTF8)
        /// </summary>
        /// <param name="array"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string GetString(this byte[] array, Encoding encoding = null)
        {
            return (encoding ?? Encoding.UTF8).GetString(array ?? new byte[0]);
        }

        /// <summary>
        /// Преобразовать массив байт в строку Base64
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string GetBase64String(this byte[] array)
        {
            return Convert.ToBase64String(array);
        }

        /// <summary>
        /// Получить Md5-хеш массива байт
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static byte[] GetMd5(this byte[] array)
        {
            return System.Security.Cryptography.MD5.Create().ComputeHash(array);
        }

        /// <summary>
        /// Получить hex-представление байтового массива
        /// </summary>
        /// <param name="array"></param>
        /// <param name="upperCase"></param>
        /// <returns></returns>
        public static string ToHex(this byte[] array, bool upperCase = false)
        {
            var c = new char[array.Length * 2];
            var a = upperCase ? 'A' : 'a';
            for (int bx = 0, cx = 0; bx < array.Length; ++bx, ++cx)
            {
                var b = ((byte)(array[bx] >> 4));
                c[cx] = (char)(b > 9 ? b - 10 + a : b + '0');

                b = ((byte)(array[bx] & 0x0F));
                c[++cx] = (char)(b > 9 ? b - 10 + a : b + '0');
            }

            return new string(c);
        }
    }
}