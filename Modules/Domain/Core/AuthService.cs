using Domain.Entities;
using Infra.CrossCutting.Auth;
using Infra.CrossCutting.Auth.Intefaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Domain.Core
{
    public class AuthService : IAuthService<User>
    {
        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(AuthSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(AuthRules.UserId, user.Id.ToString()),
                    new Claim(AuthRules.UserEmail, user.Email.ToString()),
                    new Claim(AuthRules.UserName, user.Name.ToString()),
                    new Claim(AuthRules.UserRole, user.IsAdmin.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
