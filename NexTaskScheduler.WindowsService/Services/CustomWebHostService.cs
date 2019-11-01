namespace NexTaskScheduler.WindowsService.Services
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using Hangfire;
    using Hangfire.Storage;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.WindowsServices;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NexTaskScheduler.AutoJobs.Interfaces;

    [DesignerCategory("Code")]
    public class CustomWebHostService : WebHostService
    {
        private readonly ILogger logger;
        private readonly IAutoJobInitial autoJobs;

        public CustomWebHostService(IWebHost host) : base(host)
        {
            logger = host.Services.GetService<ILogger<CustomWebHostService>>();
            autoJobs = host.Services.CreateScope().ServiceProvider.GetService<IAutoJobInitial>();
        }

        protected override void OnStarted()
        {
            logger.LogInformation(string.Format("Service started at {0}", DateTime.Now));
            InitAutoJobs();
            base.OnStarted();
        }

        protected override void OnStopping()
        {
            logger.LogInformation(string.Format("Servie stop at {0}", DateTime.Now));
            RemoveCurrentHangfireServer();
            base.OnStopping();
        }

        public void InitAutoJobs()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            autoJobs.Initialize();
        }

        private static void RemoveCurrentHangfireServer()
        {
            IMonitoringApi monitoringApi = JobStorage.Current.GetMonitoringApi();
            var serverToRemove = monitoringApi.Servers().FirstOrDefault(s => s.Name.ToLowerInvariant().Contains(Environment.MachineName.ToLowerInvariant()));
            if (serverToRemove != null)
            {
                JobStorage.Current.GetConnection().RemoveServer(serverToRemove?.Name);
            }
        }
    }
}