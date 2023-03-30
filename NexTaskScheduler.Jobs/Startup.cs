namespace NexTaskScheduler.Jobs
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NexTaskScheduler.Core;
    using NexTaskScheduler.Core.Models;
    using NexTaskScheduler.Jobs.Middleware;
    using NexTaskScheduler.Jobs.Workers;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            AddConfiguration(services, Configuration);
            AddDependencyInjection(services);
            services.AddHttpClient();

            AddWorker(services);
            AddLumiWorker(services);
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseMiddleware<ErrorHandler>();

            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Auth}/{action=Index}/{id?}");
            });
        }

        private static void AddConfiguration(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<List<JobSetting>>(configuration.GetSection("JobSettings"));
        }

        private static void AddDependencyInjection(IServiceCollection services)
        {
            services.AddSingleton<ILogServiceProvider, LogServiceProvider>();
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
            services.AddHostedService<ImportDataFromBioStarWorker>();
            services.AddHostedService<SendScheduledMessageWorker>();
            services.AddHostedService<SendScheduledVegetarianMessagesWorker>();
            services.AddHostedService<ViolateDisciplineWarningWorker>();
            services.AddHostedService<ReSendErrorMessageWorker>();
            services.AddHostedService<NexGoalAutoJobWorker>();
            services.AddHostedService<SkypeAccountAutomapWorker>();
            services.AddHostedService<LateInEarlyOutAutoDeductWorker>();
        }

        private static void AddLumiWorker(IServiceCollection services)
        {
            services.AddHostedService<LumiGoalAutoJobWorker>();
            services.AddHostedService<LumiFingerprintAndCardVerifyAlertWorker>();
            services.AddHostedService<LumiDeductLateInEarlyOutAlertWorker>();
            services.AddHostedService<LumiLateInEarlyOutViolateOnMonthWarningWorker>();
            services.AddHostedService<LumiRemindBirthdayAndFirstWorkingdayWorker>();
            services.AddHostedService<LumiRemindMissingCheckInCheckOutWorker>();
            services.AddHostedService<LumiRemindReachedWorkingTimeWorker>();
            services.AddHostedService<LumiImportDataFromBioStarWorker>();
            services.AddHostedService<LumiSendScheduledMessageWorker>();
            services.AddHostedService<LumiViolateDisciplineWarningWorker>();
            services.AddHostedService<LuminetSkypeAccountAutoMappingWorker>();
        }
    }
}