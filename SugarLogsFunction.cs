using FitnessApi.DataAccess;
using FitnessApi.Interfaces;
using FitnessApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FitnessApi;

public class SugarLogsFunction(ISugarLogsDao sugarLogsDao, ILogger<SugarLogsFunction> logger)
{
    private readonly ISugarLogsDao _sugarLogsDao = sugarLogsDao;
    private readonly ILogger<SugarLogsFunction> _logger = logger;

    [Function("GetAllSugarLogs")]
    [Authorize]
    public async Task<HttpResponseData> GetAllSugarLogs(
        [HttpTrigger(AuthorizationLevel.User, "get", Route = "SugarLogs/GetAllSugarLogs")] HttpRequestData req,
        FunctionContext executionContext)
    {
        _logger.LogInformation("GetAllSugarLogs Initiated");

        // Get Authorization header
        string userId = await GetUserIdFromJWT(req);

        _logger.LogInformation("user Id : {0}", userId);

        var response = req.CreateResponse(HttpStatusCode.OK);
        var items = await _sugarLogsDao.GetAllSugarLogs(userId);
        await response.WriteAsJsonAsync(items);
        return response;
    }

    private static async Task<string> GetUserIdFromJWT(HttpRequestData req)
    {
        if (!req.Headers.TryGetValues("Authorization", out var authHeaders))
        {
            var unauthorizedResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
            await unauthorizedResponse.WriteStringAsync("Missing Authorization header");
            throw new UnauthorizedAccessException("Missing Authorization header");
        }

        var bearerToken = authHeaders.FirstOrDefault()?.Replace("Bearer ", string.Empty);
        if (string.IsNullOrEmpty(bearerToken))
        {
            var unauthorizedResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
            await unauthorizedResponse.WriteStringAsync("Invalid Authorization header");
            throw new UnauthorizedAccessException("Invalid Authorization header");
        }

        // Parse JWT
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(bearerToken);

        // Extract claim (adjust claim type based on your token)
        return jwtToken.Claims.FirstOrDefault(c => c.Type == "email" || c.Type == ClaimTypes.Email)?.Value ?? string.Empty;
    }

    [Function("AddSugarLog")]
    [Authorize]
    public async Task<HttpResponseData> AddSugarLog([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "SugarLogs/AddSugarLog")] HttpRequestData req, FunctionContext executionContext)
    {
        _logger.LogInformation("AddSugarLog Initiated");
        var dto = await req.ReadFromJsonAsync<SugarLogDto?>();
        if (dto == null)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Request body is null or could not be deserialized.");
            return badResponse;
        }

        var userId = await GetUserIdFromJWT(req);

        await _sugarLogsDao.AddSugarLog(userId, dto);
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(dto);
        return response;
    }
}