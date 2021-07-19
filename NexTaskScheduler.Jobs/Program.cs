namespace NexTaskScheduler.Jobs
{
    using System.Collections.Generic;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.AspNetCore;
    using Fanex.Logging.Sentry;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NexTaskScheduler.Core;
    using NexTaskScheduler.Core.Models;
    using NexTaskScheduler.Jobs.Workers;
    using NLog.Web;
    using RestSharp;
    using Sentry;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

            Host.CreateDefaultBuilder(args)
                .UseNLog()
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    AddConfiguration(services, configuration);
                    AddDependencyInjection(services, configuration);
                    AddWorker(services);
                }).Build().Run();
        }

        private static void AddConfiguration(IServiceCollection services, IConfiguration configuration)
        {
            LogManager
            .SetDefaultLogCategory(configuration.GetSection("LogCategory").Value)
            .Use(new List<ILogEngine>
            {
                new SentryLogging(new SentryEngineOptions
                {
                    Dsn = new Dsn(configuration.GetSection("SentryEngineOptions:Dsn").Value)
                })
            });

            ExceptionHandler
                .UseConfig(new LoggingConfiguration());

            services.Configure<List<JobSetting>>(configuration.GetSection("JobSettings"));
            
        }

        private static void AddDependencyInjection(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<ILogServiceProvider, LogServiceProvider>();

            services.AddSingleton<IRestClient, RestClient>();
        }

        private static void AddWorker(IServiceCollection services)
        {
            services.AddHostedService<NewBioDeviceAlertWorker>();
            services.AddHostedService<BioTimeChangeAlertWorker>();
            services.AddHostedService<FingerprintAndCardVerifyAlertWorker>();
            services.AddHostedService<DeductLateInEarlyOutAlertWorker>();
            services.AddHostedService<LateInEarlyOutViolateOnMonthWarningWorker>();
            services.AddHostedService<RemindBirthdayAndFirstWorkingdayWorker>();
            services.AddHostedService<RemindMissingCheckInCheckOutWorker>();
            services.AddHostedService<RemindReachedWorkingTimeWorker>();
            services.AddHostedService<SendScheduledMessageWorker>();
            services.AddHostedService<ViolateDisciplineWarningWorker>();

            services.AddHostedService<ReSendErrorMessageWorker>();

            services.AddHostedService<NexGoalAutoJobWorker>();
            services.AddHostedService<LumiGoalAutoJobWorker>();
        }
    }
}
