namespace NexTaskScheduler.Jobs.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using NexTaskScheduler.Core;

    public class ErrorHandler
    {
        private readonly RequestDelegate next;
        private readonly ILogServiceProvider logService;

        public ErrorHandler(RequestDelegate next, ILogServiceProvider logService)
        {
            this.next = next;
            this.logService = logService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await logService.WriteExceptionSentryErrorAsync(ex);
            }
        }
    }
}