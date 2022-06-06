namespace NexTaskScheduler.Jobs
{
    using System;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using NLog.Web;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Info("Application start.");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Application terminated unexpectedly.");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls($"http://*:5000")
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog();

            services.AddHostedService<LumiFingerprintAndCardVerifyAlertWorker>();
            services.AddHostedService<LumiDeductLateInEarlyOutAlertWorker>();
            services.AddHostedService<LumiLateInEarlyOutViolateOnMonthWarningWorker>();
            services.AddHostedService<LumiRemindBirthdayAndFirstWorkingdayWorker>();
            services.AddHostedService<LumiRemindMissingCheckInCheckOutWorker>();
            services.AddHostedService<LumiRemindReachedWorkingTimeWorker>();
            services.AddHostedService<LumiSendScheduledMessageWorker>();
            services.AddHostedService<LumiViolateDisciplineWarningWorker>();
            
    }
}