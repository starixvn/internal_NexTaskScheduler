namespace NexTaskScheduler.Service.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using NexTaskScheduler.Core.Enum;
    using NexTaskScheduler.Core.Extentions;
    using NexTaskScheduler.Core.Models;
    using NexTaskScheduler.Criteria.Hangfire;
    using NexTaskScheduler.Service.Models.Nexbot;
    using NexTaskScheduler.Service.Services.Interfaces;
    using RestSharp;

    public class NexbotService : BaseService, INexbotService
    {
        private readonly IRestClient webClient;
        private readonly IOptionsSnapshot<NexbotConfigs> configuration;

        public NexbotService(IOptionsSnapshot<NexbotConfigs> configuration)
        {
            webClient = new RestClient { Encoding = Encoding.UTF8 };
            this.configuration = configuration;
        }

        public async Task SendMessage(string conversationId, string message)
        {
            var token = await GetToken().ConfigureAwait(false);
            await ForwardToBot(message, conversationId, token).ConfigureAwait(false);
        }

        public async Task<IEnumerable<string>> GetRegisteredConversation(MessageCategory categoryId)
        {
            return await GetDataAsync<string>(new GetRegisteredConversationCriteria { CategoryId = categoryId }).ConfigureAwait(false);
        }

        private async Task ForwardToBot(string message, string conversationId, Token token)
        {
            webClient.BaseUrl = new Uri(configuration.Value.BotServiceUrl);

            var request = new RestRequest("/messages/forward", Method.POST);
            request.AddHeader("Authorization", $"Bearer {token.AccessToken}");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("message", message);
            request.AddParameter("conversationId", conversationId);

            await webClient.ExecutePostTaskAsync(request).ConfigureAwait(false);
        }

        private async Task<Token> GetToken()
        {
            webClient.BaseUrl = new Uri(configuration.Value.TokenUrl);

            var request = new RestRequest(Method.POST);
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", configuration.Value.MicrosoftAppId);
            request.AddParameter("client_secret", configuration.Value.MicrosoftAppPassword);
            request.AddParameter("scope", $"{configuration.Value.MicrosoftAppId}/.default");

            request.AddHeader("Content-type", "application/x-www-form-urlencoded");

            var response = await webClient.ExecutePostTaskAsync<Token>(request).ConfigureAwait(false);

            return response.Data;
        }

        public async Task WriteLog(string messageType, string logContent)
        {
            var criteria = new WriteLogCriteria
            {
                MessageType = messageType,
                LogContent = logContent
            };

            await ExecuteNonQueryAsync(criteria);
        }

        public async Task WriteCategoryMessageLog(MessageCategory category)
        {
            string type = category.GetDisplayName();
            await WriteLog(type, $"Task {type} executed at {DateTime.Now.ToShortFormatedDate()}");
        }
    }
}