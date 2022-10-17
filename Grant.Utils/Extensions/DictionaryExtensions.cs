namespace Grant.Utils.Extensions
{
    using System.Collections.Generic;

    /// <summary>
    /// Методы расширения для объектов типа IDictionary
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Получить значение словаря по ключу, в случае отсутствия ключа вернет default
        /// </summary>
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue defaultValue = default (TValue))
        {
            ArgumentChecker.NotNull(source, "source");

            if (source.ContainsKey(key))
            {
                return source[key];
            }

            return defaultValue;
        }

        /// <summary>
        /// Объединение словарей, в случае одинаковых ключей значение будет перезаписано
        /// </summary>
        public static void Apply<TKey, TValue>(this IDictionary<TKey, TValue> destination, IDictionary<TKey, TValue> source)
        {
            foreach (var key in source.Keys)
            {
                destination[key] = source[key];
            }
        }

        /// <summary>
        /// Объединение словарей, в случае одинаковых ключей значение не будет изменено
        /// </summary>
        public static void ApplyIf<TKey, TValue>(this IDictionary<TKey, TValue> destination, IDictionary<TKey, TValue> source)
        {
            foreach (var key in source.Keys)
            {
                if (!destination.ContainsKey(key))
                {
                    destination[key] = source[key];
                }
            }
        }

        /// <summary>
        /// Объединение словарей, в случае одинаковых ключей значение будет перезаписано
        /// </summary>
        public static void Apply<TKey, TValue1, TValue2>(this IDictionary<TKey, TValue1> destination, IDictionary<TKey, TValue2> source) where TValue2 : TValue1
        {
            foreach (var key in source.Keys)
            {
                destination[key] = source[key];
            }
        }

        /// <summary>
        /// Объединение словарей, в случае одинаковых ключей значение не будет изменено
        /// </summary>
        public static void ApplyIf<TKey, TValue1, TValue2>(this IDictionary<TKey, TValue1> destination, IDictionary<TKey, TValue2> source) where TValue2 : TValue1
        {
            foreach (var key in source.Keys)
            {
                if (!destination.ContainsKey(key))
                {
                    destination[key] = source[key];
                }
            }
        }
    }
}