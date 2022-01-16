using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JwtPractice.JWT
{
    public class JwtAuthenticationManager : IJwtAuthenticationService
    {

        public IConfiguration Configuration;

        public JwtAuthenticationManager(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        //Jwt'yi test etmek için key,value tutan bir dictionary oluşturdum.
        private readonly Dictionary<string, string> users = new Dictionary<string, string>
        {
            {"ugur","1234" },
            {"ahmet","12345" }
        };


        public string Authentication(string userName, string password)
        {
            //Ön yüzden girilen kllanıcı adı ve şifre ile dictionary yapısındakiler kontrol ediliyor.
            if (!users.Any(u => u.Key == userName && u.Value == password))
            {
                return null;
            }
            //TokenHandler oluşturdum, nesne bize jwt'yi okuma ve yazmamıza olanak sağlıyor.
            var tokenHandler = new JwtSecurityTokenHandler();

            //TokenKey'imizin byte array şeklineçeviriyoruz çünkü SymmetricticSecurityKey oluştururken bizden byte array istiyor.
            var tokenKey = Encoding.ASCII.GetBytes(Configuration["TokenOptions:SecurityKey"]);

            //Tokenin Descriptor bölümünü oluşturuyorum.Bu kısım tokenin payload kısmı.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userName)
                }),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(Configuration["TokenOptions:AccessTokenExpiration"])),
                NotBefore = DateTime.UtcNow,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha512),
                Issuer = Configuration["TokenOptions:Issuer"],
                Audience = Configuration["TokenOptions:Audience"],

            };
            
            //Token oluşturup return ediyorum.
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}