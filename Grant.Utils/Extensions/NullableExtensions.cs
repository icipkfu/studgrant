namespace Grant.Utils.Extensions
{
    using System;

    public static class NullableExtensions
    {
        public static string ToDateString(this DateTime? date, string format = "d")
        {
            return date.HasValue ? date.Value.ToShortDateString() : "";
        }
    }
}