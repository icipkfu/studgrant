using Grant.Utils.Extensions;

namespace Grant.Utils
{
    using System;
    using Exceptions;

    public static class ArgumentChecker
    {
        /// <summary>
        /// Проверка на null
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="paramName"></param>
        public static void NotNull(object obj, string paramName)
        {
            if (obj == null)
            {
                throw new ArgumentEmptyException(paramName);
            }
        }

        /// <summary>
        /// проверка на null
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="paramName"></param>
        public static void Null(object obj, string paramName)
        {
            if (obj != null)
            {
                throw new ArgumentEmptyException(paramName);
            }
        }

        /// <summary>
        /// Проверка строкового представления объекта на пустоту
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="paramName"></param>
        public static void NotEmpty(object obj, string paramName)
        {
            NotNull(obj, paramName);

            if (obj.ToString().IsEmpty())
            {
                throw new ArgumentEmptyException(paramName);
            }
        }

        public static void AreEquals(object arg1, object arg2, string paramName)
        {
            if (arg1 == null && arg2 == null)
            {
                return;
            }

            if (arg1 == null || arg2 == null)
            {
                throw new ArgumentException(paramName);
            }

            if (ReferenceEquals(arg1, arg2) && Equals(arg1, arg2))
            {
                return;
            }

            throw new ArgumentException(paramName);
        }
    }
}