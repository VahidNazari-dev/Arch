using Arch.Domain;
using Arch.EFCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using UsageApi.Data;
using MediatR;
using Arch.CQRS.Query.Behavior;
using Arch.CQRS.Command.Behavior;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Arch.Test.TestInfra
{
    public class IntegrationTestFixture<TStartup> : WebApplicationFactory<TStartup>, IDisposable where TStartup : class
    {
        public Mock<IEventBus> EventBusMock { get;  set; }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {

            builder.ConfigureServices(services =>
            {
                var descriptors = services.Where(
    d => d.ServiceType == typeof(DbContextOptions<UsageDbContext>)).ToList();

                foreach (var descriptor in descriptors)
                {
                    services.Remove(descriptor);
                }

                services
            .AddEntityFrameworkInMemoryDatabase()
            .AddDbContext<UsageDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("Usage").UseInternalServiceProvider(sp).ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            }).AddEntityFrameworkSqlServerNetTopologySuite();

                var uow = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IUnitOfWork));
                if (uow != null)
                    services.Remove(uow);

                EventBusMock = new Mock<IEventBus>();
                services.AddSingleton<IEventBus>(EventBusMock.Object);

                services.AddScoped<IUnitOfWork>(x =>
           new UnitOfWork(x.GetRequiredService<IEventBus>(), x.GetRequiredService<UsageDbContext>(), x.GetRequiredService<ILogger<UnitOfWork>>()));

                var mediatR = services.SingleOrDefault(
                  d => d.ServiceType == typeof(IMediator));
                if (mediatR != null)
                    services.Remove(mediatR);

                services.AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssemblies(typeof(TStartup).Assembly);

                    cfg.AddOpenBehavior(typeof(QueryCachedBehave<,>));//EnableQueryCaching

                    //EnableValidate

                    {
                        cfg.AddOpenBehavior(typeof(CommandValidateBehaveResult<,>));
                    }

                });
            });
        }
        protected override IHost CreateHost(IHostBuilder builder)
        {
            // اطمینان از اجرا نشدن سرور واقعی
            builder.UseEnvironment("Test");

            return base.CreateHost(builder);
        }
        public void Dispose()
        {
            // Clean up if needed
            this.Dispose(true);
        }
    }
    public class MockEventBus : IEventBus
    {
        public Task Execute<TEvent>(TEvent @event, Dictionary<string, string> headers, CancellationToken cancellationToken = default) where TEvent : Event
        {
            MockBus.Setup(x => x.Execute(@event, headers, cancellationToken)).Returns(Task.CompletedTask);
            return Task.CompletedTask;
        }
        public Mock<IEventBus> MockBus => new Mock<IEventBus>();
    }
}
