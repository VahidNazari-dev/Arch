
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Arch.Test.TestInfra;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Reqnroll;
using System.Text;
using UsageApi.CQRS.Commnand;
using Xunit;

namespace Arch.Test.AcceptanceTest.Steps.TestServer
{
    [Binding]
    public class CreateUsage01Steps : IClassFixture<IntegrationTestFixture<Program>>
    {
        private readonly HttpClient client;
        private readonly JsonSerializerSettings jsonSetting;
        private readonly ScenarioContext _scenarioContext;
        public CreateUsage01Steps(IntegrationTestFixture<Program> fixture, ScenarioContext scenarioContext)
        {
            client = fixture.CreateClient();
            jsonSetting = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            _scenarioContext = scenarioContext;
        }
        [Given(@"the API is running")]
        public async Task GivenTheApiIsRunningAsync()
        {
            // Check API health
            var responseGet = await client.GetAsync("/api/Usage", CancellationToken.None);
            responseGet.EnsureSuccessStatusCode();

        }

        [When(@"I create a Usage01 with title (.*)")]
        public async Task WhenICreateAUseage01WithNameAsync(string title)
        {
            // Call API endpoint
            var command = new CreateUsageCommand { Title = title };
            var param = JsonConvert.SerializeObject(command, jsonSetting);
            var content = new StringContent(param, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/Usage", content, CancellationToken.None);
            _scenarioContext["Response"] = response;
        }

        [Then(@"the response should be (.*) Created")]
        public async Task ThenTheResponseShouldBeCreatedAsync(string statusCode)
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");
            Assert.AreEqual(response.StatusCode.ToString(), statusCode);

        }

    }
}
