using Arch.BaseApi;
using Arch.CQRS.Query.Behavior;
using Arch.Domian;
using Arch.EFCore;
using Arch.Kafka.Producer;
using Arch.Kafka.Register;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using UsageApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<UsageDbContext>(x => x.UseSqlServer(
           builder.Configuration.GetConnectionString("SqlServer"),
           y =>
           {
               y.MigrationsAssembly(nameof(UsageApi)).EnableRetryOnFailure();
           }
       ));
builder.Services.AddControllersWithViews(x => { x.Filters.Add(typeof(BaseExceptionHandler)); })
                .AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    //options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
}
);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg => { 
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddOpenBehavior(typeof(QueryCachedBehave<,>));//EnableQueryCaching
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddKafka(p =>
{
    p.BootstrapServers = builder.Configuration.GetConnectionString("Kafka");
},
consumer =>
{
    consumer.OffsetResetType = AutoOffsetReset.Earliest;
    consumer.GroupId = nameof(UsageApi);
    consumer.BootstrappServers = builder.Configuration.GetConnectionString("Kafka");
    consumer.EventAssemblies = new[] { typeof(Program).Assembly, typeof(Event).Assembly };
    consumer.MaxPollIntervalMs = 50_000;
    consumer.SessionTimeoutMs = 50_000;
    consumer.PreMessageHandlingHandler = (provider, @event, headers) => ValueTask.CompletedTask;
});
builder.Services.AddScoped<IUnitOfWork>(x =>
new UnitOfWork(x.GetRequiredService<IEventBus>(), x.GetRequiredService<UsageDbContext>(), x.GetRequiredService<ILogger<UnitOfWork>>()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
