namespace OwinConfiguration
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Grant.Core.Logging;
    using Microsoft.Owin;

    public class Logger : OwinMiddleware
	{
        private readonly ILogManager _logManager = new DefaultLogManager();

        public Logger(OwinMiddleware next)
            : base(next)
		{
		}

		public override async Task Invoke(IOwinContext context)
		{
            context.Set("reqGuid", Guid.NewGuid().ToString());

            _logManager.AuditRequest(context);

			var stopWatch = Stopwatch.StartNew();

		    try
		    {
                await Next.Invoke(context).ContinueWith(appTask =>
		        {
                    _logManager.AuditResponse(context, stopWatch.ElapsedMilliseconds);

                    if (appTask.IsFaulted && appTask.Exception != null)
                    {
                        Handle(appTask.Exception, context);
                    }

		            var ex = context.GetSwallowedException();
		            if (ex != null)
		            {
                        Handle(ex, context);
		            }

		            return appTask;
		        });
		    }
		    catch (Exception e)
		    {
                Handle(e, context);
		        throw;
		    }
		}

        void Handle(Exception e, IOwinContext context)
        {
            _logManager.Error("Необработанное исключение", e);
            context.Set("handledException", true);
        }
	}
}
