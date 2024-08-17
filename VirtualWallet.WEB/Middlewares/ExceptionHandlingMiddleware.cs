using System.Net;
using System.Security.Authentication;
using System.Text.Json;
using ForumProject.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ForumProject.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode status;
            string message;

            switch (exception)
            {
                case NullReferenceException e:
                    status = HttpStatusCode.NotFound;
                    message = e.Message;
                    break;
                case EntityNotFoundException e:
                    status = HttpStatusCode.NotFound;
                    message = e.Message;
                    break;
                case InvalidCredentialException e:
                    status = HttpStatusCode.Unauthorized;
                    message = e.Message;
                    break;
                case UnauthorizedAccessException e:
                    status = HttpStatusCode.Forbidden;
                    message = e.Message;
                    break;
                case DuplicateEntityException e:
                    status = HttpStatusCode.Conflict;
                    message = e.Message;
                    break;
                case BadRequestException e:
                    status = HttpStatusCode.BadRequest;
                    message = e.Message;
                    break;
                case Exception e:
                    status = HttpStatusCode.InternalServerError;
                    message = e.Message;
                    break;
                default:
                    status = HttpStatusCode.InternalServerError;
                    message = "An unexpected error occurred.";
                    break;
            }

            if (context.Request.Path.StartsWithSegments("/api"))
            {
                var result = JsonSerializer.Serialize(new { error = message });
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)status;
                return context.Response.WriteAsync(result);
            }
            else
            {
                if (context.Request.Path.StartsWithSegments("/Error"))
                {
                    context.Response.StatusCode = (int)status;
                    return context.Response.WriteAsync(message);
                }
                else
                {
                    context.Response.StatusCode = (int) status;
                    context.Response.Redirect($"/Error?statusCode={(int)status}&message={Uri.EscapeDataString(message)}");
                    return Task.CompletedTask;
                }
            }
        }
    }
}
