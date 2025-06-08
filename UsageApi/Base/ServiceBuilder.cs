using Arch.BaseApi;
using Arch.CQRS.Register;
using Arch.Domain;
using Arch.EFCore;
using Arch.Kafka.Register;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using UsageApi.Data;

namespace UsageApi.Base
{
    public static class ServiceBuilder
    {
        public static WebApplication BuildOver(this WebApplicationBuilder builder)
        {
            if (!builder.Environment.IsEnvironment("Test"))
            {
                builder.Services.AddDbContext<UsageDbContext>(x => x.UseSqlServer(
                      builder.Configuration.GetConnectionString("SqlServer"),
                      y =>
                      {
                          y.MigrationsAssembly(nameof(UsageApi)).EnableRetryOnFailure();
                      }
                  ));
                

                builder.Services.AddScoped<IUnitOfWork>(x =>
            new UnitOfWork(x.GetRequiredService<IEventBus>(), x.GetRequiredService<UsageDbContext>(), x.GetRequiredService<ILogger<UnitOfWork>>()));
                
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
                builder.Services.AddCQRS(true, true);
            }
           
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
           
            builder.Services.AddDistributedMemoryCache();
           
            
            return builder.Build();
        }
        public static void RunOver(this WebApplication app)
        {
            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseRouting();
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
