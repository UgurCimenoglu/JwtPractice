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

        private readonly Dictionary<string, string> users = new Dictionary<string, string>
        {
            {"ugur","1234" },
            {"ahmet","12345" }
        };


        public string Authentication(string userName, string password)
        {
            if (!users.Any(u => u.Key == userName && u.Value == password))
            {
                return null;
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(Configuration["TokenOptions:SecurityKey"]);
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
            var a = Configuration["TokenOptions:AccessTokenExpiration"];
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}