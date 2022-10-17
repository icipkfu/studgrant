namespace Grant.Utils.Extensions
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json;

    /// <summary>
    /// Методы расширения для объектов типа string
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Обрезанка строки до указанной длины.
        /// В случае если входящая строка равна null, то вернется пустая строка
        /// </summary>
        /// <param name="source"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Cut(this string source, int length)
        {
            if (source.IsEmpty())
            {
                return "";
            }

            if (source.Length <= length)
            {
                return source;
            }

            return source.Substring(0, length);
        }

        /// <summary>
        /// Проверка строки на пустоту
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// Преобразование строки в массив байт с указанной кодировкой (по умолчанию - UTF8)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this string str, Encoding encoding = null)
        {
            if (IsEmpty(str))
            {
                return new byte[0];
            }

            return (encoding ?? Encoding.UTF8).GetBytes(str);
        }

        /// <summary>
        /// Проверка строки на соответствие паттерну
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool IsMatch(this string str, string pattern)
        {
            ArgumentChecker.NotEmpty(pattern, "pattern");

            if (str == null) return false;

            return new Regex(pattern).IsMatch(str);
        }

        /// <summary>
        /// Преобразование объекта из json в объект указанного типа
        /// </summary>
        /// <param name="json"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object Deserialize(this string json, Type targetType)
        {
            return JsonConvert.DeserializeObject(json, targetType);
        }

        /// <summary>
        /// Преобразование объекта из json в объект указанного типа
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this string json)
        {
            return (T) Deserialize(json, typeof (T));
        }

        public static Guid ToGuid(this string str)
        {
            ArgumentChecker.NotEmpty(str, "str");

            Guid guid;

            if (Guid.TryParse(str, out guid))
            {
                return guid;
            }

            return Guid.Empty;
        }

        public static string GetMd5(this string str)
        {
            return GetBytes(str)
                .GetMd5()
                .GetString();
        }
    }
}