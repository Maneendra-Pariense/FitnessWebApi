using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FitnessApi.Middleware
{
    public class ConfigureJWTBearerOptions : IFunctionsWorkerMiddleware
    {
        private readonly IConfiguration _config;

        public ConfigureJWTBearerOptions(IConfiguration config)
        {
            _config = config;
            Console.WriteLine("ConfigureJWTBearerOptions constructor called");
        }

        public void Configure(string name, JwtBearerOptions options)
        {
            if (!name.Equals(JwtBearerDefaults.AuthenticationScheme)) return;

            var instance = _config.GetSection("AzureAd").GetValue<string>("Instance");
            var appId = _config.GetSection("AzureAd").GetValue<string>("AppId");
            var tenantId = _config.GetSection("AzureAd").GetValue<string>("TenantId");
            var authority = $"{instance}{tenantId}";
            options.Authority = authority;
            options.Audience = appId;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
            {
                ValidAudience = appId,
                ValidIssuer = authority,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = false,
                SaveSigninToken = true,
            };
        }

        public void Configure(JwtBearerOptions options)
        {
            Configure(string.Empty, options);
        }

        public Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            throw new NotImplementedException();
        }
    }
}
