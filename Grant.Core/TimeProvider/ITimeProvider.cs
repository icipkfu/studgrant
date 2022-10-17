using System;

namespace Grant.Core
{
    public interface IDateTimeProvider
    {
        DateTime GetNowUtc();
    }
}
