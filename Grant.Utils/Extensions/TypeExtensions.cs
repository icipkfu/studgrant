namespace Grant.Utils.Extensions
{
    using System;

    /// <summary>
    /// Методы расширения для объектов типа Type
    /// </summary>
    public static partial class TypeExtensions
    {
        public static bool IsNullable(this Type type)
        {
            return typeof (Nullable<>).IsAssignableFrom(type);
        }

        public static bool Is(this Type type, Type assignableType)
        {
            return type == assignableType || assignableType.IsAssignableFrom(type);
        }

        public static int InheritanceLevel(this Type type)
        {
            if (type == null)
            {
                return Int32.MaxValue;
            }

            return type.BaseType == null ? 0 : type.BaseType.InheritanceLevel() + 1;
        }
    }
}