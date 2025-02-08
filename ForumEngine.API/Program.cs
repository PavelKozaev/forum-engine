using ForumEngine.Domain.UseCases.CreateTopic;
using ForumEngine.Domain.UseCases.GetForums;
using ForumEngine.Storage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IGetForumsUseCase, GetForumsUseCase>();
builder.Services.AddScoped<ICreateTopicUseCase, CreateTopicUseCase>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContextPool<ForumDbContext>(options => options
    .UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
