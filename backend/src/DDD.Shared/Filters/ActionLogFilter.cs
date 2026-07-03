using System.Text.Json;
using DDD.Shared.Attributes;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DDD.Shared.Filters;

[Injectable(ServiceLifetime.Scoped)]
public sealed class ActionLogFilter : IAsyncActionFilter
{
    private static readonly HashSet<string> SensitiveKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "password", "confirmpassword", "oldpassword", "newpassword", "secret",
        "token", "accesstoken", "refreshtoken", "authorization", "cookie",
        "apikey", "privatekey", "secretkey"
    };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly ILogger<ActionLogFilter> _logger;

    public ActionLogFilter(ILogger<ActionLogFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var request = context.HttpContext.Request;
        var method = request.Method;
        var path = request.Path;
        var queryString = request.QueryString.HasValue ? request.QueryString.Value : string.Empty;
        var fullUrl = $"{request.Scheme}://{request.Host}{path}{queryString}";
        var clientIp = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var controllerName = context.Controller.GetType().Name;
        var actionName = context.ActionDescriptor.DisplayName;
        var sanitizedParams = SanitizeParameters(context.ActionArguments);

        _logger.LogInformation(
            "[请求开始] {Method} {FullUrl} | 客户端IP: {ClientIp} | Controller: {Controller} | Action: {Action} | 参数: {@Parameters}",
            method, fullUrl, clientIp, controllerName, actionName, sanitizedParams);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var executedContext = await next();

        stopwatch.Stop();

        if (executedContext.Exception != null)
        {
            _logger.LogError(
                executedContext.Exception,
                "[请求异常] {Method} {FullUrl} | 耗时: {Elapsed}ms",
                method, fullUrl, stopwatch.ElapsedMilliseconds);
        }
        else
        {
            var statusCode = executedContext.HttpContext.Response.StatusCode;
            _logger.LogInformation(
                "[请求结束] {Method} {FullUrl} | 状态码: {StatusCode} | 耗时: {Elapsed}ms",
                method, fullUrl, statusCode, stopwatch.ElapsedMilliseconds);
        }
    }

    private static object SanitizeParameters(IDictionary<string, object?> arguments)
    {
        if (arguments.Count == 0)
        {
            return new { };
        }

        var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        foreach (var kvp in arguments)
        {
            if (kvp.Value is null)
            {
                result[kvp.Key] = null;
                continue;
            }

            var type = kvp.Value.GetType();

            if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal) || type == typeof(Guid))
            {
                result[kvp.Key] = SensitiveKeys.Contains(kvp.Key) ? "***" : kvp.Value.ToString();
            }
            else
            {
                try
                {
                    var json = JsonSerializer.Serialize(kvp.Value, JsonOptions);
                    var sanitized = SanitizeJsonString(json);
                    result[kvp.Key] = sanitized;
                }
                catch
                {
                    result[kvp.Key] = kvp.Value.ToString();
                }
            }
        }

        return result;
    }

    private static string SanitizeJsonString(string json)
    {
        var doc = JsonDocument.Parse(json);
        var sanitized = SanitizeJsonElement(doc.RootElement);
        return JsonSerializer.Serialize(sanitized, JsonOptions);
    }

    private static object SanitizeJsonElement(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var dict = new Dictionary<string, object?>();
                foreach (var prop in element.EnumerateObject())
                {
                    dict[prop.Name] = SensitiveKeys.Contains(prop.Name)
                        ? "***"
                        : SanitizeJsonElement(prop.Value);
                }
                return dict;

            case JsonValueKind.Array:
                var list = new List<object?>();
                foreach (var item in element.EnumerateArray())
                {
                    list.Add(SanitizeJsonElement(item));
                }
                return list;

            default:
                return element.ToString();
        }
    }
}