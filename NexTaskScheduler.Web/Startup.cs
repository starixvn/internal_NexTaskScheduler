namespace NexTaskScheduler.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Hangfire;
    using Hangfire.Storage;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using NexTaskScheduler.AutoJobs;
    using NexTaskScheduler.AutoJobs.BioTasks;
    using NexTaskScheduler.AutoJobs.HRMTasks;
    using NexTaskScheduler.AutoJobs.NexGoalTasks;
    using NexTaskScheduler.AutoJobs.RedmineTasks;
    using NexTaskScheduler.Core.Authorize;
    using NexTaskScheduler.Core.Enum;
    using NexTaskScheduler.Core.Models;
    using NexTaskScheduler.Service.Respositories;
    using NexTaskScheduler.Service.Respositories.Interfaces;
    using NexTaskScheduler.Service.Services;
    using NexTaskScheduler.Service.Services.Interfaces;
    using RestSharp;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Dictionary<string, string> connections = Configuration
                .GetSection("ConnectionStrings")
                .GetChildren()
                .ToDictionary(connection => connection.Key, connection => connection.Value, StringComparer.OrdinalIgnoreCase);

            DbSettingProviderManager
               .StartNewSession()
               .UseConnectionStrings(connections)
               .UseDefaultDbSettingProvider("~/App_Data")
               .Run();

            services.Configure<NexbotConfigs>(options => Configuration.GetSection("NexbotConfigs").Bind(options));
            services.Configure<CronExpressionConfigs>(options => Configuration.GetSection("CronExpressions").Bind(options));
            services.Configure<RedmineConfigs>(options => Configuration.GetSection("RedmineConfigs").Bind(options));
            services.Configure<NexGoalConfigs>(options => Configuration.GetSection("NexGoalConfigs").Bind(options));
            services.Configure<NexHrmConfigs>(options => Configuration.GetSection("NexHrmConfigs").Bind(options));
            services.AddHangfire(config => config.UseSqlServerStorage(Cryptography.DecryptConnectionString(Configuration.GetConnectionString("Hangfire"))));

            InjecServices(services);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            InjectAutoJobs(services);

            InitAutoJobs(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStopping.Register(OnShutdown);

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorization() }
            });
            app.UseHangfireServer();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void InitAutoJobs(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var initAutoJobs = serviceProvider.GetService<InitAutoJobs>();
            initAutoJobs.Initialize();
        }

        private void InjecServices(IServiceCollection services)
        {
            services.AddSingleton(_ => Configuration);
            services.AddSingleton<IRecurringJobManager, RecurringJobManager>();
            services.AddSingleton<IBaseService, BaseService>();
            services.AddTransient<IRestClient, RestClient>();
            services.AddTransient<INexbotService, NexbotService>();
            services.AddTransient<IRedmineService, RedmineService>();
            services.AddTransient<IHRMRepository, HRMRepository>();
            services.AddTransient<IHRMService, HRMService>();
            services.AddTransient<IRedmineService, RedmineService>();
            services.AddTransient<IHangFireService, HangFireService>();
        }

        private void InjectAutoJobs(IServiceCollection services)
        {
            services.AddTransient<InitAutoJobs>();
            services.AddTransient<BioDeviceScheduleAlert>();
            services.AddTransient<BioTimeChangeScheduleAlert>();
            services.AddTransient<FingerprintAndCardVerifyScheduleAlert>();
            services.AddTransient<RemindMissingCheckInCheckOutSchedule>();
            services.AddTransient<ForgotLogtimeScheduleReminder>();
            services.AddTransient<DeductLateInEarlyOutScheduleAlert>();
            services.AddTransient<LateInEarlyOutViolateOnMonthScheduleWarning>();
            services.AddTransient<ViolateDisciplineScheduleWarning>();
            services.AddTransient<NexGoalAutoJobSchedule>();
            services.AddTransient<ScheduledMessage>();

            services.AddTransient<Func<MessageCategory, IJobSchedule>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case MessageCategory.BioDevice:
                        return serviceProvider.GetService<BioDeviceScheduleAlert>();

                    case MessageCategory.BioTimeChange:
                        return serviceProvider.GetService<BioTimeChangeScheduleAlert>();

                    case MessageCategory.BioFingerprintAndCard:
                        return serviceProvider.GetService<FingerprintAndCardVerifyScheduleAlert>();

                    case MessageCategory.MissingCheckInCheckOut:
                        return serviceProvider.GetService<RemindMissingCheckInCheckOutSchedule>();

                    case MessageCategory.ForgotSpendTime:
                        return serviceProvider.GetService<ForgotLogtimeScheduleReminder>();

                    case MessageCategory.DeductLateInEarlyOut:
                        return serviceProvider.GetService<DeductLateInEarlyOutScheduleAlert>();

                    case MessageCategory.LateInEarlyOutViolateInMonth:
                        return serviceProvider.GetService<LateInEarlyOutViolateOnMonthScheduleWarning>();

                    case MessageCategory.ViolateLateInEarlyOutDiscipline:
                        return serviceProvider.GetService<ViolateDisciplineScheduleWarning>();

                    case MessageCategory.NexGoalAutoJobSchedule:
                        return serviceProvider.GetService<NexGoalAutoJobSchedule>();

                    case MessageCategory.NexHrmScheduledMessage:
                        return serviceProvider.GetService<ScheduledMessage>();

                    default:
                        throw new KeyNotFoundException(); // or maybe return null, up to you
                }
            });
        }

        private void OnShutdown()
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                foreach (var recurringJob in connection.GetRecurringJobs())
                {
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }
            }
        }
    }
}