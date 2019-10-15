namespace NexTaskScheduler.WindowsService.Extensions
{
    using System.ServiceProcess;
    using Microsoft.AspNetCore.Hosting;
    using NexTaskScheduler.WindowsService.Services;

    public static class WebHostExtension
    {
        public static void RunAsCustomService(this IWebHost host, bool isService)
        {
            var webHostService = new CustomWebHostService(host);

            if (isService)
            {
                ServiceBase.Run(webHostService);
            }
            else
            {
                webHostService.InitAutoJobs();
                host.Run();
            }
        }
    }
}