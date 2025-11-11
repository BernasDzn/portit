using Google.Apis.Auth;

namespace Api.Application.Services.Auth.Providers
{
    public class GoogleAuthProvider : IAuthProvider
    {
        private readonly IConfiguration _config;
        public string Name => "google";

        public GoogleAuthProvider(IConfiguration config)
        {
            _config = config;
        }

        public async Task<AuthProviderResult> ValidateTokenAsync(string token)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(token, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _config["Authentication:Google:ClientId"] }
                });
                return new AuthProviderResult
                {
                    IsValid = true,
                    ExternalId = payload.Subject,
                    Email = payload.Email,
                    Name = payload.Name,
                    Picture = payload.Picture
                };
            }
            catch (InvalidJwtException ex)
            {
                return new AuthProviderResult { IsValid = false, Error = ex.Message };
            }
        }
    }
}
