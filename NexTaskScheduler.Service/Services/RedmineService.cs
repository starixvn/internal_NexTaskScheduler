namespace NexTaskScheduler.Service.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json.Linq;
    using NexTaskScheduler.Core.Models;
    using NexTaskScheduler.Core.Models.Redmine;
    using NexTaskScheduler.Criteria.HRM;
    using NexTaskScheduler.Service.Services.Interfaces;

    public class RedmineService : BaseService, IRedmineService
    {
        private readonly IOptionsSnapshot<RedmineConfigs> configs;
        private readonly string dateTimeFormat = "yyyy-MM-dd";

        public RedmineService(IOptionsSnapshot<RedmineConfigs> configs)
        {
            this.configs = configs;
        }

        public async Task<IEnumerable<RedmineUser>> GetForgotLogtimeEmployees()
        {
            var employees = await GetDataAsync<RedmineUser>(new GetRedmineUserCriteria());

            if (employees.Any())
            {
                foreach (var employee in employees)
                {
                    await GetForgotDates(employee);
                }
            }

            return employees.Where(e => e.ForgotLogtimeDates.Any());
        }

        private HttpClient AuthorizeRequestHeader()
        {
            var authValue = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{Cryptography.DecryptPassword(configs.Value.RedmineConnection)}")
                    ));

            var client = new HttpClient
            {
                DefaultRequestHeaders = { Authorization = authValue }
            };

            return client;
        }

        private async Task GetForgotDates(RedmineUser employee)
        {
            var toDate = DateTime.Now.AddDays(-1);
            var fromDate = toDate.AddDays(-6);
            var logtimeDates = await GetTimeEntries(employee.RedmineId, fromDate, toDate);
            var workingDates = await GetWorkedDates(employee.EmpId, fromDate, toDate);

            employee.ForgotLogtimeDates = workingDates.Except(logtimeDates).Distinct().OrderBy(x => x).ToList();
        }

        private async Task<IEnumerable<DateTime>> GetTimeEntries(int redmineId, DateTime fromDate, DateTime toDate)
        {
            string timeEntriesUrl = configs.Value.BaseUrl + "time_entries.json?user_id={0}&spent_on=><{1}|{2}";
            var url = new Uri(string.Format(timeEntriesUrl, redmineId, fromDate.ToString(dateTimeFormat), toDate.ToString(dateTimeFormat)));
            HttpClient client = AuthorizeRequestHeader();
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
            var listDates = new List<DateTime>();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                JObject obj = JObject.Parse(json);

                var timeEntries = obj["time_entries"];

                foreach (var timeEntry in timeEntries)
                {
                    var spendOn = DateTime.ParseExact(timeEntry["spent_on"].ToString(), dateTimeFormat, CultureInfo.InvariantCulture);
                    listDates.Add(spendOn);
                }
            }

            return listDates;
        }

        private async Task<IEnumerable<DateTime>> GetWorkedDates(int empId, DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetEmployeeWorkingDatesCriteria
            {
                EmployeeId = empId,
                FromDate = fromDate,
                ToDate = toDate
            };

            return await GetDataAsync<DateTime>(criteria);
        }
    }
}