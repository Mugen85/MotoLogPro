using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace MotoLogPro.API.Middleware
{
    public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Eccezione non gestita: {Message}", ex.Message);
                await WriteProblemDetailsAsync(context, ex);
            }
        }

        private static Task WriteProblemDetailsAsync(HttpContext context, Exception ex)
        {
            var (statusCode, title) = ex switch
            {
                DbUpdateException { InnerException.Message: var msg }
                    when msg.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase)
                    => (HttpStatusCode.Conflict, "Violazione di unicità: il record esiste già."),

                DbUpdateException
                    => (HttpStatusCode.Conflict, "Errore durante il salvataggio dei dati."),

                UnauthorizedAccessException
                    => (HttpStatusCode.Unauthorized, "Accesso non autorizzato."),

                KeyNotFoundException
                    => (HttpStatusCode.NotFound, "Risorsa non trovata."),

                _ => (HttpStatusCode.InternalServerError, "Si è verificato un errore interno.")
            };

            var problem = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = title,
                Instance = context.Request.Path
            };

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/problem+json";

            return context.Response.WriteAsync(JsonSerializer.Serialize(problem, _jsonOptions));
        }
    }
}