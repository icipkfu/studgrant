namespace Grant.Core.Logging
{
    using System;
    using System.IO;
    using System.Text;
    using Context;
    using System.Web;
    using Microsoft.Owin;

    public class DefaultLogManager : ILogManager
    {
        //public DefaultLogManager(IDateTimeProvider dateTimeProvider)
        //{
        //    timeProvider = dateTimeProvider;
        //}

        private const string LogDirectory = ".logs";
        private const string InfoLogFile = "info.log";
        private const string DebugLogFile = "debug.log";
        private const string ErrorLogFile = "error.log";
       // private const string AuditFile = "audit.log";

      ///  private readonly IDateTimeProvider timeProvider;


        public void AuditResponse(IOwinContext context, long ms)
        {
          var message = MakeAuditResponseRecord(context, ms);
          var guid = context.Get<string>("reqGuid");

         // AppendMessage(AuditFile, FormatMessage(message, guid));
        }

        public void AuditRequest(IOwinContext context)
        {
          var message = MakeAuditRequestRecord(context);
          var guid = context.Get<string>("reqGuid");

        //  AppendMessage(AuditFile, FormatMessage(message, guid));
        }

        private string MakeAuditResponseRecord(IOwinContext context, long ms)
        {
            var method = context.Get<string>(OwinConstants.RequestMethod);
            var path = context.Get<string>(OwinConstants.RequestPath);
            var guid = context.Get<string>("reqGuid");

            var statusCode = context.Get<int>(OwinConstants.ResponseStatusCode);

            if (statusCode == 500)
            {
                if (!context.Environment.ContainsKey("handledException"))
                {
                    Error("Необработанное исключение", guid: guid);
                }
            }

            return  String.Format("Ответ\t{0}\t{1}\t{2}\t{3}ms\t{4}", method, path, ms,
                      context.Get<int>(OwinConstants.ResponseStatusCode),
                      context.Get<string>(OwinConstants.ResponseReasonPhrase));
        }

        private string MakeAuditRequestRecord(IOwinContext context)
        {
            var method = context.Get<string>(OwinConstants.RequestMethod);
            var path = context.Get<string>(OwinConstants.RequestPath);

            string requestBody;
            var stream = context.Get<Stream>(OwinConstants.RequestBody);

            using (var sr = new StreamReader(stream))
            {
                requestBody = sr.ReadToEnd();
            }
            context.Set(OwinConstants.RequestBody, new MemoryStream(Encoding.UTF8.GetBytes(requestBody)));

            return String.Format("Вызов\t{0}\t{1}\t{2}", method, path, requestBody);
        }

        public void Info(string message)
        {
            AppendMessage(InfoLogFile, FormatMessage(message));
        }

        public void InfoFormat(string format, params object[] args)
        {
            Info(string.Format(format, args));
        }

        public void Debug(string message)
        {
            AppendMessage(DebugLogFile, FormatMessage(message));
        }

        public void DebugFormat(string format, params object[] args)
        {
            Debug(string.Format(format, args));
        }

        public void Error(string message, Exception e = null, string guid = null)
        {
            if (e == null)
            {
                AppendMessage(ErrorLogFile, FormatMessage(message, guid));
                return;
            }

            var exc = e;

            var sb = new StringBuilder();

            while (exc != null)
            {
                if (sb.Length > 0)
                {
                    sb.Append("\r\nInnerException:");
                }

                sb.AppendFormat("{0}\r\n{1}", exc.Message, exc.StackTrace);
                exc = exc.InnerException;
            }

            AppendMessage(ErrorLogFile, FormatMessage(sb.ToString(), guid));
        }

        private string FormatMessage(string message, string guid = null)
        {
            if (guid == null)
            {
                if (HttpContext.Current != null && HttpContext.Current.Items.Contains("Owin.Environment"))
                {
                    guid = HttpContext.Current.GetOwinContext().Get<string>("reqGuid");
                }
                else
                {
                    guid = "                                    "; 
                }
            }

            return string.Format("{0} {1} {2}", DateTime.UtcNow, guid, message);
        }

        private static void AppendMessage(string filePath, string message)
        {
            var directory = ApplicationContext.Current.MapPath(LogDirectory);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var file = Path.Combine(directory, filePath);

            try
            {
                using (var sw = new StreamWriter(file, true))
                {
                    sw.WriteLine(message);
                }
            }
            catch (Exception)
            {
                
               // todo
            }
        }
    }
}