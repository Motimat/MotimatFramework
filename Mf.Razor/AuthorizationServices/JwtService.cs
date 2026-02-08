using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Mf.Razor
{
    public static class JwtService
    {
        //static string securityAlgorithm = SecurityAlgorithms.HmacSha512Signature;
        public static string CreateJwtToken(string securityKey, List<Claim> claims, TimeSpan? expireDate = null)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = new DateTime();
            if(expireDate == null)
            {
                expires = DateTime.Now.AddDays(1);
            }
            else
            {
                expires = DateTime.Now.AddMinutes(expireDate.Value.TotalMinutes);
            }
            
            var token = new JwtSecurityToken(
                "daliranpv.ir",
                "your_audience",
                claims,
                expires: expires,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);


            //var tokenHandler = new JwtSecurityTokenHandler();
            //var key = Encoding.ASCII.GetBytes(Tokens.AuthenticaitonSystemOne.Key);
            //var tokenDescriptor = new SecurityTokenDescriptor
            //{
            //    Subject = new ClaimsIdentity(claims),
            //    Expires = DateTime.UtcNow.AddDays(7),
            //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            //};
            //var token = tokenHandler.CreateToken(tokenDescriptor);
            //return tokenHandler.WriteToken(token);



        }
    }
}
