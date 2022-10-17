namespace Grant.Core
{
    using System;

    class TimeProvider : IDateTimeProvider
    {
        public DateTime GetNowUtc()
        {
            return DateTime.UtcNow;
        }
    }
}
