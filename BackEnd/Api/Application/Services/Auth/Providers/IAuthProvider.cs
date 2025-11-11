namespace Api.Application.Services.Auth.Providers;

public interface IAuthProvider
{
	string Name { get; }
	Task<AuthProviderResult> ValidateTokenAsync(string token);
}

public class AuthProviderResult
{
	public bool IsValid { get; set; }
	public string? ExternalId { get; set; }
	public string? Email { get; set; }
	public string? Name { get; set; }
	public string? Picture { get; set; }
	public string? Error { get; set; }
}