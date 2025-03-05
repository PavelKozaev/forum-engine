using ForumEngine.API.Middlewares;
using ForumEngine.Domain.DependencyInjection;
using ForumEngine.Storage.DependencyInjection;
using Serilog;
using Serilog.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(b => b.AddSerilog(new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.WithProperty("Application", "ForumEngine.API")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .WriteTo.Logger(lc => lc
        .Filter.ByExcluding(Matching.FromSource("Microsoft"))
        .WriteTo.OpenSearch(
            builder.Configuration.GetConnectionString("Logs"),
            "forum-logs-{0.yyyy.MM.dd}"))
    .WriteTo.Logger(lc => lc
        .WriteTo.Console())
    .CreateLogger()));

builder.Services
    .AddForumDomain()
    .AddForumStorage(builder.Configuration.GetConnectionString("Postgres"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.Run();
