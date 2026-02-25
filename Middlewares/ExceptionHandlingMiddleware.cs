using System.Net;
using System.Text.Json;

namespace WebApi.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var statusCode = GetStatusCode(ex);
                context.Response.StatusCode = (int)statusCode;
                context.Response.ContentType = "application/json";

                _logger.LogError(ex, "An exception occurred while processing request: {Message}", ex.Message);
                await context.Response.WriteAsJsonAsync(GetCustomMessage(ex, statusCode));
            }
        }

        private static HttpStatusCode GetStatusCode(Exception exception)
        {
            return exception switch
            {
                DirectoryNotFoundException => HttpStatusCode.NotFound,
                FileNotFoundException => HttpStatusCode.NotFound,
                InvalidDataException => HttpStatusCode.InternalServerError,
                JsonException => HttpStatusCode.InternalServerError,
                WebException webEx when webEx.Response is HttpWebResponse response => response.StatusCode,
                _ => HttpStatusCode.InternalServerError
            };
        }

        private static string GetCustomMessage(Exception ex, HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.NotFound when ex is FileNotFoundException or DirectoryNotFoundException
                    => "Requested resource not found.",
                _ => "Internal server error."
            };
        }
    }
}