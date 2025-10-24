using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewsApi.Infrastructure.Security;

namespace NewsApi.Presentation.Controllers;

/// <summary>
/// Authentication controller for admin panel login
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _jwtTokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(JwtTokenService jwtTokenService, ILogger<AuthController> logger)
    {
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    /// <summary>
    /// Login endpoint for admin users
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>JWT token and user info</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        try
        {
            // Simple authentication - replace with real user validation
            // For demo purposes, hardcoded admin credentials
            if (request.Username == "admin" && request.Password == "admin123")
            {
                var token = _jwtTokenService.GenerateToken(
                    userId: "1",
                    username: request.Username,
                    expirationMinutes: 60
                );

                var response = new LoginResponse
                {
                    Token = token,
                    UserId = "1",
                    Username = request.Username,
                    ExpiresIn = 3600,
                };

                _logger.LogInformation("User {Username} logged in successfully", request.Username);
                return Ok(response);
            }

            _logger.LogWarning("Failed login attempt for username: {Username}", request.Username);
            return Unauthorized(new { message = "Geçersiz kullanıcı adı veya şifre" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, new { message = "Giriş işlemi sırasında bir hata oluştu" });
        }
    }

    /// <summary>
    /// Validate token endpoint
    /// </summary>
    /// <returns>Current user info if token is valid</returns>
    [HttpGet("validate")]
    [ProducesResponseType(typeof(ValidateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult ValidateToken()
    {
        try
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new { message = "Token bulunamadı" });
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var principal = _jwtTokenService.ValidateToken(token);

            if (principal == null)
            {
                return Unauthorized(new { message = "Geçersiz token" });
            }

            var userId = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var username = principal.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

            var response = new ValidateResponse
            {
                UserId = userId ?? "",
                Username = username ?? "",
                IsValid = true,
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            return Unauthorized(new { message = "Token doğrulama hatası" });
        }
    }
}

/// <summary>
/// Login request model
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Username
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Password
    /// </summary>
    public required string Password { get; set; }
}

/// <summary>
/// Login response model
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// JWT token
    /// </summary>
    public required string Token { get; set; }

    /// <summary>
    /// User ID
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// Username
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Token expiration time in seconds
    /// </summary>
    public int ExpiresIn { get; set; }
}

/// <summary>
/// Token validation response model
/// </summary>
public class ValidateResponse
{
    /// <summary>
    /// User ID
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// Username
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Whether token is valid
    /// </summary>
    public bool IsValid { get; set; }
}
