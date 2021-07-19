namespace NexTaskScheduler.Jobs.Workers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Newtonsoft.Json;
    using NexTaskScheduler.Core;
    using NexTaskScheduler.Core.Models;
    using RestSharp;

    public class BaseWorker : BackgroundService
    {
        private readonly ILogServiceProvider logService;

        private Timer timer;
        private bool isRunning;

        protected JobSetting jobSetting;
        protected IRestClient restClient;
        private bool isFirstRun = true;


        public BaseWorker(ILogServiceProvider logService,
            IRestClient restClient)
        {
            this.logService = logService;
            this.restClient = restClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            timer = new Timer(Execute,
               null,
               TimeSpan.Zero,
               TimeSpan.FromSeconds(jobSetting.TimeInterval));

            await Task.CompletedTask;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logService.WriteLogFileInfo($"{jobSetting.JobName}: Service Starting");
            return base.StartAsync(cancellationToken);
        }

        private void Execute(object state)
        {
            try
            {
                if (IsJobReadyToRun() && !isRunning)
                {
                    isRunning = true;
                    logService.WriteLogFileInfo($"{jobSetting.JobName}: Doing...");
                    _ = ExecuteJobAsync();
                    logService.WriteLogFileInfo($"{jobSetting.JobName}: Done.");
                }
            }
            catch (Exception ex)
            {
                logService.WriteExceptionSentryErrorAsync(ex);
                logService.WriteLogFileInfo($"{jobSetting.JobName}: Error - {ex}");
            }
            finally
            {
                isRunning = false;
                isFirstRun = false;
            }
        }

        private bool IsJobReadyToRun()
        {
            bool isReady;
            if(!jobSetting.IsEnabled)
            {
                isReady = false;
            }
            else
            {
                if (isFirstRun && jobSetting.IsRunAtStarted)
                {
                    isReady = true;
                }
                else
                {
                    isReady = IsValidTimeRun();
                }
            }

            return isReady;
        }

        private bool IsValidTimeRun()
        {
            var isValid = string.IsNullOrWhiteSpace(jobSetting.RunAtFixedTime)
                || jobSetting.RunAtFixedTime.Contains(DateTime.Now.ToString("HH:mm"));
            return isValid;
        }

        public virtual async Task ExecuteJobAsync()
        {
            var method = string.Equals(jobSetting.Method, "GET", StringComparison.OrdinalIgnoreCase) 
                ? Method.GET : Method.POST;
            var request = CreateRequestHeader(jobSetting.RequestUrl, method);
            restClient.BaseUrl = new Uri(jobSetting.BaseUrl);
            if (jobSetting.IsAuth)
            {
                var tokenReponse = await restClient.ExecuteAsync(GetToken());
                if (tokenReponse.IsSuccessful)
                {
                    request.AddHeader("Authorization", $"Bearer {JsonConvert.DeserializeObject(tokenReponse.Content)}");
                }
            }

            await restClient.ExecuteAsync(request);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logService.WriteLogFileInfo($"{jobSetting.JobName}: Stopping Service");
            timer?.Change(Timeout.Infinite, 0);
            isFirstRun = true;
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            logService.WriteLogFileInfo($"{jobSetting.JobName}: Disposing Service");
            timer.Dispose();
            GC.SuppressFinalize(this);
            base.Dispose();
        }

        private RestRequest CreateRequestHeader(string resource, Method method)
        {
            var request = new RestRequest(resource, method);
            return request;
        }

        private RestRequest GetToken()
        {
            var request = new RestRequest(jobSetting.AuthRequestUrl, Method.POST);
            return request;
        }
    }
}
