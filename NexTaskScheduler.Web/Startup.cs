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
    using NexTaskScheduler.Service.Models.Nexbot;
    using NexTaskScheduler.Service.Services;
    using NexTaskScheduler.Service.Services.Interfaces;

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
            services.AddHangfire(config => config.UseSqlServerStorage(Cryptography.DecryptConnectionString(Configuration.GetConnectionString("Hangfire"))));

            services.AddScoped<IBaseService, BaseService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseHangfireDashboard();
            app.UseHangfireServer();
            app.UseMvc();
        }
    }
}