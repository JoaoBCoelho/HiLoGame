using HiLoGame.API.Middlewares;
using HiLoGame.IoC;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();

builder.Services.ConfigureServices();
builder.Services.ConfigureRepositories();

builder.Services.ConfigureDbContext(configuration["ConnectionString"]);
builder.Services.ConfigureAutoMapper();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();