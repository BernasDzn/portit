using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using Google.Apis.Auth;
using Api.Application.Services;

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
            config["Jwt:Issuer"]!,
            config["Jwt:Audience"]!,
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

            var result = await _systemUserService.GetBySub(googleUserId);
            if (result == null || !result.IsActive)
            {
                throw new UnauthorizedAccessException("User not found or inactive.");
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
            });

            return Ok(new
            {
                Token = token, // This is the token that will have tobe used as a bearer in the future
                expiresIn = _config.GetValue<int>("Jwt:ExpiresMinutes"),
                user = new { id = googleUserId, email = email, name = payload.Name, picture = payload.Picture }
            });
        }
        catch (InvalidJwtException ex)
        {
            return Unauthorized("Invalid Google token: " + ex.Message);
        }
    }

    [Authorize(Policy = "ApiUser")]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var sub = User.FindFirst("id")?.Value;
        var email = User.FindFirst("email_address")?.Value;
        var name = User.FindFirst("name")?.Value;
        var picture = User.FindFirst("picture")?.Value;

        return Ok(new { sub, email, name, picture });
    }
}