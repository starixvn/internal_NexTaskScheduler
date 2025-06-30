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
    using NexTaskScheduler.Jobs.Workers.NexBot;
    using NexTaskScheduler.Jobs.Workers.NexVision;

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
            AddNexVisionWorker(services);
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
            services.AddHostedService<ReSendErrorMessageWorker>();
            services.AddHostedService<RemoveOldMessageLogAndSystemLogWorker>();

            services.AddHostedService<NewBioDeviceAlertWorker>();
            services.AddHostedService<BioTimeChangeAlertWorker>();
            services.AddHostedService<FingerprintAndCardVerifyAlertWorker>();
            services.AddHostedService<DeductLateInEarlyOutWorker>();
            services.AddHostedService<LateInEarlyOutViolateOnMonthWarningWorker>();
            services.AddHostedService<RemindBirthdayAndFirstWorkingdayWorker>();
            services.AddHostedService<RemindMissingCheckInCheckOutWorker>();
            services.AddHostedService<ReminderLateInEarlyOutMoreThanSixtyMinuteWorker>();
            services.AddHostedService<RemindReachedWorkingTimeWorker>();
            services.AddHostedService<ImportDataFromBioStarWorker>();
            services.AddHostedService<SendScheduledMessageWorker>();
            services.AddHostedService<RemindVegetarianMealWorker>();
            services.AddHostedService<ViolateDisciplineWarningWorker>();
            services.AddHostedService<SkypeAccountAutomapWorker>();
            services.AddHostedService<LateInEarlyOutAutoDeductWorker>();
            services.AddHostedService<NotifyEmployeeCheckoutWorker>();
            services.AddHostedService<NotifyEmployeeCheckinWorker>();
            services.AddHostedService<RemindProcessPendingRequestWorker>();
            services.AddHostedService<LogTimeRemindWorker>();
            services.AddHostedService<RemindCancelMealWorker>();
        }

        private static void AddLumiWorker(IServiceCollection services)
        {
            services.AddHostedService<LumiDeductLateInEarlyOutAlertWorker>();
            services.AddHostedService<LumiLateInEarlyOutViolateOnMonthWarningWorker>();
            services.AddHostedService<LumiRemindBirthdayAndFirstWorkingdayWorker>();
            services.AddHostedService<LumiRemindMissingCheckInCheckOutWorker>();
            services.AddHostedService<LumiRemindReachedWorkingTimeWorker>();
            services.AddHostedService<LumiRemindVegetarianMealWorker>();
            services.AddHostedService<LumiImportDataFromBioStarWorker>();
            services.AddHostedService<LumiViolateDisciplineWarningWorker>();
            services.AddHostedService<LuminetSkypeAccountAutoMappingWorker>();
            services.AddHostedService<LumiDeductLateInEarlyOutWorker>();
            services.AddHostedService<LumiNotifyEmployeeCheckoutWorker>();
            services.AddHostedService<LumiNotifyEmployeeCheckinWorker>();
            services.AddHostedService<LumiRemindProcessPendingRequestWorker>();
            services.AddHostedService<LumiRemindCancelMealWorker>();
        }

        private static void AddNexVisionWorker(IServiceCollection services)
        {
            services.AddHostedService<NexVisionDeductLateInEarlyOutAlertWorker>();
            services.AddHostedService<NexVisionLateInEarlyOutViolateOnMonthWarningWorker>();
            services.AddHostedService<NexVisionRemindBirthdayAndFirstWorkingdayWorker>();
            services.AddHostedService<NexVisionRemindMissingCheckInCheckOutWorker>();
            services.AddHostedService<NexVisionRemindReachedWorkingTimeWorker>();
            services.AddHostedService<NexVisionRemindVegetarianMealWorker>();
            services.AddHostedService<NexVisionImportDataFromBioStarWorker>();
            services.AddHostedService<NexVisionViolateDisciplineWarningWorker>();
            services.AddHostedService<NexVisionSkypeAccountAutoMappingWorker>();
            services.AddHostedService<NexVisionDeductLateInEarlyOutWorker>();
            services.AddHostedService<NexVisionNotifyEmployeeCheckoutWorker>();
            services.AddHostedService<NexVisionNotifyEmployeeCheckinWorker>();
            services.AddHostedService<NexVisionRemindProcessPendingRequestWorker>();
        }
    }
}