using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Api.Application.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly string _key;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expiresMinutes;

    public JwtTokenService(string key, string issuer, string audience, int expiresMinutes)
    {
        _key = key;
        _issuer = issuer;
        _audience = audience;
        _expiresMinutes = expiresMinutes;
    }

    // Generate our JWT tokens for internal use
    public string GenerateToken(IDictionary<string, string>? claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler(); // Create a token handler to generate JWT tokens
        var key = Encoding.UTF8.GetBytes(_key); // Convert our secret key to byte array, this is the key we use to sign our tokens

        // The list of claims we want to include in the token payload
        // That is what data can be accessed from the token
        var claimList = new List<Claim>();

        // All specified claims go here (name, pfp, &c)
        if (claims != null)
            foreach (var kvp in claims)
                claimList.Add(new Claim(kvp.Key, kvp.Value));

        // Define the token descriptor, which includes the claims, expiration, issuer, audience, and signing credentials
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claimList),
            Expires = DateTime.UtcNow.AddMinutes(_expiresMinutes),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        // Build, sign, and return the token using the token handler
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}