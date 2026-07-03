using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DDD.Shared.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var request = context.Request;
            var method = request.Method;
            var path = request.Path;
            var queryString = request.QueryString.HasValue ? request.QueryString.Value : string.Empty;
            var fullUrl = $"{request.Scheme}://{request.Host}{path}{queryString}";
            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            _logger.LogError(ex,
                "[全局异常] {Method} {FullUrl} | 客户端IP: {ClientIp} | 异常类型: {ExceptionType} | 异常消息: {Message}",
                method, fullUrl, clientIp, ex.GetType().Name, ex.Message);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            ArgumentException or InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "未授权访问"),
            KeyNotFoundException => (HttpStatusCode.NotFound, "资源未找到"),
            _ => (HttpStatusCode.InternalServerError, "服务器内部错误")
        };

        context.Response.StatusCode = (int)statusCode;

        var result = ApiResult.Fail(message);
        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}