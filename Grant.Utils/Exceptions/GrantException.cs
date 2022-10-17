namespace Grant.Utils.Exceptions
{
    using System;

    public class GrantException : Exception
    {
        public GrantException()
        {
            
        }

        public GrantException(string message)
            : base(message)
        {
        }

        public GrantException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public GrantException(int errorCode, string message)
            : base(message)
        {
            this.ErrorCode = errorCode;
        }

        public GrantException(int errorCode, string message, Exception innerException)
            : base(message, innerException)
        {
            this.ErrorCode = errorCode;
        }

        public int? ErrorCode { get; protected set; }
    }
}