namespace Api.Application.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Api.Application.Services;
using Api.Application.Services.Auth.Providers;
using Api.Domain.Entities;
using Api.Application.DataTransfer;
using System.Text.Json;
using Api.Application.Exceptions;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IEnumerable<IAuthProvider> _authProviders;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ISystemUserService _systemUserService;
    private readonly IConfiguration _config;

    public AuthController(
        IEnumerable<IAuthProvider> authProviders,
        IJwtTokenService jwtTokenService,
        ISystemUserService systemUserService,
        IConfiguration config)
    {
        _authProviders = authProviders;
        _jwtTokenService = jwtTokenService;
        _systemUserService = systemUserService;
        _config = config;
    }

    [HttpPost("login/{provider}")]
    public async Task<IActionResult> Login(string provider, [FromBody] dynamic request)
    {
        var authProvider = _authProviders.FirstOrDefault(p => p.Name.Equals(provider, StringComparison.OrdinalIgnoreCase));
        if (authProvider == null)
            return BadRequest($"Unknown provider: {provider}");

        string? token = null;

        try
        {
            if (request is JsonElement elem && elem.TryGetProperty("token", out var tokenProp) && tokenProp.ValueKind == JsonValueKind.String)
            {
                token = tokenProp.GetString();
            }
            else if (request?.Token != null)
            {
                token = request.Token;
            }
        }
        catch { }
        if (string.IsNullOrEmpty(token))
            return BadRequest("ID Token is required.");

        var result = await authProvider.ValidateTokenAsync(token);
        if (!result.IsValid)
            return Unauthorized($"Invalid {provider} token: {result.Error}");


        SystemUserDto? user = null;
        try
        {
            user = await _systemUserService.GetBySub(result.ExternalId ?? string.Empty);
            if (user.IsActive != null && !user.IsActive.Value)
                return Unauthorized("User inactive.");
        }
        catch (EntityNotFoundException)
        {
            return Unauthorized("User not found.");
        }

        string roleName = "";
        if (user.Role.HasValue && Enum.IsDefined(typeof(SystemUserRoleType), user.Role.Value))
            roleName = ((SystemUserRoleType)user.Role.Value).ToString();

        var jwt = _jwtTokenService.GenerateToken(new Dictionary<string, string>
        {
            { "id", result.ExternalId! },
            { "email_address", result.Email ?? "" },
            { "name", result.Name ?? "" },
            { "picture", result.Picture ?? "" },
            { "user_role", roleName }
        });

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(_config.GetValue<int>("Jwt:ExpiresMinutes"))
        };
        Response.Cookies.Append("AuthToken", jwt, cookieOptions);

        return Ok(new
        {
            Token = jwt,
            expiresIn = _config.GetValue<int>("Jwt:ExpiresMinutes"),
            user = new { id = result.ExternalId, email = result.Email, name = result.Name, picture = result.Picture, role = user.Role }
        });
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
        var roleNumerical = Enum.Parse<SystemUserRoleType>(User.FindFirst("user_role")?.Value ?? "Administrator");

        var sub = User.FindFirst("id")?.Value;
        var email = User.FindFirst("email_address")?.Value;
        var name = User.FindFirst("name")?.Value;
        var picture = User.FindFirst("picture")?.Value;
        var role = roleNumerical;
        return Ok(new { sub, email, name, picture, role });
    }
}
