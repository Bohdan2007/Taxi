using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Taxi.Exceptions;

namespace Taxi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context); 
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var message = "Внутрішня помилка сервера.";
            
            if (exception is NotFoundException)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                message = exception.Message;
            }
            else if (exception is BadRequestException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                message = exception.Message;
            }
            
            var result = JsonSerializer.Serialize(new { error = message, status = context.Response.StatusCode });
            return context.Response.WriteAsync(result);
        }
    }
}