using FitnessApi.Interfaces;
using FitnessApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Security.Claims;

namespace FitnessApi;

public class SugarLogsFunction(ISugarLogsDao sugarLogsDao, ILogger<SugarLogsFunction> logger)
{
    private readonly ISugarLogsDao _sugarLogsDao = sugarLogsDao;
    private readonly ILogger<SugarLogsFunction> _logger = logger;

    [Function("GetAllSugarLogs")]
    [Authorize]
    public async Task<HttpResponseData> GetAllSugarLogs([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "SugarLogs/GetAllSugarLogs")] HttpRequestData req, FunctionContext executionContext)
    {
        _logger.LogInformation("GetAllSugarLogs Initiated");
        var principal = req.FunctionContext?.GetHttpContext()?.User;
        var principal1 = executionContext.Features.Get<ClaimsPrincipal>();
        // Read the claim you need (e.g., "sub" or "userId")
        var userId = principal?.FindFirst("userId")?.Value
                     ?? principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? string.Empty;
        _logger.LogInformation("user Id : {0}", userId);
        var response = req.CreateResponse(HttpStatusCode.OK);
        var items = await _sugarLogsDao.GetAllSugarLogs(userId);
        await response.WriteAsJsonAsync(items);
        return response;
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

        var principal = executionContext.Features.Get<ClaimsPrincipal>();
        // Read the claim you need (e.g., "sub" or "userId")
        var userId = principal?.FindFirst("userId")?.Value
                     ?? principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? string.Empty;

        await _sugarLogsDao.AddSugarLog(userId, dto);
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(dto);
        return response;
    }
}