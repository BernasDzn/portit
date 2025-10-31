
namespace Api.Application.Services;

public interface IJwtTokenService
{
    string GenerateToken(IDictionary<string, string>? claims = null);
}
