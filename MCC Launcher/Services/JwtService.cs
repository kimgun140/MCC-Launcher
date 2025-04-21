using MCC_Launcher.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static DevExpress.Data.Helpers.FindSearchRichParser;

namespace MCC_Launcher.Services
{
    public class JwtService
    {

        private static readonly string SecretKey = "ThisIsAVerySecureSecretKeyThatIsLongEnough123!"; // 비밀키 다른걸로 바꾸기
        private readonly SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));

        private static readonly string Issuer = "MCCLauncher"; // 발급자

        public static string GenerateToken(string userId, string role, int expireMinutes = 60)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey)); //서명키
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);// 서명 알고리즘

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId),//아이디
            new Claim(ClaimTypes.Role, role),//역할
            //new Claim(ClaimTypes.P, userId)//이름
        };
            //토큰 생성
            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
            //토큰 문자열로 반환 
        }


        public string GenerateToken(UserInfo user)
        {
            var role = user.UserRoles.FirstOrDefault()?.Role.RoleName ?? "Guest";

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId),
            new Claim(ClaimTypes.Role, role),
            new Claim("activated", user.Activated.ToString().ToLower()) // true/false 저장
        };

            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "MCC_Launcher",
                audience: "MCC_Launcher",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static ClaimsPrincipal? ValidateToken(string token)//검증
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(SecretKey);

            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Issuer,
                ValidateAudience = true,
                ValidAudience = Issuer,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            try
            {
                return tokenHandler.ValidateToken(token, parameters, out _);
            }
            catch
            {
                return null;
            }
        }
    }
}
