namespace Grant.Utils.Extensions
{
    using System;

    public static partial class TypeExtensions
    {
        public static bool Is<T>(this Type type)
        {
            return type.Is(typeof (T));
        }

        /// <summary>
        /// Вычисление значения из переданного объекта.
        /// Если объект равен null или возникла ошибка выполнения (в Func), то возвращает null.
        /// Имеется подобный метод ReturnSafe для возврата ValueType.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="evaluator"></param>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static TResult With<TInput, TResult>(this TInput instance, Func<TInput, TResult> evaluator)
            where TResult : class
            where TInput : class
        {
            if (instance == null)
            {
                return null;
            }

            TResult result;
            try
            {
                result = evaluator(instance);
            }
            catch
            {
                result = null;
            }

            return result;
        }

        /// <summary>
        /// Выполняет проверку условия на экземляре.
        /// Возвращает  default(TInput), если проверка вернула false,
        /// иначе возвращается переданный экземляр.
        /// Если экземпляр равен null, то возвращает null.
        /// Если возникла ошибка выполнения (в Func), то метод выбросит исключение.     
        /// </summary>
        /// <param name="o">
        /// Объект.
        /// </param>
        /// <param name="evaluator">
        /// Делегат, представляющий проверяемое условие.
        /// </param>
        /// <typeparam name="TInput">
        /// Тип объекта.
        /// </typeparam>
        /// <returns>
        /// </returns>
        public static TInput If<TInput>(this TInput o, Func<TInput, bool> evaluator)
        {
            if (ReferenceEquals(null, o))
            {
                return default(TInput);
            }

            return evaluator(o) ? o : default(TInput);
        }
    }
}