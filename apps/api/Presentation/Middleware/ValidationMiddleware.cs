using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace NewsApi.Presentation.Middleware;

public class ValidationMiddleware
{
    private readonly RequestDelegate _next;

    public ValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context).ConfigureAwait(false);
        }
        catch (ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex).ConfigureAwait(false);
        }
    }

    private static async Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";
        response.StatusCode = (int)HttpStatusCode.BadRequest;

        var errors = exception
            .Errors.GroupBy(error => error.PropertyName)
            .ToDictionary(errorGroup => errorGroup.Key, errorGroup => errorGroup.Select(error => error.ErrorMessage).ToArray());

        var validationResponse = new ValidationErrorResponse { Message = "Validation failed", Errors = errors };

        var jsonResponse = JsonSerializer.Serialize(
            validationResponse,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );

        await response.WriteAsync(jsonResponse).ConfigureAwait(false);
    }
}

public class ValidationErrorResponse
{
    public string Message { get; set; } = string.Empty;

    public Dictionary<string, string[]> Errors { get; set; } = new();
}
