using Arch.BaseApi;
using Arch.CQRS.Query.Behavior;
using Arch.EFCore;
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
builder.Services.AddScoped<DbContext, UsageDbContext>();
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
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddDistributedMemoryCache();
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
