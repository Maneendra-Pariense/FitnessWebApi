using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using FitnessApi.DataAccess;
using FitnessApi.DataLayer;
using FitnessApi.Interfaces;
using FitnessApi.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var builder = FunctionsApplication.CreateBuilder(args);
builder.ConfigureFunctionsWebApplication();

builder.UseFunctionExecutionMiddleware()
       .UseMiddleware<ConfigureJWTBearerOptions>();
    


builder.Configuration.AddAzureKeyVault(
     new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
    new DefaultAzureCredential(),
    new KeyVaultSecretManager());

//var conn = builder.Configuration.GetSection("ParienseDbConnectionString").Value;
var conn = builder.Configuration.GetSection("ParienseLocalDbConnectionString").Value;


builder.Services.AddDbContext<ParienseDbContext>(options => options.UseSqlServer(conn));

builder.Services.AddScoped<ISugarLogsDao, SugarLogsDao>();

// JWT authentication
builder.Services.AddSingleton<IFunctionsWorkerMiddleware, ConfigureJWTBearerOptions>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme);

// Add authorization
builder.Services.AddAuthorization();
builder.Services.AddRouting();

var app = builder.Build();

app.Run();