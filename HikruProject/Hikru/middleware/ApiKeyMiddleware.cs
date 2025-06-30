using HikruCodeChallenge.Infrastructure.Services;

namespace HikruCodeChallenge.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip authentication for Swagger endpoints and auth endpoints
        var path = context.Request.Path.Value?.ToLower() ?? "";
        
        if (path.StartsWith("/swagger") || 
            path.StartsWith("/api/auth") ||
            path.Equals("/") ||
            path.Equals(""))
        {
            await _next(context);
            return;
        }

        // Resolve the auth service from the request scope
        var authService = context.RequestServices.GetRequiredService<IAuthService>();

        var apiKey = context.Request.Headers["X-API-Key"].FirstOrDefault() ??
                     context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

        if (string.IsNullOrEmpty(apiKey) || !authService.ValidateApiKey(apiKey))
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"message\":\"API Key missing or invalid\",\"statusCode\":401}");
            return;
        }

        await _next(context);
    }
}
