using MCC_Launcher.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static DevExpress.Data.Helpers.FindSearchRichParser;

namespace MCC_Launcher.Services
{
    public class JwtService
    {

        private static readonly string SecretKey = "ThisIsAVerySecureSecretKeyThatIsLongEnough123!"; // 비밀키 다른걸로 바꾸기
        private readonly SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));

        private static readonly string Issuer = "MCCLauncher"; // 발급자

        //사용 x
        //public static string GenerateToken(string userId, string role, int expireMinutes = 60)
        //{
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey)); //서명키
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);// 서명 알고리즘

        //    var claims = new[]
        //    {
        //    new Claim(ClaimTypes.NameIdentifier, userId),//아이디
        //    new Claim(ClaimTypes.Role, role),//역할
        //    //new Claim(ClaimTypes.P, userId)//이름
        //};
        //    //토큰 생성
        //    var token = new JwtSecurityToken(
        //        issuer: Issuer,
        //        audience: Issuer,
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddMinutes(expireMinutes),
        //        signingCredentials: creds
        //    );

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //    //토큰 문자열로 반환 
        //}


        public string GenerateToken(UserInfo user, string OptionsFilePath)
        // 
        {
            var role = user.UserRoles.FirstOrDefault()?.Role.RoleName ?? "Guest";

            var claims = new List<Claim>
            {
           new Claim("UserId", user.UserId),
           //new Claim(JwtRegisteredClaimNames.Sub, user.UserId),
            new Claim(ClaimTypes.Role, role),
            new Claim("activated", user.Activated.ToString().ToLower()) // true/false 저장
            // activated 는 다른걸로 수저오디어야함 사용기간 만료인지 아닌지로 activated로 하면 막힐수 없음 
        };
            var options = LoadOptionElementsFromXml(OptionsFilePath);
            foreach (var option in options)
            {
                claims.Add(new Claim($"option:{option.Key}", option.Value));
            }
            //var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(
                issuer: "MCC_Launcher",
                audience: "MCC_Launcher",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
        public Dictionary<string, string> LoadOptionElementsFromXml(string filePath)
        { // xml 변환 
            var doc = XDocument.Load(filePath);
            var optionElement = doc.Element("Option");//루트 
            var dict = new Dictionary<string, string>();

            if (optionElement != null)
            {
                foreach (var child in optionElement.Elements())
                {
                    var name = child.Name.LocalName;
                    var value = child.Value;
                    dict[name] = value;
                }
            }

            return dict;
        }
        public static ClaimsPrincipal? ValidateToken(string token)//토큰 검증함수 
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
