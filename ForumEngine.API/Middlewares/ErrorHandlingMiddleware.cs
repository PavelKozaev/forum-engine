using FluentValidation;
using ForumEngine.Domain.Authorization;
using ForumEngine.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ForumEngine.API.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext httpContext,
            ProblemDetailsFactory problemDetailsFactory)
        {
            try
            {
                await _next.Invoke(httpContext);
            }
            catch (Exception exception)
            {
                var problemDetails = exception switch
                {
                    IntentionManagerException intentionManagerException => problemDetailsFactory.CreateFrom(httpContext, intentionManagerException),
                    ValidationException validationException => problemDetailsFactory.CreateFrom(httpContext, validationException),
                    DomainException domainException => problemDetailsFactory.CreateFrom(httpContext, domainException),
                    _ => problemDetailsFactory.CreateProblemDetails(httpContext, StatusCodes.Status500InternalServerError,
                        "Unhandled error: Please contact us.")
                };

                httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
                await httpContext.Response.WriteAsJsonAsync(problemDetails, problemDetails.GetType());
            }
        }
    }
}