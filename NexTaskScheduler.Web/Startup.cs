namespace NexTaskScheduler.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Hangfire;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using NexTaskScheduler.AutoJobs;
    using NexTaskScheduler.AutoJobs.BioTasks;
    using NexTaskScheduler.AutoJobs.HRMTasks;
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
            services.Configure<HangfireConfigs>(options => Configuration.GetSection("HangfireConfigs").Bind(options));
            services.Configure<RedmineConfigs>(options => Configuration.GetSection("RedmineConfigs").Bind(options));
            services.AddHangfire(config => config.UseSqlServerStorage(Cryptography.DecryptConnectionString(Configuration.GetConnectionString("Hangfire"))));

            Injector(services);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            InjectAutoJobs(services);

            InitAutoJobs(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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

        private void Injector(IServiceCollection services)
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
            services.AddTransient<BioDeviceSchedule>();
            services.AddTransient<BioTimeChangeSchedule>();
            services.AddTransient<FingerprintAndCardVerifySchedule>();
            services.AddTransient<RemindMissingCheckInCheckOutSchedule>();
            services.AddTransient<RemindForgotLogtimeSchedule>();
            services.AddTransient<RemindHRDeductLateInEarlyOutSchedule>();
            services.AddTransient<RemindCurrentMonthLateInEarlyOutViolateSchedule>();
            services.AddTransient<RemindViolateDisciplineSchedule>();

            services.AddTransient<Func<MessageCategory, IAutoReminder>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case MessageCategory.BioDevice:
                        return serviceProvider.GetService<BioDeviceSchedule>();

                    case MessageCategory.BioTimeChange:
                        return serviceProvider.GetService<BioTimeChangeSchedule>();

                    case MessageCategory.BioFingerprintAndCard:
                        return serviceProvider.GetService<FingerprintAndCardVerifySchedule>();

                    case MessageCategory.MissingCheckInCheckOut:
                        return serviceProvider.GetService<RemindMissingCheckInCheckOutSchedule>();

                    case MessageCategory.ForgotSpendTime:
                        return serviceProvider.GetService<RemindForgotLogtimeSchedule>();

                    case MessageCategory.LateInEarlyOutWarningDeduct:
                        return serviceProvider.GetService<RemindHRDeductLateInEarlyOutSchedule>();

                    case MessageCategory.LateInEarlyOutViolateCurrentMonth:
                        return serviceProvider.GetService<RemindCurrentMonthLateInEarlyOutViolateSchedule>();

                    case MessageCategory.ViolateLateInEarlyOutDiscipline:
                        return serviceProvider.GetService<RemindViolateDisciplineSchedule>();

                    default:
                        throw new KeyNotFoundException(); // or maybe return null, up to you
                }
            });
        }
    }
}