
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using MotimatFrameWork.Razor.Entity.Interfaces;
using MotimatFrameWork.Razor.Services;

namespace BlazorWasm
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _http;
        private readonly LogService _logger;
        private string _tokenKey;
        public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient http,LogService logger)
        {
            _logger = logger;
            _localStorage = localStorage;
            _http = http;
        }

        public async Task SetToken(string tokenKey)
        {
            _tokenKey = tokenKey;
        }
        public async Task<bool> RemoveToke()
        {
            try
            {
                await _localStorage.RemoveItemAsync("token");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string token = await _localStorage.GetItemAsStringAsync("token");



            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_tokenKey);

            try
            {
                
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
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
