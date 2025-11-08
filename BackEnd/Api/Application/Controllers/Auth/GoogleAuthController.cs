using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using Google.Apis.Auth;
using Api.Application.Services;
using Api.Domain.Entities;
using Api.Application.DataTransfer;

namespace GoogleAuth.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IConfiguration _config;
    private readonly ISystemUserService _systemUserService;

    public LoginController(ISystemUserService systemUserService, IConfiguration config)
    {
        _jwtTokenService = new JwtTokenService(
            config["Jwt:Key"]!,
            config["backend_url"]!,
            config["frontend_url"]!,
            config.GetValue<int>("Jwt:ExpiresMinutes")
        );
        _config = config;
        _systemUserService = systemUserService;
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleAuthRequest request)
    {
        if (string.IsNullOrEmpty(request.Token))
            return BadRequest("ID Token is required.");

        try
        {
            // Validate the token with Google's libraries
            // We call google's oauth library to verify and decode the ID token
            // If valid we get the decoded payload with user info from google servers
            var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token, new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new[] { _config["Authentication:Google:ClientId"] }
            });

            // payload contains info such as Email, Sub (Google user id), Name, Picture, etc.
            var googleUserId = payload.Subject;
            var email = payload.Email ?? "unknown";

            SystemUserDto? user = null;
            try
            {
                user = await _systemUserService.GetBySub(googleUserId);
                if (user == null || user.IsActive != null && !user.IsActive.Value)
                {
                    // treat as unauthorized (user missing or not active)
                    return Unauthorized("User not found or inactive.");
                }
            }
            catch (Api.Application.Exceptions.EntityNotFoundException)
            {
                // If the user is not present in the system, return Unauthorized instead of bubbling as 500
                return Unauthorized("User not found or inactive.");
            }

            // Create our own JWT token for the user
            // We need to create this because the google token was issue by google and we cant accept any token not issued by us
            // (or any app with google signin could generate valid tokens for out backend), so we have to issue it ourselfs
            // sending as payload whatever we need from the google api
            var token = _jwtTokenService.GenerateToken(new Dictionary<string, string>
            {
                { "id", googleUserId},
                { "email_address", email },
                { "name", payload.Name ?? "" },
                { "picture", payload.Picture ?? "" },
                { "user_role", user.Role.ToString() ?? "" }
            });

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(_config.GetValue<int>("Jwt:ExpiresMinutes"))
            };

            Response.Cookies.Append("AuthToken", token, cookieOptions);

            return Ok(new
            {
                Token = token, // This is the token that will have tobe used as a bearer in the future
                expiresIn = _config.GetValue<int>("Jwt:ExpiresMinutes"),
                user = new { id = googleUserId, email = email, name = payload.Name, picture = payload.Picture, role = user.Role }
            });
        }
        catch (InvalidJwtException ex)
        {
            return Unauthorized("Invalid Google token: " + ex.Message);
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(-1)
        };
        Response.Cookies.Delete("AuthToken", cookieOptions);
        return NoContent();
    }

    [Authorize(Policy = "ApiUser")]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var sub = User.FindFirst("id")?.Value;
        var email = User.FindFirst("email_address")?.Value;
        var name = User.FindFirst("name")?.Value;
        var picture = User.FindFirst("picture")?.Value;
        var role = User.FindFirst("user_role")?.Value;

        Console.WriteLine($"Me called for user {sub} - {email}");
        Console.WriteLine($"Name: {name}, Picture: {picture}, Role: {role}");

        return Ok(new { sub, email, name, picture, role });
    }
}