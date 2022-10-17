namespace OwinConfiguration
{
    using System;
    using Microsoft.Owin;

    public static class ExceptionProvider
    {
        public static Exception GetSwallowedException(this IOwinContext context)
        {
            object exception = null;
            context.Environment.TryGetValue("webapi.exception", out exception);
            return (Exception)exception;
        }
    }
}
