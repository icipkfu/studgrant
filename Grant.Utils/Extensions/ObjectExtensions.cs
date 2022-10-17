namespace Grant.Utils.Extensions
{
    using Newtonsoft.Json;

    /// <summary>
    /// Методы расширения для объектов типа object
    /// </summary>
    public static partial class ObjectExtensions
    {
        /// <summary>
        /// Преобразование объекта к типу int
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt(this object obj, int defaultValue = default (int))
        {
            if (obj == null)
            {
                return defaultValue;
            }

            if (obj is int)
            {
                return (int) obj;
            }

            int result;

            if (int.TryParse(obj.ToString(), out result))
            {
                return result;
            }

            return defaultValue;
        }

        /// <summary>
        /// Преобразование объекта к типу long
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long ToLong(this object obj, long defaultValue = default (long))
        {
            if (obj == null)
            {
                return defaultValue;
            }

            if (obj is long)
            {
                return (long)obj;
            }

            long result;

            if (long.TryParse(obj.ToString(), out result))
            {
                return result;
            }

            return defaultValue;
        }

        public static bool ToBool(this object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is bool)
            {
                return (bool) obj;
            }

            bool result;

            if (bool.TryParse(obj.ToString(), out result))
            {
                return result;
            }

            return false;
        }

        /// <summary>
        /// Преобразование объекта в строку json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}