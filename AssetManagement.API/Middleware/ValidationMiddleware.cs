using AssetManagement.Application.DTOs;
using System.Text.Json;

namespace AssetManagement.API.Middleware
{
    public class ValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ValidationMiddleware> _logger;

        public ValidationMiddleware(RequestDelegate next, ILogger<ValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log request details
            _logger.LogInformation("Processing request: {Method} {Path}", 
                context.Request.Method, context.Request.Path);

            await _next(context);

            // Log response details
            _logger.LogInformation("Response status: {StatusCode}", context.Response.StatusCode);
        }
    }
}