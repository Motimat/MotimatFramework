
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Mf.Razor.Entity.Interfaces;
using Mf.Razor.Services;

namespace Mf.Razor
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        
        private readonly ITokenService _tokenService;
        private readonly LogService _logger;
        public CustomAuthStateProvider(ITokenService tokenService,LogService logger)
        {
            _logger = logger;
           _tokenService = tokenService;
        }

        public async Task SetToken(string tokenKey)
        {
           await _tokenService.SetToken(tokenKey);
        }
        public async Task<bool> RemoveToke()
        {
            try
            {
               await _tokenService.RemoveToke();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string token = await _tokenService.GetToken();

            if (String.IsNullOrEmpty(token))
            {
                var state = new AuthenticationState(new ClaimsPrincipal());
                NotifyAuthenticationStateChanged(Task.FromResult(state));
                _logger.Log("Auth Is False");
                return state;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
           

            try
            {
                
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    // Set clock skew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later by default)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                // Create ClaimsPrincipal from the token's claims
                var claimsIdentity = new ClaimsIdentity(jwtToken.Claims, "jwt");
                var user = new ClaimsPrincipal(claimsIdentity);
                var state = new AuthenticationState(user);
                NotifyAuthenticationStateChanged(Task.FromResult(state));
                _logger.Log("Auth is ON");
                return state;
            }
            catch (Exception)
            {
                
                // Token validation failed
                var state = new AuthenticationState(new ClaimsPrincipal());
                NotifyAuthenticationStateChanged(Task.FromResult(state));
                _logger.Log("Auth Is False");
                return state;
            }
        }

        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
