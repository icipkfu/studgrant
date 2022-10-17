namespace Grant.Core.Logging
{
    using System;
    using Microsoft.Owin;

    public interface ILogManager
    {

        void AuditResponse(IOwinContext context, long ms);

        void AuditRequest(IOwinContext context);

        void Info(string message);

        void InfoFormat(string format, params object[] args);

        void Debug(string message);

        void DebugFormat(string format, params object[] args);

        void Error(string message, Exception e = null, string guid = null);
    }
}