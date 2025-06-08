using Arch.Test.TestInfra;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using UsageApi.CQRS.Commnand;
using UsageApi.CQRS.Query;
using UsageApi.Domain;
using Xunit;
using Arch.EFCore;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
namespace Arch.Test.IntegrationTest
{
    public class TestCreateUsage01 : IClassFixture<IntegrationTestFixture<Program>>
    {
        private readonly IMediator mediator;
        private readonly IUnitOfWork unitOfWork;
        public TestCreateUsage01(IntegrationTestFixture<Program> fixture)
        {
            mediator = fixture.Services.GetRequiredService<IMediator>();
            unitOfWork = fixture.Services.GetRequiredService<IUnitOfWork>();
        }
        [Fact]
        public async Task Create()
        {
            var command = new CreateUsageCommand { Title = "Test" };
            await mediator.Send(command);
            var result = await mediator.Send(new GetAllUsageQuery());
            var @event = new UsageTestEvent()
            {
                Id = result.First().Id,
            };
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.First().Title, command.Title);
            var usage = new Usage01(command.Title, Usage01Type.Type02);
            unitOfWork.Repo<Usage01>().Add(usage);
            await unitOfWork.Save(usage, true);
            result = await mediator.Send(new GetAllUsageQuery());
            Assert.AreEqual(2, result.Count());
            Assert.AreNotEqual(result.First().Id, result.Last().Id);
        }

    }
}
