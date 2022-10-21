namespace NexTaskScheduler.Jobs.Workers
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Newtonsoft.Json;
    using NexTaskScheduler.Core;
    using NexTaskScheduler.Core.Models;

    public abstract class BaseWorker : BackgroundService
    {
        private readonly ILogServiceProvider logService;
        protected JobSetting jobSetting;
        private readonly IHttpClientFactory clientFactory;
        private bool isFirstRun = true;

        protected BaseWorker(ILogServiceProvider logService,
            IHttpClientFactory clientFactory)
        {
            this.logService = logService;
            this.clientFactory = clientFactory;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logService.WriteLogFileInfo($"{jobSetting.JobName}: Service starting");
            return base.StartAsync(cancellationToken);
        }

        public async Task DoWorkAsync()
        {
            string token = string.Empty;
            if (jobSetting.IsAuth)
            {
                token = await GetTokenAsync();
            }

            await SendHttpClientAsync<JobCommonResponse>(jobSetting.BaseUrl, jobSetting.RequestUrl, jobSetting.Method, token);
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
                logService.WriteLogFileException(ex);
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

        private async Task<string> GetTokenAsync()
        {
            using var request = CreateRequest(jobSetting.BaseUrl, jobSetting.AuthRequestUrl, "POST");
            var client = clientFactory.CreateClient();

            using var response = await client.SendAsync(request);
            var jsonResult = await response.Content.ReadAsStringAsync();

            return jsonResult;
        }

        public async Task<T> SendHttpClientAsync<T>(string baseUrl, string endppoint, string method, string token)
        {
            using var request = CreateRequest(baseUrl, endppoint, method);
            var client = clientFactory.CreateClient();

            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            using var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonResult = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<T>(jsonResult);
                return result;
            }

            var err = await response.Content.ReadAsStringAsync();
            logService.WriteLogFileInfo(err);
            return default;
        }

        private HttpRequestMessage CreateRequest(string baseUrl, string endPoint, string method)
        {
            var httpMethod = string.Equals(method, "GET", StringComparison.OrdinalIgnoreCase)
                ? HttpMethod.Get : HttpMethod.Post;

            Uri url = new($"{baseUrl}{endPoint}");
            var request = new HttpRequestMessage(httpMethod, url);

            return request;
        }
    }
}