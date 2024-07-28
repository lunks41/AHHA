
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace AHHA.API.ExceptionHandling
{
    public class GlobalExcaptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExcaptionHandler> _logger;

        public GlobalExcaptionHandler(ILogger<GlobalExcaptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            httpContext.Response.ContentType = "application/json";

            _logger.LogError($"Something went wrong: {exception}");

            var message = exception switch
            {
                AccessViolationException => "Access violation error from the IExceptionHandler",
                _ => "Internal Server Error from the IExceptionHandler"
            };

            var errorDetails = new ErrorDetails()
            {
                StatusCode = httpContext.Response.StatusCode,
                Message = message
            };
            var errorDetailsJson = errorDetails.ToString();

            await httpContext.Response.WriteAsync(errorDetailsJson);

            return true;
        }
    }

}
