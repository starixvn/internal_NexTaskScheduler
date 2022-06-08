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

    public abstract class BaseWorker : BackgroundService
    {
        private readonly ILogServiceProvider logService;
        protected JobSetting jobSetting;
        protected IRestClient restClient;
        private bool isFirstRun = true;

        protected BaseWorker(ILogServiceProvider logService,
            IRestClient restClient)
        {
            this.logService = logService;
            this.restClient = restClient;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logService.WriteLogFileInfo($"{jobSetting.JobName}: Service starting");
            return base.StartAsync(cancellationToken);
        }

        public async Task DoWorkAsync()
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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const byte MillisecondsDelay = 10;
            await Task.Delay(MillisecondsDelay).ConfigureAwait(false);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await RunJobAsync();

                    await Task.Delay(TimeSpan.FromSeconds(jobSetting.TimeInterval), stoppingToken);
                }

                logService.WriteLogFileInfo($"Execution ended. Cancelation token cancelled = {stoppingToken.IsCancellationRequested}");
            }
            catch (Exception ex)
            {
                logService.WriteLogFileInfo("Unhandeled exception. Execution Stopping");
                logService.WriteLogFileInfo(ex.Message);
            }
        }

        private async Task RunJobAsync()
        {
            try
            {
                if (IsJobReadyToRun())
                {
                    logService.WriteLogFileInfo($"{jobSetting.JobName}: Doing...");
                    await DoWorkAsync();
                    logService.WriteLogFileInfo($"{jobSetting.JobName}: Done.");
                }
            }
            catch (Exception ex)
            {
                await logService.WriteExceptionSentryErrorAsync(ex);
                logService.WriteLogFileInfo($"{jobSetting.JobName}: Error - {ex.Message}");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logService.WriteLogFileInfo($"{jobSetting.JobName}: Stopping service");

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            logService.WriteLogFileInfo($"{jobSetting.JobName}: Disposing service");
            GC.SuppressFinalize(this);
            base.Dispose();
        }

        private bool IsJobReadyToRun()
        {
            bool isReady;
            if (!jobSetting.IsEnabled)
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

                isFirstRun = false;
            }

            return isReady;
        }

        private bool IsValidTimeRun()
        {
            var isValid = string.IsNullOrWhiteSpace(jobSetting.RunAtFixedTime)
                || jobSetting.RunAtFixedTime.Contains(DateTime.Now.ToString("HH:mm"));
            return isValid;
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