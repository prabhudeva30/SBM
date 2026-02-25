using System.Net;

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

                var message = statusCode == HttpStatusCode.InternalServerError ? "Internal server error." : ex.Message;
                _logger.LogError(ex, "An exception occurred while processing request: {Message}", ex.Message);

                await context.Response.WriteAsJsonAsync(message);
            }
        }

        private static HttpStatusCode GetStatusCode(Exception exception)
        {
            return exception switch
            {
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                ArgumentOutOfRangeException => HttpStatusCode.BadRequest,
                ArgumentNullException => HttpStatusCode.BadRequest,
                ArgumentException => HttpStatusCode.BadRequest,
                DirectoryNotFoundException => HttpStatusCode.NotFound,
                FileNotFoundException => HttpStatusCode.NotFound,
                WebException webEx when webEx.Response is HttpWebResponse response => response.StatusCode,
                _ => HttpStatusCode.InternalServerError
            };
        }
    }
}