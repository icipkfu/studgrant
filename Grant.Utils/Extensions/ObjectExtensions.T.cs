namespace Grant.Utils.Extensions
{
    using System;
    using System.Globalization;

    public static partial class ObjectExtensions
    {
        public static TOut Return<TIn, TOut>(this TIn obj, Func<TIn, TOut> selector, TOut defaultValue = default (TOut))
        {
            var t = typeof (TIn);

            if (t.IsValueType && !t.IsNullable())
            {
                return selector(obj);
            }

            return (object) obj == null ? defaultValue : selector(obj);
        }

        /// <summary>
        /// Преобразование объекта к указанному типу
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T To<T>(this object obj)
        {
            return (T)Convert.ChangeType(obj, typeof(T));
        }

        /// <summary>
        /// Приведение объекта к указанному типу
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T As<T>(this object obj) where T : class
        {
            return obj as T;
        }

        /// <summary>
        /// Конвертация значения в строку инвариантной культуры с указанием формата. 
        /// Если формат указан в виде "C2", то он преобразуется в "{0:C2}". 
        /// Формат в виде "{0:C2}" допустим, формат в виде "0:C2" недопустим. 
        /// </summary>
        /// <param name="instance">Экземляр, который необходимо перевести в строку.</param>
        /// <param name="format">Формат.</param>
        /// <returns></returns>
        public static string ToInvariantString<T>(this T instance, string format = null)
        {
            return string.Format(CultureInfo.InvariantCulture,
                format.If(x => !x.IsEmpty())
                    .Return(x => x.Contains("{") ? x : "{0:" + format + "}", "{0}"),
                instance);
        }
    }
}