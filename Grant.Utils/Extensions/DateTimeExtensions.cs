namespace Grant.Utils.Extensions
{
    using System;

    public static class DateTimeExtensions
    {
        public static long ToUnixTime(this DateTime dt)
        {
            if (dt.Kind == DateTimeKind.Local)
            {
                dt = dt.ToUniversalTime();
            }
            else if (dt.Kind == DateTimeKind.Unspecified)
            {
                dt = new DateTime(
                    dt.Year,
                    dt.Month,
                    dt.Day,
                    dt.Hour,
                    dt.Minute,
                    dt.Second,
                    dt.Millisecond,
                    DateTimeKind.Utc);
            }

            return (long)((dt - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
        }
    }
}