using Arch.Test.TestInfra;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Text;
using UsageApi.CQRS.Commnand;
using UsageApi.CQRS.Query;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Arch.BaseApi;
using System.Net;
using Moq;
using Arch.Domain;
using UsageApi.Domain;
namespace Arch.Test.SystemTest
{
    public class TestCreateUsage01 : IClassFixture<IntegrationTestFixture<Program>>
    {
        private readonly HttpClient client;
        private readonly JsonSerializerSettings jsonSetting;
        private Mock<IEventBus> eventBusMock;
        public TestCreateUsage01(IntegrationTestFixture<Program> fixture)
        {
            client = fixture.CreateClient();
            jsonSetting = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            eventBusMock = fixture.EventBusMock;
        }
        [Fact]
        public async Task Create()
        {
            var command = new CreateUsageCommand { Title = "Test" };
            var param = JsonConvert.SerializeObject(command, jsonSetting);
            var content = new StringContent(param, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/Usage", content, CancellationToken.None);
            response.EnsureSuccessStatusCode();
            var responseGet = await client.GetAsync("/api/Usage", CancellationToken.None);
            response.EnsureSuccessStatusCode();
            var result = await SetResponse<ApiResult<List<GetAllUsageResult>>>(responseGet);
            Assert.AreEqual(1, result.Data.Count());
            Assert.AreEqual(result.Data.First().Title, command.Title);
            eventBusMock.Verify(
            bus => bus.Execute(It.Is<Event>(e=>e.GetType()==typeof(UsageTestEvent)),
        It.IsAny<Dictionary<string, string>>(),
        It.IsAny<CancellationToken>()),
            Times.Once);
        }
        private static async Task<T> SetResponse<T>(HttpResponseMessage httpResponse)
        {
            var response = await httpResponse.Content.ReadAsStringAsync();

            try
            {
                var result = JsonConvert.DeserializeObject<T>(response);

                var type = result.GetType();
                var statusCode = type.GetProperty(nameof(ApiResult.StatusCode));
                statusCode?.SetValue(result, httpResponse.StatusCode, null);

                return result;
            }
            catch (Exception)//TODO لاگ ثبت شود
            {
                return default;
            }
        }
    }
    public class ApiResult<T>
    {
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        public string? Message { get; set; }
        public IEnumerable<ResponseError> Errors { get; set; }
        public T Data { get; set; }
        public int? Count { get; set; }
    }
}
