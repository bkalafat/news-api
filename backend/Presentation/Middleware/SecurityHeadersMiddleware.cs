using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace NewsApi.Presentation.Middleware;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Add security headers to protect against OWASP Top 10 vulnerabilities
        context.Response.Headers.Remove("Server");
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
        context.Response.Headers.Append(
            "Content-Security-Policy",
            "default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:; font-src 'self' data:; connect-src 'self'"
        );

        // Prevent sensitive information disclosure
        context.Response.Headers.Append("X-Permitted-Cross-Domain-Policies", "none");
        context.Response.Headers.Append("Cross-Origin-Embedder-Policy", "require-corp");
        context.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-origin");
        context.Response.Headers.Append("Cross-Origin-Resource-Policy", "same-origin");

        await _next(context);
    }
}
