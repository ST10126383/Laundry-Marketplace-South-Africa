using System.Net;
using System.Text.Json;

namespace LaundryMarketplace.Web.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred. Path: {Path}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "text/html";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // For API-style requests return JSON
            if (context.Request.Headers.Accept.ToString().Contains("application/json") ||
                context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.ContentType = "application/json";
                var payload = new
                {
                    error = "An unexpected error occurred.",
                    detail = _env.IsDevelopment() ? exception.Message : null,
                    traceId = context.TraceIdentifier
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
                return;
            }

            // For normal browser requests redirect to a friendly error page
            // We store the message in TempData-like fashion via query or just show generic page
            context.Response.Redirect($"/Home/Error?message={Uri.EscapeDataString(_env.IsDevelopment() ? exception.Message : "Something went wrong. Please try again.")}");
        }
    }

    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}
