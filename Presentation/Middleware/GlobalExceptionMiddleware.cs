using HackDemoAPI.Application.Exceptions;
using HackDemoAPI.Common;
using System.ComponentModel.DataAnnotations;
using ValidationException = HackDemoAPI.Application.Exceptions.ValidationException;

namespace HackDemoAPI.Presentation.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation Error: {Message}", ex.Message);

                await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, ex.Message);

                await HandleExceptionAsync(context, StatusCodes.Status404NotFound, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Unhandled Exception: {Message}",ex.Message);

                await HandleExceptionAsync(context, StatusCodes.Status500InternalServerError, ValidationConstant.InternalServerError);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, int statusCode, string message)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = statusCode;

            var response = new
            {
                Success = false,
                Message = message
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
